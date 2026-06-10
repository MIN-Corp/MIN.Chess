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
    };

    byte[] IIpcSerializer.Serialize(IpcMessage message)
    {
        var json = JsonSerializer.Serialize(message, message.GetType());
        var bytes = Encoding.UTF8.GetBytes(json);
        return BitConverter.GetBytes(bytes.Length).Concat(bytes).ToArray();
    }

    IpcMessage IIpcSerializer.Deserialize(byte[] data)
    {
        using var doc = JsonDocument.Parse(data);
        var type = (IpcMessageType)doc.RootElement.GetProperty("type").GetByte();
        return (IpcMessage)JsonSerializer.Deserialize(data, typeMap[type])!;
    }
}
