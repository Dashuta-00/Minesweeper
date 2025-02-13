namespace Minesweeper.Models;

public class GameInfoResponse
{
    public Guid Game_id { get; set; }  
    public int Width { get; set; }  
    public int Height { get; set; } 
    public int MinesCount { get; set; }
    public bool Completed { get; set; }
    public List<List<string>> Field { get; set; }
}
