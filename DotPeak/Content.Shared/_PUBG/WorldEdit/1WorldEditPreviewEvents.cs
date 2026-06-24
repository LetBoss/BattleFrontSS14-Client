// Decompiled with JetBrains decompiler
// Type: Content.Shared._PUBG.WorldEdit.WorldEditPreviewDataEvent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared._PUBG.WorldEdit;

[NetSerializable]
[Serializable]
public sealed class WorldEditPreviewDataEvent : EntityEventArgs
{
  public List<WorldEditPreviewEntityData> Entities { get; }

  public List<WorldEditPreviewTileData> Tiles { get; }

  public int Width { get; }

  public int Height { get; }

  public int Degrees { get; }

  public WorldEditPreviewDataEvent(
    List<WorldEditPreviewEntityData> entities,
    List<WorldEditPreviewTileData> tiles,
    int width,
    int height,
    int degrees)
  {
    this.Entities = entities;
    this.Tiles = tiles;
    this.Width = width;
    this.Height = height;
    this.Degrees = degrees;
  }
}
