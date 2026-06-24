using System;
using System.Collections.Generic;
using Content.Client.UserInterface.Controls;
using Content.Shared.Silicons.StationAi;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.GameObjects;

namespace Content.Client.Silicons.StationAi;

public sealed class StationAiBoundUserInterface : BoundUserInterface
{
	private SimpleRadialMenu? _menu;

	public StationAiBoundUserInterface(EntityUid owner, Enum uiKey)
		: base(owner, uiKey)
	{
	}//IL_0001: Unknown result type (might be due to invalid IL or missing references)


	protected override void Open()
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		((BoundUserInterface)this).Open();
		GetStationAiRadialEvent getStationAiRadialEvent = new GetStationAiRadialEvent();
		((IDirectedEventBus)base.EntMan.EventBus).RaiseLocalEvent<GetStationAiRadialEvent>(((BoundUserInterface)this).Owner, ref getStationAiRadialEvent, false);
		_menu = BoundUserInterfaceExt.CreateWindow<SimpleRadialMenu>((BoundUserInterface)(object)this);
		_menu.Track(((BoundUserInterface)this).Owner);
		IEnumerable<RadialMenuActionOption> models = ConvertToButtons(getStationAiRadialEvent.Actions);
		_menu.SetButtons(models);
		((BaseWindow)_menu).Open();
	}

	private IEnumerable<RadialMenuActionOption> ConvertToButtons(IReadOnlyList<StationAiRadial> actions)
	{
		RadialMenuActionOption[] array = new RadialMenuActionOption[actions.Count];
		for (int i = 0; i < actions.Count; i++)
		{
			StationAiRadial stationAiRadial = actions[i];
			array[i] = new RadialMenuActionOption<BaseStationAiAction>(HandleRadialMenuClick, stationAiRadial.Event)
			{
				Sprite = stationAiRadial.Sprite,
				ToolTip = stationAiRadial.Tooltip
			};
		}
		return array;
	}

	private void HandleRadialMenuClick(BaseStationAiAction p)
	{
		((BoundUserInterface)this).SendPredictedMessage((BoundUserInterfaceMessage)(object)new StationAiRadialMessage
		{
			Event = p
		});
	}
}
