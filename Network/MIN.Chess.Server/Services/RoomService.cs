using MIN.Chess.Logic.Enums;
using MIN.Chess.Logic.Models;
using MIN.Chess.Messages.Messages;
using MIN.Chess.Server.Helpers;
using MIN.Ipc.Services.Contracts.Interfaces;
using MIN.Ipc.Services.Contracts.Models;

namespace MIN.Chess.Server.Services;

/// <summary>
/// Сервис по отслежке состояния участников и их ролей
/// </summary>
internal class RoomService
{
    private readonly ILoggerProvider logger;
    private readonly MessageService messageService;
    private readonly Dictionary<Guid, Participant> spectators = [];

    private readonly static Dictionary<Guid, InitRequestMessage> pendingInits = [];
    private readonly static Dictionary<Player, Participant?> players = new()
    {
        { Player.White, null },
        { Player.Black, null },
    };

    /// <summary>
    /// Получить список по умолчанию на исключение
    /// </summary>
    public static IEnumerable<Guid> GetBroadcastExcludeList(Guid? excludeId = null)
    {
        if (excludeId.HasValue)
        {
            return pendingInits.Keys.Append(excludeId.Value);
        }
        return pendingInits.Keys;
    }

    /// <summary>
    /// Выяснить роль участника
    /// </summary>
    public static Player ResolveRole(Guid senderId)
    {
        if (players[Player.White]?.Id == senderId || players[Player.White] == null)
        {
            return Player.White;
        }
        else if (players[Player.Black]?.Id == senderId || players[Player.Black] == null)
        {
            return Player.Black;
        }
        else
        {
            return Player.None;
        }
    }

    public static bool IsBothPlayersJoined()
        => players[Player.White] != null && players[Player.Black] != null;

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="RoomService"/>
    /// </summary>
    public RoomService(ILoggerProvider logger, MessageService messageService)
    {
        this.logger = logger;
        this.messageService = messageService;
    }

    /// <summary>
    /// Обработать инициализацию игрока
    /// </summary>
    public async Task HandleInit(InitRequestMessage message)
    {
        logger.Log("InitRequestMessage received");
        logger.Log($"sending init {gameState.ToString()}");

        var senderId = Guid.Parse(message.SenderId);
        var role = ResolveRole(senderId);

        logger.Log($"ROLE {role}");

        players.TryGetValue(role, out var sender);

        if (sender == null)
        {
            spectators.TryGetValue(senderId, out sender);
        }

        if (sender != null)
        {
            await SendInitResponse(sender);
            return;
        }

        pendingInits[senderId] = message;
    }

    /// <summary>
    /// Обработать смену роли
    /// </summary>
    /// <remarks>
    /// Пока только с наблюдателя в игроки
    /// </remarks>
    public async Task HandleChangeRole(INetworkMessage message)
    {
        var senderId = Guid.Parse(message.SenderId);
        var role = ResolveRole(senderId);

        logger.Log($"ChangeRole received from {role}");

        if (IsBothPlayersJoined())
        {
            await messageService.SendErrorAsync(senderId, "Не кого заменять");
            return;
        }

        spectators.TryGetValue(senderId, out var participant);

        if (participant == null)
        {
            logger.Log($"Didnt found spectator {senderId}");
            return;
        }

        if (players[Player.White] == null)
        {
            logger.Log($"{participant.Name} joined as White");
            players[Player.White] = participant;

            gameState.Board = players[Player.Black] == null
                ? Board.Initial(Player.White)
                : Board.Initial();

            if (players[Player.Black] != null)
            {
                gameState.CurrentPlayer = Player.White;
            }

            await messageService.BroadcastAsync(new PlayerConnectedMessage()
            {
                Role = Player.White,
                Participant = participant,
            });
        }
        else if (players[Player.Black] == null)
        {
            logger.Log($"{participant.Name} joined as Black");
            players[Player.Black] = participant;

            gameState.Board = Board.Initial();
            gameState.CurrentPlayer = Player.White;

            await messageService.BroadcastAsync(new PlayerConnectedMessage()
            {
                Role = Player.Black,
                Participant = participant,
            });
        }

        spectators.Remove(senderId);

        await messageService.BroadcastAsync(new PlayerDisconnectedMessage()
        {
            Role = Player.None,
        });

        if (IsBothPlayersJoined())
        {
            await SendStateUpdate();
        }
    }

    /// <summary>
    /// Обработать присоединения участника
    /// </summary>
    public async Task HandleParticipantJoining(Participant participant)
    {
        if (IsBothPlayersJoined())
        {
            logger.Log($"{participant.Id} joined as Spectator");
            spectators[participant.Id] = participant;

            await messageService.BroadcastExcludingAsync(new PlayerConnectedMessage()
            {
                Role = Player.None,
                Participant = participant,
            }, participant.Id);

            if (pendingInits.Remove(participant.Id, out _))
            {
                await SendInitResponse(participant);
            }

            return;
        }

        lastMove = null;

        if (players[Player.White] == null)
        {
            logger.Log($"{participant.Name} joined as White");
            players[Player.White] = participant;

            gameState.Board = players[Player.Black] == null
                ? Board.Initial(Player.White)
                : Board.Initial();

            if (players[Player.Black] != null)
            {
                gameState.CurrentPlayer = Player.White;
            }

            await messageService.BroadcastExcludingAsync(new PlayerConnectedMessage()
            {
                Role = Player.White,
                Participant = participant,
            }, participant.Id);
        }
        else if (players[Player.Black] == null)
        {
            logger.Log($"{participant.Name} joined as Black");
            players[Player.Black] = participant;

            gameState.Board = Board.Initial();
            gameState.CurrentPlayer = Player.White;

            await messageService.BroadcastExcludingAsync(new PlayerConnectedMessage()
            {
                Role = Player.Black,
                Participant = participant,
            }, participant.Id);
        }

        if (pendingInits.Remove(participant.Id, out _))
        {
            await SendInitResponse(participant);
        }

        if (IsBothPlayersJoined())
        {
            await SendStateUpdate(participant.Id);
        }
    }

    /// <summary>
    /// Обработать выход участника
    /// </summary>
    public async Task HandleParticipantDisconnecting(Guid participantId)
    {
        if (spectators.ContainsKey(participantId))
        {
            spectators.Remove(participantId);

            await messageService.BroadcastExcludingAsync(new PlayerDisconnectedMessage()
            {
                Role = Player.None,
            }, participantId);
            return;
        }

        StopTimer();
        gameState.CurrentPlayer = Player.None;

        if (players[Player.White]?.Id == participantId)
        {
            logger.Log($"White disconnected");
            players[Player.White] = null;

            await messageService.BroadcastExcludingAsync(new PlayerDisconnectedMessage()
            {
                Role = Player.White,
            }, participantId);
        }
        else if (players[Player.Black]?.Id == participantId)
        {
            logger.Log($"Black disconnected");
            players[Player.Black] = null;

            await messageService.BroadcastExcludingAsync(new PlayerDisconnectedMessage()
            {
                Role = Player.Black,
            }, participantId);
        }
    }
}
