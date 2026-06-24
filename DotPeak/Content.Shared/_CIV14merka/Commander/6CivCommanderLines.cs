// Decompiled with JetBrains decompiler
// Type: Content.Shared._CIV14merka.Commander.CivCommanderLabelCreateRequestEvent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Serialization;
using System;
using System.Numerics;

#nullable enable
namespace Content.Shared._CIV14merka.Commander;

[NetSerializable]
[Serializable]
public sealed class CivCommanderLabelCreateRequestEvent : EntityEventArgs
{
  public CivCommanderLineColor Color { get; }

  public MapId MapId { get; }

  public Vector2 Position { get; }

  public float Rotation { get; }

  public string Text { get; }

  public CivCommanderLabelCreateRequestEvent(
    CivCommanderLineColor color,
    MapId mapId,
    Vector2 position,
    float rotation,
    string text)
  {
    this.Color = color;
    this.MapId = mapId;
    this.Position = position;
    this.Rotation = rotation;
    this.Text = text;
  }
}
