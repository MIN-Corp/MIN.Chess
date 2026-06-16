using MIN.Chess.Logic.Enums;
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
    private readonly MessageService messageService;
    private readonly ILoggerProvider logger;
    private readonly Dictionary<Guid, Participant> spectators = [];

    private readonly static Dictionary<Guid, InitRequestMessage> pendingInits = [];

    private readonly Dictionary<Player, Participant?> players = new()
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
    /// Событие по получении запроса на init
    /// </summary>
    public Action<Participant>? OnInitRequestReceived;

    /// <summary>
    /// Событие по присоединении игрока с указанной ролью
    /// </summary>
    public Action<Player>? OnPlayerJoined;

    /// <summary>
    /// Событие по уходе игрока с указанной ролью
    /// </summary>
    public Action<Player>? OnPlayerLeft;

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="RoomService"/>
    /// </summary>
    public RoomService(MessageService messageService, ILoggerProvider logger)
    {
        this.messageService = messageService;
        this.logger = logger;
    }

    /// <summary>
    /// Выяснить роль участника
    /// </summary>
    public Player ResolveRole(Guid senderId)
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

    /// <summary>
    /// Получить список игроков
    /// </summary>
    public Dictionary<Player, Participant?> GetPlayers() => players;

    /// <summary>
    /// Получить количество смотрящих партию
    /// </summary>
    public int SpectatorCount => spectators.Count;

    /// <summary>
    /// Зашло ли оба игрока
    /// </summary>
    public bool IsBothPlayersJoined() => IsWhitePlayersJoined() && IsBlackPlayersJoined();

    /// <summary>
    /// Зашёл ли белый игрок
    /// </summary>
    public bool IsWhitePlayersJoined() => players[Player.White] != null;

    /// <summary>
    /// Зашёл ли чёрный игрок
    /// </summary>
    public bool IsBlackPlayersJoined() => players[Player.Black] != null;

    /// <summary>
    /// Обработать инициализацию игрока
    /// </summary>
    public async Task HandleInit(InitRequestMessage message)
    {
        logger.Log("InitRequestMessage received");

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
            OnInitRequestReceived?.Invoke(sender);
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

            OnPlayerJoined?.Invoke(Player.White);

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

            OnPlayerJoined?.Invoke(Player.Black);

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

            OnPlayerJoined?.Invoke(Player.None);

            await messageService.BroadcastExcludingAsync(new PlayerConnectedMessage()
            {
                Role = Player.None,
                Participant = participant,
            }, participant.Id);

            if (pendingInits.Remove(participant.Id, out _))
            {
                OnInitRequestReceived?.Invoke(participant);
            }

            return;
        }

        if (players[Player.White] == null)
        {
            logger.Log($"{participant.Name} joined as White");
            players[Player.White] = participant;

            OnPlayerJoined?.Invoke(Player.White);

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

            OnPlayerJoined?.Invoke(Player.Black);

            await messageService.BroadcastExcludingAsync(new PlayerConnectedMessage()
            {
                Role = Player.Black,
                Participant = participant,
            }, participant.Id);
        }

        if (pendingInits.Remove(participant.Id, out _))
        {
            OnInitRequestReceived?.Invoke(participant);
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

            OnPlayerLeft?.Invoke(Player.None);

            await messageService.BroadcastExcludingAsync(new PlayerDisconnectedMessage()
            {
                Role = Player.None,
            }, participantId);
            return;
        }

        if (players[Player.White]?.Id == participantId)
        {
            logger.Log($"White disconnected");
            players[Player.White] = null;

            OnPlayerLeft?.Invoke(Player.White);

            await messageService.BroadcastExcludingAsync(new PlayerDisconnectedMessage()
            {
                Role = Player.White,
            }, participantId);
        }
        else if (players[Player.Black]?.Id == participantId)
        {
            logger.Log($"Black disconnected");
            players[Player.Black] = null;

            OnPlayerLeft?.Invoke(Player.Black);

            await messageService.BroadcastExcludingAsync(new PlayerDisconnectedMessage()
            {
                Role = Player.Black,
            }, participantId);
        }
    }
}
