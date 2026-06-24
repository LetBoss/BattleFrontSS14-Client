using System;
using System.Runtime.CompilerServices;
using Content.Shared.Mind;
using Robust.Shared.GameObjects;
using Robust.Shared.Localization;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;

namespace Content.Shared.Administration;

[Serializable]
[NetSerializable]
public sealed record PlayerInfo(string Username, string CharacterName, string IdentityName, string StartingJob, bool Antag, ProtoId<RoleTypePrototype>? RoleProto, LocId? Subtype, int SortWeight, NetEntity? NetEntity, NetUserId SessionId, bool Connected, bool ActiveThisRound, TimeSpan? OverallPlaytime)
{
	public bool IsPinned { get; set; }

	public string PlaytimeString => _playtimeString ?? (_playtimeString = OverallPlaytime?.ToString("%d':'hh':'mm") ?? Loc.GetString("generic-unknown-title"));

	private string? _playtimeString;

	public bool Equals(PlayerInfo? other)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		NetUserId? val = other?.SessionId;
		NetUserId sessionId = SessionId;
		if (!val.HasValue)
		{
			return false;
		}
		return val.GetValueOrDefault() == sessionId;
	}

	public override int GetHashCode()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		return ((object)SessionId/*cast due to constrained. prefix*/).GetHashCode();
	}

	[CompilerGenerated]
	private PlayerInfo(PlayerInfo original)
	{
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		Username = original.Username;
		CharacterName = original.CharacterName;
		IdentityName = original.IdentityName;
		StartingJob = original.StartingJob;
		Antag = original.Antag;
		RoleProto = original.RoleProto;
		Subtype = original.Subtype;
		SortWeight = original.SortWeight;
		NetEntity = original.NetEntity;
		SessionId = original.SessionId;
		Connected = original.Connected;
		ActiveThisRound = original.ActiveThisRound;
		OverallPlaytime = original.OverallPlaytime;
		_playtimeString = original._playtimeString;
		IsPinned = original.IsPinned;
	}
}
