// Decompiled with JetBrains decompiler
// Type: Content.Shared.Body.Systems.SharedInternalsSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Xenonids;
using Content.Shared.Alert;
using Content.Shared.Atmos.Components;
using Content.Shared.Atmos.EntitySystems;
using Content.Shared.Body.Components;
using Content.Shared.DoAfter;
using Content.Shared.Hands.Components;
using Content.Shared.IdentityManagement;
using Content.Shared.Internals;
using Content.Shared.Inventory;
using Content.Shared.Popups;
using Content.Shared.Verbs;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Utility;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared.Body.Systems;

public abstract class SharedInternalsSystem : EntitySystem
{
  [Dependency]
  private AlertsSystem _alerts;
  [Dependency]
  private InventorySystem _inventory;
  [Dependency]
  private SharedDoAfterSystem _doAfter;
  [Dependency]
  private SharedGasTankSystem _gasTank;
  [Dependency]
  private SharedPopupSystem _popupSystem;

  public virtual void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<InternalsComponent, GetVerbsEvent<InteractionVerb>>(new EntityEventRefHandler<InternalsComponent, GetVerbsEvent<InteractionVerb>>((object) this, __methodptr(OnGetInteractionVerbs)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<InternalsComponent, ComponentStartup>(new EntityEventRefHandler<InternalsComponent, ComponentStartup>((object) this, __methodptr(OnInternalsStartup)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<InternalsComponent, ComponentShutdown>(new EntityEventRefHandler<InternalsComponent, ComponentShutdown>((object) this, __methodptr(OnInternalsShutdown)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<InternalsComponent, InternalsDoAfterEvent>(new EntityEventRefHandler<InternalsComponent, InternalsDoAfterEvent>((object) this, __methodptr(OnDoAfter)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<InternalsComponent, ToggleInternalsAlertEvent>(new EntityEventRefHandler<InternalsComponent, ToggleInternalsAlertEvent>((object) this, __methodptr(OnToggleInternalsAlert)), (Type[]) null, (Type[]) null);
  }

  private void OnGetInteractionVerbs(
    Entity<InternalsComponent> ent,
    ref GetVerbsEvent<InteractionVerb> args)
  {
    if (this.HasComp<XenoComponent>(args.User) || !args.CanAccess || !args.CanInteract || args.Hands == null || !this.AreInternalsWorking(Entity<InternalsComponent>.op_Implicit(ent)) && ent.Comp.BreathTools.Count == 0)
      return;
    EntityUid user = args.User;
    InteractionVerb interactionVerb1 = new InteractionVerb();
    interactionVerb1.Icon = (SpriteSpecifier) new SpriteSpecifier.Texture(new ResPath("/Textures/Interface/VerbIcons/dot.svg.192dpi.png"));
    InteractionVerb interactionVerb2 = interactionVerb1;
    if (this.AreInternalsWorking(Entity<InternalsComponent>.op_Implicit(ent)))
    {
      interactionVerb2.Act = (Action) (() => this.ToggleInternals(Entity<InternalsComponent>.op_Implicit(ent), user, false, Entity<InternalsComponent>.op_Implicit(ent), ToggleMode.Off));
      interactionVerb2.Message = this.Loc.GetString("action-description-internals-toggle-off");
      interactionVerb2.Text = this.Loc.GetString("action-name-internals-toggle-off");
    }
    else
    {
      interactionVerb2.Act = (Action) (() => this.ToggleInternals(Entity<InternalsComponent>.op_Implicit(ent), user, false, Entity<InternalsComponent>.op_Implicit(ent), ToggleMode.On));
      interactionVerb2.Message = this.Loc.GetString("action-description-internals-toggle-on");
      interactionVerb2.Text = this.Loc.GetString("action-name-internals-toggle-on");
    }
    args.Verbs.Add(interactionVerb2);
  }

  protected bool ToggleInternals(
    EntityUid target,
    EntityUid user,
    bool force,
    InternalsComponent? internals = null,
    ToggleMode mode = ToggleMode.Toggle)
  {
    if (!this.Resolve<InternalsComponent>(target, ref internals, false))
      return false;
    if (internals.BreathTools.Count == 0)
    {
      this._popupSystem.PopupClient(EntityUid.op_Equality(user, target) ? this.Loc.GetString("internals-self-no-breath-tool") : this.Loc.GetString("internals-other-no-breath-tool", ("ent", (object) Identity.Name(target, (IEntityManager) this.EntityManager, new EntityUid?(user)))), target, new EntityUid?(user));
      return false;
    }
    Entity<GasTankComponent>? bestGasTank = this.FindBestGasTank(Entity<HandsComponent, InventoryComponent, ContainerManagerComponent>.op_Implicit(target));
    if (!bestGasTank.HasValue)
    {
      this._popupSystem.PopupClient(EntityUid.op_Equality(user, target) ? this.Loc.GetString("internals-self-no-tank") : this.Loc.GetString("internals-other-no-tank", ("ent", (object) Identity.Name(target, (IEntityManager) this.EntityManager, new EntityUid?(user)))), target, new EntityUid?(user));
      return false;
    }
    if (!force && EntityUid.op_Inequality(user, target))
      return this.StartToggleInternalsDoAfter(user, Entity<InternalsComponent>.op_Implicit((target, internals)), mode);
    GasTankComponent gasTankComponent;
    return this.TryComp<GasTankComponent>(internals.GasTankEntity, ref gasTankComponent) ? mode != ToggleMode.On && this._gasTank.DisconnectFromInternals(Entity<GasTankComponent>.op_Implicit((internals.GasTankEntity.Value, gasTankComponent)), new EntityUid?(user)) : mode != ToggleMode.Off && this._gasTank.ConnectToInternals(bestGasTank.Value, new EntityUid?(user));
  }

  private bool StartToggleInternalsDoAfter(
    EntityUid user,
    Entity<InternalsComponent> targetEnt,
    ToggleMode mode)
  {
    TimeSpan delay = !EntityUid.op_Equality(user, targetEnt.Owner) ? targetEnt.Comp.Delay : TimeSpan.Zero;
    return this._doAfter.TryStartDoAfter(new DoAfterArgs((IEntityManager) this.EntityManager, user, delay, (DoAfterEvent) new InternalsDoAfterEvent(mode), new EntityUid?(Entity<InternalsComponent>.op_Implicit(targetEnt)), new EntityUid?(Entity<InternalsComponent>.op_Implicit(targetEnt)))
    {
      BreakOnDamage = true,
      BreakOnMove = true,
      MovementThreshold = 0.1f
    });
  }

  private void OnDoAfter(Entity<InternalsComponent> ent, ref InternalsDoAfterEvent args)
  {
    if (args.Cancelled || args.Handled)
      return;
    this.ToggleInternals(Entity<InternalsComponent>.op_Implicit(ent), args.User, true, Entity<InternalsComponent>.op_Implicit(ent), args.ToggleMode);
    args.Handled = true;
  }

  private void OnToggleInternalsAlert(
    Entity<InternalsComponent> ent,
    ref ToggleInternalsAlertEvent args)
  {
    if (args.Handled)
      return;
    args.Handled |= this.ToggleInternals(Entity<InternalsComponent>.op_Implicit(ent), Entity<InternalsComponent>.op_Implicit(ent), false, ent.Comp);
  }

  private void OnInternalsStartup(Entity<InternalsComponent> ent, ref ComponentStartup args)
  {
    this._alerts.ShowAlert(Entity<InternalsComponent>.op_Implicit(ent), ent.Comp.InternalsAlert, new short?(this.GetSeverity(Entity<InternalsComponent>.op_Implicit(ent))));
  }

  private void OnInternalsShutdown(Entity<InternalsComponent> ent, ref ComponentShutdown args)
  {
    this._alerts.ClearAlert(Entity<InternalsComponent>.op_Implicit(ent), ent.Comp.InternalsAlert);
  }

  public void ConnectBreathTool(Entity<InternalsComponent> ent, EntityUid toolEntity)
  {
    if (!ent.Comp.BreathTools.Add(toolEntity))
      return;
    BreathToolComponent breathToolComponent;
    if (this.TryComp<BreathToolComponent>(toolEntity, ref breathToolComponent))
    {
      breathToolComponent.ConnectedInternalsEntity = new EntityUid?(ent.Owner);
      this.Dirty(toolEntity, (IComponent) breathToolComponent, (MetaDataComponent) null);
    }
    this.Dirty<InternalsComponent>(ent, (MetaDataComponent) null);
    this._alerts.ShowAlert(Entity<InternalsComponent>.op_Implicit(ent), ent.Comp.InternalsAlert, new short?(this.GetSeverity(Entity<InternalsComponent>.op_Implicit(ent))));
  }

  public void DisconnectBreathTool(
    Entity<InternalsComponent> ent,
    EntityUid toolEntity,
    bool forced = false)
  {
    if (!ent.Comp.BreathTools.Remove(toolEntity))
      return;
    this.Dirty<InternalsComponent>(ent, (MetaDataComponent) null);
    BreathToolComponent breathToolComponent;
    if (this.TryComp<BreathToolComponent>(toolEntity, ref breathToolComponent))
    {
      breathToolComponent.ConnectedInternalsEntity = new EntityUid?();
      this.Dirty(toolEntity, (IComponent) breathToolComponent, (MetaDataComponent) null);
    }
    if (ent.Comp.BreathTools.Count == 0)
      this.DisconnectTank(ent, forced);
    this._alerts.ShowAlert(Entity<InternalsComponent>.op_Implicit(ent), ent.Comp.InternalsAlert, new short?(this.GetSeverity(Entity<InternalsComponent>.op_Implicit(ent))));
  }

  public void DisconnectTank(Entity<InternalsComponent> ent, bool forced = false)
  {
    GasTankComponent gasTankComponent;
    if (this.TryComp<GasTankComponent>(ent.Comp.GasTankEntity, ref gasTankComponent))
    {
      SharedGasTankSystem gasTank = this._gasTank;
      Entity<GasTankComponent> ent1 = Entity<GasTankComponent>.op_Implicit((ent.Comp.GasTankEntity.Value, gasTankComponent));
      bool flag = forced;
      EntityUid? user = new EntityUid?();
      int num = flag ? 1 : 0;
      gasTank.DisconnectFromInternals(ent1, user, num != 0);
    }
    ent.Comp.GasTankEntity = new EntityUid?();
    this.Dirty<InternalsComponent>(ent, (MetaDataComponent) null);
    this._alerts.ShowAlert(ent.Owner, ent.Comp.InternalsAlert, new short?(this.GetSeverity(ent.Comp)));
  }

  public bool TryConnectTank(Entity<InternalsComponent> ent, EntityUid tankEntity)
  {
    if (ent.Comp.BreathTools.Count == 0)
      return false;
    GasTankComponent gasTankComponent;
    if (this.TryComp<GasTankComponent>(ent.Comp.GasTankEntity, ref gasTankComponent))
      this._gasTank.DisconnectFromInternals(Entity<GasTankComponent>.op_Implicit((ent.Comp.GasTankEntity.Value, gasTankComponent)));
    ent.Comp.GasTankEntity = new EntityUid?(tankEntity);
    this.Dirty<InternalsComponent>(ent, (MetaDataComponent) null);
    this._alerts.ShowAlert(Entity<InternalsComponent>.op_Implicit(ent), ent.Comp.InternalsAlert, new short?(this.GetSeverity(Entity<InternalsComponent>.op_Implicit(ent))));
    return true;
  }

  public bool AreInternalsWorking(EntityUid uid, InternalsComponent? component = null)
  {
    return this.Resolve<InternalsComponent>(uid, ref component, false) && this.AreInternalsWorking(component);
  }

  public bool AreInternalsWorking(InternalsComponent component)
  {
    BreathToolComponent breathToolComponent;
    return this.TryComp<BreathToolComponent>(Extensions.FirstOrNull<EntityUid>((IEnumerable<EntityUid>) component.BreathTools), ref breathToolComponent) && breathToolComponent.IsFunctional && this.HasComp<GasTankComponent>(component.GasTankEntity);
  }

  protected short GetSeverity(InternalsComponent component)
  {
    if (component.BreathTools.Count == 0 || !this.AreInternalsWorking(component))
      return 2;
    GasTankComponent gasTankComponent;
    return this.TryComp<GasTankComponent>(component.GasTankEntity, ref gasTankComponent) && gasTankComponent.IsLowPressure ? (short) 0 : (short) 1;
  }

  public Entity<GasTankComponent>? FindBestGasTank(
    Entity<HandsComponent?, InventoryComponent?, ContainerManagerComponent?> user)
  {
    if (!this.Resolve<InventoryComponent, ContainerManagerComponent>(Entity<HandsComponent, InventoryComponent, ContainerManagerComponent>.op_Implicit(user), ref user.Comp2, ref user.Comp3, true))
      return new Entity<GasTankComponent>?();
    EntityUid? entityUid1;
    GasTankComponent gasTankComponent1;
    if (this._inventory.TryGetSlotEntity(Entity<HandsComponent, InventoryComponent, ContainerManagerComponent>.op_Implicit(user), "back", out entityUid1, user.Comp2, user.Comp3) && this.TryComp<GasTankComponent>(entityUid1, ref gasTankComponent1) && this._gasTank.CanConnectToInternals(Entity<GasTankComponent>.op_Implicit((entityUid1.Value, gasTankComponent1))))
      return new Entity<GasTankComponent>?(Entity<GasTankComponent>.op_Implicit((entityUid1.Value, gasTankComponent1)));
    EntityUid? entityUid2;
    GasTankComponent gasTankComponent2;
    if (this._inventory.TryGetSlotEntity(Entity<HandsComponent, InventoryComponent, ContainerManagerComponent>.op_Implicit(user), "suitstorage", out entityUid2, user.Comp2, user.Comp3) && this.TryComp<GasTankComponent>(entityUid2, ref gasTankComponent2) && this._gasTank.CanConnectToInternals(Entity<GasTankComponent>.op_Implicit((entityUid2.Value, gasTankComponent2))))
      return new Entity<GasTankComponent>?(Entity<GasTankComponent>.op_Implicit((entityUid2.Value, gasTankComponent2)));
    foreach (EntityUid orInventoryEntity in this._inventory.GetHandOrInventoryEntities(Entity<HandsComponent, InventoryComponent>.op_Implicit((user.Owner, user.Comp1, user.Comp2))))
    {
      if (this.TryComp<GasTankComponent>(orInventoryEntity, ref gasTankComponent2) && this._gasTank.CanConnectToInternals(Entity<GasTankComponent>.op_Implicit((orInventoryEntity, gasTankComponent2))))
        return new Entity<GasTankComponent>?(Entity<GasTankComponent>.op_Implicit((orInventoryEntity, gasTankComponent2)));
    }
    return new Entity<GasTankComponent>?();
  }
}
