// Decompiled with JetBrains decompiler
// Type: Content.Client._CIV14merka.Commander.CivAirstrikeFlybyInstance
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared._CIV14merka.Commander;
using Robust.Shared.Map;
using System;
using System.Numerics;

#nullable disable
namespace Content.Client._CIV14merka.Commander;

public sealed class CivAirstrikeFlybyInstance
{
  public required Vector2 Origin;
  public required Vector2 Entry;
  public required Vector2 EntryCtr;
  public required Vector2 Approach;
  public required Vector2 Target;
  public required Vector2 RunEnd;
  public required Vector2 ExitTurn;
  public required Vector2 ExitCtr;
  public required Vector2 Exit;
  public required float EntryLineLen;
  public required float EntryArcLen;
  public required float ExitLen;
  public required bool EntryCcw;
  public required bool ExitCcw;
  public required float Speed;
  public required int Count;
  public required float Spacing;
  public required float Alpha;
  public required float ScaleMin;
  public required float ScaleMax;
  public required CivAirstrikeSide Side;
  public required MapId MapId;
  public required TimeSpan StartTime;

  public float RunInLen => (this.Target - this.Approach).Length();

  public float RunOutLen => (this.RunEnd - this.Target).Length();

  public float ExitLineLen => (this.Exit - this.ExitTurn).Length();

  public float ExitArcLen => MathF.Max(0.0f, this.ExitLen - this.ExitLineLen);

  public float EntryTurnEnd => this.EntryLineLen + this.EntryArcLen;

  public float ToTarget => this.EntryTurnEnd + this.RunInLen;

  public float RunEndDist => this.ToTarget + this.RunOutLen;

  public float ExitTurnEnd => this.RunEndDist + this.ExitArcLen;

  public float Total => this.ExitTurnEnd + this.ExitLineLen;
}
