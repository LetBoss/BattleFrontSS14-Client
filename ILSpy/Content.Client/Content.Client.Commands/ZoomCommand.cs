using System;
using System.Numerics;
using Content.Client.Movement.Systems;
using Content.Shared.Movement.Components;
using Robust.Client.Graphics;
using Robust.Client.Player;
using Robust.Shared.Console;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Player;

namespace Content.Client.Commands;

public sealed class ZoomCommand : LocalizedCommands
{
	[Dependency]
	private IEntityManager _entityManager;

	[Dependency]
	private IEyeManager _eyeManager;

	[Dependency]
	private IPlayerManager _playerManager;

	public override string Command => "zoom";

	public override void Execute(IConsoleShell shell, string argStr, string[] args)
	{
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		int num = args.Length;
		float result;
		if ((uint)(num - 1) > 2u)
		{
			shell.WriteLine(((LocalizedCommands)this).Help);
		}
		else if (!float.TryParse(args[0], out result))
		{
			shell.WriteError(base.LocalizationManager.GetString("cmd-parse-failure-float", (ValueTuple<string, object>)("arg", args[0])));
		}
		else if (result > 0f)
		{
			Vector2 zoom = new Vector2(result, result);
			if (args.Length == 2)
			{
				if (!float.TryParse(args[1], out var result2))
				{
					shell.WriteError(base.LocalizationManager.GetString("cmd-parse-failure-float", (ValueTuple<string, object>)("arg", args[1])));
					return;
				}
				if (!(result2 > 0f))
				{
					shell.WriteError(base.LocalizationManager.GetString("cmd-" + ((LocalizedCommands)this).Command + "-error"));
					return;
				}
				zoom.Y = result2;
			}
			bool result3 = true;
			if (args.Length == 3 && !bool.TryParse(args[2], out result3))
			{
				shell.WriteError(base.LocalizationManager.GetString("cmd-parse-failure-bool", (ValueTuple<string, object>)("arg", args[2])));
				return;
			}
			ICommonSession localSession = ((ISharedPlayerManager)_playerManager).LocalSession;
			EntityUid? val = ((localSession != null) ? localSession.AttachedEntity : ((EntityUid?)null));
			ContentEyeComponent content = default(ContentEyeComponent);
			if (_entityManager.TryGetComponent<ContentEyeComponent>(val, ref content))
			{
				_entityManager.System<ContentEyeSystem>().RequestZoom(val.Value, zoom, ignoreLimit: true, result3, content);
			}
			else
			{
				_eyeManager.CurrentEye.Zoom = zoom;
			}
		}
		else
		{
			shell.WriteError(base.LocalizationManager.GetString("cmd-" + ((LocalizedCommands)this).Command + "-error"));
		}
	}
}
