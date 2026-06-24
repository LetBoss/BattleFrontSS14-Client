using System;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Timing;
using Robust.Shared.ViewVariables;

namespace Content.Client.UserInterface.Controls;

public sealed class ConfirmButton : Button
{
	[Dependency]
	private IGameTiming _gameTiming;

	public const string ConfirmPrefix = "confirm-";

	private TimeSpan? _nextReset;

	private TimeSpan? _nextCooldown;

	private string? _confirmationText;

	private string? _text;

	[ViewVariables]
	public bool IsConfirming;

	public string? Text
	{
		get
		{
			return _text;
		}
		set
		{
			_text = value;
			((Button)this).Text = (IsConfirming ? _confirmationText : value);
		}
	}

	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public string ConfirmationText
	{
		get
		{
			return _confirmationText ?? Loc.GetString("generic-confirm");
		}
		set
		{
			_confirmationText = value;
		}
	}

	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public TimeSpan ResetTime { get; set; } = TimeSpan.FromSeconds(2L);

	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public TimeSpan CooldownTime { get; set; } = TimeSpan.FromSeconds(0.5);

	public event Action<ButtonEventArgs>? OnPressed;

	public ConfirmButton()
	{
		IoCManager.InjectDependencies<ConfirmButton>(this);
		((BaseButton)this).OnPressed += HandleOnPressed;
	}

	protected override void FrameUpdate(FrameEventArgs args)
	{
		if (IsConfirming)
		{
			TimeSpan curTime = _gameTiming.CurTime;
			TimeSpan? nextReset = _nextReset;
			if (curTime > nextReset)
			{
				IsConfirming = false;
				((Button)this).Text = Text;
				((BaseButton)this).DrawModeChanged();
			}
		}
		if (((BaseButton)this).Disabled)
		{
			TimeSpan curTime = _gameTiming.CurTime;
			TimeSpan? nextReset = _nextCooldown;
			if (curTime > nextReset)
			{
				((BaseButton)this).Disabled = false;
			}
		}
	}

	protected override void DrawModeChanged()
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Expected I4, but got Unknown
		if (IsConfirming)
		{
			DrawModeEnum drawMode = ((BaseButton)this).DrawMode;
			switch ((int)drawMode)
			{
			case 0:
				((Control)this).SetOnlyStylePseudoClass("confirm-normal");
				break;
			case 1:
				((Control)this).SetOnlyStylePseudoClass("confirm-pressed");
				break;
			case 2:
				((Control)this).SetOnlyStylePseudoClass("confirm-hover");
				break;
			case 3:
				((Control)this).SetOnlyStylePseudoClass("confirm-disabled");
				break;
			default:
				throw new ArgumentOutOfRangeException();
			}
		}
		else
		{
			((ContainerButton)this).DrawModeChanged();
		}
	}

	private void HandleOnPressed(ButtonEventArgs buttonEvent)
	{
		if (!IsConfirming || !(_nextCooldown > _gameTiming.CurTime))
		{
			if (!IsConfirming)
			{
				_nextCooldown = _gameTiming.CurTime + CooldownTime;
				_nextReset = _gameTiming.CurTime + ResetTime;
				((BaseButton)this).Disabled = true;
			}
			else
			{
				this.OnPressed?.Invoke(buttonEvent);
			}
			((Button)this).Text = (IsConfirming ? Text : ConfirmationText);
			IsConfirming = !IsConfirming;
		}
	}
}
