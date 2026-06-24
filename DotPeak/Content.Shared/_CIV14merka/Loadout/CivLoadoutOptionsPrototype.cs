// Decompiled with JetBrains decompiler
// Type: Content.Shared._CIV14merka.Loadout.CivLoadoutOptionsPrototype
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._CIV14merka.Teams;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared._CIV14merka.Loadout;

[Prototype(null, 1)]
public sealed class CivLoadoutOptionsPrototype : IPrototype
{
  [DataField(null, false, 1, true, false, null)]
  public string Faction;
  [DataField(null, false, 1, true, false, null)]
  public CivTdmClass Class;
  [DataField(null, false, 1, true, false, null)]
  public Dictionary<string, List<EntProtoId>> Slots = new Dictionary<string, List<EntProtoId>>();

  [IdDataField(1, null)]
  public string ID { get; private set; }
}
