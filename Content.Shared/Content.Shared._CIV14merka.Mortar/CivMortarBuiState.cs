using System;
using System.Collections.Generic;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;

namespace Content.Shared._CIV14merka.Mortar;

[Serializable]
[NetSerializable]
public sealed class CivMortarBuiState(Vector2i target, Vector2i dial, List<CivMortarQueuedRequest> requests) : BoundUserInterfaceState
{
	public Vector2i Target = target;

	public Vector2i Dial = dial;

	public List<CivMortarQueuedRequest> Requests = requests;
}
