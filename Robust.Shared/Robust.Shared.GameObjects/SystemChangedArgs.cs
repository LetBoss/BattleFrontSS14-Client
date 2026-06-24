using System;

namespace Robust.Shared.GameObjects;

public sealed class SystemChangedArgs : EventArgs
{
	public IEntitySystem System { get; }

	public SystemChangedArgs(IEntitySystem system)
	{
		System = system;
	}
}
