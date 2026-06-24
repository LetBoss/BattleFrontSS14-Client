using System;
using Robust.Client.Console;

namespace Content.Client.Replay;

public sealed class ReplayConGroup : IClientConGroupImplementation
{
	public event Action? ConGroupUpdated
	{
		add
		{
		}
		remove
		{
		}
	}

	public bool CanAdminMenu()
	{
		return true;
	}

	public bool CanAdminPlace()
	{
		return true;
	}

	public bool CanCommand(string cmdName)
	{
		return true;
	}

	public bool CanScript()
	{
		return true;
	}

	public bool CanViewVar()
	{
		return true;
	}
}
