using System.Collections.Generic;
using Robust.Shared.Maths;

namespace Robust.Shared.Map;

internal static class GridChunkPartition
{
	public static void PartitionChunk(MapChunk chunk, out Box2i bounds, out List<Box2i> rectangles)
	{
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		rectangles = new List<Box2i>();
		for (ushort num = 0; num < chunk.ChunkSize; num++)
		{
			int num2 = 0;
			bool flag = false;
			for (ushort num3 = 0; num3 < chunk.ChunkSize; num3++)
			{
				if (!chunk.GetTile(num3, num).IsEmpty)
				{
					flag = true;
				}
				else
				{
					if (flag)
					{
						rectangles.Add(new Box2i(num2, (int)num, (int)num3, num + 1));
					}
					num2 = num3 + 1;
					flag = false;
				}
			}
			if (flag)
			{
				rectangles.Add(new Box2i(num2, (int)num, (int)chunk.ChunkSize, num + 1));
			}
		}
		for (int num4 = rectangles.Count - 1; num4 >= 0; num4--)
		{
			Box2i val = rectangles[num4];
			for (int num5 = num4 - 1; num5 >= 0; num5--)
			{
				Box2i val2 = rectangles[num5];
				if (val2.Top < val.Bottom)
				{
					break;
				}
				if (val.Left == val2.Left && val.Right == val2.Right)
				{
					((Box2i)(ref val))._002Ector(val.Left, val2.Bottom, val.Right, val.Top);
					rectangles[num4] = val;
					rectangles.RemoveAt(num5);
					num4--;
				}
			}
		}
		bounds = default(Box2i);
		foreach (Box2i rectangle in rectangles)
		{
			Box2i current = rectangle;
			bounds = (((Box2i)(ref bounds)).IsEmpty() ? current : ((Box2i)(ref bounds)).Union(ref current));
		}
	}
}
