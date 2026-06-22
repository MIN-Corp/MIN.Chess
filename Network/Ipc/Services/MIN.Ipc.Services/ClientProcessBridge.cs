using System.Text;
using System.Text.Json;
using MIN.Ipc.Messages;
using MIN.Ipc.Messages.Contracts.Enums;
using MIN.Ipc.Messages.Contracts.Models;
using MIN.Ipc.Serialization.Contracts.Interfaces;
using MIN.Ipc.Serialization.Json.Services;
using MIN.Ipc.Services.Contracts.Interfaces;
using MIN.Ipc.Services.Contracts.Models;
using MIN.Ipc.Transport.Contracts.Enums;
using MIN.Ipc.Transport.Contracts.Interfaces;

namespace MIN.Ipc.Services;

/// <summary>
/// Сервис для отслеживания состояния сессий
/// </summary>
public class ClientProcessBridge : IClientProcessBridge
{
    private readonly IIpcSerializer ipcSerializer = new IpcJsonSerializer();
    private readonly NetworkMessageSerializerRegistry registry = new();
    private IProcessClientTransport? processTransport;

    Task IClientProcessBridge.StartListeningAsync(CancellationToken cancellationToken)
    {
        processTransport?.MessageReceived += OnTransportMessage;
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public event EventHandler<INetworkMessage>? NetworkMessageReceived;

    /// <inheritdoc />
    public event EventHandler<Participant>? ParticipantConnected;

    /// <inheritdoc />
    public event EventHandler<Guid>? ParticipantDisconnected;

    /// <inheritdoc />
    public event EventHandler? CloseRequested;

    private async void OnTransportMessage(object? sender, ProcessClientTransportMessageEventArgs e)
    {
        var envelope = JsonSerializer.Deserialize<IpcMinMessageEnvelope>(e.Data);

        if (envelope == null)
        {
            return;
        }

        var message = ipcSerializer.Deserialize(envelope.Body);
        HandleIpcMessage(message, envelope.SenderId);
    }

    private void HandleIpcMessage(IpcMessage message, Guid senderId)
    {
        switch (message)
        {
            case ParticipantConnectedMessage participantConnectedMessage:
                ParticipantConnected?.Invoke(this, new Participant()
                {
                    Id = Guid.Parse(participantConnectedMessage.ParticipantId),
                    Name = participantConnectedMessage.Name,
                });
                break;

            case ParticipantDisconnectedMessage participantDisconnectedMessage:
                ParticipantDisconnected?.Invoke(this, Guid.Parse(participantDisconnectedMessage.ParticipantId));
                break;

            case InSessionMessage inSessionMessage:
                var json = registry.Deserialize(inSessionMessage.Body)
                    ?? throw new ArgumentException($"Не удалось распарсить сообщение: {inSessionMessage.Body}");

                var networkMessage = (INetworkMessage)json;
                networkMessage.SenderId = senderId.ToString();

                NetworkMessageReceived?.Invoke(this, networkMessage);
                break;

            case CloseMessage closeMessage:
                CloseRequested?.Invoke(this, EventArgs.Empty);
                break;

            default:
                return;
        }
    }

    async Task IClientProcessBridge.ConnectAsync(string connectionString, int timeOutMs, CancellationToken cancellationToken)
    {
        var connectCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        connectCts.CancelAfter(timeOutMs);
        processTransport = SessionProcessTransportFactory.Create(connectionString);
        await processTransport.ConnectAsync(connectionString, connectCts.Token);
    }

    void IClientProcessBridge.RegisterMessage<T>(string? typeTag)
        where T : class
    {
        registry.Register<T>(typeTag);
    }

    void IClientProcessBridge.RegisterAllFromAnchor<TAnchor>()
    {
        registry.RegisterAllFromAnchor<TAnchor>();
    }

    /// <inheritdoc />
    private async Task SendIpcMessage(IpcMessage message, SessionMessageRoute route, Guid? RecipientId, IEnumerable<Guid>? BroadcastExcludeIds, CancellationToken cancellationToken)
    {
        if (processTransport == null)
        {
            return;
        }

        var messageData = ipcSerializer.Serialize(message);
        var envelope = new IpcProcessMessageEnvelope()
        {
            Body = messageData,
            RecipientId = RecipientId,
            BroadcastExcludeIds = BroadcastExcludeIds,
            Route = route,
        };

        var json = JsonSerializer.Serialize(envelope);
        var data = Encoding.UTF8.GetBytes(json);

        await processTransport.SendAsync(data, cancellationToken);
    }

    async Task IClientProcessBridge.SendReadyMessage(CancellationToken cancellationToken)
    {
        await SendIpcMessage(new ReadyMessage(), SessionMessageRoute.ViaServer, null, null, cancellationToken);
    }

    async Task IClientProcessBridge.SendMessage<T>(T message, string? msgType, SessionMessageRoute route, Guid? RecipientId, IEnumerable<Guid>? BroadcastExcludeIds, CancellationToken cancellationToken)
        where T : class
    {
        var data = registry.Serialize(msgType, message);
        await SendIpcMessage(new InSessionMessage(data), route, RecipientId, BroadcastExcludeIds, cancellationToken);
    }

    Task IClientProcessBridge.StopListeningAsync(CancellationToken cancellationToken)
    {
        processTransport?.MessageReceived -= OnTransportMessage;
        return Task.CompletedTask;
    }
}
