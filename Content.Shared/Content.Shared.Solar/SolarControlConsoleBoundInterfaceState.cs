using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;

namespace Content.Shared.Solar;

[Serializable]
[NetSerializable]
public sealed class SolarControlConsoleBoundInterfaceState : BoundUserInterfaceState
{
	public Angle Rotation;

	public Angle AngularVelocity;

	public float OutputPower;

	public Angle TowardsSun;

	public SolarControlConsoleBoundInterfaceState(Angle r, Angle vm, float p, Angle tw)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		Rotation = r;
		AngularVelocity = vm;
		OutputPower = p;
		TowardsSun = tw;
	}
}
