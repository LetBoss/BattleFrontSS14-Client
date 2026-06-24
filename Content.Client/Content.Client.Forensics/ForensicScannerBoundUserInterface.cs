using System;
using System.Threading;
using Content.Shared.Forensics;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Timing;
using Robust.Shared.ViewVariables;

namespace Content.Client.Forensics;

public sealed class ForensicScannerBoundUserInterface : BoundUserInterface
{
	[Dependency]
	private IGameTiming _gameTiming;

	[ViewVariables]
	private ForensicScannerMenu? _window;

	[ViewVariables]
	private TimeSpan _printCooldown;

	public ForensicScannerBoundUserInterface(EntityUid owner, Enum uiKey)
		: base(owner, uiKey)
	{
	}//IL_0001: Unknown result type (might be due to invalid IL or missing references)


	protected override void Open()
	{
		((BoundUserInterface)this).Open();
		_window = BoundUserInterfaceExt.CreateWindow<ForensicScannerMenu>((BoundUserInterface)(object)this);
		((BaseButton)_window.Print).OnPressed += delegate
		{
			Print();
		};
		((BaseButton)_window.Clear).OnPressed += delegate
		{
			Clear();
		};
	}

	private void Print()
	{
		((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)new ForensicScannerPrintMessage());
		if (_window != null)
		{
			_window.UpdatePrinterState(disabled: true);
		}
		Timer.Spawn(_printCooldown, (Action)delegate
		{
			if (_window != null)
			{
				_window.UpdatePrinterState(disabled: false);
			}
		}, default(CancellationToken));
	}

	private void Clear()
	{
		((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)new ForensicScannerClearMessage());
	}

	protected override void UpdateState(BoundUserInterfaceState state)
	{
		((BoundUserInterface)this).UpdateState(state);
		if (_window == null || !(state is ForensicScannerBoundUserInterfaceState forensicScannerBoundUserInterfaceState))
		{
			return;
		}
		_printCooldown = forensicScannerBoundUserInterfaceState.PrintCooldown;
		if (forensicScannerBoundUserInterfaceState.PrintReadyAt > _gameTiming.CurTime)
		{
			Timer.Spawn(forensicScannerBoundUserInterfaceState.PrintReadyAt - _gameTiming.CurTime, (Action)delegate
			{
				if (_window != null)
				{
					_window.UpdatePrinterState(disabled: false);
				}
			}, default(CancellationToken));
		}
		_window.UpdateState(forensicScannerBoundUserInterfaceState);
	}
}
