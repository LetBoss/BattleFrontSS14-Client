using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Numerics;
using Content.Shared._RMC14.Hands;
using Content.Shared._RMC14.Inventory;
using Content.Shared.ActionBlocker;
using Content.Shared.Administration.Logs;
using Content.Shared.Atmos;
using Content.Shared.Camera;
using Content.Shared.Cuffs;
using Content.Shared.Database;
using Content.Shared.Examine;
using Content.Shared.Hands.Components;
using Content.Shared.IdentityManagement;
using Content.Shared.Input;
using Content.Shared.Interaction;
using Content.Shared.Inventory;
using Content.Shared.Inventory.VirtualItem;
using Content.Shared.Item;
using Content.Shared.Localizations;
using Content.Shared.Movement.Systems;
using Content.Shared.Projectiles;
using Content.Shared.Storage.EntitySystems;
using Content.Shared.Tag;
using Content.Shared.Weapons.Ranged.Events;
using Content.Shared.Wieldable;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.Input.Binding;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Physics.Components;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;

namespace Content.Shared.Hands.EntitySystems;

public abstract class SharedHandsSystem : EntitySystem
{
	[Dependency]
	private ISharedAdminLogManager _adminLogger;

	[Dependency]
	private ActionBlockerSystem _actionBlocker;

	[Dependency]
	protected SharedContainerSystem ContainerSystem;

	[Dependency]
	private SharedInteractionSystem _interactionSystem;

	[Dependency]
	private InventorySystem _inventory;

	[Dependency]
	private SharedStorageSystem _storage;

	[Dependency]
	protected SharedTransformSystem TransformSystem;

	[Dependency]
	private SharedVirtualItemSystem _virtualSystem;

	[Dependency]
	private TagSystem _tagSystem;

	private static readonly ProtoId<TagPrototype> BypassDropChecksTag = ProtoId<TagPrototype>.op_Implicit("BypassDropChecks");

	[Dependency]
	private RMCHandsSystem _rmcHands;

	public const float MaxAnimationRange = 10f;

	public event Action<Entity<HandsComponent>, string, HandLocation>? OnPlayerAddHand;

	public event Action<Entity<HandsComponent>, string>? OnPlayerRemoveHand;

	protected event Action<Entity<HandsComponent>?>? OnHandSetActive;

