// Decompiled with JetBrains decompiler
// Type: Content.Client.Atmos.Overlays.AtmosDebugOverlay
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.Atmos.EntitySystems;
using Content.Client.Resources;
using Content.Shared.Atmos;
using Content.Shared.Atmos.EntitySystems;
using Robust.Client.Graphics;
using Robust.Client.Input;
using Robust.Client.ResourceManagement;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Numerics;

#nullable enable
namespace Content.Client.Atmos.Overlays;

public sealed class AtmosDebugOverlay : Overlay
{
  [Dependency]
  private IEntityManager _entManager;
  [Dependency]
  private IMapManager _mapManager;
  [Dependency]
  private IInputManager _input;
  [Dependency]
  private IUserInterfaceManager _ui;
  [Dependency]
  private IResourceCache _cache;
  private readonly SharedTransformSystem _transform;
  private readonly AtmosDebugOverlaySystem _system;
  private readonly SharedMapSystem _map;
  private readonly Font _font;
  private List<(Entity<MapGridComponent>, SharedAtmosDebugOverlaySystem.AtmosDebugOverlayMessage)> _grids = new List<(Entity<MapGridComponent>, SharedAtmosDebugOverlaySystem.AtmosDebugOverlayMessage)>();

  public virtual OverlaySpace Space => (OverlaySpace) 6;

  internal AtmosDebugOverlay(AtmosDebugOverlaySystem system)
  {
    IoCManager.InjectDependencies<AtmosDebugOverlay>(this);
    this._system = system;
    this._transform = this._entManager.System<SharedTransformSystem>();
    this._map = this._entManager.System<SharedMapSystem>();
    this._font = this._cache.GetFont("/Fonts/NotoSans/NotoSans-Regular.ttf", 12);
  }

  protected virtual void Draw(in OverlayDrawArgs args)
  {
    if (args.Space == 2)
    {
      this.DrawTooltip(in args);
    }
    else
    {
      DrawingHandleWorld worldHandle = ((OverlayDrawArgs) ref args).WorldHandle;
      this.GetGrids(args.MapId, args.WorldBounds);
      foreach ((Entity<MapGridComponent> entity, SharedAtmosDebugOverlaySystem.AtmosDebugOverlayMessage msg) in this._grids)
      {
        DrawingHandleWorld drawingHandleWorld = worldHandle;
        Matrix3x2 worldMatrix = this._transform.GetWorldMatrix(Entity<MapGridComponent>.op_Implicit(entity));
        ref Matrix3x2 local = ref worldMatrix;
        ((DrawingHandleBase) drawingHandleWorld).SetTransform(ref local);
        this.DrawData(msg, worldHandle);
      }
      DrawingHandleWorld drawingHandleWorld1 = worldHandle;
      Matrix3x2 identity = Matrix3x2.Identity;
      ref Matrix3x2 local1 = ref identity;
      ((DrawingHandleBase) drawingHandleWorld1).SetTransform(ref local1);
    }
  }

  private void DrawData(
    SharedAtmosDebugOverlaySystem.AtmosDebugOverlayMessage msg,
    DrawingHandleWorld handle)
  {
    foreach (SharedAtmosDebugOverlaySystem.AtmosDebugOverlayData? nullable in msg.OverlayData)
    {
      if (nullable.HasValue)
        this.DrawGridTile(nullable.Value, handle);
    }
  }

  private void DrawGridTile(
    SharedAtmosDebugOverlaySystem.AtmosDebugOverlayData data,
    DrawingHandleWorld handle)
  {
    this.DrawFill(data, handle);
    this.DrawBlocked(data, handle);
  }

  private void DrawFill(
    SharedAtmosDebugOverlaySystem.AtmosDebugOverlayData data,
    DrawingHandleWorld handle)
  {
    Vector2 indices = data.Indices;
    float num = (this.GetFillData(data) - this._system.CfgBase) / this._system.CfgScale;
    Color color1 = !this._system.CfgCBM ? ((double) num < 0.5 ? Color.InterpolateBetween(Color.Red, Color.LimeGreen, num * 2f) : Color.InterpolateBetween(Color.LimeGreen, Color.Blue, (float) (((double) num - 0.5) * 2.0))) : Color.InterpolateBetween(Color.Black, Color.White, num);
    Color color2 = ((Color) ref color1).WithAlpha(0.75f);
    handle.DrawRect(Box2.FromDimensions(new Vector2(indices.X, indices.Y), new Vector2(1f, 1f)), color2, true);
  }

