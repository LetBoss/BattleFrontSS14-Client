using System;
using Content.Shared._PUBG.Boombox;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.GameObjects;
using Robust.Shared.ViewVariables;

namespace Content.Client._PUBG.Boombox.UI;

public sealed class PubgPhoneBoundUserInterface : BoundUserInterface
{
	[ViewVariables]
	private PubgPhoneWindow? _window;

	public PubgPhoneBoundUserInterface(EntityUid owner, Enum uiKey)
		: base(owner, uiKey)
	{
	}//IL_0001: Unknown result type (might be due to invalid IL or missing references)


	protected override void Open()
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		((BoundUserInterface)this).Open();
		_window = new PubgPhoneWindow(base.EntMan, ((BoundUserInterface)this).Owner);
		((BaseWindow)_window).OpenCentered();
		((BaseWindow)_window).OnClose += base.Close;
		_window.OnSyncPressed += delegate
		{
			((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)new PubgBoomboxSyncMessage());
		};
		_window.OnPlayPressed += delegate(string trackId)
		{
			((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)new PubgBoomboxPlayMessage(trackId));
		};
		_window.OnStopPressed += delegate
		{
			((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)new PubgBoomboxStopMessage());
		};
		_window.OnSeekReleased += delegate(float seconds)
		{
			((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)new PubgBoomboxSeekMessage(seconds));
		};
		_window.OnVolumeReleased += delegate(float volume)
		{
			((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)new PubgBoomboxVolumeMessage(volume));
		};
		_window.OnRangeReleased += delegate(float range)
		{
			((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)new PubgBoomboxRangeMessage(range));
		};
	}

	protected override void UpdateState(BoundUserInterfaceState state)
	{
		((BoundUserInterface)this).UpdateState(state);
		if (state is PubgBoomboxUiState state2)
		{
			_window?.UpdateLibrary(state2);
		}
	}

	protected override void Dispose(bool disposing)
	{
		((BoundUserInterface)this).Dispose(disposing);
		if (disposing)
		{
			PubgPhoneWindow? window = _window;
			if (window != null)
			{
				((Control)window).Dispose();
			}
			_window = null;
		}
	}
}
