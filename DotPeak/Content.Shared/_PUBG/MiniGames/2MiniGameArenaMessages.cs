// Decompiled with JetBrains decompiler
// Type: Content.Shared._PUBG.MiniGames.MiniGameArenaSaveMessage
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using System;

#nullable enable
namespace Content.Shared._PUBG.MiniGames;

[NetSerializable]
[Serializable]
public sealed class MiniGameArenaSaveMessage : EntityEventArgs
{
  public string DisplayName { get; }

  public bool Overwrite { get; }

  public string? ExistingArenaName { get; }

  public MiniGameArenaSaveMessage(string displayName, bool overwrite = false, string? existingArenaName = null)
  {
    this.DisplayName = displayName;
    this.Overwrite = overwrite;
    this.ExistingArenaName = existingArenaName;
  }
}
