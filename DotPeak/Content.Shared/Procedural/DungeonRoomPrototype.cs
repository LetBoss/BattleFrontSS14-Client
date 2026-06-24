// Decompiled with JetBrains decompiler
// Type: Content.Shared.Procedural.DungeonRoomPrototype
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Maps;
using Content.Shared.Tag;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Utility;
using Robust.Shared.ViewVariables;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared.Procedural;

[Prototype(null, 1)]
public sealed class DungeonRoomPrototype : IPrototype
{
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [DataField(null, false, 1, false, false, null)]
  public List<ProtoId<TagPrototype>> Tags = new List<ProtoId<TagPrototype>>();
  [DataField(null, false, 1, true, false, null)]
  public Vector2i Size;
  [DataField("atlas", false, 1, true, false, null)]
  public ResPath AtlasPath;
  [DataField(null, false, 1, true, false, null)]
  public Vector2i Offset;
  [DataField(null, false, 1, false, false, null)]
  public ProtoId<ContentTileDefinition>? IgnoreTile;

  [IdDataField(1, null)]
  public string ID { get; private set; } = string.Empty;
}
