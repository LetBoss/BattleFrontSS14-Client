using System;
using System.Collections.Generic;
using Content.Shared.Light.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;

namespace Content.Shared.Light;

public abstract class SharedRgbLightControllerSystem : EntitySystem
{
	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<RgbLightControllerComponent, ComponentGetState>((ComponentEventRefHandler<RgbLightControllerComponent, ComponentGetState>)OnGetState, (Type[])null, (Type[])null);
	}

	private void OnGetState(EntityUid uid, RgbLightControllerComponent component, ref ComponentGetState args)
	{
		((ComponentGetState)(ref args)).State = (IComponentState)(object)new RgbLightControllerState(component.CycleRate, component.Layers);
	}

	public void SetLayers(EntityUid uid, List<int>? layers, RgbLightControllerComponent? rgb = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<RgbLightControllerComponent>(uid, ref rgb, true))
		{
			rgb.Layers = layers;
			((EntitySystem)this).Dirty(uid, (IComponent)(object)rgb, (MetaDataComponent)null);
		}
	}

	public void SetCycleRate(EntityUid uid, float rate, RgbLightControllerComponent? rgb = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<RgbLightControllerComponent>(uid, ref rgb, true))
		{
			rgb.CycleRate = Math.Clamp(0.01f, rate, 1f);
			((EntitySystem)this).Dirty(uid, (IComponent)(object)rgb, (MetaDataComponent)null);
		}
	}
}
