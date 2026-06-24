// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Damage.RMCXenoDamageVisualsSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Damage;
using Content.Shared.FixedPoint;
using Content.Shared.Mobs.Components;
using Content.Shared.Mobs.Systems;
using Content.Shared.Rounding;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using System;

#nullable enable
namespace Content.Shared._RMC14.Xenonids.Damage;

public sealed class RMCXenoDamageVisualsSystem : EntitySystem
{
  [Dependency]
  private SharedAppearanceSystem _appearance;
  [Dependency]
  private MobThresholdSystem _thresholds;
  private Robust.Shared.GameObjects.EntityQuery<MobThresholdsComponent> _mobThresholdsQuery;

  public override void Initialize()
  {
    this._mobThresholdsQuery = this.GetEntityQuery<MobThresholdsComponent>();
    this.SubscribeLocalEvent<RMCXenoDamageVisualsComponent, DamageChangedEvent>(new EntityEventRefHandler<RMCXenoDamageVisualsComponent, DamageChangedEvent>(this.OnVisualsDamageChanged));
  }

  private void OnVisualsDamageChanged(
    Entity<RMCXenoDamageVisualsComponent> ent,
    ref DamageChangedEvent args)
  {
    MobThresholdsComponent component;
    FixedPoint2? threshold;
    if (!this._mobThresholdsQuery.TryComp((EntityUid) ent, out component) || !this._thresholds.TryGetIncapThreshold((EntityUid) ent, out threshold, component))
      return;
    double actual = args.Damageable.TotalDamage.Double();
    double max = threshold.Value.Double();
    FixedPoint2 fixedPoint2 = (FixedPoint2) actual;
    FixedPoint2? nullable = threshold;
    int num = (nullable.HasValue ? (fixedPoint2 > nullable.GetValueOrDefault() ? 1 : 0) : 0) == 0 ? ContentHelpers.RoundToEqualLevels(actual, max, ent.Comp.States + 1) : ent.Comp.States + 1;
    this._appearance.SetData((EntityUid) ent, (Enum) RMCDamageVisuals.State, (object) num);
  }
}
