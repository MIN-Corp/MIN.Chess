using MIN.Ipc.Messages.Contracts.Enums;
using MIN.Ipc.Messages.Contracts.Models;

namespace MIN.Ipc.Messages;

/// <inheritdoc cref="IpcMessageType.ServerShutdown"/>
public sealed record ServerShutdownMessage(string Reason) : IpcMessage(IpcMessageType.ServerShutdown);
