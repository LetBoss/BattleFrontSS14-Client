using System;
using Content.Shared._RMC14.Hands;
using Content.Shared.Weapons.Ranged.Events;
using Robust.Client.Player;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Player;
using Robust.Shared.Timing;

namespace Content.Client._RMC14.ItemPickup;

public sealed class ItemPickupSystem : EntitySystem
{
	[Dependency]
	private IPlayerManager _player;

	[Dependency]
	private IGameTiming _timing;

	private TimeSpan _lastPickUp;

	public bool RecentItemPickUp { get; private set; }

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<RequestStopShootEvent>((EntityEventHandler<RequestStopShootEvent>)OnRequestStopShoot, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ItemPickedUpEvent>((EntityEventRefHandler<ItemPickedUpEvent>)OnItemPickedUp, (Type[])null, (Type[])null);
	}

	private void OnRequestStopShoot(RequestStopShootEvent ev)
	{
		RecentItemPickUp = false;
	}

	private void OnItemPickedUp(ref ItemPickedUpEvent ev)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		EntityUid user = ev.User;
		EntityUid? localEntity = ((ISharedPlayerManager)_player).LocalEntity;
		if (localEntity.HasValue && !(user != localEntity.GetValueOrDefault()))
		{
			RecentItemPickUp = true;
			_lastPickUp = _timing.CurTime;
		}
	}

	public override void Update(float frameTime)
	{
		if (RecentItemPickUp && _timing.CurTime > _lastPickUp + TimeSpan.FromSeconds(0.15000000596046448))
		{
			RecentItemPickUp = false;
		}
	}
}
