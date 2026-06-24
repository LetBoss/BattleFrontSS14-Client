// Decompiled with JetBrains decompiler
// Type: Content.Client.Atmos.Consoles.AtmosMonitoringConsoleNavMapControl
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.Pinpointer.UI;
using Content.Shared.Atmos.Components;
using Robust.Client.Graphics;
using Robust.Shared.Collections;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

#nullable enable
namespace Content.Client.Atmos.Consoles;

public sealed class AtmosMonitoringConsoleNavMapControl : NavMapControl
{
  [Dependency]
  private IEntityManager _entManager;
  public bool ShowPipeNetwork = true;
  public int? FocusNetId;
  private const int ChunkSize = 4;
  private const float ScaleModifier = 4f;
  private readonly float[] _layerFraction = new float[3]
  {
    0.5f,
    0.75f,
    0.25f
  };
  private const float LineThickness = 0.05f;
  private readonly Color _basePipeNetColor = Color.LightGray;
  private readonly Color _unfocusedPipeNetColor = Color.DimGray;
  private List<AtmosMonitoringConsoleLine> _atmosPipeNetwork = new List<AtmosMonitoringConsoleLine>();
  private Dictionary<Color, Color> _sRGBLookUp = new Dictionary<Color, Color>();
  private Dictionary<Color, Dictionary<Vector2i, Vector2i>> _horizLines = new Dictionary<Color, Dictionary<Vector2i, Vector2i>>();
  private Dictionary<Color, Dictionary<Vector2i, Vector2i>> _horizLinesReversed = new Dictionary<Color, Dictionary<Vector2i, Vector2i>>();
  private Dictionary<Color, Dictionary<Vector2i, Vector2i>> _vertLines = new Dictionary<Color, Dictionary<Vector2i, Vector2i>>();
  private Dictionary<Color, Dictionary<Vector2i, Vector2i>> _vertLinesReversed = new Dictionary<Color, Dictionary<Vector2i, Vector2i>>();

  public AtmosMonitoringConsoleNavMapControl()
  {
    this.PostWallDrawingAction += new Action<DrawingHandleScreen>(this.DrawAllPipeNetworks);
  }

  protected override void UpdateNavMap()
  {
    base.UpdateNavMap();
    AtmosMonitoringConsoleComponent consoleComponent;
    MapGridComponent grid;
    if (!this._entManager.TryGetComponent<AtmosMonitoringConsoleComponent>(this.Owner, ref consoleComponent) || !this._entManager.TryGetComponent<MapGridComponent>(this.MapUid, ref grid))
      return;
    this._atmosPipeNetwork = this.GetDecodedAtmosPipeChunks(consoleComponent.AtmosPipeChunks, grid);
  }

  private void DrawAllPipeNetworks(DrawingHandleScreen handle)
  {
    if (!this.ShowPipeNetwork || this._atmosPipeNetwork == null || !this._atmosPipeNetwork.Any<AtmosMonitoringConsoleLine>())
      return;
    this.DrawPipeNetwork(handle, this._atmosPipeNetwork);
  }

