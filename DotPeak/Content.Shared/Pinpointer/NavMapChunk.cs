// Decompiled with JetBrains decompiler
// Type: Content.Shared.Pinpointer.NavMapChunk
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Maths;
using Robust.Shared.Serialization;
using Robust.Shared.Timing;
using System;

#nullable enable
namespace Content.Shared.Pinpointer;

[NetSerializable]
[Serializable]
public sealed class NavMapChunk(Vector2i origin)
{
  [Robust.Shared.ViewVariables.ViewVariables]
  public readonly Vector2i Origin = origin;
  [Robust.Shared.ViewVariables.ViewVariables]
  public int[] TileData = new int[64 /*0x40*/];
  [NonSerialized]
  public GameTick LastUpdate;
}
