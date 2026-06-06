namespace MIN.Chess.Messages;

/// <summary>
/// Тип сообщения
/// </summary>
public enum MessageType : byte
{
    /// <summary>
    /// Запрос на ход
    /// </summary>
    MoveRequest = 0,

    /// <summary>
    /// Запрос на сдачу
    /// </summary>
    ResignRequest = 1,

    /// <summary>
    /// Запрос на рестарт
    /// </summary>
    RestartRequest = 2,

    /// <summary>
    /// Обновление с сервера
    /// </summary>
    StateUpdate = 3,

    /// <summary>
    /// Результат игры
    /// </summary>
    GameResult = 4,

    /// <summary>
    /// Ошибка
    /// </summary>
    Error = 5,

    /// <summary>
    /// Присоединение участника
    /// </summary>
    ParticipantConnected = 6,

    /// <summary>
    /// Отсоединения участника
    /// </summary>
    ParticipantDisconnected = 7,

    /// <summary>
    /// Начало игры
    /// </summary>
    GameStart = 8,
}
