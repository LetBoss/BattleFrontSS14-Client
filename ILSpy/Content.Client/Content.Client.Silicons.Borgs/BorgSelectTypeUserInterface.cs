using System;
using Content.Shared.Silicons.Borgs;
using Content.Shared.Silicons.Borgs.Components;
using Robust.Client.UserInterface;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.ViewVariables;

namespace Content.Client.Silicons.Borgs;

public sealed class BorgSelectTypeUserInterface : BoundUserInterface
{
	[ViewVariables]
	private BorgSelectTypeMenu? _menu;

	public BorgSelectTypeUserInterface(EntityUid owner, Enum uiKey)
		: base(owner, uiKey)
	{
	}//IL_0001: Unknown result type (might be due to invalid IL or missing references)


	protected override void Open()
	{
		((BoundUserInterface)this).Open();
		_menu = BoundUserInterfaceExt.CreateWindow<BorgSelectTypeMenu>((BoundUserInterface)(object)this);
		_menu.ConfirmedBorgType += delegate(ProtoId<BorgTypePrototype> prototype)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			((BoundUserInterface)this).SendPredictedMessage((BoundUserInterfaceMessage)(object)new BorgSelectTypeMessage(prototype));
		};
	}
}
