namespace MIN.Ipc.Services.Contracts.Models;

/// <summary>
/// Участник
/// </summary>
public record Participant
{
    /// <summary>
    /// Идентификатор
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Имя
    /// </summary>
    public string Name { get; set; } = string.Empty;
}
