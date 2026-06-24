// Decompiled with JetBrains decompiler
// Type: Content.Client.Pinpointer.UI.NavMapControl
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.Stylesheets;
using Content.Client.UserInterface.Controls;
using Content.Shared.Atmos;
using Content.Shared.Input;
using Content.Shared.Pinpointer;
using Robust.Client.Graphics;
using Robust.Client.ResourceManagement;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Analyzers;
using Robust.Shared.Collections;
using Robust.Shared.GameObjects;
using Robust.Shared.Input;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;
using Robust.Shared.Physics;
using Robust.Shared.Physics.Collision.Shapes;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Dynamics;
using Robust.Shared.Timing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Client.Pinpointer.UI;

[Virtual]
public class NavMapControl : MapGridControl
{
  [Dependency]
  private IResourceCache _cache;
  private readonly SharedTransformSystem _transformSystem;
  private readonly SharedNavMapSystem _navMapSystem;
  public EntityUid? Owner;
  public EntityUid? MapUid;
  public Dictionary<EntityCoordinates, (bool Visible, Color Color)> TrackedCoordinates;
  public Dictionary<NetEntity, NavMapBlip> TrackedEntities;
  public List<(Vector2, Vector2)> TileLines;
  public List<(Vector2, Vector2)> TileRects;
  public List<(Vector2[], Color)> TilePolygons;
  public List<NavMapRegionOverlay> RegionOverlays;
  public Color WallColor;
  public Color TileColor;
  protected float UpdateTime;
  protected float MaxSelectableDistance;
  protected float MinDragDistance;
  protected static float MinDisplayedRange = 8f;
  protected static float MaxDisplayedRange = 128f;
  protected static float DefaultDisplayedRange = 48f;
  protected float MinmapScaleModifier;
  protected float FullWallInstep;
  protected float ThinWallThickness;
  protected float ThinDoorThickness;
  private float _updateTimer;
  private Dictionary<Color, Color> _sRGBLookUp;
  protected Color BackgroundColor;
  protected float BackgroundOpacity;
  private int _targetFontsize;
  private Dictionary<Vector2i, Vector2i> _horizLines;
  private Dictionary<Vector2i, Vector2i> _horizLinesReversed;
  private Dictionary<Vector2i, Vector2i> _vertLines;
  private Dictionary<Vector2i, Vector2i> _vertLinesReversed;
  private NavMapComponent? _navMap;
  private MapGridComponent? _grid;
  private TransformComponent? _xform;
  private PhysicsComponent? _physics;
  private FixturesComponent? _fixtures;
  private readonly Label _zoom;
  private readonly Button _recenter;
  private readonly CheckBox _beacons;

  protected override bool Draggable => true;

  public event Action<NetEntity?>? TrackedEntitySelectedAction;

  public event Action<DrawingHandleScreen>? PostWallDrawingAction;

