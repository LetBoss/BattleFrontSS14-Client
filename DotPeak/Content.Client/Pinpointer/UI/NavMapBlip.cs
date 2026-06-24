// Decompiled with JetBrains decompiler
// Type: Content.Client.Pinpointer.UI.NavMapBlip
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Robust.Client.Graphics;
using Robust.Shared.Map;
using Robust.Shared.Maths;

#nullable enable
namespace Content.Client.Pinpointer.UI;

public struct NavMapBlip(
  EntityCoordinates coordinates,
  Texture texture,
  Color color,
  bool blinks,
  bool selectable = true,
  float scale = 1f)
{
  public EntityCoordinates Coordinates = coordinates;
  public Texture Texture = texture;
  public Color Color = color;
  public bool Blinks = blinks;
  public bool Selectable = selectable;
  public float Scale = scale;
}
