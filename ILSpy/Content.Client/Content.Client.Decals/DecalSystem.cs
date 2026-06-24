using System;
using System.Collections.Generic;
using Content.Client.Decals.Overlays;
using Content.Shared.Decals;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.IoC;
using Robust.Shared.Maths;

namespace Content.Client.Decals;

public sealed class DecalSystem : SharedDecalSystem
{
	[Dependency]
	private IOverlayManager _overlayManager;

	[Dependency]
	private SpriteSystem _sprites;

	private DecalOverlay? _overlay;

	private HashSet<uint> _removedUids = new HashSet<uint>();

	private readonly List<Vector2i> _removedChunks = new List<Vector2i>();

	public override void Initialize()
	{
		base.Initialize();
		_overlay = new DecalOverlay(_sprites, (IEntityManager)(object)((EntitySystem)this).EntityManager, PrototypeManager);
		_overlayManager.AddOverlay((Overlay)(object)_overlay);
		((EntitySystem)this).SubscribeLocalEvent<DecalGridComponent, ComponentHandleState>((ComponentEventRefHandler<DecalGridComponent, ComponentHandleState>)OnHandleState, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeNetworkEvent<DecalChunkUpdateEvent>((EntityEventHandler<DecalChunkUpdateEvent>)OnChunkUpdate, (Type[])null, (Type[])null);
	}

	public void ToggleOverlay()
	{
		if (_overlay != null)
		{
			if (_overlayManager.HasOverlay<DecalOverlay>())
			{
				_overlayManager.RemoveOverlay((Overlay)(object)_overlay);
			}
			else
			{
				_overlayManager.AddOverlay((Overlay)(object)_overlay);
			}
		}
	}

	public override void Shutdown()
	{
		((EntitySystem)this).Shutdown();
		if (_overlay != null)
		{
			_overlayManager.RemoveOverlay((Overlay)(object)_overlay);
		}
	}

	protected override void OnDecalRemoved(EntityUid gridId, uint decalId, DecalGridComponent component, Vector2i indices, DecalGridComponent.DecalChunk chunk)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		base.OnDecalRemoved(gridId, decalId, component, indices, chunk);
		chunk.Decals.Remove(decalId);
	}

	private void OnHandleState(EntityUid gridUid, DecalGridComponent gridComp, ref ComponentHandleState args)
	{
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		_removedChunks.Clear();
		IComponentState current = ((ComponentHandleState)(ref args)).Current;
		Dictionary<Vector2i, DecalGridComponent.DecalChunk> dictionary;
		if (!(current is DecalGridDeltaState decalGridDeltaState))
		{
			if (!(current is DecalGridState decalGridState))
			{
				return;
			}
			dictionary = decalGridState.Chunks;
			foreach (Vector2i key in gridComp.ChunkCollection.ChunkCollection.Keys)
			{
				if (!decalGridState.Chunks.ContainsKey(key))
				{
					_removedChunks.Add(key);
				}
			}
		}
		else
		{
			dictionary = decalGridDeltaState.ModifiedChunks;
			foreach (Vector2i key2 in gridComp.ChunkCollection.ChunkCollection.Keys)
			{
				if (!decalGridDeltaState.AllChunks.Contains(key2))
				{
					_removedChunks.Add(key2);
				}
			}
		}
		if (_removedChunks.Count > 0)
		{
			RemoveChunks(gridUid, gridComp, _removedChunks);
		}
		if (dictionary.Count > 0)
		{
			UpdateChunks(gridUid, gridComp, dictionary);
		}
	}

	private void OnChunkUpdate(DecalChunkUpdateEvent ev)
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		NetEntity key;
		DecalGridComponent gridComp = default(DecalGridComponent);
		foreach (KeyValuePair<NetEntity, Dictionary<Vector2i, DecalGridComponent.DecalChunk>> datum in ev.Data)
		{
			datum.Deconstruct(out key, out var value);
			NetEntity val = key;
			Dictionary<Vector2i, DecalGridComponent.DecalChunk> dictionary = value;
			if (dictionary.Count != 0)
			{
				EntityUid entity = ((EntitySystem)this).GetEntity(val);
				if (!((EntitySystem)this).TryComp<DecalGridComponent>(entity, ref gridComp))
				{
					((EntitySystem)this).Log.Error($"Received decal information for an entity without a decal component: {((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(entity))}");
				}
				else
				{
					UpdateChunks(entity, gridComp, dictionary);
				}
			}
		}
		DecalGridComponent gridComp2 = default(DecalGridComponent);
		foreach (KeyValuePair<NetEntity, HashSet<Vector2i>> removedChunk in ev.RemovedChunks)
		{
			removedChunk.Deconstruct(out key, out var value2);
			NetEntity val2 = key;
			HashSet<Vector2i> hashSet = value2;
			if (hashSet.Count != 0)
			{
				EntityUid entity2 = ((EntitySystem)this).GetEntity(val2);
				if (!((EntitySystem)this).TryComp<DecalGridComponent>(entity2, ref gridComp2))
				{
					((EntitySystem)this).Log.Error($"Received decal information for an entity without a decal component: {((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(entity2))}");
				}
				else
				{
					RemoveChunks(entity2, gridComp2, hashSet);
				}
			}
		}
	}

	private void UpdateChunks(EntityUid gridId, DecalGridComponent gridComp, Dictionary<Vector2i, DecalGridComponent.DecalChunk> updatedGridChunks)
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		Dictionary<Vector2i, DecalGridComponent.DecalChunk> chunkCollection = gridComp.ChunkCollection.ChunkCollection;
		foreach (var (val2, decalChunk2) in updatedGridChunks)
		{
			if (chunkCollection.TryGetValue(val2, out var value))
			{
				_removedUids.Clear();
				_removedUids.UnionWith(value.Decals.Keys);
				_removedUids.ExceptWith(decalChunk2.Decals.Keys);
				foreach (uint removedUid in _removedUids)
				{
					OnDecalRemoved(gridId, removedUid, gridComp, val2, value);
					gridComp.DecalIndex.Remove(removedUid);
				}
			}
			chunkCollection[val2] = decalChunk2;
			foreach (var (key, _) in decalChunk2.Decals)
			{
				gridComp.DecalIndex[key] = val2;
			}
		}
	}

	private void RemoveChunks(EntityUid gridId, DecalGridComponent gridComp, IEnumerable<Vector2i> chunks)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		Dictionary<Vector2i, DecalGridComponent.DecalChunk> chunkCollection = gridComp.ChunkCollection.ChunkCollection;
		foreach (Vector2i chunk in chunks)
		{
			if (!chunkCollection.TryGetValue(chunk, out var value))
			{
				continue;
			}
			foreach (uint key in value.Decals.Keys)
			{
				OnDecalRemoved(gridId, key, gridComp, chunk, value);
				gridComp.DecalIndex.Remove(key);
			}
			chunkCollection.Remove(chunk);
		}
	}
}
