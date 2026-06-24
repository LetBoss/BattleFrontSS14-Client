// Decompiled with JetBrains decompiler
// Type: Content.Client.Power.PowerMonitoringConsoleNavMapControl
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.Pinpointer.UI;
using Content.Shared.Power;
using Robust.Client.Graphics;
using Robust.Shared.Collections;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;
using System;
using System.Collections.Generic;
using System.Numerics;

#nullable enable
namespace Content.Client.Power;

public sealed class PowerMonitoringConsoleNavMapControl : NavMapControl
{
  [Dependency]
  private IEntityManager _entManager;
  private readonly Color[] _powerCableColors = new Color[3]
  {
    Color.OrangeRed,
    Color.Yellow,
    Color.LimeGreen
  };
  private readonly Vector2[] _powerCableOffsets = new Vector2[3]
  {
    new Vector2(-0.2f, -0.2f),
    Vector2.Zero,
    new Vector2(0.2f, 0.2f)
  };
  private Dictionary<Color, Color> _sRGBLookUp = new Dictionary<Color, Color>();
  public PowerMonitoringCableNetworksComponent? PowerMonitoringCableNetworks;
  public List<PowerMonitoringConsoleLineGroup> HiddenLineGroups = new List<PowerMonitoringConsoleLineGroup>();
  public List<PowerMonitoringConsoleLine> PowerCableNetwork = new List<PowerMonitoringConsoleLine>();
  public List<PowerMonitoringConsoleLine> FocusCableNetwork = new List<PowerMonitoringConsoleLine>();
  private Dictionary<Vector2i, Vector2i>[] _horizLines = new Dictionary<Vector2i, Vector2i>[3]
  {
    new Dictionary<Vector2i, Vector2i>(),
    new Dictionary<Vector2i, Vector2i>(),
    new Dictionary<Vector2i, Vector2i>()
  };
  private Dictionary<Vector2i, Vector2i>[] _horizLinesReversed = new Dictionary<Vector2i, Vector2i>[3]
  {
    new Dictionary<Vector2i, Vector2i>(),
    new Dictionary<Vector2i, Vector2i>(),
    new Dictionary<Vector2i, Vector2i>()
  };
  private Dictionary<Vector2i, Vector2i>[] _vertLines = new Dictionary<Vector2i, Vector2i>[3]
  {
    new Dictionary<Vector2i, Vector2i>(),
    new Dictionary<Vector2i, Vector2i>(),
    new Dictionary<Vector2i, Vector2i>()
  };
  private Dictionary<Vector2i, Vector2i>[] _vertLinesReversed = new Dictionary<Vector2i, Vector2i>[3]
  {
    new Dictionary<Vector2i, Vector2i>(),
    new Dictionary<Vector2i, Vector2i>(),
    new Dictionary<Vector2i, Vector2i>()
  };
  private MapGridComponent? _grid;

  public PowerMonitoringConsoleNavMapControl()
  {
    this.TileColor = new Color((byte) 30, (byte) 57, (byte) 67, byte.MaxValue);
    this.WallColor = new Color((byte) 102, (byte) 164, (byte) 217, byte.MaxValue);
    this.BackgroundColor = Color.FromSrgb(((Color) ref this.TileColor).WithAlpha(this.BackgroundOpacity));
    this.PostWallDrawingAction += new Action<DrawingHandleScreen>(this.DrawAllCableNetworks);
  }

  protected override void UpdateNavMap()
  {
    base.UpdateNavMap();
    PowerMonitoringCableNetworksComponent networksComponent;
    if (!this.Owner.HasValue || !this._entManager.TryGetComponent<PowerMonitoringCableNetworksComponent>(this.Owner, ref networksComponent))
      return;
    this.PowerCableNetwork = this.GetDecodedPowerCableChunks(networksComponent.AllChunks);
    this.FocusCableNetwork = this.GetDecodedPowerCableChunks(networksComponent.FocusChunks);
  }

  public void DrawAllCableNetworks(DrawingHandleScreen handle)
  {
    if (!this._entManager.TryGetComponent<MapGridComponent>(this.MapUid, ref this._grid))
      return;
    if (this.PowerCableNetwork != null && this.PowerCableNetwork.Count > 0)
    {
      Color modulator = this.FocusCableNetwork == null || this.FocusCableNetwork.Count <= 0 ? Color.White : Color.DimGray;
      this.DrawCableNetwork(handle, this.PowerCableNetwork, modulator);
    }
    if (this.FocusCableNetwork == null || this.FocusCableNetwork.Count <= 0)
      return;
    this.DrawCableNetwork(handle, this.FocusCableNetwork, Color.White);
  }

