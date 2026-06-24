using System;
using System.Collections.Generic;
using Content.Shared._CIV14merka.Commander;
using Content.Shared._CIV14merka.Teams;
using Robust.Client.Graphics;
using Robust.Client.Player;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Player;

namespace Content.Client._CIV14merka.Commander;

public sealed class CivCommanderVisionSystem : EntitySystem
{
	[Dependency]
	private IOverlayManager _overlays;

	[Dependency]
	private IPlayerManager _player;

	private readonly Dictionary<EntityUid, Dictionary<Vector2i, byte[]>> _gridChunks = new Dictionary<EntityUid, Dictionary<Vector2i, byte[]>>();

	private CivCommanderVisionOverlay? _overlay;

	private CivCommanderVisionSetAlphaOverlay? _setAlphaOverlay;

	private CivCommanderVisionResetAlphaOverlay? _resetAlphaOverlay;

	private bool _statusActive;

	public bool Active { get; private set; }

	public float VisionRange { get; private set; }

	public IReadOnlyDictionary<EntityUid, Dictionary<Vector2i, byte[]>> GridChunks => _gridChunks;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		_overlay = new CivCommanderVisionOverlay(this);
		_setAlphaOverlay = new CivCommanderVisionSetAlphaOverlay();
		_resetAlphaOverlay = new CivCommanderVisionResetAlphaOverlay();
		_overlays.AddOverlay((Overlay)(object)_overlay);
		_overlays.AddOverlay((Overlay)(object)_setAlphaOverlay);
		_overlays.AddOverlay((Overlay)(object)_resetAlphaOverlay);
		((EntitySystem)this).SubscribeNetworkEvent<CivCommanderVisionStatusEvent>((EntityEventHandler<CivCommanderVisionStatusEvent>)OnStatus, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeNetworkEvent<CivCommanderVisionResetEvent>((EntityEventHandler<CivCommanderVisionResetEvent>)OnReset, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeNetworkEvent<CivCommanderVisionUpdateEvent>((EntityEventHandler<CivCommanderVisionUpdateEvent>)OnUpdate, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<GridRemovalEvent>((EntityEventHandler<GridRemovalEvent>)OnGridRemoved, (Type[])null, (Type[])null);
	}

	public override void Update(float frameTime)
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).Update(frameTime);
		EntityUid? localEntity = ((ISharedPlayerManager)_player).LocalEntity;
		CivTeamMemberComponent civTeamMemberComponent = default(CivTeamMemberComponent);
		Active = _statusActive && localEntity.HasValue && ((EntitySystem)this).TryComp<CivTeamMemberComponent>(localEntity.Value, ref civTeamMemberComponent) && civTeamMemberComponent.IsCommander;
	}

	public override void Shutdown()
	{
		((EntitySystem)this).Shutdown();
		base.EntityManager.System<CivCommanderVisionHideSystem>().RestoreCachedAlphas();
		if (_overlay != null && _overlays.HasOverlay(((object)_overlay).GetType()))
		{
			_overlays.RemoveOverlay((Overlay)(object)_overlay);
		}
		if (_setAlphaOverlay != null && _overlays.HasOverlay(((object)_setAlphaOverlay).GetType()))
		{
			_overlays.RemoveOverlay((Overlay)(object)_setAlphaOverlay);
		}
		if (_resetAlphaOverlay != null && _overlays.HasOverlay(((object)_resetAlphaOverlay).GetType()))
		{
			_overlays.RemoveOverlay((Overlay)(object)_resetAlphaOverlay);
		}
	}

	private void OnStatus(CivCommanderVisionStatusEvent ev)
	{
		_statusActive = ev.Active;
		VisionRange = ev.Range;
		if (!ev.Active)
		{
			_gridChunks.Clear();
		}
	}

	private void OnReset(CivCommanderVisionResetEvent ev)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		EntityUid entity = ((EntitySystem)this).GetEntity(ev.GridId);
		if (entity != EntityUid.Invalid)
		{
			_gridChunks.Remove(entity);
		}
	}

	private void OnUpdate(CivCommanderVisionUpdateEvent ev)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		if (!_statusActive)
		{
			return;
		}
		EntityUid entity = ((EntitySystem)this).GetEntity(ev.GridId);
		if (!(entity == EntityUid.Invalid))
		{
			if (!_gridChunks.TryGetValue(entity, out Dictionary<Vector2i, byte[]> value))
			{
				value = new Dictionary<Vector2i, byte[]>();
				_gridChunks[entity] = value;
			}
			CivCommanderVisionChunk[] chunks = ev.Chunks;
			foreach (CivCommanderVisionChunk civCommanderVisionChunk in chunks)
			{
				value[civCommanderVisionChunk.Index] = civCommanderVisionChunk.TileStates;
			}
		}
	}

	private void OnGridRemoved(GridRemovalEvent ev)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		_gridChunks.Remove(ev.EntityUid);
	}

	public bool TryGetTileState(EntityUid gridUid, Vector2i tile, out CivCommanderVisionTileState state)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		state = CivCommanderVisionTileState.Unseen;
		if (!_gridChunks.TryGetValue(gridUid, out Dictionary<Vector2i, byte[]> value))
		{
			return false;
		}
		int num = 16;
		Vector2i val = default(Vector2i);
		((Vector2i)(ref val))._002Ector(FloorDiv(tile.X, num), FloorDiv(tile.Y, num));
		if (!value.TryGetValue(val, out var value2) || value2.Length != 256)
		{
			return false;
		}
		int num2 = val.X * num;
		int num3 = val.Y * num;
		int num4 = tile.X - num2;
		int num5 = tile.Y - num3;
		if (num4 < 0 || num4 >= num || num5 < 0 || num5 >= num)
		{
			return false;
		}
		int num6 = num5 * num + num4;
		state = (CivCommanderVisionTileState)value2[num6];
		return true;
	}

	private static int FloorDiv(int value, int size)
	{
		if (value >= 0)
		{
			return value / size;
		}
		return (value - size + 1) / size;
	}
}
