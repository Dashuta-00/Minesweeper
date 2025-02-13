namespace Minesweeper.Services;

using Minesweeper.Models;

/// <summary>
/// Сервис для управления играми
/// </summary>
public class GameManagerService
{
    private readonly Dictionary<Guid, GameLogic> _games = new();
    private const int MaxHeight = 30;
    private const int MaxWidth = 30;

    /// <summary>
    /// Создает новую игру с заданными параметрами
    /// </summary>
    /// <param name="request">Запрос для создания новой игры (содержит размер поля и число мин)</param>
    /// <returns>Информация о созданной игре</returns>
    /// <exception cref="ArgumentException">Выбрасывается, если размеры поля или количество мин некорректны</exception>
    public GameInfoResponse CreateGame(NewGameRequest request)
    {
        if (request.Width > MaxWidth || request.Height > MaxHeight)
        {
            throw new ArgumentException("Размеры поля должны быть не больше 30х30");
        }

        if (request.Mines_count > request.Width * request.Height - 1)
        {
            throw new ArgumentException($"Число мин должно быть не больше {request.Width * request.Height - 1}");
        }

        var game = new GameLogic(request.Width, request.Height, request.Mines_count);
        _games[game.GameInfo.Game_id] = game;

        return GetGameInfo(game);
    }

    /// <summary>
    /// Обрабатывает нажатие клетки на поле
    /// </summary>
    /// <param name="request">Запрос на выполнение хода (содержит Guid игры и координаты клетки)</param>
    /// <returns>Информация об игре после хода</returns>
    /// <exception cref="ArgumentException">Выбрасывается, если игра не найдена</exception>
    /// <exception cref="InvalidOperationException">Выбрасывается, если игра завершена или ячейка уже открыта</exception>
    public GameInfoResponse MakeTurn(GameTurnRequest request)
    {
        if (!_games.ContainsKey(request.Game_id))
        {
            throw new ArgumentException("Игра не найдена");
        }

        var game = _games[request.Game_id];

        if (game.GameInfo.Completed)
        {
            throw new InvalidOperationException("Игра уже завершена");
        }

        if (game.OpenedCells[request.Row, request.Col])
        {
            throw new InvalidOperationException("Ячейка уже открыта");
        }

        game.ProcessCell(request.Row, request.Col);

        return GetGameInfo(game);
    }

    /// <summary>
    /// Получает текущее состояние игры
    /// </summary>
    /// <param name="game">Игровой объект</param>
    /// <returns>Информация о текущем состоянии игры</returns>
    private GameInfoResponse GetGameInfo(GameLogic game)
    {
        var fieldCopy = new List<List<string>>();

        for (int i = 0; i < game.GameInfo.Height; i++)
        {
            var row = new List<string>();
            for (int j = 0; j < game.GameInfo.Width; j++)
            {
                if (game.OpenedCells[i, j])
                {
                    row.Add(game.GameInfo.Field[i][j]);
                }
                else
                {
                    row.Add(" ");
                }
            }
            fieldCopy.Add(row);
        }

        return new GameInfoResponse
        {
            Game_id = game.GameInfo.Game_id,
            Width = game.GameInfo.Width,
            Height = game.GameInfo.Height,
            MinesCount = game.GameInfo.MinesCount,
            Completed = game.GameInfo.Completed,
            Field = fieldCopy
        };
    }
}