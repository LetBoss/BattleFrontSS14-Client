using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._PUBG.Reputation;

[Serializable]
[NetSerializable]
public sealed class PubgReputationStateMessage : EntityEventArgs
{
	public int Reputation { get; }

	public PubgReputationStateMessage(int reputation)
	{
		Reputation = reputation;
	}
}