  public void DrawCableNetwork(
    DrawingHandleScreen handle,
    List<PowerMonitoringConsoleLine> fullCableNetwork,
    Color modulator)
  {
    if (!this._entManager.TryGetComponent<MapGridComponent>(this.MapUid, ref this._grid))
      return;
    Vector2 offset = this.GetOffset();
    Vector2 vector2_1 = offset with { Y = -offset.Y };
    if ((double) this.WorldRange / (double) this.WorldMaxRange > 0.5)
    {
      ValueList<Vector2>[] valueListArray = new ValueList<Vector2>[3];
      foreach (PowerMonitoringConsoleLine monitoringConsoleLine in fullCableNetwork)
      {
        if (!this.HiddenLineGroups.Contains(monitoringConsoleLine.Group))
        {
          Vector2 powerCableOffset = this._powerCableOffsets[(int) monitoringConsoleLine.Group];
          Vector2 vector2_2 = this.ScalePosition(monitoringConsoleLine.Origin + powerCableOffset - vector2_1);
          Vector2 vector2_3 = this.ScalePosition(monitoringConsoleLine.Terminus + powerCableOffset - vector2_1);
          valueListArray[(int) monitoringConsoleLine.Group].Add(vector2_2);
          valueListArray[(int) monitoringConsoleLine.Group].Add(vector2_3);
        }
      }
      for (int index = 0; index < valueListArray.Length; ++index)
      {
        ValueList<Vector2> valueList = valueListArray[index];
        if (valueList.Count > 0)
        {
          Color key = Color.op_Multiply(ref this._powerCableColors[index], ref modulator);
          Color srgb;
          if (!this._sRGBLookUp.TryGetValue(key, out srgb))
          {
            srgb = Color.ToSrgb(key);
            this._sRGBLookUp[key] = srgb;
          }
          ((DrawingHandleBase) handle).DrawPrimitives((DrawPrimitiveTopology) 4, (ReadOnlySpan<Vector2>) valueList.Span, srgb);
        }
      }
    }
    else
    {
      ValueList<Vector2>[] valueListArray = new ValueList<Vector2>[3];
      foreach (PowerMonitoringConsoleLine monitoringConsoleLine in fullCableNetwork)
      {
        if (!this.HiddenLineGroups.Contains(monitoringConsoleLine.Group))
        {
          Vector2 powerCableOffset = this._powerCableOffsets[(int) monitoringConsoleLine.Group];
          Vector2 vector2_4 = this.ScalePosition(new Vector2(Math.Min(monitoringConsoleLine.Origin.X, monitoringConsoleLine.Terminus.X) - 0.1f, Math.Min(monitoringConsoleLine.Origin.Y, monitoringConsoleLine.Terminus.Y) - 0.1f) + powerCableOffset - vector2_1);
          Vector2 vector2_5 = this.ScalePosition(new Vector2(Math.Max(monitoringConsoleLine.Origin.X, monitoringConsoleLine.Terminus.X) + 0.1f, Math.Min(monitoringConsoleLine.Origin.Y, monitoringConsoleLine.Terminus.Y) - 0.1f) + powerCableOffset - vector2_1);
          Vector2 vector2_6 = this.ScalePosition(new Vector2(Math.Min(monitoringConsoleLine.Origin.X, monitoringConsoleLine.Terminus.X) - 0.1f, Math.Max(monitoringConsoleLine.Origin.Y, monitoringConsoleLine.Terminus.Y) + 0.1f) + powerCableOffset - vector2_1);
          Vector2 vector2_7 = this.ScalePosition(new Vector2(Math.Max(monitoringConsoleLine.Origin.X, monitoringConsoleLine.Terminus.X) + 0.1f, Math.Max(monitoringConsoleLine.Origin.Y, monitoringConsoleLine.Terminus.Y) + 0.1f) + powerCableOffset - vector2_1);
          valueListArray[(int) monitoringConsoleLine.Group].Add(vector2_6);
          valueListArray[(int) monitoringConsoleLine.Group].Add(vector2_4);
          valueListArray[(int) monitoringConsoleLine.Group].Add(vector2_7);
          valueListArray[(int) monitoringConsoleLine.Group].Add(vector2_4);
          valueListArray[(int) monitoringConsoleLine.Group].Add(vector2_7);
          valueListArray[(int) monitoringConsoleLine.Group].Add(vector2_5);
        }
      }
      for (int index = 0; index < valueListArray.Length; ++index)
      {
        ValueList<Vector2> valueList = valueListArray[index];
        if (valueList.Count > 0)
        {
          Color key = Color.op_Multiply(ref this._powerCableColors[index], ref modulator);
          Color srgb;
          if (!this._sRGBLookUp.TryGetValue(key, out srgb))
          {
            srgb = Color.ToSrgb(key);
            this._sRGBLookUp[key] = srgb;
          }
          ((DrawingHandleBase) handle).DrawPrimitives((DrawPrimitiveTopology) 1, (ReadOnlySpan<Vector2>) valueList.Span, srgb);
        }
      }
    }
  }

