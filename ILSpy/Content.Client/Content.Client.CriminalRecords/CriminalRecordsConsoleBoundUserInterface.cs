using System;
using Content.Shared.Access.Systems;
using Content.Shared.CriminalRecords;
using Content.Shared.CriminalRecords.Components;
using Content.Shared.Security;
using Content.Shared.StationRecords;
using Robust.Client.Player;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;

namespace Content.Client.CriminalRecords;

public sealed class CriminalRecordsConsoleBoundUserInterface : BoundUserInterface
{
	[Dependency]
	private IPrototypeManager _proto;

	[Dependency]
	private IRobustRandom _random;

	[Dependency]
	private IPlayerManager _playerManager;

	private readonly AccessReaderSystem _accessReader;

	private CriminalRecordsConsoleWindow? _window;

	private CrimeHistoryWindow? _historyWindow;

	public CriminalRecordsConsoleBoundUserInterface(EntityUid owner, Enum uiKey)
		: base(owner, uiKey)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		_accessReader = base.EntMan.System<AccessReaderSystem>();
	}

	protected override void Open()
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		((BoundUserInterface)this).Open();
		CriminalRecordsConsoleComponent component = base.EntMan.GetComponent<CriminalRecordsConsoleComponent>(((BoundUserInterface)this).Owner);
		_window = new CriminalRecordsConsoleWindow(((BoundUserInterface)this).Owner, component.MaxStringLength, _playerManager, _proto, _random, _accessReader);
		CriminalRecordsConsoleWindow? window = _window;
		window.OnKeySelected = (Action<uint?>)Delegate.Combine(window.OnKeySelected, (Action<uint?>)delegate(uint? key)
		{
			((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)new SelectStationRecord(key));
		});
		CriminalRecordsConsoleWindow? window2 = _window;
		window2.OnFiltersChanged = (Action<StationRecordFilterType, string>)Delegate.Combine(window2.OnFiltersChanged, (Action<StationRecordFilterType, string>)delegate(StationRecordFilterType type, string filterValue)
		{
			((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)new SetStationRecordFilter(type, filterValue));
		});
		CriminalRecordsConsoleWindow? window3 = _window;
		window3.OnStatusSelected = (Action<SecurityStatus>)Delegate.Combine(window3.OnStatusSelected, (Action<SecurityStatus>)delegate(SecurityStatus status)
		{
			((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)new CriminalRecordChangeStatus(status, null));
		});
		CriminalRecordsConsoleWindow? window4 = _window;
		window4.OnDialogConfirmed = (Action<SecurityStatus, string>)Delegate.Combine(window4.OnDialogConfirmed, (Action<SecurityStatus, string>)delegate(SecurityStatus status, string reason)
		{
			((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)new CriminalRecordChangeStatus(status, reason));
		});
		CriminalRecordsConsoleWindow? window5 = _window;
		window5.OnStatusFilterPressed = (Action<SecurityStatus>)Delegate.Combine(window5.OnStatusFilterPressed, (Action<SecurityStatus>)delegate(SecurityStatus statusFilter)
		{
			((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)new CriminalRecordSetStatusFilter(statusFilter));
		});
		CriminalRecordsConsoleWindow? window6 = _window;
		window6.OnHistoryUpdated = (Action<CriminalRecord, bool, bool>)Delegate.Combine(window6.OnHistoryUpdated, new Action<CriminalRecord, bool, bool>(UpdateHistory));
		CriminalRecordsConsoleWindow? window7 = _window;
		window7.OnHistoryClosed = (Action)Delegate.Combine(window7.OnHistoryClosed, (Action)delegate
		{
			CrimeHistoryWindow? historyWindow3 = _historyWindow;
			if (historyWindow3 != null)
			{
				((BaseWindow)historyWindow3).Close();
			}
		});
		((BaseWindow)_window).OnClose += base.Close;
		_historyWindow = new CrimeHistoryWindow(component.MaxStringLength);
		CrimeHistoryWindow? historyWindow = _historyWindow;
		historyWindow.OnAddHistory = (Action<string>)Delegate.Combine(historyWindow.OnAddHistory, (Action<string>)delegate(string line)
		{
			((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)new CriminalRecordAddHistory(line));
		});
		CrimeHistoryWindow? historyWindow2 = _historyWindow;
		historyWindow2.OnDeleteHistory = (Action<uint>)Delegate.Combine(historyWindow2.OnDeleteHistory, (Action<uint>)delegate(uint index)
		{
			((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)new CriminalRecordDeleteHistory(index));
		});
		((BaseWindow)_historyWindow).Close();
	}

	private void UpdateHistory(CriminalRecord record, bool access, bool open)
	{
		_historyWindow.UpdateHistory(record, access);
		if (open)
		{
			((BaseWindow)_historyWindow).OpenCentered();
		}
	}

	protected override void UpdateState(BoundUserInterfaceState state)
	{
		((BoundUserInterface)this).UpdateState(state);
		if (state is CriminalRecordsConsoleState state2)
		{
			_window?.UpdateState(state2);
		}
	}

	protected override void Dispose(bool disposing)
	{
		((BoundUserInterface)this).Dispose(disposing);
		CriminalRecordsConsoleWindow? window = _window;
		if (window != null)
		{
			((BaseWindow)window).Close();
		}
		CrimeHistoryWindow? historyWindow = _historyWindow;
		if (historyWindow != null)
		{
			((BaseWindow)historyWindow).Close();
		}
	}
}
