using System;
using Content.Shared._CIV14merka.Teams;
using Content.Shared._RMC14.Vehicle;
using Content.Shared.Ghost;
using Content.Shared.Mobs.Components;
using Content.Shared.Mobs.Systems;
using Content.Shared.Vehicle.Components;
using Robust.Client.Graphics;
using Robust.Client.Player;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Player;
using Robust.Shared.Timing;

namespace Content.Client._CIV14merka.Teams;

public sealed class CivTeamVisualsSystem : EntitySystem
{
	[Dependency]
	private IOverlayManager _overlayManager;

	[Dependency]
	private IPlayerManager _player;

	[Dependency]
	private IGameTiming _timing;

	[Dependency]
	private MobStateSystem _mobState;

	private CivTeamPlateOverlay? _plateOverlay;

	private CivTeamIconOverlay? _iconOverlay;

	private EntityUid? _lastLocalEntity;

	private TimeSpan _nextRefresh;

	private bool _dirty = true;

	private static readonly Color TeamColor = Color.FromHex((ReadOnlySpan<char>)"#4da6ff", (Color?)null);

	private static readonly Color SquadColor = Color.FromHex((ReadOnlySpan<char>)"#54ff72", (Color?)null);

	private static readonly Color SquadLeaderColor = Color.FromHex((ReadOnlySpan<char>)"#ffd54f", (Color?)null);

	private static readonly Color GhostBlueTeamColor = Color.FromHex((ReadOnlySpan<char>)"#4da6ff", (Color?)null);

	private static readonly Color GhostRedTeamColor = Color.FromHex((ReadOnlySpan<char>)"#ff5c5c", (Color?)null);

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		_plateOverlay = new CivTeamPlateOverlay((IEntityManager)(object)base.EntityManager, _player, this);
		_iconOverlay = new CivTeamIconOverlay();
		((EntitySystem)this).SubscribeLocalEvent<CivTeamMemberComponent, ComponentStartup>((EntityEventRefHandler<CivTeamMemberComponent, ComponentStartup>)OnTeamMemberChanged, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<CivTeamMemberComponent, ComponentShutdown>((EntityEventRefHandler<CivTeamMemberComponent, ComponentShutdown>)OnTeamMemberChanged, (Type[])null, (Type[])null);
	}

	public override void Shutdown()
	{
		((EntitySystem)this).Shutdown();
		RemoveOverlays();
	}

