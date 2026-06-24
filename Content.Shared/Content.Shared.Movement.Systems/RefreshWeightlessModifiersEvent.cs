using Robust.Shared.GameObjects;

namespace Content.Shared.Movement.Systems;

[ByRefEvent]
public record struct RefreshWeightlessModifiersEvent
{
	public float WeightlessAcceleration;

	public float WeightlessAccelerationMod;

	public float WeightlessModifier;

	public float WeightlessFriction;

	public float WeightlessFrictionMod;

	public float WeightlessFrictionNoInput;

	public float WeightlessFrictionNoInputMod;

	public void ModifyFriction(float friction, float noInput)
	{
		WeightlessFrictionMod *= friction;
		WeightlessFrictionNoInput *= noInput;
	}

	public void ModifyFriction(float friction)
	{
		ModifyFriction(friction, friction);
	}

	public void ModifyAcceleration(float acceleration, float modifier)
	{
		WeightlessAcceleration *= acceleration;
		WeightlessModifier *= modifier;
	}

	public void ModifyAcceleration(float modifier)
	{
		ModifyAcceleration(modifier, modifier);
	}
}
