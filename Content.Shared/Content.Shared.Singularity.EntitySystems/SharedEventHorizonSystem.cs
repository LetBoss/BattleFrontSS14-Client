using System;
using Content.Shared.Ghost;
using Content.Shared.Singularity.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map.Components;
using Robust.Shared.Physics;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Dynamics;
using Robust.Shared.Physics.Events;
using Robust.Shared.Physics.Systems;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Singularity.EntitySystems;

public abstract class SharedEventHorizonSystem : EntitySystem
{
	[Dependency]
	private FixtureSystem _fixtures;

	[Dependency]
	private SharedPhysicsSystem _physics;

	[Dependency]
	protected IViewVariablesManager Vvm;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<EventHorizonComponent, ComponentStartup>((ComponentEventHandler<EventHorizonComponent, ComponentStartup>)OnEventHorizonStartup, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<EventHorizonComponent, PreventCollideEvent>((ComponentEventRefHandler<EventHorizonComponent, PreventCollideEvent>)OnPreventCollide, (Type[])null, (Type[])null);
		ViewVariablesTypeHandler<EventHorizonComponent> typeHandler = Vvm.GetTypeHandler<EventHorizonComponent>();
		typeHandler.AddPath<float>("Radius", (ComponentPropertyGetter<EventHorizonComponent, float>)((EntityUid _, EventHorizonComponent comp) => comp.Radius), (ComponentPropertySetter<EventHorizonComponent, float>)delegate(EntityUid uid, float value, EventHorizonComponent? comp)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			SetRadius(uid, value, updateFixture: true, comp);
		});
		typeHandler.AddPath<bool>("CanBreachContainment", (ComponentPropertyGetter<EventHorizonComponent, bool>)((EntityUid _, EventHorizonComponent comp) => comp.CanBreachContainment), (ComponentPropertySetter<EventHorizonComponent, bool>)delegate(EntityUid uid, bool value, EventHorizonComponent? comp)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			SetCanBreachContainment(uid, value, updateFixture: true, comp);
		});
		typeHandler.AddPath<string>("ColliderFixtureId", (ComponentPropertyGetter<EventHorizonComponent, string>)((EntityUid _, EventHorizonComponent comp) => comp.ColliderFixtureId), (ComponentPropertySetter<EventHorizonComponent, string>)delegate(EntityUid uid, string value, EventHorizonComponent? comp)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			SetColliderFixtureId(uid, value, updateFixture: true, comp);
		});
		typeHandler.AddPath<string>("ConsumerFixtureId", (ComponentPropertyGetter<EventHorizonComponent, string>)((EntityUid _, EventHorizonComponent comp) => comp.ConsumerFixtureId), (ComponentPropertySetter<EventHorizonComponent, string>)delegate(EntityUid uid, string value, EventHorizonComponent? comp)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			SetConsumerFixtureId(uid, value, updateFixture: true, comp);
		});
	}

	public override void Shutdown()
	{
		ViewVariablesTypeHandler<EventHorizonComponent> typeHandler = Vvm.GetTypeHandler<EventHorizonComponent>();
		typeHandler.RemovePath("Radius");
		typeHandler.RemovePath("CanBreachContainment");
		typeHandler.RemovePath("ColliderFixtureId");
		typeHandler.RemovePath("ConsumerFixtureId");
		((EntitySystem)this).Shutdown();
	}

	public void SetRadius(EntityUid uid, float value, bool updateFixture = true, EventHorizonComponent? eventHorizon = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<EventHorizonComponent>(uid, ref eventHorizon, true))
		{
			return;
		}
		float oldValue = eventHorizon.Radius;
		if (value != oldValue)
		{
			eventHorizon.Radius = value;
			((EntitySystem)this).Dirty(uid, (IComponent)(object)eventHorizon, (MetaDataComponent)null);
			if (updateFixture)
			{
				UpdateEventHorizonFixture(uid, null, eventHorizon);
			}
		}
	}

	public void SetCanBreachContainment(EntityUid uid, bool value, bool updateFixture = true, EventHorizonComponent? eventHorizon = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<EventHorizonComponent>(uid, ref eventHorizon, true))
		{
			return;
		}
		bool oldValue = eventHorizon.CanBreachContainment;
		if (value != oldValue)
		{
			eventHorizon.CanBreachContainment = value;
			((EntitySystem)this).Dirty(uid, (IComponent)(object)eventHorizon, (MetaDataComponent)null);
			if (updateFixture)
			{
				UpdateEventHorizonFixture(uid, null, eventHorizon);
			}
		}
	}

	public void SetColliderFixtureId(EntityUid uid, string? value, bool updateFixture = true, EventHorizonComponent? eventHorizon = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<EventHorizonComponent>(uid, ref eventHorizon, true))
		{
			return;
		}
		string oldValue = eventHorizon.ColliderFixtureId;
		if (!(value == oldValue))
		{
			eventHorizon.ColliderFixtureId = value;
			((EntitySystem)this).Dirty(uid, (IComponent)(object)eventHorizon, (MetaDataComponent)null);
			if (updateFixture)
			{
				UpdateEventHorizonFixture(uid, null, eventHorizon);
			}
		}
	}

	public void SetConsumerFixtureId(EntityUid uid, string? value, bool updateFixture = true, EventHorizonComponent? eventHorizon = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<EventHorizonComponent>(uid, ref eventHorizon, true))
		{
			return;
		}
		string oldValue = eventHorizon.ConsumerFixtureId;
		if (!(value == oldValue))
		{
			eventHorizon.ConsumerFixtureId = value;
			((EntitySystem)this).Dirty(uid, (IComponent)(object)eventHorizon, (MetaDataComponent)null);
			if (updateFixture)
			{
				UpdateEventHorizonFixture(uid, null, eventHorizon);
			}
		}
	}

	public void UpdateEventHorizonFixture(EntityUid uid, FixturesComponent? fixtures = null, EventHorizonComponent? eventHorizon = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<EventHorizonComponent>(uid, ref eventHorizon, true))
		{
			return;
		}
		string consumerId = eventHorizon.ConsumerFixtureId;
		string colliderId = eventHorizon.ColliderFixtureId;
		if (consumerId != null && colliderId != null && ((EntitySystem)this).Resolve<FixturesComponent>(uid, ref fixtures, false))
		{
			Fixture consumer = _fixtures.GetFixtureOrNull(uid, consumerId, fixtures);
			if (consumer != null)
			{
				_physics.SetRadius(uid, consumerId, consumer, consumer.Shape, eventHorizon.Radius, fixtures, (PhysicsComponent)null, (TransformComponent)null);
				_physics.SetHard(uid, consumer, false, fixtures);
			}
			Fixture collider = _fixtures.GetFixtureOrNull(uid, colliderId, fixtures);
			if (collider != null)
			{
				_physics.SetRadius(uid, colliderId, collider, collider.Shape, eventHorizon.Radius, fixtures, (PhysicsComponent)null, (TransformComponent)null);
				_physics.SetHard(uid, collider, true, fixtures);
			}
			((EntitySystem)this).Dirty(uid, (IComponent)(object)fixtures, (MetaDataComponent)null);
		}
	}

	private void OnEventHorizonStartup(EntityUid uid, EventHorizonComponent comp, ComponentStartup args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		UpdateEventHorizonFixture(uid, null, comp);
	}

	private void OnPreventCollide(EntityUid uid, EventHorizonComponent comp, ref PreventCollideEvent args)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		if (!args.Cancelled)
		{
			PreventCollide(uid, comp, ref args);
		}
	}

	protected virtual bool PreventCollide(EntityUid uid, EventHorizonComponent comp, ref PreventCollideEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		EntityUid otherUid = args.OtherEntity;
		if (((EntitySystem)this).HasComp<MapGridComponent>(otherUid) || ((EntitySystem)this).HasComp<GhostComponent>(otherUid))
		{
			args.Cancelled = true;
			return true;
		}
		if (((EntitySystem)this).HasComp<ContainmentFieldComponent>(otherUid) || ((EntitySystem)this).HasComp<ContainmentFieldGeneratorComponent>(otherUid))
		{
			if (comp.CanBreachContainment)
			{
				args.Cancelled = true;
			}
			return true;
		}
		return false;
	}
}
