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
    /// Присоединение игрока
    /// </summary>
    PlayerConnected = 7,

    /// <summary>
    /// Отсоединения игрока
    /// </summary>
    PlayerDisconnected = 8,

    /// <summary>
    /// Ход с сервера
    /// </summary>
    MoveUpdate = 9,

    /// <summary>
    /// Запрос на первоначальные данные от сервера
    /// </summary>
    InitRequest = 10,

    /// <summary>
    /// Первоначальные данные от сервера
    /// </summary>
    InitResponse = 11,

    /// <summary>
    /// Голос на рестарт
    /// </summary>
    RestartVote = 12,

    /// <summary>
    /// Запрос на смену роли
    /// </summary>
    ChangeRole = 13,
}
