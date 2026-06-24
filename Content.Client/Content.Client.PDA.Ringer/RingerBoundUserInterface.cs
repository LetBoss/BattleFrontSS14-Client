using System;
using System.Threading;
using Content.Shared.PDA;
using Content.Shared.PDA.Ringer;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.GameObjects;
using Robust.Shared.Timing;
using Robust.Shared.ViewVariables;

namespace Content.Client.PDA.Ringer;

public sealed class RingerBoundUserInterface : BoundUserInterface
{
	[ViewVariables]
	private RingtoneMenu? _menu;

	public RingerBoundUserInterface(EntityUid owner, Enum uiKey)
		: base(owner, uiKey)
	{
	}//IL_0001: Unknown result type (might be due to invalid IL or missing references)


	protected override void Open()
	{
		((BoundUserInterface)this).Open();
		_menu = BoundUserInterfaceExt.CreateWindow<RingtoneMenu>((BoundUserInterface)(object)this);
		((BaseWindow)_menu).OpenToLeft();
		_menu.TestRingtoneButtonPressed += OnTestRingtoneButtonPressed;
		_menu.SetRingtoneButtonPressed += OnSetRingtoneButtonPressed;
		((BoundUserInterface)this).Update();
	}

	private bool TryGetRingtone(out Note[] ringtone)
	{
		if (_menu == null)
		{
			ringtone = Array.Empty<Note>();
			return false;
		}
		ringtone = new Note[_menu.RingerNoteInputs.Length];
		for (int i = 0; i < _menu.RingerNoteInputs.Length; i++)
		{
			if (!Enum.TryParse<Note>(_menu.RingerNoteInputs[i].Text.Replace("#", "sharp"), ignoreCase: false, out var result))
			{
				return false;
			}
			ringtone[i] = result;
		}
		return true;
	}

	public override void Update()
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		((BoundUserInterface)this).Update();
		RingerComponent ringerComponent = default(RingerComponent);
		if (_menu == null || !base.EntMan.TryGetComponent<RingerComponent>(((BoundUserInterface)this).Owner, ref ringerComponent))
		{
			return;
		}
		for (int i = 0; i < _menu.RingerNoteInputs.Length; i++)
		{
			string text = ringerComponent.Ringtone[i].ToString();
			if (RingtoneMenu.IsNote(text))
			{
				_menu.PreviousNoteInputs[i] = text.Replace("sharp", "#");
				_menu.RingerNoteInputs[i].Text = _menu.PreviousNoteInputs[i];
			}
		}
		((BaseButton)_menu.TestRingerButton).Disabled = ringerComponent.Active;
	}

	private void OnTestRingtoneButtonPressed()
	{
		if (_menu != null)
		{
			((BoundUserInterface)this).SendPredictedMessage((BoundUserInterfaceMessage)(object)new RingerPlayRingtoneMessage());
			((BaseButton)_menu.TestRingerButton).Disabled = true;
		}
	}

	private void OnSetRingtoneButtonPressed()
	{
		if (_menu == null || !TryGetRingtone(out Note[] ringtone))
		{
			return;
		}
		((BoundUserInterface)this).SendPredictedMessage((BoundUserInterfaceMessage)(object)new RingerSetRingtoneMessage(ringtone));
		((BaseButton)_menu.SetRingerButton).Disabled = true;
		Timer.Spawn(333, (Action)delegate
		{
			RingtoneMenu menu = _menu;
			if (menu != null && !((Control)menu).Disposed)
			{
				Button setRingerButton = menu.SetRingerButton;
				if (setRingerButton != null && !((Control)setRingerButton).Disposed)
				{
					((BaseButton)setRingerButton).Disabled = false;
				}
			}
		}, default(CancellationToken));
	}
}
