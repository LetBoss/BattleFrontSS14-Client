using System;
using System.Collections.Generic;
using System.Numerics;
using Content.Shared._PUBG.FogOfWar;
using Content.Shared._PUBG.Gulag;
using Content.Shared.CombatMode;
using Content.Shared.Humanoid;
using Robust.Client.Graphics;
using Robust.Client.Input;
using Robust.Client.Player;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Player;

namespace Content.Client._PUBG.FogOfWar;

public sealed class PubgFogOfWarSystem : EntitySystem
{
	[Dependency]
	private IOverlayManager _overlayManager;

	[Dependency]
	private IPlayerManager _player;

	[Dependency]
	private SharedTransformSystem _xform;

	[Dependency]
	private IInputManager _input;

	[Dependency]
	private IEyeManager _eye;

	private readonly Dictionary<EntityUid, Dictionary<Vector2i, byte[]>> _gridChunks = new Dictionary<EntityUid, Dictionary<Vector2i, byte[]>>();

	private PubgFogOfWarOverlay? _overlay;

	private PubgFogOfWarSetAlphaOverlay? _setAlphaOverlay;

	private PubgFogOfWarResetAlphaOverlay? _resetAlphaOverlay;

	private MapId? _lobbyMapId;

	private MapId? _gameMapId;

	private bool _statusActive;

	private const float ViewAngleHalfLife = 0.05f;

	public bool Active { get; private set; }

