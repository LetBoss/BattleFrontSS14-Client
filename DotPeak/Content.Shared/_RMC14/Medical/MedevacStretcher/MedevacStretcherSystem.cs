// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Medical.MedevacStretcher.MedevacStretcherSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Areas;
using Content.Shared._RMC14.Dropship.Utility.Events;
using Content.Shared._RMC14.Dropship.Weapon;
using Content.Shared._RMC14.Marines.Skills;
using Content.Shared._RMC14.Xenonids;
using Content.Shared.Buckle.Components;
using Content.Shared.Coordinates;
using Content.Shared.Coordinates.Helpers;
using Content.Shared.Examine;
using Content.Shared.Foldable;
using Content.Shared.Interaction;
using Content.Shared.Popups;
using Content.Shared.Verbs;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable
namespace Content.Shared._RMC14.Medical.MedevacStretcher;

public sealed class MedevacStretcherSystem : EntitySystem
{
  [Dependency]
  private SharedAppearanceSystem _appearance;
  [Dependency]
  private AreaSystem _areas;
  [Dependency]
  private SharedDropshipWeaponSystem _dropshipWeapon;
  [Dependency]
  private IMapManager _mapManager;
  [Dependency]
  private INetManager _net;
  [Dependency]
  private SharedPopupSystem _popup;
  [Dependency]
  private SkillsSystem _skills;
  [Dependency]
  private IGameTiming _timing;
  [Dependency]
  private SharedTransformSystem _transform;
  private static readonly EntProtoId<SkillDefinitionComponent> SkillType = (EntProtoId<SkillDefinitionComponent>) "RMCSkillMedical";
  private const int MinimumRequiredSkill = 1;

  public override void Initialize()
  {
    this.SubscribeLocalEvent<MedevacStretcherComponent, GetVerbsEvent<InteractionVerb>>(new EntityEventRefHandler<MedevacStretcherComponent, GetVerbsEvent<InteractionVerb>>(this.AddActivateBeaconVerb));
    this.SubscribeLocalEvent<MedevacStretcherComponent, FoldedEvent>(new EntityEventRefHandler<MedevacStretcherComponent, FoldedEvent>(this.OnFold));
    this.SubscribeLocalEvent<MedevacStretcherComponent, PrepareMedevacEvent>(new EntityEventRefHandler<MedevacStretcherComponent, PrepareMedevacEvent>(this.PrepareMedevac));
    this.SubscribeLocalEvent<MedevacStretcherComponent, ExaminedEvent>(new EntityEventRefHandler<MedevacStretcherComponent, ExaminedEvent>(this.OnExamine));
    this.SubscribeLocalEvent<MedevacStretcherComponent, InteractHandEvent>(new EntityEventRefHandler<MedevacStretcherComponent, InteractHandEvent>(this.OnInteract));
    this.SubscribeLocalEvent<MedevacStretcherComponent, StrappedEvent>(new EntityEventRefHandler<MedevacStretcherComponent, StrappedEvent>(this.OnStrapped<StrappedEvent>));
    this.SubscribeLocalEvent<MedevacStretcherComponent, UnstrappedEvent>(new EntityEventRefHandler<MedevacStretcherComponent, UnstrappedEvent>(this.OnStrapped<UnstrappedEvent>));
  }

  public void Medevac(Entity<MedevacStretcherComponent> ent, EntityUid medevacEntity)
  {
    StrapComponent comp;
    if (this._net.IsClient || !this.TryComp<StrapComponent>(ent.Owner, out comp) || comp.BuckledEntities.Count == 0)
      return;
    foreach (EntityUid buckledEntity in comp.BuckledEntities)
      this._transform.PlaceNextTo((Entity<TransformComponent>) buckledEntity, (Entity<TransformComponent>) medevacEntity);
    this._appearance.SetData((EntityUid) ent, (Enum) MedevacStretcherVisuals.MedevacingState, (object) false);
  }

  private void OnExamine(Entity<MedevacStretcherComponent> ent, ref ExaminedEvent args)
  {
    StrapComponent comp;
    if (!this.TryComp<StrapComponent>((EntityUid) ent, out comp) || comp.BuckledEntities.Count == 0)
      return;
    string str = this.Name(comp.BuckledEntities.First<EntityUid>());
    using (args.PushGroup("MedevacStretcherComponent"))
      args.PushText(this.Loc.GetString("rmc-medevac-stretcher-examine-id", ("id", (object) str)));
  }

