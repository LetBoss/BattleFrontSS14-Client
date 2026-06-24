using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Atmos.Piping.Portable.Components;

[Serializable]
[NetSerializable]
public sealed class SpaceHeaterChangeModeMessage : BoundUserInterfaceMessage
{
	public SpaceHeaterMode Mode { get; }

	public SpaceHeaterChangeModeMessage(SpaceHeaterMode mode)
	{
		Mode = mode;
	}
}
