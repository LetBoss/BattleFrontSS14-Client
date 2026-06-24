using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;

namespace Robust.Shared.Map.Events;

[ByRefEvent]
internal readonly record struct RegenerateGridBoundsEvent(EntityUid Entity, Dictionary<MapChunk, List<Box2i>> ChunkRectangles, List<MapChunk> RemovedChunks)
{
	public readonly EntityUid Entity = Entity;

	public readonly Dictionary<MapChunk, List<Box2i>> ChunkRectangles = ChunkRectangles;

	public readonly List<MapChunk> RemovedChunks = RemovedChunks;

	[CompilerGenerated]
	public void Deconstruct(out EntityUid Entity, out Dictionary<MapChunk, List<Box2i>> ChunkRectangles, out List<MapChunk> RemovedChunks)
	{
		Entity = this.Entity;
		ChunkRectangles = this.ChunkRectangles;
		RemovedChunks = this.RemovedChunks;
	}
}
