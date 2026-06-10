using MIN.Ipc.Messages.Contracts.Enums;
using MIN.Ipc.Messages.Contracts.Models;

namespace MIN.Ipc.Messages;

/// <inheritdoc cref="IpcMessageType.Ready"/>
public sealed record ReadyMessage() : IpcMessage(IpcMessageType.Ready);
