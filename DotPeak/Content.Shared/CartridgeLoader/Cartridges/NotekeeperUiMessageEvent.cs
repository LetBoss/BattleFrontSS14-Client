// Decompiled with JetBrains decompiler
// Type: Content.Shared.CartridgeLoader.Cartridges.NotekeeperUiMessageEvent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Serialization;
using System;

#nullable enable
namespace Content.Shared.CartridgeLoader.Cartridges;

[NetSerializable]
[Serializable]
public sealed class NotekeeperUiMessageEvent : CartridgeMessageEvent
{
  public readonly NotekeeperUiAction Action;
  public readonly string Note;

  public NotekeeperUiMessageEvent(NotekeeperUiAction action, string note)
  {
    this.Action = action;
    this.Note = note;
  }
}
