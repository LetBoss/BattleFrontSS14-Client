// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Marines.ControlComputer.MarineControlComputerMedalMessageEvent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Dialog;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Marines.ControlComputer;

[ByRefEvent]
[NetSerializable]
[Serializable]
public sealed record MarineControlComputerMedalMessageEvent(
  NetEntity Actor,
  NetEntity? Marine,
  string Name,
  string Message = "",
  string? LastPlayerId = null) : DialogInputEvent(Message)
{
  public NetEntity Actor { get; init; } = Actor;

  public NetEntity? Marine { get; init; } = Marine;

  public string Name { get; init; } = Name;

  public string? LastPlayerId { get; init; } = LastPlayerId;

  [CompilerGenerated]
  public sealed override bool Equals(DialogInputEvent? other) => this.Equals((object) other);

  [CompilerGenerated]
  public void Deconstruct(
    out NetEntity Actor,
    out NetEntity? Marine,
    out string Name,
    out string Message,
    out string? LastPlayerId)
  {
    Actor = this.Actor;
    Marine = this.Marine;
    Name = this.Name;
    Message = this.Message;
    LastPlayerId = this.LastPlayerId;
  }
}
