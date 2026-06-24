using System;
using Content.Shared._CIV14merka.Rangefinder;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;

namespace Content.Client._CIV14merka.Rangefinder;

public sealed class CivRangefinderBui : BoundUserInterface
{
	private CivRangefinderWindow? _window;

	public CivRangefinderBui(EntityUid owner, Enum uiKey)
		: base(owner, uiKey)
	{
	}//IL_0001: Unknown result type (might be due to invalid IL or missing references)


	protected override void Open()
	{
		((BoundUserInterface)this).Open();
		_window = BoundUserInterfaceExt.CreateWindow<CivRangefinderWindow>((BoundUserInterface)(object)this);
		((BaseButton)_window.ShareButton).OnPressed += delegate
		{
			((BoundUserInterface)this).SendPredictedMessage((BoundUserInterfaceMessage)(object)new CivRangefinderShareCoordinatesBuiMsg());
		};
		Refresh();
	}

	public void Refresh()
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		CivRangefinderWindow window = _window;
		if (window == null || !((BaseWindow)window).IsOpen)
		{
			return;
		}
		CivRangefinderComponent civRangefinderComponent = default(CivRangefinderComponent);
		if (base.EntMan.TryGetComponent<CivRangefinderComponent>(((BoundUserInterface)this).Owner, ref civRangefinderComponent))
		{
			Vector2i? lastTarget = civRangefinderComponent.LastTarget;
			if (lastTarget.HasValue)
			{
				Vector2i valueOrDefault = lastTarget.GetValueOrDefault();
				_window.Longitude.Text = $"X: {valueOrDefault.X}";
				_window.Latitude.Text = $"Y: {valueOrDefault.Y}";
				((BaseButton)_window.ShareButton).Disabled = false;
				return;
			}
		}
		_window.Longitude.Text = "X: -";
		_window.Latitude.Text = "Y: -";
		((BaseButton)_window.ShareButton).Disabled = true;
	}
}
