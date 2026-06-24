using System;
using Robust.Shared.Serialization;

namespace Content.Shared.Telephone;

[Serializable]
[NetSerializable]
public enum TelephoneState : byte
{
	Idle,
	Calling,
	Ringing,
	InCall,
	EndingCall
}
