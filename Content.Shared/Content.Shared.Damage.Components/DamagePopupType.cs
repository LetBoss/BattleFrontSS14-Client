using System;
using Robust.Shared.Serialization;

namespace Content.Shared.Damage.Components;

[Serializable]
[NetSerializable]
public enum DamagePopupType : byte
{
	Combined,
	Total,
	Delta,
	Hit
}
