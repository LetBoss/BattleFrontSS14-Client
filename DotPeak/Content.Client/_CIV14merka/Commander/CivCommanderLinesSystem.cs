// Decompiled with JetBrains decompiler
// Type: Content.Client._CIV14merka.Commander.CivCommanderLinesSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared._CIV14merka.Commander;
using Content.Shared._CIV14merka.Input;
using Content.Shared._CIV14merka.Teams;
using Robust.Client.Graphics;
using Robust.Client.Input;
using Robust.Client.Player;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.GameObjects;
using Robust.Shared.Input.Binding;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Player;
using System;
using System.Collections.Generic;
using System.Numerics;

#nullable enable
namespace Content.Client._CIV14merka.Commander;

public sealed class CivCommanderLinesSystem : EntitySystem
{
  [Dependency]
  private IEyeManager _eye;
  [Dependency]
  private IInputManager _input;
  [Dependency]
  private IOverlayManager _overlays;
  [Dependency]
  private IPlayerManager _player;
  [Dependency]
  private IUserInterfaceManager _ui;
  public const float EraseRadius = 2f;
  private readonly Dictionary<int, CivCommanderLineState> _lines = new Dictionary<int, CivCommanderLineState>();
  private readonly Dictionary<int, CivCommanderLabelState> _labels = new Dictionary<int, CivCommanderLabelState>();
  private CivCommanderLinesOverlay? _overlay;
  private CivCommanderLabelsOverlay? _labelsOverlay;

  public CivCommanderLineColor SelectedColor { get; set; }

  public bool IsDrawing { get; private set; }

  public MapCoordinates DrawStart { get; private set; }

  public bool IsPlacingLabel { get; private set; }

  public string PendingLabelText { get; private set; } = string.Empty;

  public float PendingLabelRotation { get; private set; }

  public IReadOnlyDictionary<int, CivCommanderLineState> Lines
  {
    get => (IReadOnlyDictionary<int, CivCommanderLineState>) this._lines;
  }

  public IReadOnlyDictionary<int, CivCommanderLabelState> Labels
  {
    get => (IReadOnlyDictionary<int, CivCommanderLabelState>) this._labels;
  }

