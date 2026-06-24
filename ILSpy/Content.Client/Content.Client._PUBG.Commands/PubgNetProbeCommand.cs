using System;
using Content.Client._PUBG.NetProbe;
using Content.Shared.Administration;
using Robust.Client.Player;
using Robust.Shared.Console;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Player;

namespace Content.Client._PUBG.Commands;

[AnyCommand]
public sealed class PubgNetProbeCommand : LocalizedCommands
{
	[Dependency]
	private IEntitySystemManager _systems;

	[Dependency]
	private IPlayerManager _player;

	public override string Command => "pubgnetprobe";

	public override void Execute(IConsoleShell shell, string argStr, string[] args)
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Invalid comparison between Unknown and I4
		bool flag;
		if (args.Length == 2)
		{
			ICommonSession localSession = ((ISharedPlayerManager)_player).LocalSession;
			SessionStatus? val = ((localSession != null) ? new SessionStatus?(localSession.Status) : ((SessionStatus?)null));
			if (val.HasValue)
			{
				SessionStatus valueOrDefault = val.GetValueOrDefault();
				if (valueOrDefault - 2 <= 1)
				{
					flag = true;
					goto IL_0049;
				}
			}
			flag = false;
			goto IL_0049;
		}
		int result;
		if (args.Length != 1)
		{
			shell.WriteError(((LocalizedCommands)this).Loc.GetString("cmd-pubgnetprobe-usage"));
		}
		else if (!int.TryParse(args[0], out result))
		{
			shell.WriteError(((LocalizedCommands)this).Loc.GetString("cmd-pubgnetprobe-invalid-number", (ValueTuple<string, object>)("value", args[0])));
		}
		else if (result <= 0)
		{
			shell.WriteError(((LocalizedCommands)this).Loc.GetString("cmd-pubgnetprobe-not-positive"));
		}
		else if (result > 3072)
		{
			shell.WriteError(((LocalizedCommands)this).Loc.GetString("cmd-pubgnetprobe-cap", (ValueTuple<string, object>)("limitKb", 3072)));
		}
		else
		{
			_systems.GetEntitySystem<PubgNetProbeSystem>().StartLocal(result);
		}
		return;
		IL_0049:
		if (!flag)
		{
			shell.WriteError(((LocalizedCommands)this).Loc.GetString("cmd-pubgnetprobe-not-connected"));
		}
		else
		{
			shell.RemoteExecuteCommand(argStr);
		}
	}
}