  private void DrawPipeNetwork(
    DrawingHandleScreen handle,
    List<AtmosMonitoringConsoleLine> atmosPipeNetwork)
  {
    Vector2 offset = this.GetOffset();
    Vector2 vector2_1 = offset with { Y = -offset.Y };
    if ((double) this.WorldRange / (double) this.WorldMaxRange > 0.5)
    {
      Dictionary<Color, ValueList<Vector2>> dictionary = new Dictionary<Color, ValueList<Vector2>>();
      foreach (AtmosMonitoringConsoleLine monitoringConsoleLine in atmosPipeNetwork)
      {
        Vector2 vector2_2 = this.ScalePosition(monitoringConsoleLine.Origin - vector2_1);
        Vector2 vector2_3 = this.ScalePosition(monitoringConsoleLine.Terminus - vector2_1);
        ValueList<Vector2> valueList;
        if (!dictionary.TryGetValue(monitoringConsoleLine.Color, out valueList))
          valueList = new ValueList<Vector2>();
        valueList.Add(vector2_2);
        valueList.Add(vector2_3);
        dictionary[monitoringConsoleLine.Color] = valueList;
      }
      foreach ((Color key, ValueList<Vector2> valueList) in dictionary)
      {
        if (valueList.Count > 0)
          ((DrawingHandleBase) handle).DrawPrimitives((DrawPrimitiveTopology) 4, (ReadOnlySpan<Vector2>) valueList.Span, key);
      }
    }
    else
    {
      Dictionary<Color, ValueList<Vector2>> dictionary = new Dictionary<Color, ValueList<Vector2>>();
      foreach (AtmosMonitoringConsoleLine monitoringConsoleLine in atmosPipeNetwork)
      {
        Vector2 vector2_4 = this.ScalePosition(new Vector2(Math.Min(monitoringConsoleLine.Origin.X, monitoringConsoleLine.Terminus.X) - 0.05f, Math.Min(monitoringConsoleLine.Origin.Y, monitoringConsoleLine.Terminus.Y) - 0.05f) - vector2_1);
        Vector2 vector2_5 = this.ScalePosition(new Vector2(Math.Max(monitoringConsoleLine.Origin.X, monitoringConsoleLine.Terminus.X) + 0.05f, Math.Min(monitoringConsoleLine.Origin.Y, monitoringConsoleLine.Terminus.Y) - 0.05f) - vector2_1);
        Vector2 vector2_6 = this.ScalePosition(new Vector2(Math.Min(monitoringConsoleLine.Origin.X, monitoringConsoleLine.Terminus.X) - 0.05f, Math.Max(monitoringConsoleLine.Origin.Y, monitoringConsoleLine.Terminus.Y) + 0.05f) - vector2_1);
        Vector2 vector2_7 = this.ScalePosition(new Vector2(Math.Max(monitoringConsoleLine.Origin.X, monitoringConsoleLine.Terminus.X) + 0.05f, Math.Max(monitoringConsoleLine.Origin.Y, monitoringConsoleLine.Terminus.Y) + 0.05f) - vector2_1);
        ValueList<Vector2> valueList;
        if (!dictionary.TryGetValue(monitoringConsoleLine.Color, out valueList))
          valueList = new ValueList<Vector2>();
        valueList.Add(vector2_6);
        valueList.Add(vector2_4);
        valueList.Add(vector2_7);
        valueList.Add(vector2_4);
        valueList.Add(vector2_7);
        valueList.Add(vector2_5);
        dictionary[monitoringConsoleLine.Color] = valueList;
      }
      foreach ((Color key, ValueList<Vector2> valueList) in dictionary)
      {
        if (valueList.Count > 0)
          ((DrawingHandleBase) handle).DrawPrimitives((DrawPrimitiveTopology) 1, (ReadOnlySpan<Vector2>) valueList.Span, key);
      }
    }
  }

