using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;

namespace Content.Shared.Cuffs.Components;

[Serializable]
[NetSerializable]
public sealed class CuffableComponentState : ComponentState
{
	public readonly bool CanStillInteract;

	public readonly int NumHandsCuffed;

	public readonly string? RSI;

	public readonly string? IconState;

	public readonly Color? Color;

	public CuffableComponentState(int numHandsCuffed, bool canStillInteract, string? rsiPath, string? iconState, Color? color)
	{
		NumHandsCuffed = numHandsCuffed;
		CanStillInteract = canStillInteract;
		RSI = rsiPath;
		IconState = iconState;
		Color = color;
	}
}
