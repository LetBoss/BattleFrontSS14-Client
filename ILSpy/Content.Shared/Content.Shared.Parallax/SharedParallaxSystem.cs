using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Parallax;

public abstract class SharedParallaxSystem : EntitySystem
{
	[Serializable]
	[NetSerializable]
	protected sealed class ParallaxComponentState : ComponentState
	{
		public string Parallax = string.Empty;
	}
}
