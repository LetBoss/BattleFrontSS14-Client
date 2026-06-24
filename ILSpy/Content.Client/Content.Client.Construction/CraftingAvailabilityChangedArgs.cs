using System;

namespace Content.Client.Construction;

public sealed class CraftingAvailabilityChangedArgs : EventArgs
{
	public bool Available { get; }

	public CraftingAvailabilityChangedArgs(bool available)
	{
		Available = available;
	}
}
