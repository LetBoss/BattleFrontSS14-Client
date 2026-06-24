// Decompiled with JetBrains decompiler
// Type: Content.Client._RMC14.Chat.RepeatedMessage
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Chat;
using Robust.Shared.GameObjects;
using Robust.Shared.Utility;

#nullable enable
namespace Content.Client._RMC14.Chat;

public sealed class RepeatedMessage(
  int index,
  FormattedMessage formattedMessage,
  NetEntity senderEntity,
  string message,
  ChatChannel channel)
{
  public readonly int Index = index;
  public readonly FormattedMessage FormattedMessage = formattedMessage;
  public readonly NetEntity SenderEntity = senderEntity;
  public readonly string Message = message;
  public readonly ChatChannel Channel = channel;
  public int Count = 1;
}
