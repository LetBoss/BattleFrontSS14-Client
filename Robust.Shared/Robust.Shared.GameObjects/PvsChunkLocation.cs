using Robust.Shared.Maths;

namespace Robust.Shared.GameObjects;

internal readonly record struct PvsChunkLocation(EntityUid Uid, Vector2i Indices);
