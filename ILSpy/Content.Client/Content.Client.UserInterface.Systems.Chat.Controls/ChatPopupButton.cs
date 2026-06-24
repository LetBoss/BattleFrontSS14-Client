using System.Numerics;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Timing;

namespace Content.Client.UserInterface.Systems.Chat.Controls;

public abstract class ChatPopupButton<TPopup> : Button where TPopup : Popup, new()
{
	private readonly IGameTiming _gameTiming;

	public readonly TPopup Popup;

	private uint _frameLastPopupChanged;

	protected ChatPopupButton()
	{
		_gameTiming = IoCManager.Resolve<IGameTiming>();
		((BaseButton)this).ToggleMode = true;
		((BaseButton)this).OnToggled += OnButtonToggled;
		Popup = ((Control)this).UserInterfaceManager.CreatePopup<TPopup>();
		((Control)(object)Popup).OnVisibilityChanged += OnPopupVisibilityChanged;
	}

	protected override void KeyBindDown(GUIBoundKeyEventArgs args)
	{
		if (_frameLastPopupChanged != _gameTiming.CurFrame)
		{
			((BaseButton)this).KeyBindDown(args);
		}
	}

	protected abstract UIBox2 GetPopupPosition();

	private void OnButtonToggled(ButtonToggledEventArgs args)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		if (args.Pressed)
		{
			((Popup)Popup).Open((UIBox2?)GetPopupPosition(), (Vector2?)null, (Vector2?)null);
		}
		else
		{
			((Popup)Popup).Close();
		}
	}

	private void OnPopupVisibilityChanged(Control control)
	{
		((BaseButton)this).Pressed = control.Visible;
		_frameLastPopupChanged = _gameTiming.CurFrame;
	}
}
