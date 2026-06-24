// Decompiled with JetBrains decompiler
// Type: Content.Shared._PUBG.Ghost.PubgCkeyGhostPrototype
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Ghost;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared._PUBG.Ghost;

[Prototype(null, 1)]
public sealed class PubgCkeyGhostPrototype : IPrototype
{
  [IdDataField(1, null)]
  public string ID { get; private set; }

  [DataField("ckeys", false, 1, true, false, null)]
  public List<string> Ckeys { get; private set; } = new List<string>();

  [DataField("proto", false, 1, true, false, null)]
  public EntProtoId<GhostComponent> GhostEntityPrototype { get; private set; }

  [DataField("adminProto", false, 1, false, false, null)]
  public EntProtoId<GhostComponent>? AdminGhostEntityPrototype { get; private set; }

  public bool MatchesCkey(string ckey)
  {
    foreach (string ckey1 in this.Ckeys)
    {
      if (ckey1.Equals(ckey, StringComparison.OrdinalIgnoreCase))
        return true;
    }
    return false;
  }
}
