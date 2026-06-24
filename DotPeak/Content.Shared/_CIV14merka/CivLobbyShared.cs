// Decompiled with JetBrains decompiler
// Type: Content.Shared._CIV14merka.CivLobbyStatusEvent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using System;

#nullable enable
namespace Content.Shared._CIV14merka;

[NetSerializable]
[Serializable]
public sealed class CivLobbyStatusEvent : EntityEventArgs
{
  public TimeSpan StartTime { get; }

  public bool CountdownActive { get; }

  public int ReadyCount { get; }

  public string RoundModeName { get; }

  public string MapName { get; }

  public bool RandomMapSelection { get; }

  public CivLobbyStatusEvent(
    TimeSpan startTime,
    bool countdownActive,
    int readyCount,
    string roundModeName,
    string mapName,
    bool randomMapSelection)
  {
    this.StartTime = startTime;
    this.CountdownActive = countdownActive;
    this.ReadyCount = readyCount;
    this.RoundModeName = roundModeName;
    this.MapName = mapName;
    this.RandomMapSelection = randomMapSelection;
  }
}
