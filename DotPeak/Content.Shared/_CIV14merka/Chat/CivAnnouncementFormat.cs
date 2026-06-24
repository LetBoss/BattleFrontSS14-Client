// Decompiled with JetBrains decompiler
// Type: Content.Shared._CIV14merka.Chat.CivAnnouncementFormat
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._CIV14merka.Localization;
using Robust.Shared.Localization;
using Robust.Shared.Maths;
using Robust.Shared.Utility;

#nullable enable
namespace Content.Shared._CIV14merka.Chat;

public static class CivAnnouncementFormat
{
  public static string BuildSideTag(Color sideColor, string sideTagText)
  {
    return $"[color={((Color) ref sideColor).ToHex()}][{FormattedMessage.EscapeText(sideTagText)}][/color]";
  }

  public static string BuildWrapped(
    CivAnnouncementKind kind,
    Color color,
    Color sideColor,
    string sideTagText,
    string body,
    string speakerName = "",
    string channelTag = "")
  {
    string str1 = CivAnnouncementFormat.BuildSideTag(sideColor, sideTagText);
    string str2 = FormattedMessage.EscapeText(body);
    switch (kind)
    {
      case CivAnnouncementKind.TeamSystem:
        string str3 = $"{str1} {FormattedMessage.EscapeText("[Team] [SYS]")}";
        string str4 = FormattedMessage.EscapeText(Loc.GetString("civ-sys-chat-speaker-system"));
        return $"[color={((Color) ref color).ToHex()}][bold]{str3} {str4}:[/bold] {str2}[/color]";
      case CivAnnouncementKind.SquadAnnounce:
        string str5 = $"{str1} {FormattedMessage.EscapeText($"[Squad] [{Loc.GetString("civ-sys-chat-tag-command")}]")}";
        string str6 = FormattedMessage.EscapeText(Loc.GetString("civ-sys-chat-speaker-command"));
        return $"[color={((Color) ref color).ToHex()}][bold]{str5} {str6}[/bold] {Loc.GetString("civ-sys-chat-reports")} [bold]{str2}[/bold][/color]";
      case CivAnnouncementKind.PlayerNotice:
        string str7 = $"{str1} {FormattedMessage.EscapeText(channelTag)}";
        string str8 = FormattedMessage.EscapeText(speakerName);
        return $"[color={((Color) ref color).ToHex()}][bold]{str7} {str8}:[/bold] {str2}[/color]";
      default:
        string str9 = $"{str1} {FormattedMessage.EscapeText("[TEAM] [OBJ]")}";
        string str10 = FormattedMessage.EscapeText(Loc.GetString("civ-sys-chat-speaker-command"));
        return $"[color={((Color) ref color).ToHex()}][bold]{str9} {str10}[/bold] {Loc.GetString("civ-sys-chat-reports")} [bold]{str2}[/bold][/color]";
    }
  }
}
