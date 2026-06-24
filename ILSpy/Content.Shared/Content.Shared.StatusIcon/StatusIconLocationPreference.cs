using System;
using Robust.Shared.Serialization;

namespace Content.Shared.StatusIcon;

[Serializable]
[NetSerializable]
public enum StatusIconLocationPreference : byte
{
	None,
	Left,
	Right
}
