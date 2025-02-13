namespace Minesweeper.Services;

using Minesweeper.Models;

/// <summary>
/// Сервис для обработки логики игры
/// </summary>
public class GameLogic
{
    public GameInfoResponse GameInfo { get; set; }
    public bool[,] OpenedCells { get; set; }

    /// <summary>
    /// Конструктор для создания игры
    /// </summary>
    /// <param name="width">Ширина поля</param>
    /// <param name="height">Высота поля</param>
    /// <param name="minesCount">Количество мин в игре</param>
    public GameLogic(int width, int height, int minesCount)
    {
        GameInfo = new GameInfoResponse
        {
            Game_id = Guid.NewGuid(),
            Width = width,
            Height = height,
            MinesCount = minesCount,
            Completed = false,
            Field = new List<List<string>>()
        };

        InitField();
        CreateMines();
        CalculateNumbers();

        OpenedCells = new bool[height, width];
    }

    /// <summary>
    /// Инициализирует игровое поле, заполняя его пустыми значениями
    /// </summary>
    private void InitField()
    {
        for (int i = 0; i < GameInfo.Height; i++)
        {
            var row = new List<string>();

            for (int j = 0; j < GameInfo.Width; j++)
            {
                row.Add(" ");
            }
            GameInfo.Field.Add(row);
        }
    }

    /// <summary>
    /// Размещает мины на поле случайным образом
    /// </summary>
    private void CreateMines()
    {
        var random = new Random();
        int minesCountCreated = 0;

        while (minesCountCreated < GameInfo.MinesCount)
        {
            int row = random.Next(GameInfo.Height);
            int col = random.Next(GameInfo.Width);

            if (GameInfo.Field[row][col] != "X")
            {
                GameInfo.Field[row][col] = "X";
                minesCountCreated++;
            }
        }
    }

    /// <summary>
    /// Вычисляет количество мин вокруг каждой ячейки
    /// </summary>
    private void CalculateNumbers()
    {
        for (int row = 0; row < GameInfo.Height; row++)
        {
            for (int col = 0; col < GameInfo.Width; col++)
            {
                if (GameInfo.Field[row][col] == "X")
                {
                    continue;
                }

                int minesCountAround = CountMines(row, col);
                GameInfo.Field[row][col] = minesCountAround.ToString();
            }
        }
    }

    /// <summary>
    /// Считает количество мин в соседних ячейках
    /// </summary>
    /// <param name="row">Номер строки текущей ячейки</param>
    /// <param name="col">Номер столбца текущей ячейки</param>
    /// <returns>Количество мин вокруг ячейки</returns>
    private int CountMines(int row, int col)
    {
        int minesCount = 0;

        for (int rowOffset = -1; rowOffset <= 1; rowOffset++)
        {
            for (int colOffset = -1; colOffset <= 1; colOffset++)
            {
                if (rowOffset == 0 && colOffset == 0)
                {
                    continue;
                }

                int neighborRow = row + rowOffset;
                int neighborCol = col + colOffset;

                if (neighborRow >= 0 && neighborRow < GameInfo.Height
                    && neighborCol >= 0 && neighborCol < GameInfo.Width)
                {
                    if (GameInfo.Field[neighborRow][neighborCol] == "X")
                    {
                        minesCount++;
                    }
                }
            }
        }

        return minesCount;
    }

    /// <summary>
    /// Обрабатывает ход по выбранной ячейке
    /// </summary>
    /// <param name="row">Номер строки ячейки</param>
    /// <param name="col">Номер столбца ячейки</param>
    public void ProcessCell(int row, int col)
    {
        if (CellIsMine(row, col))
        {
            ProcessLose();
            return;
        }

        OpenCell(row, col);
    }

    /// <summary>
    /// Обрабатывает случай проигрыша, открывая все ячейки
    /// </summary>
    private void ProcessLose()
    {
        OpenAllCells(false);
        GameInfo.Completed = true;
    }

    /// <summary>
    /// Открывает все ячейки. Если победа, то меняет мину на "M"
    /// </summary>
    /// <param name="win">Флаг победы (если true, меняет мину на "M")</param>
    private void OpenAllCells(bool win)
    {
        for (int i = 0; i < GameInfo.Height; i++)
        {
            for (int j = 0; j < GameInfo.Width; j++)
            {
                OpenedCells[i, j] = true;

                if (GameInfo.Field[i][j] == "X" && win)
                {
                    GameInfo.Field[i][j] = "M";
                }
            }
        }
    }

    /// <summary>
    /// Проверяет, является ли выбранная ячейка миной
    /// </summary>
    /// <param name="row">Номер строки ячейки</param>
    /// <param name="col">Номер столбца ячейки</param>
    /// <returns>Возвращает true, если ячейка содержит мину, иначе false.</returns>
    private bool CellIsMine(int row, int col)
    {
        return GameInfo.Field[row][col] == "X";
    }

    /// <summary>
    /// Открывает выбранную ячейку и рекурсивно соседние, если рядом нет мин
    /// </summary>
    /// <param name="row">Номер строки клетки.</param>
    /// <param name="col">Номер столбца клетки.</param>
    private void OpenCell(int row, int col)
    {
        if (OpenedCells[row, col])
        {
            return;
        }

        OpenedCells[row, col] = true;

        int adjacentMines = CountMines(row, col);
        if (adjacentMines == 0)
        {
            for (int rowOffset = -1; rowOffset <= 1; rowOffset++)
            {
                for (int colOffset = -1; colOffset <= 1; colOffset++)
                {
                    int neighborRow = row + rowOffset;
                    int neighborCol = col + colOffset;

                    if (neighborRow >= 0 && neighborRow < GameInfo.Height
                        && neighborCol >= 0 && neighborCol < GameInfo.Width)
                    {
                        OpenCell(neighborRow, neighborCol);
                    }
                }
            }
        }

        GameInfo.Field[row][col] = adjacentMines == 0 ? "0" : adjacentMines.ToString();

        CheckWin();
    }

    /// <summary>
    /// Проверяет, была ли выиграна игра
    /// </summary>
    private void CheckWin()
    {
        bool allSafeCellsOpened = true;

        for (int i = 0; i < GameInfo.Height; i++)
        {
            for (int j = 0; j < GameInfo.Width; j++)
            {
                if (!OpenedCells[i, j] && GameInfo.Field[i][j] != "X")
                {
                    allSafeCellsOpened = false;
                    break;
                }
            }

            if (!allSafeCellsOpened)
            {
                break;
            }
        }

        if (allSafeCellsOpened)
        {
            OpenAllCells(true);
        }
    }

}
