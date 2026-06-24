using System.Collections.Generic;
using Robust.Shared.Maths;

namespace Content.Shared.Storage;

public static class StorageHelper
{
	public static Box2i GetBoundingBox(this IReadOnlyList<Box2i> boxes)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		if (boxes.Count == 0)
		{
			return default(Box2i);
		}
		Box2i firstBox = boxes[0];
		if (boxes.Count == 1)
		{
			return firstBox;
		}
		int bottom = firstBox.Bottom;
		int left = firstBox.Left;
		int top = firstBox.Top;
		int right = firstBox.Right;
		for (int i = 1; i < boxes.Count; i++)
		{
			Box2i box = boxes[i];
			if (bottom > box.Bottom)
			{
				bottom = box.Bottom;
			}
			if (left > box.Left)
			{
				left = box.Left;
			}
			if (top < box.Top)
			{
				top = box.Top;
			}
			if (right < box.Right)
			{
				right = box.Right;
			}
		}
		return new Box2i(left, bottom, right, top);
	}

	public static int GetArea(this IReadOnlyList<Box2i> boxes)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		int area = 0;
		Box2i bounding = boxes.GetBoundingBox();
		for (int y = bounding.Bottom; y <= bounding.Top; y++)
		{
			for (int x = bounding.Left; x <= bounding.Right; x++)
			{
				if (boxes.Contains(x, y))
				{
					area++;
				}
			}
		}
		return area;
	}

	public static bool Contains(this IReadOnlyList<Box2i> boxes, int x, int y)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		foreach (Box2i box2 in boxes)
		{
			Box2i box = box2;
			if (((Box2i)(ref box)).Contains(x, y))
			{
				return true;
			}
		}
		return false;
	}

	public static bool Contains(this IReadOnlyList<Box2i> boxes, Vector2i point)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		foreach (Box2i box2 in boxes)
		{
			Box2i box = box2;
			if (((Box2i)(ref box)).Contains(point, true))
			{
				return true;
			}
		}
		return false;
	}
}
