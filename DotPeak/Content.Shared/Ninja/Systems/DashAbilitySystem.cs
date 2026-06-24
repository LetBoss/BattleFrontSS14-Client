// Decompiled with JetBrains decompiler
// Type: Content.Shared.Ninja.Systems.DashAbilitySystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Actions;
using Content.Shared.Charges.Components;
using Content.Shared.Charges.Systems;
using Content.Shared.Examine;
using Content.Shared.Hands.Components;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Interaction;
using Content.Shared.Movement.Pulling.Components;
using Content.Shared.Movement.Pulling.Systems;
using Content.Shared.Ninja.Components;
using Content.Shared.Popups;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Timing;

#nullable enable
namespace Content.Shared.Ninja.Systems;

public sealed class DashAbilitySystem : EntitySystem
{
  [Dependency]
  private ActionContainerSystem _actionContainer;
  [Dependency]
  private IGameTiming _timing;
  [Dependency]
  private SharedChargesSystem _sharedCharges;
  [Dependency]
  private SharedHandsSystem _hands;
  [Dependency]
  private ExamineSystemShared _examine;
  [Dependency]
  private SharedPopupSystem _popup;
  [Dependency]
  private PullingSystem _pullingSystem;
  [Dependency]
  private SharedTransformSystem _transform;

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<DashAbilityComponent, GetItemActionsEvent>(new EntityEventRefHandler<DashAbilityComponent, GetItemActionsEvent>(this.OnGetActions));
    this.SubscribeLocalEvent<DashAbilityComponent, DashEvent>(new EntityEventRefHandler<DashAbilityComponent, DashEvent>(this.OnDash));
    this.SubscribeLocalEvent<DashAbilityComponent, MapInitEvent>(new EntityEventRefHandler<DashAbilityComponent, MapInitEvent>(this.OnMapInit));
  }

  private void OnMapInit(Entity<DashAbilityComponent> ent, ref MapInitEvent args)
  {
    (EntityUid entityUid, DashAbilityComponent comp) = ent;
    this._actionContainer.EnsureAction(entityUid, ref comp.DashActionEntity, (string) comp.DashAction);
    this.Dirty(entityUid, (IComponent) comp);
  }

  private void OnGetActions(Entity<DashAbilityComponent> ent, ref GetItemActionsEvent args)
  {
    if (!this.CheckDash((EntityUid) ent, args.User))
      return;
    args.AddAction(ent.Comp.DashActionEntity);
  }

  private void OnDash(Entity<DashAbilityComponent> ent, ref DashEvent args)
  {
    if (!this._timing.IsFirstTimePredicted)
      return;
    (EntityUid entityUid, DashAbilityComponent _) = ent;
    EntityUid performer = args.Performer;
    if (!this.CheckDash(entityUid, performer))
      return;
    if (!this._hands.IsHolding((Entity<HandsComponent>) performer, new EntityUid?(entityUid), out string _))
      this._popup.PopupClient(this.Loc.GetString("dash-ability-not-held", ("item", (object) entityUid)), performer, new EntityUid?(performer));
    else if (!this._examine.InRangeUnOccluded(this._transform.GetMapCoordinates(performer), this._transform.ToMapCoordinates(args.Target), 100f, (SharedInteractionSystem.Ignored) null))
      this._popup.PopupClient(this.Loc.GetString("dash-ability-cant-see", ("item", (object) entityUid)), performer, new EntityUid?(performer));
    else if (!this._sharedCharges.TryUseCharge((Entity<LimitedChargesComponent>) entityUid))
    {
      this._popup.PopupClient(this.Loc.GetString("dash-ability-no-charges", ("item", (object) entityUid)), performer, new EntityUid?(performer));
    }
    else
    {
      PullableComponent comp1;
      if (this.TryComp<PullableComponent>(performer, out comp1) && this._pullingSystem.IsPulled(performer, comp1))
        this._pullingSystem.TryStopPull(performer, comp1);
      PullerComponent comp2;
      PullableComponent comp3;
      if (this.TryComp<PullerComponent>(performer, out comp2) && this.TryComp<PullableComponent>(comp2.Pulling, out comp3))
        this._pullingSystem.TryStopPull(comp2.Pulling.Value, comp3);
      TransformComponent xform = this.Transform(performer);
      this._transform.SetCoordinates(performer, xform, args.Target);
      this._transform.AttachToGridOrMap(performer, xform);
      args.Handled = true;
    }
  }

  public bool CheckDash(EntityUid uid, EntityUid user)
  {
    CheckDashEvent args = new CheckDashEvent(user);
    this.RaiseLocalEvent<CheckDashEvent>(uid, ref args);
    return !args.Cancelled;
  }
}
