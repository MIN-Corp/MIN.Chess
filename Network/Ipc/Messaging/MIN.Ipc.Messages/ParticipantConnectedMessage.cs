using MIN.Ipc.Messages.Contracts.Enums;
using MIN.Ipc.Messages.Contracts.Models;

namespace MIN.Ipc.Messages;

/// <inheritdoc cref="IpcMessageType.ParticipantConnected"/>
public sealed record ParticipantConnectedMessage(string ParticipantId,
    string Name
) : IpcMessage(IpcMessageType.ParticipantConnected);
