using System;
using Robust.Shared.Serialization;

namespace Content.Shared._CIV14merka.Mortar;

[Serializable]
[NetSerializable]
public sealed class CivMortarQueuedRequest(int requestId, int x, int y)
{
	public int RequestId = requestId;

	public int X = x;

	public int Y = y;
}
