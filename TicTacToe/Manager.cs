namespace TicTacToe;

public static class Manager
{
    public static Tuple<int, int> BoardIndexToCoordinate(int index)
    {
        var row = index / 3;
        var column = index % 3;
        
        return new Tuple<int, int>(row, column);
    }

    public static int BoardCoordinateToIndex(int row, int column)
    {
        return row * 3 + column;
    }
}