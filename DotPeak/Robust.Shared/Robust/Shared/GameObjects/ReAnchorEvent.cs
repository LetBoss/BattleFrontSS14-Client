// Decompiled with JetBrains decompiler
// Type: Robust.Shared.GameObjects.ReAnchorEvent
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Maths;

#nullable enable
namespace Robust.Shared.GameObjects;

[ByRefEvent]
public readonly struct ReAnchorEvent(
  EntityUid uid,
  EntityUid oldGrid,
  EntityUid grid,
  Vector2i tilePos,
  TransformComponent xform)
{
  public readonly EntityUid Entity = uid;
  public readonly EntityUid OldGrid = oldGrid;
  public readonly EntityUid Grid = grid;
  public readonly TransformComponent Xform = xform;
  public readonly Vector2i TilePos = tilePos;
}
