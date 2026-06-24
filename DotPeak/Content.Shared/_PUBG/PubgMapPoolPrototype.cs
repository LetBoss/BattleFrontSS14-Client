// Decompiled with JetBrains decompiler
// Type: Content.Shared._PUBG.PubgMapPoolPrototype
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared._PUBG;

[Prototype(null, 1)]
public sealed class PubgMapPoolPrototype : IPrototype
{
  [IdDataField(1, null)]
  public string ID { get; private set; }

  [DataField("maps", false, 1, true, false, null)]
  public List<string> Maps { get; private set; } = new List<string>();
}
