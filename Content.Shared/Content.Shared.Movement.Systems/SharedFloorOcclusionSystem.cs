using System;
using Content.Shared._RMC14.Water;
using Content.Shared.Movement.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Physics.Events;

namespace Content.Shared.Movement.Systems;

public abstract class SharedFloorOcclusionSystem : EntitySystem
{
	[Dependency]
	private RMCWaterSystem _rmcWater;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<FloorOccluderComponent, StartCollideEvent>((EntityEventRefHandler<FloorOccluderComponent, StartCollideEvent>)OnStartCollide, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<FloorOccluderComponent, EndCollideEvent>((EntityEventRefHandler<FloorOccluderComponent, EndCollideEvent>)OnEndCollide, (Type[])null, (Type[])null);
	}

	private void OnStartCollide(Entity<FloorOccluderComponent> entity, ref StartCollideEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		EntityUid other = args.OtherEntity;
		FloorOcclusionComponent occlusion = default(FloorOcclusionComponent);
		if (((EntitySystem)this).TryComp<FloorOcclusionComponent>(other, ref occlusion) && !occlusion.Colliding.Contains(entity.Owner) && _rmcWater.CanCollide(Entity<RMCWaterComponent>.op_Implicit(entity.Owner), other))
		{
			occlusion.Colliding.Add(entity.Owner);
			((EntitySystem)this).Dirty(other, (IComponent)(object)occlusion, (MetaDataComponent)null);
			SetEnabled(Entity<FloorOcclusionComponent>.op_Implicit((other, occlusion)));
		}
	}

	private void OnEndCollide(Entity<FloorOccluderComponent> entity, ref EndCollideEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		EntityUid other = args.OtherEntity;
		FloorOcclusionComponent occlusion = default(FloorOcclusionComponent);
		if (((EntitySystem)this).TryComp<FloorOcclusionComponent>(other, ref occlusion) && occlusion.Colliding.Remove(entity.Owner))
		{
			((EntitySystem)this).Dirty(other, (IComponent)(object)occlusion, (MetaDataComponent)null);
			SetEnabled(Entity<FloorOcclusionComponent>.op_Implicit((other, occlusion)));
		}
	}

	protected virtual void SetEnabled(Entity<FloorOcclusionComponent> entity)
	{
	}
}
