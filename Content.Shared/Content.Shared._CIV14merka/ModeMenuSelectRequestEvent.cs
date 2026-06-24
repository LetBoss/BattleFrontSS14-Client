using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._CIV14merka;

[Serializable]
[NetSerializable]
public sealed class ModeMenuSelectRequestEvent : EntityEventArgs
{
	public ModeMenuMode Mode { get; }

	public ModeMenuSelectRequestEvent(ModeMenuMode mode)
	{
		Mode = mode;
	}
}
