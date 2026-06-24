// Decompiled with JetBrains decompiler
// Type: Content.Shared.StatusEffect.StatusEffectPrototype
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Alert;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;

#nullable enable
namespace Content.Shared.StatusEffect;

[Prototype(null, 1)]
public sealed class StatusEffectPrototype : IPrototype
{
  [IdDataField(1, null)]
  public string ID { get; private set; }

  [DataField("alert", false, 1, false, false, null)]
  public ProtoId<AlertPrototype>? Alert { get; private set; }

  [DataField("alwaysAllowed", false, 1, false, false, null)]
  public bool AlwaysAllowed { get; private set; }
}
