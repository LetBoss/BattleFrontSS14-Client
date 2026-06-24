using System;
using System.Collections.Generic;
using System.Numerics;
using Content.Shared.Examine;
using Content.Shared.Hands.Components;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Interaction;
using Content.Shared.Popups;
using Content.Shared.Storage.EntitySystems;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Physics;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Systems;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Stacks;

public abstract class SharedStackSystem : EntitySystem
{
	[Dependency]
	private IGameTiming _gameTiming;

	[Dependency]
	private IPrototypeManager _prototype;

	[Dependency]
	private IViewVariablesManager _vvm;

	[Dependency]
	protected SharedAppearanceSystem Appearance;

	[Dependency]
	protected SharedHandsSystem Hands;

	[Dependency]
	protected SharedTransformSystem Xform;

	[Dependency]
	private EntityLookupSystem _entityLookup;

	[Dependency]
	private SharedPhysicsSystem _physics;

	[Dependency]
	protected SharedPopupSystem Popup;

	[Dependency]
	private SharedStorageSystem _storage;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<StackComponent, ComponentGetState>((ComponentEventRefHandler<StackComponent, ComponentGetState>)OnStackGetState, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<StackComponent, ComponentHandleState>((ComponentEventRefHandler<StackComponent, ComponentHandleState>)OnStackHandleState, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<StackComponent, ComponentStartup>((ComponentEventHandler<StackComponent, ComponentStartup>)OnStackStarted, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<StackComponent, ExaminedEvent>((ComponentEventHandler<StackComponent, ExaminedEvent>)OnStackExamined, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<StackComponent, InteractUsingEvent>((ComponentEventHandler<StackComponent, InteractUsingEvent>)OnStackInteractUsing, (Type[])null, (Type[])null);
		_vvm.GetTypeHandler<StackComponent>().AddPath<int>("Count", (ComponentPropertyGetter<StackComponent, int>)((EntityUid _, StackComponent comp) => comp.Count), (ComponentPropertySetter<StackComponent, int>)SetCount);
	}

	public override void Shutdown()
	{
		((EntitySystem)this).Shutdown();
		_vvm.GetTypeHandler<StackComponent>().RemovePath("Count");
	}

	private void OnStackInteractUsing(EntityUid uid, StackComponent stack, InteractUsingEvent args)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		StackComponent recipientStack = default(StackComponent);
		if (((HandledEntityEventArgs)args).Handled || !((EntitySystem)this).TryComp<StackComponent>(args.Used, ref recipientStack))
		{
			return;
		}
		Angle localRotation = ((EntitySystem)this).Transform(args.Used).LocalRotation;
		if (!TryMergeStacks(uid, args.Used, out var transfered, stack, recipientStack))
		{
			return;
		}
		((HandledEntityEventArgs)args).Handled = true;
		if (!_gameTiming.IsFirstTimePredicted)
		{
			return;
		}
		EntityCoordinates popupPos = args.ClickLocation;
		EntityCoordinates userCoords = ((EntitySystem)this).Transform(args.User).Coordinates;
		if (!((EntityCoordinates)(ref popupPos)).IsValid((IEntityManager)(object)base.EntityManager))
		{
			popupPos = userCoords;
		}
		int num = transfered;
		if (num <= 0)
		{
			if (num == 0 && GetAvailableSpace(recipientStack) == 0)
			{
				Popup.PopupCoordinates(base.Loc.GetString("comp-stack-already-full"), popupPos, Filter.Local(), recordReplay: false);
			}
		}
		else
		{
			Popup.PopupCoordinates($"+{transfered}", popupPos, Filter.Local(), recordReplay: false);
			if (GetAvailableSpace(recipientStack) == 0)
			{
				Popup.PopupCoordinates(base.Loc.GetString("comp-stack-becomes-full"), ((EntityCoordinates)(ref popupPos)).Offset(new Vector2(0f, -0.5f)), Filter.Local(), recordReplay: false);
			}
		}
		_storage.PlayPickupAnimation(args.Used, popupPos, userCoords, localRotation, args.User);
	}

	private bool TryMergeStacks(EntityUid donor, EntityUid recipient, out int transferred, StackComponent? donorStack = null, StackComponent? recipientStack = null)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		transferred = 0;
		if (donor == recipient)
		{
			return false;
		}
		if (!((EntitySystem)this).Resolve<StackComponent>(recipient, ref recipientStack, false) || !((EntitySystem)this).Resolve<StackComponent>(donor, ref donorStack, false))
		{
			return false;
		}
		if (string.IsNullOrEmpty(recipientStack.StackTypeId) || !recipientStack.StackTypeId.Equals(donorStack.StackTypeId))
		{
			return false;
		}
		transferred = Math.Min(donorStack.Count, GetAvailableSpace(recipientStack));
		SetCount(donor, donorStack.Count - transferred, donorStack);
		SetCount(recipient, recipientStack.Count + transferred, recipientStack);
		return transferred > 0;
	}

	public void TryMergeToHands(EntityUid item, EntityUid user, StackComponent? itemStack = null, HandsComponent? hands = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<HandsComponent>(user, ref hands, false))
		{
			return;
		}
		if (!((EntitySystem)this).Resolve<StackComponent>(item, ref itemStack, false))
		{
			Hands.PickupOrDrop(user, item, checkActionBlocker: true, animateUser: false, animate: true, dropNear: false, hands);
			return;
		}
		foreach (EntityUid held in Hands.EnumerateHeld(Entity<HandsComponent>.op_Implicit((user, hands))))
		{
			TryMergeStacks(item, held, out var _, itemStack);
			if (itemStack.Count == 0)
			{
				return;
			}
		}
		Hands.PickupOrDrop(user, item, checkActionBlocker: true, animateUser: false, animate: true, dropNear: false, hands);
	}

