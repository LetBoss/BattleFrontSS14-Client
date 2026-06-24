using System;
using Content.Shared.Item.ItemToggle;
using Content.Shared.Item.ItemToggle.Components;
using Content.Shared.ProximityDetection.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Timing;

namespace Content.Shared.ProximityDetection.Systems;

public sealed class ProximityDetectionSystem : EntitySystem
{
	[Dependency]
	private IGameTiming _timing;

	[Dependency]
	private ItemToggleSystem _toggle;

	private EntityQuery<TransformComponent> _xformQuery;

	public override void Initialize()
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<ProximityDetectorComponent, MapInitEvent>((EntityEventRefHandler<ProximityDetectorComponent, MapInitEvent>)OnMapInit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ProximityDetectorComponent, ItemToggledEvent>((EntityEventRefHandler<ProximityDetectorComponent, ItemToggledEvent>)OnToggled, (Type[])null, (Type[])null);
		_xformQuery = ((EntitySystem)this).GetEntityQuery<TransformComponent>();
	}

	private void OnMapInit(Entity<ProximityDetectorComponent> ent, ref MapInitEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		ProximityDetectorComponent component = ent.Comp;
		component.NextUpdate = _timing.CurTime + component.UpdateCooldown;
		((EntitySystem)this).DirtyField<ProximityDetectorComponent>(Entity<ProximityDetectorComponent>.op_Implicit(ent), component, "NextUpdate", (MetaDataComponent)null);
	}

	private void OnToggled(Entity<ProximityDetectorComponent> ent, ref ItemToggledEvent args)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		if (args.Activated)
		{
			UpdateTarget(ent);
		}
		else
		{
			ClearTarget(ent);
		}
	}

	public override void Update(float frameTime)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		EntityQueryEnumerator<ProximityDetectorComponent> query = ((EntitySystem)this).EntityQueryEnumerator<ProximityDetectorComponent>();
		EntityUid uid = default(EntityUid);
		ProximityDetectorComponent component = default(ProximityDetectorComponent);
		while (query.MoveNext(ref uid, ref component))
		{
			if (!(component.NextUpdate > _timing.CurTime))
			{
				component.NextUpdate += component.UpdateCooldown;
				((EntitySystem)this).DirtyField<ProximityDetectorComponent>(uid, component, "NextUpdate", (MetaDataComponent)null);
				if (_toggle.IsActivated(Entity<ItemToggleComponent>.op_Implicit(uid)))
				{
					UpdateTarget(Entity<ProximityDetectorComponent>.op_Implicit((uid, component)));
				}
			}
		}
	}

	private void ClearTarget(Entity<ProximityDetectorComponent> ent)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		ProximityDetectorComponent component = ent.Comp;
		if (component.Target.HasValue)
		{
			component.Distance = float.PositiveInfinity;
			((EntitySystem)this).DirtyField<ProximityDetectorComponent>(Entity<ProximityDetectorComponent>.op_Implicit(ent), component, "Distance", (MetaDataComponent)null);
			component.Target = null;
			((EntitySystem)this).DirtyField<ProximityDetectorComponent>(Entity<ProximityDetectorComponent>.op_Implicit(ent), component, "Target", (MetaDataComponent)null);
			ProximityTargetUpdatedEvent updatedEv = new ProximityTargetUpdatedEvent(component.Distance, ent);
			((EntitySystem)this).RaiseLocalEvent<ProximityTargetUpdatedEvent>(Entity<ProximityDetectorComponent>.op_Implicit(ent), ref updatedEv, false);
			NewProximityTargetEvent newTargetEv = new NewProximityTargetEvent(component.Distance, ent);
			((EntitySystem)this).RaiseLocalEvent<NewProximityTargetEvent>(Entity<ProximityDetectorComponent>.op_Implicit(ent), ref newTargetEv, false);
		}
	}

	private void UpdateTarget(Entity<ProximityDetectorComponent> detector)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		ProximityDetectorComponent component = detector.Comp;
		TransformComponent transform = default(TransformComponent);
		if (!_xformQuery.TryGetComponent(Entity<ProximityDetectorComponent>.op_Implicit(detector), ref transform))
		{
			return;
		}
		if (((EntitySystem)this).Deleted(component.Target))
		{
			ClearTarget(detector);
		}
		float closestDistance = float.PositiveInfinity;
		EntityUid? closestUid = null;
		CompRegistryEntityEnumerator query = base.EntityManager.CompRegistryQueryEnumerator(component.Components);
		EntityUid uid = default(EntityUid);
		TransformComponent xForm = default(TransformComponent);
		float distance = default(float);
		while (((CompRegistryEntityEnumerator)(ref query)).MoveNext(ref uid))
		{
			if (!_xformQuery.TryGetComponent(uid, ref xForm))
			{
				continue;
			}
			EntityCoordinates coordinates = transform.Coordinates;
			if (((EntityCoordinates)(ref coordinates)).TryDistance((IEntityManager)(object)base.EntityManager, xForm.Coordinates, ref distance) && !(distance > component.Range) && !(distance >= closestDistance))
			{
				ProximityDetectionAttemptEvent detectAttempt = new ProximityDetectionAttemptEvent(distance, detector, uid);
				((EntitySystem)this).RaiseLocalEvent<ProximityDetectionAttemptEvent>(Entity<ProximityDetectorComponent>.op_Implicit(detector), ref detectAttempt, false);
				if (!detectAttempt.Cancelled)
				{
					closestDistance = distance;
					closestUid = uid;
				}
			}
		}
		bool num = component.Distance != closestDistance;
		EntityUid? target = component.Target;
		EntityUid? val = closestUid;
		bool newTarget = target.HasValue != val.HasValue || (target.HasValue && target.GetValueOrDefault() != val.GetValueOrDefault());
		if (num)
		{
			ProximityTargetUpdatedEvent updatedEv = new ProximityTargetUpdatedEvent(closestDistance, detector, closestUid);
			((EntitySystem)this).RaiseLocalEvent<ProximityTargetUpdatedEvent>(Entity<ProximityDetectorComponent>.op_Implicit(detector), ref updatedEv, false);
			component.Distance = closestDistance;
			((EntitySystem)this).DirtyField<ProximityDetectorComponent>(Entity<ProximityDetectorComponent>.op_Implicit(detector), component, "Distance", (MetaDataComponent)null);
		}
		if (newTarget)
		{
			NewProximityTargetEvent newTargetEv = new NewProximityTargetEvent(closestDistance, detector, closestUid);
			((EntitySystem)this).RaiseLocalEvent<NewProximityTargetEvent>(Entity<ProximityDetectorComponent>.op_Implicit(detector), ref newTargetEv, false);
			component.Target = closestUid;
			((EntitySystem)this).DirtyField<ProximityDetectorComponent>(Entity<ProximityDetectorComponent>.op_Implicit(detector), component, "Target", (MetaDataComponent)null);
		}
	}
}