  private float GetFillData(
    SharedAtmosDebugOverlaySystem.AtmosDebugOverlayData data)
  {
    if (data.Moles == null)
      return 0.0f;
    switch (this._system.CfgMode)
    {
      case AtmosDebugOverlayMode.TotalMoles:
        float fillData = 0.0f;
        foreach (float mole in data.Moles)
          fillData += mole;
        return fillData;
      case AtmosDebugOverlayMode.GasMoles:
        return data.Moles[this._system.CfgSpecificGas];
      default:
        return data.Temperature;
    }
  }

  private void DrawBlocked(
    SharedAtmosDebugOverlaySystem.AtmosDebugOverlayData data,
    DrawingHandleWorld handle)
  {
    Vector2 indices = data.Indices;
    Vector2 vector2_1 = indices + 0.5f * Vector2.One;
    this.CheckAndShowBlockDir(data, handle, AtmosDirection.North, vector2_1);
    this.CheckAndShowBlockDir(data, handle, AtmosDirection.South, vector2_1);
    this.CheckAndShowBlockDir(data, handle, AtmosDirection.East, vector2_1);
    this.CheckAndShowBlockDir(data, handle, AtmosDirection.West, vector2_1);
    if (data.PressureDirection != AtmosDirection.Invalid)
      this.DrawPressureDirection(handle, data.PressureDirection, vector2_1, Color.Blue);
    else if (data.LastPressureDirection != AtmosDirection.Invalid)
      this.DrawPressureDirection(handle, data.LastPressureDirection, vector2_1, Color.LightGray);
    int? inExcitedGroup = data.InExcitedGroup;
    if (inExcitedGroup.HasValue)
    {
      int valueOrDefault = inExcitedGroup.GetValueOrDefault();
      Vector2 vector2_2 = indices;
      Vector2 vector2_3 = indices + new Vector2(1f, 1f);
      Vector2 vector2_4 = indices + new Vector2(0.0f, 1f);
      Vector2 vector2_5 = indices + new Vector2(1f, 0.0f);
      Color white = Color.White;
      Color color1 = ((Color) ref white).WithRed((float) (valueOrDefault & 15));
      color1 = ((Color) ref color1).WithGreen((float) ((valueOrDefault & 240 /*0xF0*/) >> 4));
      Color color2 = ((Color) ref color1).WithBlue((float) ((valueOrDefault & 3840 /*0x0F00*/) >> 8));
      ((DrawingHandleBase) handle).DrawLine(vector2_2, vector2_3, color2);
      ((DrawingHandleBase) handle).DrawLine(vector2_4, vector2_5, color2);
    }
    if (data.IsSpace)
      ((DrawingHandleBase) handle).DrawCircle(vector2_1, 0.15f, Color.Yellow, true);
    if (data.MapAtmosphere)
      ((DrawingHandleBase) handle).DrawCircle(vector2_1, 0.1f, Color.Orange, true);
    if (!data.NoGrid)
      return;
    ((DrawingHandleBase) handle).DrawCircle(vector2_1, 0.05f, Color.Black, true);
  }

  private void CheckAndShowBlockDir(
    SharedAtmosDebugOverlaySystem.AtmosDebugOverlayData data,
    DrawingHandleWorld handle,
    AtmosDirection dir,
    Vector2 tileCentre)
  {
    if (!data.BlockDirection.HasFlag((Enum) dir))
      return;
    Angle angle = Angle.op_Subtraction(dir.ToAngle(), Angle.FromDegrees(90.0));
    Vector2 vector2_1 = ((Angle) ref angle).ToVec() * 0.45f;
    Vector2 vector2_2 = new Vector2(vector2_1.Y, -vector2_1.X);
    Vector2 vector2_3 = tileCentre + vector2_1 - vector2_2;
    Vector2 vector2_4 = tileCentre + vector2_1 + vector2_2;
    ((DrawingHandleBase) handle).DrawLine(vector2_3, vector2_4, Color.Azure);
  }

