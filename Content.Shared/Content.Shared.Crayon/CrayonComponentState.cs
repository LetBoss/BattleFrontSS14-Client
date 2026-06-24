using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;

namespace Content.Shared.Crayon;

[Serializable]
[NetSerializable]
public sealed class CrayonComponentState : ComponentState
{
	public readonly Color Color;

	public readonly string State;

	public readonly int Charges;

	public readonly int Capacity;

	public CrayonComponentState(Color color, string state, int charges, int capacity)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		Color = color;
		State = state;
		Charges = charges;
		Capacity = capacity;
	}
}
