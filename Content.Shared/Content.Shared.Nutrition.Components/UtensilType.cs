using System;

namespace Content.Shared.Nutrition.Components;

[Flags]
public enum UtensilType : byte
{
	None = 0,
	Fork = 1,
	Spoon = 2,
	Knife = 4
}