  private void DrawPressureDirection(
    DrawingHandleWorld handle,
    AtmosDirection d,
    Vector2 center,
    Color color)
  {
    Angle angle = Angle.op_Subtraction(d.ToAngle(), Angle.FromDegrees(90.0));
    Vector2 vector2 = ((Angle) ref angle).ToVec() * 0.4f;
    ((DrawingHandleBase) handle).DrawLine(center, center + vector2, color);
  }

  private void DrawTooltip(in OverlayDrawArgs args)
  {
    DrawingHandleScreen screenHandle = ((OverlayDrawArgs) ref args).ScreenHandle;
    ScreenCoordinates mouseScreenPosition = this._input.MouseScreenPosition;
    if (!((ScreenCoordinates) ref mouseScreenPosition).IsValid || !(this._ui.MouseGetControl(mouseScreenPosition) is IViewportControl control))
      return;
    MapCoordinates map = control.PixelToMap(mouseScreenPosition.Position);
    Box2 box2 = Box2.CenteredAround(map.Position, 3f * Vector2.One);
    this.GetGrids(map.MapId, new Box2Rotated(box2));
    foreach ((Entity<MapGridComponent> entity, SharedAtmosDebugOverlaySystem.AtmosDebugOverlayMessage debugOverlayMessage) in this._grids)
    {
      Vector2i tile = this._map.WorldToTile(Entity<MapGridComponent>.op_Implicit(entity), Entity<MapGridComponent>.op_Implicit(entity), map.Position);
      foreach (SharedAtmosDebugOverlaySystem.AtmosDebugOverlayData? nullable in debugOverlayMessage.OverlayData)
      {
        Vector2? indices = nullable?.Indices;
        Vector2 vector2 = Vector2i.op_Implicit(tile);
        if ((indices.HasValue ? (indices.GetValueOrDefault() == vector2 ? 1 : 0) : 0) != 0)
        {
          this.DrawTooltip(screenHandle, mouseScreenPosition.Position, nullable.Value);
          return;
        }
      }
    }
  }

  private void DrawTooltip(
    DrawingHandleScreen handle,
    Vector2 pos,
    SharedAtmosDebugOverlaySystem.AtmosDebugOverlayData data)
  {
    Vector2 vector2_1 = new Vector2(0.0f, (float) this._font.GetLineHeight(1f));
    string str1 = data.Moles == null ? "No Air" : ((IEnumerable<float>) data.Moles).Sum().ToString((IFormatProvider) CultureInfo.InvariantCulture);
    handle.DrawString(this._font, pos, "Moles: " + str1);
    pos += vector2_1;
    handle.DrawString(this._font, pos, $"Temp: {data.Temperature}");
    pos += vector2_1;
    DrawingHandleScreen drawingHandleScreen = handle;
    Font font = this._font;
    Vector2 vector2_2 = pos;
    int? inExcitedGroup = data.InExcitedGroup;
    ref int? local = ref inExcitedGroup;
    string str2 = "Excited: " + ((local.HasValue ? local.GetValueOrDefault().ToString() : (string) null) ?? "None");
    drawingHandleScreen.DrawString(font, vector2_2, str2);
    pos += vector2_1;
    handle.DrawString(this._font, pos, $"Space: {data.IsSpace}");
    pos += vector2_1;
    handle.DrawString(this._font, pos, $"Map: {data.MapAtmosphere}");
    pos += vector2_1;
    handle.DrawString(this._font, pos, $"NoGrid: {data.NoGrid}");
    pos += vector2_1;
    handle.DrawString(this._font, pos, $"Immutable: {data.Immutable}");
  }

  private void GetGrids(MapId mapId, Box2Rotated box)
  {
    this._grids.Clear();
    // ISSUE: method pointer
    this._mapManager.FindGridsIntersecting<List<(Entity<MapGridComponent>, SharedAtmosDebugOverlaySystem.AtmosDebugOverlayMessage)>>(mapId, box, ref this._grids, new GridCallback<List<(Entity<MapGridComponent>, SharedAtmosDebugOverlaySystem.AtmosDebugOverlayMessage)>>((object) this, __methodptr(\u003CGetGrids\u003Eb__23_0)), false, true);
  }
}
