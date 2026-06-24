using System;
using System.Numerics;
using Content.Shared.Movement.Components;
using Content.Shared.Movement.Systems;
using Robust.Client.Player;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Player;

namespace Content.Client.Movement.Systems;

public sealed class ContentEyeSystem : SharedContentEyeSystem
{
	[Dependency]
	private IPlayerManager _player;

	public void RequestZoom(EntityUid uid, Vector2 zoom, bool ignoreLimit, bool scalePvs, ContentEyeComponent? content = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<ContentEyeComponent>(uid, ref content, false))
		{
			((EntitySystem)this).RaisePredictiveEvent<RequestTargetZoomEvent>(new RequestTargetZoomEvent
			{
				TargetZoom = zoom,
				IgnoreLimit = ignoreLimit
			});
			if (scalePvs)
			{
				RequestPvsScale(Math.Max(zoom.X, zoom.Y));
			}
		}
	}

	public void RequestPvsScale(float scale)
	{
		((EntitySystem)this).RaiseNetworkEvent((EntityEventArgs)(object)new RequestPvsScaleEvent(scale));
	}

	public void RequestToggleFov()
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? localEntity = ((ISharedPlayerManager)_player).LocalEntity;
		if (localEntity.HasValue)
		{
			EntityUid valueOrDefault = localEntity.GetValueOrDefault();
			RequestToggleFov(valueOrDefault);
		}
	}

	public void RequestToggleFov(EntityUid uid, EyeComponent? eye = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<EyeComponent>(uid, ref eye, false))
		{
			RequestEye(!eye.DrawFov, eye.DrawLight);
		}
	}

	public void RequestToggleLight(EntityUid uid, EyeComponent? eye = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<EyeComponent>(uid, ref eye, false))
		{
			RequestEye(eye.DrawFov, !eye.DrawLight);
		}
	}

	public void RequestEye(bool drawFov, bool drawLight)
	{
		((EntitySystem)this).RaisePredictiveEvent<RequestEyeEvent>(new RequestEyeEvent(drawFov, drawLight));
	}

	public override void FrameUpdate(float frameTime)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).FrameUpdate(frameTime);
		AllEntityQueryEnumerator<ContentEyeComponent, EyeComponent> val = ((EntitySystem)this).AllEntityQuery<ContentEyeComponent, EyeComponent>();
		EntityUid item = default(EntityUid);
		ContentEyeComponent contentEyeComponent = default(ContentEyeComponent);
		EyeComponent item2 = default(EyeComponent);
		while (val.MoveNext(ref item, ref contentEyeComponent, ref item2))
		{
			UpdateEyeOffset(Entity<EyeComponent>.op_Implicit((item, item2)));
		}
	}
}
