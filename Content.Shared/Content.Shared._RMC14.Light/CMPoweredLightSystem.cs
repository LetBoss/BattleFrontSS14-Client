using System;
using Content.Shared._RMC14.Xenonids;
using Content.Shared.Interaction.Events;
using Content.Shared.Popups;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Shared._RMC14.Light;

public sealed class CMPoweredLightSystem : EntitySystem
{
	[Dependency]
	private SharedPointLightSystem _pointLight;

	[Dependency]
	private SharedPopupSystem _popup;

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<LightBurnHandAttemptEvent>((EntityEventRefHandler<LightBurnHandAttemptEvent>)OnLightBurnHandAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<PreventAttackLightOffComponent, GettingAttackedAttemptEvent>((EntityEventRefHandler<PreventAttackLightOffComponent, GettingAttackedAttemptEvent>)OnPreventAttackLightOffAttackedAttempt, (Type[])null, (Type[])null);
	}

	private void OnLightBurnHandAttempt(ref LightBurnHandAttemptEvent ev)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		ev.Cancelled = true;
		if (!((EntitySystem)this).HasComp<XenoComponent>(ev.User))
		{
			_popup.PopupClient(base.Loc.GetString("cm-light-failed"), ev.Light, ev.User);
		}
	}

	private void OnPreventAttackLightOffAttackedAttempt(Entity<PreventAttackLightOffComponent> ent, ref GettingAttackedAttemptEvent args)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		if (!args.Cancelled && IsOff(Entity<PreventAttackLightOffComponent>.op_Implicit(ent)))
		{
			args.Cancelled = true;
		}
	}

	public bool IsOff(EntityUid light)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		SharedPointLightComponent pointLight = default(SharedPointLightComponent);
		if (_pointLight.TryGetLight(light, ref pointLight))
		{
			return !pointLight.Enabled;
		}
		return true;
	}
}
