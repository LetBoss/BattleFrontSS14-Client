using System;
using Robust.Shared.Serialization;

namespace Content.Shared._RMC14.Anchor;

[Serializable]
[NetSerializable]
public enum DeployableItemPosition
{
	None,
	Lower,
	Upper
}
