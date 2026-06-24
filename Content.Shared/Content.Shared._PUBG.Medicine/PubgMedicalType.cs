using System;
using Robust.Shared.Serialization;

namespace Content.Shared._PUBG.Medicine;

[Serializable]
[NetSerializable]
public enum PubgMedicalType : byte
{
	Bandage,
	Medkit,
	FullMedkit
}
