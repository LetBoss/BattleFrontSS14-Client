using System;
using System.Collections.Generic;
using System.Linq;
using Content.Shared.Access.Systems;
using Content.Shared.Coordinates;
using Content.Shared.Interaction;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Network;
using Robust.Shared.Physics;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Dynamics;
using Robust.Shared.Physics.Systems;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;

namespace Content.Shared._RMC14.AegisCrate;

public abstract class SharedAegisCrateSystem : EntitySystem
{
	[Dependency]
	private AccessReaderSystem _accessReader;

	[Dependency]
	private SharedTransformSystem _transform;

	[Dependency]
	private IGameTiming _timing;

	[Dependency]
	private SharedAudioSystem _audio;

	[Dependency]
	private INetManager _net;

	[Dependency]
	private SharedPhysicsSystem _physics;

	protected readonly TimeSpan OpeningSpeed = TimeSpan.FromSeconds(1.5);

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<AegisCrateComponent, ComponentStartup>((EntityEventRefHandler<AegisCrateComponent, ComponentStartup>)OnStartup, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<AegisCrateComponent, ActivateInWorldEvent>((EntityEventRefHandler<AegisCrateComponent, ActivateInWorldEvent>)OnActivate, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<AegisCrateComponent, InteractUsingEvent>((EntityEventRefHandler<AegisCrateComponent, InteractUsingEvent>)OnInteractUsing, (Type[])null, (Type[])null);
	}

	protected virtual void OnStartup(Entity<AegisCrateComponent> crate, ref ComponentStartup args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		UpdateCrateVisuals(crate);
	}

	private void UpdateState(Entity<AegisCrateComponent> crate, AegisCrateState newState)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		if (crate.Comp.State != newState)
		{
			crate.Comp.State = newState;
			((EntitySystem)this).Dirty<AegisCrateComponent>(crate, (MetaDataComponent)null);
			UpdateCrateVisuals(crate);
		}
	}

	private void UpdateCrateVisuals(Entity<AegisCrateComponent> crate)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		AegisCrateStateChangedEvent ev = default(AegisCrateStateChangedEvent);
		((EntitySystem)this).RaiseLocalEvent<AegisCrateStateChangedEvent>(Entity<AegisCrateComponent>.op_Implicit(crate), ref ev, false);
	}

	private void OpenAegis(Entity<AegisCrateComponent> crate, EntityUid user)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		if (crate.Comp.State == AegisCrateState.Closed && _accessReader.IsAllowed(user, Entity<AegisCrateComponent>.op_Implicit(crate)))
		{
			UpdateState(crate, AegisCrateState.Opening);
			_audio.PlayPredicted(crate.Comp.OpenSound, Entity<AegisCrateComponent>.op_Implicit(crate), (EntityUid?)user, (AudioParams?)null);
			crate.Comp.OpenAt = _timing.CurTime + OpeningSpeed;
		}
	}

	private void OnActivate(Entity<AegisCrateComponent> crate, ref ActivateInWorldEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		OpenAegis(crate, args.User);
	}

	private void OnInteractUsing(Entity<AegisCrateComponent> crate, ref InteractUsingEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		OpenAegis(crate, args.User);
	}

	public override void Update(float frameTime)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		if (_net.IsClient)
		{
			return;
		}
		TimeSpan time = _timing.CurTime;
		EntityQueryEnumerator<AegisCrateComponent> query = ((EntitySystem)this).EntityQueryEnumerator<AegisCrateComponent>();
		EntityUid uid = default(EntityUid);
		AegisCrateComponent aegis = default(AegisCrateComponent);
		FixturesComponent fixture = default(FixturesComponent);
		while (query.MoveNext(ref uid, ref aegis))
		{
			if (!aegis.Spawned && aegis.OpenAt.HasValue)
			{
				TimeSpan value = time;
				TimeSpan? openAt = aegis.OpenAt;
				if (!(value < openAt) && ((EntitySystem)this).TryComp<FixturesComponent>(uid, ref fixture))
				{
					UpdateState(Entity<AegisCrateComponent>.op_Implicit((uid, aegis)), AegisCrateState.Open);
					KeyValuePair<string, Fixture> fix = fixture.Fixtures.First();
					_physics.SetCollisionLayer(uid, fix.Key, fix.Value, 65, fixture, (PhysicsComponent)null);
					aegis.Spawned = true;
					((EntitySystem)this).Dirty(uid, (IComponent)(object)aegis, (MetaDataComponent)null);
					EntityCoordinates coords = uid.ToCoordinates();
					EntityUid ob = ((EntitySystem)this).SpawnAtPosition(EntProtoId.op_Implicit(aegis.OB), coords, (ComponentRegistry)null);
					((EntitySystem)this).Log.Info($"{ob.Id} spawned at {_transform.GetWorldPosition(ob)}");
				}
			}
		}
	}
}
