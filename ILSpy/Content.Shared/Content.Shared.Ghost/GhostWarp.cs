using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Ghost;

[Serializable]
[NetSerializable]
public struct GhostWarp(NetEntity entity, string displayName, bool isWarpPoint, bool isPubgMap, bool canWarp, int? gameInstanceId = null, int? partyId = null)
{
	public NetEntity Entity { get; } = entity;

	public string DisplayName { get; } = displayName;

	public bool IsWarpPoint { get; } = isWarpPoint;

	public bool IsPubgMap { get; } = isPubgMap;

	public bool CanWarp { get; } = canWarp;

	public int? GameInstanceId { get; } = gameInstanceId;

	public int? PartyId { get; } = partyId;
}
