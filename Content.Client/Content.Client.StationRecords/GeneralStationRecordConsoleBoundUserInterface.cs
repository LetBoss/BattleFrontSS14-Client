using System;
using Content.Shared.StationRecords;
using Robust.Client.UserInterface;
using Robust.Shared.GameObjects;
using Robust.Shared.ViewVariables;

namespace Content.Client.StationRecords;

public sealed class GeneralStationRecordConsoleBoundUserInterface : BoundUserInterface
{
	[ViewVariables]
	private GeneralStationRecordConsoleWindow? _window;

	public GeneralStationRecordConsoleBoundUserInterface(EntityUid owner, Enum uiKey)
		: base(owner, uiKey)
	{
	}//IL_0001: Unknown result type (might be due to invalid IL or missing references)


	protected override void Open()
	{
		((BoundUserInterface)this).Open();
		_window = BoundUserInterfaceExt.CreateWindow<GeneralStationRecordConsoleWindow>((BoundUserInterface)(object)this);
		GeneralStationRecordConsoleWindow? window = _window;
		window.OnKeySelected = (Action<uint?>)Delegate.Combine(window.OnKeySelected, (Action<uint?>)delegate(uint? key)
		{
			((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)new SelectStationRecord(key));
		});
		GeneralStationRecordConsoleWindow? window2 = _window;
		window2.OnFiltersChanged = (Action<StationRecordFilterType, string>)Delegate.Combine(window2.OnFiltersChanged, (Action<StationRecordFilterType, string>)delegate(StationRecordFilterType type, string filterValue)
		{
			((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)new SetStationRecordFilter(type, filterValue));
		});
		GeneralStationRecordConsoleWindow? window3 = _window;
		window3.OnDeleted = (Action<uint>)Delegate.Combine(window3.OnDeleted, (Action<uint>)delegate(uint id)
		{
			((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)new DeleteStationRecord(id));
		});
	}

	protected override void UpdateState(BoundUserInterfaceState state)
	{
		((BoundUserInterface)this).UpdateState(state);
		if (state is GeneralStationRecordConsoleState state2)
		{
			_window?.UpdateState(state2);
		}
	}
}
