using System;

namespace Content.Shared.Atmos.Reactions;

[Flags]
public enum ReactionResult : byte
{
	NoReaction = 0,
	Reacting = 1,
	StopReactions = 2
}
