// Decompiled with JetBrains decompiler
// Type: Robust.Shared.EntitySerialization.MapLoadOptions
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Map;
using Robust.Shared.Maths;
using System.Numerics;

#nullable disable
namespace Robust.Shared.EntitySerialization;

public struct MapLoadOptions
{
  public static readonly MapLoadOptions Default = new MapLoadOptions();
  public MapId? MergeMap;
  public Vector2 Offset;
  public Angle Rotation;
  public DeserializationOptions DeserializationOptions;
  public MapId? ForceMapId;
  public FileCategory? ExpectedCategory;

  public MapLoadOptions()
  {
    this.Offset = new Vector2();
    this.Rotation = new Angle();
    this.ForceMapId = new MapId?();
    this.ExpectedCategory = new FileCategory?();
    this.MergeMap = new MapId?();
    this.DeserializationOptions = DeserializationOptions.Default;
  }
}
