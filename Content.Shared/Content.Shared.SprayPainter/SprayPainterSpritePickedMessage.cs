using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.SprayPainter;

[Serializable]
[NetSerializable]
public sealed class SprayPainterSpritePickedMessage : BoundUserInterfaceMessage
{
	public readonly int Index;

	public SprayPainterSpritePickedMessage(int index)
	{
		Index = index;
	}
}
