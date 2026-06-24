// Decompiled with JetBrains decompiler
// Type: Content.Client._PUBG.FogOfWar.PubgFogOfWarSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

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
using System;
using System.Collections.Generic;
using System.Numerics;

#nullable enable
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

  public IReadOnlyDictionary<EntityUid, Dictionary<Vector2i, byte[]>> GridChunks
  {
    get => (IReadOnlyDictionary<EntityUid, Dictionary<Vector2i, byte[]>>) this._gridChunks;
  }

  public virtual void Initialize()
  {
    base.Initialize();
    this._overlay = new PubgFogOfWarOverlay(this);
    this._setAlphaOverlay = new PubgFogOfWarSetAlphaOverlay();
    this._resetAlphaOverlay = new PubgFogOfWarResetAlphaOverlay();
    this._overlayManager.AddOverlay((Overlay) this._setAlphaOverlay);
    this._overlayManager.AddOverlay((Overlay) this._overlay);
    this._overlayManager.AddOverlay((Overlay) this._resetAlphaOverlay);
    this.SubscribeNetworkEvent<PubgFogOfWarStatusEvent>(new EntityEventHandler<PubgFogOfWarStatusEvent>(this.OnStatus), (Type[]) null, (Type[]) null);
    this.SubscribeNetworkEvent<PubgFogOfWarResetEvent>(new EntityEventHandler<PubgFogOfWarResetEvent>(this.OnReset), (Type[]) null, (Type[]) null);
    this.SubscribeNetworkEvent<PubgFogOfWarUpdateEvent>(new EntityEventHandler<PubgFogOfWarUpdateEvent>(this.OnUpdate), (Type[]) null, (Type[]) null);
    this.SubscribeNetworkEvent<GulagMapInfoEvent>(new EntityEventHandler<GulagMapInfoEvent>(this.OnMapInfo), (Type[]) null, (Type[]) null);
    this.SubscribeLocalEvent<GridRemovalEvent>(new EntityEventHandler<GridRemovalEvent>(this.OnGridRemoved), (Type[]) null, (Type[]) null);
  }

  public virtual void Update(float frameTime)
  {
    base.Update(frameTime);
    EntityUid? localEntity = ((ISharedPlayerManager) this._player).LocalEntity;
    if (!localEntity.HasValue)
      return;
    this.Active = this._statusActive && this.HasRequiredPlayerComponents(localEntity.Value) && this.IsOnAllowedMap(localEntity.Value);
  }

  public virtual void FrameUpdate(float frameTime)
  {
    base.FrameUpdate(frameTime);
    EntityUid? localEntity = ((ISharedPlayerManager) this._player).LocalEntity;
    PubgFogOfWarComponent fogOfWarComponent;
    TransformComponent transformComponent;
    if (!localEntity.HasValue || !this.Active || !this.TryComp<PubgFogOfWarComponent>(localEntity.Value, ref fogOfWarComponent) || !this.TryComp(localEntity.Value, ref transformComponent))
      return;
    int num1 = !fogOfWarComponent.DesiredViewAngle.HasValue ? 1 : 0;
    Angle angle = this._xform.GetWorldRotation(transformComponent);
    CombatModeComponent combatModeComponent;
    if (this.TryComp<CombatModeComponent>(localEntity.Value, ref combatModeComponent) && combatModeComponent.IsInCombatMode)
    {
      ScreenCoordinates mouseScreenPosition = this._input.MouseScreenPosition;
      if (((ScreenCoordinates) ref mouseScreenPosition).IsValid)
      {
        MapCoordinates map = this._eye.PixelToMap(this._input.MouseScreenPosition);
        if (MapId.op_Inequality(map.MapId, MapId.Nullspace))
        {
          Vector2 position = this._xform.GetMapCoordinates(localEntity.Value, transformComponent).Position;
          angle = DirectionExtensions.ToWorldAngle(map.Position - position);
        }
      }
    }
    fogOfWarComponent.DesiredViewAngle = new Angle?(angle);
    if (num1 != 0)
    {
      fogOfWarComponent.CurrentAngle = angle;
    }
    else
    {
      float num2 = 1f - MathF.Pow(2f, (float) -((double) frameTime / 0.05000000074505806));
      fogOfWarComponent.CurrentAngle = Angle.Lerp(ref fogOfWarComponent.CurrentAngle, ref angle, num2);
    }
  }

  private void OnStatus(PubgFogOfWarStatusEvent ev) => this._statusActive = ev.Active;

  private void OnReset(PubgFogOfWarResetEvent ev)
  {
    if (!this.Active)
      return;
    EntityUid entity = this.GetEntity(ev.GridId);
    if (EntityUid.op_Equality(entity, EntityUid.Invalid))
      return;
    this._gridChunks.Remove(entity);
  }

  private void OnUpdate(PubgFogOfWarUpdateEvent ev)
  {
    if (!this.Active)
      return;
    EntityUid entity = this.GetEntity(ev.GridId);
    if (EntityUid.op_Equality(entity, EntityUid.Invalid))
      return;
    Dictionary<Vector2i, byte[]> dictionary;
    if (!this._gridChunks.TryGetValue(entity, out dictionary))
    {
      dictionary = new Dictionary<Vector2i, byte[]>();
      this._gridChunks[entity] = dictionary;
    }
    foreach (PubgFogOfWarChunk chunk in ev.Chunks)
      dictionary[chunk.Index] = chunk.TileStates;
  }

  private void OnGridRemoved(GridRemovalEvent ev) => this._gridChunks.Remove(ev.EntityUid);

  private void OnMapInfo(GulagMapInfoEvent ev)
  {
    this._lobbyMapId = ev.LobbyMapId;
    this._gameMapId = ev.GameMapId;
  }

  public virtual void Shutdown()
  {
    base.Shutdown();
    if (this._overlayManager.HasOverlay<PubgFogOfWarOverlay>())
      this._overlayManager.RemoveOverlay((Overlay) this._overlay);
    if (this._setAlphaOverlay != null && this._overlayManager.HasOverlay(((object) this._setAlphaOverlay).GetType()))
      this._overlayManager.RemoveOverlay((Overlay) this._setAlphaOverlay);
    if (this._resetAlphaOverlay == null || !this._overlayManager.HasOverlay(((object) this._resetAlphaOverlay).GetType()))
      return;
    this._overlayManager.RemoveOverlay((Overlay) this._resetAlphaOverlay);
  }

  public void SetActiveClient(bool active) => this._statusActive = active;

  private bool HasRequiredPlayerComponents(EntityUid uid)
  {
    return this.HasComp<PubgFogOfWarComponent>(uid) && this.HasComp<HumanoidAppearanceComponent>(uid);
  }

  private bool IsOnAllowedMap(EntityUid uid)
  {
    MapId mapId = this.Transform(uid).MapID;
    return !MapId.op_Equality(mapId, MapId.Nullspace) && (!this._lobbyMapId.HasValue && !this._gameMapId.HasValue || this._lobbyMapId.HasValue && MapId.op_Equality(mapId, this._lobbyMapId.Value) || this._gameMapId.HasValue && MapId.op_Equality(mapId, this._gameMapId.Value));
  }

  public bool TryGetTileState(EntityUid gridUid, Vector2i tile, out PubgFogOfWarTileState state)
  {
    state = PubgFogOfWarTileState.Unseen;
    Dictionary<Vector2i, byte[]> dictionary;
    if (!this._gridChunks.TryGetValue(gridUid, out dictionary))
      return false;
    Vector2i key;
    // ISSUE: explicit constructor call
    ((Vector2i) ref key).\u002Ector(PubgFogOfWarSystem.FloorDiv(tile.X, 16 /*0x10*/), PubgFogOfWarSystem.FloorDiv(tile.Y, 16 /*0x10*/));
    byte[] numArray;
    if (!dictionary.TryGetValue(key, out numArray) || numArray.Length != 256 /*0x0100*/)
      return false;
    int num1 = key.X * 16 /*0x10*/;
    int num2 = key.Y * 16 /*0x10*/;
    int num3 = tile.X - num1;
    int num4 = tile.Y - num2;
    if (num3 < 0 || num3 >= 16 /*0x10*/ || num4 < 0 || num4 >= 16 /*0x10*/)
      return false;
    int index = num4 * 16 /*0x10*/ + num3;
    state = (PubgFogOfWarTileState) numArray[index];
    return true;
  }

  private static int FloorDiv(int value, int size)
  {
    return value >= 0 ? value / size : (value - size + 1) / size;
  }
}
