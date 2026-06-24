using System;
using System.Collections.Generic;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Mech;

[Serializable]
[NetSerializable]
public sealed class MechGrabberUiState : BoundUserInterfaceState
{
	public List<NetEntity> Contents = new List<NetEntity>();

	public int MaxContents;
}