	public virtual void SetCount(EntityUid uid, int amount, StackComponent? component = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<StackComponent>(uid, ref component, true) && amount != component.Count)
		{
			int old = component.Count;
			amount = Math.Min(amount, GetMaxCount(component));
			amount = Math.Max(amount, 0);
			component.Count = amount;
			((EntitySystem)this).Dirty(uid, (IComponent)(object)component, (MetaDataComponent)null);
			Appearance.SetData(uid, (Enum)StackVisuals.Actual, (object)component.Count, (AppearanceComponent)null);
			((EntitySystem)this).RaiseLocalEvent<StackCountChangedEvent>(uid, new StackCountChangedEvent(old, component.Count), false);
		}
	}

	public bool Use(EntityUid uid, int amount, StackComponent? stack = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<StackComponent>(uid, ref stack, true))
		{
			return false;
		}
		if (stack.Count < amount)
		{
			return false;
		}
		if (!stack.Unlimited)
		{
			SetCount(uid, stack.Count - amount, stack);
		}
		return true;
	}

	public bool TryMergeToContacts(EntityUid uid, StackComponent? stack = null, TransformComponent? xform = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<StackComponent, TransformComponent>(uid, ref stack, ref xform, false))
		{
			return false;
		}
		MapId map = xform.MapID;
		Box2 bounds = _physics.GetWorldAABB(uid, (FixturesComponent)null, (PhysicsComponent)null, (TransformComponent)null);
		HashSet<Entity<StackComponent>> intersecting = new HashSet<Entity<StackComponent>>();
		_entityLookup.GetEntitiesIntersecting<StackComponent>(map, bounds, intersecting, (LookupFlags)10);
		bool merged = false;
		foreach (Entity<StackComponent> otherStack in intersecting)
		{
			EntityUid otherEnt = otherStack.Owner;
			if (!((EntitySystem)this).TerminatingOrDeleted(otherEnt, (MetaDataComponent)null) && !base.EntityManager.IsQueuedForDeletion(otherEnt) && TryMergeStacks(uid, otherEnt, out var _, stack, Entity<StackComponent>.op_Implicit(otherStack)))
			{
				merged = true;
				if (stack.Count <= 0)
				{
					break;
				}
			}
		}
		return merged;
	}

	public int GetCount(EntityUid uid, StackComponent? component = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<StackComponent>(uid, ref component, false))
		{
			return 1;
		}
		return component.Count;
	}

	public int GetMaxCount(string entityId)
	{
		StackComponent stackComp = default(StackComponent);
		_prototype.Index<EntityPrototype>(entityId).TryGetComponent<StackComponent>(ref stackComp, base.EntityManager.ComponentFactory);
		return GetMaxCount(stackComp);
	}

	public int GetMaxCount(EntityUid uid)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		return GetMaxCount(((EntitySystem)this).CompOrNull<StackComponent>(uid));
	}

	public int GetMaxCount(StackComponent? component)
	{
		if (component == null)
		{
			return 1;
		}
		if (component.MaxCountOverride.HasValue)
		{
			return component.MaxCountOverride.Value;
		}
		if (string.IsNullOrEmpty(component.StackTypeId))
		{
			return 1;
		}
		return _prototype.Index<StackPrototype>(component.StackTypeId).MaxCount ?? int.MaxValue;
	}

	public int GetAvailableSpace(StackComponent component)
	{
		return GetMaxCount(component) - component.Count;
	}

	public bool TryAdd(EntityUid insertEnt, EntityUid targetEnt, StackComponent? insertStack = null, StackComponent? targetStack = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<StackComponent>(insertEnt, ref insertStack, true) || !((EntitySystem)this).Resolve<StackComponent>(targetEnt, ref targetStack, true))
		{
			return false;
		}
		int count = insertStack.Count;
		return TryAdd(insertEnt, targetEnt, count, insertStack, targetStack);
	}

	public bool TryAdd(EntityUid insertEnt, EntityUid targetEnt, int count, StackComponent? insertStack = null, StackComponent? targetStack = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<StackComponent>(insertEnt, ref insertStack, true) || !((EntitySystem)this).Resolve<StackComponent>(targetEnt, ref targetStack, true))
		{
			return false;
		}
		if (insertStack.StackTypeId != targetStack.StackTypeId)
		{
			return false;
		}
		int available = GetAvailableSpace(targetStack);
		if (available <= 0)
		{
			return false;
		}
		int change = Math.Min(available, count);
		SetCount(targetEnt, targetStack.Count + change, targetStack);
		SetCount(insertEnt, insertStack.Count - change, insertStack);
		return true;
	}

	private void OnStackStarted(EntityUid uid, StackComponent component, ComponentStartup args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		SetCount(uid, component.Count, component);
		AppearanceComponent appearance = default(AppearanceComponent);
		if (((EntitySystem)this).TryComp<AppearanceComponent>(uid, ref appearance))
		{
			Appearance.SetData(uid, (Enum)StackVisuals.Actual, (object)component.Count, appearance);
			Appearance.SetData(uid, (Enum)StackVisuals.MaxCount, (object)GetMaxCount(component), appearance);
			Appearance.SetData(uid, (Enum)StackVisuals.Hide, (object)false, appearance);
		}
	}

	private void OnStackGetState(EntityUid uid, StackComponent component, ref ComponentGetState args)
	{
		((ComponentGetState)(ref args)).State = (IComponentState)(object)new StackComponentState(component.Count, component.MaxCountOverride, component.Lingering);
	}

	private void OnStackHandleState(EntityUid uid, StackComponent component, ref ComponentHandleState args)
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		if (((ComponentHandleState)(ref args)).Current is StackComponentState cast)
		{
			component.MaxCountOverride = cast.MaxCount;
			component.Lingering = cast.Lingering;
			SetCount(uid, cast.Count, component);
		}
	}

	private void OnStackExamined(EntityUid uid, StackComponent component, ExaminedEvent args)
	{
		if (args.IsInDetailsRange)
		{
			args.PushMarkup(base.Loc.GetString("comp-stack-examine-detail-count", (ValueTuple<string, object>)("count", component.Count), (ValueTuple<string, object>)("markupCountColor", "lightgray")));
		}
	}
}
