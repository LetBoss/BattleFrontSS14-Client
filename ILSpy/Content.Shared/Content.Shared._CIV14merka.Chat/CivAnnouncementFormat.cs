using Content.Shared._CIV14merka.Localization;
using Robust.Shared.Localization;
using Robust.Shared.Maths;
using Robust.Shared.Utility;

namespace Content.Shared._CIV14merka.Chat;

public static class CivAnnouncementFormat
{
	public static string BuildSideTag(Color sideColor, string sideTagText)
	{
		return $"[color={((Color)(ref sideColor)).ToHex()}][{FormattedMessage.EscapeText(sideTagText)}][/color]";
	}

	public static string BuildWrapped(CivAnnouncementKind kind, Color color, Color sideColor, string sideTagText, string body, string speakerName = "", string channelTag = "")
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		string sideTag = BuildSideTag(sideColor, sideTagText);
		string escapedBody = FormattedMessage.EscapeText(body);
		switch (kind)
		{
		case CivAnnouncementKind.PlayerNotice:
		{
			string prefix4 = sideTag + " " + FormattedMessage.EscapeText(channelTag);
			string name = FormattedMessage.EscapeText(speakerName);
			return $"[color={((Color)(ref color)).ToHex()}][bold]{prefix4} {name}:[/bold] {escapedBody}[/color]";
		}
		case CivAnnouncementKind.SquadAnnounce:
		{
			string prefix3 = sideTag + " " + FormattedMessage.EscapeText("[Squad] [" + Loc.GetString("civ-sys-chat-tag-command") + "]");
			string speaker3 = FormattedMessage.EscapeText(Loc.GetString("civ-sys-chat-speaker-command"));
			return $"[color={((Color)(ref color)).ToHex()}][bold]{prefix3} {speaker3}[/bold] {Loc.GetString("civ-sys-chat-reports")} [bold]{escapedBody}[/bold][/color]";
		}
		case CivAnnouncementKind.TeamSystem:
		{
			string prefix2 = sideTag + " " + FormattedMessage.EscapeText("[Team] [SYS]");
			string speaker2 = FormattedMessage.EscapeText(Loc.GetString("civ-sys-chat-speaker-system"));
			return $"[color={((Color)(ref color)).ToHex()}][bold]{prefix2} {speaker2}:[/bold] {escapedBody}[/color]";
		}
		default:
		{
			string prefix = sideTag + " " + FormattedMessage.EscapeText("[TEAM] [OBJ]");
			string speaker = FormattedMessage.EscapeText(Loc.GetString("civ-sys-chat-speaker-command"));
			return $"[color={((Color)(ref color)).ToHex()}][bold]{prefix} {speaker}[/bold] {Loc.GetString("civ-sys-chat-reports")} [bold]{escapedBody}[/bold][/color]";
		}
		}
	}
}
