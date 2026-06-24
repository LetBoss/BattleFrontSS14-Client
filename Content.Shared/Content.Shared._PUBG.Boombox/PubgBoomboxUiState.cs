using System;
using System.Collections.Generic;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._PUBG.Boombox;

[Serializable]
[NetSerializable]
public sealed class PubgBoomboxUiState : BoundUserInterfaceState
{
	public List<PubgBoomboxTrackInfo> Tracks = new List<PubgBoomboxTrackInfo>();

	public bool Linked = true;

	public bool Syncing;
}
