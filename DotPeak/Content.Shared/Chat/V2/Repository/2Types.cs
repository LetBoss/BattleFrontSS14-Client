// Decompiled with JetBrains decompiler
// Type: Content.Shared.Chat.V2.Repository.MessagePatchedEvent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using System;

#nullable enable
namespace Content.Shared.Chat.V2.Repository;

[NetSerializable]
[Serializable]
public sealed class MessagePatchedEvent : EntityEventArgs
{
  public uint MessageId;
  public string NewMessage;

  public MessagePatchedEvent(uint id, string newMessage)
  {
    this.MessageId = id;
    this.NewMessage = newMessage;
    // ISSUE: explicit constructor call
    base.\u002Ector();
  }
}
