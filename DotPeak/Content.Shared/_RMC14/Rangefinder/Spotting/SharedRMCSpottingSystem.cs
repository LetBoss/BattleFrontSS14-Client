// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Rangefinder.Spotting.SharedRMCSpottingSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Stealth;
using Content.Shared._RMC14.Targeting;
using Content.Shared.Actions;
using Content.Shared.Actions.Components;
using Content.Shared.Examine;
using Content.Shared.Hands.Components;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Popups;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Timing;
using System;

#nullable enable
namespace Content.Shared._RMC14.Rangefinder.Spotting;

public abstract class SharedRMCSpottingSystem : EntitySystem
{
  [Dependency]
  private SharedRMCTargetingSystem _targeting;
  [Dependency]
  private SharedAppearanceSystem _appearance;
  [Dependency]
  private SharedAudioSystem _audio;
  [Dependency]
  private SharedHandsSystem _hands;
  [Dependency]
  private SharedPopupSystem _popup;
  [Dependency]
  private SharedActionsSystem _actions;
  [Dependency]
  private ExamineSystemShared _examine;
  [Dependency]
  protected IGameTiming Timing;

  public override void Initialize()
  {
    this.SubscribeLocalEvent<SpottingComponent, GetItemActionsEvent>(new EntityEventRefHandler<SpottingComponent, GetItemActionsEvent>(this.OnSpotterGetItemActions));
    this.SubscribeLocalEvent<SpottingComponent, SpotTargetActionEvent>(new EntityEventRefHandler<SpottingComponent, SpotTargetActionEvent>(this.OnSpotTarget));
    this.SubscribeLocalEvent<SpottingComponent, TargetingFinishedEvent>(new EntityEventRefHandler<SpottingComponent, TargetingFinishedEvent>(this.OnTargetingFinished));
  }

  private void OnSpotterGetItemActions(Entity<SpottingComponent> ent, ref GetItemActionsEvent args)
  {
    args.AddAction(ref ent.Comp.Action, (string) ent.Comp.ActionId);
    this.Dirty(ent.Owner, (IComponent) ent.Comp);
  }

  private void OnSpotTarget(Entity<SpottingComponent> ent, ref SpotTargetActionEvent args)
  {
    ent.Comp.Activated = !ent.Comp.Activated;
    this.Dirty<SpottingComponent>(ent);
    SharedActionsSystem actions = this._actions;
    EntityUid? action1 = ent.Comp.Action;
    Entity<ActionComponent>? action2 = action1.HasValue ? new Entity<ActionComponent>?((Entity<ActionComponent>) action1.GetValueOrDefault()) : new Entity<ActionComponent>?();
    int num = ent.Comp.Activated ? 1 : 0;
    actions.SetToggled(action2, num != 0);
    args.Handled = true;
  }

  private void OnTargetingFinished(Entity<SpottingComponent> ent, ref TargetingFinishedEvent args)
  {
    TargetingComponent comp;
    if (!this.TryComp<TargetingComponent>((EntityUid) ent, out comp))
      return;
    this._targeting.StopTargeting((Entity<TargetingComponent>) ((EntityUid) ent, comp), args.Target);
  }

  protected void SpotRequested(NetEntity netSpottingTool, NetEntity netUser, NetEntity netTarget)
  {
    EntityUid entity1 = this.GetEntity(netSpottingTool);
    EntityUid entity2 = this.GetEntity(netUser);
    EntityUid entity3 = this.GetEntity(netTarget);
    SpottingComponent comp1;
    if (!this.TryComp<SpottingComponent>(entity1, out comp1) || !this.CanSpot((Entity<SpottingComponent>) (entity1, comp1), entity3, entity2))
      return;
    EntityTurnInvisibleComponent comp2;
    TargetingLaserComponent comp3;
    if (this.TryComp<EntityTurnInvisibleComponent>(entity2, out comp2) && this.TryComp<TargetingLaserComponent>(entity1, out comp3))
    {
      comp3.ShowLaser = !comp2.Enabled;
      this.Dirty(entity1, (IComponent) comp3);
    }
    comp1.NextSpot = this.Timing.CurTime + comp1.SpottingCooldown;
    this.Dirty(entity1, (IComponent) comp1);
    SpottedComponent spottedComponent = this.EnsureComp<SpottedComponent>(entity3);
    this.Dirty(entity3, (IComponent) spottedComponent);
    this._audio.PlayPredicted(comp1.SpottingSound, entity1, new EntityUid?(entity2));
    object obj;
    this._appearance.TryGetData(entity1, (Enum) RangefinderLayers.Layer, out obj);
    if (obj != null)
      this._appearance.SetData(entity1, (Enum) RangefinderLayers.Layer, (object) RangefinderMode.Spotter);
    this._targeting.Target(entity1, entity2, entity3, comp1.SpottingDuration, TargetedEffects.Spotted);
  }

  private bool CanSpot(Entity<SpottingComponent> ent, EntityUid target, EntityUid user)
  {
    if (!this.HasComp<SpottableComponent>(target) || !this._examine.InRangeUnOccluded(user, target, (float) ent.Comp.SpottingRange) || ent.Comp.NextSpot > this.Timing.CurTime)
      return false;
    if (!this.HasComp<SpotterWhitelistComponent>(user))
    {
      this._popup.PopupClient(this.Loc.GetString("rmc-action-popup-spotting-user-no-skill", ("rangefinder", (object) ent)), user, new EntityUid?(user));
      return false;
    }
    EntityUid? nullable1;
    if (this._hands.TryGetActiveItem((Entity<HandsComponent>) user, out nullable1))
    {
      EntityUid? nullable2 = nullable1;
      EntityUid entityUid = (EntityUid) ent;
      if ((nullable2.HasValue ? (nullable2.GetValueOrDefault() != entityUid ? 1 : 0) : 1) == 0)
        return true;
    }
    this._popup.PopupClient(this.Loc.GetString("rmc-action-popup-spotting-user-must-hold", ("rangefinder", (object) ent)), user, new EntityUid?(user));
    return false;
  }
}
