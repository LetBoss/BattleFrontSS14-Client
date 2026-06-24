using System;
using System.Collections.Generic;
using Content.Shared.Implants;
using Robust.Client.UserInterface;
using Robust.Shared.GameObjects;
using Robust.Shared.ViewVariables;

namespace Content.Client.Implants.UI;

public sealed class DeimplantBoundUserInterface : BoundUserInterface
{
	[ViewVariables]
	private DeimplantChoiceWindow? _window;

	public DeimplantBoundUserInterface(EntityUid owner, Enum uiKey)
		: base(owner, uiKey)
	{
	}//IL_0001: Unknown result type (might be due to invalid IL or missing references)


	protected override void Open()
	{
		((BoundUserInterface)this).Open();
		_window = BoundUserInterfaceExt.CreateWindow<DeimplantChoiceWindow>((BoundUserInterface)(object)this);
		DeimplantChoiceWindow? window = _window;
		window.OnImplantChange = (Action<string>)Delegate.Combine(window.OnImplantChange, (Action<string>)delegate(string? implant)
		{
			((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)new DeimplantChangeVerbMessage(implant));
		});
	}

	public void UpdateState(Dictionary<string, string> implantList, string? implant)
	{
		if (_window != null)
		{
			_window.UpdateImplantList(implantList);
			_window.UpdateState(implant);
		}
	}
}
