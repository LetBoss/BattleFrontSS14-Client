using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Robust.Shared.Map.Components;

[Serializable]
[NetSerializable]
public sealed class MapComponentState(MapId mapId, bool lightingEnabled, bool paused, bool init) : ComponentState
{
	public MapId MapId = mapId;

	public bool LightingEnabled = lightingEnabled;

	public bool MapPaused = paused;

	public bool Initialized = init;
}
