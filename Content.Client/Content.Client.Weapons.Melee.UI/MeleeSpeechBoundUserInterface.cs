using System;
using Content.Shared.Speech.Components;
using Robust.Client.UserInterface;
using Robust.Shared.GameObjects;
using Robust.Shared.ViewVariables;

namespace Content.Client.Weapons.Melee.UI;

public sealed class MeleeSpeechBoundUserInterface : BoundUserInterface
{
	[ViewVariables]
	private MeleeSpeechWindow? _window;

	public MeleeSpeechBoundUserInterface(EntityUid owner, Enum uiKey)
		: base(owner, uiKey)
	{
	}//IL_0001: Unknown result type (might be due to invalid IL or missing references)


	protected override void Open()
	{
		((BoundUserInterface)this).Open();
		_window = BoundUserInterfaceExt.CreateWindow<MeleeSpeechWindow>((BoundUserInterface)(object)this);
		_window.OnBattlecryEntered += OnBattlecryChanged;
	}

	private void OnBattlecryChanged(string newBattlecry)
	{
		((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)new MeleeSpeechBattlecryChangedMessage(newBattlecry));
	}

	protected override void UpdateState(BoundUserInterfaceState state)
	{
		((BoundUserInterface)this).UpdateState(state);
		if (_window != null && state is MeleeSpeechBoundUserInterfaceState meleeSpeechBoundUserInterfaceState)
		{
			_window.SetCurrentBattlecry(meleeSpeechBoundUserInterfaceState.CurrentBattlecry);
		}
	}

	protected override void Dispose(bool disposing)
	{
		((BoundUserInterface)this).Dispose(disposing);
		if (disposing)
		{
			MeleeSpeechWindow? window = _window;
			if (window != null)
			{
				((Control)window).Orphan();
			}
		}
	}
}
