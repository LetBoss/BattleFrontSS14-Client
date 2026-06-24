using System;
using System.Numerics;
using Content.Shared.Camera;
using Content.Shared.CCVar;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;

namespace Content.Client.Camera;

public sealed class CameraRecoilSystem : SharedCameraRecoilSystem
{
	[Dependency]
	private IConfigurationManager _configManager;

	private float _intensity;

	private float _pubgIntensity = 1f;

	public override void Initialize()
	{
		base.Initialize();
		((EntitySystem)this).SubscribeNetworkEvent<CameraKickEvent>((EntityEventHandler<CameraKickEvent>)OnCameraKick, (Type[])null, (Type[])null);
		EntitySystemSubscriptionExt.CVar<float>(((EntitySystem)this).Subs, _configManager, CCVars.ScreenShakeIntensity, (Action<float>)OnCvarChanged, true);
		EntitySystemSubscriptionExt.CVar<float>(((EntitySystem)this).Subs, _configManager, CCVars.PubgShootingScreenShakeIntensity, (Action<float>)OnPubgCvarChanged, true);
	}

	private void OnCvarChanged(float value)
	{
		_intensity = value;
	}

	private void OnPubgCvarChanged(float value)
	{
		_pubgIntensity = Math.Clamp(value, 0f, 1f);
	}

	private void OnCameraKick(CameraKickEvent ev)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		KickCamera(((EntitySystem)this).GetEntity(ev.NetEntity), ev.Recoil);
	}

	public override void KickCamera(EntityUid uid, Vector2 recoil, CameraRecoilComponent? component = null)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		float num = _intensity * _pubgIntensity;
		if (!(num <= 0f) && ((EntitySystem)this).Resolve<CameraRecoilComponent>(uid, ref component, false))
		{
			recoil *= num;
			float num2 = component.CurrentKick.Length() / 1f;
			component.CurrentKick += recoil * (1f - num2);
			if (component.CurrentKick.Length() > 1f)
			{
				component.CurrentKick = Vector2Helpers.Normalized(component.CurrentKick) * 1f;
			}
			component.LastKickTime = 0f;
		}
	}
}
