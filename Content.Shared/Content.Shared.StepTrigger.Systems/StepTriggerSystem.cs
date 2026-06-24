using System;
using Content.Shared.Gravity;
using Content.Shared.StepTrigger.Components;
using Content.Shared.Whitelist;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map.Components;
using Robust.Shared.Map.Enumerators;
using Robust.Shared.Maths;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Events;

namespace Content.Shared.StepTrigger.Systems;

public sealed class StepTriggerSystem : EntitySystem
{
	[Dependency]
	private EntityLookupSystem _entityLookup;

	[Dependency]
	private SharedGravitySystem _gravity;

	[Dependency]
	private SharedMapSystem _map;

	[Dependency]
	private EntityWhitelistSystem _whitelistSystem;

	public override void Initialize()
	{
		((EntitySystem)this).UpdatesOutsidePrediction = true;
		((EntitySystem)this).SubscribeLocalEvent<StepTriggerComponent, AfterAutoHandleStateEvent>((ComponentEventRefHandler<StepTriggerComponent, AfterAutoHandleStateEvent>)TriggerHandleState, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<StepTriggerComponent, StartCollideEvent>((ComponentEventRefHandler<StepTriggerComponent, StartCollideEvent>)OnStartCollide, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<StepTriggerComponent, EndCollideEvent>((ComponentEventRefHandler<StepTriggerComponent, EndCollideEvent>)OnEndCollide, (Type[])null, (Type[])null);
	}

	public override void Update(float frameTime)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		EntityQuery<PhysicsComponent> query = ((EntitySystem)this).GetEntityQuery<PhysicsComponent>();
		EntityQueryEnumerator<StepTriggerActiveComponent, StepTriggerComponent, TransformComponent> enumerator = ((EntitySystem)this).EntityQueryEnumerator<StepTriggerActiveComponent, StepTriggerComponent, TransformComponent>();
		EntityUid uid = default(EntityUid);
		StepTriggerActiveComponent active = default(StepTriggerActiveComponent);
		StepTriggerComponent trigger = default(StepTriggerComponent);
		TransformComponent transform = default(TransformComponent);
		while (enumerator.MoveNext(ref uid, ref active, ref trigger, ref transform))
		{
			if (Update(uid, trigger, transform, query))
			{
				((EntitySystem)this).RemCompDeferred(uid, (IComponent)(object)active);
			}
		}
	}

	private bool Update(EntityUid uid, StepTriggerComponent component, TransformComponent transform, EntityQuery<PhysicsComponent> query)
	{
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		if (!component.Active || component.Colliding.Count == 0)
		{
			return true;
		}
		MapGridComponent grid = default(MapGridComponent);
		if (component.Blacklist != null && ((EntitySystem)this).TryComp<MapGridComponent>(transform.GridUid, ref grid))
		{
			Vector2i positon = _map.LocalToTile(transform.GridUid.Value, grid, transform.Coordinates);
			AnchoredEntitiesEnumerator anch = _map.GetAnchoredEntitiesEnumerator(uid, grid, positon);
			EntityUid? ent = default(EntityUid?);
			while (((AnchoredEntitiesEnumerator)(ref anch)).MoveNext(ref ent))
			{
				EntityUid? val = ent;
				if ((!val.HasValue || !(val.GetValueOrDefault() == uid)) && _whitelistSystem.IsBlacklistPass(component.Blacklist, ent.Value))
				{
					return false;
				}
			}
		}
		foreach (EntityUid otherUid in component.Colliding)
		{
			UpdateColliding(uid, component, transform, otherUid, query);
		}
		return false;
	}

	private void UpdateColliding(EntityUid uid, StepTriggerComponent component, TransformComponent ownerXform, EntityUid otherUid, EntityQuery<PhysicsComponent> query)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		PhysicsComponent otherPhysics = default(PhysicsComponent);
		if (!query.TryGetComponent(otherUid, ref otherPhysics))
		{
			return;
		}
		TransformComponent otherXform = ((EntitySystem)this).Transform(otherUid);
		Box2 ourAabb = _entityLookup.GetAABBNoContainer(uid, ownerXform.LocalPosition, ownerXform.LocalRotation);
		Box2 otherAabb = _entityLookup.GetAABBNoContainer(otherUid, otherXform.LocalPosition, otherXform.LocalRotation);
		if (!((Box2)(ref ourAabb)).Intersects(ref otherAabb))
		{
			if (component.CurrentlySteppedOn.Remove(otherUid))
			{
				((EntitySystem)this).Dirty(uid, (IComponent)(object)component, (MetaDataComponent)null);
			}
			return;
		}
		Box2 val = ((Box2)(ref otherAabb)).Intersect(ref ourAabb);
		float intersect = Box2.Area(ref val);
		float ratio = Math.Max(intersect / Box2.Area(ref otherAabb), intersect / Box2.Area(ref ourAabb));
		if (!(otherPhysics.LinearVelocity.Length() < component.RequiredTriggeredSpeed) && !component.CurrentlySteppedOn.Contains(otherUid) && !(ratio < component.IntersectRatio) && CanTrigger(uid, otherUid, component))
		{
			if (component.StepOn)
			{
				StepTriggeredOnEvent evStep = new StepTriggeredOnEvent(uid, otherUid);
				((EntitySystem)this).RaiseLocalEvent<StepTriggeredOnEvent>(uid, ref evStep, false);
			}
			else
			{
				StepTriggeredOffEvent evStep2 = new StepTriggeredOffEvent(uid, otherUid);
				((EntitySystem)this).RaiseLocalEvent<StepTriggeredOffEvent>(uid, ref evStep2, false);
			}
			component.CurrentlySteppedOn.Add(otherUid);
			((EntitySystem)this).Dirty(uid, (IComponent)(object)component, (MetaDataComponent)null);
		}
	}

