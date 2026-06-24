using System;
using Robust.Shared.Serialization;

namespace Content.Shared.ParcelWrap.Components;

[Serializable]
[NetSerializable]
public enum WrappedParcelVisuals : byte
{
	Size,
	Layer
}
