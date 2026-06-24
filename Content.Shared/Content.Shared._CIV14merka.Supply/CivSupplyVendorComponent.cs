using System;
using System.Collections.Generic;
using Content.Shared.Whitelist;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.ViewVariables;

namespace Content.Shared._CIV14merka.Supply;

[RegisterComponent]
public sealed class CivSupplyVendorComponent : Component, ISerializationGenerated<CivSupplyVendorComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public List<EntProtoId> RestockBlacklist = new List<EntProtoId>();

	[DataField(null, false, 1, false, false, null)]
	public bool RestrictByTeam = true;

	[DataField(null, false, 1, false, false, null)]
	public CivSupplyVendorSide Side;

	[DataField(null, false, 1, false, false, null)]
	public string RequiredSideId = string.Empty;

	[DataField(null, false, 1, false, false, null)]
	public string StockGroup = string.Empty;

	[DataField(null, false, 1, false, false, null)]
	public bool StartEmpty;

	[DataField(null, false, 1, false, false, null)]
	public bool RequireSquadLeader;

	[DataField(null, false, 1, false, false, null)]
	public bool RequireSquadLeaderDuringBriefing = true;

	[DataField(null, false, 1, false, false, null)]
	public string NotSquadLeaderMessage = "Только сквадной может запрашивать припасы.";

	[DataField(null, false, 1, false, false, null)]
	public string WrongTeamMessage = "Это терминал другой стороны.";

	[DataField(null, false, 1, false, false, null)]
	public float AutoCleanupRadius = 5f;

	[DataField(null, false, 1, false, false, null)]
	public TimeSpan AutoCleanupInterval = TimeSpan.FromSeconds(15L);

	[ViewVariables]
	public TimeSpan NextAutoCleanup;

	[DataField(null, false, 1, false, false, null)]
	public float VehicleLoadRadius = 6f;

	[DataField(null, false, 1, false, false, null)]
	public EntityWhitelist? VehicleCargoWhitelist;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref CivSupplyVendorComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (CivSupplyVendorComponent)(object)definitionCast;
		if (serialization.TryCustomCopy<CivSupplyVendorComponent>(this, ref target, hookCtx, false, context))
		{
			return;
		}
		List<EntProtoId> RestockBlacklistTemp = null;
		if (RestockBlacklist == null)
		{
			throw new NullNotAllowedException();
		}
		if (!serialization.TryCustomCopy<List<EntProtoId>>(RestockBlacklist, ref RestockBlacklistTemp, hookCtx, true, context))
		{
			RestockBlacklistTemp = serialization.CreateCopy<List<EntProtoId>>(RestockBlacklist, hookCtx, context, false);
		}
		target.RestockBlacklist = RestockBlacklistTemp;
		bool RestrictByTeamTemp = false;
		if (!serialization.TryCustomCopy<bool>(RestrictByTeam, ref RestrictByTeamTemp, hookCtx, false, context))
		{
			RestrictByTeamTemp = RestrictByTeam;
		}
		target.RestrictByTeam = RestrictByTeamTemp;
		CivSupplyVendorSide SideTemp = CivSupplyVendorSide.Attack;
		if (!serialization.TryCustomCopy<CivSupplyVendorSide>(Side, ref SideTemp, hookCtx, false, context))
		{
			SideTemp = Side;
		}
		target.Side = SideTemp;
		string RequiredSideIdTemp = null;
		if (RequiredSideId == null)
		{
			throw new NullNotAllowedException();
		}
		if (!serialization.TryCustomCopy<string>(RequiredSideId, ref RequiredSideIdTemp, hookCtx, false, context))
		{
			RequiredSideIdTemp = RequiredSideId;
		}
		target.RequiredSideId = RequiredSideIdTemp;
		string StockGroupTemp = null;
		if (StockGroup == null)
		{
			throw new NullNotAllowedException();
		}
		if (!serialization.TryCustomCopy<string>(StockGroup, ref StockGroupTemp, hookCtx, false, context))
		{
			StockGroupTemp = StockGroup;
		}
		target.StockGroup = StockGroupTemp;
		bool StartEmptyTemp = false;
		if (!serialization.TryCustomCopy<bool>(StartEmpty, ref StartEmptyTemp, hookCtx, false, context))
		{
			StartEmptyTemp = StartEmpty;
		}
		target.StartEmpty = StartEmptyTemp;
		bool RequireSquadLeaderTemp = false;
		if (!serialization.TryCustomCopy<bool>(RequireSquadLeader, ref RequireSquadLeaderTemp, hookCtx, false, context))
		{
			RequireSquadLeaderTemp = RequireSquadLeader;
		}
		target.RequireSquadLeader = RequireSquadLeaderTemp;
		bool RequireSquadLeaderDuringBriefingTemp = false;
		if (!serialization.TryCustomCopy<bool>(RequireSquadLeaderDuringBriefing, ref RequireSquadLeaderDuringBriefingTemp, hookCtx, false, context))
		{
			RequireSquadLeaderDuringBriefingTemp = RequireSquadLeaderDuringBriefing;
		}
		target.RequireSquadLeaderDuringBriefing = RequireSquadLeaderDuringBriefingTemp;
		string NotSquadLeaderMessageTemp = null;
		if (NotSquadLeaderMessage == null)
		{
			throw new NullNotAllowedException();
		}
		if (!serialization.TryCustomCopy<string>(NotSquadLeaderMessage, ref NotSquadLeaderMessageTemp, hookCtx, false, context))
		{
			NotSquadLeaderMessageTemp = NotSquadLeaderMessage;
		}
		target.NotSquadLeaderMessage = NotSquadLeaderMessageTemp;
		string WrongTeamMessageTemp = null;
		if (WrongTeamMessage == null)
		{
			throw new NullNotAllowedException();
		}
		if (!serialization.TryCustomCopy<string>(WrongTeamMessage, ref WrongTeamMessageTemp, hookCtx, false, context))
		{
			WrongTeamMessageTemp = WrongTeamMessage;
		}
		target.WrongTeamMessage = WrongTeamMessageTemp;
		float AutoCleanupRadiusTemp = 0f;
		if (!serialization.TryCustomCopy<float>(AutoCleanupRadius, ref AutoCleanupRadiusTemp, hookCtx, false, context))
		{
			AutoCleanupRadiusTemp = AutoCleanupRadius;
		}
		target.AutoCleanupRadius = AutoCleanupRadiusTemp;
		TimeSpan AutoCleanupIntervalTemp = default(TimeSpan);
		if (!serialization.TryCustomCopy<TimeSpan>(AutoCleanupInterval, ref AutoCleanupIntervalTemp, hookCtx, false, context))
		{
			AutoCleanupIntervalTemp = serialization.CreateCopy<TimeSpan>(AutoCleanupInterval, hookCtx, context, false);
		}
		target.AutoCleanupInterval = AutoCleanupIntervalTemp;
		float VehicleLoadRadiusTemp = 0f;
		if (!serialization.TryCustomCopy<float>(VehicleLoadRadius, ref VehicleLoadRadiusTemp, hookCtx, false, context))
		{
			VehicleLoadRadiusTemp = VehicleLoadRadius;
		}
		target.VehicleLoadRadius = VehicleLoadRadiusTemp;
		EntityWhitelist VehicleCargoWhitelistTemp = null;
		if (!serialization.TryCustomCopy<EntityWhitelist>(VehicleCargoWhitelist, ref VehicleCargoWhitelistTemp, hookCtx, false, context))
		{
			if (VehicleCargoWhitelist == null)
			{
				VehicleCargoWhitelistTemp = null;
			}
			else
			{
				serialization.CopyTo<EntityWhitelist>(VehicleCargoWhitelist, ref VehicleCargoWhitelistTemp, hookCtx, context, false);
			}
		}
		target.VehicleCargoWhitelist = VehicleCargoWhitelistTemp;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref CivSupplyVendorComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		CivSupplyVendorComponent cast = (CivSupplyVendorComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		CivSupplyVendorComponent cast = (CivSupplyVendorComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		CivSupplyVendorComponent def = (CivSupplyVendorComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override CivSupplyVendorComponent Instantiate()
	{
		return new CivSupplyVendorComponent();
	}
}
