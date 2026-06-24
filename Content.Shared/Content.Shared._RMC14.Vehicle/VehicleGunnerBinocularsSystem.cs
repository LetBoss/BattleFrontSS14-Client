using System;
using Content.Shared._RMC14.Scoping;
using Content.Shared.Buckle.Components;
using Content.Shared.Hands.EntitySystems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;

namespace Content.Shared._RMC14.Vehicle;

public sealed class VehicleGunnerBinocularsSystem : EntitySystem
{
	[Dependency]
	private SharedHandsSystem _hands;

	[Dependency]
	private SharedScopeSystem _scope;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<VehicleGunnerBinocularsGiverComponent, StrappedEvent>((EntityEventRefHandler<VehicleGunnerBinocularsGiverComponent, StrappedEvent>)OnStrapped, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<VehicleGunnerBinocularsGiverComponent, UnstrappedEvent>((EntityEventRefHandler<VehicleGunnerBinocularsGiverComponent, UnstrappedEvent>)OnUnstrapped, (Type[])null, (Type[])null);
	}

	private void OnStrapped(Entity<VehicleGunnerBinocularsGiverComponent> seat, ref StrappedEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		EntityUid gunner = args.Buckle.Owner;
		EntityUid binoculars = ((EntitySystem)this).Spawn(EntProtoId.op_Implicit(seat.Comp.BinocularsPrototype), ((EntitySystem)this).Transform(gunner).Coordinates);
		VehicleGunnerBinocularsComponent marker = ((EntitySystem)this).EnsureComp<VehicleGunnerBinocularsComponent>(binoculars);
		marker.Gunner = gunner;
		((EntitySystem)this).Dirty(binoculars, (IComponent)(object)marker, (MetaDataComponent)null);
		if (_hands.TryPickupAnyHand(gunner, binoculars))
		{
			ScopeComponent scopeComp = default(ScopeComponent);
			if (((EntitySystem)this).TryComp<ScopeComponent>(binoculars, ref scopeComp))
			{
				_scope.StartScoping(Entity<ScopeComponent>.op_Implicit((binoculars, scopeComp)), gunner);
			}
		}
		else
		{
			((EntitySystem)this).QueueDel((EntityUid?)binoculars);
		}
	}

	private void OnUnstrapped(Entity<VehicleGunnerBinocularsGiverComponent> seat, ref UnstrappedEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		EntityUid gunner = args.Buckle.Owner;
		EntityQueryEnumerator<VehicleGunnerBinocularsComponent> query = ((EntitySystem)this).EntityQueryEnumerator<VehicleGunnerBinocularsComponent>();
		EntityUid uid = default(EntityUid);
		VehicleGunnerBinocularsComponent comp = default(VehicleGunnerBinocularsComponent);
		while (query.MoveNext(ref uid, ref comp))
		{
			EntityUid? gunner2 = comp.Gunner;
			EntityUid val = gunner;
			if (gunner2.HasValue && gunner2.GetValueOrDefault() == val)
			{
				((EntitySystem)this).QueueDel((EntityUid?)uid);
			}
		}
	}
}
