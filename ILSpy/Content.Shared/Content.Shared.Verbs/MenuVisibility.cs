using System;

namespace Content.Shared.Verbs;

[Flags]
public enum MenuVisibility
{
	Default = 0,
	NoFov = 1,
	InContainer = 2,
	Invisible = 4,
	All = 7
}
