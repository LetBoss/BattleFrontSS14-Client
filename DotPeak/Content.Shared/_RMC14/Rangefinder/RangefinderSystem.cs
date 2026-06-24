// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Rangefinder.RangefinderSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Areas;
using Content.Shared._RMC14.Dropship.Weapon;
using Content.Shared._RMC14.Inventory;
using Content.Shared._RMC14.Marines.Skills;
using Content.Shared._RMC14.Marines.Squads;
using Content.Shared._RMC14.Rules;
using Content.Shared.Coordinates.Helpers;
using Content.Shared.DoAfter;
using Content.Shared.Examine;
using Content.Shared.Hands;
using Content.Shared.Interaction;
using Content.Shared.Interaction.Events;
using Content.Shared.Inventory.Events;
using Content.Shared.Popups;
using Content.Shared.Timing;
using Content.Shared.Verbs;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared._RMC14.Rangefinder;

public sealed class RangefinderSystem : EntitySystem
{
  [Dependency]
  private SharedAppearanceSystem _appearance;
  [Dependency]
  private AreaSystem _area;
  [Dependency]
  private SharedAudioSystem _audio;
  [Dependency]
  private SharedDoAfterSystem _doAfter;
  [Dependency]
  private SharedDropshipWeaponSystem _dropshipWeapon;
  [Dependency]
  private ExamineSystemShared _examine;
  [Dependency]
  private IMapManager _mapManager;
  [Dependency]
  private INetManager _net;
  [Dependency]
  private MetaDataSystem _metaData;
  [Dependency]
  private SharedPopupSystem _popup;
  [Dependency]
  private RMCPlanetSystem _rmcPlanet;
  [Dependency]
  private SkillsSystem _skills;
  [Dependency]
  private SquadSystem _squad;
  [Dependency]
  private SharedTransformSystem _transform;
  [Dependency]
  private SharedUserInterfaceSystem _ui;
  [Dependency]
  private UseDelaySystem _useDelay;

  public override void Initialize()
  {
    this.SubscribeLocalEvent<RangefinderComponent, MapInitEvent>(new EntityEventRefHandler<RangefinderComponent, MapInitEvent>(this.OnRangefinderMapInit));
    this.SubscribeLocalEvent<RangefinderComponent, AfterInteractEvent>(new EntityEventRefHandler<RangefinderComponent, AfterInteractEvent>(this.OnRangefinderAfterInteract));
    this.SubscribeLocalEvent<RangefinderComponent, LaserDesignatorDoAfterEvent>(new EntityEventRefHandler<RangefinderComponent, LaserDesignatorDoAfterEvent>(this.OnRangefinderDoAfter));
    this.SubscribeLocalEvent<RangefinderComponent, ExaminedEvent>(new EntityEventRefHandler<RangefinderComponent, ExaminedEvent>(this.OnRangefinderExamined));
    this.SubscribeLocalEvent<RangefinderComponent, GetVerbsEvent<AlternativeVerb>>(new EntityEventRefHandler<RangefinderComponent, GetVerbsEvent<AlternativeVerb>>(this.OnRangefinderGetAlternativeVerbs));
    this.SubscribeLocalEvent<ActiveLaserDesignatorComponent, ComponentRemove>(new EntityEventRefHandler<ActiveLaserDesignatorComponent, ComponentRemove>(this.OnLaserDesignatorRemove<ComponentRemove>));
    this.SubscribeLocalEvent<ActiveLaserDesignatorComponent, EntityTerminatingEvent>(new EntityEventRefHandler<ActiveLaserDesignatorComponent, EntityTerminatingEvent>(this.OnLaserDesignatorRemove<EntityTerminatingEvent>));
    this.SubscribeLocalEvent<ActiveLaserDesignatorComponent, DroppedEvent>(new EntityEventRefHandler<ActiveLaserDesignatorComponent, DroppedEvent>(this.OnLaserDesignatorDropped<DroppedEvent>));
    this.SubscribeLocalEvent<ActiveLaserDesignatorComponent, RMCDroppedEvent>(new EntityEventRefHandler<ActiveLaserDesignatorComponent, RMCDroppedEvent>(this.OnLaserDesignatorDropped<RMCDroppedEvent>));
    this.SubscribeLocalEvent<ActiveLaserDesignatorComponent, GotUnequippedHandEvent>(new EntityEventRefHandler<ActiveLaserDesignatorComponent, GotUnequippedHandEvent>(this.OnLaserDesignatorDropped<GotUnequippedHandEvent>));
    this.SubscribeLocalEvent<ActiveLaserDesignatorComponent, GotUnequippedEvent>(new EntityEventRefHandler<ActiveLaserDesignatorComponent, GotUnequippedEvent>(this.OnLaserDesignatorUnequipped));
    this.SubscribeLocalEvent<LaserDesignatorTargetComponent, ComponentRemove>(new EntityEventRefHandler<LaserDesignatorTargetComponent, ComponentRemove>(this.OnLaserDesignatorTargetRemove<ComponentRemove>));
    this.SubscribeLocalEvent<LaserDesignatorTargetComponent, EntityTerminatingEvent>(new EntityEventRefHandler<LaserDesignatorTargetComponent, EntityTerminatingEvent>(this.OnLaserDesignatorTargetRemove<EntityTerminatingEvent>));
  }

