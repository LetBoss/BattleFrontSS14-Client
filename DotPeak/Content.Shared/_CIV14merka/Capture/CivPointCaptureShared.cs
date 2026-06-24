// Decompiled with JetBrains decompiler
// Type: Content.Shared._CIV14merka.Capture.CivPointCapturePointState
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Map;
using Robust.Shared.Serialization;
using System;
using System.Numerics;

#nullable enable
namespace Content.Shared._CIV14merka.Capture;

[NetSerializable]
[Serializable]
public sealed class CivPointCapturePointState
{
  public int Id { get; }

  public string Label { get; }

  public MapId MapId { get; }

  public Vector2 Position { get; }

  public int OwnerTeamId { get; }

  public int CapturingTeamId { get; }

  public float CaptureProgress { get; }

  public CivPointCapturePointState(
    int id,
    string label,
    MapId mapId,
    Vector2 position,
    int ownerTeamId,
    int capturingTeamId,
    float captureProgress)
  {
    this.Id = id;
    this.Label = label;
    this.MapId = mapId;
    this.Position = position;
    this.OwnerTeamId = ownerTeamId;
    this.CapturingTeamId = capturingTeamId;
    this.CaptureProgress = captureProgress;
  }
}
