// Decompiled with JetBrains decompiler
// Type: Content.Client.Shuttles.UI.GridDrawData
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Robust.Shared.Timing;
using System.Collections.Generic;
using System.Numerics;

#nullable enable
namespace Content.Client.Shuttles.UI;

public sealed class GridDrawData
{
  public List<Vector2> Vertices = new List<Vector2>();
  public int EdgeIndex;
  public GameTick LastBuild;
}
