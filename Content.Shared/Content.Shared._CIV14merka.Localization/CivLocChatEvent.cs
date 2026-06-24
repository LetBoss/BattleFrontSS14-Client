using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;

namespace Content.Shared._CIV14merka.Localization;

[Serializable]
[NetSerializable]
public sealed class CivLocChatEvent : EntityEventArgs
{
	public CivLocMessage Body;

	public CivAnnouncementKind Kind;

	public Color Color;

	public Color SideColor;

	public string SideTag = string.Empty;

	public string Speaker = string.Empty;

	public string ChannelTag = string.Empty;
}
