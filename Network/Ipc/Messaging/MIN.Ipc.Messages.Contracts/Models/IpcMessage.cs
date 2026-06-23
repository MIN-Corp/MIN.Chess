using MIN.Ipc.Messages.Contracts.Enums;

namespace MIN.Ipc.Messages.Contracts.Models;

/// <summary>
/// Между-процессорное сообщение
/// </summary>
public abstract record IpcMessage(IpcMessageType Type);
