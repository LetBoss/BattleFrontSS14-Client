// Decompiled with JetBrains decompiler
// Type: Content.Client.Administration.UI.SpawnExplosion.SpawnExplosionEui
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.Eui;
using Content.Shared.Administration;
using Content.Shared.Eui;
using Robust.Client.Graphics;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Client.Administration.UI.SpawnExplosion;

public sealed class SpawnExplosionEui : BaseEui
{
  [Dependency]
  private EntityManager _entManager;
  [Dependency]
  private IOverlayManager _overlayManager;
  private readonly SpawnExplosionWindow _window;
  private ExplosionDebugOverlay? _debugOverlay;

  public SpawnExplosionEui()
  {
    IoCManager.InjectDependencies<SpawnExplosionEui>(this);
    this._window = new SpawnExplosionWindow(this);
    ((BaseWindow) this._window).OnClose += new Action(this.SendClosedMessage);
  }

  public override void Opened()
  {
    base.Opened();
    ((BaseWindow) this._window).OpenCentered();
  }

  public override void Closed()
  {
    base.Closed();
    ((BaseWindow) this._window).OnClose -= new Action(this.SendClosedMessage);
    ((BaseWindow) this._window).Close();
    this.ClearOverlay();
  }

  public void SendClosedMessage() => this.SendMessage((EuiMessageBase) new CloseEuiMessage());

  public void ClearOverlay()
  {
    if (this._overlayManager.HasOverlay<ExplosionDebugOverlay>())
      this._overlayManager.RemoveOverlay<ExplosionDebugOverlay>();
    this._debugOverlay = (ExplosionDebugOverlay) null;
  }

  public void RequestPreviewData(
    MapCoordinates epicenter,
    string typeId,
    float totalIntensity,
    float intensitySlope,
    float maxIntensity)
  {
    this.SendMessage((EuiMessageBase) new SpawnExplosionEuiMsg.PreviewRequest(epicenter, typeId, totalIntensity, intensitySlope, maxIntensity));
  }

  public override void HandleMessage(EuiMessageBase msg)
  {
    if (!(msg is SpawnExplosionEuiMsg.PreviewData previewData))
      return;
    if (this._debugOverlay == null)
    {
      this._debugOverlay = new ExplosionDebugOverlay();
      this._overlayManager.AddOverlay((Overlay) this._debugOverlay);
    }
    Dictionary<EntityUid, Dictionary<int, List<Vector2i>>> dictionary1 = new Dictionary<EntityUid, Dictionary<int, List<Vector2i>>>();
    this._debugOverlay.Tiles.Clear();
    foreach ((NetEntity key, Dictionary<int, List<Vector2i>> dictionary2) in previewData.Explosion.Tiles)
      dictionary1[this._entManager.GetEntity(key)] = dictionary2;
    this._debugOverlay.Tiles = dictionary1;
    this._debugOverlay.SpaceTiles = previewData.Explosion.SpaceTiles;
    this._debugOverlay.Intensity = previewData.Explosion.Intensity;
    this._debugOverlay.Slope = previewData.Slope;
    this._debugOverlay.TotalIntensity = previewData.TotalIntensity;
    this._debugOverlay.Map = previewData.Explosion.Epicenter.MapId;
    this._debugOverlay.SpaceMatrix = previewData.Explosion.SpaceMatrix;
    this._debugOverlay.SpaceTileSize = previewData.Explosion.SpaceTileSize;
  }
}
