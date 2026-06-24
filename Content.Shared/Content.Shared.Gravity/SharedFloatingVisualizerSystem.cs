using System;
using System.Numerics;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;

namespace Content.Shared.Gravity;

public abstract class SharedFloatingVisualizerSystem : EntitySystem
{
	[Dependency]
	private SharedGravitySystem GravitySystem;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<FloatingVisualsComponent, ComponentStartup>((ComponentEventHandler<FloatingVisualsComponent, ComponentStartup>)OnComponentStartup, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<GravityChangedEvent>((EntityEventRefHandler<GravityChangedEvent>)OnGravityChanged, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<FloatingVisualsComponent, EntParentChangedMessage>((ComponentEventRefHandler<FloatingVisualsComponent, EntParentChangedMessage>)OnEntParentChanged, (Type[])null, (Type[])null);
	}

	public virtual void FloatAnimation(EntityUid uid, Vector2 offset, string animationKey, float animationTime, bool stop = false)
	{
	}

	protected bool CanFloat(EntityUid uid, FloatingVisualsComponent component, TransformComponent? transform = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve(uid, ref transform, true))
		{
			return false;
		}
		if (transform.MapID == MapId.Nullspace)
		{
			return false;
		}
		component.CanFloat = GravitySystem.IsWeightless(uid, null, transform);
		((EntitySystem)this).Dirty(uid, (IComponent)(object)component, (MetaDataComponent)null);
		return component.CanFloat;
	}

	private void OnComponentStartup(EntityUid uid, FloatingVisualsComponent component, ComponentStartup args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		if (CanFloat(uid, component))
		{
			FloatAnimation(uid, component.Offset, component.AnimationKey, component.AnimationTime);
		}
	}

	private void OnGravityChanged(ref GravityChangedEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		EntityQueryEnumerator<FloatingVisualsComponent, TransformComponent> query = ((EntitySystem)this).EntityQueryEnumerator<FloatingVisualsComponent, TransformComponent>();
		EntityUid uid = default(EntityUid);
		FloatingVisualsComponent floating = default(FloatingVisualsComponent);
		TransformComponent transform = default(TransformComponent);
		while (query.MoveNext(ref uid, ref floating, ref transform))
		{
			if (transform.MapID == MapId.Nullspace)
			{
				continue;
			}
			EntityUid? gridUid = transform.GridUid;
			EntityUid changedGridIndex = args.ChangedGridIndex;
			if (gridUid.HasValue && !(gridUid.GetValueOrDefault() != changedGridIndex))
			{
				floating.CanFloat = !args.HasGravity;
				((EntitySystem)this).Dirty(uid, (IComponent)(object)floating, (MetaDataComponent)null);
				if (!args.HasGravity)
				{
					FloatAnimation(uid, floating.Offset, floating.AnimationKey, floating.AnimationTime);
				}
			}
		}
	}

	private void OnEntParentChanged(EntityUid uid, FloatingVisualsComponent component, ref EntParentChangedMessage args)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		TransformComponent transform = ((EntParentChangedMessage)(ref args)).Transform;
		if (CanFloat(uid, component, transform))
		{
			FloatAnimation(uid, component.Offset, component.AnimationKey, component.AnimationTime);
		}
	}
}
