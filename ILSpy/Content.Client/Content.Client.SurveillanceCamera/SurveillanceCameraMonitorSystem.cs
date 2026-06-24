using System;
using Robust.Shared.GameObjects;

namespace Content.Client.SurveillanceCamera;

public sealed class SurveillanceCameraMonitorSystem : EntitySystem
{
	public override void Update(float frameTime)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		EntityQueryEnumerator<ActiveSurveillanceCameraMonitorVisualsComponent> val = ((EntitySystem)this).EntityQueryEnumerator<ActiveSurveillanceCameraMonitorVisualsComponent>();
		EntityUid val2 = default(EntityUid);
		ActiveSurveillanceCameraMonitorVisualsComponent activeSurveillanceCameraMonitorVisualsComponent = default(ActiveSurveillanceCameraMonitorVisualsComponent);
		while (val.MoveNext(ref val2, ref activeSurveillanceCameraMonitorVisualsComponent))
		{
			activeSurveillanceCameraMonitorVisualsComponent.TimeLeft -= frameTime;
			if (activeSurveillanceCameraMonitorVisualsComponent.TimeLeft <= 0f)
			{
				activeSurveillanceCameraMonitorVisualsComponent.OnFinish?.Invoke();
				((EntitySystem)this).RemCompDeferred<ActiveSurveillanceCameraMonitorVisualsComponent>(val2);
			}
		}
	}

	public void AddTimer(EntityUid uid, Action onFinish)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).EnsureComp<ActiveSurveillanceCameraMonitorVisualsComponent>(uid).OnFinish = onFinish;
	}

	public void RemoveTimer(EntityUid uid)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).RemCompDeferred<ActiveSurveillanceCameraMonitorVisualsComponent>(uid);
	}
}
