using System;
using Robust.Shared.Serialization;

namespace Content.Shared.Inventory;

[Serializable]
[NetSerializable]
[Flags]
public enum SlotFlags
{
	NONE = 0,
	PREVENTEQUIP = 1,
	HEAD = 2,
	EYES = 4,
	EARS = 8,
	MASK = 0x10,
	OUTERCLOTHING = 0x20,
	INNERCLOTHING = 0x40,
	NECK = 0x80,
	BACK = 0x100,
	BELT = 0x200,
	GLOVES = 0x400,
	IDCARD = 0x800,
	POCKET = 0x1000,
	LEGS = 0x2000,
	FEET = 0x4000,
	SUITSTORAGE = 0x8000,
	All = -1,
	WITHOUT_POCKET = -4097
}
