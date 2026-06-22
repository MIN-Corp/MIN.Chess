using System.Text;
using System.Text.Json;
using MIN.Ipc.Messages;
using MIN.Ipc.Messages.Contracts.Enums;
using MIN.Ipc.Messages.Contracts.Models;
using MIN.Ipc.Serialization.Contracts.Interfaces;

namespace MIN.Ipc.Serialization.Json.Services;

/// <inheritdoc cref="IIpcSerializer"/>
public sealed class IpcJsonSerializer : IIpcSerializer
{
    private readonly Dictionary<IpcMessageType, Type> typeMap = new()
    {
        [IpcMessageType.Ready] = typeof(ReadyMessage),
        [IpcMessageType.InSession] = typeof(InSessionMessage),
        [IpcMessageType.ParticipantConnected] = typeof(ParticipantConnectedMessage),
        [IpcMessageType.ParticipantDisconnected] = typeof(ParticipantDisconnectedMessage),
        [IpcMessageType.ServerShutdown] = typeof(ServerShutdownMessage),
        [IpcMessageType.Close] = typeof(CloseMessage),
    };

    byte[] IIpcSerializer.Serialize(IpcMessage message)
    {
        var json = JsonSerializer.Serialize(message, message.GetType());
        return Encoding.UTF8.GetBytes(json);
    }

    IpcMessage IIpcSerializer.Deserialize(byte[] data)
    {
        using var doc = JsonDocument.Parse(data);
        var type = (IpcMessageType)doc.RootElement.GetProperty(nameof(IpcMessage.Type)).GetByte();
        return (IpcMessage)JsonSerializer.Deserialize(data, typeMap[type])!;
    }
}
