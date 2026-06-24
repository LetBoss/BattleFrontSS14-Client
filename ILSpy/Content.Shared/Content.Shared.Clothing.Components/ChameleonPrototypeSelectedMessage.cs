using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Clothing.Components;

[Serializable]
[NetSerializable]
public sealed class ChameleonPrototypeSelectedMessage : BoundUserInterfaceMessage
{
	public readonly string SelectedId;

	public ChameleonPrototypeSelectedMessage(string selectedId)
	{
		SelectedId = selectedId;
	}
}
