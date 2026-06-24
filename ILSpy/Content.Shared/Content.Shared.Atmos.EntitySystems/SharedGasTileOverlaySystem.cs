using System;
using System.Collections.Generic;
using Content.Shared.Atmos.Components;
using Content.Shared.Atmos.Prototypes;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Atmos.EntitySystems;

public abstract class SharedGasTileOverlaySystem : EntitySystem
{
	[Serializable]
	[NetSerializable]
	public readonly struct GasOverlayData(byte fireState, byte[] opacity) : IEquatable<GasOverlayData>
	{
		[ViewVariables]
		public readonly byte FireState = fireState;

		[ViewVariables]
		public readonly byte[] Opacity = opacity;

		public bool Equals(GasOverlayData other)
		{
			if (FireState != other.FireState)
			{
				return false;
			}
			if (Opacity?.Length != other.Opacity?.Length)
			{
				return false;
			}
			if (Opacity != null && other.Opacity != null)
			{
				for (int i = 0; i < Opacity.Length; i++)
				{
					if (Opacity[i] != other.Opacity[i])
					{
						return false;
					}
				}
			}
			return true;
		}
	}

	[Serializable]
	[NetSerializable]
	public sealed class GasOverlayUpdateEvent : EntityEventArgs
	{
		public Dictionary<NetEntity, List<GasOverlayChunk>> UpdatedChunks = new Dictionary<NetEntity, List<GasOverlayChunk>>();

		public Dictionary<NetEntity, HashSet<Vector2i>> RemovedChunks = new Dictionary<NetEntity, HashSet<Vector2i>>();
	}

	public const byte ChunkSize = 8;

	protected float AccumulatedFrameTime;

	protected bool PvsEnabled;

	[Dependency]
	protected IPrototypeManager ProtoMan;

	public int[] VisibleGasId;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<GasTileOverlayComponent, ComponentGetState>((ComponentEventRefHandler<GasTileOverlayComponent, ComponentGetState>)OnGetState, (Type[])null, (Type[])null);
		List<int> visibleGases = new List<int>();
		for (int i = 0; i < 9; i++)
		{
			GasPrototype gasPrototype = ProtoMan.Index<GasPrototype>(i.ToString());
			if (!string.IsNullOrEmpty(gasPrototype.GasOverlayTexture) || (!string.IsNullOrEmpty(gasPrototype.GasOverlaySprite) && !string.IsNullOrEmpty(gasPrototype.GasOverlayState)))
			{
				visibleGases.Add(i);
			}
		}
		VisibleGasId = visibleGases.ToArray();
	}

	private void OnGetState(EntityUid uid, GasTileOverlayComponent component, ref ComponentGetState args)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		if (PvsEnabled && !((ComponentGetState)(ref args)).ReplayState)
		{
			return;
		}
		if (((ComponentGetState)(ref args)).FromTick <= ((Component)component).CreationTick || ((ComponentGetState)(ref args)).FromTick <= component.ForceTick)
		{
			((ComponentGetState)(ref args)).State = (IComponentState)(object)new GasTileOverlayState(component.Chunks);
			return;
		}
		Dictionary<Vector2i, GasOverlayChunk> data = new Dictionary<Vector2i, GasOverlayChunk>();
		foreach (var (index, chunk) in component.Chunks)
		{
			if (chunk.LastUpdate >= ((ComponentGetState)(ref args)).FromTick)
			{
				data[index] = chunk;
			}
		}
		((ComponentGetState)(ref args)).State = (IComponentState)(object)new GasTileOverlayDeltaState(data, new HashSet<Vector2i>(component.Chunks.Keys));
	}

	public static Vector2i GetGasChunkIndices(Vector2i indices)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		return new Vector2i((int)MathF.Floor((float)indices.X / 8f), (int)MathF.Floor((float)indices.Y / 8f));
	}
}
