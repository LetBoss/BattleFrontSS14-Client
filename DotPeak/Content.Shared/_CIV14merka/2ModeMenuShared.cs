// Decompiled with JetBrains decompiler
// Type: Content.Shared._CIV14merka.ModeMenuStatusEvent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using System;

#nullable disable
namespace Content.Shared._CIV14merka;

[NetSerializable]
[Serializable]
public sealed class ModeMenuStatusEvent : EntityEventArgs
{
  public int ServerOnlineCount { get; }

  public int PubgOnlineCount { get; }

  public int Civ14OnlineCount { get; }

  public bool PubgEnabled { get; }

  public bool Civ14Enabled { get; }

  public ModeMenuStatusEvent(
    int serverOnlineCount,
    int pubgOnlineCount,
    int civ14OnlineCount,
    bool pubgEnabled,
    bool civ14Enabled)
  {
    this.ServerOnlineCount = serverOnlineCount;
    this.PubgOnlineCount = pubgOnlineCount;
    this.Civ14OnlineCount = civ14OnlineCount;
    this.PubgEnabled = pubgEnabled;
    this.Civ14Enabled = civ14Enabled;
  }
}
