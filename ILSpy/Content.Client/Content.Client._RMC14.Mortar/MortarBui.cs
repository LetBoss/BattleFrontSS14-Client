using System;
using Content.Shared._RMC14.Mortar;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.GameObjects;
using Robust.Shared.Localization;
using Robust.Shared.Maths;

namespace Content.Client._RMC14.Mortar;

public sealed class MortarBui : BoundUserInterface
{
	private MortarWindow? _window;

	public MortarBui(EntityUid owner, Enum uiKey)
		: base(owner, uiKey)
	{
	}//IL_0001: Unknown result type (might be due to invalid IL or missing references)


	protected override void Open()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		((BoundUserInterface)this).Open();
		_window = BoundUserInterfaceExt.CreateWindow<MortarWindow>((BoundUserInterface)(object)this);
		Refresh();
		MortarComponent mortarComponent = default(MortarComponent);
		if (base.EntMan.TryGetComponent<MortarComponent>(((BoundUserInterface)this).Owner, ref mortarComponent))
		{
			SetSpinBox(_window.TargetX, mortarComponent.MaxTarget, mortarComponent.Target.X);
			SetSpinBox(_window.TargetY, mortarComponent.MaxTarget, mortarComponent.Target.Y);
			SetSpinBox(_window.DialX, mortarComponent.MaxDial, mortarComponent.Dial.X);
			SetSpinBox(_window.DialY, mortarComponent.MaxDial, mortarComponent.Dial.Y);
			((BaseButton)_window.SetTargetButton).OnPressed += delegate
			{
				//IL_0026: Unknown result type (might be due to invalid IL or missing references)
				((BoundUserInterface)this).SendPredictedMessage((BoundUserInterfaceMessage)(object)new MortarTargetBuiMsg(Vector2i.op_Implicit((Parse(_window.TargetX), Parse(_window.TargetY)))));
			};
			((BaseButton)_window.SetOffsetButton).OnPressed += delegate
			{
				//IL_0026: Unknown result type (might be due to invalid IL or missing references)
				((BoundUserInterface)this).SendPredictedMessage((BoundUserInterfaceMessage)(object)new MortarDialBuiMsg(Vector2i.op_Implicit((Parse(_window.DialX), Parse(_window.DialY)))));
			};
		}
		((BaseButton)_window.ViewCameraButton).OnPressed += delegate
		{
			((BoundUserInterface)this).SendPredictedMessage((BoundUserInterfaceMessage)(object)new MortarViewCamerasMsg());
		};
		static int Parse(FloatSpinBox spinBox)
		{
			return (int)spinBox.Value;
		}
		static void SetSpinBox(FloatSpinBox spinBox, int limit, int value)
		{
			spinBox.Value = value;
			spinBox.OnValueChanged += delegate(FloatSpinBoxEventArgs args)
			{
				float value2 = Math.Clamp(args.Value, -limit, limit);
				spinBox.Value = value2;
			};
		}
	}

	public void Refresh()
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		MortarWindow window = _window;
		MortarComponent mortarComponent = default(MortarComponent);
		if (window != null && ((BaseWindow)window).IsOpen && base.EntMan.TryGetComponent<MortarComponent>(((BoundUserInterface)this).Owner, ref mortarComponent))
		{
			SetValue(_window.TargetX, mortarComponent.Target.X);
			SetValue(_window.TargetY, mortarComponent.Target.Y);
			SetValue(_window.DialX, mortarComponent.Dial.X);
			SetValue(_window.DialY, mortarComponent.Dial.Y);
			_window.MaxDialLabel.Text = Loc.GetString("rmc-mortar-offset-max", new(string, object)[1] { ("max", mortarComponent.MaxDial) });
		}
		static void SetValue(FloatSpinBox? spinBox, int value)
		{
			if (spinBox != null)
			{
				spinBox.Value = value;
			}
		}
	}
}
