using System.Text;
using System.Text.Json;
using MIN.Ipc.Messages;
using MIN.Ipc.Messages.Contracts.Models;
using MIN.Ipc.Serialization.Contracts.Interfaces;
using MIN.Ipc.Serialization.Json.Services;
using MIN.Ipc.Services.Contracts.Interfaces;
using MIN.Ipc.Services.Contracts.Models;
using MIN.Ipc.Transport.Contracts.Enums;
using MIN.Ipc.Transport.Contracts.Interfaces;
using MIN.Ipc.Transport.NamedPipes;

namespace MIN.Ipc.Services;

/// <summary>
/// Сервис для отслеживания состояния сессий
/// </summary>
public class ClientProcessBridge : IClientProcessBridge
{
    private readonly IIpcSerializer ipcSerializer = new IpcJsonSerializer();
    private readonly IProcessClientTransport processTransport = new NamedPipeProcessClientTransport();
    private readonly NetworkMessageSerializerRegistry registry = new();

    Task IClientProcessBridge.StartListeningAsync(CancellationToken cancellationToken)
    {
        processTransport.MessageReceived += OnTransportMessage;
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public event EventHandler<INetworkMessage>? NetworkMessageReceived;

    /// <inheritdoc />
    public event EventHandler<Participant>? ParticipantConnected;

    /// <inheritdoc />
    public event EventHandler<Guid>? ParticipantDisconnected;

    private async void OnTransportMessage(object? sender, ProcessClientTransportMessageEventArgs e)
    {
        var message = ipcSerializer.Deserialize(e.Data);
        HandleIpcMessage(message);
    }

    private void HandleIpcMessage(IpcMessage message)
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

                NetworkMessageReceived?.Invoke(this, (INetworkMessage)json);
                break;

            default:
                return;
        }
    }

    async Task IClientProcessBridge.ConnectAsync(string connectionString, int timeOutMs, CancellationToken cancellationToken)
    {
        var connectCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        connectCts.CancelAfter(timeOutMs);
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
    private async Task SendIpcMessage(IpcMessage message, Guid? RecipientId, CancellationToken cancellationToken)
    {
        var messageData = ipcSerializer.Serialize(message);
        var envelope = new IpcMessageEnvelope()
        {
            Body = messageData,
            RecipientId = RecipientId,
        };

        var json = JsonSerializer.Serialize(envelope);
        var data = Encoding.UTF8.GetBytes(json);

        await processTransport.SendAsync(data, cancellationToken);
    }

    async Task IClientProcessBridge.SendReadyMessage(CancellationToken cancellationToken)
    {
        await SendIpcMessage(new ReadyMessage(), RecipientId: null, cancellationToken);
    }

    async Task IClientProcessBridge.SendMessage<T>(T message, Guid? RecipientId, string? msgType, CancellationToken cancellationToken)
        where T : class
    {
        var data = registry.Serialize(msgType, message);
        await SendIpcMessage(new InSessionMessage(data), RecipientId, cancellationToken);
    }

    Task IClientProcessBridge.StopListeningAsync(CancellationToken cancellationToken)
    {
        processTransport.MessageReceived -= OnTransportMessage;
        return Task.CompletedTask;
    }
}
