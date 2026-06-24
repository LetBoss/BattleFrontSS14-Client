// Decompiled with JetBrains decompiler
// Type: Content.Shared.Chat.SharedSuicideSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Damage;
using Content.Shared.Damage.Prototypes;
using Content.Shared.FixedPoint;
using Content.Shared.Mobs.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using System;
using System.Linq;

#nullable enable
namespace Content.Shared.Chat;

public sealed class SharedSuicideSystem : EntitySystem
{
  private static readonly ProtoId<DamageTypePrototype> FallbackDamageType = ProtoId<DamageTypePrototype>.op_Implicit("Blunt");
  [Dependency]
  private DamageableSystem _damageableSystem;
  [Dependency]
  private IPrototypeManager _prototypeManager;

  public void ApplyLethalDamage(Entity<DamageableComponent> target, DamageSpecifier damageSpecifier)
  {
    DamageSpecifier damage = new DamageSpecifier(damageSpecifier);
    MobThresholdsComponent thresholdsComponent;
    if (!this.TryComp<MobThresholdsComponent>(Entity<DamageableComponent>.op_Implicit(target), ref thresholdsComponent))
      return;
    FixedPoint2 fixedPoint2_1 = thresholdsComponent.Thresholds.Keys.Last<FixedPoint2>() - target.Comp.TotalDamage;
    FixedPoint2 total = damage.GetTotal();
    damage.DamageDict.Remove("Structural");
    foreach ((string key, FixedPoint2 fixedPoint2_2) in damage.DamageDict)
      damage.DamageDict[key] = (FixedPoint2) Math.Ceiling((double) (fixedPoint2_2 * fixedPoint2_1 / total));
    this._damageableSystem.TryChangeDamage(new EntityUid?(Entity<DamageableComponent>.op_Implicit(target)), damage, true, origin: new EntityUid?(Entity<DamageableComponent>.op_Implicit(target)));
  }

  public void ApplyLethalDamage(
    Entity<DamageableComponent> target,
    ProtoId<DamageTypePrototype>? damageType)
  {
    MobThresholdsComponent thresholdsComponent;
    if (!this.TryComp<MobThresholdsComponent>(Entity<DamageableComponent>.op_Implicit(target), ref thresholdsComponent))
      return;
    FixedPoint2 fixedPoint2 = thresholdsComponent.Thresholds.Keys.Last<FixedPoint2>() - target.Comp.TotalDamage;
    DamageTypePrototype type;
    if (!this._prototypeManager.TryIndex<DamageTypePrototype>(damageType, ref type) || type.ID == "Structural")
    {
      this.Log.Error($"{nameof (SharedSuicideSystem)} could not find the damage type prototype associated with {damageType}. Falling back to {SharedSuicideSystem.FallbackDamageType}");
      type = this._prototypeManager.Index<DamageTypePrototype>(SharedSuicideSystem.FallbackDamageType);
    }
    DamageSpecifier damage = new DamageSpecifier(type, fixedPoint2);
    this._damageableSystem.TryChangeDamage(new EntityUid?(Entity<DamageableComponent>.op_Implicit(target)), damage, true, origin: new EntityUid?(Entity<DamageableComponent>.op_Implicit(target)));
  }
}
