using System;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared._RMC14.Scoping;

[Serializable]
[DataRecord]
[NetSerializable]
public record struct ScopeZoomLevel(string? Name, float Zoom, float Offset, bool AllowMovement, TimeSpan DoAfter);
