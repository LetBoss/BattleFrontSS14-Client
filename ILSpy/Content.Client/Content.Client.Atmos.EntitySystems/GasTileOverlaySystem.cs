using System;
using System.Collections.Generic;
using Content.Client.Atmos.Overlays;
using Content.Shared.Atmos;
using Content.Shared.Atmos.Components;
using Content.Shared.Atmos.EntitySystems;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Client.ResourceManagement;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.IoC;
using Robust.Shared.Maths;

namespace Content.Client.Atmos.EntitySystems;

public sealed class GasTileOverlaySystem : SharedGasTileOverlaySystem
{
	[Dependency]
	private IResourceCache _resourceCache;

	[Dependency]
	private IOverlayManager _overlayMan;

	[Dependency]
	private SpriteSystem _spriteSys;

	[Dependency]
	private SharedTransformSystem _xformSys;

	private GasTileOverlay _overlay;

	public override void Initialize()
	{
		base.Initialize();
		((EntitySystem)this).SubscribeNetworkEvent<GasOverlayUpdateEvent>((EntityEventHandler<GasOverlayUpdateEvent>)HandleGasOverlayUpdate, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<GasTileOverlayComponent, ComponentHandleState>((ComponentEventRefHandler<GasTileOverlayComponent, ComponentHandleState>)OnHandleState, (Type[])null, (Type[])null);
		_overlay = new GasTileOverlay(this, (IEntityManager)(object)((EntitySystem)this).EntityManager, _resourceCache, ProtoMan, _spriteSys, _xformSys);
		_overlayMan.AddOverlay((Overlay)(object)_overlay);
	}

	public override void Shutdown()
	{
		((EntitySystem)this).Shutdown();
		_overlayMan.RemoveOverlay<GasTileOverlay>();
	}

	private void OnHandleState(EntityUid gridUid, GasTileOverlayComponent comp, ref ComponentHandleState args)
	{
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		IComponentState current = ((ComponentHandleState)(ref args)).Current;
		Dictionary<Vector2i, GasOverlayChunk> dictionary;
		if (!(current is GasTileOverlayDeltaState gasTileOverlayDeltaState))
		{
			if (!(current is GasTileOverlayState gasTileOverlayState))
			{
				return;
			}
			dictionary = gasTileOverlayState.Chunks;
			foreach (Vector2i key2 in comp.Chunks.Keys)
			{
				if (!gasTileOverlayState.Chunks.ContainsKey(key2))
				{
					comp.Chunks.Remove(key2);
				}
			}
		}
		else
		{
			dictionary = gasTileOverlayDeltaState.ModifiedChunks;
			foreach (Vector2i key3 in comp.Chunks.Keys)
			{
				if (!gasTileOverlayDeltaState.AllChunks.Contains(key3))
				{
					comp.Chunks.Remove(key3);
				}
			}
		}
		foreach (var (key, value) in dictionary)
		{
			comp.Chunks[key] = value;
		}
	}

	private void HandleGasOverlayUpdate(GasOverlayUpdateEvent ev)
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		NetEntity key;
		GasTileOverlayComponent gasTileOverlayComponent = default(GasTileOverlayComponent);
		foreach (KeyValuePair<NetEntity, HashSet<Vector2i>> removedChunk in ev.RemovedChunks)
		{
			removedChunk.Deconstruct(out key, out var value);
			NetEntity val = key;
			HashSet<Vector2i> hashSet = value;
			EntityUid entity = ((EntitySystem)this).GetEntity(val);
			if (!((EntitySystem)this).TryComp<GasTileOverlayComponent>(entity, ref gasTileOverlayComponent))
			{
				continue;
			}
			foreach (Vector2i item in hashSet)
			{
				gasTileOverlayComponent.Chunks.Remove(item);
			}
		}
		GasTileOverlayComponent gasTileOverlayComponent2 = default(GasTileOverlayComponent);
		foreach (KeyValuePair<NetEntity, List<GasOverlayChunk>> updatedChunk in ev.UpdatedChunks)
		{
			updatedChunk.Deconstruct(out key, out var value2);
			NetEntity val2 = key;
			List<GasOverlayChunk> list = value2;
			EntityUid entity2 = ((EntitySystem)this).GetEntity(val2);
			if (!((EntitySystem)this).TryComp<GasTileOverlayComponent>(entity2, ref gasTileOverlayComponent2))
			{
				continue;
			}
			foreach (GasOverlayChunk item2 in list)
			{
				gasTileOverlayComponent2.Chunks[item2.Index] = item2;
			}
		}
	}
}