  public NavMapControl()
  {
    Label label = new Label();
    ((Control) label).VerticalAlignment = (Control.VAlignment) 1;
    ((Control) label).HorizontalExpand = true;
    ((Control) label).Margin = new Thickness(8f, 8f);
    this._zoom = label;
    Button button = new Button();
    button.Text = Loc.GetString("navmap-recenter");
    ((Control) button).VerticalAlignment = (Control.VAlignment) 1;
    ((Control) button).HorizontalAlignment = (Control.HAlignment) 3;
    ((Control) button).HorizontalExpand = true;
    ((Control) button).Margin = new Thickness(8f, 4f);
    ((BaseButton) button).Disabled = true;
    this._recenter = button;
    CheckBox checkBox = new CheckBox();
    checkBox.Text = Loc.GetString("navmap-toggle-beacons");
    ((Control) checkBox).VerticalAlignment = (Control.VAlignment) 2;
    ((Control) checkBox).HorizontalAlignment = (Control.HAlignment) 2;
    ((Control) checkBox).HorizontalExpand = true;
    ((Control) checkBox).Margin = new Thickness(4f, 0.0f);
    ((BaseButton) checkBox).Pressed = true;
    this._beacons = checkBox;
    // ISSUE: explicit constructor call
    base.\u002Ector(NavMapControl.MinDisplayedRange, NavMapControl.MaxDisplayedRange, NavMapControl.DefaultDisplayedRange);
    IoCManager.InjectDependencies<NavMapControl>(this);
    this._transformSystem = this.EntManager.System<SharedTransformSystem>();
    this._navMapSystem = this.EntManager.System<SharedNavMapSystem>();
    this.BackgroundColor = Color.FromSrgb(((Color) ref this.TileColor).WithAlpha(this.BackgroundOpacity));
    ((Control) this).RectClipContent = true;
    ((Control) this).HorizontalExpand = true;
    ((Control) this).VerticalExpand = true;
    PanelContainer panelContainer1 = new PanelContainer();
    panelContainer1.PanelOverride = (StyleBox) new StyleBoxFlat()
    {
      BackgroundColor = ((Color) ref StyleNano.ButtonColorContext).WithAlpha(1f),
      BorderColor = StyleNano.PanelDark
    };
    ((Control) panelContainer1).VerticalExpand = false;
    ((Control) panelContainer1).HorizontalExpand = true;
    ((Control) panelContainer1).SetWidth = 650f;
    Control.OrderedChildCollection children = ((Control) panelContainer1).Children;
    BoxContainer boxContainer1 = new BoxContainer();
    boxContainer1.Orientation = (BoxContainer.LayoutOrientation) 0;
    ((Control) boxContainer1).Children.Add((Control) this._zoom);
    ((Control) boxContainer1).Children.Add((Control) this._beacons);
    ((Control) boxContainer1).Children.Add((Control) this._recenter);
    children.Add((Control) boxContainer1);
    PanelContainer panelContainer2 = panelContainer1;
    BoxContainer boxContainer2 = new BoxContainer();
    boxContainer2.Orientation = (BoxContainer.LayoutOrientation) 1;
    ((Control) boxContainer2).HorizontalExpand = true;
    ((Control) boxContainer2).Children.Add((Control) panelContainer2);
    ((Control) boxContainer2).Children.Add(new Control()
    {
      Name = "DrawingControl",
      VerticalExpand = true,
      Margin = new Thickness(5f, 5f)
    });
    ((Control) this).AddChild((Control) boxContainer2);
    ((Control) panelContainer2).Measure(Vector2Helpers.Infinity);
    ((BaseButton) this._recenter).OnPressed += (Action<BaseButton.ButtonEventArgs>) (args => this.Recentering = true);
    this.ForceNavMapUpdate();
  }

  public void ForceNavMapUpdate()
  {
    this.EntManager.TryGetComponent<NavMapComponent>(this.MapUid, ref this._navMap);
    this.EntManager.TryGetComponent<MapGridComponent>(this.MapUid, ref this._grid);
    this.EntManager.TryGetComponent<TransformComponent>(this.MapUid, ref this._xform);
    this.EntManager.TryGetComponent<PhysicsComponent>(this.MapUid, ref this._physics);
    this.EntManager.TryGetComponent<FixturesComponent>(this.MapUid, ref this._fixtures);
    this.UpdateNavMap();
  }

  public void CenterToCoordinates(EntityCoordinates coordinates)
  {
    if (this._physics != null)
      this.Offset = new Vector2(((EntityCoordinates) ref coordinates).X, ((EntityCoordinates) ref coordinates).Y) - this._physics.LocalCenter;
    ((BaseButton) this._recenter).Disabled = false;
  }

