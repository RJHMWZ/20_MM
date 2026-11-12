using System;

namespace CodeKits.Grid
{
    public class EasyGrid<T>
    {
        private T[,] _grid;
        private int _width;
        private int _height;

        public int Width
        {
            get => _width;
            set => _width = value;
        }

        public int Height
        {
            get => _height;
            set => _height = value;
        }

        public EasyGrid(int w, int h)
        {
            _width = w;
            _height = h;
            _grid = new T[w, h];
        }

        public T this[int xIdx, int yIdx]
        {
            get
            {
                if (xIdx >= 0 && xIdx < _width && yIdx >= 0 && yIdx < _height)
                    return _grid[xIdx, yIdx];
                else
                    return default;
            }
            set
            {
                if (xIdx >= 0 && xIdx < _width && yIdx >= 0 && yIdx < _height)
                    _grid[xIdx, yIdx] = value;
            }
        }

        /// <summary>
        /// 填充满
        /// </summary>
        /// <param name="t"></param>
        public void Fill(T t)
        {
            for (int x = 0; x < _width; x++)
            {
                for (int y = 0; y < _height; y++)
                {
                    _grid[x, y] = t;
                }
            }
        }

        public void Fill(Func<int, int, T> func)
        {
            for (int x = 0; x < _width; x++)
            {
                for (int y = 0; y < _height; y++)
                {
                    _grid[x, y] = func(x, y);
                }
            }
        }

        /// <summary>
        /// 改变大小
        /// </summary>
        public void Resize(int w, int h, Func<int, int, T> addFunc)
        {
            var newGrid = new T[w, h];
            for (int x = 0; x < _width; x++)
            {
                for (int y = 0; y < _height; y++)
                {
                    newGrid[x, y] = _grid[x, y];
                }

                // add x
                for (var y = _height; y < h; y++)
                    newGrid[x, y] = addFunc(x, y);
            }

            for (var x = _width; x < w; x++)
            {
                // add y
                for (var y = 0; y < h; y++)
                    newGrid[x, y] = addFunc(x, y);
            }

            _width = w;
            _height = h;
            _grid = newGrid;
        }

        public void ForEach(Action<int, int, T> each)
        {
            for (var x = 0; x < _width; x++)
            {
                for (var y = 0; y < _height; y++)
                {
                    each(x, y, _grid[x, y]);
                }
            }
        }

        public void ForEach(Action<T> each)
        {
            for (int x = 0; x < _width; x++)
            {
                for (int y = 0; y < _height; y++)
                {
                    each(_grid[x, y]);
                }
            }
        }
        
        public void Clear(Action<T> cleanAction = null)
        {
            for (var x = 0; x < _width; x++)
            {
                for (var y = 0; y < _height; y++)
                {
                    cleanAction?.Invoke(_grid[x, y]);
                    _grid[x, y] = default;
                }
            }

            _grid = null;
        }
    }
}
