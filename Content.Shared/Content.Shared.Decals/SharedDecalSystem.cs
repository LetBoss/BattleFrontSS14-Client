using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;

namespace Content.Shared.Decals;

public abstract class SharedDecalSystem : EntitySystem
{
	[Dependency]
	protected IPrototypeManager PrototypeManager;

	[Dependency]
	protected IMapManager MapManager;

	protected bool PvsEnabled;

	public const int ChunkSize = 32;

	public static Vector2i GetChunkIndices(Vector2 coordinates)
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		return new Vector2i((int)Math.Floor(coordinates.X / 32f), (int)Math.Floor(coordinates.Y / 32f));
	}

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<GridInitializeEvent>((EntityEventHandler<GridInitializeEvent>)OnGridInitialize, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<DecalGridComponent, ComponentStartup>((ComponentEventHandler<DecalGridComponent, ComponentStartup>)OnCompStartup, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<DecalGridComponent, ComponentGetState>((ComponentEventRefHandler<DecalGridComponent, ComponentGetState>)OnGetState, (Type[])null, (Type[])null);
	}

	private void OnGetState(EntityUid uid, DecalGridComponent component, ref ComponentGetState args)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		if (PvsEnabled && !((ComponentGetState)(ref args)).ReplayState)
		{
			return;
		}
		if (((ComponentGetState)(ref args)).FromTick <= ((Component)component).CreationTick || ((ComponentGetState)(ref args)).FromTick <= component.ForceTick)
		{
			((ComponentGetState)(ref args)).State = (IComponentState)(object)new DecalGridState(component.ChunkCollection.ChunkCollection);
			return;
		}
		Dictionary<Vector2i, DecalGridComponent.DecalChunk> data = new Dictionary<Vector2i, DecalGridComponent.DecalChunk>();
		foreach (var (index, chunk) in component.ChunkCollection.ChunkCollection)
		{
			if (chunk.LastModified >= ((ComponentGetState)(ref args)).FromTick)
			{
				data[index] = chunk;
			}
		}
		((ComponentGetState)(ref args)).State = (IComponentState)(object)new DecalGridDeltaState(data, new HashSet<Vector2i>(component.ChunkCollection.ChunkCollection.Keys));
	}

	private void OnGridInitialize(GridInitializeEvent msg)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).EnsureComp<DecalGridComponent>(msg.EntityUid);
	}

	private void OnCompStartup(EntityUid uid, DecalGridComponent component, ComponentStartup args)
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		foreach (var (indices, decalChunk2) in component.ChunkCollection.ChunkCollection)
		{
			foreach (uint decalUid in decalChunk2.Decals.Keys)
			{
				component.DecalIndex[decalUid] = indices;
			}
		}
		((EntitySystem)this).Dirty(uid, (IComponent)(object)component, (MetaDataComponent)null);
	}

	protected Dictionary<Vector2i, DecalGridComponent.DecalChunk>? ChunkCollection(EntityUid gridEuid, DecalGridComponent? comp = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<DecalGridComponent>(gridEuid, ref comp, true))
		{
			return null;
		}
		return comp.ChunkCollection.ChunkCollection;
	}

	protected virtual void DirtyChunk(EntityUid id, Vector2i chunkIndices, DecalGridComponent.DecalChunk chunk)
	{
	}

	protected bool RemoveDecalInternal(EntityUid gridId, uint decalId, [NotNullWhen(true)] out Decal? removed, DecalGridComponent? component = null)
	{
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		removed = null;
		if (!((EntitySystem)this).Resolve<DecalGridComponent>(gridId, ref component, true))
		{
			return false;
		}
		if (!component.DecalIndex.Remove(decalId, out var indices) || !component.ChunkCollection.ChunkCollection.TryGetValue(indices, out DecalGridComponent.DecalChunk chunk) || !chunk.Decals.Remove(decalId, out removed))
		{
			return false;
		}
		if (chunk.Decals.Count == 0)
		{
			component.ChunkCollection.ChunkCollection.Remove(indices);
		}
		DirtyChunk(gridId, indices, chunk);
		OnDecalRemoved(gridId, decalId, component, indices, chunk);
		return true;
	}

	protected virtual void OnDecalRemoved(EntityUid gridId, uint decalId, DecalGridComponent component, Vector2i indices, DecalGridComponent.DecalChunk chunk)
	{
	}

	public virtual HashSet<(uint Index, Decal Decal)> GetDecalsInRange(EntityUid gridId, Vector2 position, float distance = 0.75f, Func<Decal, bool>? validDelegate = null)
	{
		return new HashSet<(uint, Decal)>();
	}

	public virtual bool RemoveDecal(EntityUid gridId, uint decalId, DecalGridComponent? component = null)
	{
		return true;
	}
}
