using System;
using Content.Shared.Humanoid.Markings;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Humanoid;

[Serializable]
[NetSerializable]
public sealed class HumanoidMarkingModifierMarkingSetMessage : BoundUserInterfaceMessage
{
	public MarkingSet MarkingSet { get; }

	public bool ResendState { get; }

	public HumanoidMarkingModifierMarkingSetMessage(MarkingSet set, bool resendState)
	{
		MarkingSet = set;
		ResendState = resendState;
	}
}
