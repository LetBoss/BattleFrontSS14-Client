using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Fax;

[Serializable]
[NetSerializable]
public sealed class FaxDestinationMessage : BoundUserInterfaceMessage
{
	public string Address { get; }

	public FaxDestinationMessage(string address)
	{
		Address = address;
	}
}
