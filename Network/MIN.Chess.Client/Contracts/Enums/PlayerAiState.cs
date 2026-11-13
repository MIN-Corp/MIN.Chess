namespace MIN.Chess.Client.Contracts.Enums
{
    /// <summary>
    /// Состояние игроков за доской
    /// </summary>
    public enum PlayerAiState
    {
        /// <summary>
        /// Оба ИИ
        /// </summary>
        BothAi,

        /// <summary>
        /// Один за доской
        /// </summary>
        Alone,

        /// <summary>
        /// Игра с ИИ
        /// </summary>
        WithAi,

        /// <summary>
        /// Игроки люди
        /// </summary>
        BothHuman
    }
}
