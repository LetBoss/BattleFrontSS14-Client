using System;
using System.Numerics;

namespace Content.Shared._CIV14merka.GlobalMap;

public static class CivMapGrid
{
	public const int Divisions = 8;

	public static (int Col, int Row) GetCell(Vector2 boundsMin, Vector2 boundsMax, Vector2 world)
	{
		Vector2 size = boundsMax - boundsMin;
		int value = ((size.X > 0f) ? ((int)((world.X - boundsMin.X) / size.X * 8f)) : 0);
		return new ValueTuple<int, int>(item2: Math.Clamp((size.Y > 0f) ? ((int)((boundsMax.Y - world.Y) / size.Y * 8f)) : 0, 0, 7), item1: Math.Clamp(value, 0, 7));
	}

	public static string GetCellLabel(Vector2 boundsMin, Vector2 boundsMax, Vector2 world)
	{
		var (col, row) = GetCell(boundsMin, boundsMax, world);
		return GetColumnLabel(col) + (row + 1);
	}

	public static string GetColumnLabel(int col)
	{
		return ((char)(65 + Math.Clamp(col, 0, 25))).ToString();
	}

	public static float LineX(Vector2 boundsMin, Vector2 boundsMax, int i)
	{
		return boundsMin.X + (boundsMax.X - boundsMin.X) * (float)i / 8f;
	}

	public static float LineY(Vector2 boundsMin, Vector2 boundsMax, int i)
	{
		return boundsMax.Y - (boundsMax.Y - boundsMin.Y) * (float)i / 8f;
	}

	public static float ColumnCenterX(Vector2 boundsMin, Vector2 boundsMax, int col)
	{
		return boundsMin.X + (boundsMax.X - boundsMin.X) * ((float)col + 0.5f) / 8f;
	}

	public static float RowCenterY(Vector2 boundsMin, Vector2 boundsMax, int row)
	{
		return boundsMax.Y - (boundsMax.Y - boundsMin.Y) * ((float)row + 0.5f) / 8f;
	}
}