  private void OnRangefinderMapInit(Entity<RangefinderComponent> rangefinder, ref MapInitEvent args)
  {
    if (this._net.IsClient)
      return;
    RangefinderComponent comp = rangefinder.Comp;
    if (comp.CanDesignate)
      comp.Id = new int?(this._dropshipWeapon.ComputeNextId());
    else
      comp.Mode = RangefinderMode.Rangefinder;
    if (comp.SwitchModeDelay > TimeSpan.Zero)
      this._useDelay.SetLength((Entity<UseDelayComponent>) rangefinder.Owner, comp.SwitchModeDelay, comp.SwitchModeUseDelay);
    if (comp.TargetDelay > TimeSpan.Zero)
      this._useDelay.SetLength((Entity<UseDelayComponent>) rangefinder.Owner, comp.TargetDelay, comp.TargetUseDelay);
    this.Dirty<RangefinderComponent>(rangefinder);
    this.UpdateAppearance(rangefinder);
  }

  private void OnRangefinderAfterInteract(
    Entity<RangefinderComponent> rangefinder,
    ref AfterInteractEvent args)
  {
    EntityUid user = args.User;
    EntityCoordinates grid = args.ClickLocation.SnapToGrid((IEntityManager) this.EntityManager, this._mapManager);
    if (!grid.IsValid((IEntityManager) this.EntityManager))
      return;
    args.Handled = true;
    if (!this._examine.InRangeUnOccluded(user, grid, (float) rangefinder.Comp.Range))
    {
      this._popup.PopupClient(this.Loc.GetString("rmc-laser-designator-out-of-range"), grid, new EntityUid?(user), PopupType.SmallCaution);
    }
    else
    {
      TimeSpan delay = rangefinder.Comp.Delay - (double) this._skills.GetSkill((Entity<SkillsComponent>) user, rangefinder.Comp.Skill) * rangefinder.Comp.TimePerSkillLevel;
      if (delay < rangefinder.Comp.MinimumDelay)
        delay = rangefinder.Comp.MinimumDelay;
      if (!this.HasComp<RMCPlanetComponent>(this._transform.GetGrid(grid)))
        this._popup.PopupClient(this.Loc.GetString("rmc-laser-designator-not-surface"), grid, new EntityUid?(user), PopupType.SmallCaution);
      else if (rangefinder.Comp.Mode == RangefinderMode.Rangefinder)
        this.TryTarget(rangefinder, user, delay, grid);
      else if (this.HasComp<ActiveLaserDesignatorComponent>((EntityUid) rangefinder))
        this._popup.PopupClient(this.Loc.GetString("rmc-laser-designator-already-targeting"), grid, new EntityUid?(user), PopupType.SmallCaution);
      else if (!this._dropshipWeapon.CasDebug && (!this._area.CanCAS(grid) || rangefinder.Comp.Mode == RangefinderMode.Designator && !this._area.CanLase(grid)))
        this._popup.PopupClient(this.Loc.GetString("rmc-laser-designator-not-cas"), grid, new EntityUid?(user), PopupType.SmallCaution);
      else
        this.TryTarget(rangefinder, args.User, delay, grid);
    }
  }