  protected override void KeyBindUp(GUIBoundKeyEventArgs args)
  {
    base.KeyBindUp(args);
    if (BoundKeyFunction.op_Equality(((BoundKeyEventArgs) args).Function, EngineKeyFunctions.UIClick))
    {
      if (this.TrackedEntitySelectedAction == null || this._xform == null || this._physics == null || this.TrackedEntities.Count == 0)
        return;
      Vector2 vector2_1 = this.StartDragPosition - ((BoundKeyEventArgs) args).PointerLocation.Position;
      if ((double) vector2_1.Length() > (double) this.MinDragDistance)
        return;
      Vector2 vector2_2 = this.Offset + this._physics.LocalCenter;
      Vector2 vector2_3 = (((BoundKeyEventArgs) args).PointerLocation.Position - Vector2i.op_Implicit(((Control) this).GlobalPixelPosition) - this.MidPointVector) / this.MinimapScale;
      Vector2 vector2_4 = Vector2.Transform(new Vector2(vector2_3.X, -vector2_3.Y) + vector2_2, this._transformSystem.GetWorldMatrix(this._xform));
      NetEntity netEntity = NetEntity.Invalid;
      float num1 = float.PositiveInfinity;
      foreach ((NetEntity key, NavMapBlip navMapBlip) in this.TrackedEntities)
      {
        if (navMapBlip.Selectable)
        {
          vector2_1 = this._transformSystem.ToMapCoordinates(navMapBlip.Coordinates, true).Position - vector2_4;
          float num2 = vector2_1.Length();
          if ((double) num1 >= (double) num2 && (double) num2 * (double) this.MinimapScale <= (double) this.MaxSelectableDistance)
          {
            netEntity = key;
            num1 = num2;
          }
        }
      }
      if ((double) num1 > (double) this.MaxSelectableDistance || !((NetEntity) ref netEntity).IsValid())
        return;
      this.TrackedEntitySelectedAction(new NetEntity?(netEntity));
    }
    else if (BoundKeyFunction.op_Equality(((BoundKeyEventArgs) args).Function, EngineKeyFunctions.UIRightClick))
    {
      Action<NetEntity?> entitySelectedAction = this.TrackedEntitySelectedAction;
      if (entitySelectedAction == null)
        return;
      entitySelectedAction(new NetEntity?());
    }
    else
    {
      if (!BoundKeyFunction.op_Equality(((BoundKeyEventArgs) args).Function, ContentKeyFunctions.ExamineEntity))
        return;
      ((BaseButton) this._beacons).Pressed = !((BaseButton) this._beacons).Pressed;
    }
  }

  protected override void MouseMove(GUIMouseMoveEventArgs args)
  {
    base.MouseMove(args);
    if (this.Offset != Vector2.Zero)
      ((BaseButton) this._recenter).Disabled = false;
    else
      ((BaseButton) this._recenter).Disabled = true;
  }

