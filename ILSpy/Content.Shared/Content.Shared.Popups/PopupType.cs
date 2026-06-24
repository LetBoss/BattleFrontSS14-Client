using System;
using Robust.Shared.Serialization;

namespace Content.Shared.Popups;

[Serializable]
[NetSerializable]
public enum PopupType : byte
{
	Small,
	SmallCaution,
	Medium,
	MediumCaution,
	Large,
	LargeCaution,
	MediumXeno
}