  private void AddActivateBeaconVerb(
    Entity<MedevacStretcherComponent> ent,
    ref GetVerbsEvent<InteractionVerb> args)
  {
    if (!args.CanAccess || !args.CanInteract || args.Hands == null || !this._skills.HasSkill((Entity<SkillsComponent>) args.User, MedevacStretcherSystem.SkillType, 1))
      return;
    GetVerbsEvent<InteractionVerb> @event = args;
    SortedSet<InteractionVerb> verbs = args.Verbs;
    InteractionVerb interactionVerb = new InteractionVerb();
    interactionVerb.Text = this.Loc.GetString("rmc-medevac-toggle-beacon-verb");
    interactionVerb.Act = (Action) (() => this.ToggleBeacon(@event.Target, @event.User));
    interactionVerb.Priority = 1;
    verbs.Add(interactionVerb);
  }

  private void OnFold(Entity<MedevacStretcherComponent> ent, ref FoldedEvent args)
  {
    if (this._timing.ApplyingState)
      return;
    if (args.IsFolded)
      this.DeactivateBeacon(ent.Owner);
    else
      this.ActivateBeacon(ent.Owner, new EntityUid?());
  }

  private void PrepareMedevac(Entity<MedevacStretcherComponent> ent, ref PrepareMedevacEvent args)
  {
    StrapComponent comp;
    if (!this.TryComp<StrapComponent>(ent.Owner, out comp) || comp.BuckledEntities.Count == 0)
      return;
    this._appearance.SetData(ent.Owner, (Enum) MedevacStretcherVisuals.MedevacingState, (object) true);
    args.ReadyForMedevac = true;
  }

  private void OnInteract(Entity<MedevacStretcherComponent> ent, ref InteractHandEvent args)
  {
    FoldableComponent comp;
    if (this.HasComp<XenoComponent>(args.User) || args.Handled || this.TryComp<FoldableComponent>(ent.Owner, out comp) && comp.IsFolded)
      return;
    this.ToggleBeacon(args.Target, args.User);
    args.Handled = true;
  }

  private void OnStrapped<T>(Entity<MedevacStretcherComponent> ent, ref T args)
  {
    DropshipTargetComponent comp;
    if (!this.TryComp<DropshipTargetComponent>((EntityUid) ent, out comp))
      return;
    comp.Abbreviation = this.GetName((Entity<StrapComponent>) ent.Owner);
    this.Dirty((EntityUid) ent, (IComponent) comp);
    this._dropshipWeapon.TargetUpdated((Entity<DropshipTargetComponent>) ((EntityUid) ent, comp));
  }

  private void ToggleBeacon(EntityUid stretcher, EntityUid user)
  {
    if (this.HasComp<DropshipTargetComponent>(stretcher))
      this.DeactivateBeacon(stretcher);
    else
      this.ActivateBeacon(stretcher, new EntityUid?(user));
  }

  private void ActivateBeacon(EntityUid stretcher, EntityUid? user)
  {
    if (this.HasComp<DropshipTargetComponent>(stretcher))
      return;
    EntityCoordinates coordinates = stretcher.ToCoordinates();
    EntityCoordinates grid = stretcher.ToCoordinates().SnapToGrid((IEntityManager) this.EntityManager, this._mapManager);
    Entity<AreaComponent>? area;
    if (!this._dropshipWeapon.CasDebug && (!this._areas.TryGetArea(grid, out area, out EntityPrototype _) || !area.Value.Comp.Medevac))
    {
      this._popup.PopupClient(this.Loc.GetString("rmc-medevac-area-not-cas"), coordinates, user);
    }
    else
    {
      if (!this.HasComp<MedevacStretcherComponent>(stretcher))
        return;
      string name = this.GetName((Entity<StrapComponent>) stretcher);
      this._dropshipWeapon.MakeTarget(stretcher, name, false);
      this._appearance.SetData(stretcher, (Enum) MedevacStretcherVisuals.BeaconState, (object) BeaconVisuals.On);
      this._popup.PopupClient(this.Loc.GetString("rmc-medevac-activate-beacon"), coordinates, user);
    }
  }

  private void DeactivateBeacon(EntityUid stretcher)
  {
    if (!this.HasComp<DropshipTargetComponent>(stretcher))
      return;
    this.RemCompDeferred<DropshipTargetComponent>(stretcher);
    this._appearance.SetData(stretcher, (Enum) MedevacStretcherVisuals.BeaconState, (object) BeaconVisuals.Off);
    this._appearance.SetData(stretcher, (Enum) MedevacStretcherVisuals.MedevacingState, (object) false);
  }

  private string GetName(Entity<StrapComponent?> stretcher)
  {
    return !this.Resolve<StrapComponent>((EntityUid) stretcher, ref stretcher.Comp, false) || stretcher.Comp.BuckledEntities.Count <= 0 ? "Empty" : this.Name(stretcher.Comp.BuckledEntities.First<EntityUid>());
  }
}
