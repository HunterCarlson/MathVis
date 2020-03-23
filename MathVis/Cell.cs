namespace MathVis
{
    public class Cell
    {
        public Cell(int index, byte direction)
        {
            Index = index;
            Direction = direction;
        }

        public int Index { get; }
        public byte Direction { get; }
    }
}