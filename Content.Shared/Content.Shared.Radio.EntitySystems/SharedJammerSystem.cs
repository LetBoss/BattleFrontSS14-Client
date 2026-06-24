using System;
using Content.Shared.DeviceNetwork.Components;
using Content.Shared.DeviceNetwork.Systems;
using Content.Shared.Examine;
using Content.Shared.Popups;
using Content.Shared.Radio.Components;
using Content.Shared.Verbs;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;

namespace Content.Shared.Radio.EntitySystems;

public abstract class SharedJammerSystem : EntitySystem
{
	[Dependency]
	private SharedAppearanceSystem _appearance;

	[Dependency]
	private SharedDeviceNetworkJammerSystem _jammer;

	[Dependency]
	protected SharedPopupSystem Popup;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<RadioJammerComponent, GetVerbsEvent<Verb>>((EntityEventRefHandler<RadioJammerComponent, GetVerbsEvent<Verb>>)OnGetVerb, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RadioJammerComponent, ExaminedEvent>((EntityEventRefHandler<RadioJammerComponent, ExaminedEvent>)OnExamine, (Type[])null, (Type[])null);
	}

	private void OnGetVerb(Entity<RadioJammerComponent> entity, ref GetVerbsEvent<Verb> args)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		if (!args.CanAccess || !args.CanInteract)
		{
			return;
		}
		EntityUid user = args.User;
		byte index = 0;
		RadioJammerComponent.RadioJamSetting[] settings = entity.Comp.Settings;
		for (int i = 0; i < settings.Length; i++)
		{
			RadioJammerComponent.RadioJamSetting setting = settings[i];
			byte currIndex = index;
			Verb verb = new Verb
			{
				Priority = currIndex,
				Category = VerbCategory.PowerLevel,
				Disabled = (entity.Comp.SelectedPowerLevel == currIndex),
				Act = delegate
				{
					//IL_002c: Unknown result type (might be due to invalid IL or missing references)
					//IL_0052: Unknown result type (might be due to invalid IL or missing references)
					//IL_0057: Unknown result type (might be due to invalid IL or missing references)
					//IL_006d: Unknown result type (might be due to invalid IL or missing references)
					//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
					//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
					//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
					entity.Comp.SelectedPowerLevel = currIndex;
					((EntitySystem)this).Dirty<RadioJammerComponent>(entity, (MetaDataComponent)null);
					_jammer.TrySetRange(Entity<DeviceNetworkJammerComponent>.op_Implicit(entity.Owner), GetCurrentRange(entity));
					Popup.PopupClient(base.Loc.GetString(LocId.op_Implicit(setting.Message)), user, user);
				},
				Text = base.Loc.GetString(LocId.op_Implicit(setting.Name))
			};
			args.Verbs.Add(verb);
			index++;
		}
	}

	private void OnExamine(Entity<RadioJammerComponent> ent, ref ExaminedEvent args)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		if (args.IsInDetailsRange)
		{
			string powerIndicator = (((EntitySystem)this).HasComp<ActiveRadioJammerComponent>(Entity<RadioJammerComponent>.op_Implicit(ent)) ? base.Loc.GetString("radio-jammer-component-examine-on-state") : base.Loc.GetString("radio-jammer-component-examine-off-state"));
			args.PushMarkup(powerIndicator);
			string powerLevel = base.Loc.GetString(LocId.op_Implicit(ent.Comp.Settings[ent.Comp.SelectedPowerLevel].Name));
			string switchIndicator = base.Loc.GetString("radio-jammer-component-switch-setting", (ValueTuple<string, object>)("powerLevel", powerLevel));
			args.PushMarkup(switchIndicator);
		}
	}

	public float GetCurrentWattage(Entity<RadioJammerComponent> jammer)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		return jammer.Comp.Settings[jammer.Comp.SelectedPowerLevel].Wattage;
	}

	public float GetCurrentRange(Entity<RadioJammerComponent> jammer)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		return jammer.Comp.Settings[jammer.Comp.SelectedPowerLevel].Range;
	}

	protected void ChangeLEDState(Entity<AppearanceComponent?> ent, bool isLEDOn)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		_appearance.SetData(Entity<AppearanceComponent>.op_Implicit(ent), (Enum)RadioJammerVisuals.LEDOn, (object)isLEDOn, ent.Comp);
	}

	protected void ChangeChargeLevel(Entity<AppearanceComponent?> ent, RadioJammerChargeLevel chargeLevel)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		_appearance.SetData(Entity<AppearanceComponent>.op_Implicit(ent), (Enum)RadioJammerVisuals.ChargeLevel, (object)chargeLevel, ent.Comp);
	}
}
