using System;
using Robust.Shared.Serialization;

namespace Content.Shared.Atmos.Components;

[Serializable]
[NetSerializable]
public enum AtmosPipeLayer
{
	Primary,
	Secondary,
	Tertiary
}
