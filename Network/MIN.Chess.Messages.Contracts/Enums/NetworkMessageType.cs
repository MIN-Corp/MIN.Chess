namespace MIN.Chess.Messages.Contracts.Enums;

/// <summary>
/// Тип сообщения
/// </summary>
public enum NetworkMessageType : byte
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
    /// Запрос на обновление с сервера
    /// </summary>
    StateUpdateRequest = 3,

    /// <summary>
    /// Обновление с сервера
    /// </summary>
    StateUpdate = 4,

    /// <summary>
    /// Результат игры
    /// </summary>
    GameResult = 5,

    /// <summary>
    /// Ошибка
    /// </summary>
    Error = 6,

    /// <summary>
    /// Присоединение оппонента
    /// </summary>
    OpponentConnected = 7,

    /// <summary>
    /// Отсоединения оппонента
    /// </summary>
    OpponentDisconnected = 8,

    /// <summary>
    /// Ход с сервера
    /// </summary>
    MoveUpdate = 9,
}
