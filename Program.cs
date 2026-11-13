using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

// Show the web page in the wwwroot folder (index.html)
app.UseDefaultFiles();
app.UseStaticFiles();

// The list of ships in the game
List<Ship> ships = new List<Ship>();


// TASK 1: start the game
// Read the ships from the ships.json file
app.MapPost("/start", () =>
{
    string json = File.ReadAllText("ships.json");
    ships = JsonSerializer.Deserialize<List<Ship>>(json);

    foreach (Ship ship in ships)
    {
        ship.CreateCells();
    }

    return "Battle initialized";
});


// TASK 2: fire a missile
// Example: POST /fire?x=3&y=8
app.MapPost("/fire", (int x, int y) =>
{
    if (ships.Count == 0)
    {
        return "Start the game first!";
    }

    if (x < 0 || x > 9 || y < 0 || y > 9)
    {
        return "Invalid coordinates! Use numbers from 0 to 9";
    }

    // Check every cell of every ship
    foreach (Ship ship in ships)
    {
        foreach (Cell cell in ship.Cells)
        {
            if (cell.X == x && cell.Y == y)
            {
                if (cell.Hit == true)
                {
                    return "already hit!";
                }

                cell.Hit = true;

                if (ship.IsSunk())
                {
                    return "sank " + ship.Name + "!";
                }

                return "hit!";
            }
        }
    }

    return "missed!";
});

app.Run();


// ---------- Classes ----------

public class Cell
{
    public int X { get; set; }
    public int Y { get; set; }
    public bool Hit { get; set; }
}

public class Ship
{
    public string Name { get; set; }
    public int StartX { get; set; }
    public int StartY { get; set; }
    public int Size { get; set; }
    public bool Horizontal { get; set; }
    public List<Cell> Cells { get; set; }

    // Create the cells of the ship from the start position
    public void CreateCells()
    {
        Cells = new List<Cell>();

        for (int i = 0; i < Size; i++)
        {
            Cell cell = new Cell();

            if (Horizontal == true)
            {
                cell.X = StartX + i;   // go right
                cell.Y = StartY;
            }
            else
            {
                cell.X = StartX;
                cell.Y = StartY + i;   // go down
            }

            Cells.Add(cell);
        }
    }

    // The ship is sunk when all its cells are hit
    public bool IsSunk()
    {
        foreach (Cell cell in Cells)
        {
            if (cell.Hit == false)
            {
                return false;
            }
        }
        return true;
    }
}
