using System;
using Content.Client.Administration.Managers;
using Content.Shared.Administration;
using Robust.Shared.Console;
using Robust.Shared.IoC;

namespace Content.Client.NodeContainer;

public sealed class NodeVisCommand : LocalizedEntityCommands
{
	[Dependency]
	private IClientAdminManager _adminManager;

	[Dependency]
	private NodeGroupSystem _nodeSystem;

	public override string Command => "nodevis";

	public override void Execute(IConsoleShell shell, string argStr, string[] args)
	{
		if (!_adminManager.HasFlag(AdminFlags.Debug))
		{
			shell.WriteError(((LocalizedCommands)this).Loc.GetString("shell-missing-required-permission", (ValueTuple<string, object>)("perm", "+DEBUG")));
		}
		else
		{
			_nodeSystem.SetVisEnabled(!_nodeSystem.VisEnabled);
		}
	}
}
