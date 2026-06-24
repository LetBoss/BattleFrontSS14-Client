// Decompiled with JetBrains decompiler
// Type: Content.Shared.Administration.BwoinkPlayerTypingUpdated
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Network;
using Robust.Shared.Serialization;
using System;

#nullable enable
namespace Content.Shared.Administration;

[NetSerializable]
[Serializable]
public sealed class BwoinkPlayerTypingUpdated : EntityEventArgs
{
  public NetUserId Channel { get; }

  public string PlayerName { get; }

  public bool Typing { get; }

  public BwoinkPlayerTypingUpdated(NetUserId channel, string playerName, bool typing)
  {
    this.Channel = channel;
    this.PlayerName = playerName;
    this.Typing = typing;
  }
}