	public override void Update(float frameTime)
	{
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).Update(frameTime);
		EntityUid? localEntity = ((ISharedPlayerManager)_player).LocalEntity;
		EntityUid? lastLocalEntity = _lastLocalEntity;
		EntityUid? val = localEntity;
		if (lastLocalEntity.HasValue != val.HasValue || (lastLocalEntity.HasValue && lastLocalEntity.GetValueOrDefault() != val.GetValueOrDefault()))
		{
			_lastLocalEntity = localEntity;
			_dirty = true;
		}
		if (_dirty || !(_timing.CurTime < _nextRefresh))
		{
			RefreshVisuals();
			_dirty = false;
			_nextRefresh = _timing.CurTime + TimeSpan.FromSeconds(0.2);
		}
	}

	public bool TryGetRelationColor(EntityUid uid, out Color color)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		color = Color.White;
		EntityUid? localEntity = ((ISharedPlayerManager)_player).LocalEntity;
		if (!localEntity.HasValue || localEntity.Value == uid)
		{
			return false;
		}
		CivTeamMemberComponent otherMember = default(CivTeamMemberComponent);
		if (((EntitySystem)this).TryComp<CivTeamMemberComponent>(uid, ref otherMember))
		{
			return TryGetTeamMemberRelationColor(localEntity.Value, uid, otherMember, out color);
		}
		VehicleComponent vehicle = default(VehicleComponent);
		if (((EntitySystem)this).TryComp<VehicleComponent>(uid, ref vehicle))
		{
			return TryGetVehicleRelationColor(localEntity.Value, uid, vehicle, out color);
		}
		return false;
	}

	private void OnTeamMemberChanged(Entity<CivTeamMemberComponent> ent, ref ComponentStartup args)
	{
		_dirty = true;
	}

	private void OnTeamMemberChanged(Entity<CivTeamMemberComponent> ent, ref ComponentShutdown args)
	{
		_dirty = true;
	}

	private void RefreshVisuals()
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? localEntity = ((ISharedPlayerManager)_player).LocalEntity;
		if (!localEntity.HasValue || !CanViewerSeeTeamVisuals(localEntity.Value))
		{
			RemoveOverlays();
		}
		else
		{
			UpdateOverlays();
		}
	}

	private bool CanViewerSeeTeamVisuals(EntityUid uid)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).HasComp<CivTeamMemberComponent>(uid))
		{
			return ((EntitySystem)this).HasComp<GhostComponent>(uid);
		}
		return true;
	}

	private void UpdateOverlays()
	{
		if (_plateOverlay == null || _iconOverlay == null)
		{
			return;
		}
		if (!HasTrackedAllies())
		{
			RemoveOverlays();
			return;
		}
		if (!_overlayManager.HasOverlay<CivTeamPlateOverlay>())
		{
			_overlayManager.AddOverlay((Overlay)(object)_plateOverlay);
		}
		if (!_overlayManager.HasOverlay<CivTeamIconOverlay>())
		{
			_overlayManager.AddOverlay((Overlay)(object)_iconOverlay);
		}
	}

	private void RemoveOverlays()
	{
		if (_overlayManager.HasOverlay<CivTeamPlateOverlay>())
		{
			_overlayManager.RemoveOverlay<CivTeamPlateOverlay>();
		}
		if (_overlayManager.HasOverlay<CivTeamIconOverlay>())
		{
			_overlayManager.RemoveOverlay<CivTeamIconOverlay>();
		}
	}

	private bool HasTrackedAllies()
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? localEntity = ((ISharedPlayerManager)_player).LocalEntity;
		if (localEntity.HasValue && ((EntitySystem)this).HasComp<CivTeamMemberComponent>(localEntity.Value))
		{
			return true;
		}
		EntityQueryEnumerator<CivTeamMemberComponent> val = ((EntitySystem)this).EntityQueryEnumerator<CivTeamMemberComponent>();
		EntityUid uid = default(EntityUid);
		CivTeamMemberComponent civTeamMemberComponent = default(CivTeamMemberComponent);
		while (val.MoveNext(ref uid, ref civTeamMemberComponent))
		{
			if (TryGetRelationColor(uid, out var _))
			{
				return true;
			}
		}
		return false;
	}

	private static Color GetRelationColor(CivTeamMemberComponent localMember, CivTeamMemberComponent otherMember)
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		if (localMember.SquadId != 0 && localMember.SquadId == otherMember.SquadId)
		{
			if (otherMember.IsSquadLeader)
			{
				return SquadLeaderColor;
			}
			return SquadColor;
		}
		return TeamColor;
	}

	private bool TryGetTeamMemberRelationColor(EntityUid local, EntityUid other, CivTeamMemberComponent otherMember, out Color color)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		color = Color.White;
		if (otherMember.TeamId == 0)
		{
			return false;
		}
		MobStateComponent component = default(MobStateComponent);
		if (((EntitySystem)this).TryComp<MobStateComponent>(other, ref component) && _mobState.IsDead(other, component))
		{
			return false;
		}
		GhostComponent ghostComponent = default(GhostComponent);
		if (((EntitySystem)this).TryComp<GhostComponent>(local, ref ghostComponent))
		{
			color = GetGhostRelationColor(otherMember.TeamId);
			return color != Color.White;
		}
		CivTeamMemberComponent civTeamMemberComponent = default(CivTeamMemberComponent);
		if (!((EntitySystem)this).TryComp<CivTeamMemberComponent>(local, ref civTeamMemberComponent) || civTeamMemberComponent.TeamId == 0 || civTeamMemberComponent.TeamId != otherMember.TeamId)
		{
			return false;
		}
		color = GetRelationColor(civTeamMemberComponent, otherMember);
		return true;
	}

	private bool TryGetVehicleRelationColor(EntityUid local, EntityUid vehicleUid, VehicleComponent vehicle, out Color color)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		color = Color.White;
		int best = 0;
		EntityUid? val = vehicle.Operator;
		if (val.HasValue)
		{
			EntityUid valueOrDefault = val.GetValueOrDefault();
			CivTeamMemberComponent otherMember = default(CivTeamMemberComponent);
			if (((EntitySystem)this).TryComp<CivTeamMemberComponent>(valueOrDefault, ref otherMember))
			{
				TryPickVehicleColor(local, valueOrDefault, otherMember, ref color, ref best);
			}
		}
		EntityQueryEnumerator<RMCVehicleInteriorOccupantComponent, CivTeamMemberComponent> val2 = ((EntitySystem)this).EntityQueryEnumerator<RMCVehicleInteriorOccupantComponent, CivTeamMemberComponent>();
		EntityUid val3 = default(EntityUid);
		RMCVehicleInteriorOccupantComponent rMCVehicleInteriorOccupantComponent = default(RMCVehicleInteriorOccupantComponent);
		CivTeamMemberComponent otherMember2 = default(CivTeamMemberComponent);
		while (val2.MoveNext(ref val3, ref rMCVehicleInteriorOccupantComponent, ref otherMember2))
		{
			val = rMCVehicleInteriorOccupantComponent.Vehicle;
			EntityUid val4 = vehicleUid;
			if (val.HasValue && !(val.GetValueOrDefault() != val4))
			{
				val4 = val3;
				val = vehicle.Operator;
				if (!val.HasValue || !(val4 == val.GetValueOrDefault()))
				{
					TryPickVehicleColor(local, val3, otherMember2, ref color, ref best);
				}
			}
		}
		return best > 0;
	}

	private bool TryPickVehicleColor(EntityUid local, EntityUid other, CivTeamMemberComponent otherMember, ref Color color, ref int best)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		if (!TryGetTeamMemberRelationColor(local, other, otherMember, out var color2))
		{
			return false;
		}
		int colorRank = GetColorRank(color2);
		if (colorRank <= best)
		{
			return false;
		}
		best = colorRank;
		color = color2;
		return true;
	}

	private static int GetColorRank(Color color)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		if (color == SquadLeaderColor)
		{
			return 3;
		}
		if (color == SquadColor)
		{
			return 2;
		}
		if (color == TeamColor || color == GhostBlueTeamColor || color == GhostRedTeamColor)
		{
			return 1;
		}
		return 0;
	}

	private static Color GetGhostRelationColor(int teamId)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		return (Color)(teamId switch
		{
			1 => GhostBlueTeamColor, 
			2 => GhostRedTeamColor, 
			_ => Color.White, 
		});
	}
}