  public List<PowerMonitoringConsoleLine> GetDecodedPowerCableChunks(
    Dictionary<Vector2i, PowerCableChunk>? chunks)
  {
    List<PowerMonitoringConsoleLine> powerCableChunks = new List<PowerMonitoringConsoleLine>();
    if (!this._entManager.TryGetComponent<MapGridComponent>(this.MapUid, ref this._grid) || chunks == null)
      return powerCableChunks;
    Array.ForEach<Dictionary<Vector2i, Vector2i>>(this._horizLines, (Action<Dictionary<Vector2i, Vector2i>>) (x => x.Clear()));
    Array.ForEach<Dictionary<Vector2i, Vector2i>>(this._horizLinesReversed, (Action<Dictionary<Vector2i, Vector2i>>) (x => x.Clear()));
    Array.ForEach<Dictionary<Vector2i, Vector2i>>(this._vertLines, (Action<Dictionary<Vector2i, Vector2i>>) (x => x.Clear()));
    Array.ForEach<Dictionary<Vector2i, Vector2i>>(this._vertLinesReversed, (Action<Dictionary<Vector2i, Vector2i>>) (x => x.Clear()));
    foreach ((Vector2i key5, PowerCableChunk powerCableChunk1) in chunks)
    {
      Vector2i vector2i1 = key5;
      PowerCableChunk powerCableChunk2 = powerCableChunk1;
      for (int index1 = 0; index1 < 3; ++index1)
      {
        Dictionary<Vector2i, Vector2i> horizLine = this._horizLines[index1];
        Dictionary<Vector2i, Vector2i> lookupReversed1 = this._horizLinesReversed[index1];
        Dictionary<Vector2i, Vector2i> vertLine = this._vertLines[index1];
        Dictionary<Vector2i, Vector2i> lookupReversed2 = this._vertLinesReversed[index1];
        int num1 = powerCableChunk2.PowerCableData[index1];
        for (int index2 = 0; index2 < 25; ++index2)
        {
          int num2 = 1 << index2;
          if ((num1 & num2) != 0)
          {
            Vector2i tileFromIndex = SharedPowerMonitoringConsoleSystem.GetTileFromIndex(index2);
            Vector2i vector2i2 = Vector2i.op_Multiply(Vector2i.op_Addition(Vector2i.op_Multiply(powerCableChunk2.Origin, 5), tileFromIndex), (int) this._grid.TileSize);
            key5 = vector2i2;
            key5.Y = -vector2i2.Y;
            Vector2i vector2i3 = key5;
            PowerCableChunk powerCableChunk3;
            bool flag1;
            if (tileFromIndex.X == 4)
            {
              flag1 = chunks.TryGetValue(Vector2i.op_Addition(vector2i1, new Vector2i(1, 0)), out powerCableChunk3) && (powerCableChunk3.PowerCableData[index1] & SharedPowerMonitoringConsoleSystem.GetFlag(new Vector2i(0, tileFromIndex.Y))) != 0;
            }
            else
            {
              int flag2 = SharedPowerMonitoringConsoleSystem.GetFlag(Vector2i.op_Addition(tileFromIndex, new Vector2i(1, 0)));
              flag1 = (num1 & flag2) != 0;
            }
            if (flag1)
              this.AddOrUpdateNavMapLine(vector2i3, Vector2i.op_Addition(vector2i3, new Vector2i((int) this._grid.TileSize, 0)), horizLine, lookupReversed1);
            bool flag3;
            if (tileFromIndex.Y == 4)
            {
              flag3 = chunks.TryGetValue(Vector2i.op_Addition(vector2i1, new Vector2i(0, 1)), out powerCableChunk3) && (powerCableChunk3.PowerCableData[index1] & SharedPowerMonitoringConsoleSystem.GetFlag(new Vector2i(tileFromIndex.X, 0))) != 0;
            }
            else
            {
              int flag4 = SharedPowerMonitoringConsoleSystem.GetFlag(Vector2i.op_Addition(tileFromIndex, new Vector2i(0, 1)));
              flag3 = (num1 & flag4) != 0;
            }
            if (flag3)
              this.AddOrUpdateNavMapLine(Vector2i.op_Addition(vector2i3, new Vector2i(0, (int) -this._grid.TileSize)), vector2i3, vertLine, lookupReversed2);
          }
        }
      }
    }
    Vector2 vector2 = new Vector2((float) this._grid.TileSize * 0.5f, (float) -this._grid.TileSize * 0.5f);
    Vector2i key4;
    for (int group = 0; group < this._horizLines.Length; ++group)
    {
      foreach ((key5, key4) in this._horizLines[group])
      {
        Vector2i vector2i4 = key5;
        Vector2i vector2i5 = key4;
        powerCableChunks.Add(new PowerMonitoringConsoleLine(Vector2i.op_Implicit(vector2i4) + vector2, Vector2i.op_Implicit(vector2i5) + vector2, (PowerMonitoringConsoleLineGroup) group));
      }
    }
    for (int group = 0; group < this._vertLines.Length; ++group)
    {
      foreach ((key4, key5) in this._vertLines[group])
      {
        Vector2i vector2i6 = key4;
        Vector2i vector2i7 = key5;
        powerCableChunks.Add(new PowerMonitoringConsoleLine(Vector2i.op_Implicit(vector2i6) + vector2, Vector2i.op_Implicit(vector2i7) + vector2, (PowerMonitoringConsoleLineGroup) group));
      }
    }
    return powerCableChunks;
  }
}
