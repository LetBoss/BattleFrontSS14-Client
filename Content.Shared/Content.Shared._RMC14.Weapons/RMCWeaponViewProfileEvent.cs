using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._RMC14.Weapons;

[Serializable]
[NetSerializable]
public sealed class RMCWeaponViewProfileEvent : EntityEventArgs
{
	public int Nonce { get; }

	public bool ComponentDrawFov { get; }

	public bool RuntimeDrawFov { get; }

	public bool ComponentDrawLight { get; }

	public bool RuntimeDrawLight { get; }

	public bool ExaminerSkipChecks { get; }

	public bool ExaminerCheckInRangeUnOccluded { get; }

	public RMCWeaponViewProfileEvent(int nonce, bool componentDrawFov, bool runtimeDrawFov, bool componentDrawLight, bool runtimeDrawLight, bool examinerSkipChecks, bool examinerCheckInRangeUnOccluded)
	{
		Nonce = nonce;
		ComponentDrawFov = componentDrawFov;
		RuntimeDrawFov = runtimeDrawFov;
		ComponentDrawLight = componentDrawLight;
		RuntimeDrawLight = runtimeDrawLight;
		ExaminerSkipChecks = examinerSkipChecks;
		ExaminerCheckInRangeUnOccluded = examinerCheckInRangeUnOccluded;
	}
}