  protected override void Draw(DrawingHandleScreen handle)
  {
    base.Draw(handle);
    this.EntManager.TryGetComponent<NavMapComponent>(this.MapUid, ref this._navMap);
    this.EntManager.TryGetComponent<MapGridComponent>(this.MapUid, ref this._grid);
    this.EntManager.TryGetComponent<TransformComponent>(this.MapUid, ref this._xform);
    this.EntManager.TryGetComponent<PhysicsComponent>(this.MapUid, ref this._physics);
    this.EntManager.TryGetComponent<FixturesComponent>(this.MapUid, ref this._fixtures);
    if (this._navMap == null || this._grid == null || this._xform == null)
      return;
    ((BaseButton) this._recenter).Disabled = this.DrawRecenter();
    this._zoom.Text = Loc.GetString("navmap-zoom", new (string, object)[1]
    {
      ("value", (object) $"{NavMapControl.DefaultDisplayedRange / this.WorldRange:0.0}")
    });
    Vector2 offset = this.Offset;
    if (this._physics != null)
      offset += this._physics.LocalCenter;
    Vector2 vector2_1 = new Vector2(offset.X, -offset.Y);
    Color srgb;
    if (!this._sRGBLookUp.TryGetValue(this.WallColor, out srgb))
    {
      srgb = Color.ToSrgb(this.WallColor);
      this._sRGBLookUp[this.WallColor] = srgb;
    }
    if (this.TilePolygons.Any<(Vector2[], Color)>())
    {
      Span<Vector2> span = (Span<Vector2>) new Vector2[8];
      foreach ((Vector2[] vector2Array, Color color) in this.TilePolygons)
      {
        for (int index = 0; index < vector2Array.Length; ++index)
        {
          Vector2 vector2_2 = vector2Array[index] - offset;
          span[index] = this.ScalePosition(new Vector2(vector2_2.X, -vector2_2.Y));
        }
        ((DrawingHandleBase) handle).DrawPrimitives((DrawPrimitiveTopology) 2, (ReadOnlySpan<Vector2>) span.Slice(0, vector2Array.Length), color);
      }
    }
    if (this._grid != null)
    {
      foreach (NavMapRegionOverlay regionOverlay in this.RegionOverlays)
      {
        foreach ((Vector2i, Vector2i) gridCoord in regionOverlay.GridCoords)
        {
          Vector2 vector2_3 = this.ScalePosition(new Vector2((float) gridCoord.Item1.X, (float) -gridCoord.Item1.Y) - new Vector2(offset.X, -offset.Y));
          Vector2 vector2_4 = this.ScalePosition(new Vector2((float) (gridCoord.Item2.X + (int) this._grid.TileSize), (float) (-gridCoord.Item2.Y - (int) this._grid.TileSize)) - new Vector2(offset.X, -offset.Y));
          UIBox2 uiBox2;
          // ISSUE: explicit constructor call
          ((UIBox2) ref uiBox2).\u002Ector(vector2_3, vector2_4);
          handle.DrawRect(uiBox2, regionOverlay.Color, true);
        }
      }
    }
    if (this.TileLines.Any<(Vector2, Vector2)>())
    {
      ValueList<Vector2> valueList = new ValueList<Vector2>(this.TileLines.Count * 2);
      foreach ((Vector2 vector2_5, Vector2 vector2_6) in this.TileLines)
      {
        Vector2 vector2_7 = this.ScalePosition(vector2_5 - vector2_1);
        Vector2 vector2_8 = this.ScalePosition(vector2_6 - vector2_1);
        valueList.Add(vector2_7);
        valueList.Add(vector2_8);
      }
      if (valueList.Count > 0)
        ((DrawingHandleBase) handle).DrawPrimitives((DrawPrimitiveTopology) 4, (ReadOnlySpan<Vector2>) valueList.Span, srgb);
    }
    if (this.TileRects.Any<(Vector2, Vector2)>())
    {
      ValueList<Vector2> valueList = new ValueList<Vector2>(this.TileRects.Count * 8);
      foreach ((Vector2 vector2_9, Vector2 vector2_10) in this.TileRects)
      {
        Vector2 vector2_11 = this.ScalePosition(vector2_9 - vector2_1);
        Vector2 vector2_12 = this.ScalePosition(vector2_10 - vector2_1);
        Vector2 vector2_13 = new Vector2(vector2_12.X, vector2_11.Y);
        Vector2 vector2_14 = new Vector2(vector2_11.X, vector2_12.Y);
        valueList.Add(vector2_11);
        valueList.Add(vector2_13);
        valueList.Add(vector2_13);
        valueList.Add(vector2_12);
        valueList.Add(vector2_12);
        valueList.Add(vector2_14);
        valueList.Add(vector2_14);
        valueList.Add(vector2_11);
      }
      if (valueList.Count > 0)
        ((DrawingHandleBase) handle).DrawPrimitives((DrawPrimitiveTopology) 4, (ReadOnlySpan<Vector2>) valueList.Span, srgb);
    }
    if (this.PostWallDrawingAction != null)
      this.PostWallDrawingAction(handle);
    TimeSpan realTime = this.Timing.RealTime;
    float num1 = 1f;
    bool flag = realTime.TotalSeconds % (double) num1 > (double) num1 / 2.0;
    foreach ((EntityCoordinates key, (bool Visible, Color Color) tuple) in this.TrackedCoordinates)
    {
      if (flag && tuple.Visible)
      {
        MapCoordinates mapCoordinates = this._transformSystem.ToMapCoordinates(key, true);
        if (MapId.op_Inequality(mapCoordinates.MapId, MapId.Nullspace))
        {
          Vector2 vector2_15 = Vector2.Transform(mapCoordinates.Position, this._transformSystem.GetInvWorldMatrix(this._xform)) - offset;
          Vector2 vector2_16 = this.ScalePosition(new Vector2(vector2_15.X, -vector2_15.Y));
          ((DrawingHandleBase) handle).DrawCircle(vector2_16, float.Sqrt(this.MinimapScale) * 2f, tuple.Color, true);
        }
      }
    }
    foreach (NavMapBlip navMapBlip in this.TrackedEntities.Values)
    {
      if ((!navMapBlip.Blinks || flag) && navMapBlip.Texture != null)
      {
        MapCoordinates mapCoordinates = this._transformSystem.ToMapCoordinates(navMapBlip.Coordinates, true);
        if (MapId.op_Inequality(mapCoordinates.MapId, MapId.Nullspace))
        {
          Vector2 vector2_17 = Vector2.Transform(mapCoordinates.Position, this._transformSystem.GetInvWorldMatrix(this._xform)) - offset;
          Vector2 vector2_18 = this.ScalePosition(new Vector2(vector2_17.X, -vector2_17.Y));
          float num2 = this.MinmapScaleModifier * float.Sqrt(this.MinimapScale);
          Vector2 vector2_19 = new Vector2(num2 * navMapBlip.Scale * (float) navMapBlip.Texture.Width, num2 * navMapBlip.Scale * (float) navMapBlip.Texture.Height);
          handle.DrawTextureRect(navMapBlip.Texture, new UIBox2(vector2_18 - vector2_19, vector2_18 + vector2_19), new Color?(navMapBlip.Color));
        }
      }
    }
    if (!((BaseButton) this._beacons).Pressed)
      return;
    Vector2 vector2_20 = new Vector2(5f, 3f);
    int num3 = (int) Math.Round(1.0 / (double) this.WorldRange * (double) NavMapControl.DefaultDisplayedRange * (double) ((Control) this).UIScale * (double) this._targetFontsize, 0);
    VectorFont vectorFont = new VectorFont(this._cache.GetResource<FontResource>("/Fonts/NotoSans/NotoSans-Bold.ttf", true), num3);
    foreach (SharedNavMapSystem.NavMapBeacon navMapBeacon in this._navMap.Beacons.Values)
    {
      Vector2 vector2_21 = navMapBeacon.Position - offset;
      Vector2 vector2_22 = this.ScalePosition(vector2_21 with
      {
        Y = -vector2_21.Y
      });
      Vector2 dimensions = handle.GetDimensions((Font) vectorFont, (ReadOnlySpan<char>) navMapBeacon.Text, 1f);
      handle.DrawRect(new UIBox2(vector2_22 - dimensions / 2f - vector2_20, vector2_22 + dimensions / 2f + vector2_20), this.BackgroundColor, true);
      handle.DrawString((Font) vectorFont, vector2_22 - dimensions / 2f, navMapBeacon.Text, navMapBeacon.Color);
    }
  }

