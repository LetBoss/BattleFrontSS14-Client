using System;
using System.Numerics;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Player;

namespace Robust.Shared.GameObjects;

public abstract class SharedEyeSystem : EntitySystem
{
	[Dependency]
	private readonly SharedViewSubscriberSystem _views;

	[Dependency]
	protected readonly SharedTransformSystem TransformSystem;

	public override void Initialize()
	{
		base.Initialize();
		SubscribeLocalEvent<EyeComponent, PlayerAttachedEvent>(OnEyePlayerAttached);
		SubscribeLocalEvent<EyeComponent, PlayerDetachedEvent>(OnEyePlayerDetached);
	}

	private void OnEyePlayerAttached(Entity<EyeComponent> ent, ref PlayerAttachedEvent args)
	{
		EntityUid? target = ent.Comp.Target;
		if (target.HasValue && TryComp(ent.Owner, out ActorComponent comp))
		{
			_views.AddViewSubscriber(target.Value, comp.PlayerSession);
		}
	}

	private void OnEyePlayerDetached(Entity<EyeComponent> ent, ref PlayerDetachedEvent args)
	{
		EntityUid? target = ent.Comp.Target;
		if (target.HasValue && TryComp(ent.Owner, out ActorComponent comp))
		{
			_views.RemoveViewSubscriber(target.Value, comp.PlayerSession);
		}
	}

	public void UpdateEye(Entity<EyeComponent?> entity)
	{
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		EyeComponent component = entity.Comp;
		if (Resolve(entity, ref component))
		{
			component.Eye.Offset = component.Offset;
			component.Eye.DrawFov = component.DrawFov;
			component.Eye.DrawLight = component.DrawLight;
			component.Eye.Rotation = component.Rotation;
			component.Eye.Zoom = component.Zoom;
		}
	}

	public void SetOffset(EntityUid uid, Vector2 value, EyeComponent? eyeComponent = null)
	{
		if (Resolve(uid, ref eyeComponent) && !eyeComponent.Offset.Equals(value))
		{
			eyeComponent.Offset = value;
			eyeComponent.Eye.Offset = value;
			DirtyField(uid, eyeComponent, "Offset");
		}
	}

	public void SetDrawFov(EntityUid uid, bool value, EyeComponent? eyeComponent = null)
	{
		if (Resolve(uid, ref eyeComponent) && !eyeComponent.DrawFov.Equals(value))
		{
			eyeComponent.DrawFov = value;
			eyeComponent.Eye.DrawFov = value;
			DirtyField(uid, eyeComponent, "DrawFov");
		}
	}

	public void SetDrawLight(Entity<EyeComponent?> entity, bool value)
	{
		if (Resolve(entity, ref entity.Comp) && entity.Comp.DrawLight != value)
		{
			entity.Comp.DrawLight = value;
			entity.Comp.Eye.DrawLight = value;
			DirtyField(entity, "DrawLight");
		}
	}

	public void SetRotation(EntityUid uid, Angle rotation, EyeComponent? eyeComponent = null)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		if (Resolve(uid, ref eyeComponent) && !((Angle)(ref eyeComponent.Rotation)).Equals(rotation))
		{
			eyeComponent.Rotation = rotation;
			eyeComponent.Eye.Rotation = rotation;
		}
	}

	public void SetTarget(EntityUid uid, EntityUid? value, EyeComponent? eyeComponent = null)
	{
		if (!Resolve(uid, ref eyeComponent) || eyeComponent.Target.Equals(value))
		{
			return;
		}
		if (TryComp(uid, out ActorComponent comp))
		{
			if (value.HasValue)
			{
				_views.AddViewSubscriber(value.Value, comp.PlayerSession);
			}
			EntityUid? target = eyeComponent.Target;
			if (target.HasValue)
			{
				EntityUid valueOrDefault = target.GetValueOrDefault();
				_views.RemoveViewSubscriber(valueOrDefault, comp.PlayerSession);
			}
		}
		eyeComponent.Target = value;
		DirtyField(uid, eyeComponent, "Target");
	}

	public void SetZoom(EntityUid uid, Vector2 value, EyeComponent? eyeComponent = null)
	{
		if (Resolve(uid, ref eyeComponent) && !eyeComponent.Zoom.Equals(value))
		{
			eyeComponent.Zoom = value;
			eyeComponent.Eye.Zoom = value;
		}
	}

	public void SetPvsScale(Entity<EyeComponent?> eye, float scale)
	{
		if (Resolve(eye.Owner, ref eye.Comp, logMissing: false))
		{
			if (!float.IsFinite(scale))
			{
				base.Log.Error($"Attempted to set pvs scale to invalid value: {scale}. Eye: {ToPrettyString(eye)}");
				SetPvsScale(eye, 1f);
			}
			else
			{
				eye.Comp.PvsScale = Math.Clamp(scale, 0.1f, 100f);
			}
		}
	}

	public void SetVisibilityMask(EntityUid uid, int value, EyeComponent? eyeComponent = null)
	{
		if (Resolve(uid, ref eyeComponent) && !eyeComponent.VisibilityMask.Equals(value))
		{
			eyeComponent.VisibilityMask = value;
			DirtyField(uid, eyeComponent, "VisibilityMask");
		}
	}

	public void RefreshVisibilityMask(Entity<EyeComponent?> entity)
	{
		if (Resolve(entity.Owner, ref entity.Comp, logMissing: false))
		{
			GetVisMaskEvent getVisMaskEvent = new GetVisMaskEvent();
			getVisMaskEvent.Entity = entity.Owner;
			GetVisMaskEvent args = getVisMaskEvent;
			RaiseLocalEvent(entity.Owner, ref args, broadcast: true);
			SetVisibilityMask(entity.Owner, args.VisibilityMask, entity.Comp);
		}
	}
}
