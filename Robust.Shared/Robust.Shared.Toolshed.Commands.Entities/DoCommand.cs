using System;
using System.Collections.Generic;
using System.Globalization;
using System.Numerics;
using Robust.Shared.Console;
using Robust.Shared.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Toolshed.Invocation;

namespace Robust.Shared.Toolshed.Commands.Entities;

[ToolshedCommand]
internal sealed class DoCommand : ToolshedCommand
{
	private SharedTransformSystem? _xformSys;

	[CommandImplementation(null)]
	[TakesPipedTypeAsGeneric]
	public IEnumerable<T> Do<T>(IInvocationContext ctx, [PipedArgument] IEnumerable<T> input, string command)
	{
		if (!(ctx is OldShellInvocationContext { Shell: not null } reqCtx))
		{
			throw new NotImplementedException("do can only be executed in a shell invocation context. Some commands like emplace provide their own context.");
		}
		if (_xformSys == null)
		{
			_xformSys = GetSys<SharedTransformSystem>();
		}
		EntityQuery<TransformComponent> xformQ = GetEntityQuery<TransformComponent>();
		IConsoleShell shell = reqCtx.Shell;
		foreach (T item in input)
		{
			string text = command;
			if (item is EntityUid uid)
			{
				Vector2 worldPosition = _xformSys.GetWorldPosition(uid, xformQ);
				EntityCoordinates coordinates = xformQ.GetComponent(uid).Coordinates;
				text = text.Replace("$ID", uid.ToString()).Replace("$PID", (reqCtx.Session?.AttachedEntity ?? EntityUid.Invalid).ToString()).Replace("$WX", worldPosition.X.ToString(CultureInfo.InvariantCulture))
					.Replace("$WY", worldPosition.Y.ToString(CultureInfo.InvariantCulture))
					.Replace("$LX", coordinates.X.ToString(CultureInfo.InvariantCulture))
					.Replace("$LY", coordinates.Y.ToString(CultureInfo.InvariantCulture));
			}
			text = text.Replace("$SELF", item.ToString() ?? "");
			shell.ExecuteCommand(text);
			yield return item;
		}
	}
}
