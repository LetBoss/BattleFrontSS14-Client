using System;
using System.Collections.Generic;
using Content.Shared.Eui;
using Robust.Shared.Network;
using Robust.Shared.Serialization;

namespace Content.Shared.Administration;

[Serializable]
[NetSerializable]
public sealed class PermissionsEuiState : EuiStateBase
{
	[Serializable]
	[NetSerializable]
	public struct AdminData
	{
		public NetUserId UserId;

		public string? UserName;

		public string? Title;

		public bool Suspended;

		public AdminFlags PosFlags;

		public AdminFlags NegFlags;

		public int? RankId;
	}

	[Serializable]
	[NetSerializable]
	public struct AdminRankData
	{
		public string Name;

		public AdminFlags Flags;
	}

	public bool IsLoading;

	public AdminData[] Admins = Array.Empty<AdminData>();

	public Dictionary<int, AdminRankData> AdminRanks = new Dictionary<int, AdminRankData>();
}
