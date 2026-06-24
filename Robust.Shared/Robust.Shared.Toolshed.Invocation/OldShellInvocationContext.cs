using System.Collections.Generic;
using System.Linq;
using Robust.Shared.Console;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Network;
using Robust.Shared.Player;
using Robust.Shared.Toolshed.Errors;
using Robust.Shared.Utility;

namespace Robust.Shared.Toolshed.Invocation;

internal sealed class OldShellInvocationContext : IInvocationContext
{
	private readonly List<IConError> _errors = new List<IConError>();

	public IConsoleShell? Shell;

	[field: Dependency]
	public ToolshedManager Toolshed { get; }

	public ToolshedEnvironment Environment => Toolshed.DefaultEnvironment;

	public NetUserId? User { get; }

	public ICommonSession? Session => Shell?.Player;

	public bool HasErrors => _errors.Count > 0;

	public Dictionary<string, object?> Variables { get; } = new Dictionary<string, object>();

	public OldShellInvocationContext(IConsoleShell shell)
	{
		IoCManager.InjectDependencies(this);
		Shell = shell;
		User = Session?.UserId;
	}

	public void WriteLine(string line)
	{
		Shell?.WriteLine(line);
	}

	public void WriteLine(FormattedMessage line)
	{
		Shell?.WriteLine(line);
	}

	public void ReportError(IConError err)
	{
		_errors.Add(err);
	}

	public IEnumerable<IConError> GetErrors()
	{
		return _errors;
	}

	public void ClearErrors()
	{
		_errors.Clear();
	}

	public object? ReadVar(string name)
	{
		if (name == "self")
		{
			EntityUid? entityUid = Session?.AttachedEntity;
			if (entityUid.HasValue)
			{
				EntityUid valueOrDefault = entityUid.GetValueOrDefault();
				return valueOrDefault;
			}
		}
		return Variables.GetValueOrDefault(name);
	}

	public void WriteVar(string name, object? value)
	{
		if (name == "self")
		{
			ReportError(new ReadonlyVariableError("self"));
		}
		else
		{
			Variables[name] = value;
		}
	}

	public bool IsReadonlyVar(string name)
	{
		return name == "self";
	}

	public IEnumerable<string> GetVars()
	{
		ICommonSession? session = Session;
		if (session == null || !session.AttachedEntity.HasValue)
		{
			return Variables.Keys;
		}
		return Variables.Keys.Append("self");
	}
}
