using MIN.Chess.Logic.Enums;
using MIN.Chess.Logic.Extensions;
using MIN.Chess.Messages.Messages;
using MIN.Chess.Server.Helpers;
using MIN.Ipc.Services.Contracts.Interfaces;

namespace MIN.Chess.Server.Services;

/// <summary>
/// Сервис по отслежке состояния голосования
/// </summary>
internal class VoteService
{
    private readonly RoomService roomService;
    private readonly MessageService messageService;
    private readonly ILoggerProvider logger;
    private readonly Dictionary<Player, bool> restartVote = new()
    {
        { Player.White, false },
        { Player.Black, false },
    };
    private readonly Dictionary<Player, bool> drawVote = new()
    {
        { Player.White, false },
        { Player.Black, false },
    };
    private readonly Dictionary<Player, bool> switchSideVote = new()
    {
        { Player.White, false },
        { Player.Black, false },
    };

    /// <summary>
    /// Событие по сдаче игрока
    /// </summary>
    public Action<Player>? OnResign;

    /// <summary>
    /// Событие по наборе голосов на ничью
    /// </summary>
    public Action? OnDraw;

    /// <summary>
    /// Событие по наборе голосов на рестарт
    /// </summary>
    public Action? OnRestart;

    /// <summary>
    /// Событие по наборе голосов на смену сторон
    /// </summary>
    public Action? OnSwitchSide;

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="VoteService"/>
    /// </summary>
    public VoteService(RoomService roomService,
        MessageService messageService,
        ILoggerProvider logger)
    {
        this.roomService = roomService;
        this.messageService = messageService;
        this.logger = logger;
    }

    /// <summary>
    /// Обработать сдачу
    /// </summary>
    public async Task HandleResign(INetworkMessage message)
    {
        var senderId = Guid.Parse(message.SenderId);
        var role = roomService.ResolveRole(senderId);

        logger.Log($"Resign received from {role}");

        if (role == Player.None)
        {
            await messageService.SendErrorAsync(senderId, "У вас нету права на это");
            return;
        }

        OnResign?.Invoke(role.Opponent());
    }

    /// <summary>
    /// Обработать ничью
    /// </summary>
    public async Task HandleDraw(INetworkMessage message)
    {
        var senderId = Guid.Parse(message.SenderId);
        var role = roomService.ResolveRole(senderId);

        logger.Log($"Draw request received from {role}");

        if (role == Player.None)
        {
            await messageService.SendErrorAsync(senderId, "У вас нету права на это");
            return;
        }

        if (drawVote[role])
        {
            return;
        }

        if (!roomService.IsBothPlayersJoined())
        {
        }
        else
        {
            var votes = 1;
            drawVote[role] = true;
            if (drawVote[Player.White] && drawVote[Player.Black])
            {
                votes = 0;
                drawVote[Player.White] = false;
                drawVote[Player.Black] = false;

                OnDraw?.Invoke();
            }

            await messageService.BroadcastAsync(new DrawVoteMessage()
            {
                CurrentVotes = votes
            });
        }
    }

    /// <summary>
    /// Обработать рестарт
    /// </summary>
    public async Task HandleRestart(INetworkMessage message)
    {
        var senderId = Guid.Parse(message.SenderId);
        var role = roomService.ResolveRole(senderId);

        logger.Log($"Restart received from {role}");

        if (role == Player.None)
        {
            await messageService.SendErrorAsync(senderId, "У вас нету права на это");
            return;
        }

        if (restartVote[role])
        {
            return;
        }

        if (!roomService.IsBothPlayersJoined())
        {
            return;
        }

        if (roomService.GetPlayers()[role.Opponent()]?.IsAi == true)
        {
            OnRestart?.Invoke();
            return;
        }

        var votes = 1;
        restartVote[role] = true;
        if (restartVote[Player.White] && restartVote[Player.Black])
        {
            OnRestart?.Invoke();
            votes = 0;
            restartVote[Player.White] = false;
            restartVote[Player.Black] = false;
        }

        await messageService.BroadcastAsync(new RestartVoteMessage()
        {
            CurrentVotes = votes
        });
    }

    /// <summary>
    /// Обработать смену сторон
    /// </summary>
    public async Task HandleSwitchSide(INetworkMessage message)
    {
        var senderId = Guid.Parse(message.SenderId);
        var role = roomService.ResolveRole(senderId);

        logger.Log($"Restart received from {role}");

        if (role == Player.None)
        {
            await messageService.SendErrorAsync(senderId, "У вас нету права на это");
            return;
        }

        if (switchSideVote[role])
        {
            return;
        }

        if (!roomService.IsBothPlayersJoined())
        {
            return;
        }

        if (roomService.GetPlayers()[role.Opponent()]?.IsAi == true)
        {
            OnSwitchSide?.Invoke();
            return;
        }

        var votes = 1;
        switchSideVote[role] = true;
        if (switchSideVote[Player.White] && switchSideVote[Player.Black])
        {
            OnSwitchSide?.Invoke();
            votes = 0;
            switchSideVote[Player.White] = false;
            switchSideVote[Player.Black] = false;
        }

        await messageService.BroadcastAsync(new SwitchSidesVoteMessage()
        {
            CurrentVotes = votes
        });
    }
}