  protected virtual void FrameUpdate(FrameEventArgs args)
  {
    this._updateTimer += ((FrameEventArgs) ref args).DeltaSeconds;
    if ((double) this._updateTimer < (double) this.UpdateTime)
      return;
    this._updateTimer -= this.UpdateTime;
    this.UpdateNavMap();
  }

  protected virtual void UpdateNavMap()
  {
    this.TilePolygons.Clear();
    this.TileLines.Clear();
    this.TileRects.Clear();
    this.UpdateNavMapFloorTiles();
    this.UpdateNavMapWallLines();
    this.UpdateNavMapAirlocks();
  }

  private void UpdateNavMapFloorTiles()
  {
    if (this._fixtures == null)
      return;
    Vector2[] array = new Vector2[8];
    foreach (Fixture fixture in this._fixtures.Fixtures.Values)
    {
      if (fixture.Shape is PolygonShape shape)
      {
        for (int index = 0; index < shape.VertexCount; ++index)
        {
          Vector2 vertex = shape.Vertices[index];
          array[index] = new Vector2(MathF.Round(vertex.X), MathF.Round(vertex.Y));
        }
        this.TilePolygons.Add((RuntimeHelpers.GetSubArray<Vector2>(array, Range.EndAt((Index) shape.VertexCount)), this.TileColor));
      }
    }
  }

