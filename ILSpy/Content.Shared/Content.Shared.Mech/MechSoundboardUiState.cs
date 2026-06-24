using System;
using System.Collections.Generic;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Mech;

[Serializable]
[NetSerializable]
public sealed class MechSoundboardUiState : BoundUserInterfaceState
{
	public List<string> Sounds = new List<string>();
}