	public IReadOnlyDictionary<EntityUid, Dictionary<Vector2i, byte[]>> GridChunks => _gridChunks;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		_overlay = new PubgFogOfWarOverlay(this);
		_setAlphaOverlay = new PubgFogOfWarSetAlphaOverlay();
		_resetAlphaOverlay = new PubgFogOfWarResetAlphaOverlay();
		_overlayManager.AddOverlay((Overlay)(object)_setAlphaOverlay);
		_overlayManager.AddOverlay((Overlay)(object)_overlay);
		_overlayManager.AddOverlay((Overlay)(object)_resetAlphaOverlay);
		((EntitySystem)this).SubscribeNetworkEvent<PubgFogOfWarStatusEvent>((EntityEventHandler<PubgFogOfWarStatusEvent>)OnStatus, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeNetworkEvent<PubgFogOfWarResetEvent>((EntityEventHandler<PubgFogOfWarResetEvent>)OnReset, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeNetworkEvent<PubgFogOfWarUpdateEvent>((EntityEventHandler<PubgFogOfWarUpdateEvent>)OnUpdate, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeNetworkEvent<GulagMapInfoEvent>((EntityEventHandler<GulagMapInfoEvent>)OnMapInfo, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<GridRemovalEvent>((EntityEventHandler<GridRemovalEvent>)OnGridRemoved, (Type[])null, (Type[])null);
	}

	public override void Update(float frameTime)
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).Update(frameTime);
		EntityUid? localEntity = ((ISharedPlayerManager)_player).LocalEntity;
		if (localEntity.HasValue)
		{
			Active = _statusActive && HasRequiredPlayerComponents(localEntity.Value) && IsOnAllowedMap(localEntity.Value);
		}
	}

	public override void FrameUpdate(float frameTime)
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).FrameUpdate(frameTime);
		EntityUid? localEntity = ((ISharedPlayerManager)_player).LocalEntity;
		PubgFogOfWarComponent pubgFogOfWarComponent = default(PubgFogOfWarComponent);
		TransformComponent val = default(TransformComponent);
		if (!localEntity.HasValue || !Active || !((EntitySystem)this).TryComp<PubgFogOfWarComponent>(localEntity.Value, ref pubgFogOfWarComponent) || !((EntitySystem)this).TryComp(localEntity.Value, ref val))
		{
			return;
		}
		bool num = !pubgFogOfWarComponent.DesiredViewAngle.HasValue;
		Angle val2 = _xform.GetWorldRotation(val);
		CombatModeComponent combatModeComponent = default(CombatModeComponent);
		if (((EntitySystem)this).TryComp<CombatModeComponent>(localEntity.Value, ref combatModeComponent) && combatModeComponent.IsInCombatMode)
		{
			ScreenCoordinates mouseScreenPosition = _input.MouseScreenPosition;
			if (((ScreenCoordinates)(ref mouseScreenPosition)).IsValid)
			{
				MapCoordinates val3 = _eye.PixelToMap(_input.MouseScreenPosition);
				if (val3.MapId != MapId.Nullspace)
				{
					Vector2 position = _xform.GetMapCoordinates(localEntity.Value, val).Position;
					val2 = DirectionExtensions.ToWorldAngle(val3.Position - position);
				}
			}
		}
		pubgFogOfWarComponent.DesiredViewAngle = val2;
		if (num)
		{
			pubgFogOfWarComponent.CurrentAngle = val2;
			return;
		}
		float num2 = 1f - MathF.Pow(2f, 0f - frameTime / 0.05f);
		pubgFogOfWarComponent.CurrentAngle = Angle.Lerp(ref pubgFogOfWarComponent.CurrentAngle, ref val2, num2);
	}

	private void OnStatus(PubgFogOfWarStatusEvent ev)
	{
		_statusActive = ev.Active;
	}

	private void OnReset(PubgFogOfWarResetEvent ev)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		if (Active)
		{
			EntityUid entity = ((EntitySystem)this).GetEntity(ev.GridId);
			if (!(entity == EntityUid.Invalid))
			{
				_gridChunks.Remove(entity);
			}
		}
	}

	private void OnUpdate(PubgFogOfWarUpdateEvent ev)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		if (!Active)
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
			PubgFogOfWarChunk[] chunks = ev.Chunks;
			foreach (PubgFogOfWarChunk pubgFogOfWarChunk in chunks)
			{
				value[pubgFogOfWarChunk.Index] = pubgFogOfWarChunk.TileStates;
			}
		}
	}

	private void OnGridRemoved(GridRemovalEvent ev)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		_gridChunks.Remove(ev.EntityUid);
	}

	private void OnMapInfo(GulagMapInfoEvent ev)
	{
		_lobbyMapId = ev.LobbyMapId;
		_gameMapId = ev.GameMapId;
	}

	public override void Shutdown()
	{
		((EntitySystem)this).Shutdown();
		if (_overlayManager.HasOverlay<PubgFogOfWarOverlay>())
		{
			_overlayManager.RemoveOverlay((Overlay)(object)_overlay);
		}
		if (_setAlphaOverlay != null && _overlayManager.HasOverlay(((object)_setAlphaOverlay).GetType()))
		{
			_overlayManager.RemoveOverlay((Overlay)(object)_setAlphaOverlay);
		}
		if (_resetAlphaOverlay != null && _overlayManager.HasOverlay(((object)_resetAlphaOverlay).GetType()))
		{
			_overlayManager.RemoveOverlay((Overlay)(object)_resetAlphaOverlay);
		}
	}

	public void SetActiveClient(bool active)
	{
		_statusActive = active;
	}

	private bool HasRequiredPlayerComponents(EntityUid uid)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).HasComp<PubgFogOfWarComponent>(uid))
		{
			return ((EntitySystem)this).HasComp<HumanoidAppearanceComponent>(uid);
		}
		return false;
	}

	private bool IsOnAllowedMap(EntityUid uid)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		MapId mapID = ((EntitySystem)this).Transform(uid).MapID;
		if (mapID == MapId.Nullspace)
		{
			return false;
		}
		if (!_lobbyMapId.HasValue && !_gameMapId.HasValue)
		{
			return true;
		}
		if (_lobbyMapId.HasValue && mapID == _lobbyMapId.Value)
		{
			return true;
		}
		if (_gameMapId.HasValue && mapID == _gameMapId.Value)
		{
			return true;
		}
		return false;
	}

	public bool TryGetTileState(EntityUid gridUid, Vector2i tile, out PubgFogOfWarTileState state)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		state = PubgFogOfWarTileState.Unseen;
		if (!_gridChunks.TryGetValue(gridUid, out Dictionary<Vector2i, byte[]> value))
		{
			return false;
		}
		Vector2i val = default(Vector2i);
		((Vector2i)(ref val))._002Ector(FloorDiv(tile.X, 16), FloorDiv(tile.Y, 16));
		if (!value.TryGetValue(val, out var value2) || value2.Length != 256)
		{
			return false;
		}
		int num = val.X * 16;
		int num2 = val.Y * 16;
		int num3 = tile.X - num;
		int num4 = tile.Y - num2;
		if (num3 < 0 || num3 >= 16 || num4 < 0 || num4 >= 16)
		{
			return false;
		}
		int num5 = num4 * 16 + num3;
		state = (PubgFogOfWarTileState)value2[num5];
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
