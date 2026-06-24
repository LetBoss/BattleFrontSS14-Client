using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Salvage.Magnet;

[Serializable]
[NetSerializable]
public sealed class MagnetClaimOfferEvent : BoundUserInterfaceMessage
{
	public int Index;
}
