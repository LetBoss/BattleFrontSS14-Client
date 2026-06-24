// Decompiled with JetBrains decompiler
// Type: Content.Client._CIV14merka.Commander.CivCommanderVisionSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared._CIV14merka.Commander;
using Content.Shared._CIV14merka.Teams;
using Robust.Client.Graphics;
using Robust.Client.Player;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Player;
using System;
using System.Collections.Generic;

#nullable enable
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

  public IReadOnlyDictionary<EntityUid, Dictionary<Vector2i, byte[]>> GridChunks
  {
    get => (IReadOnlyDictionary<EntityUid, Dictionary<Vector2i, byte[]>>) this._gridChunks;
  }

  public virtual void Initialize()
  {
    base.Initialize();
    this._overlay = new CivCommanderVisionOverlay(this);
    this._setAlphaOverlay = new CivCommanderVisionSetAlphaOverlay();
    this._resetAlphaOverlay = new CivCommanderVisionResetAlphaOverlay();
    this._overlays.AddOverlay((Overlay) this._overlay);
    this._overlays.AddOverlay((Overlay) this._setAlphaOverlay);
    this._overlays.AddOverlay((Overlay) this._resetAlphaOverlay);
    this.SubscribeNetworkEvent<CivCommanderVisionStatusEvent>(new EntityEventHandler<CivCommanderVisionStatusEvent>(this.OnStatus), (Type[]) null, (Type[]) null);
    this.SubscribeNetworkEvent<CivCommanderVisionResetEvent>(new EntityEventHandler<CivCommanderVisionResetEvent>(this.OnReset), (Type[]) null, (Type[]) null);
    this.SubscribeNetworkEvent<CivCommanderVisionUpdateEvent>(new EntityEventHandler<CivCommanderVisionUpdateEvent>(this.OnUpdate), (Type[]) null, (Type[]) null);
    this.SubscribeLocalEvent<GridRemovalEvent>(new EntityEventHandler<GridRemovalEvent>(this.OnGridRemoved), (Type[]) null, (Type[]) null);
  }

  public virtual void Update(float frameTime)
  {
    base.Update(frameTime);
    EntityUid? localEntity = ((ISharedPlayerManager) this._player).LocalEntity;
    CivTeamMemberComponent teamMemberComponent;
    this.Active = this._statusActive && localEntity.HasValue && this.TryComp<CivTeamMemberComponent>(localEntity.Value, ref teamMemberComponent) && teamMemberComponent.IsCommander;
  }

  public virtual void Shutdown()
  {
    base.Shutdown();
    this.EntityManager.System<CivCommanderVisionHideSystem>().RestoreCachedAlphas();
    if (this._overlay != null && this._overlays.HasOverlay(((object) this._overlay).GetType()))
      this._overlays.RemoveOverlay((Overlay) this._overlay);
    if (this._setAlphaOverlay != null && this._overlays.HasOverlay(((object) this._setAlphaOverlay).GetType()))
      this._overlays.RemoveOverlay((Overlay) this._setAlphaOverlay);
    if (this._resetAlphaOverlay == null || !this._overlays.HasOverlay(((object) this._resetAlphaOverlay).GetType()))
      return;
    this._overlays.RemoveOverlay((Overlay) this._resetAlphaOverlay);
  }

  private void OnStatus(CivCommanderVisionStatusEvent ev)
  {
    this._statusActive = ev.Active;
    this.VisionRange = ev.Range;
    if (ev.Active)
      return;
    this._gridChunks.Clear();
  }

  private void OnReset(CivCommanderVisionResetEvent ev)
  {
    EntityUid entity = this.GetEntity(ev.GridId);
    if (!EntityUid.op_Inequality(entity, EntityUid.Invalid))
      return;
    this._gridChunks.Remove(entity);
  }

  private void OnUpdate(CivCommanderVisionUpdateEvent ev)
  {
    if (!this._statusActive)
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
    foreach (CivCommanderVisionChunk chunk in ev.Chunks)
      dictionary[chunk.Index] = chunk.TileStates;
  }

  private void OnGridRemoved(GridRemovalEvent ev) => this._gridChunks.Remove(ev.EntityUid);

  public bool TryGetTileState(
    EntityUid gridUid,
    Vector2i tile,
    out CivCommanderVisionTileState state)
  {
    state = CivCommanderVisionTileState.Unseen;
    Dictionary<Vector2i, byte[]> dictionary;
    if (!this._gridChunks.TryGetValue(gridUid, out dictionary))
      return false;
    int size = 16 /*0x10*/;
    Vector2i key;
    // ISSUE: explicit constructor call
    ((Vector2i) ref key).\u002Ector(CivCommanderVisionSystem.FloorDiv(tile.X, size), CivCommanderVisionSystem.FloorDiv(tile.Y, size));
    byte[] numArray;
    if (!dictionary.TryGetValue(key, out numArray) || numArray.Length != 256 /*0x0100*/)
      return false;
    int num1 = key.X * size;
    int num2 = key.Y * size;
    int num3 = tile.X - num1;
    int num4 = tile.Y - num2;
    if (num3 < 0 || num3 >= size || num4 < 0 || num4 >= size)
      return false;
    int index = num4 * size + num3;
    state = (CivCommanderVisionTileState) numArray[index];
    return true;
  }

  private static int FloorDiv(int value, int size)
  {
    return value >= 0 ? value / size : (value - size + 1) / size;
  }
}
