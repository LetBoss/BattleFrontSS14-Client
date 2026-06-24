using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Voting;

[Serializable]
[NetSerializable]
public sealed class VotePlayerListRequestEvent : EntityEventArgs
{
}