  private void OnRangefinderDoAfter(
    Entity<RangefinderComponent> rangefinder,
    ref LaserDesignatorDoAfterEvent args)
  {
    EntityUid user = args.User;
    if (args.Cancelled || args.Handled)
      return;
    args.Handled = true;
    EntityCoordinates coordinates = this.GetCoordinates(args.Coordinates);
    if (!coordinates.IsValid((IEntityManager) this.EntityManager))
      return;
    if (rangefinder.Comp.Mode == RangefinderMode.Designator)
      this._popup.PopupClient(this.Loc.GetString("rmc-laser-designator-acquired"), coordinates, new EntityUid?(user), PopupType.Medium);
    this._audio.PlayPredicted(rangefinder.Comp.AcquireSound, (EntityUid) rangefinder, new EntityUid?(user));
    if (this._net.IsClient)
      return;
    ActiveLaserDesignatorComponent designatorComponent = this.EnsureComp<ActiveLaserDesignatorComponent>((EntityUid) rangefinder);
    designatorComponent.BreakRange = rangefinder.Comp.BreakRange;
    this.QueueDel(designatorComponent.Target);
    EntProtoId prototype = rangefinder.Comp.Mode == RangefinderMode.Designator ? (EntProtoId) rangefinder.Comp.DesignatorSpawn : rangefinder.Comp.RangefinderSpawn;
    EntityCoordinates moverCoordinates = this._transform.GetMoverCoordinates(coordinates);
    designatorComponent.Target = new EntityUid?(this.Spawn((string) prototype, moverCoordinates));
    designatorComponent.Origin = this._transform.GetMoverCoordinates((EntityUid) rangefinder);
    this.Dirty((EntityUid) rangefinder, (IComponent) designatorComponent);
    if (rangefinder.Comp.Mode == RangefinderMode.Rangefinder)
    {
      MapCoordinates mapCoordinates = this._transform.ToMapCoordinates(moverCoordinates);
      Vector2i vector2i = Vector2Helpers.Floored(mapCoordinates.Position);
      Vector2i offset;
      if (this._rmcPlanet.TryGetOffset(mapCoordinates, out offset))
        vector2i = Vector2i.op_Addition(vector2i, offset);
      rangefinder.Comp.LastTarget = new Vector2i?(vector2i);
      rangefinder.Comp.LastCoords = new MapCoordinates?(mapCoordinates);
      this.Dirty<RangefinderComponent>(rangefinder);
      this._ui.OpenUi((Entity<UserInterfaceComponent>) rangefinder.Owner, (Enum) RangefinderUiKey.Key, new EntityUid?(args.User));
    }
    else
    {
      EntityUid entityUid = designatorComponent.Target.Value;
      LaserDesignatorTargetComponent designatorTargetComponent = this.EnsureComp<LaserDesignatorTargetComponent>(entityUid);
      int id = this.EnsureId(rangefinder);
      designatorTargetComponent.Id = id;
      this.Dirty(entityUid, (IComponent) designatorTargetComponent);
      string str = this.Loc.GetString("rmc-laser-designator-target-name", ("id", (object) id));
      string userAbbreviation = this._dropshipWeapon.GetUserAbbreviation(user, id);
      Entity<SquadTeamComponent> squad;
      if (this._squad.TryGetMemberSquad((Entity<SquadMemberComponent>) user, out squad))
        str = this.Loc.GetString("rmc-laser-designator-target-name-squad", ("squad", (object) squad), ("id", (object) id));
      this._dropshipWeapon.MakeDropshipTarget(entityUid, userAbbreviation);
      this._metaData.SetEntityName(entityUid, str);
    }
  }

  private void OnRangefinderExamined(
    Entity<RangefinderComponent> rangefinder,
    ref ExaminedEvent args)
  {
    using (args.PushGroup("RangefinderComponent"))
    {
      RangefinderComponent comp = rangefinder.Comp;
      Vector2i? lastTarget = comp.LastTarget;
      if (lastTarget.HasValue)
      {
        Vector2i valueOrDefault = lastTarget.GetValueOrDefault();
        args.PushMarkup(this.Loc.GetString("rmc-rangefinder-examine", ("item", (object) rangefinder), ("x", (object) valueOrDefault.X), ("y", (object) valueOrDefault.Y)));
      }
      int? id = comp.Id;
      if (id.HasValue)
      {
        int valueOrDefault = id.GetValueOrDefault();
        args.PushMarkup(this.Loc.GetString("rmc-laser-designator-examine-id", ("id", (object) valueOrDefault)));
      }
      if (!comp.CanDesignate)
        return;
      switch (comp.Mode)
      {
        case RangefinderMode.Rangefinder:
          string markup1 = this.Loc.GetString("rmc-laser-designator-in-rangefinder-mode", ("item", (object) rangefinder));
          args.PushMarkup(markup1);
          break;
        case RangefinderMode.Designator:
          string markup2 = this.Loc.GetString("rmc-laser-designator-in-designator-mode", ("item", (object) rangefinder));
          args.PushMarkup(markup2);
          break;
      }
      args.PushMarkup(this.Loc.GetString("rmc-laser-designator-to-switch"));
    }
  }

  private void OnRangefinderGetAlternativeVerbs(
    Entity<RangefinderComponent> rangefinder,
    ref GetVerbsEvent<AlternativeVerb> args)
  {
    if (!args.CanInteract || !args.CanAccess)
      return;
    RangefinderMode nextMode = rangefinder.Comp.Mode == RangefinderMode.Rangefinder ? RangefinderMode.Designator : RangefinderMode.Rangefinder;
    if (nextMode == RangefinderMode.Designator && !rangefinder.Comp.CanDesignate)
      return;
    SortedSet<AlternativeVerb> verbs = args.Verbs;
    AlternativeVerb alternativeVerb = new AlternativeVerb();
    alternativeVerb.Priority = 100;
    alternativeVerb.Act = (Action) (() => this.ChangeDesignatorMode(rangefinder, nextMode));
    alternativeVerb.Text = this.Loc.GetString("rmc-laser-designator-switch-mode", ("mode", (object) nextMode));
    verbs.Add(alternativeVerb);
  }

