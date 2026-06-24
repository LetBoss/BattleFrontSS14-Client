// Decompiled with JetBrains decompiler
// Type: Content.Shared.Drunk.SharedDrunkSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Speech.EntitySystems;
using Content.Shared.StatusEffect;
using Content.Shared.Traits.Assorted;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using System;

#nullable enable
namespace Content.Shared.Drunk;

public abstract class SharedDrunkSystem : EntitySystem
{
  public static readonly ProtoId<StatusEffectPrototype> DrunkKey = (ProtoId<StatusEffectPrototype>) "Drunk";
  [Dependency]
  private StatusEffectsSystem _statusEffectsSystem;
  [Dependency]
  private SharedSlurredSystem _slurredSystem;

  public void TryApplyDrunkenness(
    EntityUid uid,
    float boozePower,
    bool applySlur = true,
    StatusEffectsComponent? status = null)
  {
    if (!this.Resolve<StatusEffectsComponent>(uid, ref status, false))
      return;
    LightweightDrunkComponent comp;
    if (this.TryComp<LightweightDrunkComponent>(uid, out comp))
      boozePower *= comp.BoozeStrengthMultiplier;
    if (applySlur)
      this._slurredSystem.DoSlur(uid, TimeSpan.FromSeconds((double) boozePower), status);
    if (!this._statusEffectsSystem.HasStatusEffect(uid, (string) SharedDrunkSystem.DrunkKey, status))
      this._statusEffectsSystem.TryAddStatusEffect<DrunkComponent>(uid, (string) SharedDrunkSystem.DrunkKey, TimeSpan.FromSeconds((double) boozePower), true, status);
    else
      this._statusEffectsSystem.TryAddTime(uid, (string) SharedDrunkSystem.DrunkKey, TimeSpan.FromSeconds((double) boozePower), status);
  }

  public void TryRemoveDrunkenness(EntityUid uid)
  {
    this._statusEffectsSystem.TryRemoveStatusEffect(uid, (string) SharedDrunkSystem.DrunkKey);
  }

  public void TryRemoveDrunkenessTime(EntityUid uid, double timeRemoved)
  {
    this._statusEffectsSystem.TryRemoveTime(uid, (string) SharedDrunkSystem.DrunkKey, TimeSpan.FromSeconds(timeRemoved));
  }
}
