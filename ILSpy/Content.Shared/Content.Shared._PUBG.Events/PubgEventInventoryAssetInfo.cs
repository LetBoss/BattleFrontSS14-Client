using System;
using Robust.Shared.Serialization;

namespace Content.Shared._PUBG.Events;

[Serializable]
[NetSerializable]
public sealed class PubgEventInventoryAssetInfo
{
	public string AssetKey { get; set; } = string.Empty;

	public string AssetType { get; set; } = string.Empty;

	public string TitleKey { get; set; } = string.Empty;

	public string? DescKey { get; set; }

	public string? IconPath { get; set; }

	public int Quantity { get; set; }

	public DateTime? ExpiresAt { get; set; }

	public bool IsExpired { get; set; }

	public string MetadataJson { get; set; } = "{}";
}
