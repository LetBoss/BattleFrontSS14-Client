using System.Collections.Generic;
using Robust.Shared.Network;
using Robust.Shared.Player;
using Robust.Shared.Toolshed.Errors;
using Robust.Shared.Utility;

namespace Robust.Shared.Toolshed;

public interface IInvocationContext
{
	ToolshedEnvironment Environment { get; }

	ICommonSession? Session { get; }

	NetUserId? User { get; }

	ToolshedManager Toolshed { get; }

	bool HasErrors { get; }

	bool CheckInvokable(CommandSpec command, out IConError? error)
	{
		return Toolshed.CheckInvokable(command, Session, out error);
	}

	void WriteLine(string line);

	void WriteLine(FormattedMessage line)
	{
		if (Session == null)
		{
			WriteLine(line.ToString());
		}
		else
		{
			WriteLine(line.ToMarkup());
		}
	}

	void WriteMarkup(string markup)
	{
		WriteLine(FormattedMessage.FromMarkupPermissive(markup));
	}

	void WriteError(IConError error)
	{
		WriteLine(error.Describe());
	}

	void ReportError(IConError err);

	IEnumerable<IConError> GetErrors();

	void ClearErrors();

	object? ReadVar(string name);

	void WriteVar(string name, object? value);

	bool IsReadonlyVar(string name)
	{
		return false;
	}

	IEnumerable<string> GetVars();
}
