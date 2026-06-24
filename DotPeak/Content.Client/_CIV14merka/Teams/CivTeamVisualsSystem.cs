// Decompiled with JetBrains decompiler
// Type: Content.Client._CIV14merka.Teams.CivTeamVisualsSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

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
using System;

#nullable enable
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
  private static readonly Color TeamColor = Color.FromHex((ReadOnlySpan<char>) "#4da6ff", new Color?());
  private static readonly Color SquadColor = Color.FromHex((ReadOnlySpan<char>) "#54ff72", new Color?());
  private static readonly Color SquadLeaderColor = Color.FromHex((ReadOnlySpan<char>) "#ffd54f", new Color?());
  private static readonly Color GhostBlueTeamColor = Color.FromHex((ReadOnlySpan<char>) "#4da6ff", new Color?());
  private static readonly Color GhostRedTeamColor = Color.FromHex((ReadOnlySpan<char>) "#ff5c5c", new Color?());

  public virtual void Initialize()
  {
    base.Initialize();
    this._plateOverlay = new CivTeamPlateOverlay((IEntityManager) this.EntityManager, this._player, this);
    this._iconOverlay = new CivTeamIconOverlay();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<CivTeamMemberComponent, ComponentStartup>(new EntityEventRefHandler<CivTeamMemberComponent, ComponentStartup>((object) this, __methodptr(OnTeamMemberChanged)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<CivTeamMemberComponent, ComponentShutdown>(new EntityEventRefHandler<CivTeamMemberComponent, ComponentShutdown>((object) this, __methodptr(OnTeamMemberChanged)), (Type[]) null, (Type[]) null);
  }

  public virtual void Shutdown()
  {
    base.Shutdown();
    this.RemoveOverlays();
  }

  public virtual void Update(float frameTime)
  {
    base.Update(frameTime);
    EntityUid? localEntity = ((ISharedPlayerManager) this._player).LocalEntity;
    EntityUid? lastLocalEntity = this._lastLocalEntity;
    EntityUid? nullable = localEntity;
    if ((lastLocalEntity.HasValue == nullable.HasValue ? (lastLocalEntity.HasValue ? (EntityUid.op_Inequality(lastLocalEntity.GetValueOrDefault(), nullable.GetValueOrDefault()) ? 1 : 0) : 0) : 1) != 0)
    {
      this._lastLocalEntity = localEntity;
      this._dirty = true;
    }
    if (!this._dirty && this._timing.CurTime < this._nextRefresh)
      return;
    this.RefreshVisuals();
    this._dirty = false;
    this._nextRefresh = this._timing.CurTime + TimeSpan.FromSeconds(0.2);
  }

  public bool TryGetRelationColor(EntityUid uid, out Color color)
  {
    color = Color.White;
    EntityUid? localEntity = ((ISharedPlayerManager) this._player).LocalEntity;
    if (!localEntity.HasValue || EntityUid.op_Equality(localEntity.Value, uid))
      return false;
    CivTeamMemberComponent otherMember;
    if (this.TryComp<CivTeamMemberComponent>(uid, ref otherMember))
      return this.TryGetTeamMemberRelationColor(localEntity.Value, uid, otherMember, out color);
    VehicleComponent vehicle;
    return this.TryComp<VehicleComponent>(uid, ref vehicle) && this.TryGetVehicleRelationColor(localEntity.Value, uid, vehicle, out color);
  }

  private void OnTeamMemberChanged(Entity<CivTeamMemberComponent> ent, ref ComponentStartup args)
  {
    this._dirty = true;
  }

  private void OnTeamMemberChanged(Entity<CivTeamMemberComponent> ent, ref ComponentShutdown args)
  {
    this._dirty = true;
  }

  private void RefreshVisuals()
  {
    EntityUid? localEntity = ((ISharedPlayerManager) this._player).LocalEntity;
    if (!localEntity.HasValue || !this.CanViewerSeeTeamVisuals(localEntity.Value))
      this.RemoveOverlays();
    else
      this.UpdateOverlays();
  }

  private bool CanViewerSeeTeamVisuals(EntityUid uid)
  {
    return this.HasComp<CivTeamMemberComponent>(uid) || this.HasComp<GhostComponent>(uid);
  }

  private void UpdateOverlays()
  {
    if (this._plateOverlay == null || this._iconOverlay == null)
      return;
    if (!this.HasTrackedAllies())
    {
      this.RemoveOverlays();
    }
    else
    {
      if (!this._overlayManager.HasOverlay<CivTeamPlateOverlay>())
        this._overlayManager.AddOverlay((Overlay) this._plateOverlay);
      if (this._overlayManager.HasOverlay<CivTeamIconOverlay>())
        return;
      this._overlayManager.AddOverlay((Overlay) this._iconOverlay);
    }
  }

  private void RemoveOverlays()
  {
    if (this._overlayManager.HasOverlay<CivTeamPlateOverlay>())
      this._overlayManager.RemoveOverlay<CivTeamPlateOverlay>();
    if (!this._overlayManager.HasOverlay<CivTeamIconOverlay>())
      return;
    this._overlayManager.RemoveOverlay<CivTeamIconOverlay>();
  }

  private bool HasTrackedAllies()
  {
    EntityUid? localEntity = ((ISharedPlayerManager) this._player).LocalEntity;
    if (localEntity.HasValue && this.HasComp<CivTeamMemberComponent>(localEntity.Value))
      return true;
    EntityQueryEnumerator<CivTeamMemberComponent> entityQueryEnumerator = this.EntityQueryEnumerator<CivTeamMemberComponent>();
    EntityUid uid;
    CivTeamMemberComponent teamMemberComponent;
    while (entityQueryEnumerator.MoveNext(ref uid, ref teamMemberComponent))
    {
      if (this.TryGetRelationColor(uid, out Color _))
        return true;
    }
    return false;
  }

  private static Color GetRelationColor(
    CivTeamMemberComponent localMember,
    CivTeamMemberComponent otherMember)
  {
    if (localMember.SquadId == 0 || localMember.SquadId != otherMember.SquadId)
      return CivTeamVisualsSystem.TeamColor;
    return otherMember.IsSquadLeader ? CivTeamVisualsSystem.SquadLeaderColor : CivTeamVisualsSystem.SquadColor;
  }

  private bool TryGetTeamMemberRelationColor(
    EntityUid local,
    EntityUid other,
    CivTeamMemberComponent otherMember,
    out Color color)
  {
    color = Color.White;
    MobStateComponent component;
    if (otherMember.TeamId == 0 || this.TryComp<MobStateComponent>(other, ref component) && this._mobState.IsDead(other, component))
      return false;
    GhostComponent ghostComponent;
    if (this.TryComp<GhostComponent>(local, ref ghostComponent))
    {
      color = CivTeamVisualsSystem.GetGhostRelationColor(otherMember.TeamId);
      return Color.op_Inequality(color, Color.White);
    }
    CivTeamMemberComponent localMember;
    if (!this.TryComp<CivTeamMemberComponent>(local, ref localMember) || localMember.TeamId == 0 || localMember.TeamId != otherMember.TeamId)
      return false;
    color = CivTeamVisualsSystem.GetRelationColor(localMember, otherMember);
    return true;
  }

  private bool TryGetVehicleRelationColor(
    EntityUid local,
    EntityUid vehicleUid,
    VehicleComponent vehicle,
    out Color color)
  {
    color = Color.White;
    int best = 0;
    EntityUid? nullable1 = vehicle.Operator;
    if (nullable1.HasValue)
    {
      EntityUid valueOrDefault = nullable1.GetValueOrDefault();
      CivTeamMemberComponent otherMember;
      if (this.TryComp<CivTeamMemberComponent>(valueOrDefault, ref otherMember))
        this.TryPickVehicleColor(local, valueOrDefault, otherMember, ref color, ref best);
    }
    EntityQueryEnumerator<RMCVehicleInteriorOccupantComponent, CivTeamMemberComponent> entityQueryEnumerator = this.EntityQueryEnumerator<RMCVehicleInteriorOccupantComponent, CivTeamMemberComponent>();
    EntityUid other;
    RMCVehicleInteriorOccupantComponent occupantComponent;
    CivTeamMemberComponent otherMember1;
    while (entityQueryEnumerator.MoveNext(ref other, ref occupantComponent, ref otherMember1))
    {
      EntityUid? vehicle1 = occupantComponent.Vehicle;
      EntityUid entityUid1 = vehicleUid;
      if ((vehicle1.HasValue ? (EntityUid.op_Inequality(vehicle1.GetValueOrDefault(), entityUid1) ? 1 : 0) : 1) == 0)
      {
        EntityUid entityUid2 = other;
        EntityUid? nullable2 = vehicle.Operator;
        if ((nullable2.HasValue ? (EntityUid.op_Equality(entityUid2, nullable2.GetValueOrDefault()) ? 1 : 0) : 0) == 0)
          this.TryPickVehicleColor(local, other, otherMember1, ref color, ref best);
      }
    }
    return best > 0;
  }

  private bool TryPickVehicleColor(
    EntityUid local,
    EntityUid other,
    CivTeamMemberComponent otherMember,
    ref Color color,
    ref int best)
  {
    Color color1;
    if (!this.TryGetTeamMemberRelationColor(local, other, otherMember, out color1))
      return false;
    int colorRank = CivTeamVisualsSystem.GetColorRank(color1);
    if (colorRank <= best)
      return false;
    best = colorRank;
    color = color1;
    return true;
  }

  private static int GetColorRank(Color color)
  {
    if (Color.op_Equality(color, CivTeamVisualsSystem.SquadLeaderColor))
      return 3;
    if (Color.op_Equality(color, CivTeamVisualsSystem.SquadColor))
      return 2;
    return Color.op_Equality(color, CivTeamVisualsSystem.TeamColor) || Color.op_Equality(color, CivTeamVisualsSystem.GhostBlueTeamColor) || Color.op_Equality(color, CivTeamVisualsSystem.GhostRedTeamColor) ? 1 : 0;
  }

  private static Color GetGhostRelationColor(int teamId)
  {
    Color ghostRelationColor;
    switch (teamId)
    {
      case 1:
        ghostRelationColor = CivTeamVisualsSystem.GhostBlueTeamColor;
        break;
      case 2:
        ghostRelationColor = CivTeamVisualsSystem.GhostRedTeamColor;
        break;
      default:
        ghostRelationColor = Color.White;
        break;
    }
    return ghostRelationColor;
  }
}
