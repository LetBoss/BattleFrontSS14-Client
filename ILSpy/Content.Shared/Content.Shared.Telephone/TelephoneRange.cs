using System;
using Robust.Shared.Serialization;

namespace Content.Shared.Telephone;

[Serializable]
[NetSerializable]
public enum TelephoneRange : byte
{
	Grid,
	Map,
	Unlimited
}
