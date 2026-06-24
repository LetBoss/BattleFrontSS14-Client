using System;
using Robust.Shared.Serialization;

namespace Content.Shared.Telephone;

[Serializable]
[NetSerializable]
public struct TelephoneCallOptions
{
	public bool IgnoreRange;

	public bool ForceConnect;

	public bool ForceJoin;

	public bool MuteSource;

	public bool MuteReceiver;
}
