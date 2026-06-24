using System;
using Robust.Shared.Serialization;

namespace Content.Shared._RMC14.Attachable;

[Serializable]
[NetSerializable]
public enum AttachmentUI : byte
{
	StripKey,
	ChooseSlotKey
}
