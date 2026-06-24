using System;
using Content.Shared.Examine;
using Content.Shared.Popups;
using Content.Shared.Power;
using Content.Shared.Power.Components;
using Content.Shared.Power.EntitySystems;
using Content.Shared.Temperature.Components;
using Content.Shared.Verbs;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Shared.Temperature.Systems;

public abstract class SharedEntityHeaterSystem : EntitySystem
{
	[Dependency]
	private SharedAppearanceSystem _appearance;

	[Dependency]
	private SharedPopupSystem _popup;

	[Dependency]
	private SharedPowerReceiverSystem _receiver;

	[Dependency]
	private SharedAudioSystem _audio;

	private readonly int _settingCount = Enum.GetValues<EntityHeaterSetting>().Length;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<EntityHeaterComponent, ExaminedEvent>((EntityEventRefHandler<EntityHeaterComponent, ExaminedEvent>)OnExamined, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<EntityHeaterComponent, GetVerbsEvent<AlternativeVerb>>((EntityEventRefHandler<EntityHeaterComponent, GetVerbsEvent<AlternativeVerb>>)OnGetVerbs, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<EntityHeaterComponent, PowerChangedEvent>((EntityEventRefHandler<EntityHeaterComponent, PowerChangedEvent>)OnPowerChanged, (Type[])null, (Type[])null);
	}

	private void OnExamined(Entity<EntityHeaterComponent> ent, ref ExaminedEvent args)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		if (args.IsInDetailsRange)
		{
			args.PushMarkup(base.Loc.GetString("entity-heater-examined", (ValueTuple<string, object>)("setting", ent.Comp.Setting)));
		}
	}

	private void OnGetVerbs(Entity<EntityHeaterComponent> ent, ref GetVerbsEvent<AlternativeVerb> args)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		if (args.CanAccess && args.CanInteract)
		{
			int nextSettingIndex = (int)(ent.Comp.Setting + 1) % _settingCount;
			EntityHeaterSetting nextSetting = (EntityHeaterSetting)nextSettingIndex;
			EntityUid user = args.User;
			args.Verbs.Add(new AlternativeVerb
			{
				Text = base.Loc.GetString("entity-heater-switch-setting", (ValueTuple<string, object>)("setting", nextSetting)),
				Act = delegate
				{
					//IL_0007: Unknown result type (might be due to invalid IL or missing references)
					//IL_0013: Unknown result type (might be due to invalid IL or missing references)
					ChangeSetting(ent, nextSetting, user);
				}
			});
		}
	}

	private void OnPowerChanged(Entity<EntityHeaterComponent> ent, ref PowerChangedEvent args)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		EntityHeaterSetting setting = (args.Powered ? ent.Comp.Setting : EntityHeaterSetting.Off);
		_appearance.SetData(Entity<EntityHeaterComponent>.op_Implicit(ent), (Enum)EntityHeaterVisuals.Setting, (object)setting, (AppearanceComponent)null);
	}

	protected virtual void ChangeSetting(Entity<EntityHeaterComponent> ent, EntityHeaterSetting setting, EntityUid? user = null)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		ent.Comp.Setting = setting;
		_audio.PlayPredicted((SoundSpecifier)(object)ent.Comp.SettingSound, Entity<EntityHeaterComponent>.op_Implicit(ent), user, (AudioParams?)null);
		_popup.PopupClient(base.Loc.GetString("entity-heater-switched-setting", (ValueTuple<string, object>)("setting", setting)), Entity<EntityHeaterComponent>.op_Implicit(ent), user);
		((EntitySystem)this).Dirty<EntityHeaterComponent>(ent, (MetaDataComponent)null);
		if (_receiver.IsPowered(Entity<SharedApcPowerReceiverComponent>.op_Implicit(ent.Owner)))
		{
			_appearance.SetData(Entity<EntityHeaterComponent>.op_Implicit(ent), (Enum)EntityHeaterVisuals.Setting, (object)setting, (AppearanceComponent)null);
		}
	}

	protected float SettingPower(EntityHeaterSetting setting, float max)
	{
		return setting switch
		{
			EntityHeaterSetting.Low => max / 3f, 
			EntityHeaterSetting.Medium => max * 2f / 3f, 
			EntityHeaterSetting.High => max, 
			_ => 0.01f, 
		};
	}
}
