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

        if (roomService.IsBothPlayersJoined())
        {
            await messageService.SendErrorAsync(senderId, "С тобой уже играет другой игрок");
            return;
        }

        var agent = agentFactory.GetAgent(agentType);
        var agentRole = roomService.ResolveRole(senderId).Opponent();
        var agentPlayer = new ChessPlayer(agent.Type.ToString())
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
    public async Task<Move?> HandleAgentMove(Guid senderId, GameState gameState)
    {
        if (agents.TryGetValue(gameState.CurrentPlayer, out var agent))
        {
            return await agent.GetMove(gameState);
        }

        await messageService.SendErrorAsync(senderId, "Агент не нашёлся");
        return null;
    }

    /// <summary>
    /// Получить идентификатор агента
    /// </summary>
    public Guid GetAgentId(Player role)
        => agentPlayers.TryGetValue(role, out var agentPlayer) ? agentPlayer.Id : Guid.Empty;
}
