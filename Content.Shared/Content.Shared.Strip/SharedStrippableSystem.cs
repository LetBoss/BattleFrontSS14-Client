using System;
using System.Linq;
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
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;

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

	private static readonly EntProtoId<SkillDefinitionComponent> MultiStripSkill = EntProtoId<SkillDefinitionComponent>.op_Implicit("RMCSkillPolice");

	private const int MultiStripSkillLevel = 2;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<StrippableComponent, GetVerbsEvent<Verb>>((ComponentEventHandler<StrippableComponent, GetVerbsEvent<Verb>>)AddStripVerb, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<StrippableComponent, GetVerbsEvent<ExamineVerb>>((ComponentEventHandler<StrippableComponent, GetVerbsEvent<ExamineVerb>>)AddStripExamineVerb, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<StrippableComponent, StrippingSlotButtonPressed>((EntityEventRefHandler<StrippableComponent, StrippingSlotButtonPressed>)OnStripButtonPressed, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<HandsComponent, DoAfterAttemptEvent<StrippableDoAfterEvent>>((EntityEventRefHandler<HandsComponent, DoAfterAttemptEvent<StrippableDoAfterEvent>>)OnStrippableDoAfterRunning, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<HandsComponent, StrippableDoAfterEvent>((EntityEventRefHandler<HandsComponent, StrippableDoAfterEvent>)OnStrippableDoAfterFinished, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<StrippingComponent, CanDropTargetEvent>((ComponentEventRefHandler<StrippingComponent, CanDropTargetEvent>)OnCanDropOn, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<StrippableComponent, CanDropDraggedEvent>((ComponentEventRefHandler<StrippableComponent, CanDropDraggedEvent>)OnCanDrop, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<StrippableComponent, DragDropDraggedEvent>((ComponentEventRefHandler<StrippableComponent, DragDropDraggedEvent>)OnDragDrop, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<StrippableComponent, ActivateInWorldEvent>((ComponentEventHandler<StrippableComponent, ActivateInWorldEvent>)OnActivateInWorld, (Type[])null, (Type[])null);
	}

	private void AddStripVerb(EntityUid uid, StrippableComponent component, GetVerbsEvent<Verb> args)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Expected O, but got Unknown
		if (args.Hands != null && args.CanAccess && args.CanInteract && !(args.Target == args.User))
		{
			Verb verb = new Verb
			{
				Text = base.Loc.GetString("strip-verb-get-data-text"),
				Icon = (SpriteSpecifier?)new Texture(new ResPath("/Textures/Interface/VerbIcons/outfit.svg.192dpi.png")),
				Act = delegate
				{
					//IL_000c: Unknown result type (might be due to invalid IL or missing references)
					//IL_0012: Unknown result type (might be due to invalid IL or missing references)
					//IL_0022: Unknown result type (might be due to invalid IL or missing references)
					TryOpenStrippingUi(args.User, Entity<StrippableComponent>.op_Implicit((uid, component)), openInCombat: true);
				}
			};
			args.Verbs.Add(verb);
		}
	}

	private void AddStripExamineVerb(EntityUid uid, StrippableComponent component, GetVerbsEvent<ExamineVerb> args)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Expected O, but got Unknown
		if (args.Hands != null && args.CanAccess && args.CanInteract && !(args.Target == args.User))
		{
			ExamineVerb verb = new ExamineVerb
			{
				Text = base.Loc.GetString("strip-verb-get-data-text"),
				Icon = (SpriteSpecifier?)new Texture(new ResPath("/Textures/Interface/VerbIcons/outfit.svg.192dpi.png")),
				Act = delegate
				{
					//IL_000c: Unknown result type (might be due to invalid IL or missing references)
					//IL_0012: Unknown result type (might be due to invalid IL or missing references)
					//IL_0022: Unknown result type (might be due to invalid IL or missing references)
					TryOpenStrippingUi(args.User, Entity<StrippableComponent>.op_Implicit((uid, component)), openInCombat: true);
				},
				Category = VerbCategory.Examine
			};
			args.Verbs.Add(verb);
		}
	}

	private void OnStripButtonPressed(Entity<StrippableComponent> strippable, ref StrippingSlotButtonPressed args)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		EntityUid user = ((BaseBoundUserInterfaceEvent)args).Actor;
		HandsComponent userHands = default(HandsComponent);
		if (!((EntityUid)(ref user)).Valid || !((EntitySystem)this).TryComp<HandsComponent>(user, ref userHands))
		{
			return;
		}
		StrippableAttemptEvent attempt = new StrippableAttemptEvent(user, strippable.Owner);
		((EntitySystem)this).RaiseLocalEvent<StrippableAttemptEvent>(strippable.Owner, ref attempt, false);
		if (attempt.Cancelled)
		{
			return;
		}
		if (args.IsHand)
		{
			StripHand(Entity<HandsComponent>.op_Implicit((user, userHands)), Entity<HandsComponent>.op_Implicit((ValueTuple<EntityUid, HandsComponent>)(strippable.Owner, null)), args.Slot, Entity<StrippableComponent>.op_Implicit(strippable));
		}
		else
		{
			InventoryComponent inventory = default(InventoryComponent);
			if (!((EntitySystem)this).TryComp<InventoryComponent>(Entity<StrippableComponent>.op_Implicit(strippable), ref inventory))
			{
				return;
			}
			EntityUid? held;
			bool hasEnt = _inventorySystem.TryGetSlotEntity(Entity<StrippableComponent>.op_Implicit(strippable), args.Slot, out held, inventory);
			EntityUid? activeItem = _handsSystem.GetActiveItem(Entity<HandsComponent>.op_Implicit((user, userHands)));
			if (activeItem.HasValue)
			{
				EntityUid activeItem2 = activeItem.GetValueOrDefault();
				if (!hasEnt)
				{
					StartStripInsertInventory(Entity<HandsComponent>.op_Implicit((user, userHands)), strippable.Owner, activeItem2, args.Slot);
					return;
				}
			}
			if (hasEnt)
			{
				StartStripRemoveInventory(user, strippable.Owner, held.Value, args.Slot);
			}
		}
	}

	private void StripHand(Entity<HandsComponent?> user, Entity<HandsComponent?> target, string handId, StrippableComponent? targetStrippable)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<HandsComponent>(Entity<HandsComponent>.op_Implicit(user), ref user.Comp, true) || !((EntitySystem)this).Resolve<HandsComponent>(Entity<HandsComponent>.op_Implicit(target), ref target.Comp, true) || !((EntitySystem)this).Resolve<StrippableComponent>(Entity<HandsComponent>.op_Implicit(target), ref targetStrippable, true) || !target.Comp.CanBeStripped)
		{
			return;
		}
		EntityUid? heldEntity = _handsSystem.GetHeldItem(Entity<HandsComponent>.op_Implicit(target.Owner), handId);
		VirtualItemComponent virtualItem = default(VirtualItemComponent);
		CuffableComponent cuffable = default(CuffableComponent);
		if (((EntitySystem)this).TryComp<VirtualItemComponent>(heldEntity, ref virtualItem) && ((EntitySystem)this).TryComp<CuffableComponent>(target.Owner, ref cuffable) && _cuffableSystem.GetAllCuffs(cuffable).Contains(virtualItem.BlockingEntity))
		{
			_cuffableSystem.TryUncuff(target.Owner, Entity<HandsComponent>.op_Implicit(user), virtualItem.BlockingEntity, cuffable);
			return;
		}
		EntityUid? activeItem = _handsSystem.GetActiveItem(user.AsNullable());
		if (activeItem.HasValue)
		{
			EntityUid activeItem2 = activeItem.GetValueOrDefault();
			if (!heldEntity.HasValue)
			{
				StartStripInsertHand(user, target, activeItem2, handId, targetStrippable);
				return;
			}
		}
		if (heldEntity.HasValue)
		{
			StartStripRemoveHand(user, target, heldEntity.Value, handId, targetStrippable);
		}
	}

	private bool CanStripInsertInventory(Entity<HandsComponent?> user, EntityUid target, EntityUid held, string slot)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<HandsComponent>(Entity<HandsComponent>.op_Implicit(user), ref user.Comp, true))
		{
			return false;
		}
		if (_handsSystem.TryGetActiveItem(user, out var activeItem))
		{
			EntityUid? val = activeItem;
			if (val.HasValue && !(val.GetValueOrDefault() != held))
			{
				if (!_handsSystem.CanDropHeld(Entity<HandsComponent>.op_Implicit(user), user.Comp.ActiveHandId))
				{
					_popupSystem.PopupCursor(base.Loc.GetString("strippable-component-cannot-drop"));
					return false;
				}
				EntityManager entityManager = base.EntityManager;
				val = null;
				EntityUid targetIdentity = Identity.Entity(target, (IEntityManager)(object)entityManager, val);
				if (_inventorySystem.TryGetSlotEntity(target, slot, out val))
				{
					_popupSystem.PopupCursor(base.Loc.GetString("strippable-component-item-slot-occupied", (ValueTuple<string, object>)("owner", targetIdentity)));
					return false;
				}
				if (!_inventorySystem.CanEquip(Entity<HandsComponent>.op_Implicit(user), target, held, slot, out string _))
				{
					_popupSystem.PopupCursor(base.Loc.GetString("strippable-component-cannot-equip-message", (ValueTuple<string, object>)("owner", targetIdentity)));
					return false;
				}
				return true;
			}
		}
		return false;
	}

	private void StartStripInsertInventory(Entity<HandsComponent?> user, EntityUid target, EntityUid held, string slot)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0229: Unknown result type (might be due to invalid IL or missing references)
		//IL_022a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0239: Unknown result type (might be due to invalid IL or missing references)
		//IL_023a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0244: Unknown result type (might be due to invalid IL or missing references)
		//IL_024a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0280: Unknown result type (might be due to invalid IL or missing references)
		//IL_0281: Unknown result type (might be due to invalid IL or missing references)
		//IL_0286: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<HandsComponent>(Entity<HandsComponent>.op_Implicit(user), ref user.Comp, true) || !CanStripInsertInventory(user, target, held, slot))
		{
			return;
		}
		if (!_inventorySystem.TryGetSlot(target, slot, out SlotDefinition slotDef))
		{
			((EntitySystem)this).Log.Error($"{((EntitySystem)this).ToPrettyString((EntityUid?)Entity<HandsComponent>.op_Implicit(user), (MetaDataComponent)null)} attempted to place an item in a non-existent inventory slot ({slot}) on {((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(target))}");
			return;
		}
		var (time, stealth) = GetStripTimeModifiers(Entity<HandsComponent>.op_Implicit(user), target, held, slotDef.StripTime);
		if (!stealth)
		{
			_popupSystem.PopupEntity(base.Loc.GetString("strippable-component-alert-owner-insert", (ValueTuple<string, object>)("user", Identity.Entity(Entity<HandsComponent>.op_Implicit(user), (IEntityManager)(object)base.EntityManager)), (ValueTuple<string, object>)("item", _handsSystem.GetActiveItem(Entity<HandsComponent>.op_Implicit((Entity<HandsComponent>.op_Implicit(user), user.Comp))).Value)), target, target, PopupType.Large);
		}
		string prefix = (stealth ? "stealthily " : "");
		ISharedAdminLogManager adminLogger = _adminLogger;
		LogStringHandler handler = new LogStringHandler(41, 5);
		handler.AppendFormatted(((EntitySystem)this).ToPrettyString((EntityUid?)Entity<HandsComponent>.op_Implicit(user), (MetaDataComponent)null), "actor", "ToPrettyString(user)");
		handler.AppendLiteral(" is trying to ");
		handler.AppendFormatted(prefix);
		handler.AppendLiteral("place the item ");
		handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(held)), "item", "ToPrettyString(held)");
		handler.AppendLiteral(" in ");
		handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(target)), "target", "ToPrettyString(target)");
		handler.AppendLiteral("'s ");
		handler.AppendFormatted(slot);
		handler.AppendLiteral(" slot");
		adminLogger.Add(LogType.Stripping, LogImpact.Low, ref handler);
		DoAfterArgs doAfterArgs = new DoAfterArgs((IEntityManager)(object)base.EntityManager, Entity<HandsComponent>.op_Implicit(user), time, new StrippableDoAfterEvent(insertOrRemove: true, inventoryOrHand: true, slot), Entity<HandsComponent>.op_Implicit(user), target, held)
		{
			Hidden = stealth,
			AttemptFrequency = AttemptFrequency.EveryTick,
			BreakOnDamage = true,
			BreakOnMove = true,
			NeedHand = true,
			DuplicateCondition = DuplicateConditions.SameTool,
			ForceVisible = (user.Owner != target)
		};
		_doAfterSystem.TryStartDoAfter(doAfterArgs);
	}

	private void StripInsertInventory(Entity<HandsComponent?> user, EntityUid target, EntityUid held, string slot)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<HandsComponent>(Entity<HandsComponent>.op_Implicit(user), ref user.Comp, true) && CanStripInsertInventory(user, target, held, slot) && _handsSystem.TryDrop(user))
		{
			_inventorySystem.TryEquip(Entity<HandsComponent>.op_Implicit(user), target, held, slot, silent: false, force: false, predicted: false, null, null, checkDoafter: false, triggerHandContact: true);
			ISharedAdminLogManager adminLogger = _adminLogger;
			LogStringHandler handler = new LogStringHandler(33, 4);
			handler.AppendFormatted(((EntitySystem)this).ToPrettyString((EntityUid?)Entity<HandsComponent>.op_Implicit(user), (MetaDataComponent)null), "actor", "ToPrettyString(user)");
			handler.AppendLiteral(" has placed the item ");
			handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(held)), "item", "ToPrettyString(held)");
			handler.AppendLiteral(" in ");
			handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(target)), "target", "ToPrettyString(target)");
			handler.AppendLiteral("'s ");
			handler.AppendFormatted(slot);
			handler.AppendLiteral(" slot");
			adminLogger.Add(LogType.Stripping, LogImpact.Medium, ref handler);
		}
	}

	private bool CanStripRemoveInventory(EntityUid user, EntityUid target, EntityUid item, string slot)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		if (!_inventorySystem.TryGetSlotEntity(target, slot, out var slotItem))
		{
			_popupSystem.PopupCursor(base.Loc.GetString("strippable-component-item-slot-free-message", (ValueTuple<string, object>)("owner", Identity.Entity(target, (IEntityManager)(object)base.EntityManager))));
			return false;
		}
		EntityUid? val = slotItem;
		if (!val.HasValue || val.GetValueOrDefault() != item)
		{
			return false;
		}
		if (!_inventorySystem.CanUnequip(user, target, slot, out string reason))
		{
			_popupSystem.PopupCursor(base.Loc.GetString(reason));
			return false;
		}
		if (((EntitySystem)this).HasComp<RMCUnstrippableComponent>(slotItem))
		{
			_popupSystem.PopupCursor(base.Loc.GetString("rmc-unstrippable", (ValueTuple<string, object>)("item", slotItem), (ValueTuple<string, object>)("owner", Identity.Entity(target, (IEntityManager)(object)base.EntityManager))));
			return false;
		}
		return true;
	}

	private void StartStripRemoveInventory(EntityUid user, EntityUid target, EntityUid item, string slot)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_020e: Unknown result type (might be due to invalid IL or missing references)
		//IL_020f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0221: Unknown result type (might be due to invalid IL or missing references)
		//IL_022c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0232: Unknown result type (might be due to invalid IL or missing references)
		//IL_0238: Unknown result type (might be due to invalid IL or missing references)
		//IL_0274: Unknown result type (might be due to invalid IL or missing references)
		//IL_0275: Unknown result type (might be due to invalid IL or missing references)
		//IL_027a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0291: Unknown result type (might be due to invalid IL or missing references)
		//IL_0292: Unknown result type (might be due to invalid IL or missing references)
		if (!CanStripRemoveInventory(user, target, item, slot))
		{
			return;
		}
		if (!_inventorySystem.TryGetSlot(target, slot, out SlotDefinition slotDef))
		{
			((EntitySystem)this).Log.Error($"{((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(user))} attempted to take an item from a non-existent inventory slot ({slot}) on {((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(target))}");
			return;
		}
		var (time, stealth) = GetStripTimeModifiers(user, target, item, slotDef.StripTime);
		if (!stealth)
		{
			if (IsStripHidden(slotDef, user))
			{
				_popupSystem.PopupEntity(base.Loc.GetString("strippable-component-alert-owner-hidden", (ValueTuple<string, object>)("slot", slot)), target, target, PopupType.Large);
			}
			else
			{
				_popupSystem.PopupEntity(base.Loc.GetString("strippable-component-alert-owner", (ValueTuple<string, object>)("user", Identity.Entity(user, (IEntityManager)(object)base.EntityManager)), (ValueTuple<string, object>)("item", item)), target, target, PopupType.Large);
			}
		}
		string prefix = (stealth ? "stealthily " : "");
		ISharedAdminLogManager adminLogger = _adminLogger;
		LogStringHandler handler = new LogStringHandler(43, 5);
		handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(user)), "actor", "ToPrettyString(user)");
		handler.AppendLiteral(" is trying to ");
		handler.AppendFormatted(prefix);
		handler.AppendLiteral("strip the item ");
		handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(item)), "item", "ToPrettyString(item)");
		handler.AppendLiteral(" from ");
		handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(target)), "target", "ToPrettyString(target)");
		handler.AppendLiteral("'s ");
		handler.AppendFormatted(slot);
		handler.AppendLiteral(" slot");
		adminLogger.Add(LogType.Stripping, LogImpact.Low, ref handler);
		_interactionSystem.DoContactInteraction(user, item);
		DoAfterArgs doAfterArgs = new DoAfterArgs((IEntityManager)(object)base.EntityManager, user, time, new StrippableDoAfterEvent(insertOrRemove: false, inventoryOrHand: true, slot), user, target, item)
		{
			Hidden = stealth,
			AttemptFrequency = AttemptFrequency.EveryTick,
			BreakOnDamage = true,
			BreakOnMove = true,
			NeedHand = true,
			BreakOnHandChange = false,
			DuplicateCondition = (_skills.HasSkill(Entity<SkillsComponent>.op_Implicit(user), MultiStripSkill, 2) ? DuplicateConditions.SameTool : DuplicateConditions.SameEvent),
			ForceVisible = (user != target)
		};
		_doAfterSystem.TryStartDoAfter(doAfterArgs);
	}

	private void StripRemoveInventory(EntityUid user, EntityUid target, EntityUid item, string slot, bool stealth)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		if (CanStripRemoveInventory(user, target, item, slot) && _inventorySystem.TryUnequip(user, target, slot, silent: false, force: false, predicted: false, null, null, reparent: true, checkDoafter: false, triggerHandContact: true))
		{
			((EntitySystem)this).RaiseLocalEvent<DroppedEvent>(item, new DroppedEvent(user), true);
			_handsSystem.PickupOrDrop(user, item, checkActionBlocker: true, stealth, !stealth);
			ISharedAdminLogManager adminLogger = _adminLogger;
			LogStringHandler handler = new LogStringHandler(37, 4);
			handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(user)), "actor", "ToPrettyString(user)");
			handler.AppendLiteral(" has stripped the item ");
			handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(item)), "item", "ToPrettyString(item)");
			handler.AppendLiteral(" from ");
			handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(target)), "target", "ToPrettyString(target)");
			handler.AppendLiteral("'s ");
			handler.AppendFormatted(slot);
			handler.AppendLiteral(" slot");
			adminLogger.Add(LogType.Stripping, LogImpact.High, ref handler);
		}
	}

	private bool CanStripInsertHand(Entity<HandsComponent?> user, Entity<HandsComponent?> target, EntityUid held, string handName)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<HandsComponent>(Entity<HandsComponent>.op_Implicit(user), ref user.Comp, true) || !((EntitySystem)this).Resolve<HandsComponent>(Entity<HandsComponent>.op_Implicit(target), ref target.Comp, true))
		{
			return false;
		}
		if (!target.Comp.CanBeStripped)
		{
			return false;
		}
		if (_handsSystem.TryGetActiveItem(user, out var activeItem))
		{
			EntityUid? val = activeItem;
			if (val.HasValue && !(val.GetValueOrDefault() != held))
			{
				if (!_handsSystem.CanDropHeld(Entity<HandsComponent>.op_Implicit(user), user.Comp.ActiveHandId))
				{
					_popupSystem.PopupCursor(base.Loc.GetString("strippable-component-cannot-drop"));
					return false;
				}
				if (!_handsSystem.CanPickupToHand(Entity<HandsComponent>.op_Implicit(target), activeItem.Value, handName, checkActionBlocker: false, target.Comp))
				{
					_popupSystem.PopupCursor(base.Loc.GetString("strippable-component-cannot-put-message", (ValueTuple<string, object>)("owner", Identity.Entity(Entity<HandsComponent>.op_Implicit(target), (IEntityManager)(object)base.EntityManager))));
					return false;
				}
				return true;
			}
		}
		return false;
	}

	private void StartStripInsertHand(Entity<HandsComponent?> user, Entity<HandsComponent?> target, EntityUid held, string handName, StrippableComponent? targetStrippable = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01db: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_021c: Unknown result type (might be due to invalid IL or missing references)
		//IL_021d: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<HandsComponent>(Entity<HandsComponent>.op_Implicit(user), ref user.Comp, true) && ((EntitySystem)this).Resolve<HandsComponent>(Entity<HandsComponent>.op_Implicit(target), ref target.Comp, true) && ((EntitySystem)this).Resolve<StrippableComponent>(Entity<HandsComponent>.op_Implicit(target), ref targetStrippable, true) && CanStripInsertHand(user, target, held, handName))
		{
			var (time, stealth) = GetStripTimeModifiers(Entity<HandsComponent>.op_Implicit(user), Entity<HandsComponent>.op_Implicit(target), null, targetStrippable.HandStripDelay);
			if (!stealth)
			{
				_popupSystem.PopupEntity(base.Loc.GetString("strippable-component-alert-owner-insert-hand", (ValueTuple<string, object>)("user", Identity.Entity(Entity<HandsComponent>.op_Implicit(user), (IEntityManager)(object)base.EntityManager)), (ValueTuple<string, object>)("item", _handsSystem.GetActiveItem(user).Value)), Entity<HandsComponent>.op_Implicit(target), Entity<HandsComponent>.op_Implicit(target), PopupType.Large);
			}
			string prefix = (stealth ? "stealthily " : "");
			ISharedAdminLogManager adminLogger = _adminLogger;
			LogStringHandler handler = new LogStringHandler(41, 4);
			handler.AppendFormatted(((EntitySystem)this).ToPrettyString((EntityUid?)Entity<HandsComponent>.op_Implicit(user), (MetaDataComponent)null), "actor", "ToPrettyString(user)");
			handler.AppendLiteral(" is trying to ");
			handler.AppendFormatted(prefix);
			handler.AppendLiteral("place the item ");
			handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(held)), "item", "ToPrettyString(held)");
			handler.AppendLiteral(" in ");
			handler.AppendFormatted(((EntitySystem)this).ToPrettyString((EntityUid?)Entity<HandsComponent>.op_Implicit(target), (MetaDataComponent)null), "target", "ToPrettyString(target)");
			handler.AppendLiteral("'s hands");
			adminLogger.Add(LogType.Stripping, LogImpact.Low, ref handler);
			DoAfterArgs doAfterArgs = new DoAfterArgs((IEntityManager)(object)base.EntityManager, Entity<HandsComponent>.op_Implicit(user), time, new StrippableDoAfterEvent(insertOrRemove: true, inventoryOrHand: false, handName), Entity<HandsComponent>.op_Implicit(user), Entity<HandsComponent>.op_Implicit(target), held)
			{
				Hidden = stealth,
				AttemptFrequency = AttemptFrequency.EveryTick,
				BreakOnDamage = true,
				BreakOnMove = true,
				NeedHand = true,
				DuplicateCondition = DuplicateConditions.SameTool,
				ForceVisible = (user != target)
			};
			_doAfterSystem.TryStartDoAfter(doAfterArgs);
		}
	}

	private void StripInsertHand(Entity<HandsComponent?> user, Entity<HandsComponent?> target, EntityUid held, string handName, bool stealth)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<HandsComponent>(Entity<HandsComponent>.op_Implicit(user), ref user.Comp, true) && ((EntitySystem)this).Resolve<HandsComponent>(Entity<HandsComponent>.op_Implicit(target), ref target.Comp, true) && CanStripInsertHand(user, target, held, handName))
		{
			_handsSystem.TryDrop(user, null, false);
			_handsSystem.TryPickup(Entity<HandsComponent>.op_Implicit(target), held, handName, checkActionBlocker: false, stealth, !stealth, target.Comp);
			ISharedAdminLogManager adminLogger = _adminLogger;
			LogStringHandler handler = new LogStringHandler(33, 3);
			handler.AppendFormatted(((EntitySystem)this).ToPrettyString((EntityUid?)Entity<HandsComponent>.op_Implicit(user), (MetaDataComponent)null), "actor", "ToPrettyString(user)");
			handler.AppendLiteral(" has placed the item ");
			handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(held)), "item", "ToPrettyString(held)");
			handler.AppendLiteral(" in ");
			handler.AppendFormatted(((EntitySystem)this).ToPrettyString((EntityUid?)Entity<HandsComponent>.op_Implicit(target), (MetaDataComponent)null), "target", "ToPrettyString(target)");
			handler.AppendLiteral("'s hands");
			adminLogger.Add(LogType.Stripping, LogImpact.Medium, ref handler);
		}
	}

	private bool CanStripRemoveHand(EntityUid user, Entity<HandsComponent?> target, EntityUid item, string handName)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<HandsComponent>(Entity<HandsComponent>.op_Implicit(target), ref target.Comp, true))
		{
			return false;
		}
		if (!target.Comp.CanBeStripped)
		{
			return false;
		}
		if (!_handsSystem.TryGetHand(target, handName, out var _))
		{
			_popupSystem.PopupCursor(base.Loc.GetString("strippable-component-item-slot-free-message", (ValueTuple<string, object>)("owner", Identity.Entity(Entity<HandsComponent>.op_Implicit(target), (IEntityManager)(object)base.EntityManager))));
			return false;
		}
		if (!_handsSystem.TryGetHeldItem(target, handName, out var heldEntity))
		{
			return false;
		}
		if (((EntitySystem)this).HasComp<VirtualItemComponent>(heldEntity))
		{
			return false;
		}
		EntityUid? val = heldEntity;
		if (!val.HasValue || val.GetValueOrDefault() != item)
		{
			return false;
		}
		if (!_handsSystem.CanDropHeld(Entity<HandsComponent>.op_Implicit(target), handName, checkActionBlocker: false))
		{
			_popupSystem.PopupCursor(base.Loc.GetString("strippable-component-cannot-drop-message", (ValueTuple<string, object>)("owner", Identity.Entity(Entity<HandsComponent>.op_Implicit(target), (IEntityManager)(object)base.EntityManager))));
			return false;
		}
		return true;
	}

	private void StartStripRemoveHand(Entity<HandsComponent?> user, Entity<HandsComponent?> target, EntityUid item, string handName, StrippableComponent? targetStrippable = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01da: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_022b: Unknown result type (might be due to invalid IL or missing references)
		//IL_022c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0231: Unknown result type (might be due to invalid IL or missing references)
		//IL_0236: Unknown result type (might be due to invalid IL or missing references)
		//IL_024d: Unknown result type (might be due to invalid IL or missing references)
		//IL_024e: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<HandsComponent>(Entity<HandsComponent>.op_Implicit(user), ref user.Comp, true) && ((EntitySystem)this).Resolve<HandsComponent>(Entity<HandsComponent>.op_Implicit(target), ref target.Comp, true) && ((EntitySystem)this).Resolve<StrippableComponent>(Entity<HandsComponent>.op_Implicit(target), ref targetStrippable, true) && CanStripRemoveHand(Entity<HandsComponent>.op_Implicit(user), target, item, handName))
		{
			var (time, stealth) = GetStripTimeModifiers(Entity<HandsComponent>.op_Implicit(user), Entity<HandsComponent>.op_Implicit(target), null, targetStrippable.HandStripDelay);
			if (!stealth)
			{
				_popupSystem.PopupEntity(base.Loc.GetString("strippable-component-alert-owner", (ValueTuple<string, object>)("user", Identity.Entity(Entity<HandsComponent>.op_Implicit(user), (IEntityManager)(object)base.EntityManager)), (ValueTuple<string, object>)("item", item)), Entity<HandsComponent>.op_Implicit(target), Entity<HandsComponent>.op_Implicit(target));
			}
			string prefix = (stealth ? "stealthily " : "");
			ISharedAdminLogManager adminLogger = _adminLogger;
			LogStringHandler handler = new LogStringHandler(43, 4);
			handler.AppendFormatted(((EntitySystem)this).ToPrettyString((EntityUid?)Entity<HandsComponent>.op_Implicit(user), (MetaDataComponent)null), "actor", "ToPrettyString(user)");
			handler.AppendLiteral(" is trying to ");
			handler.AppendFormatted(prefix);
			handler.AppendLiteral("strip the item ");
			handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(item)), "item", "ToPrettyString(item)");
			handler.AppendLiteral(" from ");
			handler.AppendFormatted(((EntitySystem)this).ToPrettyString((EntityUid?)Entity<HandsComponent>.op_Implicit(target), (MetaDataComponent)null), "target", "ToPrettyString(target)");
			handler.AppendLiteral("'s hands");
			adminLogger.Add(LogType.Stripping, LogImpact.Low, ref handler);
			_interactionSystem.DoContactInteraction(Entity<HandsComponent>.op_Implicit(user), item);
			DoAfterArgs doAfterArgs = new DoAfterArgs((IEntityManager)(object)base.EntityManager, Entity<HandsComponent>.op_Implicit(user), time, new StrippableDoAfterEvent(insertOrRemove: false, inventoryOrHand: false, handName), Entity<HandsComponent>.op_Implicit(user), Entity<HandsComponent>.op_Implicit(target), item)
			{
				Hidden = stealth,
				AttemptFrequency = AttemptFrequency.EveryTick,
				BreakOnDamage = true,
				BreakOnMove = true,
				NeedHand = true,
				BreakOnHandChange = false,
				DuplicateCondition = (_skills.HasSkill(Entity<SkillsComponent>.op_Implicit(user.Owner), MultiStripSkill, 2) ? DuplicateConditions.SameTool : DuplicateConditions.SameEvent),
				ForceVisible = (user != target)
			};
			_doAfterSystem.TryStartDoAfter(doAfterArgs);
		}
	}

	private void StripRemoveHand(Entity<HandsComponent?> user, Entity<HandsComponent?> target, EntityUid item, string handName, bool stealth)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<HandsComponent>(Entity<HandsComponent>.op_Implicit(user), ref user.Comp, true) && ((EntitySystem)this).Resolve<HandsComponent>(Entity<HandsComponent>.op_Implicit(target), ref target.Comp, true) && CanStripRemoveHand(Entity<HandsComponent>.op_Implicit(user), target, item, handName))
		{
			_handsSystem.TryDrop(target, item, null, checkActionBlocker: false);
			_handsSystem.PickupOrDrop(Entity<HandsComponent>.op_Implicit(user), item, checkActionBlocker: true, stealth, !stealth, dropNear: false, user.Comp);
			ISharedAdminLogManager adminLogger = _adminLogger;
			LogStringHandler handler = new LogStringHandler(37, 3);
			handler.AppendFormatted(((EntitySystem)this).ToPrettyString((EntityUid?)Entity<HandsComponent>.op_Implicit(user), (MetaDataComponent)null), "actor", "ToPrettyString(user)");
			handler.AppendLiteral(" has stripped the item ");
			handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(item)), "item", "ToPrettyString(item)");
			handler.AppendLiteral(" from ");
			handler.AppendFormatted(((EntitySystem)this).ToPrettyString((EntityUid?)Entity<HandsComponent>.op_Implicit(target), (MetaDataComponent)null), "target", "ToPrettyString(target)");
			handler.AppendLiteral("'s hands");
			adminLogger.Add(LogType.Stripping, LogImpact.High, ref handler);
		}
	}

	private void OnStrippableDoAfterRunning(Entity<HandsComponent> entity, ref DoAfterAttemptEvent<StrippableDoAfterEvent> ev)
	{
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		DoAfterArgs args = ev.DoAfter.Args;
		if (ev.Event.InventoryOrHand)
		{
			if ((ev.Event.InsertOrRemove && !CanStripInsertInventory(Entity<HandsComponent>.op_Implicit((entity.Owner, entity.Comp)), args.Target.Value, args.Used.Value, ev.Event.SlotOrHandName)) || (!ev.Event.InsertOrRemove && !CanStripRemoveInventory(entity.Owner, args.Target.Value, args.Used.Value, ev.Event.SlotOrHandName)))
			{
				((CancellableEntityEventArgs)ev).Cancel();
			}
		}
		else if ((ev.Event.InsertOrRemove && !CanStripInsertHand(Entity<HandsComponent>.op_Implicit((entity.Owner, entity.Comp)), Entity<HandsComponent>.op_Implicit(args.Target.Value), args.Used.Value, ev.Event.SlotOrHandName)) || (!ev.Event.InsertOrRemove && !CanStripRemoveHand(entity.Owner, Entity<HandsComponent>.op_Implicit(args.Target.Value), args.Used.Value, ev.Event.SlotOrHandName)))
		{
			((CancellableEntityEventArgs)ev).Cancel();
		}
	}

	private void OnStrippableDoAfterFinished(Entity<HandsComponent> entity, ref StrippableDoAfterEvent ev)
	{
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		if (ev.Cancelled)
		{
			return;
		}
		if (ev.InventoryOrHand)
		{
			if (ev.InsertOrRemove)
			{
				StripInsertInventory(Entity<HandsComponent>.op_Implicit((entity.Owner, entity.Comp)), ev.Target.Value, ev.Used.Value, ev.SlotOrHandName);
			}
			else
			{
				StripRemoveInventory(entity.Owner, ev.Target.Value, ev.Used.Value, ev.SlotOrHandName, ev.Args.Hidden);
			}
		}
		else if (ev.InsertOrRemove)
		{
			StripInsertHand(Entity<HandsComponent>.op_Implicit((entity.Owner, entity.Comp)), Entity<HandsComponent>.op_Implicit(ev.Target.Value), ev.Used.Value, ev.SlotOrHandName, ev.Args.Hidden);
		}
		else
		{
			StripRemoveHand(Entity<HandsComponent>.op_Implicit((entity.Owner, entity.Comp)), Entity<HandsComponent>.op_Implicit(ev.Target.Value), ev.Used.Value, ev.SlotOrHandName, ev.Args.Hidden);
		}
	}

	private void OnActivateInWorld(EntityUid uid, StrippableComponent component, ActivateInWorldEvent args)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		if (!((HandledEntityEventArgs)args).Handled && args.Complex && !(args.Target == args.User) && TryOpenStrippingUi(args.User, Entity<StrippableComponent>.op_Implicit((uid, component))))
		{
			((HandledEntityEventArgs)args).Handled = true;
		}
	}

	public (TimeSpan Time, bool Stealth) GetStripTimeModifiers(EntityUid user, EntityUid targetPlayer, EntityUid? targetItem, TimeSpan initialTime)
	{
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		BeforeItemStrippedEvent itemEv = new BeforeItemStrippedEvent(initialTime);
		if (targetItem.HasValue)
		{
			((EntitySystem)this).RaiseLocalEvent<BeforeItemStrippedEvent>(targetItem.Value, ref itemEv, false);
		}
		BeforeStripEvent userEv = new BeforeStripEvent(itemEv.Time, itemEv.Stealth);
		((EntitySystem)this).RaiseLocalEvent<BeforeStripEvent>(user, ref userEv, false);
		BeforeGettingStrippedEvent targetEv = new BeforeGettingStrippedEvent(userEv.Time, userEv.Stealth);
		((EntitySystem)this).RaiseLocalEvent<BeforeGettingStrippedEvent>(targetPlayer, ref targetEv, false);
		return (Time: targetEv.Time, Stealth: targetEv.Stealth);
	}

	private void OnDragDrop(EntityUid uid, StrippableComponent component, ref DragDropDraggedEvent args)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		if (!args.Handled && !(args.Target != args.User) && TryOpenStrippingUi(args.User, Entity<StrippableComponent>.op_Implicit((uid, component))))
		{
			args.Handled = true;
		}
	}

	public bool TryOpenStrippingUi(EntityUid user, Entity<StrippableComponent> target, bool openInCombat = false)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		CombatModeComponent mode = default(CombatModeComponent);
		if (!openInCombat && ((EntitySystem)this).TryComp<CombatModeComponent>(user, ref mode) && mode.IsInCombatMode)
		{
			return false;
		}
		if (!((EntitySystem)this).HasComp<StrippingComponent>(user))
		{
			return false;
		}
		_ui.OpenUi(Entity<UserInterfaceComponent>.op_Implicit(target.Owner), (Enum)StrippingUiKey.Key, (EntityUid?)user, false);
		return true;
	}

	private void OnCanDropOn(EntityUid uid, StrippingComponent component, ref CanDropTargetEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		bool val = uid == args.User && ((EntitySystem)this).HasComp<StrippableComponent>(args.Dragged) && ((EntitySystem)this).HasComp<HandsComponent>(args.User) && ((EntitySystem)this).HasComp<StrippingComponent>(args.User);
		args.Handled |= val;
		args.CanDrop |= val;
	}

	private void OnCanDrop(EntityUid uid, StrippableComponent component, ref CanDropDraggedEvent args)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		args.CanDrop |= args.Target == args.User && ((EntitySystem)this).HasComp<StrippingComponent>(args.User) && ((EntitySystem)this).HasComp<HandsComponent>(args.User);
		if (args.CanDrop)
		{
			args.Handled = true;
		}
	}

	public bool IsStripHidden(SlotDefinition definition, EntityUid? viewer)
	{
		if (!definition.StripHidden)
		{
			return false;
		}
		if (!viewer.HasValue)
		{
			return true;
		}
		return !((EntitySystem)this).HasComp<BypassInteractionChecksComponent>(viewer);
	}
}
