using System;
using System.Collections.Generic;
using System.Linq;
using Content.Shared._RMC14.Attachable.Systems;
using Content.Shared.Examine;
using Content.Shared.Hands.Components;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Inventory;
using Content.Shared.Weapons.Ranged.Events;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Physics.Events;
using Robust.Shared.Prototypes;

namespace Content.Shared._RMC14.Weapons.Ranged.IFF;

public sealed class GunIFFSystem : EntitySystem
{
	[Dependency]
	private SharedContainerSystem _container;

	[Dependency]
	private SharedHandsSystem _hands;

	[Dependency]
	private InventorySystem _inventory;

	private EntityQuery<UserIFFComponent> _userIFFQuery;

	private readonly HashSet<EntProtoId<IFFFactionComponent>> _factionBuffer = new HashSet<EntProtoId<IFFFactionComponent>>();

	public override void Initialize()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		_userIFFQuery = ((EntitySystem)this).GetEntityQuery<UserIFFComponent>();
		((EntitySystem)this).SubscribeLocalEvent<UserIFFComponent, GetIFFFactionEvent>((EntityEventRefHandler<UserIFFComponent, GetIFFFactionEvent>)OnUserIFFGetFaction, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<InventoryComponent, GetIFFFactionEvent>((EntityEventRefHandler<InventoryComponent, GetIFFFactionEvent>)OnInventoryIFFGetFaction, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<HandsComponent, GetIFFFactionEvent>((EntityEventRefHandler<HandsComponent, GetIFFFactionEvent>)OnHandsIFFGetFaction, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ItemIFFComponent, InventoryRelayedEvent<GetIFFFactionEvent>>((EntityEventRefHandler<ItemIFFComponent, InventoryRelayedEvent<GetIFFFactionEvent>>)OnItemIFFGetFaction, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<GunIFFComponent, AmmoShotEvent>((EntityEventRefHandler<GunIFFComponent, AmmoShotEvent>)OnGunIFFAmmoShot, new Type[1] { typeof(AttachableIFFSystem) }, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<GunIFFComponent, ExaminedEvent>((EntityEventRefHandler<GunIFFComponent, ExaminedEvent>)OnGunIFFExamined, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ProjectileIFFComponent, PreventCollideEvent>((EntityEventRefHandler<ProjectileIFFComponent, PreventCollideEvent>)OnProjectileIFFPreventCollide, (Type[])null, (Type[])null);
	}

	private void OnUserIFFGetFaction(Entity<UserIFFComponent> ent, ref GetIFFFactionEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		args.Factions.UnionWith(ent.Comp.Factions);
	}

	private void OnInventoryIFFGetFaction(Entity<InventoryComponent> ent, ref GetIFFFactionEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		_inventory.RelayEvent(ent, ref args);
	}

	private void OnHandsIFFGetFaction(Entity<HandsComponent> ent, ref GetIFFFactionEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		foreach (EntityUid held in _hands.EnumerateHeld(Entity<HandsComponent>.op_Implicit((Entity<HandsComponent>.op_Implicit(ent), Entity<HandsComponent>.op_Implicit(ent)))))
		{
			((EntitySystem)this).RaiseLocalEvent<GetIFFFactionEvent>(held, ref args, false);
		}
	}

	private void OnItemIFFGetFaction(Entity<ItemIFFComponent> ent, ref InventoryRelayedEvent<GetIFFFactionEvent> args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		if (ent.Comp.Factions.Count > 0)
		{
			args.Args.Factions.UnionWith(ent.Comp.Factions);
		}
	}

	private void OnGunIFFAmmoShot(Entity<GunIFFComponent> ent, ref AmmoShotEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		GiveAmmoIFF(ent.Owner, ref args, ent.Comp.Intrinsic, ent.Comp.Enabled);
	}

	private void OnGunIFFExamined(Entity<GunIFFComponent> ent, ref ExaminedEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		if (!ent.Comp.Enabled)
		{
			return;
		}
		using (args.PushGroup("GunIFFComponent"))
		{
			args.PushMarkup(base.Loc.GetString("rmc-examine-text-iff"));
		}
	}

	private void OnProjectileIFFPreventCollide(Entity<ProjectileIFFComponent> ent, ref PreventCollideEvent args)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		if (args.Cancelled)
		{
			return;
		}
		foreach (EntProtoId<IFFFactionComponent> faction in ent.Comp.Factions)
		{
			if (((EntitySystem)this).HasComp<EntityIFFComponent>(args.OtherEntity) && IsInFaction(args.OtherEntity, faction))
			{
				args.Cancelled = true;
				break;
			}
			if (ent.Comp.Enabled && IsInFaction(args.OtherEntity, faction))
			{
				args.Cancelled = true;
				break;
			}
		}
	}

	public bool TryGetUserFaction(Entity<UserIFFComponent?> user, out EntProtoId<IFFFactionComponent> faction)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		faction = default(EntProtoId<IFFFactionComponent>);
		if (!_userIFFQuery.Resolve(Entity<UserIFFComponent>.op_Implicit(user), ref user.Comp, false) || user.Comp.Factions.Count == 0)
		{
			return false;
		}
		faction = user.Comp.Factions.First();
		return true;
	}

	public bool TryGetFaction(Entity<UserIFFComponent?> user, out EntProtoId<IFFFactionComponent> faction, SlotFlags slots = SlotFlags.IDCARD)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		faction = default(EntProtoId<IFFFactionComponent>);
		HashSet<EntProtoId<IFFFactionComponent>> buffer = new HashSet<EntProtoId<IFFFactionComponent>>();
		if (!TryGetFactions(user, buffer, slots))
		{
			return false;
		}
		faction = buffer.First();
		return true;
	}

	public bool TryGetFactions(Entity<UserIFFComponent?> user, HashSet<EntProtoId<IFFFactionComponent>> factions, SlotFlags slots = SlotFlags.IDCARD)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		factions.Clear();
		if (!_userIFFQuery.Resolve(Entity<UserIFFComponent>.op_Implicit(user), ref user.Comp, false))
		{
			return false;
		}
		factions.UnionWith(user.Comp.Factions);
		GetIFFFactionEvent ev = new GetIFFFactionEvent(slots, new HashSet<EntProtoId<IFFFactionComponent>>());
		((EntitySystem)this).RaiseLocalEvent<GetIFFFactionEvent>(Entity<UserIFFComponent>.op_Implicit(user), ref ev, false);
		factions.UnionWith(ev.Factions);
		if (factions.Count == 0)
		{
			return false;
		}
		return true;
	}

	public bool IsInFaction(Entity<UserIFFComponent?> user, EntProtoId<IFFFactionComponent> faction)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		EntityUid uid = user.Owner;
		if (_userIFFQuery.Resolve(Entity<UserIFFComponent>.op_Implicit(user), ref user.Comp, false) && user.Comp.Factions.Count > 0 && user.Comp.Factions.Contains(faction))
		{
			return true;
		}
		GetIFFFactionEvent ev = new GetIFFFactionEvent(SlotFlags.IDCARD, new HashSet<EntProtoId<IFFFactionComponent>>());
		((EntitySystem)this).RaiseLocalEvent<GetIFFFactionEvent>(uid, ref ev, false);
		if (ev.Factions.Count > 0 && ev.Factions.Contains(faction))
		{
			return true;
		}
		return false;
	}

