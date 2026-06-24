using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;

namespace Content.Shared._RMC14.Deploy;

[Serializable]
[NetSerializable]
public sealed class RMCShowDeployAreaEvent(Box2 box, Color color) : EntityEventArgs
{
	public Box2 Box = box;

	public Color Color = color;
}
