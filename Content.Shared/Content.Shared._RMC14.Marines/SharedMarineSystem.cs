using System;
using System.Collections.Generic;
using Content.Shared._RMC14.Marines.Squads;
using Content.Shared.Inventory;
using Content.Shared.Inventory.Events;
using Content.Shared.NPC.Prototypes;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Timing;
using Robust.Shared.Utility;

namespace Content.Shared._RMC14.Marines;

public abstract class SharedMarineSystem : EntitySystem
{
	[Dependency]
	private InventorySystem _inventory;

	[Dependency]
	private ISerializationManager _serialization;

	[Dependency]
	private IGameTiming _timing;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<MarineComponent, GetMarineIconEvent>((EntityEventRefHandler<MarineComponent, GetMarineIconEvent>)OnMarineGetIcon, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<GrantMarineIconsComponent, GotEquippedEvent>((EntityEventRefHandler<GrantMarineIconsComponent, GotEquippedEvent>)OnGotEquipped, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<GrantMarineIconsComponent, GotUnequippedEvent>((EntityEventRefHandler<GrantMarineIconsComponent, GotUnequippedEvent>)OnGotUnequipped, (Type[])null, (Type[])null);
	}

	private void OnGotEquipped(Entity<GrantMarineIconsComponent> ent, ref GotEquippedEvent args)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		if (!_timing.ApplyingState && (ent.Comp.Slots & args.SlotFlags) != SlotFlags.NONE)
		{
			GiveMarineHud(args.Equipee, ent.Comp.Factions, ent.Comp.BypassFactionIcons);
		}
	}

	private void OnGotUnequipped(Entity<GrantMarineIconsComponent> ent, ref GotUnequippedEvent args)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		if (!_timing.ApplyingState && (ent.Comp.Slots & args.SlotFlags) != SlotFlags.NONE && !_inventory.TryGetInventoryEntity<GrantMarineIconsComponent>(Entity<InventoryComponent>.op_Implicit(args.Equipee), out Entity<GrantMarineIconsComponent> _))
		{
			((EntitySystem)this).RemCompDeferred<ShowMarineIconsComponent>(args.Equipee);
		}
	}

	private void OnMarineGetIcon(Entity<MarineComponent> marine, ref GetMarineIconEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		SpriteSpecifier icon = marine.Comp.Icon;
		if (icon != null)
		{
			args.Icon = icon;
		}
	}

	public GetMarineIconEvent GetMarineIcon(EntityUid uid)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		GetMarineIconEvent ev = default(GetMarineIconEvent);
		((EntitySystem)this).RaiseLocalEvent<GetMarineIconEvent>(uid, ref ev, false);
		return ev;
	}

	public void SetMarineIcon(EntityUid marine, SpriteSpecifier specifier)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		MarineComponent comp = default(MarineComponent);
		if (((EntitySystem)this).TryComp<MarineComponent>(marine, ref comp))
		{
			comp.Icon = _serialization.CreateCopy<SpriteSpecifier>(specifier, (ISerializationContext)null, false, true);
			((EntitySystem)this).Dirty(marine, (IComponent)(object)comp, (MetaDataComponent)null);
		}
	}

	public void ClearMarineIcon(EntityUid marine)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		MarineComponent comp = default(MarineComponent);
		if (((EntitySystem)this).TryComp<MarineComponent>(marine, ref comp))
		{
			comp.Icon = null;
			((EntitySystem)this).Dirty(marine, (IComponent)(object)comp, (MetaDataComponent)null);
		}
	}

	public void MakeMarine(EntityUid uid, SpriteSpecifier? icon)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		MarineComponent marine = ((EntitySystem)this).EnsureComp<MarineComponent>(uid);
		marine.Icon = _serialization.CreateCopy<SpriteSpecifier>(icon, (ISerializationContext)null, false, false);
		((EntitySystem)this).Dirty(uid, (IComponent)(object)marine, (MetaDataComponent)null);
	}

	public void ClearIcon(Entity<MarineComponent> marine)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		marine.Comp.Icon = null;
		((EntitySystem)this).Dirty<MarineComponent>(marine, (MetaDataComponent)null);
	}

	public Dictionary<ProtoId<NpcFactionPrototype>, SpriteSpecifier>? GetFactionIcons(EntityUid uid)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		MarineComponent marine = default(MarineComponent);
		if (((EntitySystem)this).TryComp<MarineComponent>(uid, ref marine))
		{
			return marine.GenericFactionIcons;
		}
		return null;
	}

	public void GiveMarineHud(EntityUid uid, List<ProtoId<NpcFactionPrototype>>? faction, bool bypassIcons)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		ShowMarineIconsComponent icons = ((EntitySystem)this).EnsureComp<ShowMarineIconsComponent>(uid);
		icons.Factions = faction;
		icons.BypassFactionIcons = bypassIcons;
		((EntitySystem)this).Dirty(uid, (IComponent)(object)icons, (MetaDataComponent)null);
	}
}
