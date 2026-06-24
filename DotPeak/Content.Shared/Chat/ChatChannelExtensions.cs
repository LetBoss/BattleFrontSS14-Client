// Decompiled with JetBrains decompiler
// Type: Content.Shared.Chat.ChatChannelExtensions
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Maths;

#nullable disable
namespace Content.Shared.Chat;

public static class ChatChannelExtensions
{
  public static Color TextColor(this ChatChannel channel)
  {
    Color color;
    switch (channel)
    {
      case ChatChannel.Whisper:
        color = Color.DarkGray;
        break;
      case ChatChannel.Server:
        color = Color.Orange;
        break;
      case ChatChannel.Radio:
        color = Color.LimeGreen;
        break;
      case ChatChannel.LOOC:
        color = Color.MediumTurquoise;
        break;
      case ChatChannel.OOC:
        color = Color.LightSkyBlue;
        break;
      case ChatChannel.Dead:
        color = Color.MediumPurple;
        break;
      case ChatChannel.Admin:
        color = Color.Red;
        break;
      case ChatChannel.AdminAlert:
        color = Color.Red;
        break;
      case ChatChannel.AdminChat:
        color = Color.HotPink;
        break;
      case ChatChannel.MentorChat:
        color = Color.Orange;
        break;
      case ChatChannel.Party:
        color = Color.Gold;
        break;
      case ChatChannel.Lobby:
        color = Color.LightGreen;
        break;
      case ChatChannel.Killfeed:
        color = Color.Yellow;
        break;
      default:
        color = Color.LightGray;
        break;
    }
    return color;
  }
}
