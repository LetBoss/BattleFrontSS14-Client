using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.SprayPainter;

[Serializable]
[NetSerializable]
public sealed class SprayPainterColorPickedMessage : BoundUserInterfaceMessage
{
	public readonly string? Key;

	public SprayPainterColorPickedMessage(string? key)
	{
		Key = key;
	}
}
