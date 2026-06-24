using System;
using Robust.Shared.Serialization;

namespace Content.Shared._RMC14.Marines.Dogtags;

[Serializable]
[NetSerializable]
public struct InfoTagInfo
{
	public string Name;

	public string Assignment;

	public string BloodType;
}
