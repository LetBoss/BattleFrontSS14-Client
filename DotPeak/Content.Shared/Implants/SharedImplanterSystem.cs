// Decompiled with JetBrains decompiler
// Type: Content.Shared.Implants.SharedImplanterSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.IdentityManagement;
using Content.Shared.Containers.ItemSlots;
using Content.Shared.Damage;
using Content.Shared.Examine;
using Content.Shared.Forensics;
using Content.Shared.IdentityManagement;
using Content.Shared.Implants.Components;
using Content.Shared.Interaction.Events;
using Content.Shared.Popups;
using Content.Shared.Verbs;
using Content.Shared.Whitelist;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Physics.Components;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

#nullable enable
namespace Content.Shared.Implants;

public abstract class SharedImplanterSystem : EntitySystem
{
  [Dependency]
  private SharedContainerSystem _container;
  [Dependency]
  private ItemSlotsSystem _itemSlots;
  [Dependency]
  private SharedAppearanceSystem _appearance;
  [Dependency]
  private SharedPopupSystem _popup;
  [Dependency]
  private EntityWhitelistSystem _whitelistSystem;
  [Dependency]
  private DamageableSystem _damageableSystem;
  [Dependency]
  private SharedUserInterfaceSystem _uiSystem;
  [Dependency]
  private IPrototypeManager _proto;

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<ImplanterComponent, ComponentInit>(new ComponentEventHandler<ImplanterComponent, ComponentInit>(this.OnImplanterInit));
    this.SubscribeLocalEvent<ImplanterComponent, EntInsertedIntoContainerMessage>(new ComponentEventHandler<ImplanterComponent, EntInsertedIntoContainerMessage>(this.OnEntInserted));
    this.SubscribeLocalEvent<ImplanterComponent, ExaminedEvent>(new ComponentEventHandler<ImplanterComponent, ExaminedEvent>(this.OnExamine));
    this.SubscribeLocalEvent<ImplanterComponent, UseInHandEvent>(new ComponentEventHandler<ImplanterComponent, UseInHandEvent>(this.OnUseInHand));
    this.SubscribeLocalEvent<ImplanterComponent, GetVerbsEvent<InteractionVerb>>(new ComponentEventHandler<ImplanterComponent, GetVerbsEvent<InteractionVerb>>(this.OnVerb));
    this.SubscribeLocalEvent<ImplanterComponent, DeimplantChangeVerbMessage>(new ComponentEventHandler<ImplanterComponent, DeimplantChangeVerbMessage>(this.OnSelected));
  }

  private void OnImplanterInit(EntityUid uid, ImplanterComponent component, ComponentInit args)
  {
    EntProtoId? nullable;
    if (component.Implant.HasValue)
    {
      ItemSlot implanterSlot = component.ImplanterSlot;
      nullable = component.Implant;
      string valueOrDefault = nullable.HasValue ? (string) nullable.GetValueOrDefault() : (string) null;
      implanterSlot.StartingItem = valueOrDefault;
    }
    this._itemSlots.AddItemSlot(uid, "implanter_slot", component.ImplanterSlot);
    ImplanterComponent implanterComponent = component;
    nullable = implanterComponent.DeimplantChosen;
    if (!nullable.HasValue)
      implanterComponent.DeimplantChosen = component.DeimplantWhitelist.FirstOrNull<EntProtoId>();
    this.Dirty(uid, (IComponent) component);
  }

  private void OnEntInserted(
    EntityUid uid,
    ImplanterComponent component,
    EntInsertedIntoContainerMessage args)
  {
    MetaDataComponent metaDataComponent = this.Comp<MetaDataComponent>(args.Entity);
    component.ImplantData = (metaDataComponent.EntityName, metaDataComponent.EntityDescription);
  }

  private void OnExamine(EntityUid uid, ImplanterComponent component, ExaminedEvent args)
  {
    if (!component.ImplanterSlot.HasItem || !args.IsInDetailsRange)
      return;
    args.PushMarkup(this.Loc.GetString("implanter-contained-implant-text", ("desc", (object) component.ImplantData.Item2)));
  }

  public bool CheckSameImplant(EntityUid target, EntityUid implant)
  {
    ImplantedComponent comp;
    if (!this.TryComp<ImplantedComponent>(target, out comp))
      return false;
    Robust.Shared.Prototypes.EntityPrototype implantPrototype = this.Prototype(implant);
    return comp.ImplantContainer.ContainedEntities.Any<EntityUid>((Func<EntityUid, bool>) (entity => this.Prototype(entity) == implantPrototype));
  }

  private void OnVerb(
    EntityUid uid,
    ImplanterComponent component,
    GetVerbsEvent<InteractionVerb> args)
  {
    if (!args.CanAccess || !args.CanInteract || component.CurrentMode != ImplanterToggleMode.Draw)
      return;
    SortedSet<InteractionVerb> verbs = args.Verbs;
    InteractionVerb interactionVerb = new InteractionVerb();
    interactionVerb.Text = this.Loc.GetString("implanter-set-draw-verb");
    interactionVerb.Act = (Action) (() => this.TryOpenUi(uid, args.User, component));
    verbs.Add(interactionVerb);
  }

  private void OnUseInHand(EntityUid uid, ImplanterComponent? component, UseInHandEvent args)
  {
    if (!this.Resolve<ImplanterComponent>(uid, ref component) || component.CurrentMode != ImplanterToggleMode.Draw)
      return;
    this.TryOpenUi(uid, args.User, component);
  }

  private void OnSelected(
    EntityUid uid,
    ImplanterComponent component,
    DeimplantChangeVerbMessage args)
  {
    component.DeimplantChosen = (EntProtoId?) args.Implant;
    this.SetSelectedDeimplant(uid, args.Implant, component);
  }

  private void TryOpenUi(EntityUid uid, EntityUid user, ImplanterComponent? component = null)
  {
    if (!this.Resolve<ImplanterComponent>(uid, ref component))
      return;
    this._uiSystem.TryToggleUi((Entity<UserInterfaceComponent>) uid, (Enum) DeimplantUiKey.Key, user);
    ImplanterComponent implanterComponent = component;
    if (!implanterComponent.DeimplantChosen.HasValue)
      implanterComponent.DeimplantChosen = component.DeimplantWhitelist.FirstOrNull<EntProtoId>();
    this.Dirty(uid, (IComponent) component);
  }

  public void Implant(
    EntityUid user,
    EntityUid target,
    EntityUid implanter,
    ImplanterComponent component)
  {
    EntityUid? implant;
    SubdermalImplantComponent implantComp;
    if (!this.CanImplant(user, target, implanter, component, out implant, out implantComp))
      return;
    if (!component.AllowMultipleImplants && this.CheckSameImplant(target, implant.Value))
    {
      IdentityEntity identityEntity = Identity.Name(target, (IEntityManager) this.EntityManager, new EntityUid?(user));
      this._popup.PopupEntity(this.Loc.GetString("implanter-component-implant-already", ("implant", (object) implant), (nameof (target), (object) identityEntity)), target, user);
    }
    else
    {
      Container implantContainer = this.EnsureComp<ImplantedComponent>(target).ImplantContainer;
      if (component.ImplanterSlot.ContainerSlot != null)
        this._container.Remove((Entity<TransformComponent, MetaDataComponent>) implant.Value, (BaseContainer) component.ImplanterSlot.ContainerSlot);
      implantComp.ImplantedEntity = new EntityUid?(target);
      implantContainer.OccludesLight = false;
      this._container.Insert((Entity<TransformComponent, MetaDataComponent, PhysicsComponent>) implant.Value, (BaseContainer) implantContainer);
      if (component.CurrentMode == ImplanterToggleMode.Inject && !component.ImplantOnly)
        this.DrawMode(implanter, component);
      else
        this.ImplantMode(implanter, component);
      TransferDnaEvent args = new TransferDnaEvent()
      {
        Donor = target,
        Recipient = implanter
      };
      this.RaiseLocalEvent<TransferDnaEvent>(target, ref args);
      this.Dirty(implanter, (IComponent) component);
    }
  }

  public bool CanImplant(
    EntityUid user,
    EntityUid target,
    EntityUid implanter,
    ImplanterComponent component,
    [NotNullWhen(true)] out EntityUid? implant,
    [NotNullWhen(true)] out SubdermalImplantComponent? implantComp)
  {
    ref EntityUid? local = ref implant;
    ContainerSlot containerSlot = component.ImplanterSlot.ContainerSlot;
    EntityUid? nullable = containerSlot != null ? containerSlot.ContainedEntities.FirstOrNull<EntityUid>() : new EntityUid?();
    local = nullable;
    if (!this.TryComp<SubdermalImplantComponent>(implant, out implantComp) || !this.CheckTarget(target, component.Whitelist, component.Blacklist) || !this.CheckTarget(target, implantComp.Whitelist, implantComp.Blacklist))
      return false;
    AddImplantAttemptEvent args = new AddImplantAttemptEvent(user, target, implant.Value, implanter);
    this.RaiseLocalEvent<AddImplantAttemptEvent>(target, args);
    return !args.Cancelled;
  }

  protected bool CheckTarget(
    EntityUid target,
    EntityWhitelist? whitelist,
    EntityWhitelist? blacklist)
  {
    return this._whitelistSystem.IsWhitelistPassOrNull(whitelist, target) && this._whitelistSystem.IsBlacklistFailOrNull(blacklist, target);
  }

  public void Draw(
    EntityUid implanter,
    EntityUid user,
    EntityUid target,
    ImplanterComponent component)
  {
    ContainerSlot containerSlot = component.ImplanterSlot.ContainerSlot;
    if (containerSlot == null)
      return;
    bool flag = false;
    BaseContainer container;
    if (this._container.TryGetContainer(target, "implant", out container))
    {
      Robust.Shared.GameObjects.EntityQuery<SubdermalImplantComponent> entityQuery = this.GetEntityQuery<SubdermalImplantComponent>();
      if (component.AllowDeimplantAll)
      {
        foreach (EntityUid containedEntity in (IEnumerable<EntityUid>) container.ContainedEntities)
        {
          SubdermalImplantComponent component1;
          if (entityQuery.TryGetComponent(containedEntity, out component1))
          {
            if (!this._container.CanRemove(containedEntity, container))
            {
              this.DrawPermanentFailurePopup(containedEntity, target, user);
              flag = component1.Permanent;
            }
            else
            {
              this.DrawImplantIntoImplanter(implanter, target, containedEntity, container, containerSlot, component1);
              flag = component1.Permanent;
              break;
            }
          }
        }
        if (component.CurrentMode == ImplanterToggleMode.Draw && !component.ImplantOnly && !flag)
          this.ImplantMode(implanter, component);
      }
      else
      {
        EntityUid? uid = new EntityUid?();
        foreach (EntityUid containedEntity in (IEnumerable<EntityUid>) container.ContainedEntities)
        {
          SubdermalImplantComponent comp;
          if (this.TryComp<SubdermalImplantComponent>(containedEntity, out comp))
          {
            EntProtoId? deimplantChosen = component.DeimplantChosen;
            EntProtoId? nullable = comp.DrawableProtoIdOverride;
            if ((deimplantChosen.HasValue == nullable.HasValue ? (deimplantChosen.HasValue ? (deimplantChosen.GetValueOrDefault() == nullable.GetValueOrDefault() ? 1 : 0) : 1) : 0) == 0)
            {
              if (this.Prototype(containedEntity) != null)
              {
                nullable = component.DeimplantChosen;
                EntProtoId entProtoId = (EntProtoId) this.Prototype(containedEntity);
                if ((nullable.HasValue ? (nullable.GetValueOrDefault() == entProtoId ? 1 : 0) : 0) == 0)
                  continue;
              }
              else
                continue;
            }
            uid = new EntityUid?(containedEntity);
          }
        }
        SubdermalImplantComponent component2;
        if (uid.HasValue && entityQuery.TryGetComponent(uid, out component2))
        {
          bool permanent;
          if (!this._container.CanRemove(uid.Value, container))
          {
            this.DrawPermanentFailurePopup(uid.Value, target, user);
            permanent = component2.Permanent;
          }
          else
          {
            this.DrawImplantIntoImplanter(implanter, target, uid.Value, container, containerSlot, component2);
            permanent = component2.Permanent;
          }
          if (component.CurrentMode == ImplanterToggleMode.Draw && !component.ImplantOnly && !permanent)
            this.ImplantMode(implanter, component);
        }
        else
          this.DrawCatastrophicFailure(implanter, component, user);
      }
      this.Dirty(implanter, (IComponent) component);
    }
    else
      this.DrawCatastrophicFailure(implanter, component, user);
  }

  private void DrawPermanentFailurePopup(EntityUid implant, EntityUid target, EntityUid user)
  {
    this._popup.PopupEntity(this.Loc.GetString("implanter-draw-failed-permanent", (nameof (implant), (object) Identity.Entity(implant, (IEntityManager) this.EntityManager)), (nameof (target), (object) Identity.Entity(target, (IEntityManager) this.EntityManager))), target, user);
  }

  private void DrawImplantIntoImplanter(
    EntityUid implanter,
    EntityUid target,
    EntityUid implant,
    BaseContainer implantContainer,
    ContainerSlot implanterContainer,
    SubdermalImplantComponent implantComp)
  {
    this._container.Remove((Entity<TransformComponent, MetaDataComponent>) implant, implantContainer);
    implantComp.ImplantedEntity = new EntityUid?();
    this._container.Insert((Entity<TransformComponent, MetaDataComponent, PhysicsComponent>) implant, (BaseContainer) implanterContainer);
    TransferDnaEvent args = new TransferDnaEvent()
    {
      Donor = target,
      Recipient = implanter
    };
    this.RaiseLocalEvent<TransferDnaEvent>(target, ref args);
  }

  private void DrawCatastrophicFailure(
    EntityUid implanter,
    ImplanterComponent component,
    EntityUid user)
  {
    this._damageableSystem.TryChangeDamage(new EntityUid?(user), component.DeimplantFailureDamage, true, origin: new EntityUid?(implanter));
    this._popup.PopupEntity(this.Loc.GetString("implanter-draw-failed-catastrophically", (nameof (user), (object) Identity.Entity(user, (IEntityManager) this.EntityManager))), user, PopupType.MediumCaution);
  }

  private void ImplantMode(EntityUid uid, ImplanterComponent component)
  {
    component.CurrentMode = ImplanterToggleMode.Inject;
    this.ChangeOnImplantVisualizer(uid, component);
  }

  private void DrawMode(EntityUid uid, ImplanterComponent component)
  {
    component.CurrentMode = ImplanterToggleMode.Draw;
    this.ChangeOnImplantVisualizer(uid, component);
  }

  private void ChangeOnImplantVisualizer(EntityUid uid, ImplanterComponent component)
  {
    AppearanceComponent comp;
    if (!this.TryComp<AppearanceComponent>(uid, out comp))
      return;
    bool hasItem = component.ImplanterSlot.HasItem;
    if (component.CurrentMode == ImplanterToggleMode.Inject && !component.ImplantOnly)
      this._appearance.SetData(uid, (Enum) ImplanterVisuals.Full, (object) hasItem, comp);
    else if (component.CurrentMode == ImplanterToggleMode.Inject && component.ImplantOnly)
    {
      this._appearance.SetData(uid, (Enum) ImplanterVisuals.Full, (object) hasItem, comp);
      this._appearance.SetData(uid, (Enum) ImplanterImplantOnlyVisuals.ImplantOnly, (object) component.ImplantOnly, comp);
    }
    else
      this._appearance.SetData(uid, (Enum) ImplanterVisuals.Full, (object) hasItem, comp);
  }

  public void SetSelectedDeimplant(EntityUid uid, string? implant, ImplanterComponent? component = null)
  {
    if (!this.Resolve<ImplanterComponent>(uid, ref component, false))
      return;
    Robust.Shared.Prototypes.EntityPrototype prototype;
    if (implant != null && this._proto.TryIndex<Robust.Shared.Prototypes.EntityPrototype>(implant, out prototype))
      component.DeimplantChosen = new EntProtoId?((EntProtoId) prototype);
    this.Dirty(uid, (IComponent) component);
  }
}
