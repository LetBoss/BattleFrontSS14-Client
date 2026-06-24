// Decompiled with JetBrains decompiler
// Type: Content.Shared._CIV14merka.Commander.CivAirstrikeFlybyEvent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Serialization;
using System;
using System.Numerics;

#nullable disable
namespace Content.Shared._CIV14merka.Commander;

[NetSerializable]
[Serializable]
public sealed class CivAirstrikeFlybyEvent : EntityEventArgs
{
  public Vector2 Origin { get; }

  public Vector2 Entry { get; }

  public Vector2 EntryCtr { get; }

  public Vector2 Approach { get; }

  public Vector2 Target { get; }

  public Vector2 RunEnd { get; }

  public Vector2 ExitTurn { get; }

  public Vector2 ExitCtr { get; }

  public Vector2 Exit { get; }

  public float EntryLineLen { get; }

  public float EntryArcLen { get; }

  public float ExitLen { get; }

  public bool EntryCcw { get; }

  public bool ExitCcw { get; }

  public float Speed { get; }

  public int Count { get; }

  public float Spacing { get; }

  public float Alpha { get; }

  public float StartDelay { get; }

  public float ScaleMin { get; }

  public float ScaleMax { get; }

  public CivAirstrikeSide Side { get; }

  public MapId MapId { get; }

  public CivAirstrikeFlybyEvent(
    Vector2 origin,
    Vector2 entry,
    Vector2 entryCtr,
    Vector2 approach,
    Vector2 target,
    Vector2 runEnd,
    Vector2 exitTurn,
    Vector2 exitCtr,
    Vector2 exit,
    float entryLineLen,
    float entryArcLen,
    float exitLen,
    bool entryCcw,
    bool exitCcw,
    float speed,
    int count,
    float spacing,
    float alpha,
    float startDelay,
    float scaleMin,
    float scaleMax,
    CivAirstrikeSide side,
    MapId mapId)
  {
    this.Origin = origin;
    this.Entry = entry;
    this.EntryCtr = entryCtr;
    this.Approach = approach;
    this.Target = target;
    this.RunEnd = runEnd;
    this.ExitTurn = exitTurn;
    this.ExitCtr = exitCtr;
    this.Exit = exit;
    this.EntryLineLen = entryLineLen;
    this.EntryArcLen = entryArcLen;
    this.ExitLen = exitLen;
    this.EntryCcw = entryCcw;
    this.ExitCcw = exitCcw;
    this.Speed = speed;
    this.Count = count;
    this.Spacing = spacing;
    this.Alpha = alpha;
    this.StartDelay = startDelay;
    this.ScaleMin = scaleMin;
    this.ScaleMax = scaleMax;
    this.Side = side;
    this.MapId = mapId;
  }
}
