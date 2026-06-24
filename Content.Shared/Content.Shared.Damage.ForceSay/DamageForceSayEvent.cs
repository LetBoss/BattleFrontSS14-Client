using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Damage.ForceSay;

[Serializable]
[NetSerializable]
public sealed class DamageForceSayEvent : EntityEventArgs
{
	public string? Suffix;
}
