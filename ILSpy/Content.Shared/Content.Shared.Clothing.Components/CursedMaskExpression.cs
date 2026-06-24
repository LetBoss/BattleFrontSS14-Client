using System;
using Robust.Shared.Serialization;

namespace Content.Shared.Clothing.Components;

[Serializable]
[NetSerializable]
public enum CursedMaskExpression : byte
{
	Neutral,
	Joy,
	Despair,
	Anger
}
