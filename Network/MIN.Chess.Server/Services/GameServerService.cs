using MIN.Chess.Messages;
using MIN.Chess.Messages.Messages;
using MIN.Chess.Server.Helpers;
using MIN.Ipc.Services;
using MIN.Ipc.Services.Contracts.Interfaces;
using MIN.Ipc.Services.Contracts.Models;

namespace MIN.Chess.Server.Services;

/// <summary>
/// Оркестратор сервера, маршрутизация сообщений
/// </summary>
internal class GameServerService
{
    private readonly IClientProcessBridge bridge = new ClientProcessBridge();
    private readonly ILoggerProvider logger;
    private readonly MessageService messageService;
    private readonly VoteService voteService;
    private readonly GameService gameService;
    private readonly RoomService roomService;
    private readonly TimerService timerService;
    private readonly CancellationTokenSource appCts = new();
    private const int ConnectionTimeoutMs = 5_000;

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="GameServerService"/>
    /// </summary>
    public GameServerService(ILoggerProvider logger)
    {
        this.logger = logger;
        bridge.RegisterAllFromAnchor<IChessMessagesAnchor>();

        messageService = new(bridge, appCts);
        roomService = new(messageService, logger);
        voteService = new(roomService, messageService, logger);
        timerService = new(messageService, roomService, logger, appCts);
        gameService = new(messageService, roomService, voteService, timerService, logger);
    }

    /// <summary>
    /// Инициализировать сервис с подключением к MIN
    /// </summary>
    public async Task InitializeNetworking(string connectionString)
    {
        try
        {
            logger.Log("Connecting to MIN . . .");
            await bridge.ConnectAsync(connectionString, ConnectionTimeoutMs, appCts.Token);

            bridge.NetworkMessageReceived += NetworkMessageReceived;
            bridge.ParticipantConnected += ParticipantConnected;
            bridge.ParticipantDisconnected += ParticipantDisconnected;

            await bridge.StartListeningAsync(appCts.Token);

            logger.Log("Connection succeded, sending ready message");
            await bridge.SendReadyMessage(appCts.Token);
        }
        catch (Exception ex)
        {
            logger.Log($"Failed: {ex.Message}");
        }
    }

    private async void NetworkMessageReceived(object? sender, INetworkMessage message)
    {
        await HandleMessage(message);
    }

    private async void ParticipantConnected(object? sender, Participant participant)
    {
        await roomService.HandleParticipantJoining(participant);
    }

    private async void ParticipantDisconnected(object? sender, Guid participantId)
    {
        await roomService.HandleParticipantDisconnecting(participantId);
    }

    private async Task HandleMessage(INetworkMessage message)
    {
        switch (message)
        {
            case InitRequestMessage initRequestMessage:
                await roomService.HandleInit(initRequestMessage);
                break;

            case StateUpdateRequestMessage _:
                await gameService.HandleStateUpdate(message);
                break;

            case MoveRequestMessage moveRequestMessage:
                await gameService.HandleMove(moveRequestMessage);
                break;

            case ResignRequestMessage _:
                await voteService.HandleResign(message);
                break;

            case DrawRequestMessage _:
                await voteService.HandleDraw(message);
                break;

            case RestartRequestMessage _:
                await voteService.HandleRestart(message);
                break;

            case ChangeRoleRequestMessage _:
                await roomService.HandleChangeRole(message);
                break;

            case GameSettingsMessage gameSettingsMessage:
                await timerService.HandleGameSettings(gameSettingsMessage);
                break;
        }
    }
}
