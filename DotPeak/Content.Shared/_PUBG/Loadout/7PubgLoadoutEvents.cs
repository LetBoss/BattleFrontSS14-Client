// Decompiled with JetBrains decompiler
// Type: Content.Shared._PUBG.Loadout.PubgLoadoutActionResultMessage
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using System;

#nullable enable
namespace Content.Shared._PUBG.Loadout;

[NetSerializable]
[Serializable]
public sealed class PubgLoadoutActionResultMessage : EntityEventArgs
{
  public PubgLoadoutActionType Action { get; }

  public NetEntity Item { get; }

  public bool Success { get; }

  public string? ErrorLocKey { get; }

  public PubgLoadoutActionResultMessage(
    PubgLoadoutActionType action,
    NetEntity item,
    bool success,
    string? errorLocKey = null)
  {
    this.Action = action;
    this.Item = item;
    this.Success = success;
    this.ErrorLocKey = errorLocKey;
  }
}
