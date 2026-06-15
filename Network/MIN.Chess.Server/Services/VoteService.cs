using MIN.Chess.Logic.Enums;
using MIN.Chess.Logic.Models;
using MIN.Chess.Messages.Messages;
using MIN.Chess.Server.Helpers;
using MIN.Ipc.Services.Contracts.Interfaces;

namespace MIN.Chess.Server.Services;

/// <summary>
/// Сервис по отслежке состояния голосования
/// </summary>
internal class VoteService
{
    private readonly ILoggerProvider logger;
    private readonly MessageService messageService;
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

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="VoteService"/>
    /// </summary>
    public VoteService(ILoggerProvider logger, MessageService messageService)
    {
        this.logger = logger;
        this.messageService = messageService;
    }

    /// <summary>
    /// Обработать сдачу
    /// </summary>
    public async Task HandleResign(INetworkMessage message)
    {
        var senderId = Guid.Parse(message.SenderId);
        var role = ResolveRole(senderId);

        logger.Log($"Resign received from {role}");

        if (role == Player.None)
        {
            await messageService.SendErrorAsync(senderId, "У вас нету права на это");
            return;
        }
        await SendGameResult(new Result(role.Opponent(), EndReason.Resign));

        await messageService.BroadcastExcludingAsync(new GameResultMessage()
        {
            Result = new Result(role.Opponent(), EndReason.Resign),
        });
    }

    /// <summary>
    /// Обработать ничью
    /// </summary>
    public async Task HandleDraw(INetworkMessage message)
    {
        var senderId = Guid.Parse(message.SenderId);
        var role = ResolveRole(senderId);

        logger.Log($"Draw request received from {role}");

        if (role == Player.None)
        {
            await messageService.SendErrorAsync(senderId, "У вас нету права на это");
            return;
        }

        if (drawVote[role] == true)
        {
            return;
        }

        if (!IsBothPlayersJoined())
        {
            return;
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
                await SendGameResult(Result.Draw(EndReason.Draw));
            }

            await messageService.BroadcastExcludingAsync(new DrawVoteMessage()
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
        var role = ResolveRole(senderId);

        logger.Log($"Restart received from {role}");

        if (role == Player.None)
        {
            await messageService.SendErrorAsync(senderId, "У вас нету права на это");
            return;
        }

        if (restartVote[role] == true)
        {
            return;
        }

        if (!IsBothPlayersJoined())
        {
            return;
        }
        else
        {
            var votes = 1;
            restartVote[role] = true;
            if (restartVote[Player.White] && restartVote[Player.Black])
            {
                await Restart();
                votes = 0;
                restartVote[Player.White] = false;
                restartVote[Player.Black] = false;
            }

            await bridge.SendMessage(new RestartVoteMessage()
            {
                CurrentVotes = votes
            }, cancellationToken: appCts.Token);
        }
    }

}
