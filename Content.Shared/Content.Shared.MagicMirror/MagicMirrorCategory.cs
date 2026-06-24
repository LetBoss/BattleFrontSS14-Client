using System;
using Robust.Shared.Serialization;

namespace Content.Shared.MagicMirror;

[Serializable]
[NetSerializable]
public enum MagicMirrorCategory : byte
{
	Hair,
	FacialHair
}
