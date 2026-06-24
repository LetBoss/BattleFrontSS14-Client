using System;
using System.Collections.Generic;
using Content.Shared.Radiation.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Radiation.Events;

[Serializable]
[NetSerializable]
public sealed class OnRadiationOverlayUpdateEvent(double elapsedTimeMs, int sourcesCount, int receiversCount, List<DebugRadiationRay> rays) : EntityEventArgs
{
	public readonly double ElapsedTimeMs = elapsedTimeMs;

	public readonly int SourcesCount = sourcesCount;

	public readonly int ReceiversCount = receiversCount;

	public readonly List<DebugRadiationRay> Rays = rays;
}
