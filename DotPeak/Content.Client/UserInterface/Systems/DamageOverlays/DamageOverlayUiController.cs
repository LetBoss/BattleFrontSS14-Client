// Decompiled with JetBrains decompiler
// Type: Content.Client.UserInterface.Systems.DamageOverlays.DamageOverlayUiController
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.UserInterface.Systems.DamageOverlays.Overlays;
using Content.Shared.Damage;
using Content.Shared.Damage.Prototypes;
using Content.Shared.FixedPoint;
using Content.Shared.Mobs;
using Content.Shared.Mobs.Components;
using Content.Shared.Mobs.Systems;
using Content.Shared.Traits.Assorted;
using Robust.Client.Graphics;
using Robust.Client.Player;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controllers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using System;

#nullable enable
namespace Content.Client.UserInterface.Systems.DamageOverlays;

public sealed class DamageOverlayUiController : UIController
{
  [Dependency]
  private IOverlayManager _overlayManager;
  [Dependency]
  private IPlayerManager _playerManager;
  [UISystemDependency]
  private readonly MobThresholdSystem _mobThresholdSystem;
  private DamageOverlay _overlay;

  public virtual void Initialize()
  {
    this._overlay = new DamageOverlay();
    this.SubscribeLocalEvent<LocalPlayerAttachedEvent>(new EntityEventHandler<LocalPlayerAttachedEvent>(this.OnPlayerAttach), (Type[]) null, (Type[]) null);
    this.SubscribeLocalEvent<LocalPlayerDetachedEvent>(new EntityEventHandler<LocalPlayerDetachedEvent>(this.OnPlayerDetached), (Type[]) null, (Type[]) null);
    this.SubscribeLocalEvent<MobStateChangedEvent>(new EntityEventHandler<MobStateChangedEvent>(this.OnMobStateChanged), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<MobThresholdChecked>(new EntityEventRefHandler<MobThresholdChecked>((object) this, __methodptr(OnThresholdCheck)), (Type[]) null, (Type[]) null);
  }

  private void OnPlayerAttach(LocalPlayerAttachedEvent args)
  {
    this.ClearOverlay();
    MobStateComponent mobState;
    if (!this.EntityManager.TryGetComponent<MobStateComponent>(args.Entity, ref mobState))
      return;
    if (mobState.CurrentState != MobState.Dead)
      this.UpdateOverlays(args.Entity, mobState);
    this._overlayManager.AddOverlay((Overlay) this._overlay);
  }

  private void OnPlayerDetached(LocalPlayerDetachedEvent args)
  {
    this._overlayManager.RemoveOverlay((Overlay) this._overlay);
    this.ClearOverlay();
  }

  private void OnMobStateChanged(MobStateChangedEvent args)
  {
    EntityUid target = args.Target;
    EntityUid? localEntity = ((ISharedPlayerManager) this._playerManager).LocalEntity;
    if ((localEntity.HasValue ? (EntityUid.op_Inequality(target, localEntity.GetValueOrDefault()) ? 1 : 0) : 1) != 0)
      return;
    this.UpdateOverlays(args.Target, args.Component);
  }

  private void OnThresholdCheck(ref MobThresholdChecked args)
  {
    EntityUid target = args.Target;
    EntityUid? localEntity = ((ISharedPlayerManager) this._playerManager).LocalEntity;
    if ((localEntity.HasValue ? (EntityUid.op_Inequality(target, localEntity.GetValueOrDefault()) ? 1 : 0) : 1) != 0)
      return;
    this.UpdateOverlays(args.Target, args.MobState, args.Damageable, args.Threshold);
  }

  private void ClearOverlay()
  {
    this._overlay.DeadLevel = 0.0f;
    this._overlay.CritLevel = 0.0f;
    this._overlay.PainLevel = 0.0f;
    this._overlay.OxygenLevel = 0.0f;
  }

  private void UpdateOverlays(
    EntityUid entity,
    MobStateComponent? mobState,
    DamageableComponent? damageable = null,
    MobThresholdsComponent? thresholds = null)
  {
    FixedPoint2? threshold;
    if (mobState == null && !this.EntityManager.TryGetComponent<MobStateComponent>(entity, ref mobState) || thresholds == null && !this.EntityManager.TryGetComponent<MobThresholdsComponent>(entity, ref thresholds) || damageable == null && !this.EntityManager.TryGetComponent<DamageableComponent>(entity, ref damageable) || !this._mobThresholdSystem.TryGetIncapThreshold(entity, out threshold, thresholds))
      return;
    if (!thresholds.ShowOverlays)
    {
      this.ClearOverlay();
    }
    else
    {
      FixedPoint2 fixedPoint2_1 = threshold.Value;
      this._overlay.State = mobState.CurrentState;
      switch (mobState.CurrentState)
      {
        case MobState.Alive:
          FixedPoint2 fixedPoint2_2 = (FixedPoint2) 0;
          this._overlay.PainLevel = 0.0f;
          FixedPoint2 fixedPoint2_3;
          if (!this.EntityManager.HasComponent<PainNumbnessComponent>(entity))
          {
            foreach (ProtoId<DamageGroupPrototype> painDamageGroup in damageable.PainDamageGroups)
            {
              FixedPoint2 fixedPoint2_4;
              damageable.DamagePerGroup.TryGetValue(ProtoId<DamageGroupPrototype>.op_Implicit(painDamageGroup), out fixedPoint2_4);
              fixedPoint2_2 += fixedPoint2_4;
            }
            DamageOverlay overlay = this._overlay;
            fixedPoint2_3 = FixedPoint2.Min((FixedPoint2) 1f, fixedPoint2_2 / fixedPoint2_1);
            double num = (double) fixedPoint2_3.Float();
            overlay.PainLevel = (float) num;
            if ((double) this._overlay.PainLevel < 0.05000000074505806)
              this._overlay.PainLevel = 0.0f;
          }
          FixedPoint2 fixedPoint2_5;
          if (damageable.DamagePerGroup.TryGetValue("Airloss", out fixedPoint2_5))
          {
            DamageOverlay overlay = this._overlay;
            fixedPoint2_3 = FixedPoint2.Min((FixedPoint2) 1f, fixedPoint2_5 / fixedPoint2_1);
            double num = (double) fixedPoint2_3.Float();
            overlay.OxygenLevel = (float) num;
          }
          this._overlay.CritLevel = 0.0f;
          this._overlay.DeadLevel = 0.0f;
          break;
        case MobState.Critical:
          FixedPoint2? percentage;
          if (!this._mobThresholdSystem.TryGetDeadPercentage(entity, FixedPoint2.Max((FixedPoint2) 0.0, damageable.TotalDamage), out percentage))
            break;
          this._overlay.CritLevel = percentage.Value.Float();
          this._overlay.PainLevel = 0.0f;
          this._overlay.DeadLevel = 0.0f;
          break;
        case MobState.Dead:
          this._overlay.PainLevel = 0.0f;
          this._overlay.CritLevel = 0.0f;
          break;
      }
    }
  }
}
