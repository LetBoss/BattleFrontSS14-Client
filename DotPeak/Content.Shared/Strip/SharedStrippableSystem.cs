// Decompiled with JetBrains decompiler
// Type: Content.Shared.Strip.SharedStrippableSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Clothing;
using Content.Shared._RMC14.Marines.Skills;
using Content.Shared.Administration.Logs;
using Content.Shared.CombatMode;
using Content.Shared.Cuffs;
using Content.Shared.Cuffs.Components;
using Content.Shared.Database;
using Content.Shared.DoAfter;
using Content.Shared.DragDrop;
using Content.Shared.Hands.Components;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.IdentityManagement;
using Content.Shared.Interaction;
using Content.Shared.Interaction.Components;
using Content.Shared.Interaction.Events;
using Content.Shared.Inventory;
using Content.Shared.Inventory.VirtualItem;
using Content.Shared.Popups;
using Content.Shared.Strip.Components;
using Content.Shared.Verbs;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;
using System;
using System.Linq;

#nullable enable
namespace Content.Shared.Strip;

public abstract class SharedStrippableSystem : EntitySystem
{
  [Dependency]
  private SharedInteractionSystem _interactionSystem;
  [Dependency]
  private SharedUserInterfaceSystem _ui;
  [Dependency]
  private InventorySystem _inventorySystem;
  [Dependency]
  private SharedCuffableSystem _cuffableSystem;
  [Dependency]
  private SharedDoAfterSystem _doAfterSystem;
  [Dependency]
  private SharedHandsSystem _handsSystem;
  [Dependency]
  private SharedPopupSystem _popupSystem;
  [Dependency]
  private ISharedAdminLogManager _adminLogger;
  [Dependency]
  private SkillsSystem _skills;
  private static readonly EntProtoId<SkillDefinitionComponent> MultiStripSkill = (EntProtoId<SkillDefinitionComponent>) "RMCSkillPolice";
  private const int MultiStripSkillLevel = 2;

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<StrippableComponent, GetVerbsEvent<Verb>>(new ComponentEventHandler<StrippableComponent, GetVerbsEvent<Verb>>(this.AddStripVerb));
    this.SubscribeLocalEvent<StrippableComponent, GetVerbsEvent<ExamineVerb>>(new ComponentEventHandler<StrippableComponent, GetVerbsEvent<ExamineVerb>>(this.AddStripExamineVerb));
    this.SubscribeLocalEvent<StrippableComponent, StrippingSlotButtonPressed>(new EntityEventRefHandler<StrippableComponent, StrippingSlotButtonPressed>(this.OnStripButtonPressed));
    this.SubscribeLocalEvent<HandsComponent, DoAfterAttemptEvent<StrippableDoAfterEvent>>(new EntityEventRefHandler<HandsComponent, DoAfterAttemptEvent<StrippableDoAfterEvent>>(this.OnStrippableDoAfterRunning));
    this.SubscribeLocalEvent<HandsComponent, StrippableDoAfterEvent>(new EntityEventRefHandler<HandsComponent, StrippableDoAfterEvent>(this.OnStrippableDoAfterFinished));
    this.SubscribeLocalEvent<StrippingComponent, CanDropTargetEvent>(new ComponentEventRefHandler<StrippingComponent, CanDropTargetEvent>(this.OnCanDropOn));
    this.SubscribeLocalEvent<StrippableComponent, CanDropDraggedEvent>(new ComponentEventRefHandler<StrippableComponent, CanDropDraggedEvent>(this.OnCanDrop));
    this.SubscribeLocalEvent<StrippableComponent, DragDropDraggedEvent>(new ComponentEventRefHandler<StrippableComponent, DragDropDraggedEvent>(this.OnDragDrop));
    this.SubscribeLocalEvent<StrippableComponent, ActivateInWorldEvent>(new ComponentEventHandler<StrippableComponent, ActivateInWorldEvent>(this.OnActivateInWorld));
  }

  private void AddStripVerb(EntityUid uid, StrippableComponent component, GetVerbsEvent<Verb> args)
  {
    if (args.Hands == null || !args.CanAccess || !args.CanInteract || args.Target == args.User)
      return;
    Verb verb = new Verb()
    {
      Text = this.Loc.GetString("strip-verb-get-data-text"),
      Icon = (SpriteSpecifier) new SpriteSpecifier.Texture(new ResPath("/Textures/Interface/VerbIcons/outfit.svg.192dpi.png")),
      Act = (Action) (() => this.TryOpenStrippingUi(args.User, (Entity<StrippableComponent>) (uid, component), true))
    };
    args.Verbs.Add(verb);
  }

  private void AddStripExamineVerb(
    EntityUid uid,
    StrippableComponent component,
    GetVerbsEvent<ExamineVerb> args)
  {
    if (args.Hands == null || !args.CanAccess || !args.CanInteract || args.Target == args.User)
      return;
    ExamineVerb examineVerb1 = new ExamineVerb();
    examineVerb1.Text = this.Loc.GetString("strip-verb-get-data-text");
    examineVerb1.Icon = (SpriteSpecifier) new SpriteSpecifier.Texture(new ResPath("/Textures/Interface/VerbIcons/outfit.svg.192dpi.png"));
    examineVerb1.Act = (Action) (() => this.TryOpenStrippingUi(args.User, (Entity<StrippableComponent>) (uid, component), true));
    examineVerb1.Category = VerbCategory.Examine;
    ExamineVerb examineVerb2 = examineVerb1;
    args.Verbs.Add(examineVerb2);
  }

  private void OnStripButtonPressed(
    Entity<StrippableComponent> strippable,
    ref StrippingSlotButtonPressed args)
  {
    EntityUid actor = args.Actor;
    HandsComponent comp1;
    if (!actor.Valid || !this.TryComp<HandsComponent>(actor, out comp1))
      return;
    StrippableAttemptEvent args1 = new StrippableAttemptEvent(actor, strippable.Owner);
    this.RaiseLocalEvent<StrippableAttemptEvent>(strippable.Owner, ref args1);
    if (args1.Cancelled)
      return;
    if (args.IsHand)
    {
      this.StripHand((Entity<HandsComponent>) (actor, comp1), (Entity<HandsComponent>) (strippable.Owner, (HandsComponent) null), args.Slot, (StrippableComponent) strippable);
    }
    else
    {
      InventoryComponent comp2;
      if (!this.TryComp<InventoryComponent>((EntityUid) strippable, out comp2))
        return;
      EntityUid? entityUid;
      bool slotEntity = this._inventorySystem.TryGetSlotEntity((EntityUid) strippable, args.Slot, out entityUid, comp2);
      EntityUid? activeItem = this._handsSystem.GetActiveItem((Entity<HandsComponent>) (actor, comp1));
      if (activeItem.HasValue)
      {
        EntityUid valueOrDefault = activeItem.GetValueOrDefault();
        if (!slotEntity)
        {
          this.StartStripInsertInventory((Entity<HandsComponent>) (actor, comp1), strippable.Owner, valueOrDefault, args.Slot);
          return;
        }
      }
      if (!slotEntity)
        return;
      this.StartStripRemoveInventory(actor, strippable.Owner, entityUid.Value, args.Slot);
    }
  }

  private void StripHand(
    Entity<HandsComponent?> user,
    Entity<HandsComponent?> target,
    string handId,
    StrippableComponent? targetStrippable)
  {
    if (!this.Resolve<HandsComponent>((EntityUid) user, ref user.Comp) || !this.Resolve<HandsComponent>((EntityUid) target, ref target.Comp) || !this.Resolve<StrippableComponent>((EntityUid) target, ref targetStrippable) || !target.Comp.CanBeStripped)
      return;
    EntityUid? heldItem = this._handsSystem.GetHeldItem((Entity<HandsComponent>) target.Owner, handId);
    VirtualItemComponent comp1;
    CuffableComponent comp2;
    if (this.TryComp<VirtualItemComponent>(heldItem, out comp1) && this.TryComp<CuffableComponent>(target.Owner, out comp2) && this._cuffableSystem.GetAllCuffs(comp2).Contains<EntityUid>(comp1.BlockingEntity))
    {
      this._cuffableSystem.TryUncuff(target.Owner, (EntityUid) user, new EntityUid?(comp1.BlockingEntity), comp2);
    }
    else
    {
      EntityUid? activeItem = this._handsSystem.GetActiveItem(user.AsNullable());
      if (activeItem.HasValue)
      {
        EntityUid valueOrDefault = activeItem.GetValueOrDefault();
        if (!heldItem.HasValue)
        {
          this.StartStripInsertHand(user, target, valueOrDefault, handId, targetStrippable);
          return;
        }
      }
      if (!heldItem.HasValue)
        return;
      this.StartStripRemoveHand(user, target, heldItem.Value, handId, targetStrippable);
    }
  }

  private bool CanStripInsertInventory(
    Entity<HandsComponent?> user,
    EntityUid target,
    EntityUid held,
    string slot)
  {
    EntityUid? nullable;
    if (!this.Resolve<HandsComponent>((EntityUid) user, ref user.Comp) || !this._handsSystem.TryGetActiveItem(user, out nullable))
      return false;
    EntityUid? entityUid1 = nullable;
    EntityUid entityUid2 = held;
    if ((entityUid1.HasValue ? (entityUid1.GetValueOrDefault() != entityUid2 ? 1 : 0) : 1) != 0)
      return false;
    if (!this._handsSystem.CanDropHeld((EntityUid) user, user.Comp.ActiveHandId))
    {
      this._popupSystem.PopupCursor(this.Loc.GetString("strippable-component-cannot-drop"));
      return false;
    }
    EntityUid uid = target;
    EntityManager entityManager = this.EntityManager;
    entityUid1 = new EntityUid?();
    EntityUid? viewer = entityUid1;
    EntityUid entityUid3 = Identity.Entity(uid, (IEntityManager) entityManager, viewer);
    if (this._inventorySystem.TryGetSlotEntity(target, slot, out entityUid1))
    {
      this._popupSystem.PopupCursor(this.Loc.GetString("strippable-component-item-slot-occupied", ("owner", (object) entityUid3)));
      return false;
    }
    if (this._inventorySystem.CanEquip((EntityUid) user, target, held, slot, out string _))
      return true;
    this._popupSystem.PopupCursor(this.Loc.GetString("strippable-component-cannot-equip-message", ("owner", (object) entityUid3)));
    return false;
  }

  private void StartStripInsertInventory(
    Entity<HandsComponent?> user,
    EntityUid target,
    EntityUid held,
    string slot)
  {
    if (!this.Resolve<HandsComponent>((EntityUid) user, ref user.Comp) || !this.CanStripInsertInventory(user, target, held, slot))
      return;
    SlotDefinition slotDefinition;
    if (!this._inventorySystem.TryGetSlot(target, slot, out slotDefinition))
    {
      this.Log.Error($"{this.ToPrettyString(new EntityUid?((EntityUid) user))} attempted to place an item in a non-existent inventory slot ({slot}) on {this.ToPrettyString((Entity<MetaDataComponent>) target)}");
    }
    else
    {
      (TimeSpan timeSpan, bool Stealth) = this.GetStripTimeModifiers((EntityUid) user, target, new EntityUid?(held), slotDefinition.StripTime);
      if (!Stealth)
        this._popupSystem.PopupEntity(this.Loc.GetString("strippable-component-alert-owner-insert", (nameof (user), (object) Identity.Entity((EntityUid) user, (IEntityManager) this.EntityManager)), ("item", (object) this._handsSystem.GetActiveItem((Entity<HandsComponent>) ((EntityUid) user, user.Comp)).Value)), target, target, PopupType.Large);
      string str = Stealth ? "stealthily " : "";
      ISharedAdminLogManager adminLogger = this._adminLogger;
      LogStringHandler logStringHandler = new LogStringHandler(41, 5);
      logStringHandler.AppendFormatted<EntityStringRepresentation?>(this.ToPrettyString(new EntityUid?((EntityUid) user)), "actor", "ToPrettyString(user)");
      logStringHandler.AppendLiteral(" is trying to ");
      logStringHandler.AppendFormatted(str);
      logStringHandler.AppendLiteral("place the item ");
      logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) held), "item", "ToPrettyString(held)");
      logStringHandler.AppendLiteral(" in ");
      logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) target), nameof (target), "ToPrettyString(target)");
      logStringHandler.AppendLiteral("'s ");
      logStringHandler.AppendFormatted(slot);
      logStringHandler.AppendLiteral(" slot");
      ref LogStringHandler local = ref logStringHandler;
      adminLogger.Add(LogType.Stripping, LogImpact.Low, ref local);
      this._doAfterSystem.TryStartDoAfter(new DoAfterArgs((IEntityManager) this.EntityManager, (EntityUid) user, timeSpan, (DoAfterEvent) new StrippableDoAfterEvent(true, true, slot), new EntityUid?((EntityUid) user), new EntityUid?(target), new EntityUid?(held))
      {
        Hidden = Stealth,
        AttemptFrequency = AttemptFrequency.EveryTick,
        BreakOnDamage = true,
        BreakOnMove = true,
        NeedHand = true,
        DuplicateCondition = DuplicateConditions.SameTool,
        ForceVisible = user.Owner != target
      });
    }
  }

  private void StripInsertInventory(
    Entity<HandsComponent?> user,
    EntityUid target,
    EntityUid held,
    string slot)
  {
    if (!this.Resolve<HandsComponent>((EntityUid) user, ref user.Comp) || !this.CanStripInsertInventory(user, target, held, slot) || !this._handsSystem.TryDrop(user))
      return;
    this._inventorySystem.TryEquip((EntityUid) user, target, held, slot, triggerHandContact: true);
    ISharedAdminLogManager adminLogger = this._adminLogger;
    LogStringHandler logStringHandler = new LogStringHandler(33, 4);
    logStringHandler.AppendFormatted<EntityStringRepresentation?>(this.ToPrettyString(new EntityUid?((EntityUid) user)), "actor", "ToPrettyString(user)");
    logStringHandler.AppendLiteral(" has placed the item ");
    logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) held), "item", "ToPrettyString(held)");
    logStringHandler.AppendLiteral(" in ");
    logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) target), nameof (target), "ToPrettyString(target)");
    logStringHandler.AppendLiteral("'s ");
    logStringHandler.AppendFormatted(slot);
    logStringHandler.AppendLiteral(" slot");
    ref LogStringHandler local = ref logStringHandler;
    adminLogger.Add(LogType.Stripping, LogImpact.Medium, ref local);
  }

  private bool CanStripRemoveInventory(
    EntityUid user,
    EntityUid target,
    EntityUid item,
    string slot)
  {
    EntityUid? entityUid1;
    if (!this._inventorySystem.TryGetSlotEntity(target, slot, out entityUid1))
    {
      this._popupSystem.PopupCursor(this.Loc.GetString("strippable-component-item-slot-free-message", ("owner", (object) Identity.Entity(target, (IEntityManager) this.EntityManager))));
      return false;
    }
    EntityUid? nullable = entityUid1;
    EntityUid entityUid2 = item;
    if ((nullable.HasValue ? (nullable.GetValueOrDefault() != entityUid2 ? 1 : 0) : 1) != 0)
      return false;
    string reason;
    if (!this._inventorySystem.CanUnequip(user, target, slot, out reason))
    {
      this._popupSystem.PopupCursor(this.Loc.GetString(reason));
      return false;
    }
    if (!this.HasComp<RMCUnstrippableComponent>(entityUid1))
      return true;
    SharedPopupSystem popupSystem = this._popupSystem;
    ILocalizationManager loc = this.Loc;
    (string, object) valueTuple1 = (nameof (item), (object) entityUid1);
    EntityUid uid = target;
    EntityManager entityManager = this.EntityManager;
    nullable = new EntityUid?();
    EntityUid? viewer = nullable;
    (string, object) valueTuple2 = ("owner", (object) Identity.Entity(uid, (IEntityManager) entityManager, viewer));
    string message = loc.GetString("rmc-unstrippable", valueTuple1, valueTuple2);
    popupSystem.PopupCursor(message);
    return false;
  }

  private void StartStripRemoveInventory(
    EntityUid user,
    EntityUid target,
    EntityUid item,
    string slot)
  {
    if (!this.CanStripRemoveInventory(user, target, item, slot))
      return;
    SlotDefinition slotDefinition;
    if (!this._inventorySystem.TryGetSlot(target, slot, out slotDefinition))
    {
      this.Log.Error($"{this.ToPrettyString((Entity<MetaDataComponent>) user)} attempted to take an item from a non-existent inventory slot ({slot}) on {this.ToPrettyString((Entity<MetaDataComponent>) target)}");
    }
    else
    {
      (TimeSpan timeSpan, bool Stealth) = this.GetStripTimeModifiers(user, target, new EntityUid?(item), slotDefinition.StripTime);
      if (!Stealth)
      {
        if (this.IsStripHidden(slotDefinition, new EntityUid?(user)))
          this._popupSystem.PopupEntity(this.Loc.GetString("strippable-component-alert-owner-hidden", (nameof (slot), (object) slot)), target, target, PopupType.Large);
        else
          this._popupSystem.PopupEntity(this.Loc.GetString("strippable-component-alert-owner", (nameof (user), (object) Identity.Entity(user, (IEntityManager) this.EntityManager)), (nameof (item), (object) item)), target, target, PopupType.Large);
      }
      string str = Stealth ? "stealthily " : "";
      ISharedAdminLogManager adminLogger = this._adminLogger;
      LogStringHandler logStringHandler = new LogStringHandler(43, 5);
      logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) user), "actor", "ToPrettyString(user)");
      logStringHandler.AppendLiteral(" is trying to ");
      logStringHandler.AppendFormatted(str);
      logStringHandler.AppendLiteral("strip the item ");
      logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) item), nameof (item), "ToPrettyString(item)");
      logStringHandler.AppendLiteral(" from ");
      logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) target), nameof (target), "ToPrettyString(target)");
      logStringHandler.AppendLiteral("'s ");
      logStringHandler.AppendFormatted(slot);
      logStringHandler.AppendLiteral(" slot");
      ref LogStringHandler local = ref logStringHandler;
      adminLogger.Add(LogType.Stripping, LogImpact.Low, ref local);
      this._interactionSystem.DoContactInteraction(user, new EntityUid?(item));
      this._doAfterSystem.TryStartDoAfter(new DoAfterArgs((IEntityManager) this.EntityManager, user, timeSpan, (DoAfterEvent) new StrippableDoAfterEvent(false, true, slot), new EntityUid?(user), new EntityUid?(target), new EntityUid?(item))
      {
        Hidden = Stealth,
        AttemptFrequency = AttemptFrequency.EveryTick,
        BreakOnDamage = true,
        BreakOnMove = true,
        NeedHand = true,
        BreakOnHandChange = false,
        DuplicateCondition = this._skills.HasSkill((Entity<SkillsComponent>) user, SharedStrippableSystem.MultiStripSkill, 2) ? DuplicateConditions.SameTool : DuplicateConditions.SameEvent,
        ForceVisible = user != target
      });
    }
  }

  private void StripRemoveInventory(
    EntityUid user,
    EntityUid target,
    EntityUid item,
    string slot,
    bool stealth)
  {
    if (!this.CanStripRemoveInventory(user, target, item, slot) || !this._inventorySystem.TryUnequip(user, target, slot, triggerHandContact: true))
      return;
    this.RaiseLocalEvent<DroppedEvent>(item, new DroppedEvent(user), true);
    this._handsSystem.PickupOrDrop(new EntityUid?(user), item, animateUser: stealth, animate: !stealth);
    ISharedAdminLogManager adminLogger = this._adminLogger;
    LogStringHandler logStringHandler = new LogStringHandler(37, 4);
    logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) user), "actor", "ToPrettyString(user)");
    logStringHandler.AppendLiteral(" has stripped the item ");
    logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) item), nameof (item), "ToPrettyString(item)");
    logStringHandler.AppendLiteral(" from ");
    logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) target), nameof (target), "ToPrettyString(target)");
    logStringHandler.AppendLiteral("'s ");
    logStringHandler.AppendFormatted(slot);
    logStringHandler.AppendLiteral(" slot");
    ref LogStringHandler local = ref logStringHandler;
    adminLogger.Add(LogType.Stripping, LogImpact.High, ref local);
  }

  private bool CanStripInsertHand(
    Entity<HandsComponent?> user,
    Entity<HandsComponent?> target,
    EntityUid held,
    string handName)
  {
    EntityUid? nullable1;
    if (!this.Resolve<HandsComponent>((EntityUid) user, ref user.Comp) || !this.Resolve<HandsComponent>((EntityUid) target, ref target.Comp) || !target.Comp.CanBeStripped || !this._handsSystem.TryGetActiveItem(user, out nullable1))
      return false;
    EntityUid? nullable2 = nullable1;
    EntityUid entityUid = held;
    if ((nullable2.HasValue ? (nullable2.GetValueOrDefault() != entityUid ? 1 : 0) : 1) != 0)
      return false;
    if (!this._handsSystem.CanDropHeld((EntityUid) user, user.Comp.ActiveHandId))
    {
      this._popupSystem.PopupCursor(this.Loc.GetString("strippable-component-cannot-drop"));
      return false;
    }
    if (this._handsSystem.CanPickupToHand((EntityUid) target, nullable1.Value, handName, false, target.Comp))
      return true;
    SharedPopupSystem popupSystem = this._popupSystem;
    ILocalizationManager loc = this.Loc;
    EntityUid uid = (EntityUid) target;
    EntityManager entityManager = this.EntityManager;
    nullable2 = new EntityUid?();
    EntityUid? viewer = nullable2;
    (string, object) valueTuple = ("owner", (object) Identity.Entity(uid, (IEntityManager) entityManager, viewer));
    string message = loc.GetString("strippable-component-cannot-put-message", valueTuple);
    popupSystem.PopupCursor(message);
    return false;
  }

  private void StartStripInsertHand(
    Entity<HandsComponent?> user,
    Entity<HandsComponent?> target,
    EntityUid held,
    string handName,
    StrippableComponent? targetStrippable = null)
  {
    if (!this.Resolve<HandsComponent>((EntityUid) user, ref user.Comp) || !this.Resolve<HandsComponent>((EntityUid) target, ref target.Comp) || !this.Resolve<StrippableComponent>((EntityUid) target, ref targetStrippable) || !this.CanStripInsertHand(user, target, held, handName))
      return;
    (TimeSpan timeSpan, bool Stealth) = this.GetStripTimeModifiers((EntityUid) user, (EntityUid) target, new EntityUid?(), targetStrippable.HandStripDelay);
    if (!Stealth)
      this._popupSystem.PopupEntity(this.Loc.GetString("strippable-component-alert-owner-insert-hand", (nameof (user), (object) Identity.Entity((EntityUid) user, (IEntityManager) this.EntityManager)), ("item", (object) this._handsSystem.GetActiveItem(user).Value)), (EntityUid) target, (EntityUid) target, PopupType.Large);
    string str = Stealth ? "stealthily " : "";
    ISharedAdminLogManager adminLogger = this._adminLogger;
    LogStringHandler logStringHandler = new LogStringHandler(41, 4);
    logStringHandler.AppendFormatted<EntityStringRepresentation?>(this.ToPrettyString(new EntityUid?((EntityUid) user)), "actor", "ToPrettyString(user)");
    logStringHandler.AppendLiteral(" is trying to ");
    logStringHandler.AppendFormatted(str);
    logStringHandler.AppendLiteral("place the item ");
    logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) held), "item", "ToPrettyString(held)");
    logStringHandler.AppendLiteral(" in ");
    logStringHandler.AppendFormatted<EntityStringRepresentation?>(this.ToPrettyString(new EntityUid?((EntityUid) target)), nameof (target), "ToPrettyString(target)");
    logStringHandler.AppendLiteral("'s hands");
    ref LogStringHandler local = ref logStringHandler;
    adminLogger.Add(LogType.Stripping, LogImpact.Low, ref local);
    this._doAfterSystem.TryStartDoAfter(new DoAfterArgs((IEntityManager) this.EntityManager, (EntityUid) user, timeSpan, (DoAfterEvent) new StrippableDoAfterEvent(true, false, handName), new EntityUid?((EntityUid) user), new EntityUid?((EntityUid) target), new EntityUid?(held))
    {
      Hidden = Stealth,
      AttemptFrequency = AttemptFrequency.EveryTick,
      BreakOnDamage = true,
      BreakOnMove = true,
      NeedHand = true,
      DuplicateCondition = DuplicateConditions.SameTool,
      ForceVisible = user != target
    });
  }

  private void StripInsertHand(
    Entity<HandsComponent?> user,
    Entity<HandsComponent?> target,
    EntityUid held,
    string handName,
    bool stealth)
  {
    if (!this.Resolve<HandsComponent>((EntityUid) user, ref user.Comp) || !this.Resolve<HandsComponent>((EntityUid) target, ref target.Comp) || !this.CanStripInsertHand(user, target, held, handName))
      return;
    this._handsSystem.TryDrop(user, checkActionBlocker: false);
    this._handsSystem.TryPickup((EntityUid) target, held, handName, false, stealth, !stealth, target.Comp);
    ISharedAdminLogManager adminLogger = this._adminLogger;
    LogStringHandler logStringHandler = new LogStringHandler(33, 3);
    logStringHandler.AppendFormatted<EntityStringRepresentation?>(this.ToPrettyString(new EntityUid?((EntityUid) user)), "actor", "ToPrettyString(user)");
    logStringHandler.AppendLiteral(" has placed the item ");
    logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) held), "item", "ToPrettyString(held)");
    logStringHandler.AppendLiteral(" in ");
    logStringHandler.AppendFormatted<EntityStringRepresentation?>(this.ToPrettyString(new EntityUid?((EntityUid) target)), nameof (target), "ToPrettyString(target)");
    logStringHandler.AppendLiteral("'s hands");
    ref LogStringHandler local = ref logStringHandler;
    adminLogger.Add(LogType.Stripping, LogImpact.Medium, ref local);
  }

  private bool CanStripRemoveHand(
    EntityUid user,
    Entity<HandsComponent?> target,
    EntityUid item,
    string handName)
  {
    if (!this.Resolve<HandsComponent>((EntityUid) target, ref target.Comp) || !target.Comp.CanBeStripped)
      return false;
    if (!this._handsSystem.TryGetHand(target, handName, out Hand? _))
    {
      this._popupSystem.PopupCursor(this.Loc.GetString("strippable-component-item-slot-free-message", ("owner", (object) Identity.Entity((EntityUid) target, (IEntityManager) this.EntityManager))));
      return false;
    }
    EntityUid? held;
    if (!this._handsSystem.TryGetHeldItem(target, handName, out held) || this.HasComp<VirtualItemComponent>(held))
      return false;
    EntityUid? nullable = held;
    EntityUid entityUid = item;
    if ((nullable.HasValue ? (nullable.GetValueOrDefault() != entityUid ? 1 : 0) : 1) != 0)
      return false;
    if (this._handsSystem.CanDropHeld((EntityUid) target, handName, false))
      return true;
    SharedPopupSystem popupSystem = this._popupSystem;
    ILocalizationManager loc = this.Loc;
    EntityUid uid = (EntityUid) target;
    EntityManager entityManager = this.EntityManager;
    nullable = new EntityUid?();
    EntityUid? viewer = nullable;
    (string, object) valueTuple = ("owner", (object) Identity.Entity(uid, (IEntityManager) entityManager, viewer));
    string message = loc.GetString("strippable-component-cannot-drop-message", valueTuple);
    popupSystem.PopupCursor(message);
    return false;
  }

  private void StartStripRemoveHand(
    Entity<HandsComponent?> user,
    Entity<HandsComponent?> target,
    EntityUid item,
    string handName,
    StrippableComponent? targetStrippable = null)
  {
    if (!this.Resolve<HandsComponent>((EntityUid) user, ref user.Comp) || !this.Resolve<HandsComponent>((EntityUid) target, ref target.Comp) || !this.Resolve<StrippableComponent>((EntityUid) target, ref targetStrippable) || !this.CanStripRemoveHand((EntityUid) user, target, item, handName))
      return;
    (TimeSpan timeSpan, bool Stealth) = this.GetStripTimeModifiers((EntityUid) user, (EntityUid) target, new EntityUid?(), targetStrippable.HandStripDelay);
    if (!Stealth)
      this._popupSystem.PopupEntity(this.Loc.GetString("strippable-component-alert-owner", (nameof (user), (object) Identity.Entity((EntityUid) user, (IEntityManager) this.EntityManager)), (nameof (item), (object) item)), (EntityUid) target, (EntityUid) target);
    string str = Stealth ? "stealthily " : "";
    ISharedAdminLogManager adminLogger = this._adminLogger;
    LogStringHandler logStringHandler = new LogStringHandler(43, 4);
    logStringHandler.AppendFormatted<EntityStringRepresentation?>(this.ToPrettyString(new EntityUid?((EntityUid) user)), "actor", "ToPrettyString(user)");
    logStringHandler.AppendLiteral(" is trying to ");
    logStringHandler.AppendFormatted(str);
    logStringHandler.AppendLiteral("strip the item ");
    logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) item), nameof (item), "ToPrettyString(item)");
    logStringHandler.AppendLiteral(" from ");
    logStringHandler.AppendFormatted<EntityStringRepresentation?>(this.ToPrettyString(new EntityUid?((EntityUid) target)), nameof (target), "ToPrettyString(target)");
    logStringHandler.AppendLiteral("'s hands");
    ref LogStringHandler local = ref logStringHandler;
    adminLogger.Add(LogType.Stripping, LogImpact.Low, ref local);
    this._interactionSystem.DoContactInteraction((EntityUid) user, new EntityUid?(item));
    this._doAfterSystem.TryStartDoAfter(new DoAfterArgs((IEntityManager) this.EntityManager, (EntityUid) user, timeSpan, (DoAfterEvent) new StrippableDoAfterEvent(false, false, handName), new EntityUid?((EntityUid) user), new EntityUid?((EntityUid) target), new EntityUid?(item))
    {
      Hidden = Stealth,
      AttemptFrequency = AttemptFrequency.EveryTick,
      BreakOnDamage = true,
      BreakOnMove = true,
      NeedHand = true,
      BreakOnHandChange = false,
      DuplicateCondition = this._skills.HasSkill((Entity<SkillsComponent>) user.Owner, SharedStrippableSystem.MultiStripSkill, 2) ? DuplicateConditions.SameTool : DuplicateConditions.SameEvent,
      ForceVisible = user != target
    });
  }

  private void StripRemoveHand(
    Entity<HandsComponent?> user,
    Entity<HandsComponent?> target,
    EntityUid item,
    string handName,
    bool stealth)
  {
    if (!this.Resolve<HandsComponent>((EntityUid) user, ref user.Comp) || !this.Resolve<HandsComponent>((EntityUid) target, ref target.Comp) || !this.CanStripRemoveHand((EntityUid) user, target, item, handName))
      return;
    this._handsSystem.TryDrop(target, item, checkActionBlocker: false);
    this._handsSystem.PickupOrDrop(new EntityUid?((EntityUid) user), item, animateUser: stealth, animate: !stealth, handsComp: user.Comp);
    ISharedAdminLogManager adminLogger = this._adminLogger;
    LogStringHandler logStringHandler = new LogStringHandler(37, 3);
    logStringHandler.AppendFormatted<EntityStringRepresentation?>(this.ToPrettyString(new EntityUid?((EntityUid) user)), "actor", "ToPrettyString(user)");
    logStringHandler.AppendLiteral(" has stripped the item ");
    logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) item), nameof (item), "ToPrettyString(item)");
    logStringHandler.AppendLiteral(" from ");
    logStringHandler.AppendFormatted<EntityStringRepresentation?>(this.ToPrettyString(new EntityUid?((EntityUid) target)), nameof (target), "ToPrettyString(target)");
    logStringHandler.AppendLiteral("'s hands");
    ref LogStringHandler local = ref logStringHandler;
    adminLogger.Add(LogType.Stripping, LogImpact.High, ref local);
  }

  private void OnStrippableDoAfterRunning(
    Entity<HandsComponent> entity,
    ref DoAfterAttemptEvent<StrippableDoAfterEvent> ev)
  {
    DoAfterArgs args = ev.DoAfter.Args;
    if (ev.Event.InventoryOrHand)
    {
      if ((!ev.Event.InsertOrRemove || this.CanStripInsertInventory((Entity<HandsComponent>) (entity.Owner, entity.Comp), args.Target.Value, args.Used.Value, ev.Event.SlotOrHandName)) && (ev.Event.InsertOrRemove || this.CanStripRemoveInventory(entity.Owner, args.Target.Value, args.Used.Value, ev.Event.SlotOrHandName)))
        return;
      ev.Cancel();
    }
    else
    {
      if ((!ev.Event.InsertOrRemove || this.CanStripInsertHand((Entity<HandsComponent>) (entity.Owner, entity.Comp), (Entity<HandsComponent>) args.Target.Value, args.Used.Value, ev.Event.SlotOrHandName)) && (ev.Event.InsertOrRemove || this.CanStripRemoveHand(entity.Owner, (Entity<HandsComponent>) args.Target.Value, args.Used.Value, ev.Event.SlotOrHandName)))
        return;
      ev.Cancel();
    }
  }

  private void OnStrippableDoAfterFinished(
    Entity<HandsComponent> entity,
    ref StrippableDoAfterEvent ev)
  {
    if (ev.Cancelled)
      return;
    if (ev.InventoryOrHand)
    {
      if (ev.InsertOrRemove)
        this.StripInsertInventory((Entity<HandsComponent>) (entity.Owner, entity.Comp), ev.Target.Value, ev.Used.Value, ev.SlotOrHandName);
      else
        this.StripRemoveInventory(entity.Owner, ev.Target.Value, ev.Used.Value, ev.SlotOrHandName, ev.Args.Hidden);
    }
    else if (ev.InsertOrRemove)
      this.StripInsertHand((Entity<HandsComponent>) (entity.Owner, entity.Comp), (Entity<HandsComponent>) ev.Target.Value, ev.Used.Value, ev.SlotOrHandName, ev.Args.Hidden);
    else
      this.StripRemoveHand((Entity<HandsComponent>) (entity.Owner, entity.Comp), (Entity<HandsComponent>) ev.Target.Value, ev.Used.Value, ev.SlotOrHandName, ev.Args.Hidden);
  }

  private void OnActivateInWorld(
    EntityUid uid,
    StrippableComponent component,
    ActivateInWorldEvent args)
  {
    if (args.Handled || !args.Complex || args.Target == args.User || !this.TryOpenStrippingUi(args.User, (Entity<StrippableComponent>) (uid, component)))
      return;
    args.Handled = true;
  }

  public (TimeSpan Time, bool Stealth) GetStripTimeModifiers(
    EntityUid user,
    EntityUid targetPlayer,
    EntityUid? targetItem,
    TimeSpan initialTime)
  {
    BeforeItemStrippedEvent args1 = new BeforeItemStrippedEvent(initialTime);
    if (targetItem.HasValue)
      this.RaiseLocalEvent<BeforeItemStrippedEvent>(targetItem.Value, ref args1);
    BeforeStripEvent args2 = new BeforeStripEvent(args1.Time, args1.Stealth);
    this.RaiseLocalEvent<BeforeStripEvent>(user, ref args2);
    BeforeGettingStrippedEvent args3 = new BeforeGettingStrippedEvent(args2.Time, args2.Stealth);
    this.RaiseLocalEvent<BeforeGettingStrippedEvent>(targetPlayer, ref args3);
    return (args3.Time, args3.Stealth);
  }

  private void OnDragDrop(
    EntityUid uid,
    StrippableComponent component,
    ref DragDropDraggedEvent args)
  {
    if (args.Handled || args.Target != args.User || !this.TryOpenStrippingUi(args.User, (Entity<StrippableComponent>) (uid, component)))
      return;
    args.Handled = true;
  }

  public bool TryOpenStrippingUi(
    EntityUid user,
    Entity<StrippableComponent> target,
    bool openInCombat = false)
  {
    CombatModeComponent comp;
    if (!openInCombat && this.TryComp<CombatModeComponent>(user, out comp) && comp.IsInCombatMode || !this.HasComp<StrippingComponent>(user))
      return false;
    this._ui.OpenUi((Entity<UserInterfaceComponent>) target.Owner, (Enum) StrippingUiKey.Key, new EntityUid?(user));
    return true;
  }

  private void OnCanDropOn(
    EntityUid uid,
    StrippingComponent component,
    ref CanDropTargetEvent args)
  {
    bool flag = uid == args.User && this.HasComp<StrippableComponent>(args.Dragged) && this.HasComp<HandsComponent>(args.User) && this.HasComp<StrippingComponent>(args.User);
    args.Handled |= flag;
    args.CanDrop |= flag;
  }

  private void OnCanDrop(
    EntityUid uid,
    StrippableComponent component,
    ref CanDropDraggedEvent args)
  {
    ref bool local = ref args.CanDrop;
    local = ((local ? 1 : 0) | (!(args.Target == args.User) || !this.HasComp<StrippingComponent>(args.User) ? 0 : (this.HasComp<HandsComponent>(args.User) ? 1 : 0))) != 0;
    if (!args.CanDrop)
      return;
    args.Handled = true;
  }

  public bool IsStripHidden(SlotDefinition definition, EntityUid? viewer)
  {
    if (!definition.StripHidden)
      return false;
    return !viewer.HasValue || !this.HasComp<BypassInteractionChecksComponent>(viewer);
  }
}
