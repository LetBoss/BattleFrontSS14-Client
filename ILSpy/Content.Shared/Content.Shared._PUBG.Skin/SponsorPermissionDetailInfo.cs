using System;
using Robust.Shared.Serialization;

namespace Content.Shared._PUBG.Skin;

[Serializable]
[NetSerializable]
public sealed class SponsorPermissionDetailInfo
{
	public string Key { get; set; } = string.Empty;

	public string Name { get; set; } = string.Empty;

	public int StackCount { get; set; }

	public string Source { get; set; } = string.Empty;

	public DateTime? ExpiresAt { get; set; }
}
