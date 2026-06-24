using System;
using Content.Shared.Light.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Physics;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Dynamics.Contacts;
using Robust.Shared.Physics.Events;
using Robust.Shared.Physics.Systems;

namespace Content.Shared.Light.EntitySystems;

public sealed class LightCollideSystem : EntitySystem
{
	[Dependency]
	private SharedPhysicsSystem _physics;

	[Dependency]
	private SlimPoweredLightSystem _lights;

	private EntityQuery<LightOnCollideComponent> _lightQuery;

	public override void Initialize()
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).Initialize();
		_lightQuery = ((EntitySystem)this).GetEntityQuery<LightOnCollideComponent>();
		((EntitySystem)this).SubscribeLocalEvent<LightOnCollideColliderComponent, PreventCollideEvent>((EntityEventRefHandler<LightOnCollideColliderComponent, PreventCollideEvent>)OnPreventCollide, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<LightOnCollideColliderComponent, StartCollideEvent>((EntityEventRefHandler<LightOnCollideColliderComponent, StartCollideEvent>)OnStart, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<LightOnCollideColliderComponent, EndCollideEvent>((EntityEventRefHandler<LightOnCollideColliderComponent, EndCollideEvent>)OnEnd, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<LightOnCollideColliderComponent, ComponentShutdown>((EntityEventRefHandler<LightOnCollideColliderComponent, ComponentShutdown>)OnCollideShutdown, (Type[])null, (Type[])null);
	}

	private void OnCollideShutdown(Entity<LightOnCollideColliderComponent> ent, ref ComponentShutdown args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).TerminatingOrDeleted(ent.Owner, (MetaDataComponent)null))
		{
			return;
		}
		ContactEnumerator contacts = _physics.GetContacts(Entity<FixturesComponent>.op_Implicit(ent.Owner), false);
		Contact contact = default(Contact);
		while (((ContactEnumerator)(ref contacts)).MoveNext(ref contact))
		{
			if (contact.IsTouching)
			{
				EntityUid other = contact.OtherEnt(ent.Owner);
				if (_lightQuery.HasComp(other))
				{
					_physics.RegenerateContacts(Entity<PhysicsComponent>.op_Implicit(other));
				}
			}
		}
	}

	private void OnPreventCollide(Entity<LightOnCollideColliderComponent> ent, ref PreventCollideEvent args)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		if (!_lightQuery.HasComp(args.OtherEntity))
		{
			args.Cancelled = true;
		}
	}

	private void OnEnd(Entity<LightOnCollideColliderComponent> ent, ref EndCollideEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		if (!(args.OurFixtureId != ent.Comp.FixtureId) && _lightQuery.HasComp(args.OtherEntity) && _physics.GetTouchingContacts(Entity<FixturesComponent>.op_Implicit(args.OtherEntity), (string)null) - 1 <= 0)
		{
			_lights.SetEnabled(Entity<SlimPoweredLightComponent>.op_Implicit(args.OtherEntity), enabled: false);
		}
	}

	private void OnStart(Entity<LightOnCollideColliderComponent> ent, ref StartCollideEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		if (!(args.OurFixtureId != ent.Comp.FixtureId) && _lightQuery.HasComp(args.OtherEntity))
		{
			_lights.SetEnabled(Entity<SlimPoweredLightComponent>.op_Implicit(args.OtherEntity), enabled: true);
		}
	}
}
