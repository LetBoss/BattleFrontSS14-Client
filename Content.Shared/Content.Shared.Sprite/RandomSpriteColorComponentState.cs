using System;
using System.Collections.Generic;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;

namespace Content.Shared.Sprite;

[Serializable]
[NetSerializable]
public sealed class RandomSpriteColorComponentState : ComponentState
{
	public Dictionary<string, (string State, Color? Color)> Selected;
}
