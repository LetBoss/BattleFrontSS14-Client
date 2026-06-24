// Decompiled with JetBrains decompiler
// Type: Content.Shared.NPC.SharedPathfindingSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Maths;
using System;
using System.Collections.Generic;
using System.Numerics;

#nullable enable
namespace Content.Shared.NPC;

public abstract class SharedPathfindingSystem : EntitySystem
{
  public const byte SubStep = 4;
  public const byte ChunkSize = 8;
  public static readonly Vector2 ChunkSizeVec = new Vector2(8f, 8f);
  protected const float StepOffset = 0.125f;
  private static readonly Vector2 StepOffsetVec = new Vector2(0.125f, 0.125f);

  public Vector2 GetCoordinate(Vector2i chunk, Vector2i index)
  {
    return new Vector2((float) index.X, (float) index.Y) / 4f + Vector2i.op_Implicit(chunk) * SharedPathfindingSystem.ChunkSizeVec + SharedPathfindingSystem.StepOffsetVec;
  }

  public static float ManhattanDistance(Vector2i start, Vector2i end)
  {
    Vector2i vector2i = Vector2i.op_Subtraction(end, start);
    return (float) (Math.Abs(vector2i.X) + Math.Abs(vector2i.Y));
  }

  public static float OctileDistance(Vector2i start, Vector2i end)
  {
    Vector2 vector2 = Vector2.Abs(Vector2i.op_Implicit(Vector2i.op_Subtraction(start, end)));
    return (float) ((double) vector2.X + (double) vector2.Y + -0.59000003337860107 * (double) Math.Min(vector2.X, vector2.Y));
  }

  public static IEnumerable<Vector2i> GetTileOutline(Vector2i center, float radius)
  {
    Vector2 vecCircle = Vector2i.op_Implicit(center) + Vector2.One / 2f;
    for (int r = 0; (double) r <= Math.Floor((double) radius * (double) MathF.Sqrt(0.5f)); ++r)
    {
      float d = MathF.Floor(MathF.Sqrt(radius * radius - (float) (r * r)));
      yield return Vector2Helpers.Floored(new Vector2(vecCircle.X - d, vecCircle.Y + (float) r));
      yield return Vector2Helpers.Floored(new Vector2(vecCircle.X + d, vecCircle.Y + (float) r));
      yield return Vector2Helpers.Floored(new Vector2(vecCircle.X - d, vecCircle.Y - (float) r));
      yield return Vector2Helpers.Floored(new Vector2(vecCircle.X + d, vecCircle.Y - (float) r));
      yield return Vector2Helpers.Floored(new Vector2(vecCircle.X + (float) r, vecCircle.Y - d));
      yield return Vector2Helpers.Floored(new Vector2(vecCircle.X + (float) r, vecCircle.Y + d));
      yield return Vector2Helpers.Floored(new Vector2(vecCircle.X - (float) r, vecCircle.Y - d));
      yield return Vector2Helpers.Floored(new Vector2(vecCircle.X - (float) r, vecCircle.Y + d));
    }
  }

  public static void GridCast(
    Vector2i start,
    Vector2i end,
    SharedPathfindingSystem.Vector2iCallback callback)
  {
    if (!callback(start))
      return;
    int num1 = end.X < start.X ? -1 : 1;
    int num2 = end.Y < start.Y ? -1 : 1;
    int num3 = num1 * (end.X - start.X);
    int num4 = num2 * (end.Y - start.Y);
    int x = start.X;
    int y = start.Y;
    if (num3 == num4)
    {
      while (num3-- > 0)
      {
        x += num1;
        y += num2;
        if (!callback(new Vector2i(x, y)))
          break;
      }
    }
    else
    {
      int num5 = -1 * ((num3 == 0 ? num2 : num1) - 1);
      int num6 = num3 + num4;
      int num7 = num3 - num4;
      int num8 = num3 * 2;
      int num9 = num4 * 2;
      while (num6-- > 0)
      {
        if (num7 > 0 || num7 == num5)
        {
          x += num1;
          num7 -= num9;
        }
        else
        {
          y += num2;
          num7 += num8;
        }
        if (!callback(new Vector2i(x, y)))
          break;
      }
    }
  }

  public delegate bool Vector2iCallback(Vector2i index);
}
