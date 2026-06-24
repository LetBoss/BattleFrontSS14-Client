using System;
using Content.Shared.CCVar;
using Content.Shared.Chat;
using Content.Shared.NukeOps;
using Robust.Client.UserInterface;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.ViewVariables;

namespace Content.Client.NukeOps;

public sealed class WarDeclaratorBoundUserInterface : BoundUserInterface
{
	[Dependency]
	private IConfigurationManager _cfg;

	[ViewVariables]
	private WarDeclaratorWindow? _window;

	public WarDeclaratorBoundUserInterface(EntityUid owner, Enum uiKey)
		: base(owner, uiKey)
	{
	}//IL_0001: Unknown result type (might be due to invalid IL or missing references)


	protected override void Open()
	{
		((BoundUserInterface)this).Open();
		_window = BoundUserInterfaceExt.CreateWindow<WarDeclaratorWindow>((BoundUserInterface)(object)this);
		_window.OnActivated += OnWarDeclaratorActivated;
	}

	protected override void UpdateState(BoundUserInterfaceState state)
	{
		((BoundUserInterface)this).UpdateState(state);
		if (_window != null && state is WarDeclaratorBoundUserInterfaceState state2)
		{
			_window?.UpdateState(state2);
		}
	}

	private void OnWarDeclaratorActivated(string message)
	{
		int cVar = _cfg.GetCVar<int>(CCVars.ChatMaxAnnouncementLength);
		string msg = SharedChatSystem.SanitizeAnnouncement(message, cVar);
		((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)new WarDeclaratorActivateMessage(msg));
	}
}
