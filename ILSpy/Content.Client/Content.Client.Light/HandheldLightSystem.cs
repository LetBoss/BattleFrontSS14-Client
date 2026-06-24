using System;
using Content.Client.Items;
using Content.Client.Light.Components;
using Content.Client.Light.EntitySystems;
using Content.Shared.Light;
using Content.Shared.Light.Components;
using Content.Shared.Toggleable;
using Robust.Client.GameObjects;
using Robust.Client.UserInterface;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Client.Light;

public sealed class HandheldLightSystem : SharedHandheldLightSystem
{
	[Dependency]
	private SharedAppearanceSystem _appearance;

	[Dependency]
	private LightBehaviorSystem _lightBehavior;

	public override void Initialize()
	{
		base.Initialize();
		((EntitySystem)this).Subs.ItemStatus<HandheldLightComponent>((Func<Entity<HandheldLightComponent>, Control?>)((Entity<HandheldLightComponent> ent) => (Control?)(object)new HandheldLightStatus(Entity<HandheldLightComponent>.op_Implicit(ent))));
		((EntitySystem)this).SubscribeLocalEvent<HandheldLightComponent, AppearanceChangeEvent>((ComponentEventRefHandler<HandheldLightComponent, AppearanceChangeEvent>)OnAppearanceChange, (Type[])null, (Type[])null);
	}

	public override bool TurnOff(Entity<HandheldLightComponent> ent, bool makeNoise = true)
	{
		return true;
	}

	public override bool TurnOn(EntityUid user, Entity<HandheldLightComponent> uid)
	{
		return true;
	}

	private void OnAppearanceChange(EntityUid uid, HandheldLightComponent? component, ref AppearanceChangeEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		bool flag = default(bool);
		HandheldLightPowerStates handheldLightPowerStates = default(HandheldLightPowerStates);
		LightBehaviourComponent item = default(LightBehaviourComponent);
		if (!((EntitySystem)this).Resolve<HandheldLightComponent>(uid, ref component, true) || !_appearance.TryGetData<bool>(uid, (Enum)ToggleableVisuals.Enabled, ref flag, args.Component) || !_appearance.TryGetData<HandheldLightPowerStates>(uid, (Enum)HandheldLightVisuals.Power, ref handheldLightPowerStates, args.Component) || !((EntitySystem)this).TryComp<LightBehaviourComponent>(uid, ref item))
		{
			return;
		}
		if (_lightBehavior.HasRunningBehaviours(Entity<LightBehaviourComponent>.op_Implicit((uid, item))))
		{
			_lightBehavior.StopLightBehaviour(Entity<LightBehaviourComponent>.op_Implicit((uid, item)), "", removeBehaviour: false, resetToOriginalSettings: true);
		}
		if (flag)
		{
			switch (handheldLightPowerStates)
			{
			case HandheldLightPowerStates.LowPower:
				_lightBehavior.StartLightBehaviour(Entity<LightBehaviourComponent>.op_Implicit((uid, item)), component.RadiatingBehaviourId);
				break;
			case HandheldLightPowerStates.Dying:
				_lightBehavior.StartLightBehaviour(Entity<LightBehaviourComponent>.op_Implicit((uid, item)), component.BlinkingBehaviourId);
				break;
			case HandheldLightPowerStates.FullPower:
				break;
			}
		}
	}
}
