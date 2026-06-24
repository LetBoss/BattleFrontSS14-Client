using System;
using System.Numerics;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;

namespace Content.Shared.Atmos.EntitySystems;

public abstract class SharedAtmosDebugOverlaySystem : EntitySystem
{
	[Serializable]
	[NetSerializable]
	public readonly record struct AtmosDebugOverlayData(Vector2 Indices, float Temperature, float[]? Moles, AtmosDirection PressureDirection, AtmosDirection LastPressureDirection, AtmosDirection BlockDirection, int? InExcitedGroup, bool IsSpace, bool MapAtmosphere, bool NoGrid, bool Immutable);

	[Serializable]
	[NetSerializable]
	public sealed class AtmosDebugOverlayMessage : EntityEventArgs
	{
		public NetEntity GridId { get; }

		public Vector2i BaseIdx { get; }

		public AtmosDebugOverlayData?[] OverlayData { get; }

		public AtmosDebugOverlayMessage(NetEntity gridIndices, Vector2i baseIdx, AtmosDebugOverlayData?[] overlayData)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			GridId = gridIndices;
			BaseIdx = baseIdx;
			OverlayData = overlayData;
		}
	}

	[Serializable]
	[NetSerializable]
	public sealed class AtmosDebugOverlayDisableMessage : EntityEventArgs
	{
	}

	public const int LocalViewRange = 16;

	protected float AccumulatedFrameTime;
}
