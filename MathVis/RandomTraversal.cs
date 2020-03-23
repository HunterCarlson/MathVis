using System;
using System.Collections.Generic;
using System.Linq;

namespace MathVis
{
    public class RandomTraversal
    {
        private const byte N = 1 << 0;
        private const byte S = 1 << 1;
        private const byte W = 1 << 2;
        private const byte E = 1 << 3;
        private readonly int _cellHeight;
        private readonly byte[] _cells;
        private readonly int _cellWidth;
        private readonly List<Cell> _frontier;

        private readonly Random _rng = new Random();

        public RandomTraversal(int width, int height)
        {
            _cellWidth = width;
            _cellHeight = height;

            _cells = new byte[width * height];
            _frontier = new List<Cell>();

            int startIndex = (height - 1) * width;
            _cells[startIndex] = 0;
            _frontier.Add(new Cell(startIndex, N));
            _frontier.Add(new Cell(startIndex, E));
        }

        public byte[] GenerateMaze()
        {
            while (_frontier.Any())
            {
                // pop a random cell from the frontier
                Cell edge = PopRandom(_frontier);
                int i0 = edge.Index;
                byte d0 = edge.Direction;

                // get the index for the next cell

                int i1 = d0 switch
                {
                    N => (i0 - _cellWidth),
                    S => (i0 + _cellWidth),
                    W => (i0 - 1),
                    E => (i0 + 1),
                    _ => throw new InvalidOperationException("invalid direction")
                };

                int x0 = i0 % _cellWidth;
                int y0 = i0 / _cellWidth;

                int x1;
                int y1;
                byte d1;

                // check if next cell is part of maze
                bool open = _cells[i1] == 0;

                // go to the next cell
                switch (d0)
                {
                    case N:
                        x1 = x0;
                        y1 = y0 - 1;
                        d1 = S;
                        break;
                    case S:
                        x1 = x0;
                        y1 = y0 + 1;
                        d1 = N;
                        break;
                    case W:
                        x1 = x0 - 1;
                        y1 = y0;
                        d1 = E;
                        break;
                    case E:
                        x1 = x0 + 1;
                        y1 = y0;
                        d1 = W;
                        break;
                    default:
                        throw new InvalidOperationException("invalid direction");
                }

                // if you can still explore the maze from the cell
                if (open)
                {
                    _cells[i0] |= d0;
                    _cells[i1] |= d1;

                    if (y1 > 0 && _cells[i1 - _cellWidth] == 0)
                    {
                        _frontier.Add(new Cell(i1, N));
                    }

                    if (y1 < _cellHeight - 1 && _cells[i1 + _cellWidth] == 0)
                    {
                        _frontier.Add(new Cell(i1, S));
                    }

                    if (x1 > 0 && _cells[i1 - 1] == 0)
                    {
                        _frontier.Add(new Cell(i1, W));
                    }

                    if (x1 < _cellWidth - 1 && _cells[i1 + 1] == 0)
                    {
                        _frontier.Add(new Cell(i1, E));
                    }
                }
            }

            return _cells;
        }

        private Cell PopRandom(IList<Cell> cells)
        {
            // make sure list has elements
            if (!cells.Any())
            {
                throw new IndexOutOfRangeException("Empty list - nothing to pop!");
            }

            int n = cells.Count;
            int i = _rng.Next(0, n);
            Cell popped = cells[i];
            cells.RemoveAt(i);
            return popped;
        }
    }
}