	private bool CanTrigger(EntityUid uid, EntityUid otherUid, StepTriggerComponent component)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Invalid comparison between Unknown and I4
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		if (!component.Active || component.CurrentlySteppedOn.Contains(otherUid))
		{
			return false;
		}
		PhysicsComponent physics = default(PhysicsComponent);
		if (!component.IgnoreWeightless && ((EntitySystem)this).TryComp<PhysicsComponent>(otherUid, ref physics) && ((int)physics.BodyStatus == 1 || _gravity.IsWeightless(otherUid, physics)))
		{
			return false;
		}
		StepTriggerAttemptEvent msg = new StepTriggerAttemptEvent
		{
			Source = uid,
			Tripper = otherUid
		};
		((EntitySystem)this).RaiseLocalEvent<StepTriggerAttemptEvent>(uid, ref msg, false);
		if (msg.Continue)
		{
			return !msg.Cancelled;
		}
		return false;
	}

	private void OnStartCollide(EntityUid uid, StepTriggerComponent component, ref StartCollideEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		EntityUid otherUid = args.OtherEntity;
		if (args.OtherFixture.Hard && CanTrigger(uid, otherUid, component))
		{
			((EntitySystem)this).EnsureComp<StepTriggerActiveComponent>(uid);
			if (component.Colliding.Add(otherUid))
			{
				((EntitySystem)this).Dirty(uid, (IComponent)(object)component, (MetaDataComponent)null);
			}
		}
	}

	private void OnEndCollide(EntityUid uid, StepTriggerComponent component, ref EndCollideEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		EntityUid otherUid = args.OtherEntity;
		if (component.Colliding.Remove(otherUid))
		{
			component.CurrentlySteppedOn.Remove(otherUid);
			((EntitySystem)this).Dirty(uid, (IComponent)(object)component, (MetaDataComponent)null);
			if (component.StepOn)
			{
				StepTriggeredOffEvent evStepOff = new StepTriggeredOffEvent(uid, otherUid);
				((EntitySystem)this).RaiseLocalEvent<StepTriggeredOffEvent>(uid, ref evStepOff, false);
			}
			if (component.Colliding.Count == 0)
			{
				((EntitySystem)this).RemCompDeferred<StepTriggerActiveComponent>(uid);
			}
		}
	}

	private void TriggerHandleState(EntityUid uid, StepTriggerComponent component, ref AfterAutoHandleStateEvent args)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		if (component.Colliding.Count > 0)
		{
			((EntitySystem)this).EnsureComp<StepTriggerActiveComponent>(uid);
		}
		else
		{
			((EntitySystem)this).RemCompDeferred<StepTriggerActiveComponent>(uid);
		}
	}

	public void SetIntersectRatio(EntityUid uid, float ratio, StepTriggerComponent? component = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<StepTriggerComponent>(uid, ref component, true) && !MathHelper.CloseToPercent(component.IntersectRatio, ratio, 1E-05))
		{
			component.IntersectRatio = ratio;
			((EntitySystem)this).Dirty(uid, (IComponent)(object)component, (MetaDataComponent)null);
		}
	}

	public void SetRequiredTriggerSpeed(EntityUid uid, float speed, StepTriggerComponent? component = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<StepTriggerComponent>(uid, ref component, true) && !MathHelper.CloseToPercent(component.RequiredTriggeredSpeed, speed, 1E-05))
		{
			component.RequiredTriggeredSpeed = speed;
			((EntitySystem)this).Dirty(uid, (IComponent)(object)component, (MetaDataComponent)null);
		}
	}

	public void SetActive(EntityUid uid, bool active, StepTriggerComponent? component = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<StepTriggerComponent>(uid, ref component, true) && active != component.Active)
		{
			component.Active = active;
			((EntitySystem)this).Dirty(uid, (IComponent)(object)component, (MetaDataComponent)null);
		}
	}
}
