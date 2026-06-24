using System;
using Content.Shared._RMC14.Xenonids.Hive;
using Content.Shared.Coordinates;
using Content.Shared.Mobs;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;

namespace Content.Shared._RMC14.Xenonids.Deathcloud;

public sealed class XenoDeathcloudSystem : EntitySystem
{
	[Dependency]
	private INetManager _net;

	[Dependency]
	private SharedXenoHiveSystem _hive;

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<XenoDeathcloudComponent, MobStateChangedEvent>((EntityEventRefHandler<XenoDeathcloudComponent, MobStateChangedEvent>)OnStateChanged, (Type[])null, (Type[])null);
	}

	private void OnStateChanged(Entity<XenoDeathcloudComponent> xeno, ref MobStateChangedEvent args)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		if (!_net.IsClient && args.NewMobState == MobState.Dead)
		{
			EntityUid spawn = ((EntitySystem)this).SpawnAtPosition(EntProtoId.op_Implicit(xeno.Comp.Spawn), xeno.Owner.ToCoordinates(), (ComponentRegistry)null);
			_hive.SetSameHive(Entity<HiveMemberComponent>.op_Implicit(xeno.Owner), Entity<HiveMemberComponent>.op_Implicit(spawn));
		}
	}
}