  private List<AtmosMonitoringConsoleLine> GetDecodedAtmosPipeChunks(
    Dictionary<Vector2i, AtmosPipeChunk>? chunks,
    MapGridComponent? grid)
  {
    List<AtmosMonitoringConsoleLine> decodedAtmosPipeChunks = new List<AtmosMonitoringConsoleLine>();
    if (chunks == null || grid == null)
      return decodedAtmosPipeChunks;
    this._horizLines.Clear();
    this._horizLinesReversed.Clear();
    this._vertLines.Clear();
    this._vertLinesReversed.Clear();
    ulong num1 = 1;
    ulong num2 = 2;
    ulong num3 = 4;
    ulong num4 = 8;
    Color key8;
    foreach ((Vector2i key10, AtmosPipeChunk atmosPipeChunk) in chunks)
    {
      List<AtmosMonitoringConsoleLine> monitoringConsoleLineList = new List<AtmosMonitoringConsoleLine>();
      foreach (((int NetId, AtmosPipeLayer PipeLayer, string HexCode), ulong num5) in atmosPipeChunk.AtmosPipeData)
      {
        int num6 = NetId;
        AtmosPipeLayer index1 = PipeLayer;
        string str = HexCode;
        ulong num7 = num5;
        key8 = Color.FromHex((ReadOnlySpan<char>) str, new Color?());
        Color key3 = Color.op_Multiply(ref key8, ref this._basePipeNetColor);
        if (this.FocusNetId.HasValue)
        {
          int? focusNetId = this.FocusNetId;
          NetId = num6;
          if (!(focusNetId.GetValueOrDefault() == NetId & focusNetId.HasValue))
            key3 = Color.op_Multiply(ref key3, ref this._unfocusedPipeNetColor);
        }
        Dictionary<Vector2i, Vector2i> lookup1;
        if (!this._horizLines.TryGetValue(key3, out lookup1))
        {
          lookup1 = new Dictionary<Vector2i, Vector2i>();
          this._horizLines[key3] = lookup1;
        }
        Dictionary<Vector2i, Vector2i> lookupReversed1;
        if (!this._horizLinesReversed.TryGetValue(key3, out lookupReversed1))
        {
          lookupReversed1 = new Dictionary<Vector2i, Vector2i>();
          this._horizLinesReversed[key3] = lookupReversed1;
        }
        Dictionary<Vector2i, Vector2i> lookup2;
        if (!this._vertLines.TryGetValue(key3, out lookup2))
        {
          lookup2 = new Dictionary<Vector2i, Vector2i>();
          this._vertLines[key3] = lookup2;
        }
        Dictionary<Vector2i, Vector2i> lookupReversed2;
        if (!this._vertLinesReversed.TryGetValue(key3, out lookupReversed2))
        {
          lookupReversed2 = new Dictionary<Vector2i, Vector2i>();
          this._vertLinesReversed[key3] = lookupReversed2;
        }
        float num8 = this._layerFraction[(int) index1];
        Vector2 vector2_1 = new Vector2((float) grid.TileSize * num8, (float) -grid.TileSize * num8);
        for (int index2 = 0; index2 < 16 /*0x10*/; ++index2)
        {
          if (num7 != 0UL)
          {
            ulong num9 = 15UL << index2 * 4;
            if (((long) num7 & (long) num9) != 0L)
            {
              Vector2i tileFromIndex = this.GetTileFromIndex(index2);
              Vector2i vector2i1 = Vector2i.op_Multiply(Vector2i.op_Addition(Vector2i.op_Multiply(atmosPipeChunk.Origin, 4), tileFromIndex), (int) grid.TileSize);
              key10 = vector2i1;
              key10.Y = -vector2i1.Y;
              Vector2i vector2i2 = key10;
              Vector2 vector2_2 = (num7 & num1 << index2 * 4) > 0UL ? new Vector2((float) grid.TileSize * num8, (float) -grid.TileSize * 1f) : vector2_1;
              Vector2 vector2_3 = (num7 & num2 << index2 * 4) > 0UL ? new Vector2((float) grid.TileSize * num8, (float) -grid.TileSize * 0.0f) : vector2_1;
              Vector2 vector2_4 = (num7 & num4 << index2 * 4) > 0UL ? new Vector2((float) grid.TileSize * 1f, (float) -grid.TileSize * num8) : vector2_1;
              Vector2 vector2_5 = (num7 & num3 << index2 * 4) > 0UL ? new Vector2((float) grid.TileSize * 0.0f, (float) -grid.TileSize * num8) : vector2_1;
              this.AddOrUpdateNavMapLine(this.ConvertVector2ToVector2i(Vector2i.op_Implicit(vector2i2) + vector2_4, 4f), this.ConvertVector2ToVector2i(Vector2i.op_Implicit(vector2i2) + vector2_5, 4f), lookup1, lookupReversed1);
              this.AddOrUpdateNavMapLine(this.ConvertVector2ToVector2i(Vector2i.op_Implicit(vector2i2) + vector2_2, 4f), this.ConvertVector2ToVector2i(Vector2i.op_Implicit(vector2i2) + vector2_3, 4f), lookup2, lookupReversed2);
            }
          }
        }
      }
    }
    Dictionary<Vector2i, Vector2i> dictionary4;
    Vector2i key9;
    foreach ((key8, dictionary4) in this._horizLines)
    {
      Color color1 = key8;
      Dictionary<Vector2i, Vector2i> dictionary3 = dictionary4;
      Color color2 = this.GetsRGBColor(color1);
      foreach ((key10, key9) in dictionary3)
      {
        Vector2i vector1 = key10;
        Vector2i vector2 = key9;
        decodedAtmosPipeChunks.Add(new AtmosMonitoringConsoleLine(this.ConvertVector2iToVector2(vector1, 0.25f), this.ConvertVector2iToVector2(vector2, 0.25f), color2));
      }
    }
    foreach ((key8, dictionary4) in this._vertLines)
    {
      Color color3 = key8;
      Dictionary<Vector2i, Vector2i> dictionary5 = dictionary4;
      Color color4 = this.GetsRGBColor(color3);
      foreach ((key9, key10) in dictionary5)
      {
        Vector2i vector3 = key9;
        Vector2i vector4 = key10;
        decodedAtmosPipeChunks.Add(new AtmosMonitoringConsoleLine(this.ConvertVector2iToVector2(vector3, 0.25f), this.ConvertVector2iToVector2(vector4, 0.25f), color4));
      }
    }
    return decodedAtmosPipeChunks;
  }

  private Vector2 ConvertVector2iToVector2(Vector2i vector, float scale = 1f)
  {
    return new Vector2((float) vector.X * scale, (float) vector.Y * scale);
  }

  private Vector2i ConvertVector2ToVector2i(Vector2 vector, float scale = 1f)
  {
    return new Vector2i((int) MathF.Round(vector.X * scale), (int) MathF.Round(vector.Y * scale));
  }

  private Vector2i GetTileFromIndex(int index) => new Vector2i(index / 4, index % 4);

  private Color GetsRGBColor(Color color)
  {
    Color srgb;
    if (!this._sRGBLookUp.TryGetValue(color, out srgb))
    {
      srgb = Color.ToSrgb(color);
      this._sRGBLookUp[color] = srgb;
    }
    return srgb;
  }
}
