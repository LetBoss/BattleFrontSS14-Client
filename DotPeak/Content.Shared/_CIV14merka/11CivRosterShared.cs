// Decompiled with JetBrains decompiler
// Type: Content.Shared._CIV14merka.CivRosterSetSquadOpenRequestEvent
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
public sealed class CivRosterSetSquadOpenRequestEvent : EntityEventArgs
{
  public int TeamId { get; }

  public int SquadId { get; }

  public bool IsOpen { get; }

  public CivRosterSetSquadOpenRequestEvent(int teamId, int squadId, bool isOpen)
  {
    this.TeamId = teamId;
    this.SquadId = squadId;
    this.IsOpen = isOpen;
  }
}
