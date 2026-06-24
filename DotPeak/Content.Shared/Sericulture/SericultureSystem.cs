// Decompiled with JetBrains decompiler
// Type: Content.Shared.Sericulture.SharedSericultureSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Actions;
using Content.Shared.Actions.Components;
using Content.Shared.Cloning.Events;
using Content.Shared.DoAfter;
using Content.Shared.Nutrition.Components;
using Content.Shared.Nutrition.EntitySystems;
using Content.Shared.Popups;
using Content.Shared.Stacks;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Network;

#nullable enable
namespace Content.Shared.Sericulture;

public abstract class SharedSericultureSystem : EntitySystem
{
  [Dependency]
  private INetManager _netManager;
  [Dependency]
  private SharedActionsSystem _actionsSystem;
  [Dependency]
  private SharedDoAfterSystem _doAfterSystem;
  [Dependency]
  private HungerSystem _hungerSystem;
  [Dependency]
  private SharedPopupSystem _popupSystem;
  [Dependency]
  private SharedStackSystem _stackSystem;

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<SericultureComponent, MapInitEvent>(new ComponentEventHandler<SericultureComponent, MapInitEvent>(this.OnMapInit));
    this.SubscribeLocalEvent<SericultureComponent, ComponentShutdown>(new ComponentEventHandler<SericultureComponent, ComponentShutdown>(this.OnCompRemove));
    this.SubscribeLocalEvent<SericultureComponent, SericultureActionEvent>(new ComponentEventHandler<SericultureComponent, SericultureActionEvent>(this.OnSericultureStart));
    this.SubscribeLocalEvent<SericultureComponent, SericultureDoAfterEvent>(new ComponentEventHandler<SericultureComponent, SericultureDoAfterEvent>(this.OnSericultureDoAfter));
    this.SubscribeLocalEvent<SericultureComponent, CloningEvent>(new EntityEventRefHandler<SericultureComponent, CloningEvent>(this.OnClone));
  }

  private void OnClone(Entity<SericultureComponent> ent, ref CloningEvent args)
  {
    if (!args.Settings.EventComponents.Contains(this.Factory.GetRegistration(ent.Comp.GetType()).Name))
      return;
    SericultureComponent sericultureComponent = this.EnsureComp<SericultureComponent>(args.CloneUid);
    sericultureComponent.PopupText = ent.Comp.PopupText;
    sericultureComponent.ProductionLength = ent.Comp.ProductionLength;
    sericultureComponent.HungerCost = ent.Comp.HungerCost;
    sericultureComponent.EntityProduced = ent.Comp.EntityProduced;
    sericultureComponent.MinHungerThreshold = ent.Comp.MinHungerThreshold;
    this.Dirty(args.CloneUid, (IComponent) sericultureComponent);
  }

  private void OnMapInit(EntityUid uid, SericultureComponent comp, MapInitEvent args)
  {
    this._actionsSystem.AddAction(uid, ref comp.ActionEntity, (string) comp.Action);
  }

  private void OnCompRemove(EntityUid uid, SericultureComponent comp, ComponentShutdown args)
  {
    SharedActionsSystem actionsSystem = this._actionsSystem;
    Entity<ActionsComponent> performer = (Entity<ActionsComponent>) uid;
    EntityUid? actionEntity = comp.ActionEntity;
    Entity<ActionComponent>? action = actionEntity.HasValue ? new Entity<ActionComponent>?((Entity<ActionComponent>) actionEntity.GetValueOrDefault()) : new Entity<ActionComponent>?();
    actionsSystem.RemoveAction(performer, action);
  }

  private void OnSericultureStart(
    EntityUid uid,
    SericultureComponent comp,
    SericultureActionEvent args)
  {
    HungerComponent comp1;
    if (this.TryComp<HungerComponent>(uid, out comp1) && this._hungerSystem.IsHungerBelowState(uid, comp.MinHungerThreshold, new float?(this._hungerSystem.GetHunger(comp1) - comp.HungerCost), comp1))
      this._popupSystem.PopupClient(this.Loc.GetString(comp.PopupText), uid, new EntityUid?(uid));
    else
      this._doAfterSystem.TryStartDoAfter(new DoAfterArgs((IEntityManager) this.EntityManager, uid, comp.ProductionLength, (DoAfterEvent) new SericultureDoAfterEvent(), new EntityUid?(uid))
      {
        BreakOnMove = true,
        BlockDuplicate = true,
        BreakOnDamage = true,
        CancelDuplicate = true
      });
  }

  private void OnSericultureDoAfter(
    EntityUid uid,
    SericultureComponent comp,
    SericultureDoAfterEvent args)
  {
    if (args.Cancelled || args.Handled || comp.Deleted)
      return;
    HungerComponent comp1;
    if (this.TryComp<HungerComponent>(uid, out comp1) && this._hungerSystem.IsHungerBelowState(uid, comp.MinHungerThreshold, new float?(this._hungerSystem.GetHunger(comp1) - comp.HungerCost), comp1))
    {
      this._popupSystem.PopupClient(this.Loc.GetString(comp.PopupText), uid, new EntityUid?(uid));
    }
    else
    {
      this._hungerSystem.ModifyHunger(uid, -comp.HungerCost);
      if (!this._netManager.IsClient)
        this._stackSystem.TryMergeToHands(this.Spawn((string) comp.EntityProduced, this.Transform(uid).Coordinates), uid);
      args.Repeat = true;
    }
  }
}
