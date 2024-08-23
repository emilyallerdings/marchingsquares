using Godot;
using System;

public partial class MarchingSquares : Node2D
{
	[Export] public int Resolution = 8;
	
	[Export(PropertyHint.Range, "0,100,0.1")] 
	public float TimeScale = 10.0f;
	
	[Export] public bool Smooth = true;
	[Export] public FastNoiseLite Noise3D;

	private float[] _grid;
	private int _width = 800;
	private int _height = 600;
	private int _cols;
	private int _rows;
	private double _timeElapsed = 0;
	
	Color white = new Color(1.0f, 1.0f, 1.0f);
	
	public override void _Ready()
	{
		_cols = 1 + _width / Resolution;
		_rows = 1 + _height / Resolution;
		_grid = new float[_cols * _rows];

		UpdateGrid();
	}

	public override void _Process(double delta)
	{
		_timeElapsed += delta;
		UpdateGrid();
		QueueRedraw();
	}

	private void UpdateGrid()
	{
		for (int y = 0; y < _rows; y++)
		{
			for (int x = 0; x < _cols; x++)
			{
				float val = Noise3D.GetNoise3D(x, y, (float)_timeElapsed * TimeScale);
				_grid[x + y * _cols] = val;
			}
		}
	}

	private int GetState(float a, float b, float c, float d)
	{
		int state = 0;
		if (a > 0) state += 8;
		if (b > 0) state += 4;
		if (c > 0) state += 2;
		if (d > 0) state += 1;
		return state;
	}

	private float Lerp(float a, float b)
	{
		if (!Smooth)
			return 0.5f;

		if (Math.Abs(a - b) < 0.00001f)
			return 0.5f;

		return (0 - a) / (b - a);
	}

	public override void _Draw()
	{
		for (int y = 0; y < _rows - 1; y++)
		{
			for (int x = 0; x < _cols - 1; x++)
			{
				int i = x * Resolution;
				int j = y * Resolution;

				float p1 = _grid[x + y * _cols];
				float p2 = _grid[x + 1 + y * _cols];
				float p3 = _grid[x + 1 + (y + 1) * _cols];
				float p4 = _grid[x + (y + 1) * _cols];

				int state = GetState(p1, p2, p3, p4);

				Vector2 a = new Vector2(i + Lerp(p1, p2) * Resolution, j);
				Vector2 b = new Vector2(i + Resolution, j + Lerp(p2, p3) * Resolution);
				Vector2 c = new Vector2(i + Lerp(p4, p3) * Resolution, j + Resolution);
				Vector2 d = new Vector2(i, j + Lerp(p1, p4) * Resolution);

				switch (state)
				{
					case 1: DrawLine(c, d, white, 1); break;
					case 2: DrawLine(b, c, white, 1); break;
					case 3: DrawLine(b, d, white, 1); break;
					case 4: DrawLine(a, b, white, 1); break;
					case 5:
						DrawLine(a, d, white, 1);
						DrawLine(b, c, white, 1);
						break;
					case 6: DrawLine(a, c, white, 1); break;
					case 7: DrawLine(a, d, white, 1); break;
					case 8: DrawLine(a, d, white, 1); break;
					case 9: DrawLine(a, c, white, 1); break;
					case 10:
						DrawLine(a, b, white, 1);
						DrawLine(c, d, white, 1);
						break;
					case 11: DrawLine(a, b, white, 1); break;
					case 12: DrawLine(b, d, white, 1); break;
					case 13: DrawLine(b, c, white, 1); break;
					case 14: DrawLine(c, d, white, 1); break;
				}
			}
		}
	}
}
