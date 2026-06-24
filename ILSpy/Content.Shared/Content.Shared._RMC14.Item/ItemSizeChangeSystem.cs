using System;
using System.Collections.Generic;
using Content.Shared._RMC14.Attachable.Systems;
using Content.Shared._RMC14.Explosion;
using Content.Shared.Item;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;

namespace Content.Shared._RMC14.Item;

public sealed class ItemSizeChangeSystem : EntitySystem
{
	[Dependency]
	private IPrototypeManager _prototypeManager;

	[Dependency]
	private SharedItemSystem _itemSystem;

	private readonly List<ItemSizePrototype> _sortedSizes = new List<ItemSizePrototype>();

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<PrototypesReloadedEventArgs>((EntityEventHandler<PrototypesReloadedEventArgs>)OnPrototypesReloaded, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ItemSizeChangeComponent, MapInitEvent>((EntityEventRefHandler<ItemSizeChangeComponent, MapInitEvent>)OnItemSizeChangeMapInit, new Type[1] { typeof(AttachableHolderSystem) }, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ChangeItemSizeOnTimerTriggerComponent, RMCActiveTimerTriggerEvent>((EntityEventRefHandler<ChangeItemSizeOnTimerTriggerComponent, RMCActiveTimerTriggerEvent>)OnChangeItemSizeOnTimerTrigger, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ChangeItemSizeOnTimerTriggerComponent, RMCTriggerEvent>((EntityEventRefHandler<ChangeItemSizeOnTimerTriggerComponent, RMCTriggerEvent>)OnTriggered, (Type[])null, (Type[])null);
		InitItemSizes();
	}

	private void OnPrototypesReloaded(PrototypesReloadedEventArgs args)
	{
		if (!args.ByType.ContainsKey(typeof(ItemSizePrototype)))
		{
			IReadOnlyDictionary<Type, HashSet<string>> removed = args.Removed;
			if (removed == null || !removed.ContainsKey(typeof(ItemSizePrototype)))
			{
				return;
			}
		}
		InitItemSizes();
	}

	private void OnItemSizeChangeMapInit(Entity<ItemSizeChangeComponent> item, ref MapInitEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		InitItem(item);
		RefreshItemSizeModifiers(Entity<ItemSizeChangeComponent>.op_Implicit((item.Owner, item.Comp)));
	}

	private void OnChangeItemSizeOnTimerTrigger(Entity<ChangeItemSizeOnTimerTriggerComponent> ent, ref RMCActiveTimerTriggerEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		ItemComponent item = default(ItemComponent);
		if (((EntitySystem)this).TryComp<ItemComponent>(Entity<ChangeItemSizeOnTimerTriggerComponent>.op_Implicit(ent), ref item))
		{
			ent.Comp.OriginalSize = item.Size;
			((EntitySystem)this).Dirty<ChangeItemSizeOnTimerTriggerComponent>(ent, (MetaDataComponent)null);
		}
		_itemSystem.SetSize(Entity<ChangeItemSizeOnTimerTriggerComponent>.op_Implicit(ent), ent.Comp.Size);
	}

	private void OnTriggered(Entity<ChangeItemSizeOnTimerTriggerComponent> ent, ref RMCTriggerEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		if (ent.Comp.OriginalSize.HasValue)
		{
			_itemSystem.SetSize(Entity<ChangeItemSizeOnTimerTriggerComponent>.op_Implicit(ent), ent.Comp.OriginalSize.Value);
		}
	}

	private void InitItemSizes()
	{
		_sortedSizes.Clear();
		foreach (ItemSizePrototype prototype in _prototypeManager.EnumeratePrototypes<ItemSizePrototype>())
		{
			if (!prototype.ID.Equals("Invalid"))
			{
				_sortedSizes.Add(prototype);
			}
		}
		_sortedSizes.Sort();
	}

	public void RefreshItemSizeModifiers(Entity<ItemSizeChangeComponent?> item)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		if (item.Comp == null)
		{
			item.Comp = ((EntitySystem)this).EnsureComp<ItemSizeChangeComponent>(item.Owner);
		}
		else if (!InitItem(Entity<ItemSizeChangeComponent>.op_Implicit((item.Owner, item.Comp))))
		{
			return;
		}
		if (item.Comp != null && item.Comp.BaseSize.HasValue)
		{
			GetItemSizeModifiersEvent ev = new GetItemSizeModifiersEvent(item.Comp.BaseSize.Value);
			((EntitySystem)this).RaiseLocalEvent<GetItemSizeModifiersEvent>(item.Owner, ref ev, false);
			ev.Size = Math.Clamp(ev.Size, 0, (_sortedSizes.Count > 0) ? (_sortedSizes.Count - 1) : 0);
			if (_sortedSizes.Count > ev.Size)
			{
				_itemSystem.SetSize(Entity<ItemSizeChangeComponent>.op_Implicit(item), ProtoId<ItemSizePrototype>.op_Implicit(_sortedSizes[ev.Size]));
			}
		}
	}

	private bool InitItem(Entity<ItemSizeChangeComponent> item, bool onlyNull = false)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		if (!onlyNull && item.Comp.BaseSize.HasValue)
		{
			return true;
		}
		if (_sortedSizes.Count <= 0)
		{
			InitItemSizes();
			if (_sortedSizes.Count <= 0)
			{
				return false;
			}
		}
		ItemComponent itemComponent = default(ItemComponent);
		ItemSizePrototype prototype = default(ItemSizePrototype);
		if (!((EntitySystem)this).TryComp<ItemComponent>(item.Owner, ref itemComponent) || !_prototypeManager.TryIndex<ItemSizePrototype>(itemComponent.Size, ref prototype))
		{
			return false;
		}
		int size = _sortedSizes.IndexOf(prototype);
		if (size < 0)
		{
			return false;
		}
		item.Comp.BaseSize = size;
		((EntitySystem)this).Dirty<ItemSizeChangeComponent>(item, (MetaDataComponent)null);
		return true;
	}
}
