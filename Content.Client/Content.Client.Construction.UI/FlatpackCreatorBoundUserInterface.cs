using System;
using Content.Shared.Construction.Components;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.GameObjects;
using Robust.Shared.ViewVariables;

namespace Content.Client.Construction.UI;

public sealed class FlatpackCreatorBoundUserInterface : BoundUserInterface
{
	[ViewVariables]
	private FlatpackCreatorMenu? _menu;

	public FlatpackCreatorBoundUserInterface(EntityUid owner, Enum uiKey)
		: base(owner, uiKey)
	{
	}//IL_0001: Unknown result type (might be due to invalid IL or missing references)


	protected override void Open()
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		((BoundUserInterface)this).Open();
		_menu = BoundUserInterfaceExt.CreateWindow<FlatpackCreatorMenu>((BoundUserInterface)(object)this);
		_menu.SetEntity(((BoundUserInterface)this).Owner);
		_menu.PackButtonPressed += delegate
		{
			((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)new FlatpackCreatorStartPackBuiMessage());
		};
		((BaseWindow)_menu).OpenCentered();
	}
}
