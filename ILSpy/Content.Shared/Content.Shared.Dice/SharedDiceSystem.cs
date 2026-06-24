using System;
using Content.Shared.Examine;
using Content.Shared.Interaction.Events;
using Content.Shared.Popups;
using Content.Shared.Throwing;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Timing;

namespace Content.Shared.Dice;

public abstract class SharedDiceSystem : EntitySystem
{
	[Dependency]
	private IGameTiming _timing;

	[Dependency]
	private SharedAudioSystem _audio;

	[Dependency]
	private SharedPopupSystem _popup;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<DiceComponent, UseInHandEvent>((EntityEventRefHandler<DiceComponent, UseInHandEvent>)OnUseInHand, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<DiceComponent, LandEvent>((EntityEventRefHandler<DiceComponent, LandEvent>)OnLand, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<DiceComponent, ExaminedEvent>((EntityEventRefHandler<DiceComponent, ExaminedEvent>)OnExamined, (Type[])null, (Type[])null);
	}

	private void OnUseInHand(Entity<DiceComponent> entity, ref UseInHandEvent args)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		if (!((HandledEntityEventArgs)args).Handled)
		{
			Roll(entity, args.User);
			((HandledEntityEventArgs)args).Handled = true;
		}
	}

	private void OnLand(Entity<DiceComponent> entity, ref LandEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		Roll(entity);
	}

	private void OnExamined(Entity<DiceComponent> entity, ref ExaminedEvent args)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		using (args.PushGroup("DiceComponent"))
		{
			args.PushMarkup(base.Loc.GetString("dice-component-on-examine-message-part-1", (ValueTuple<string, object>)("sidesAmount", entity.Comp.Sides)));
			args.PushMarkup(base.Loc.GetString("dice-component-on-examine-message-part-2", (ValueTuple<string, object>)("currentSide", entity.Comp.CurrentValue)));
		}
	}

	private void SetCurrentSide(Entity<DiceComponent> entity, int side)
	{
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		if (side < 1 || side > entity.Comp.Sides)
		{
			((EntitySystem)this).Log.Error($"Attempted to set die {((EntitySystem)this).ToPrettyString((EntityUid?)Entity<DiceComponent>.op_Implicit(entity), (MetaDataComponent)null)} to an invalid side ({side}).");
		}
		else
		{
			entity.Comp.CurrentValue = (side - entity.Comp.Offset) * entity.Comp.Multiplier;
			((EntitySystem)this).Dirty<DiceComponent>(entity, (MetaDataComponent)null);
		}
	}

	public void SetCurrentValue(Entity<DiceComponent> entity, int value)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		if (value % entity.Comp.Multiplier != 0 || value / entity.Comp.Multiplier + entity.Comp.Offset < 1)
		{
			((EntitySystem)this).Log.Error($"Attempted to set die {((EntitySystem)this).ToPrettyString((EntityUid?)Entity<DiceComponent>.op_Implicit(entity), (MetaDataComponent)null)} to an invalid value ({value}).");
		}
		else
		{
			SetCurrentSide(entity, value / entity.Comp.Multiplier + entity.Comp.Offset);
		}
	}

	private void Roll(Entity<DiceComponent> entity, EntityUid? user = null)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		int roll = new System.Random((int)_timing.CurTick.Value).Next(1, entity.Comp.Sides + 1);
		SetCurrentSide(entity, roll);
		string popupString = base.Loc.GetString("dice-component-on-roll-land", (ValueTuple<string, object>)("die", entity), (ValueTuple<string, object>)("currentSide", entity.Comp.CurrentValue));
		_popup.PopupPredicted(popupString, Entity<DiceComponent>.op_Implicit(entity), user);
		_audio.PlayPredicted(entity.Comp.Sound, Entity<DiceComponent>.op_Implicit(entity), user, (AudioParams?)null);
	}
}
