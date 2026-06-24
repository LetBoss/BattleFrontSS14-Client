using System;
using Robust.Shared.Serialization;

namespace Content.Shared._CIV14merka.Commander;

[Serializable]
[NetSerializable]
public enum CivCommanderOrderType : byte
{
	None,
	Attack,
	Defense,
	Artillery
}
