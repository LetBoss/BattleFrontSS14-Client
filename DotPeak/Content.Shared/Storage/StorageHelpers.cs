// Decompiled with JetBrains decompiler
// Type: Content.Shared.Storage.StorageHelper
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Maths;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared.Storage;

public static class StorageHelper
{
  public static Box2i GetBoundingBox(this IReadOnlyList<Box2i> boxes)
  {
    if (boxes.Count == 0)
      return new Box2i();
    Box2i box1 = boxes[0];
    if (boxes.Count == 1)
      return box1;
    int bottom = box1.Bottom;
    int left = box1.Left;
    int top = box1.Top;
    int right = box1.Right;
    for (int index = 1; index < boxes.Count; ++index)
    {
      Box2i box2 = boxes[index];
      if (bottom > box2.Bottom)
        bottom = box2.Bottom;
      if (left > box2.Left)
        left = box2.Left;
      if (top < box2.Top)
        top = box2.Top;
      if (right < box2.Right)
        right = box2.Right;
    }
    return new Box2i(left, bottom, right, top);
  }

  public static int GetArea(this IReadOnlyList<Box2i> boxes)
  {
    int area = 0;
    Box2i boundingBox = boxes.GetBoundingBox();
    for (int bottom = boundingBox.Bottom; bottom <= boundingBox.Top; ++bottom)
    {
      for (int left = boundingBox.Left; left <= boundingBox.Right; ++left)
      {
        if (boxes.Contains(left, bottom))
          ++area;
      }
    }
    return area;
  }

  public static bool Contains(this IReadOnlyList<Box2i> boxes, int x, int y)
  {
    foreach (Box2i box in (IEnumerable<Box2i>) boxes)
    {
      if (((Box2i) ref box).Contains(x, y))
        return true;
    }
    return false;
  }

  public static bool Contains(this IReadOnlyList<Box2i> boxes, Vector2i point)
  {
    foreach (Box2i box in (IEnumerable<Box2i>) boxes)
    {
      if (((Box2i) ref box).Contains(point, true))
        return true;
    }
    return false;
  }
}
