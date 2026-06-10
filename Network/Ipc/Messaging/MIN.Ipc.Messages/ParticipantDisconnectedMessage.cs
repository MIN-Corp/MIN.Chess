using MIN.Ipc.Messages.Contracts.Enums;
using MIN.Ipc.Messages.Contracts.Models;

namespace MIN.Ipc.Messages;

/// <inheritdoc cref="IpcMessageType.ParticipantDisconnected"/>
public sealed record ParticipantDisconnectedMessage(string ParticipantId) : IpcMessage(IpcMessageType.ParticipantDisconnected);
