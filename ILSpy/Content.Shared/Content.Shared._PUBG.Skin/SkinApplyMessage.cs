using System;
using System.Collections.Generic;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._PUBG.Skin;

[Serializable]
[NetSerializable]
public sealed class SkinApplyMessage : EntityEventArgs
{
	public Dictionary<string, string> NewOutfit { get; }

	public SkinApplyMessage(Dictionary<string, string> newOutfit)
	{
		NewOutfit = newOutfit;
	}
}
