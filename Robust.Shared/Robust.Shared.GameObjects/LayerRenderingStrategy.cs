using System;
using Robust.Shared.Serialization;

namespace Robust.Shared.GameObjects;

[Serializable]
[NetSerializable]
public enum LayerRenderingStrategy
{
	Default,
	SnapToCardinals,
	NoRotation,
	UseSpriteStrategy
}