  private void OnLaserDesignatorRemove<T>(Entity<ActiveLaserDesignatorComponent> active, ref T args)
  {
    if (this._net.IsClient)
      return;
    this.Del(active.Comp.Target);
  }

  private void OnLaserDesignatorDropped<T>(
    Entity<ActiveLaserDesignatorComponent> active,
    ref T args)
  {
    this.RemCompDeferred<ActiveLaserDesignatorComponent>((EntityUid) active);
  }

  private void OnLaserDesignatorUnequipped(
    Entity<ActiveLaserDesignatorComponent> active,
    ref GotUnequippedEvent args)
  {
    this.RemCompDeferred<ActiveLaserDesignatorComponent>((EntityUid) active);
  }

  private void OnLaserDesignatorTargetRemove<T>(
    Entity<LaserDesignatorTargetComponent> target,
    ref T args)
  {
    ActiveLaserDesignatorComponent comp;
    if (!this.TryComp<ActiveLaserDesignatorComponent>(target.Comp.LaserDesignator, out comp))
      return;
    comp.Target = new EntityUid?();
    this.Dirty(target.Comp.LaserDesignator.Value, (IComponent) comp);
  }

  private void ChangeDesignatorMode(Entity<RangefinderComponent> rangefinder, RangefinderMode mode)
  {
    if (!rangefinder.Comp.CanDesignate)
      return;
    string switchModeUseDelay = rangefinder.Comp.SwitchModeUseDelay;
    UseDelayComponent comp;
    if (this.TryComp<UseDelayComponent>((EntityUid) rangefinder, out comp))
    {
      if (this._useDelay.IsDelayed((Entity<UseDelayComponent>) ((EntityUid) rangefinder, comp), switchModeUseDelay))
        return;
      this._useDelay.TryResetDelay((EntityUid) rangefinder, component: comp, id: switchModeUseDelay);
    }
    if (rangefinder.Comp.DoAfter != null && this._doAfter.IsRunning(new DoAfterId?(rangefinder.Comp.DoAfter.Id)))
      this._doAfter.Cancel(new DoAfterId?(rangefinder.Comp.DoAfter.Id));
    rangefinder.Comp.Mode = mode;
    this.Dirty<RangefinderComponent>(rangefinder);
    this.UpdateAppearance(rangefinder);
  }

  private void UpdateAppearance(Entity<RangefinderComponent> rangefinder)
  {
    this._appearance.SetData((EntityUid) rangefinder, (Enum) RangefinderLayers.Layer, (object) rangefinder.Comp.Mode);
  }

  private int EnsureId(Entity<RangefinderComponent> rangefinder)
  {
    RangefinderComponent comp = rangefinder.Comp;
    comp.Id.GetValueOrDefault();
    if (!comp.Id.HasValue)
    {
      int nextId = this._dropshipWeapon.ComputeNextId();
      comp.Id = new int?(nextId);
    }
    return rangefinder.Comp.Id.Value;
  }

  private void TryTarget(
    Entity<RangefinderComponent> rangefinder,
    EntityUid user,
    TimeSpan delay,
    EntityCoordinates coordinates)
  {
    UseDelayComponent comp;
    if (this.TryComp<UseDelayComponent>((EntityUid) rangefinder, out comp))
    {
      if (this._useDelay.IsDelayed((Entity<UseDelayComponent>) ((EntityUid) rangefinder, comp), rangefinder.Comp.TargetUseDelay))
        return;
      this._useDelay.TryResetDelay((EntityUid) rangefinder, component: comp, id: rangefinder.Comp.TargetUseDelay);
    }
    LaserDesignatorDoAfterEvent @event = new LaserDesignatorDoAfterEvent(this.GetNetCoordinates(coordinates));
    if (!this._doAfter.TryStartDoAfter(new DoAfterArgs((IEntityManager) this.EntityManager, user, delay, (DoAfterEvent) @event, new EntityUid?((EntityUid) rangefinder))
    {
      BreakOnMove = true,
      NeedHand = true,
      BreakOnHandChange = false,
      MovementThreshold = 0.5f
    }))
      return;
    this._popup.PopupClient(this.Loc.GetString("rmc-laser-designator-start"), coordinates, new EntityUid?(user), PopupType.Medium);
    this._audio.PlayPredicted(rangefinder.Comp.TargetSound, (EntityUid) rangefinder, new EntityUid?(user));
    rangefinder.Comp.DoAfter = @event.DoAfter;
  }
}
