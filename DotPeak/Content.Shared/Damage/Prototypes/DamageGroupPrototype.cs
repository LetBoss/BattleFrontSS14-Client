// Decompiled with JetBrains decompiler
// Type: Content.Shared.Damage.Prototypes.DamageGroupPrototype
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.List;

#nullable enable
namespace Content.Shared.Damage.Prototypes;

[Robust.Shared.Prototypes.Prototype(2)]
public sealed class DamageGroupPrototype : IPrototype
{
  [IdDataField(1, null)]
  public string ID { get; private set; }

  [DataField(null, false, 1, true, false, null)]
  private LocId Name { get; set; }

  [Robust.Shared.ViewVariables.ViewVariables]
  public string LocalizedName => Loc.GetString(LocId.op_Implicit(this.Name));

  [DataField("damageTypes", false, 1, true, false, typeof (PrototypeIdListSerializer<DamageTypePrototype>))]
  public System.Collections.Generic.List<string> DamageTypes { get; private set; }
}