  private void UpdateNavMapWallLines()
  {
    if (this._navMap == null || this._grid == null)
      return;
    this._horizLines.Clear();
    this._horizLinesReversed.Clear();
    this._vertLines.Clear();
    this._vertLinesReversed.Clear();
    foreach ((Vector2i key5, NavMapChunk navMapChunk1) in this._navMap.Chunks)
    {
      Vector2i vector2i1 = key5;
      NavMapChunk navMapChunk2 = navMapChunk1;
      for (int index = 0; index < 64 /*0x40*/; ++index)
      {
        int num1 = navMapChunk2.TileData[index] & 240 /*0xF0*/;
        if (num1 != 0)
        {
          int tileData = num1 >> 4;
          Vector2i tileFromIndex = SharedNavMapSystem.GetTileFromIndex(index);
          Vector2i tile = Vector2i.op_Multiply(Vector2i.op_Addition(Vector2i.op_Multiply(navMapChunk2.Origin, 8), tileFromIndex), (int) this._grid.TileSize);
          if (tileData != 15)
          {
            this.AddRectForThinWall(tileData, tile);
          }
          else
          {
            key5 = tile;
            key5.Y = -tile.Y;
            Vector2i vector2i2 = key5;
            int num2 = 0;
            NavMapChunk navMapChunk3;
            if (tileFromIndex.Y != 7)
              num2 = navMapChunk2.TileData[index + 1];
            else if (this._navMap.Chunks.TryGetValue(Vector2i.op_Addition(vector2i1, Vector2i.Up), out navMapChunk3))
              num2 = navMapChunk3.TileData[index + 1 - 8];
            if ((num2 & 32 /*0x20*/) == 0)
              this.AddOrUpdateNavMapLine(Vector2i.op_Addition(vector2i2, new Vector2i(0, (int) -this._grid.TileSize)), Vector2i.op_Addition(vector2i2, new Vector2i((int) this._grid.TileSize, (int) -this._grid.TileSize)), this._horizLines, this._horizLinesReversed);
            int num3 = 0;
            if (tileFromIndex.X != 7)
              num3 = navMapChunk2.TileData[index + 8];
            else if (this._navMap.Chunks.TryGetValue(Vector2i.op_Addition(vector2i1, Vector2i.Right), out navMapChunk3))
              num3 = navMapChunk3.TileData[index + 8 - 64 /*0x40*/];
            if ((num3 & 128 /*0x80*/) == 0)
              this.AddOrUpdateNavMapLine(Vector2i.op_Addition(vector2i2, new Vector2i((int) this._grid.TileSize, (int) -this._grid.TileSize)), Vector2i.op_Addition(vector2i2, new Vector2i((int) this._grid.TileSize, 0)), this._vertLines, this._vertLinesReversed);
            int num4 = 0;
            if (tileFromIndex.Y != 0)
              num4 = navMapChunk2.TileData[index - 1];
            else if (this._navMap.Chunks.TryGetValue(Vector2i.op_Addition(vector2i1, Vector2i.Down), out navMapChunk3))
              num4 = navMapChunk3.TileData[index - 1 + 8];
            if ((num4 & 16 /*0x10*/) == 0)
              this.AddOrUpdateNavMapLine(vector2i2, Vector2i.op_Addition(vector2i2, new Vector2i((int) this._grid.TileSize, 0)), this._horizLines, this._horizLinesReversed);
            int num5 = 0;
            if (tileFromIndex.X != 0)
              num5 = navMapChunk2.TileData[index - 8];
            else if (this._navMap.Chunks.TryGetValue(Vector2i.op_Addition(vector2i1, Vector2i.Left), out navMapChunk3))
              num5 = navMapChunk3.TileData[index - 8 + 64 /*0x40*/];
            if ((num5 & 64 /*0x40*/) == 0)
              this.AddOrUpdateNavMapLine(Vector2i.op_Addition(vector2i2, new Vector2i(0, (int) -this._grid.TileSize)), vector2i2, this._vertLines, this._vertLinesReversed);
            this.TileLines.Add((Vector2i.op_Implicit(vector2i2) + new Vector2(0.0f, (float) -this._grid.TileSize), Vector2i.op_Implicit(vector2i2) + new Vector2((float) this._grid.TileSize, 0.0f)));
          }
        }
      }
    }
    Vector2i key4;
    foreach ((key5, key4) in this._horizLines)
    {
      Vector2i vector2i3 = key5;
      Vector2i vector2i4 = key4;
      this.TileLines.Add((Vector2i.op_Implicit(vector2i3), Vector2i.op_Implicit(vector2i4)));
    }
    foreach ((key4, key5) in this._vertLines)
    {
      Vector2i vector2i5 = key4;
      Vector2i vector2i6 = key5;
      this.TileLines.Add((Vector2i.op_Implicit(vector2i5), Vector2i.op_Implicit(vector2i6)));
    }
  }

