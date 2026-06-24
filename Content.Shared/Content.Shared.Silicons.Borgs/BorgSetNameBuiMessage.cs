using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Silicons.Borgs;

[Serializable]
[NetSerializable]
public sealed class BorgSetNameBuiMessage : BoundUserInterfaceMessage
{
	public string Name;

	public BorgSetNameBuiMessage(string name)
	{
		Name = name;
	}
}
