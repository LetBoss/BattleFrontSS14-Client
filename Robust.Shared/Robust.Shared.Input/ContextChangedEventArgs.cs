using System;

namespace Robust.Shared.Input;

public sealed class ContextChangedEventArgs : EventArgs
{
	public IInputCmdContext? NewContext { get; }

	public IInputCmdContext? OldContext { get; }

	public ContextChangedEventArgs(IInputCmdContext? oldContext, IInputCmdContext? newContext)
	{
		OldContext = oldContext;
		NewContext = newContext;
	}
}
