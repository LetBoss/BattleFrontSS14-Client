using System;
using Robust.Shared.Serialization;

namespace Content.Shared._RMC14.Item;

[Serializable]
[NetSerializable]
public enum CamouflageType : byte
{
	Jungle = 1,
	Desert,
	Snow,
	Classic,
	Urban
}