	public bool TrySelect(EntityUid uid, EntityUid? entity, HandsComponent? handsComp = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<HandsComponent>(uid, ref handsComp, false))
		{
			return false;
		}
		if (!IsHolding(Entity<HandsComponent>.op_Implicit((uid, handsComp)), entity, out string hand))
		{
			return false;
		}
		SetActiveHand(Entity<HandsComponent>.op_Implicit((uid, handsComp)), hand);
		return true;
	}

	public bool TrySelect<TComponent>(EntityUid uid, [NotNullWhen(true)] out TComponent? component, HandsComponent? handsComp = null) where TComponent : Component
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		component = default(TComponent);
		if (!((EntitySystem)this).Resolve<HandsComponent>(uid, ref handsComp, false))
		{
			return false;
		}
		foreach (string hand in handsComp.Hands.Keys)
		{
			if (TryGetHeldItem(Entity<HandsComponent>.op_Implicit((uid, handsComp)), hand, out var held) && ((EntitySystem)this).TryComp<TComponent>(held, ref component))
			{
				return true;
			}
		}
		return false;
	}

	public bool TrySelectEmptyHand(EntityUid uid, HandsComponent? handsComp = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		return TrySelect(uid, null, handsComp);
	}

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		InitializeInteractions();
		InitializeDrop();
		InitializePickup();
		InitializeRelay();
		((EntitySystem)this).SubscribeLocalEvent<HandsComponent, ComponentInit>((EntityEventRefHandler<HandsComponent, ComponentInit>)OnInit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<HandsComponent, MapInitEvent>((EntityEventRefHandler<HandsComponent, MapInitEvent>)OnMapInit, (Type[])null, (Type[])null);
	}

	public override void Shutdown()
	{
		((EntitySystem)this).Shutdown();
		CommandBinds.Unregister<SharedHandsSystem>();
	}

	private void OnInit(Entity<HandsComponent> ent, ref ComponentInit args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		ContainerManagerComponent container = ((EntitySystem)this).EnsureComp<ContainerManagerComponent>(Entity<HandsComponent>.op_Implicit(ent));
		foreach (string id in ent.Comp.Hands.Keys)
		{
			ContainerSystem.EnsureContainer<ContainerSlot>(Entity<HandsComponent>.op_Implicit(ent), id, container);
		}
	}

	private void OnMapInit(Entity<HandsComponent> ent, ref MapInitEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		if (ent.Comp.ActiveHandId == null)
		{
			SetActiveHand(ent.AsNullable(), ent.Comp.SortedHands.FirstOrDefault());
		}
	}

	public void AddHand(Entity<HandsComponent?> ent, string handName, HandLocation handLocation)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		AddHand(ent, handName, new Hand(handLocation));
	}

	public void AddHand(Entity<HandsComponent?> ent, string handName, Hand hand)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<HandsComponent>(Entity<HandsComponent>.op_Implicit(ent), ref ent.Comp, false) && !ent.Comp.Hands.ContainsKey(handName))
		{
			((BaseContainer)ContainerSystem.EnsureContainer<ContainerSlot>(Entity<HandsComponent>.op_Implicit(ent), handName, (ContainerManagerComponent)null)).OccludesLight = false;
			ent.Comp.Hands.Add(handName, hand);
			ent.Comp.SortedHands.Add(handName);
			((EntitySystem)this).Dirty<HandsComponent>(ent, (MetaDataComponent)null);
			this.OnPlayerAddHand?.Invoke(Entity<HandsComponent>.op_Implicit((Entity<HandsComponent>.op_Implicit(ent), ent.Comp)), handName, hand.Location);
			if (ent.Comp.ActiveHandId == null)
			{
				SetActiveHand(ent, handName);
			}
			((EntitySystem)this).RaiseLocalEvent<HandCountChangedEvent>(Entity<HandsComponent>.op_Implicit(ent), new HandCountChangedEvent(Entity<HandsComponent>.op_Implicit(ent)), false);
		}
	}

	public virtual void RemoveHand(Entity<HandsComponent?> ent, string handName)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<HandsComponent>(Entity<HandsComponent>.op_Implicit(ent), ref ent.Comp, false))
		{
			return;
		}
		this.OnPlayerRemoveHand?.Invoke(Entity<HandsComponent>.op_Implicit((Entity<HandsComponent>.op_Implicit(ent), ent.Comp)), handName);
		TryDrop(ent, handName, null, checkActionBlocker: false);
		if (ent.Comp.Hands.Remove(handName))
		{
			BaseContainer container = default(BaseContainer);
			if (ContainerSystem.TryGetContainer(Entity<HandsComponent>.op_Implicit(ent), handName, ref container, (ContainerManagerComponent)null))
			{
				ContainerSystem.ShutdownContainer(container);
			}
			ent.Comp.SortedHands.Remove(handName);
			if (ent.Comp.ActiveHandId == handName)
			{
				TrySetActiveHand(ent, ent.Comp.SortedHands.FirstOrDefault());
			}
			((EntitySystem)this).RaiseLocalEvent<HandCountChangedEvent>(Entity<HandsComponent>.op_Implicit(ent), new HandCountChangedEvent(Entity<HandsComponent>.op_Implicit(ent)), false);
			((EntitySystem)this).Dirty<HandsComponent>(ent, (MetaDataComponent)null);
		}
	}

	public void RemoveHands(Entity<HandsComponent?> ent)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<HandsComponent>(Entity<HandsComponent>.op_Implicit(ent), ref ent.Comp, false))
		{
			return;
		}
		foreach (string handId in new List<string>(ent.Comp.Hands.Keys))
		{
			RemoveHand(ent, handId);
		}
	}

	private void HandleSetHand(RequestSetHandEvent msg, EntitySessionEventArgs eventArgs)
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySessionEventArgs)(ref eventArgs)).SenderSession.AttachedEntity.HasValue)
		{
			TrySetActiveHand(Entity<HandsComponent>.op_Implicit(((EntitySessionEventArgs)(ref eventArgs)).SenderSession.AttachedEntity.Value), msg.HandName);
		}
	}

	public bool TryGetEmptyHand(Entity<HandsComponent?> ent, [NotNullWhen(true)] out string? emptyHand)
	{
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		emptyHand = null;
		if (!((EntitySystem)this).Resolve<HandsComponent>(Entity<HandsComponent>.op_Implicit(ent), ref ent.Comp, false))
		{
			return false;
		}
		foreach (string hand in EnumerateHands(ent))
		{
			if (HandIsEmpty(ent, hand))
			{
				emptyHand = hand;
				return true;
			}
		}
		return false;
	}

	public bool TryGetActiveItem(Entity<HandsComponent?> entity, [NotNullWhen(true)] out EntityUid? item)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		item = null;
		if (!((EntitySystem)this).Resolve<HandsComponent>(Entity<HandsComponent>.op_Implicit(entity), ref entity.Comp, false))
		{
			return false;
		}
		if (!TryGetHeldItem(entity, entity.Comp.ActiveHandId, out var held))
		{
			return false;
		}
		item = held;
		return true;
	}

	public EntityUid GetActiveItemOrSelf(Entity<HandsComponent?> entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		if (!TryGetActiveItem(entity, out var item))
		{
			return entity.Owner;
		}
		return item.Value;
	}

	public string? GetActiveHand(Entity<HandsComponent?> entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<HandsComponent>(Entity<HandsComponent>.op_Implicit(entity), ref entity.Comp, false))
		{
			return null;
		}
		return entity.Comp.ActiveHandId;
	}

	public EntityUid? GetActiveItem(Entity<HandsComponent?> entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<HandsComponent>(Entity<HandsComponent>.op_Implicit(entity), ref entity.Comp, false))
		{
			return null;
		}
		return GetHeldItem(entity, entity.Comp.ActiveHandId);
	}

	public bool ActiveHandIsEmpty(Entity<HandsComponent?> entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		return !GetActiveItem(entity).HasValue;
	}

	public IEnumerable<string> EnumerateHands(Entity<HandsComponent?> ent)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<HandsComponent>(Entity<HandsComponent>.op_Implicit(ent), ref ent.Comp, false))
		{
			yield break;
		}
		if (ent.Comp.ActiveHandId != null)
		{
			yield return ent.Comp.ActiveHandId;
		}
		foreach (string name in ent.Comp.SortedHands)
		{
			if (name != ent.Comp.ActiveHandId)
			{
				yield return name;
			}
		}
	}

	public IEnumerable<EntityUid> EnumerateHeld(Entity<HandsComponent?> ent)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<HandsComponent>(Entity<HandsComponent>.op_Implicit(ent), ref ent.Comp, false))
		{
			yield break;
		}
		if (TryGetActiveItem(ent, out var activeHeld))
		{
			yield return activeHeld.Value;
		}
		foreach (string name in ent.Comp.SortedHands)
		{
			if (!(name == ent.Comp.ActiveHandId) && TryGetHeldItem(ent, name, out var held))
			{
				yield return held.Value;
			}
		}
	}

	public bool TrySetActiveHand(Entity<HandsComponent?> ent, string? name)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<HandsComponent>(Entity<HandsComponent>.op_Implicit(ent), ref ent.Comp, false))
		{
			return false;
		}
		if (name == ent.Comp.ActiveHandId)
		{
			return false;
		}
		if (name != null && !ent.Comp.Hands.ContainsKey(name))
		{
			return false;
		}
		return SetActiveHand(ent, name);
	}

	public bool SetActiveHand(Entity<HandsComponent?> ent, string? handId)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<HandsComponent>(Entity<HandsComponent>.op_Implicit(ent), ref ent.Comp, true))
		{
			return false;
		}
		if (handId == ent.Comp.ActiveHandId)
		{
			return false;
		}
		if (TryGetActiveItem(ent, out var oldHeld))
		{
			((EntitySystem)this).RaiseLocalEvent<HandDeselectedEvent>(oldHeld.Value, new HandDeselectedEvent(Entity<HandsComponent>.op_Implicit(ent)), false);
		}
		if (handId == null)
		{
			ent.Comp.ActiveHandId = null;
			return true;
		}
		ent.Comp.ActiveHandId = handId;
		this.OnHandSetActive?.Invoke(Entity<HandsComponent>.op_Implicit((Entity<HandsComponent>.op_Implicit(ent), ent.Comp)));
		if (TryGetHeldItem(ent, handId, out var newHeld))
		{
			((EntitySystem)this).RaiseLocalEvent<HandSelectedEvent>(newHeld.Value, new HandSelectedEvent(Entity<HandsComponent>.op_Implicit(ent)), false);
		}
		((EntitySystem)this).Dirty<HandsComponent>(ent, (MetaDataComponent)null);
		return true;
	}

	public bool IsHolding(Entity<HandsComponent?> entity, [NotNullWhen(true)] EntityUid? item)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		string inHand;
		return IsHolding(entity, item, out inHand);
	}

	public bool IsHolding(Entity<HandsComponent?> ent, [NotNullWhen(true)] EntityUid? entity, [NotNullWhen(true)] out string? inHand)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		inHand = null;
		if (!entity.HasValue)
		{
			return false;
		}
		if (!((EntitySystem)this).Resolve<HandsComponent>(Entity<HandsComponent>.op_Implicit(ent), ref ent.Comp, false))
		{
			return false;
		}
		foreach (string hand in ent.Comp.Hands.Keys)
		{
			EntityUid? heldItem = GetHeldItem(ent, hand);
			EntityUid? val = entity;
			if (heldItem.HasValue == val.HasValue && (!heldItem.HasValue || heldItem.GetValueOrDefault() == val.GetValueOrDefault()))
			{
				inHand = hand;
				return true;
			}
		}
		return false;
	}

	public bool TryGetHand(Entity<HandsComponent?> ent, [NotNullWhen(true)] string? handId, [NotNullWhen(true)] out Hand? hand)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		hand = null;
		if (handId == null)
		{
			return false;
		}
		if (!((EntitySystem)this).Resolve<HandsComponent>(Entity<HandsComponent>.op_Implicit(ent), ref ent.Comp, false))
		{
			return false;
		}
		if (!ent.Comp.Hands.TryGetValue(handId, out var handsHand))
		{
			return false;
		}
		hand = handsHand;
		return true;
	}

	public EntityUid? GetHeldItem(Entity<HandsComponent?> ent, string? handId)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		TryGetHeldItem(ent, handId, out var held);
		return held;
	}

	public bool TryGetHeldItem(Entity<HandsComponent?> ent, string? handId, [NotNullWhen(true)] out EntityUid? held)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		held = null;
		if (!((EntitySystem)this).Resolve<HandsComponent>(Entity<HandsComponent>.op_Implicit(ent), ref ent.Comp, false))
		{
			return false;
		}
		if (handId == null || !ent.Comp.Hands.ContainsKey(handId))
		{
			return false;
		}
		BaseContainer container = default(BaseContainer);
		if (!ContainerSystem.TryGetContainer(Entity<HandsComponent>.op_Implicit(ent), handId, ref container, (ContainerManagerComponent)null))
		{
			return false;
		}
		held = Extensions.FirstOrNull<EntityUid>((IEnumerable<EntityUid>)container.ContainedEntities);
		return held.HasValue;
	}

	public bool HandIsEmpty(Entity<HandsComponent?> ent, string handId)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		return !GetHeldItem(ent, handId).HasValue;
	}

	public int GetHandCount(Entity<HandsComponent?> ent)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<HandsComponent>(Entity<HandsComponent>.op_Implicit(ent), ref ent.Comp, false))
		{
			return 0;
		}
		return ent.Comp.Hands.Count;
	}

	public int CountFreeHands(Entity<HandsComponent?> ent)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<HandsComponent>(Entity<HandsComponent>.op_Implicit(ent), ref ent.Comp, false))
		{
			return 0;
		}
		int free = 0;
		foreach (string name in ent.Comp.Hands.Keys)
		{
			if (HandIsEmpty(ent, name))
			{
				free++;
			}
		}
		return free;
	}

	public int CountFreeableHands(Entity<HandsComponent> hands, EntityUid except)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		int freeable = 0;
		foreach (string name in hands.Comp.Hands.Keys)
		{
			if (TryGetHeldItem(hands.AsNullable(), name, out var item))
			{
				EntityUid? val = item;
				if (val.HasValue && val.GetValueOrDefault() == except)
				{
					continue;
				}
			}
			if (HandIsEmpty(hands.AsNullable(), name) || CanDropHeld(Entity<HandsComponent>.op_Implicit(hands), name))
			{
				freeable++;
			}
		}
		return freeable;
	}

	private void InitializeDrop()
	{
		((EntitySystem)this).SubscribeLocalEvent<HandsComponent, EntRemovedFromContainerMessage>((ComponentEventHandler<HandsComponent, EntRemovedFromContainerMessage>)HandleEntityRemoved, (Type[])null, (Type[])null);
	}

	protected virtual void HandleEntityRemoved(EntityUid uid, HandsComponent hands, EntRemovedFromContainerMessage args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		if (TryGetHand(Entity<HandsComponent>.op_Implicit(uid), ((ContainerModifiedMessage)args).Container.ID, out var hand))
		{
			GotUnequippedHandEvent gotUnequipped = new GotUnequippedHandEvent(uid, ((ContainerModifiedMessage)args).Entity, hand.Value);
			((EntitySystem)this).RaiseLocalEvent<GotUnequippedHandEvent>(((ContainerModifiedMessage)args).Entity, gotUnequipped, false);
			DidUnequipHandEvent didUnequip = new DidUnequipHandEvent(uid, ((ContainerModifiedMessage)args).Entity, hand.Value);
			((EntitySystem)this).RaiseLocalEvent<DidUnequipHandEvent>(uid, didUnequip, false);
			VirtualItemComponent @virtual = default(VirtualItemComponent);
			if (((EntitySystem)this).TryComp<VirtualItemComponent>(((ContainerModifiedMessage)args).Entity, ref @virtual))
			{
				_virtualSystem.DeleteVirtualItem(Entity<VirtualItemComponent>.op_Implicit((((ContainerModifiedMessage)args).Entity, @virtual)), uid);
			}
		}
	}

	private bool ShouldIgnoreRestrictions(EntityUid user)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		return !_tagSystem.HasTag(user, BypassDropChecksTag);
	}

	public bool CanDrop(Entity<HandsComponent?> ent, EntityUid entity, bool checkActionBlocker = true)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<HandsComponent>(Entity<HandsComponent>.op_Implicit(ent), ref ent.Comp, false))
		{
			return false;
		}
		if (!IsHolding(ent, entity, out string hand))
		{
			return false;
		}
		return CanDropHeld(Entity<HandsComponent>.op_Implicit(ent), hand, checkActionBlocker);
	}

	public bool CanDropHeld(EntityUid uid, string handId, bool checkActionBlocker = true)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		BaseContainer container = default(BaseContainer);
		if (!ContainerSystem.TryGetContainer(uid, handId, ref container, (ContainerManagerComponent)null))
		{
			return false;
		}
		EntityUid? val = Extensions.FirstOrNull<EntityUid>((IEnumerable<EntityUid>)container.ContainedEntities);
		if (val.HasValue)
		{
			EntityUid held = val.GetValueOrDefault();
			if (!ContainerSystem.CanRemove(held, container))
			{
				return false;
			}
			if (checkActionBlocker && !_actionBlocker.CanDrop(uid, held))
			{
				return false;
			}
			return true;
		}
		return false;
	}

	public bool TryDrop(Entity<HandsComponent?> ent, EntityCoordinates? targetDropLocation = null, bool checkActionBlocker = true, bool doDropInteraction = true)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<HandsComponent>(Entity<HandsComponent>.op_Implicit(ent), ref ent.Comp, false))
		{
			return false;
		}
		if (ent.Comp.ActiveHandId == null)
		{
			return false;
		}
		return TryDrop(ent, ent.Comp.ActiveHandId, targetDropLocation, checkActionBlocker, doDropInteraction);
	}

	public bool TryDrop(Entity<HandsComponent?> ent, EntityUid entity, EntityCoordinates? targetDropLocation = null, bool checkActionBlocker = true, bool doDropInteraction = true)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<HandsComponent>(Entity<HandsComponent>.op_Implicit(ent), ref ent.Comp, false))
		{
			return false;
		}
		if (!IsHolding(ent, entity, out string hand))
		{
			return false;
		}
		return TryDrop(ent, hand, targetDropLocation, checkActionBlocker, doDropInteraction);
	}

	public bool TryDrop(Entity<HandsComponent?> ent, string handId, EntityCoordinates? targetDropLocation = null, bool checkActionBlocker = true, bool doDropInteraction = true)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<HandsComponent>(Entity<HandsComponent>.op_Implicit(ent), ref ent.Comp, false))
		{
			return false;
		}
		if (!CanDropHeld(Entity<HandsComponent>.op_Implicit(ent), handId, checkActionBlocker))
		{
			return false;
		}
		if (!TryGetHeldItem(ent, handId, out var entity))
		{
			return false;
		}
		VirtualItemComponent @virtual = default(VirtualItemComponent);
		if (((EntitySystem)this).TryComp<VirtualItemComponent>(entity, ref @virtual))
		{
			_virtualSystem.DeleteVirtualItem(Entity<VirtualItemComponent>.op_Implicit((entity.Value, @virtual)), Entity<HandsComponent>.op_Implicit(ent));
		}
		if (((EntitySystem)this).TerminatingOrDeleted(entity, (MetaDataComponent)null))
		{
			return true;
		}
		TransformComponent itemXform = ((EntitySystem)this).Transform(entity.Value);
		if (!itemXform.MapUid.HasValue)
		{
			return true;
		}
		TransformComponent userXform = ((EntitySystem)this).Transform(Entity<HandsComponent>.op_Implicit(ent));
		if (ContainerSystem.IsEntityOrParentInContainer(Entity<HandsComponent>.op_Implicit(ent), (MetaDataComponent)null, userXform))
		{
			TransformSystem.DropNextTo(Entity<TransformComponent>.op_Implicit((entity.Value, itemXform)), Entity<TransformComponent>.op_Implicit((Entity<HandsComponent>.op_Implicit(ent), userXform)));
			RMCDroppedEvent ev = new RMCDroppedEvent(Entity<HandsComponent>.op_Implicit(ent));
			((EntitySystem)this).RaiseLocalEvent<RMCDroppedEvent>(entity.Value, ref ev, true);
			return true;
		}
		DoDrop(ent, handId, doDropInteraction);
		if (!targetDropLocation.HasValue)
		{
			return true;
		}
		var (itemPos, itemRot) = TransformSystem.GetWorldPositionRotation(entity.Value);
		MapCoordinates origin = default(MapCoordinates);
		((MapCoordinates)(ref origin))._002Ector(itemPos, itemXform.MapID);
		MapCoordinates target = TransformSystem.ToMapCoordinates(targetDropLocation.Value, true);
		TransformSystem.SetWorldPositionRotation(entity.Value, GetFinalDropCoordinates(Entity<HandsComponent>.op_Implicit(ent), origin, target, entity.Value), itemRot, (TransformComponent)null);
		return true;
	}

	public bool TryDropIntoContainer(Entity<HandsComponent?> ent, EntityUid entity, BaseContainer targetContainer, bool checkActionBlocker = true)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<HandsComponent>(Entity<HandsComponent>.op_Implicit(ent), ref ent.Comp, false))
		{
			return false;
		}
		if (!IsHolding(ent, entity, out string hand))
		{
			return false;
		}
		if (!CanDropHeld(Entity<HandsComponent>.op_Implicit(ent), hand, checkActionBlocker))
		{
			return false;
		}
		if (!ContainerSystem.CanInsert(entity, targetContainer, false, (TransformComponent)null))
		{
			return false;
		}
		DoDrop(ent, hand, doDropInteraction: false);
		ContainerSystem.Insert(Entity<TransformComponent, MetaDataComponent, PhysicsComponent>.op_Implicit(entity), targetContainer, (TransformComponent)null, false);
		return true;
	}

	private Vector2 GetFinalDropCoordinates(EntityUid user, MapCoordinates origin, MapCoordinates target, EntityUid held)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		Vector2 dropVector = target.Position - origin.Position;
		float requestedDropDistance = dropVector.Length();
		float dropLength = dropVector.Length();
		if (ShouldIgnoreRestrictions(user))
		{
			if (dropVector.Length() > 1.5f)
			{
				dropVector = Vector2Helpers.Normalized(dropVector) * 1.5f;
				((MapCoordinates)(ref target))._002Ector(origin.Position + dropVector, target.MapId);
			}
			dropLength = _interactionSystem.UnobstructedDistance(origin, target, 130, (EntityUid e) => e == user || e == held);
		}
		if (dropLength < requestedDropDistance)
		{
			return origin.Position + Vector2Helpers.Normalized(dropVector) * dropLength;
		}
		return target.Position;
	}

	public virtual void DoDrop(Entity<HandsComponent?> ent, string handId, bool doDropInteraction = true, bool log = true)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		BaseContainer container = default(BaseContainer);
		if (!((EntitySystem)this).Resolve<HandsComponent>(Entity<HandsComponent>.op_Implicit(ent), ref ent.Comp, false) || !ContainerSystem.TryGetContainer(Entity<HandsComponent>.op_Implicit(ent), handId, ref container, (ContainerManagerComponent)null) || !TryGetHeldItem(ent, handId, out var entity) || ((EntitySystem)this).TerminatingOrDeleted(Entity<HandsComponent>.op_Implicit(ent), (MetaDataComponent)null) || ((EntitySystem)this).TerminatingOrDeleted(entity, (MetaDataComponent)null))
		{
			return;
		}
		if (!ContainerSystem.Remove(Entity<TransformComponent, MetaDataComponent>.op_Implicit(entity.Value), container, true, false, (EntityCoordinates?)null, (Angle?)null))
		{
			((EntitySystem)this).Log.Error($"Failed to remove {((EntitySystem)this).ToPrettyString(entity, (MetaDataComponent)null)} from users hand container when dropping. User: {((EntitySystem)this).ToPrettyString((EntityUid?)Entity<HandsComponent>.op_Implicit(ent), (MetaDataComponent)null)}. Hand: {handId}.");
			return;
		}
		((EntitySystem)this).Dirty<HandsComponent>(ent, (MetaDataComponent)null);
		if (doDropInteraction)
		{
			_interactionSystem.DroppedInteraction(Entity<HandsComponent>.op_Implicit(ent), entity.Value);
		}
		if (log)
		{
			ISharedAdminLogManager adminLogger = _adminLogger;
			LogStringHandler handler = new LogStringHandler(9, 2);
			handler.AppendFormatted(((EntitySystem)this).ToPrettyString((EntityUid?)Entity<HandsComponent>.op_Implicit(ent), (MetaDataComponent)null), "user", "ToPrettyString(ent)");
			handler.AppendLiteral(" dropped ");
			handler.AppendFormatted(((EntitySystem)this).ToPrettyString(entity, (MetaDataComponent)null), "entity", "ToPrettyString(entity)");
			adminLogger.Add(LogType.Drop, LogImpact.Low, ref handler);
		}
		if (handId == ent.Comp.ActiveHandId)
		{
			((EntitySystem)this).RaiseLocalEvent<HandDeselectedEvent>(entity.Value, new HandDeselectedEvent(Entity<HandsComponent>.op_Implicit(ent)), false);
		}
	}

	private void InitializeInteractions()
	{
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Expected O, but got Unknown
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Expected O, but got Unknown
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Expected O, but got Unknown
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Expected O, but got Unknown
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Expected O, but got Unknown
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Expected O, but got Unknown
		((EntitySystem)this).SubscribeAllEvent<RequestSetHandEvent>((EntitySessionEventHandler<RequestSetHandEvent>)HandleSetHand, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeAllEvent<RequestActivateInHandEvent>((EntitySessionEventHandler<RequestActivateInHandEvent>)HandleActivateItemInHand, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeAllEvent<RequestHandInteractUsingEvent>((EntitySessionEventHandler<RequestHandInteractUsingEvent>)HandleInteractUsingInHand, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeAllEvent<RequestUseInHandEvent>((EntitySessionEventHandler<RequestUseInHandEvent>)HandleUseInHand, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeAllEvent<RequestMoveHandItemEvent>((EntitySessionEventHandler<RequestMoveHandItemEvent>)HandleMoveItemFromHand, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeAllEvent<RequestHandAltInteractEvent>((EntitySessionEventHandler<RequestHandAltInteractEvent>)HandleHandAltInteract, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<HandsComponent, GetUsedEntityEvent>((ComponentEventRefHandler<HandsComponent, GetUsedEntityEvent>)OnGetUsedEntity, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<HandsComponent, ExaminedEvent>((ComponentEventHandler<HandsComponent, ExaminedEvent>)HandleExamined, (Type[])null, (Type[])null);
		CommandBinds.Builder.Bind(ContentKeyFunctions.UseItemInHand, InputCmdHandler.FromDelegate(new StateInputCmdDelegate(HandleUseItem), (StateInputCmdDelegate)null, false, false)).Bind(ContentKeyFunctions.AltUseItemInHand, InputCmdHandler.FromDelegate(new StateInputCmdDelegate(HandleAltUseInHand), (StateInputCmdDelegate)null, false, false)).Bind(ContentKeyFunctions.SwapHands, InputCmdHandler.FromDelegate(new StateInputCmdDelegate(SwapHandsPressed), (StateInputCmdDelegate)null, false, false))
			.Bind(ContentKeyFunctions.SwapHandsReverse, InputCmdHandler.FromDelegate(new StateInputCmdDelegate(SwapHandsReversePressed), (StateInputCmdDelegate)null, false, false))
			.Bind(ContentKeyFunctions.Drop, (InputCmdHandler)new PointerInputCmdHandler(new PointerInputCmdDelegate(DropPressed), true, false))
			.Register<SharedHandsSystem>();
	}

	private void HandleAltUseInHand(ICommonSession? session)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		if (session != null && session.AttachedEntity.HasValue)
		{
			TryUseItemInHand(session.AttachedEntity.Value, altInteract: true);
		}
	}

	private void HandleUseItem(ICommonSession? session)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		if (session != null && session.AttachedEntity.HasValue)
		{
			TryUseItemInHand(session.AttachedEntity.Value);
		}
	}

	private void HandleMoveItemFromHand(RequestMoveHandItemEvent msg, EntitySessionEventArgs args)
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySessionEventArgs)(ref args)).SenderSession.AttachedEntity.HasValue && !_rmcHands.TryStorageEjectHand(((EntitySessionEventArgs)(ref args)).SenderSession.AttachedEntity.Value, msg.HandName))
		{
			TryMoveHeldEntityToActiveHand(((EntitySessionEventArgs)(ref args)).SenderSession.AttachedEntity.Value, msg.HandName);
		}
	}

	private void HandleUseInHand(RequestUseInHandEvent msg, EntitySessionEventArgs args)
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySessionEventArgs)(ref args)).SenderSession.AttachedEntity.HasValue)
		{
			TryUseItemInHand(((EntitySessionEventArgs)(ref args)).SenderSession.AttachedEntity.Value);
		}
	}

	private void HandleActivateItemInHand(RequestActivateInHandEvent msg, EntitySessionEventArgs args)
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySessionEventArgs)(ref args)).SenderSession.AttachedEntity.HasValue)
		{
			TryActivateItemInHand(((EntitySessionEventArgs)(ref args)).SenderSession.AttachedEntity.Value, null, msg.HandName);
		}
	}

	private void HandleInteractUsingInHand(RequestHandInteractUsingEvent msg, EntitySessionEventArgs args)
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySessionEventArgs)(ref args)).SenderSession.AttachedEntity.HasValue)
		{
			TryInteractHandWithActiveHand(((EntitySessionEventArgs)(ref args)).SenderSession.AttachedEntity.Value, msg.HandName);
		}
	}

	private void HandleHandAltInteract(RequestHandAltInteractEvent msg, EntitySessionEventArgs args)
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySessionEventArgs)(ref args)).SenderSession.AttachedEntity.HasValue)
		{
			TryUseItemInHand(((EntitySessionEventArgs)(ref args)).SenderSession.AttachedEntity.Value, altInteract: true, null, msg.HandName);
		}
	}

	private void SwapHandsPressed(ICommonSession? session)
	{
		SwapHands(session, reverse: false);
	}

	private void SwapHandsReversePressed(ICommonSession? session)
	{
		SwapHands(session, reverse: true);
	}

	private void SwapHands(ICommonSession? session, bool reverse)
	{
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		HandsComponent component = default(HandsComponent);
		if (((EntitySystem)this).TryComp<HandsComponent>((session != null) ? session.AttachedEntity : ((EntityUid?)null), ref component) && component.ActiveHandId != null && component.Hands.Count >= 2)
		{
			int newActiveIndex = (component.SortedHands.IndexOf(component.ActiveHandId) + ((!reverse) ? 1 : (-1)) + component.Hands.Count) % component.Hands.Count;
			string nextHand = component.SortedHands[newActiveIndex];
			TrySetActiveHand(Entity<HandsComponent>.op_Implicit((session.AttachedEntity.Value, component)), nextHand);
		}
	}

	private bool DropPressed(ICommonSession? session, EntityCoordinates coords, EntityUid netEntity)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		HandsComponent hands = default(HandsComponent);
		if (((EntitySystem)this).TryComp<HandsComponent>((session != null) ? session.AttachedEntity : ((EntityUid?)null), ref hands) && hands.ActiveHandId != null)
		{
			TryDrop(Entity<HandsComponent>.op_Implicit((session.AttachedEntity.Value, hands)), hands.ActiveHandId, coords);
		}
		return false;
	}

	public bool TryActivateItemInHand(EntityUid uid, HandsComponent? handsComp = null, string? handName = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<HandsComponent>(uid, ref handsComp, false))
		{
			return false;
		}
		string hand = handName;
		if (!TryGetHand(Entity<HandsComponent>.op_Implicit(uid), hand, out var _))
		{
			hand = handsComp.ActiveHandId;
		}
		if (!TryGetHeldItem(Entity<HandsComponent>.op_Implicit((uid, handsComp)), hand, out var held))
		{
			return false;
		}
		return _interactionSystem.InteractionActivate(uid, held.Value);
	}

	public bool TryInteractHandWithActiveHand(EntityUid uid, string handName, HandsComponent? handsComp = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<HandsComponent>(uid, ref handsComp, false))
		{
			return false;
		}
		if (!TryGetActiveItem(Entity<HandsComponent>.op_Implicit((uid, handsComp)), out var activeHeldItem))
		{
			return false;
		}
		if (!TryGetHeldItem(Entity<HandsComponent>.op_Implicit((uid, handsComp)), handName, out var held))
		{
			return false;
		}
		_interactionSystem.InteractUsing(uid, activeHeldItem.Value, held.Value, ((EntitySystem)this).Transform(held.Value).Coordinates);
		return true;
	}

	public bool TryUseItemInHand(EntityUid uid, bool altInteract = false, HandsComponent? handsComp = null, string? handName = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<HandsComponent>(uid, ref handsComp, false))
		{
			return false;
		}
		string hand = handName;
		if (!TryGetHand(Entity<HandsComponent>.op_Implicit(uid), hand, out var _))
		{
			hand = handsComp.ActiveHandId;
		}
		if (!TryGetHeldItem(Entity<HandsComponent>.op_Implicit((uid, handsComp)), hand, out var held))
		{
			return false;
		}
		if (altInteract)
		{
			return _interactionSystem.AltInteract(uid, held.Value);
		}
		return _interactionSystem.UseInHandInteraction(uid, held.Value);
	}

	public bool TryMoveHeldEntityToActiveHand(EntityUid uid, string handName, bool checkActionBlocker = true, HandsComponent? handsComp = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<HandsComponent>(uid, ref handsComp, true))
		{
			return false;
		}
		if (handsComp.ActiveHandId == null || !HandIsEmpty(Entity<HandsComponent>.op_Implicit((uid, handsComp)), handsComp.ActiveHandId))
		{
			return false;
		}
		if (!TryGetHeldItem(Entity<HandsComponent>.op_Implicit((uid, handsComp)), handName, out var entity))
		{
			return false;
		}
		if (!CanDropHeld(uid, handName, checkActionBlocker))
		{
			return false;
		}
		if (!CanPickupToHand(uid, entity.Value, handsComp.ActiveHandId, checkActionBlocker, handsComp))
		{
			return false;
		}
		DoDrop(Entity<HandsComponent>.op_Implicit(uid), handName, doDropInteraction: false, log: false);
		DoPickup(uid, handsComp.ActiveHandId, entity.Value, handsComp, log: false);
		return true;
	}

	private void OnGetUsedEntity(EntityUid uid, HandsComponent component, ref GetUsedEntityEvent args)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		if (!args.Handled)
		{
			if (TryGetActiveItem(Entity<HandsComponent>.op_Implicit((uid, component)), out var activeHeldItem))
			{
				((EntitySystem)this).RaiseLocalEvent<GetUsedEntityEvent>(activeHeldItem.Value, ref args, false);
			}
			ref EntityUid? used = ref args.Used;
			EntityUid? val = used;
			if (!val.HasValue)
			{
				used = activeHeldItem;
			}
		}
	}

	private void HandleExamined(EntityUid examinedUid, HandsComponent handsComp, ExaminedEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		List<string> heldItemNames = (from item in EnumerateHeld(Entity<HandsComponent>.op_Implicit((examinedUid, handsComp)))
			where !((EntitySystem)this).HasComp<VirtualItemComponent>(item)
			select FormattedMessage.EscapeText((string)Identity.Name(item, (IEntityManager)(object)base.EntityManager)) into itemName
			select base.Loc.GetString("comp-hands-examine-wrapper", (ValueTuple<string, object>)("item", itemName))).ToList();
		if (heldItemNames.Count == 0 && !handsComp.ExamineShowEmpty)
		{
			return;
		}
		string locKey = ((heldItemNames.Count != 0) ? "comp-hands-examine" : "comp-hands-examine-empty");
		(string, EntityUid) locUser = ("user", Identity.Entity(examinedUid, (IEntityManager)(object)base.EntityManager));
		(string, string) locItems = ("items", ContentLocalizationManager.FormatList(heldItemNames));
		using (args.PushGroup("HandsComponent"))
		{
			ILocalizationManager loc = base.Loc;
			(string, EntityUid) tuple = locUser;
			(string, EntityUid) valueTuple = (tuple.Item1, tuple.Item2);
			(string, string) tuple2 = locItems;
			args.PushMarkup(loc.GetString(locKey, (ValueTuple<string, object>)valueTuple, (ValueTuple<string, object>)(tuple2.Item1, tuple2.Item2)));
		}
	}

	private void InitializePickup()
	{
		((EntitySystem)this).SubscribeLocalEvent<HandsComponent, EntInsertedIntoContainerMessage>((ComponentEventHandler<HandsComponent, EntInsertedIntoContainerMessage>)HandleEntityInserted, (Type[])null, (Type[])null);
	}

	protected virtual void HandleEntityInserted(EntityUid uid, HandsComponent hands, EntInsertedIntoContainerMessage args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		if (TryGetHand(Entity<HandsComponent>.op_Implicit(uid), ((ContainerModifiedMessage)args).Container.ID, out var hand))
		{
			DidEquipHandEvent didEquip = new DidEquipHandEvent(uid, ((ContainerModifiedMessage)args).Entity, hand.Value);
			((EntitySystem)this).RaiseLocalEvent<DidEquipHandEvent>(uid, didEquip, false);
			GotEquippedHandEvent gotEquipped = new GotEquippedHandEvent(uid, ((ContainerModifiedMessage)args).Entity, hand.Value);
			((EntitySystem)this).RaiseLocalEvent<GotEquippedHandEvent>(((ContainerModifiedMessage)args).Entity, gotEquipped, false);
		}
	}

	public bool TryPickupAnyHand(EntityUid uid, EntityUid entity, bool checkActionBlocker = true, bool animateUser = false, bool animate = true, HandsComponent? handsComp = null, ItemComponent? item = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<HandsComponent>(uid, ref handsComp, false))
		{
			return false;
		}
		if (!TryGetEmptyHand(Entity<HandsComponent>.op_Implicit((uid, handsComp)), out string hand))
		{
			return false;
		}
		return TryPickup(uid, entity, hand, checkActionBlocker, animateUser, animate, handsComp, item);
	}

	public bool TryPickup(EntityUid uid, EntityUid entity, string? handId = null, bool checkActionBlocker = true, bool animateUser = false, bool animate = true, HandsComponent? handsComp = null, ItemComponent? item = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<HandsComponent>(uid, ref handsComp, false))
		{
			return false;
		}
		if (handId == null)
		{
			handId = handsComp.ActiveHandId;
		}
		if (handId == null)
		{
			return false;
		}
		if (!((EntitySystem)this).Resolve<ItemComponent>(entity, ref item, false))
		{
			return false;
		}
		if (!CanPickupToHand(uid, entity, handId, checkActionBlocker, handsComp, item))
		{
			return false;
		}
		if (animate)
		{
			TransformComponent xform = ((EntitySystem)this).Transform(uid);
			EntityUid parentUid = xform.ParentUid;
			EntityUid coordinateEntity = (((EntityUid)(ref parentUid)).IsValid() ? xform.ParentUid : uid);
			TransformComponent itemXform = ((EntitySystem)this).Transform(entity);
			MapCoordinates itemPos = TransformSystem.GetMapCoordinates(entity, itemXform);
			if (itemPos.MapId == xform.MapID && (itemPos.Position - TransformSystem.GetMapCoordinates(uid, xform).Position).Length() <= 10f && ((EntitySystem)this).MetaData(entity).VisibilityMask == ((EntitySystem)this).MetaData(uid).VisibilityMask)
			{
				EntityCoordinates initialPosition = TransformSystem.ToCoordinates(Entity<TransformComponent>.op_Implicit(coordinateEntity), itemPos);
				_storage.PlayPickupAnimation(entity, initialPosition, xform.Coordinates, itemXform.LocalRotation, uid);
			}
		}
		DoPickup(uid, handId, entity, handsComp);
		return true;
	}

	public bool TryForcePickup(Entity<HandsComponent?> ent, EntityUid entity, string hand, bool checkActionBlocker = true, bool animate = true, HandsComponent? handsComp = null, ItemComponent? item = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<HandsComponent>(Entity<HandsComponent>.op_Implicit(ent), ref ent.Comp, false))
		{
			return false;
		}
		Entity<HandsComponent?> ent2 = ent;
		bool checkActionBlocker2 = checkActionBlocker;
		TryDrop(ent2, hand, null, checkActionBlocker2);
		return TryPickup(Entity<HandsComponent>.op_Implicit(ent), entity, hand, checkActionBlocker, animateUser: false, animate, handsComp, item);
	}

	public bool TryForcePickupAnyHand(EntityUid uid, EntityUid entity, bool checkActionBlocker = true, HandsComponent? handsComp = null, ItemComponent? item = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<HandsComponent>(uid, ref handsComp, false))
		{
			return false;
		}
		if (TryPickupAnyHand(uid, entity, checkActionBlocker, animateUser: false, animate: true, handsComp))
		{
			return true;
		}
		foreach (string hand in handsComp.Hands.Keys)
		{
			Entity<HandsComponent> ent = Entity<HandsComponent>.op_Implicit((uid, handsComp));
			bool checkActionBlocker2 = checkActionBlocker;
			if (TryDrop(ent, hand, null, checkActionBlocker2) && TryPickup(uid, entity, hand, checkActionBlocker, animateUser: false, animate: true, handsComp))
			{
				return true;
			}
		}
		return false;
	}

	public bool CanPickupAnyHand(EntityUid uid, EntityUid entity, bool checkActionBlocker = true, HandsComponent? handsComp = null, ItemComponent? item = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<HandsComponent>(uid, ref handsComp, false))
		{
			return false;
		}
		if (!TryGetEmptyHand(Entity<HandsComponent>.op_Implicit((uid, handsComp)), out string hand))
		{
			return false;
		}
		return CanPickupToHand(uid, entity, hand, checkActionBlocker, handsComp, item);
	}

	public bool CanPickupToHand(EntityUid uid, EntityUid entity, string handId, bool checkActionBlocker = true, HandsComponent? handsComp = null, ItemComponent? item = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Invalid comparison between Unknown and I4
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<HandsComponent>(uid, ref handsComp, false))
		{
			return false;
		}
		BaseContainer handContainer = default(BaseContainer);
		if (!ContainerSystem.TryGetContainer(uid, handId, ref handContainer, (ContainerManagerComponent)null))
		{
			return false;
		}
		if (Extensions.FirstOrNull<EntityUid>((IEnumerable<EntityUid>)handContainer.ContainedEntities).HasValue)
		{
			return false;
		}
		if (!((EntitySystem)this).Resolve<ItemComponent>(entity, ref item, false))
		{
			return false;
		}
		PhysicsComponent physics = default(PhysicsComponent);
		if (((EntitySystem)this).TryComp<PhysicsComponent>(entity, ref physics) && (int)physics.BodyType == 4)
		{
			return false;
		}
		if (checkActionBlocker && !_actionBlocker.CanPickup(uid, entity))
		{
			return false;
		}
		BaseContainer container = default(BaseContainer);
		if (ContainerSystem.TryGetContainingContainer(Entity<TransformComponent, MetaDataComponent>.op_Implicit((ValueTuple<EntityUid, TransformComponent, MetaDataComponent>)(entity, null, null)), ref container))
		{
			if (!ContainerSystem.CanRemove(entity, container))
			{
				return false;
			}
			if (_inventory.TryGetSlotEntity(uid, container.ID, out var slotEnt))
			{
				EntityUid? val = slotEnt;
				if (val.HasValue && val.GetValueOrDefault() == entity && !_inventory.CanUnequip(uid, container.ID, out string _))
				{
					return false;
				}
			}
		}
		return ContainerSystem.CanInsert(entity, handContainer, false, (TransformComponent)null);
	}

	public void PickupOrDrop(EntityUid? uid, EntityUid entity, bool checkActionBlocker = true, bool animateUser = false, bool animate = true, bool dropNear = false, HandsComponent? handsComp = null, ItemComponent? item = null)
	{
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		if (!uid.HasValue || !((EntitySystem)this).Resolve<HandsComponent>(uid.Value, ref handsComp, false) || !TryPickupAnyHand(uid.Value, entity, checkActionBlocker, animateUser, animate, handsComp, item))
		{
			ContainerSystem.AttachParentToContainerOrGrid(Entity<TransformComponent>.op_Implicit((entity, ((EntitySystem)this).Transform(entity))));
			if (dropNear && uid.HasValue)
			{
				TransformSystem.PlaceNextTo(Entity<TransformComponent>.op_Implicit(entity), Entity<TransformComponent>.op_Implicit(uid.Value));
			}
		}
	}

	public virtual void DoPickup(EntityUid uid, string hand, EntityUid entity, HandsComponent? hands = null, bool log = true)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		BaseContainer handContainer = default(BaseContainer);
		if (!((EntitySystem)this).Resolve<HandsComponent>(uid, ref hands, true) || !ContainerSystem.TryGetContainer(uid, hand, ref handContainer, (ContainerManagerComponent)null) || Extensions.FirstOrNull<EntityUid>((IEnumerable<EntityUid>)handContainer.ContainedEntities).HasValue)
		{
			return;
		}
		if (!ContainerSystem.Insert(Entity<TransformComponent, MetaDataComponent, PhysicsComponent>.op_Implicit(entity), handContainer, (TransformComponent)null, false))
		{
			((EntitySystem)this).Log.Error($"Failed to insert {((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(entity))} into users hand container when picking up. User: {((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(uid))}. Hand: {hand}.");
			return;
		}
		_interactionSystem.DoContactInteraction(uid, entity);
		if (log)
		{
			ISharedAdminLogManager adminLogger = _adminLogger;
			LogStringHandler handler = new LogStringHandler(11, 2);
			handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(uid)), "user", "ToPrettyString(uid)");
			handler.AppendLiteral(" picked up ");
			handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(entity)), "entity", "ToPrettyString(entity)");
			adminLogger.Add(LogType.Pickup, LogImpact.Low, ref handler);
		}
		((EntitySystem)this).Dirty(uid, (IComponent)(object)hands, (MetaDataComponent)null);
		if (hand == hands.ActiveHandId)
		{
			((EntitySystem)this).RaiseLocalEvent<HandSelectedEvent>(entity, new HandSelectedEvent(uid), false);
		}
	}

	private void InitializeRelay()
	{
		((EntitySystem)this).SubscribeLocalEvent<HandsComponent, GetEyeOffsetRelayedEvent>((EntityEventRefHandler<HandsComponent, GetEyeOffsetRelayedEvent>)RelayEvent, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<HandsComponent, GetEyePvsScaleRelayedEvent>((EntityEventRefHandler<HandsComponent, GetEyePvsScaleRelayedEvent>)RelayEvent, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<HandsComponent, RefreshMovementSpeedModifiersEvent>((EntityEventRefHandler<HandsComponent, RefreshMovementSpeedModifiersEvent>)RelayEvent, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<HandsComponent, ExtinguishEvent>((EntityEventRefHandler<HandsComponent, ExtinguishEvent>)RefRelayEvent, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<HandsComponent, ProjectileReflectAttemptEvent>((EntityEventRefHandler<HandsComponent, ProjectileReflectAttemptEvent>)RefRelayEvent, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<HandsComponent, HitScanReflectAttemptEvent>((EntityEventRefHandler<HandsComponent, HitScanReflectAttemptEvent>)RefRelayEvent, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<HandsComponent, WieldAttemptEvent>((EntityEventRefHandler<HandsComponent, WieldAttemptEvent>)RefRelayEvent, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<HandsComponent, UnwieldAttemptEvent>((EntityEventRefHandler<HandsComponent, UnwieldAttemptEvent>)RefRelayEvent, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<HandsComponent, TargetHandcuffedEvent>((EntityEventRefHandler<HandsComponent, TargetHandcuffedEvent>)RefRelayEvent, (Type[])null, (Type[])null);
	}

	private void RelayEvent<T>(Entity<HandsComponent> entity, ref T args) where T : EntityEventArgs
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		CoreRelayEvent(entity, ref args);
	}

	private void RefRelayEvent<T>(Entity<HandsComponent> entity, ref T args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		HeldRelayedEvent<T> ev = CoreRelayEvent(entity, ref args);
		args = ev.Args;
	}

	private HeldRelayedEvent<T> CoreRelayEvent<T>(Entity<HandsComponent> entity, ref T args)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		HeldRelayedEvent<T> ev = new HeldRelayedEvent<T>(args);
		foreach (EntityUid held in EnumerateHeld(entity.AsNullable()))
		{
			((EntitySystem)this).RaiseLocalEvent<HeldRelayedEvent<T>>(held, ref ev, false);
		}
		return ev;
	}
}
