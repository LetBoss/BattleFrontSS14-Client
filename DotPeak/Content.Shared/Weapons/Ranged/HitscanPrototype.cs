// Decompiled with JetBrains decompiler
// Type: Content.Shared.Weapons.Ranged.HitscanPrototype
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Damage;
using Content.Shared.Weapons.Reflect;
using Robust.Shared.Audio;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Utility;
using Robust.Shared.ViewVariables;

#nullable enable
namespace Content.Shared.Weapons.Ranged;

[Prototype(null, 1)]
public sealed class HitscanPrototype : IPrototype, IShootable
{
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [DataField("staminaDamage", false, 1, false, false, null)]
  public float StaminaDamage;
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [DataField("damage", false, 1, false, false, null)]
  public DamageSpecifier? Damage;
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadOnly)]
  [DataField("muzzleFlash", false, 1, false, false, null)]
  public SpriteSpecifier? MuzzleFlash;
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadOnly)]
  [DataField("travelFlash", false, 1, false, false, null)]
  public SpriteSpecifier? TravelFlash;
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadOnly)]
  [DataField("impactFlash", false, 1, false, false, null)]
  public SpriteSpecifier? ImpactFlash;
  [DataField("collisionMask", false, 1, false, false, null)]
  public int CollisionMask = 1;
  [DataField("reflective", false, 1, false, false, null)]
  public ReflectType Reflective = ReflectType.Energy;
  [DataField("sound", false, 1, false, false, null)]
  public SoundSpecifier? Sound;
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [DataField("forceSound", false, 1, false, false, null)]
  public bool ForceSound;
  [DataField("maxLength", false, 1, false, false, null)]
  public float MaxLength = 20f;

  [Robust.Shared.ViewVariables.ViewVariables]
  [IdDataField(1, null)]
  public string ID { get; private set; }
}
