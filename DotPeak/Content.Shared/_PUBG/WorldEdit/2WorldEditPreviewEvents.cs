// Decompiled with JetBrains decompiler
// Type: Content.Shared._PUBG.WorldEdit.WorldEditPreviewEntityData
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Maths;
using Robust.Shared.Serialization;
using System;
using System.Numerics;

#nullable enable
namespace Content.Shared._PUBG.WorldEdit;

[NetSerializable]
[Serializable]
public sealed class WorldEditPreviewEntityData
{
  public string PrototypeId { get; }

  public Vector2 RelativePosition { get; }

  public Angle Rotation { get; }

  public WorldEditPreviewEntityData(string prototypeId, Vector2 relativePosition, Angle rotation)
  {
    this.PrototypeId = prototypeId;
    this.RelativePosition = relativePosition;
    this.Rotation = rotation;
  }
}
