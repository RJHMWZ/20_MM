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
			get
			{
				return _width;
			}
			set
			{
				_width = value;
			}
		}

		public int Height
		{
			get
			{
				return _height;
			}
			set
			{
				_height = value;
			}
		}

		public T this[int xIdx, int yIdx]
		{
			get
			{
				if (xIdx >= 0 && xIdx < _width && yIdx >= 0 && yIdx < _height)
				{
					return _grid[xIdx, yIdx];
				}
				return default(T);
			}
			set
			{
				if (xIdx >= 0 && xIdx < _width && yIdx >= 0 && yIdx < _height)
				{
					_grid[xIdx, yIdx] = value;
				}
			}
		}

		public EasyGrid(int w, int h)
		{
			_width = w;
			_height = h;
			_grid = new T[w, h];
		}

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

		public void Resize(int w, int h, Func<int, int, T> addFunc)
		{
			T[,] newGrid = new T[w, h];
			for (int x2 = 0; x2 < _width; x2++)
			{
				for (int y2 = 0; y2 < _height; y2++)
				{
					newGrid[x2, y2] = _grid[x2, y2];
				}
				for (int y = _height; y < h; y++)
				{
					newGrid[x2, y] = addFunc(x2, y);
				}
			}
			for (int x = _width; x < w; x++)
			{
				for (int y3 = 0; y3 < h; y3++)
				{
					newGrid[x, y3] = addFunc(x, y3);
				}
			}
			_width = w;
			_height = h;
			_grid = newGrid;
		}

		public void ForEach(Action<int, int, T> each)
		{
			for (int x = 0; x < _width; x++)
			{
				for (int y = 0; y < _height; y++)
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
			for (int x = 0; x < _width; x++)
			{
				for (int y = 0; y < _height; y++)
				{
					cleanAction?.Invoke(_grid[x, y]);
					_grid[x, y] = default(T);
				}
			}
			_grid = null;
		}
	}
}
