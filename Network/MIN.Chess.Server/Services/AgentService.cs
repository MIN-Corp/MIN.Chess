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

    private readonly Dictionary<Player, IChessAgent> agents = new();
    private readonly Dictionary<Player, ChessPlayer> agentPlayers = new();

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

        agents.Remove(senderRole.Opponent());
        agentPlayers.Remove(senderRole.Opponent());
        await roomService.RemovePlayer(senderRole.Opponent());
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

        await roomService.RemovePlayer(senderRole);

        var agent = agentFactory.GetAgent(selfAgentType);
        var agentRole = senderRole;
        var agentPlayer = new ChessPlayer(agent.DisplayName)
        {
            IsAi = true,
            AgentType = agent.Type,
            Role = agentRole
        };

        agents[agentRole] = agent;
        agentPlayers[agentRole] = agentPlayer;
        await roomService.HandleParticipantJoining(agentPlayer);
    }

    /// <summary>
    /// Обработать ход агента
    /// </summary>
    public async Task<Move?> HandleAgentMove(GameState gameState)
    {
        if (agents.TryGetValue(gameState.CurrentPlayer, out var agent))
        {
            return await agent.GetMove(gameState);
        }

        Console.WriteLine("Агент не нашёлся");
        return null;
    }

    /// <summary>
    /// Получить идентификатор агента
    /// </summary>
    public Guid GetAgentId(Player role)
        => agentPlayers.TryGetValue(role, out var agentPlayer) ? agentPlayer.Id : Guid.Empty;

    private async Task<bool> HandleAgentChoosingAllowance(Guid senderId, Player senderRole)
    {
        if (senderRole == Player.None)
        {
            await messageService.SendErrorAsync(senderId, "Ты не игрок");
            return false;
        }

        if (roomService.IsBothPlayersJoined()
            && !roomService.GetPlayers()[senderRole.Opponent()]?.IsAi == true)
        {
            await messageService.SendErrorAsync(senderId, "С тобой уже играет другой игрок");
            return false;
        }

        return true;
    }
}
