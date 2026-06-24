using System;
using Content.Shared.Revenant.Components;
using Content.Shared.Revenant.EntitySystems;
using Robust.Client.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Client.Revenant;

public sealed class RevenantOverloadedLightsSystem : SharedRevenantOverloadedLightsSystem
{
	[Dependency]
	private SharedPointLightSystem _lights;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<RevenantOverloadedLightsComponent, ComponentStartup>((ComponentEventHandler<RevenantOverloadedLightsComponent, ComponentStartup>)OnStartup, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RevenantOverloadedLightsComponent, ComponentShutdown>((ComponentEventHandler<RevenantOverloadedLightsComponent, ComponentShutdown>)OnShutdown, (Type[])null, (Type[])null);
	}

	public override void Update(float frameTime)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		base.Update(frameTime);
		EntityQueryEnumerator<RevenantOverloadedLightsComponent, PointLightComponent> val = ((EntitySystem)this).EntityQueryEnumerator<RevenantOverloadedLightsComponent, PointLightComponent>();
		EntityUid val2 = default(EntityUid);
		RevenantOverloadedLightsComponent revenantOverloadedLightsComponent = default(RevenantOverloadedLightsComponent);
		PointLightComponent val3 = default(PointLightComponent);
		while (val.MoveNext(ref val2, ref revenantOverloadedLightsComponent, ref val3))
		{
			_lights.SetEnergy(val2, 2f * Math.Abs((float)Math.Sin(Math.PI / 4.0 * (double)revenantOverloadedLightsComponent.Accumulator)), (SharedPointLightComponent)(object)val3);
		}
	}

	private void OnStartup(EntityUid uid, RevenantOverloadedLightsComponent component, ComponentStartup args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		SharedPointLightComponent val = _lights.EnsureLight(uid);
		component.OriginalEnergy = val.Energy;
		component.OriginalEnabled = val.Enabled;
		_lights.SetEnabled(uid, component.OriginalEnabled, val, (MetaDataComponent)null);
		((EntitySystem)this).Dirty(uid, (IComponent)(object)val, (MetaDataComponent)null);
	}

	private void OnShutdown(EntityUid uid, RevenantOverloadedLightsComponent component, ComponentShutdown args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		SharedPointLightComponent val = default(SharedPointLightComponent);
		if (_lights.TryGetLight(uid, ref val))
		{
			if (!component.OriginalEnergy.HasValue)
			{
				((EntitySystem)this).RemComp(uid, (IComponent)(object)val);
				return;
			}
			_lights.SetEnergy(uid, component.OriginalEnergy.Value, val);
			_lights.SetEnabled(uid, component.OriginalEnabled, val, (MetaDataComponent)null);
			((EntitySystem)this).Dirty(uid, (IComponent)(object)val, (MetaDataComponent)null);
		}
	}

	protected override void OnZap(Entity<RevenantOverloadedLightsComponent> component)
	{
	}
}
