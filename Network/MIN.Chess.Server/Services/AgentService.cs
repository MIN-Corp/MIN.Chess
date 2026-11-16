using MIN.Chess.Agents.Contracts;
using MIN.Chess.Agents.Enums;
using MIN.Chess.Logic.Enums;
using MIN.Chess.Logic.Extensions;
using MIN.Chess.Logic.Models;
using MIN.Chess.Messages.Contracts.Models;

namespace MIN.Chess.Server.Services;

internal class AgentService
{
    private readonly MessageService messageService;
    private readonly RoomService roomService;
    private readonly IChessAgentFactory agentFactory;
    private CancellationTokenSource agentCts = new();

    private readonly Dictionary<Player, IChessAgent?> agents = new()
    {
        { Player.White, null },
        { Player.Black, null },
    };

    private readonly Dictionary<Player, ChessPlayer?> agentPlayers = new()
    {
        { Player.White, null },
        { Player.Black, null },
    };

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="AgentService"/>
    /// </summary>
    public AgentService(MessageService messageService, RoomService roomService, IChessAgentFactory agentFactory)
    {
        this.messageService = messageService;
        this.roomService = roomService;
        this.agentFactory = agentFactory;
    }

    /// <summary>
    /// Сменить стороны агентов
    /// </summary>
    public void SwitchSides()
    {
        (agents[Player.White], agents[Player.Black]) = (agents[Player.Black], agents[Player.White]);
        (agentPlayers[Player.White], agentPlayers[Player.Black]) = (agentPlayers[Player.Black], agentPlayers[Player.White]);
    }

    /// <summary>
    /// Зашло ли оба игрока как ИИ
    /// </summary>
    public bool IsBothAgentPlayersJoined() => IsWhiteAiPlayerJoined() && IsBlackAiPlayerJoined();

    /// <summary>
    /// Зашёл ли белый игрок как ИИ
    /// </summary>
    public bool IsWhiteAiPlayerJoined() => agentPlayers[Player.White] != null;

    /// <summary>
    /// Зашёл ли чёрный игрок как ИИ
    /// </summary>
    public bool IsBlackAiPlayerJoined() => agentPlayers[Player.Black] != null;

    /// <summary>
    /// Отменить ход агента
    /// </summary>
    public void CancelAgent()
    {
        agentCts.Cancel();
        agentCts = new CancellationTokenSource();
    }

    /// <summary>
    /// Обработать загрузку игрока
    /// </summary>
    public async Task HandleAgentChoose(string senderIdString, AgentType agentType)
    {
        var senderId = Guid.Parse(senderIdString);
        var senderRole = roomService.ResolveRole(senderId);

        if (!(await HandleAgentChoosingAllowance(senderId, senderRole)))
        {
            return;
        }

        var agent = agentFactory.GetAgent(agentType);
        var agentRole = senderRole.Opponent();
        var agentPlayer = new ChessPlayer(agent.DisplayName)
        {
            IsAi = true,
            AgentType = agent.Type,
            Role = agentRole
        };

        agents[agentRole] = agent;
        agentPlayers[agentRole] = agentPlayer;
        CancelAgent();
        await roomService.HandleParticipantJoining(agentPlayer);
    }

    /// <summary>
    /// Обработать смену агента
    /// </summary>
    public async Task HandleAgentChange(string senderIdString, AgentType agentType)
    {
        var senderId = Guid.Parse(senderIdString);
        var senderRole = roomService.ResolveRole(senderId);

        if (!(await HandleAgentChoosingAllowance(senderId, senderRole)))
        {
            return;
        }

        var agent = agentFactory.GetAgent(agentType);
        var agentRole = roomService.ResolveRole(senderId).Opponent();
        var agentPlayer = new ChessPlayer(agent.DisplayName)
        {
            IsAi = true,
            AgentType = agent.Type,
            Role = agentRole
        };

        agents[agentRole] = agent;
        agentPlayers[agentRole] = agentPlayer;
        CancelAgent();
        await roomService.ReplacePlayer(senderRole.Opponent(), agentPlayer);
    }

    /// <summary>
    /// Обработать убирание агента
    /// </summary>
    public async Task HandleAgentRemove(string senderIdString)
    {
        var senderId = Guid.Parse(senderIdString);
        var senderRole = roomService.ResolveRole(senderId);

        if (!(await HandleAgentChoosingAllowance(senderId, senderRole)))
        {
            return;
        }

        var opponnentOrBlack = senderRole == Player.None ? Player.Black : senderRole.Opponent();

        agents[opponnentOrBlack] = null;
        agentPlayers[opponnentOrBlack] = null;
        CancelAgent();
        await roomService.RemovePlayer(opponnentOrBlack);
    }

    /// <summary>
    /// Обработать битву агентов
    /// </summary>
    public async Task HandleAgentVsAgent(string senderIdString, AgentType selfAgentType, AgentType opponentAgentType)
    {
        var senderId = Guid.Parse(senderIdString);
        var senderRole = roomService.ResolveRole(senderId);

        if (!(await HandleAgentChoosingAllowance(senderId, senderRole)))
        {
            return;
        }

        if (roomService.GetPlayers()[senderRole.Opponent()] == null)
        {
            await HandleAgentChoose(senderIdString, opponentAgentType);
        }
        else if (roomService.GetPlayers()[senderRole.Opponent()]?.IsAi == true)
        {
            await HandleAgentChange(senderIdString, opponentAgentType);
        }

        await roomService.RemovePlayer(senderRole, stayAsSpectator: true);

        var agent = agentFactory.GetAgent(selfAgentType);
        var agentPlayer = new ChessPlayer(agent.DisplayName)
        {
            IsAi = true,
            AgentType = agent.Type,
            Role = senderRole
        };

        agents[senderRole] = agent;
        agentPlayers[senderRole] = agentPlayer;
        CancelAgent();
        await roomService.HandleParticipantJoining(agentPlayer);
    }

    /// <summary>
    /// Обработать ход агента
    /// </summary>
    public async Task<Move?> HandleAgentMove(GameState gameState)
    {
        if (agents.TryGetValue(gameState.CurrentPlayer, out var agent) && agent != null)
        {
            try
            {
                return await agent.GetMove(gameState, agentCts.Token);
            }
            catch (OperationCanceledException)
            {
                return null;
            }
        }

        Console.WriteLine("Агент не нашёлся");
        return null;
    }

    /// <summary>
    /// Получить идентификатор агента
    /// </summary>
    public Guid GetAgentId(Player role)
        => agentPlayers.TryGetValue(role, out var agentPlayer) ? agentPlayer?.Id ?? Guid.Empty : Guid.Empty;

    private async Task<bool> HandleAgentChoosingAllowance(Guid senderId, Player senderRole)
    {
        if (senderRole == Player.None && (agentPlayers[Player.White] == null || agentPlayers[Player.Black] == null))
        {
            await messageService.SendErrorAsync(senderId, "Игроки уже играют с ИИ");
            return false;
        }

        var opponnentOrBlack = senderRole == Player.None ? Player.Black : senderRole.Opponent();

        if (!roomService.IsBothPlayersJoined()
            || !roomService.GetPlayers()[opponnentOrBlack]?.IsAi != true)
        {
            return true;
        }

        await messageService.SendErrorAsync(senderId, "С тобой уже играет другой игрок");
        return false;
    }
}
