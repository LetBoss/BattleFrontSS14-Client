using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Eye;

[Flags]
[FlagsFor(typeof(VisibilityMaskLayer))]
public enum VisibilityFlags
{
	None = 0,
	Normal = 1,
	Ghost = 2,
	Subfloor = 4,
	Commander = 8,
	Xeno = 0x8000
}
