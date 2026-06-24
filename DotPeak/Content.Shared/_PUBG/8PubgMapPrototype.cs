// Decompiled with JetBrains decompiler
// Type: Content.Shared._PUBG.PubgGulagMapPrototype
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Utility;

#nullable enable
namespace Content.Shared._PUBG;

[Prototype(null, 1)]
public sealed class PubgGulagMapPrototype : IPrototype
{
  [DataField(null, false, 1, true, false, null)]
  public ResPath MapPath;
  [DataField(null, false, 1, false, false, null)]
  public string Name = string.Empty;

  [IdDataField(1, null)]
  public string ID { get; private set; }
}
