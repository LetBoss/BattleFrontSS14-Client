// Decompiled with JetBrains decompiler
// Type: Content.Shared.Chat.ChatChannelExt
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Localization;
using System;

#nullable enable
namespace Content.Shared.Chat;

public static class ChatChannelExt
{
  public static string GetString(this ChatChannel channel)
  {
    switch (channel)
    {
      case ChatChannel.OOC:
        return Loc.GetString("chat-channel-humanized-ooc");
      case ChatChannel.AdminChat:
        return Loc.GetString("chat-channel-humanized-admin");
      case ChatChannel.Party:
        return Loc.GetString("chat-channel-humanized-party");
      case ChatChannel.MiniGame:
        return Loc.GetString("chat-channel-humanized-minigame");
      case ChatChannel.Lobby:
        return Loc.GetString("chat-channel-humanized-lobby");
      default:
        throw new ArgumentOutOfRangeException(nameof (channel), (object) channel, (string) null);
    }
  }
}
