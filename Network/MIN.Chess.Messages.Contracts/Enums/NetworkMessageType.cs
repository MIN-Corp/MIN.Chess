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
    /// Присоединение оппонента
    /// </summary>
    OpponentConnected = 6,

    /// <summary>
    /// Отсоединения оппонента
    /// </summary>
    OpponentDisconnected = 7,

    /// <summary>
    /// Начало игры
    /// </summary>
    GameStart = 8,
}
