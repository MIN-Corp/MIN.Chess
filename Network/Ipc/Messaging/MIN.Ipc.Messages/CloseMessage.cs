using MIN.Ipc.Messages.Contracts.Enums;
using MIN.Ipc.Messages.Contracts.Models;

namespace MIN.Ipc.Messages;

/// <inheritdoc cref="IpcMessageType.Close"/>
public sealed record CloseMessage() : IpcMessage(IpcMessageType.Close);