	public bool IsInFaction(EntityUid uid, EntProtoId<IFFFactionComponent> faction)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		return IsInFaction(Entity<UserIFFComponent>.op_Implicit((ValueTuple<EntityUid, UserIFFComponent>)(uid, null)), faction);
	}

	public void SetIdFaction(Entity<ItemIFFComponent> card, EntProtoId<IFFFactionComponent> faction)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		card.Comp.Factions.Clear();
		card.Comp.Factions.Add(faction);
		((EntitySystem)this).Dirty<ItemIFFComponent>(card, (MetaDataComponent)null);
	}

	public void SetUserFaction(Entity<UserIFFComponent?> user, EntProtoId<IFFFactionComponent> faction)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		user.Comp = ((EntitySystem)this).EnsureComp<UserIFFComponent>(Entity<UserIFFComponent>.op_Implicit(user));
		user.Comp.Factions.Clear();
		user.Comp.Factions.Add(faction);
		((EntitySystem)this).Dirty<UserIFFComponent>(user, (MetaDataComponent)null);
	}

	public void AddUserFaction(Entity<UserIFFComponent?> user, EntProtoId<IFFFactionComponent> faction)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		user.Comp = ((EntitySystem)this).EnsureComp<UserIFFComponent>(Entity<UserIFFComponent>.op_Implicit(user));
		user.Comp.Factions.Add(faction);
		((EntitySystem)this).Dirty<UserIFFComponent>(user, (MetaDataComponent)null);
	}

	public void RemoveUserFaction(Entity<UserIFFComponent?> user, EntProtoId<IFFFactionComponent> faction)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		if (_userIFFQuery.Resolve(Entity<UserIFFComponent>.op_Implicit(user), ref user.Comp, false))
		{
			user.Comp.Factions.Remove(faction);
			((EntitySystem)this).Dirty<UserIFFComponent>(user, (MetaDataComponent)null);
		}
	}

	public void ClearUserFactions(Entity<UserIFFComponent?> user)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		user.Comp = ((EntitySystem)this).EnsureComp<UserIFFComponent>(Entity<UserIFFComponent>.op_Implicit(user));
		user.Comp.Factions.Clear();
		((EntitySystem)this).Dirty<UserIFFComponent>(user, (MetaDataComponent)null);
	}

	public void SetIFFState(EntityUid ent, bool enabled)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		GunIFFComponent comp = default(GunIFFComponent);
		if (((EntitySystem)this).TryComp<GunIFFComponent>(ent, ref comp))
		{
			comp.Enabled = enabled;
			((EntitySystem)this).Dirty(ent, (IComponent)(object)comp, (MetaDataComponent)null);
		}
	}

	public void EnableIntrinsicIFF(EntityUid ent)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		GunIFFComponent comp = ((EntitySystem)this).EnsureComp<GunIFFComponent>(ent);
		comp.Intrinsic = true;
		comp.Enabled = true;
		((EntitySystem)this).Dirty(ent, (IComponent)(object)comp, (MetaDataComponent)null);
	}

	public void GiveAmmoIFF(EntityUid gun, ref AmmoShotEvent args, bool intrinsic, bool enabled)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		EntityUid owner;
		if (intrinsic)
		{
			owner = gun;
		}
		else
		{
			BaseContainer container = default(BaseContainer);
			if (!_container.TryGetOuterContainer(gun, ((EntitySystem)this).Transform(gun), ref container))
			{
				return;
			}
			owner = container.Owner;
			GetIFFGunUserEvent gunUserEvent = default(GetIFFGunUserEvent);
			((EntitySystem)this).RaiseLocalEvent<GetIFFGunUserEvent>(container.Owner, ref gunUserEvent, false);
			if (gunUserEvent.GunUser.HasValue)
			{
				owner = gunUserEvent.GunUser.Value;
			}
		}
		UserIFFComponent ownerIFF = default(UserIFFComponent);
		if (!_userIFFQuery.TryComp(owner, ref ownerIFF))
		{
			return;
		}
		_factionBuffer.Clear();
		if (!TryGetFactions(Entity<UserIFFComponent>.op_Implicit((owner, ownerIFF)), _factionBuffer))
		{
			return;
		}
		bool hasAnyFaction = enabled && _factionBuffer.Count > 0;
		foreach (EntityUid projectile in args.FiredProjectiles)
		{
			ProjectileIFFComponent iff = ((EntitySystem)this).EnsureComp<ProjectileIFFComponent>(projectile);
			iff.Factions.Clear();
			foreach (EntProtoId<IFFFactionComponent> faction in _factionBuffer)
			{
				iff.Factions.Add(faction);
			}
			iff.Enabled = hasAnyFaction;
			((EntitySystem)this).Dirty(projectile, (IComponent)(object)iff, (MetaDataComponent)null);
		}
	}

	public void GiveAmmoIFF(EntityUid uid, EntProtoId<IFFFactionComponent>? faction, bool enabled)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		ProjectileIFFComponent projectileIFFComponent = ((EntitySystem)this).EnsureComp<ProjectileIFFComponent>(uid);
		projectileIFFComponent.Factions.Clear();
		if (faction.HasValue)
		{
			EntProtoId<IFFFactionComponent> add = faction.GetValueOrDefault();
			projectileIFFComponent.Factions.Add(add);
		}
		projectileIFFComponent.Enabled = enabled && projectileIFFComponent.Factions.Count > 0;
		((EntitySystem)this).Dirty(uid, (IComponent)(object)projectileIFFComponent, (MetaDataComponent)null);
	}

	public void GiveAmmoMultiFactionIFF(EntityUid uid, HashSet<EntProtoId<IFFFactionComponent>> factions, bool enabled)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		ProjectileIFFComponent projectileIFFComponent = ((EntitySystem)this).EnsureComp<ProjectileIFFComponent>(uid);
		projectileIFFComponent.Factions.Clear();
		foreach (EntProtoId<IFFFactionComponent> faction in factions)
		{
			projectileIFFComponent.Factions.Add(faction);
		}
		projectileIFFComponent.Enabled = enabled && projectileIFFComponent.Factions.Count > 0;
		((EntitySystem)this).Dirty(uid, (IComponent)(object)projectileIFFComponent, (MetaDataComponent)null);
	}
}
