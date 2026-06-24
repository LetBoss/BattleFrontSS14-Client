// Decompiled with JetBrains decompiler
// Type: Content.Shared._PUBG.MapGen.PubgBuildingPrototype
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Utility;

#nullable enable
namespace Content.Shared._PUBG.MapGen;

[Prototype(null, 1)]
public sealed class PubgBuildingPrototype : IPrototype
{
  [IdDataField(1, null)]
  public string ID { get; private set; }

  [DataField(null, false, 1, true, false, null)]
  public string Path { get; private set; }

  public ResPath FullPath => new ResPath("/Maps/_PUBG/Buildings/" + this.Path);

  [DataField(null, false, 1, false, false, null)]
  public bool Enabled { get; private set; } = true;

  [DataField(null, false, 1, false, false, null)]
  public bool CanRotate { get; private set; } = true;
}
