using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;

namespace Content.Shared.Implants;

[Serializable]
[NetSerializable]
public sealed class ChameleonControllerSelectedOutfitMessage(ProtoId<ChameleonOutfitPrototype> selectedOutfit) : BoundUserInterfaceMessage
{
	public readonly ProtoId<ChameleonOutfitPrototype> SelectedChameleonOutfit = selectedOutfit;
}
