using System;
using Robust.Shared.Serialization;

namespace Content.Shared.Emag.Systems;

[Serializable]
[Flags]
[NetSerializable]
public enum EmagType
{
	None = 0,
	All = -1,
	Interaction = 2,
	Access = 4
}