  public virtual void Initialize()
  {
    base.Initialize();
    this.SubscribeNetworkEvent<CivCommanderLineAddedEvent>(new EntityEventHandler<CivCommanderLineAddedEvent>(this.OnLineAdded), (Type[]) null, (Type[]) null);
    this.SubscribeNetworkEvent<CivCommanderLineRemovedEvent>(new EntityEventHandler<CivCommanderLineRemovedEvent>(this.OnLineRemoved), (Type[]) null, (Type[]) null);
    this.SubscribeNetworkEvent<CivCommanderLinesSnapshotEvent>(new EntityEventHandler<CivCommanderLinesSnapshotEvent>(this.OnSnapshot), (Type[]) null, (Type[]) null);
    this.SubscribeNetworkEvent<CivCommanderLabelAddedEvent>(new EntityEventHandler<CivCommanderLabelAddedEvent>(this.OnLabelAdded), (Type[]) null, (Type[]) null);
    this.SubscribeNetworkEvent<CivCommanderLabelRemovedEvent>(new EntityEventHandler<CivCommanderLabelRemovedEvent>(this.OnLabelRemoved), (Type[]) null, (Type[]) null);
    this.SubscribeNetworkEvent<CivCommanderLabelsSnapshotEvent>(new EntityEventHandler<CivCommanderLabelsSnapshotEvent>(this.OnLabelsSnapshot), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    // ISSUE: method pointer
    // ISSUE: method pointer
    CommandBinds.Builder.Bind(CivKeyFunctions.CivCommanderDrawLine, (InputCmdHandler) new PointerInputCmdHandler(new PointerInputCmdDelegate2((object) this, __methodptr(HandleDraw)), true, true)).Bind(CivKeyFunctions.CivCommanderEraseLine, (InputCmdHandler) new PointerInputCmdHandler(new PointerInputCmdDelegate2((object) this, __methodptr(HandleErase)), true, true)).Bind(CivKeyFunctions.CivCommanderLabelRotate, (InputCmdHandler) new PointerInputCmdHandler(new PointerInputCmdDelegate2((object) this, __methodptr(HandleRotate)), true, true)).Register<CivCommanderLinesSystem>();
    this._overlay = new CivCommanderLinesOverlay((IEntityManager) this.EntityManager, this);
    this._overlays.AddOverlay((Overlay) this._overlay);
    this._labelsOverlay = new CivCommanderLabelsOverlay((IEntityManager) this.EntityManager, this);
    this._overlays.AddOverlay((Overlay) this._labelsOverlay);
  }

  public virtual void Shutdown()
  {
    base.Shutdown();
    CommandBinds.Unregister<CivCommanderLinesSystem>();
    if (this._overlay != null)
    {
      this._overlays.RemoveOverlay((Overlay) this._overlay);
      this._overlay = (CivCommanderLinesOverlay) null;
    }
    if (this._labelsOverlay != null)
    {
      this._overlays.RemoveOverlay((Overlay) this._labelsOverlay);
      this._labelsOverlay = (CivCommanderLabelsOverlay) null;
    }
    this._lines.Clear();
    this._labels.Clear();
    this.IsDrawing = false;
    this.IsPlacingLabel = false;
  }

  private void OnLineAdded(CivCommanderLineAddedEvent ev) => this._lines[ev.Line.Id] = ev.Line;

  private void OnLineRemoved(CivCommanderLineRemovedEvent ev) => this._lines.Remove(ev.Id);

  private void OnSnapshot(CivCommanderLinesSnapshotEvent ev)
  {
    this._lines.Clear();
    foreach (CivCommanderLineState line in ev.Lines)
      this._lines[line.Id] = line;
  }

  private void OnLabelAdded(CivCommanderLabelAddedEvent ev) => this._labels[ev.Label.Id] = ev.Label;

  private void OnLabelRemoved(CivCommanderLabelRemovedEvent ev) => this._labels.Remove(ev.Id);

  private void OnLabelsSnapshot(CivCommanderLabelsSnapshotEvent ev)
  {
    this._labels.Clear();
    foreach (CivCommanderLabelState label in ev.Labels)
      this._labels[label.Id] = label;
  }

  public void StartLabelPlacement(string text)
  {
    if (!this.IsCommander() || string.IsNullOrWhiteSpace(text))
      return;
    this.IsPlacingLabel = true;
    this.PendingLabelText = text.Trim();
    this.PendingLabelRotation = 0.0f;
  }

  public void CancelLabelPlacement()
  {
    this.IsPlacingLabel = false;
    this.PendingLabelText = string.Empty;
    this.PendingLabelRotation = 0.0f;
  }

  public void RequestClearAll()
  {
    if (!this.IsCommander())
      return;
    this.RaiseNetworkEvent((EntityEventArgs) new CivCommanderLinesClearRequestEvent());
  }

  private bool HandleDraw(in PointerInputCmdHandler.PointerInputCmdArgs args)
  {
    if (args.State != 1 || !this.IsCommander() || !this.IsViewportHover())
      return false;
    MapCoordinates map = this._eye.PixelToMap(this._input.MouseScreenPosition);
    if (MapId.op_Equality(map.MapId, MapId.Nullspace))
      return false;
    if (this.IsPlacingLabel)
    {
      this.RaiseNetworkEvent((EntityEventArgs) new CivCommanderLabelCreateRequestEvent(this.SelectedColor, map.MapId, map.Position, this.PendingLabelRotation, this.PendingLabelText));
      this.CancelLabelPlacement();
      return true;
    }
    if (!this.IsDrawing)
    {
      this.IsDrawing = true;
      this.DrawStart = map;
      return true;
    }
    if (MapId.op_Inequality(map.MapId, this.DrawStart.MapId))
    {
      this.IsDrawing = false;
      return true;
    }
    if ((double) (map.Position - this.DrawStart.Position).LengthSquared() < 0.0099999997764825821)
    {
      this.IsDrawing = false;
      return true;
    }
    this.RaiseNetworkEvent((EntityEventArgs) new CivCommanderLineCreateRequestEvent(this.SelectedColor, this.DrawStart.MapId, this.DrawStart.Position, map.Position));
    this.IsDrawing = false;
    return true;
  }

  private bool HandleErase(in PointerInputCmdHandler.PointerInputCmdArgs args)
  {
    if (args.State != 1 || !this.IsCommander() || !this.IsViewportHover())
      return false;
    if (this.IsPlacingLabel)
    {
      this.CancelLabelPlacement();
      return true;
    }
    if (this.IsDrawing)
    {
      this.IsDrawing = false;
      return true;
    }
    MapCoordinates map = this._eye.PixelToMap(this._input.MouseScreenPosition);
    if (MapId.op_Equality(map.MapId, MapId.Nullspace))
      return false;
    this.RaiseNetworkEvent((EntityEventArgs) new CivCommanderLineDeleteNearestRequestEvent(map.MapId, map.Position, 2f));
    this.RaiseNetworkEvent((EntityEventArgs) new CivCommanderLabelDeleteNearestRequestEvent(map.MapId, map.Position, 2f));
    return true;
  }

  private bool HandleRotate(in PointerInputCmdHandler.PointerInputCmdArgs args)
  {
    if (args.State != 1 || !this.IsCommander() || !this.IsPlacingLabel)
      return false;
    this.PendingLabelRotation = CivCommanderLinesSystem.NormalizeAngle(this.PendingLabelRotation + 1.57079637f);
    return true;
  }

  private static float NormalizeAngle(float a)
  {
    a %= 6.28318548f;
    if ((double) a < 0.0)
      a += 6.28318548f;
    return a;
  }

  public Vector2 GetCursorWorldPosition()
  {
    return this._eye.PixelToMap(this._input.MouseScreenPosition).Position;
  }

  public bool IsCommander()
  {
    EntityUid? localEntity = ((ISharedPlayerManager) this._player).LocalEntity;
    CivTeamMemberComponent teamMemberComponent;
    return localEntity.HasValue && this.TryComp<CivTeamMemberComponent>(localEntity.GetValueOrDefault(), ref teamMemberComponent) && teamMemberComponent.IsCommander;
  }

  public int LocalTeamId
  {
    get
    {
      EntityUid? localEntity = ((ISharedPlayerManager) this._player).LocalEntity;
      CivTeamMemberComponent teamMemberComponent;
      return !localEntity.HasValue || !this.TryComp<CivTeamMemberComponent>(localEntity.GetValueOrDefault(), ref teamMemberComponent) ? 0 : teamMemberComponent.TeamId;
    }
  }

  private bool IsViewportHover()
  {
    return this._ui.CurrentlyHovered == null || this._ui.CurrentlyHovered is IViewportControl;
  }
}
