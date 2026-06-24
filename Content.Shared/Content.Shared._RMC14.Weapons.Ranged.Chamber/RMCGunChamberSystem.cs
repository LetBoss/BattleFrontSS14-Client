using System;
using System.Collections.Generic;
using Content.Shared._RMC14.Attachable.Systems;
using Content.Shared._RMC14.Weapons.Common;
using Content.Shared._RMC14.Weapons.Ranged.AimedShot;
using Content.Shared._RMC14.Weapons.Ranged.Flamer;
using Content.Shared._RMC14.Weapons.Ranged.Foldable;
using Content.Shared.Weapons.Ranged;
using Content.Shared.Weapons.Ranged.Events;
using Content.Shared.Weapons.Ranged.Systems;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Physics.Components;
using Robust.Shared.Timing;
using Robust.Shared.Utility;

namespace Content.Shared._RMC14.Weapons.Ranged.Chamber;

public sealed class RMCGunChamberSystem : EntitySystem
{
	[Dependency]
	private SharedAudioSystem _audio;

	[Dependency]
	private SharedContainerSystem _container;

	[Dependency]
	private SharedGunSystem _gun;

	[Dependency]
	private INetManager _net;

	[Dependency]
	private IGameTiming _timing;

	[Dependency]
	private SharedTransformSystem _transform;

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<RMCGunChamberComponent, EntRemovedFromContainerMessage>((EntityEventRefHandler<RMCGunChamberComponent, EntRemovedFromContainerMessage>)OnEntRemovedFromContainer, (Type[])null, new Type[1] { typeof(SharedGunSystem) });
		((EntitySystem)this).SubscribeLocalEvent<RMCGunChamberComponent, TakeAmmoEvent>((EntityEventRefHandler<RMCGunChamberComponent, TakeAmmoEvent>)OnTakeAmmo, new Type[1] { typeof(SharedGunSystem) }, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCGunChamberComponent, UniqueActionEvent>((EntityEventRefHandler<RMCGunChamberComponent, UniqueActionEvent>)OnUniqueAction, (Type[])null, new Type[8]
		{
			typeof(SharedRMCAimedShotSystem),
			typeof(AttachableHolderSystem),
			typeof(SharedRMCFlamerSystem),
			typeof(RMCFoldableGunSystem),
			typeof(BreechLoadedSystem),
			typeof(CMGunSystem),
			typeof(RMCAirShotSystem),
			typeof(SharedPumpActionSystem)
		});
	}

	private void OnEntRemovedFromContainer(Entity<RMCGunChamberComponent> ent, ref EntRemovedFromContainerMessage args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		if (ent.Comp.Enabled && !(((ContainerModifiedMessage)args).Container.ID != "gun_magazine"))
		{
			LoadChamber(ent, ((ContainerModifiedMessage)args).Entity);
		}
	}

	private void OnTakeAmmo(Entity<RMCGunChamberComponent> ent, ref TakeAmmoEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		BaseContainer chamber = default(BaseContainer);
		if (!ent.Comp.Enabled || !_container.TryGetContainer(Entity<RMCGunChamberComponent>.op_Implicit(ent), ent.Comp.ContainerId, ref chamber, (ContainerManagerComponent)null))
		{
			return;
		}
		int shots = args.Shots;
		for (int i = 0; i < shots; i++)
		{
			EntityUid? val = Extensions.FirstOrNull<EntityUid>((IEnumerable<EntityUid>)chamber.ContainedEntities);
			if (val.HasValue)
			{
				EntityUid bullet = val.GetValueOrDefault();
				args.Shots--;
				if (_container.Remove(Entity<TransformComponent, MetaDataComponent>.op_Implicit(bullet), chamber, true, false, (EntityCoordinates?)null, (Angle?)null))
				{
					args.Ammo.Add((bullet, _gun.EnsureShootable(bullet)));
					continue;
				}
				break;
			}
			break;
		}
	}

