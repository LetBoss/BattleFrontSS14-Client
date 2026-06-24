using System;
using Content.Shared.Light.Components;
using Content.Shared.Power;
using Content.Shared.Power.Components;
using Content.Shared.Power.EntitySystems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Shared.Light.EntitySystems;

public sealed class SlimPoweredLightSystem : EntitySystem
{
	[Dependency]
	private SharedPowerReceiverSystem _receiver;

	[Dependency]
	private SharedPointLightSystem _lights;

	private bool _setting;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<SlimPoweredLightComponent, AttemptPointLightToggleEvent>((EntityEventRefHandler<SlimPoweredLightComponent, AttemptPointLightToggleEvent>)OnLightAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<SlimPoweredLightComponent, PowerChangedEvent>((EntityEventRefHandler<SlimPoweredLightComponent, PowerChangedEvent>)OnLightPowerChanged, (Type[])null, (Type[])null);
	}

	private void OnLightAttempt(Entity<SlimPoweredLightComponent> ent, ref AttemptPointLightToggleEvent args)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		if (!_setting && ((AttemptPointLightToggleEvent)(ref args)).Enabled && !_receiver.IsPowered(Entity<SharedApcPowerReceiverComponent>.op_Implicit(ent.Owner)))
		{
			args.Cancelled = true;
		}
	}

	private void OnLightPowerChanged(Entity<SlimPoweredLightComponent> ent, ref PowerChangedEvent args)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		if (args.Powered)
		{
			if (!ent.Comp.Enabled)
			{
				return;
			}
		}
		else if (!ent.Comp.Enabled)
		{
			return;
		}
		SharedPointLightComponent light = default(SharedPointLightComponent);
		if (_lights.TryGetLight(ent.Owner, ref light))
		{
			bool enabled = ent.Comp.Enabled && args.Powered;
			_setting = true;
			_lights.SetEnabled(ent.Owner, enabled, light, (MetaDataComponent)null);
			_setting = false;
		}
	}

	public void SetEnabled(Entity<SlimPoweredLightComponent?> entity, bool enabled)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<SlimPoweredLightComponent>(entity.Owner, ref entity.Comp, false) && entity.Comp.Enabled != enabled)
		{
			entity.Comp.Enabled = enabled;
			((EntitySystem)this).Dirty<SlimPoweredLightComponent>(entity, (MetaDataComponent)null);
			_lights.SetEnabled(entity.Owner, enabled, (SharedPointLightComponent)null, (MetaDataComponent)null);
		}
	}
}
