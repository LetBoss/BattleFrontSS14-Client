using System;
using System.Collections.Generic;
using System.Numerics;
using Content.Shared._RMC14.Camera;
using Content.Shared.Movement.Components;
using Content.Shared.Movement.Systems;
using Robust.Client.GameObjects;
using Robust.Client.Physics;
using Robust.Client.Player;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Player;
using Robust.Shared.Timing;
using Robust.Shared.ViewVariables;

namespace Content.Client.Eye;

public sealed class EyeLerpingSystem : EntitySystem
{
	[Dependency]
	private IPlayerManager _playerManager;

	[Dependency]
	private IGameTiming _gameTiming;

	[Dependency]
	private SharedEyeSystem _eye;

	[Dependency]
	private SharedMoverController _mover;

	[Dependency]
	private SharedTransformSystem _transform;

	[ViewVariables]
	private IEnumerable<LerpingEyeComponent> ActiveEyes => ((EntitySystem)this).EntityQuery<LerpingEyeComponent>(false);

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<EyeComponent, ComponentStartup>((ComponentEventHandler<EyeComponent, ComponentStartup>)OnEyeStartup, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<EyeComponent, ComponentShutdown>((ComponentEventHandler<EyeComponent, ComponentShutdown>)OnEyeShutdown, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<EyeAttachedEvent>((EntityEventRefHandler<EyeAttachedEvent>)OnAttached, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<LerpingEyeComponent, EntParentChangedMessage>((ComponentEventRefHandler<LerpingEyeComponent, EntParentChangedMessage>)HandleMapChange, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<LerpingEyeComponent, LocalPlayerDetachedEvent>((ComponentEventHandler<LerpingEyeComponent, LocalPlayerDetachedEvent>)OnDetached, (Type[])null, (Type[])null);
		((EntitySystem)this).UpdatesAfter.Add(typeof(TransformSystem));
		((EntitySystem)this).UpdatesAfter.Add(typeof(PhysicsSystem));
		((EntitySystem)this).UpdatesBefore.Add(typeof(SharedEyeSystem));
		((EntitySystem)this).UpdatesOutsidePrediction = true;
	}

	private void OnEyeStartup(EntityUid uid, EyeComponent component, ComponentStartup args)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? localEntity = ((ISharedPlayerManager)_playerManager).LocalEntity;
		if (localEntity.HasValue && localEntity.GetValueOrDefault() == uid)
		{
			AddEye(uid, component, automatic: true);
		}
	}

	private void OnEyeShutdown(EntityUid uid, EyeComponent component, ComponentShutdown args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).RemCompDeferred<LerpingEyeComponent>(uid);
	}

	public void AddEye(EntityUid uid, EyeComponent? component = null, bool automatic = false)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<EyeComponent>(uid, ref component, true))
		{
			LerpingEyeComponent lerpingEyeComponent = ((EntitySystem)this).EnsureComp<LerpingEyeComponent>(uid);
			lerpingEyeComponent.TargetRotation = GetRotation(uid);
			lerpingEyeComponent.LastRotation = lerpingEyeComponent.TargetRotation;
			lerpingEyeComponent.ManuallyAdded |= !automatic;
			lerpingEyeComponent.TargetZoom = component.Zoom;
			lerpingEyeComponent.LastZoom = lerpingEyeComponent.TargetZoom;
			if (component.Eye != null)
			{
				_eye.SetRotation(uid, lerpingEyeComponent.TargetRotation, component);
				_eye.SetZoom(uid, lerpingEyeComponent.TargetZoom, component);
			}
		}
	}

	public void RemoveEye(EntityUid uid)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		LerpingEyeComponent lerpingEyeComponent = default(LerpingEyeComponent);
		if (((EntitySystem)this).TryComp<LerpingEyeComponent>(uid, ref lerpingEyeComponent))
		{
			EntityUid? localEntity = ((ISharedPlayerManager)_playerManager).LocalEntity;
			if (localEntity.HasValue && localEntity.GetValueOrDefault() == uid)
			{
				lerpingEyeComponent.ManuallyAdded = false;
			}
			else
			{
				((EntitySystem)this).RemComp(uid, (IComponent)(object)lerpingEyeComponent);
			}
		}
	}

	private void HandleMapChange(EntityUid uid, LerpingEyeComponent component, ref EntParentChangedMessage args)
	{
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? oldMapId = args.OldMapId;
		EntityUid? mapUid = ((EntParentChangedMessage)(ref args)).Transform.MapUid;
		if (oldMapId.HasValue != mapUid.HasValue || (oldMapId.HasValue && oldMapId.GetValueOrDefault() != mapUid.GetValueOrDefault()))
		{
			component.LastRotation = GetRotation(uid, ((EntParentChangedMessage)(ref args)).Transform);
		}
	}

	private void OnAttached(ref EyeAttachedEvent ev)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		AddEye(((EyeAttachedEvent)(ref ev)).Entity, ((EyeAttachedEvent)(ref ev)).Component, automatic: true);
	}

	private void OnDetached(EntityUid uid, LerpingEyeComponent component, LocalPlayerDetachedEvent args)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		if (!component.ManuallyAdded)
		{
			((EntitySystem)this).RemCompDeferred(uid, (IComponent)(object)component);
		}
	}

	public override void Update(float frameTime)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).Update(frameTime);
		if (_gameTiming.IsFirstTimePredicted)
		{
			AllEntityQueryEnumerator<LerpingEyeComponent, TransformComponent> val = ((EntitySystem)this).AllEntityQuery<LerpingEyeComponent, TransformComponent>();
			EntityUid uid = default(EntityUid);
			LerpingEyeComponent lerpingEyeComponent = default(LerpingEyeComponent);
			TransformComponent xform = default(TransformComponent);
			while (val.MoveNext(ref uid, ref lerpingEyeComponent, ref xform))
			{
				lerpingEyeComponent.LastRotation = lerpingEyeComponent.TargetRotation;
				lerpingEyeComponent.TargetRotation = GetRotation(uid, xform);
				lerpingEyeComponent.LastZoom = lerpingEyeComponent.TargetZoom;
				lerpingEyeComponent.TargetZoom = UpdateZoom(uid, frameTime);
			}
		}
	}

	private Vector2 UpdateZoom(EntityUid uid, float frameTime, EyeComponent? eye = null, ContentEyeComponent? content = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<ContentEyeComponent, EyeComponent>(uid, ref content, ref eye, false))
		{
			return Vector2.One;
		}
		Vector2 vector = content.TargetZoom - eye.Zoom;
		if (vector.LengthSquared() < 1E-05f)
		{
			return content.TargetZoom;
		}
		Vector2 vector2 = vector * Math.Min(8f * frameTime, 1f);
		return eye.Zoom + vector2;
	}

	private bool NeedsLerp(InputMoverComponent? mover)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		if (mover == null)
		{
			return false;
		}
		if (((Angle)(ref mover.RelativeRotation)).Equals(mover.TargetRelativeRotation))
		{
			return false;
		}
		return true;
	}

	private Angle GetRotation(EntityUid uid, TransformComponent? xform = null, InputMoverComponent? mover = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve(uid, ref xform, true))
		{
			return Angle.Zero;
		}
		if (((EntitySystem)this).Resolve<InputMoverComponent>(uid, ref mover, false))
		{
			return -_mover.GetParentGridAngle(mover);
		}
		EntityUid? val = xform.GridUid ?? xform.MapUid;
		if (val.HasValue)
		{
			return -_transform.GetWorldRotation(val.Value);
		}
		return Angle.Zero;
	}

	public override void FrameUpdate(float frameTime)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		float num = (float)(int)_gameTiming.TickFraction / 65535f;
		AllEntityQueryEnumerator<LerpingEyeComponent, EyeComponent, TransformComponent> val = ((EntitySystem)this).AllEntityQuery<LerpingEyeComponent, EyeComponent, TransformComponent>();
		EntityUid val2 = default(EntityUid);
		LerpingEyeComponent lerpingEyeComponent = default(LerpingEyeComponent);
		EyeComponent val3 = default(EyeComponent);
		TransformComponent xform = default(TransformComponent);
		InputMoverComponent mover = default(InputMoverComponent);
		while (val.MoveNext(ref val2, ref lerpingEyeComponent, ref val3, ref xform))
		{
			Vector2 vector = Vector2.Lerp(lerpingEyeComponent.LastZoom, lerpingEyeComponent.TargetZoom, num);
			if (!((EntitySystem)this).HasComp<RMCStaticZoomLevelComponent>(val2))
			{
				if ((double)(vector - lerpingEyeComponent.TargetZoom).Length() < 1E-05)
				{
					_eye.SetZoom(val2, lerpingEyeComponent.TargetZoom, val3);
				}
				else
				{
					_eye.SetZoom(val2, vector, val3);
				}
			}
			((EntitySystem)this).TryComp<InputMoverComponent>(val2, ref mover);
			lerpingEyeComponent.TargetRotation = GetRotation(val2, xform, mover);
			if (!NeedsLerp(mover))
			{
				_eye.SetRotation(val2, lerpingEyeComponent.TargetRotation, val3);
				continue;
			}
			Angle val4 = Angle.ShortestDistance(ref lerpingEyeComponent.LastRotation, ref lerpingEyeComponent.TargetRotation);
			if (Math.Abs(val4.Theta) < 1E-05)
			{
				_eye.SetRotation(val2, lerpingEyeComponent.TargetRotation, val3);
			}
			else
			{
				_eye.SetRotation(val2, Angle.op_Implicit(Angle.op_Implicit(val4) * (double)num) + lerpingEyeComponent.LastRotation, val3);
			}
		}
	}
}
