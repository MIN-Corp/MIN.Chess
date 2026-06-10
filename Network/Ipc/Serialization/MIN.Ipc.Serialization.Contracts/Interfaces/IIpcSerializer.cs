using MIN.Ipc.Messages.Contracts.Models;

namespace MIN.Ipc.Serialization.Contracts.Interfaces;

/// <summary>
/// Сериализатор междупроцессорных сообщений
/// </summary>
public interface IIpcSerializer
{
    /// <summary>
    /// Сериализовать междупроцессорное сообщение
    /// </summary>
    byte[] Serialize(IpcMessage message);

    /// <summary>
    /// Десериализовать междупроцессорное сообщение
    /// </summary>
    IpcMessage Deserialize(byte[] data);
}
