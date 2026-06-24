using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._PUBG.Loadout;

[Serializable]
[NetSerializable]
public sealed class PubgLoadoutActionResultMessage : EntityEventArgs
{
	public PubgLoadoutActionType Action { get; }

	public NetEntity Item { get; }

	public bool Success { get; }

	public string? ErrorLocKey { get; }

	public PubgLoadoutActionResultMessage(PubgLoadoutActionType action, NetEntity item, bool success, string? errorLocKey = null)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		Action = action;
		Item = item;
		Success = success;
		ErrorLocKey = errorLocKey;
	}
}
