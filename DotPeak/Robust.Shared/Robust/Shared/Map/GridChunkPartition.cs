// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Map.GridChunkPartition
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Maths;
using System.Collections.Generic;

#nullable enable
namespace Robust.Shared.Map;

internal static class GridChunkPartition
{
  public static void PartitionChunk(MapChunk chunk, out Box2i bounds, out List<Box2i> rectangles)
  {
    rectangles = new List<Box2i>();
    for (ushort yIndex = 0; (int) yIndex < (int) chunk.ChunkSize; ++yIndex)
    {
      int num = 0;
      bool flag = false;
      for (ushort xIndex = 0; (int) xIndex < (int) chunk.ChunkSize; ++xIndex)
      {
        if (!chunk.GetTile(xIndex, yIndex).IsEmpty)
        {
          flag = true;
        }
        else
        {
          if (flag)
            rectangles.Add(new Box2i(num, (int) yIndex, (int) xIndex, (int) yIndex + 1));
          num = (int) xIndex + 1;
          flag = false;
        }
      }
      if (flag)
        rectangles.Add(new Box2i(num, (int) yIndex, (int) chunk.ChunkSize, (int) yIndex + 1));
    }
    for (int index1 = rectangles.Count - 1; index1 >= 0; --index1)
    {
      Box2i box2i1 = rectangles[index1];
      for (int index2 = index1 - 1; index2 >= 0; --index2)
      {
        Box2i box2i2 = rectangles[index2];
        if (box2i2.Top >= box2i1.Bottom)
        {
          if (box2i1.Left == box2i2.Left && box2i1.Right == box2i2.Right)
          {
            // ISSUE: explicit constructor call
            ((Box2i) ref box2i1).\u002Ector(box2i1.Left, box2i2.Bottom, box2i1.Right, box2i1.Top);
            rectangles[index1] = box2i1;
            rectangles.RemoveAt(index2);
            --index1;
          }
        }
        else
          break;
      }
    }
    bounds = new Box2i();
    foreach (Box2i box2i in rectangles)
      bounds = ((Box2i) ref bounds).IsEmpty() ? box2i : ((Box2i) ref bounds).Union(ref box2i);
  }
}
