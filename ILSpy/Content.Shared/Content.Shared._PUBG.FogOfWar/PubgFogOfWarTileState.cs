using System;
using Robust.Shared.Serialization;

namespace Content.Shared._PUBG.FogOfWar;

[Serializable]
[NetSerializable]
public enum PubgFogOfWarTileState : byte
{
	Unseen,
	Seen,
	Visible
}
