using System.Numerics;
using Robust.Shared.GameObjects;

namespace Content.Shared.Camera;

[ByRefEvent]
public record struct GetEyeOffsetEvent(Vector2 Offset);