  private void UpdateNavMapAirlocks()
  {
    if (this._navMap == null || this._grid == null)
      return;
    foreach (NavMapChunk navMapChunk in this._navMap.Chunks.Values)
    {
      for (int index = 0; index < 64 /*0x40*/; ++index)
      {
        int num = navMapChunk.TileData[index] & 3840 /*0x0F00*/;
        if (num != 0)
        {
          int tileData = num >> 8;
          Vector2i tileFromIndex = SharedNavMapSystem.GetTileFromIndex(index);
          Vector2i tile = Vector2i.op_Multiply(Vector2i.op_Addition(Vector2i.op_Multiply(navMapChunk.Origin, 8), tileFromIndex), (int) this._grid.TileSize);
          if (tileData != 15)
          {
            this.AddRectForThinAirlock(tileData, tile);
          }
          else
          {
            this.TileRects.Add((new Vector2((float) tile.X + this.FullWallInstep, (float) -tile.Y - this.FullWallInstep), new Vector2((float) ((double) tile.X - (double) this.FullWallInstep + 1.0), (float) ((double) -tile.Y + (double) this.FullWallInstep - 1.0))));
            this.TileLines.Add((new Vector2((float) tile.X + 0.5f, (float) -tile.Y - this.FullWallInstep), new Vector2((float) tile.X + 0.5f, (float) ((double) -tile.Y + (double) this.FullWallInstep - 1.0))));
          }
        }
      }
    }
  }

  private void AddRectForThinWall(int tileData, Vector2i tile)
  {
    Vector2 vector2_1 = new Vector2(-0.5f, 0.5f - this.ThinWallThickness);
    Vector2 vector2_2 = new Vector2(0.5f, 0.5f);
    for (int index = 0; index < 4; ++index)
    {
      int direction = 1 << index;
      if ((tileData & direction) != 0)
      {
        Vector2 vector2_3 = new Vector2((float) tile.X + 0.5f, (float) -tile.Y - 0.5f);
        Angle angle = Angle.op_UnaryNegation(((AtmosDirection) direction).ToAngle());
        this.TileRects.Add((((Angle) ref angle).RotateVec(ref vector2_1) + vector2_3, ((Angle) ref angle).RotateVec(ref vector2_2) + vector2_3));
      }
    }
  }

  private void AddRectForThinAirlock(int tileData, Vector2i tile)
  {
    Vector2 vector2_1 = new Vector2(this.FullWallInstep - 0.5f, 0.5f - this.FullWallInstep - this.ThinDoorThickness);
    Vector2 vector2_2 = new Vector2(0.5f - this.FullWallInstep, 0.5f - this.FullWallInstep);
    Vector2 vector2_3 = new Vector2(0.0f, 0.5f - this.FullWallInstep - this.ThinDoorThickness);
    Vector2 vector2_4 = new Vector2(0.0f, 0.5f - this.FullWallInstep);
    for (int index = 0; index < 4; ++index)
    {
      int direction = 1 << index;
      if ((tileData & direction) != 0)
      {
        Vector2 vector2_5 = new Vector2((float) tile.X + 0.5f, (float) -tile.Y - 0.5f);
        Angle angle = Angle.op_UnaryNegation(((AtmosDirection) direction).ToAngle());
        this.TileRects.Add((((Angle) ref angle).RotateVec(ref vector2_1) + vector2_5, ((Angle) ref angle).RotateVec(ref vector2_2) + vector2_5));
        this.TileLines.Add((((Angle) ref angle).RotateVec(ref vector2_3) + vector2_5, ((Angle) ref angle).RotateVec(ref vector2_4) + vector2_5));
      }
    }
  }

  protected void AddOrUpdateNavMapLine(
    Vector2i origin,
    Vector2i terminus,
    Dictionary<Vector2i, Vector2i> lookup,
    Dictionary<Vector2i, Vector2i> lookupReversed)
  {
    if (Vector2i.op_Equality(origin, terminus))
      return;
    Vector2i key1;
    if (lookup.Remove(terminus, out key1))
    {
      Vector2i key2;
      if (lookupReversed.Remove(origin, out key2))
      {
        lookup[key2] = key1;
        lookupReversed[key1] = key2;
      }
      else
      {
        lookup[origin] = key1;
        lookupReversed[key1] = origin;
      }
    }
    else
    {
      Vector2i key3;
      if (lookupReversed.Remove(origin, out key3))
      {
        lookup[key3] = terminus;
        lookupReversed[terminus] = key3;
      }
      else
      {
        lookup.Add(origin, terminus);
        lookupReversed.Add(terminus, origin);
      }
    }
  }

  protected Vector2 GetOffset()
  {
    Vector2 offset = this.Offset;
    PhysicsComponent physics = this._physics;
    Vector2 vector2 = physics != null ? physics.LocalCenter : new Vector2();
    return offset + vector2;
  }
}
