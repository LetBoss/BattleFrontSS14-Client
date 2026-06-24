using System.Collections.Generic;
using Robust.Shared.Network;
using Robust.Shared.Player;
using Robust.Shared.Toolshed.Errors;

namespace Robust.Shared.Toolshed;

public sealed class LocalVarInvocationContext(IInvocationContext inner) : IInvocationContext
{
	public Dictionary<string, object?> LocalVars = new Dictionary<string, object>();

	public HashSet<string>? ReadonlyVars;

	public ICommonSession? Session => inner.Session;

	public ToolshedManager Toolshed => inner.Toolshed;

	public NetUserId? User => inner.User;

	public ToolshedEnvironment Environment => inner.Environment;

	public bool HasErrors => inner.HasErrors;

	public bool CheckInvokable(CommandSpec command, out IConError? error)
	{
		return inner.CheckInvokable(command, out error);
	}

	public void WriteLine(string line)
	{
		inner.WriteLine(line);
	}

	public void ReportError(IConError err)
	{
		inner.ReportError(err);
	}

	public IEnumerable<IConError> GetErrors()
	{
		return inner.GetErrors();
	}

	public void ClearErrors()
	{
		inner.ClearErrors();
	}

	public void SetLocal(string name, object? value)
	{
		LocalVars[name] = value;
	}

	public void SetLocal(string name, object? value, bool @readonly)
	{
		LocalVars[name] = value;
		SetReadonly(name, @readonly);
	}

	public void SetReadonly(string name, bool @readonly)
	{
		if (@readonly)
		{
			if (ReadonlyVars == null)
			{
				ReadonlyVars = new HashSet<string>();
			}
			ReadonlyVars.Add(name);
		}
		else
		{
			ReadonlyVars?.Remove(name);
		}
	}

	public void ClearLocal(string name)
	{
		LocalVars.Remove(name);
		ReadonlyVars?.Remove(name);
	}

	public object? ReadVar(string name)
	{
		if (!LocalVars.TryGetValue(name, out object value))
		{
			return inner.ReadVar(name);
		}
		return value;
	}

	public void WriteVar(string name, object? value)
	{
		if (ReadonlyVars != null && ReadonlyVars.Contains(name))
		{
			ReportError(new ReadonlyVariableError(name));
		}
		else if (LocalVars.ContainsKey(name))
		{
			LocalVars[name] = value;
		}
		else
		{
			inner.WriteVar(name, value);
		}
	}

	public bool IsReadonlyVar(string name)
	{
		if (ReadonlyVars != null)
		{
			return ReadonlyVars.Contains(name);
		}
		return false;
	}

	public IEnumerable<string> GetVars()
	{
		foreach (string key in LocalVars.Keys)
		{
			yield return key;
		}
		foreach (string var in inner.GetVars())
		{
			if (!LocalVars.ContainsKey(var))
			{
				yield return var;
			}
		}
	}
}
