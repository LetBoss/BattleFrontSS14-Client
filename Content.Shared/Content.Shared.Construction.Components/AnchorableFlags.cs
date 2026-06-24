using System;

namespace Content.Shared.Construction.Components;

[Flags]
public enum AnchorableFlags : byte
{
	None = 0,
	Anchorable = 1,
	Unanchorable = 2
}
