using System;
using System.Numerics;
using Content.Client._RMC14.Chemistry.Master;
using Content.Shared._RMC14.Chemistry.ChemMaster;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.GameObjects;
using Robust.Shared.Localization;
using Robust.Shared.Utility;

namespace Content.Client._RMC14.Labeler;

public sealed class RMCHandLabelerPillBottleColorBui : BoundUserInterface
{
	private RMCChemMasterPopupWindow? _window;

	public RMCHandLabelerPillBottleColorBui(EntityUid owner, Enum uiKey)
		: base(owner, uiKey)
	{
	}//IL_0001: Unknown result type (might be due to invalid IL or missing references)


	protected override void Open()
	{
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Expected O, but got Unknown
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Expected O, but got Unknown
		((BoundUserInterface)this).Open();
		RMCChemMasterPopupWindow? window = _window;
		if (window != null)
		{
			((BaseWindow)window).Close();
		}
		SpriteSystem val = base.EntMan.System<SpriteSystem>();
		RMCChemMasterPopupWindow rMCChemMasterPopupWindow = new RMCChemMasterPopupWindow();
		((DefaultWindow)rMCChemMasterPopupWindow).Title = Loc.GetString("rmc-hand-labeler-pill-bottle-color");
		_window = rMCChemMasterPopupWindow;
		((BaseWindow)_window).OnClose += delegate
		{
			_window = null;
		};
		((BaseWindow)_window).OpenCentered();
		ResPath val2 = default(ResPath);
		((ResPath)(ref val2))._002Ector("_RMC14/Objects/Chemistry/pill_canister.rsi");
		RMCPillBottleColors[] values = Enum.GetValues<RMCPillBottleColors>();
		int num = values.Length - 1;
		for (int num2 = 0; num2 < num; num2++)
		{
			State state = val.GetState(new Rsi(val2, $"pill_canister{num2}"));
			TextureButton val3 = new TextureButton
			{
				TextureNormal = state.Frame0,
				Scale = new Vector2(2f, 2f)
			};
			RMCPillBottleColors color = values[num2];
			((BaseButton)val3).OnPressed += delegate
			{
				((BoundUserInterface)this).SendPredictedMessage((BoundUserInterfaceMessage)(object)new RMCChemMasterPillBottleColorMsg(color));
				RMCChemMasterPopupWindow? window2 = _window;
				if (window2 != null)
				{
					((BaseWindow)window2).Close();
				}
			};
			((Control)_window.Grid).AddChild((Control)(object)val3);
		}
	}

	protected override void Dispose(bool disposing)
	{
		((BoundUserInterface)this).Dispose(disposing);
		if (disposing)
		{
			RMCChemMasterPopupWindow? window = _window;
			if (window != null)
			{
				((BaseWindow)window).Close();
			}
		}
	}
}