	private void LoadChamber(Entity<RMCGunChamberComponent> gun, EntityUid magazine)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		TransformComponent xform = default(TransformComponent);
		if (((EntitySystem)this).TerminatingOrDeleted(Entity<RMCGunChamberComponent>.op_Implicit(gun), (MetaDataComponent)null) || !((EntitySystem)this).TryComp(Entity<RMCGunChamberComponent>.op_Implicit(gun), ref xform))
		{
			return;
		}
		ContainerSlot chamber = _container.EnsureContainer<ContainerSlot>(Entity<RMCGunChamberComponent>.op_Implicit(gun), gun.Comp.ContainerId, (ContainerManagerComponent)null);
		if (chamber.ContainedEntity.HasValue || _net.IsClient)
		{
			return;
		}
		List<(EntityUid?, IShootable)> ammo = new List<(EntityUid?, IShootable)>();
		TakeAmmoEvent take = new TakeAmmoEvent(1, ammo, xform.Coordinates, null);
		((EntitySystem)this).RaiseLocalEvent<TakeAmmoEvent>(magazine, take, false);
		(EntityUid?, IShootable)? tuple = Extensions.FirstOrNull<(EntityUid?, IShootable)>((IEnumerable<(EntityUid?, IShootable)>)take.Ammo);
		if (tuple.HasValue)
		{
			EntityUid? item = tuple.GetValueOrDefault().Item1;
			if (item.HasValue)
			{
				EntityUid firstAmmo = item.GetValueOrDefault();
				_container.Insert(Entity<TransformComponent, MetaDataComponent, PhysicsComponent>.op_Implicit(firstAmmo), (BaseContainer)(object)chamber, (TransformComponent)null, false);
			}
		}
	}

	private void OnUniqueAction(Entity<RMCGunChamberComponent> ent, ref UniqueActionEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		TransformComponent xform = default(TransformComponent);
		if (!ent.Comp.Enabled || ((HandledEntityEventArgs)args).Handled || !((EntitySystem)this).TryComp(Entity<RMCGunChamberComponent>.op_Implicit(ent), ref xform))
		{
			return;
		}
		((HandledEntityEventArgs)args).Handled = true;
		TimeSpan time = _timing.CurTime;
		TimeSpan? lastChamberedAt = ent.Comp.LastChamberedAt;
		if (lastChamberedAt.HasValue)
		{
			TimeSpan last = lastChamberedAt.GetValueOrDefault();
			if (time < last + ent.Comp.ChamberCooldown)
			{
				return;
			}
		}
		List<(EntityUid?, IShootable)> ammo = new List<(EntityUid?, IShootable)>();
		TakeAmmoEvent take = new TakeAmmoEvent(1, ammo, xform.Coordinates, null);
		((EntitySystem)this).RaiseLocalEvent<TakeAmmoEvent>(Entity<RMCGunChamberComponent>.op_Implicit(ent), take, false);
		if (take.Ammo.Count == 0)
		{
			return;
		}
		foreach (var item in take.Ammo)
		{
			EntityUid? ammoEntNullable = item.Entity;
			if (ammoEntNullable.HasValue)
			{
				EntityUid ammoEnt = ammoEntNullable.GetValueOrDefault();
				_transform.SetCoordinates(ammoEnt, _transform.GetMoverCoordinates(ammoEnt));
				if (((EntitySystem)this).IsClientSide(ammoEnt, (MetaDataComponent)null))
				{
					((EntitySystem)this).QueueDel((EntityUid?)ammoEnt);
				}
			}
		}
		ent.Comp.LastChamberedAt = time;
		((EntitySystem)this).Dirty<RMCGunChamberComponent>(ent, (MetaDataComponent)null);
		_audio.PlayPredicted(ent.Comp.Sound, Entity<RMCGunChamberComponent>.op_Implicit(ent), (EntityUid?)args.UserUid, (AudioParams?)null);
		_gun.UpdateAmmoCount(Entity<RMCGunChamberComponent>.op_Implicit(ent));
	}
}
