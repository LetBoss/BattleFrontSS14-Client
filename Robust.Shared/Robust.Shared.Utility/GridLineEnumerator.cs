using System;
using Robust.Shared.Maths;

namespace Robust.Shared.Utility;

public struct GridLineEnumerator
{
	private int _x;

	private int _y;

	private int _i;

	private int _numerator;

	private readonly int _dx1;

	private readonly int _dy1;

	private readonly int _dx2;

	private readonly int _dy2;

	private readonly int _longest;

	private readonly int _shortest;

	public Vector2i Current => new Vector2i(_x, _y);

	public GridLineEnumerator(Vector2i start, Vector2i finish)
		: this(start.X, start.Y, finish.X, finish.Y)
	{
	}//IL_0001: Unknown result type (might be due to invalid IL or missing references)
	//IL_0007: Unknown result type (might be due to invalid IL or missing references)
	//IL_000d: Unknown result type (might be due to invalid IL or missing references)
	//IL_0013: Unknown result type (might be due to invalid IL or missing references)


	public GridLineEnumerator(int x, int y, int x2, int y2)
	{
		_x = x;
		_y = y;
		int value = x2 - x;
		int value2 = y2 - y;
		_dx1 = Math.Sign(value);
		_dy1 = Math.Sign(value2);
		_dx2 = Math.Sign(value);
		_dy2 = 0;
		_longest = Math.Abs(value);
		_shortest = Math.Abs(value2);
		if (_longest <= _shortest)
		{
			int shortest = _shortest;
			int longest = _longest;
			_longest = shortest;
			_shortest = longest;
			_dx2 = 0;
			_dy2 = Math.Sign(value2);
		}
		_numerator = _longest / 2;
		_i = -1;
	}

	public bool MoveNext()
	{
		if (_i >= _longest)
		{
			return false;
		}
		_i++;
		if (_i == 0)
		{
			return true;
		}
		_numerator += _shortest;
		if (_numerator >= _longest)
		{
			_numerator -= _longest;
			_x += _dx1;
			_y += _dy1;
		}
		else
		{
			_x += _dx2;
			_y += _dy2;
		}
		return true;
	}
}
