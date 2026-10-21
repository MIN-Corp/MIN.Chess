using MIN.Ipc.Messages.Contracts.Enums;
using MIN.Ipc.Messages.Contracts.Models;

namespace MIN.Ipc.Messages;

/// <inheritdoc cref="IpcMessageType.InSession"/>
public sealed record InSessionMessage(byte[] Body) : IpcMessage(IpcMessageType.InSession);
