using System;
using System.Collections.Generic;
using Content.Shared._RMC14.Tracker.SquadLeader;
using Content.Shared.Access;
using Content.Shared.Radio;
using Content.Shared.Roles;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Utility;

namespace Content.Shared._RMC14.Marines.Squads;

[RegisterComponent]
[NetworkedComponent]
[Access(new Type[] { typeof(SquadSystem) })]
[EntityCategory(new string[] { "Squads" })]
public sealed class SquadTeamComponent : Component, ISerializationGenerated<SquadTeamComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public bool RoundStart;

	[DataField(null, false, 1, true, false, null)]
	public Color Color;

	[DataField(null, false, 1, false, false, null)]
	public Color? AccessibleColor;

	[DataField(null, false, 1, true, false, null)]
	public ProtoId<RadioChannelPrototype>? Radio;

	[DataField(null, false, 1, true, false, null)]
	public SpriteSpecifier Background;

	[DataField(null, false, 1, false, false, null)]
	public Rsi? MinimapBackground;

	[DataField(null, false, 1, false, false, null)]
	public ProtoId<AccessLevelPrototype>[] AccessLevels = Array.Empty<ProtoId<AccessLevelPrototype>>();

	[DataField(null, false, 1, false, false, null)]
	public HashSet<EntityUid> Members = new HashSet<EntityUid>();

	[DataField(null, false, 1, false, false, null)]
	public Dictionary<ProtoId<JobPrototype>, int> Roles = new Dictionary<ProtoId<JobPrototype>, int>();

	[DataField(null, false, 1, false, false, null)]
	public Dictionary<ProtoId<JobPrototype>, int> MaxRoles = new Dictionary<ProtoId<JobPrototype>, int>();

	[DataField(null, false, 1, false, false, null)]
	public bool CanSupplyDrop = true;

	[DataField(null, false, 1, false, false, null)]
	public List<SquadArmorLayers> BlacklistedSquadArmor = new List<SquadArmorLayers>();

	[DataField(null, false, 1, false, false, null)]
	[Access(new Type[] { typeof(SquadLeaderTrackerSystem) })]
	public FireteamData Fireteams = new FireteamData();

	[DataField(null, false, 1, false, false, null)]
	public string Group = "UNMC";

	[DataField(null, false, 1, false, false, null)]
	public Rsi LeaderIcon = new Rsi(new ResPath("_RMC14/Interface/cm_job_icons.rsi"), "hudsquad_leader_a");

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref SquadTeamComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0236: Unknown result type (might be due to invalid IL or missing references)
		//IL_029e: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0337: Unknown result type (might be due to invalid IL or missing references)
		//IL_036d: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (SquadTeamComponent)(object)definitionCast;
		if (serialization.TryCustomCopy<SquadTeamComponent>(this, ref target, hookCtx, false, context))
		{
			return;
		}
		bool RoundStartTemp = false;
		if (!serialization.TryCustomCopy<bool>(RoundStart, ref RoundStartTemp, hookCtx, false, context))
		{
			RoundStartTemp = RoundStart;
		}
		target.RoundStart = RoundStartTemp;
		Color ColorTemp = default(Color);
		if (!serialization.TryCustomCopy<Color>(Color, ref ColorTemp, hookCtx, false, context))
		{
			ColorTemp = serialization.CreateCopy<Color>(Color, hookCtx, context, false);
		}
		target.Color = ColorTemp;
		Color? AccessibleColorTemp = null;
		if (!serialization.TryCustomCopy<Color?>(AccessibleColor, ref AccessibleColorTemp, hookCtx, false, context))
		{
			AccessibleColorTemp = serialization.CreateCopy<Color?>(AccessibleColor, hookCtx, context, false);
		}
		target.AccessibleColor = AccessibleColorTemp;
		ProtoId<RadioChannelPrototype>? RadioTemp = null;
		if (!serialization.TryCustomCopy<ProtoId<RadioChannelPrototype>?>(Radio, ref RadioTemp, hookCtx, false, context))
		{
			RadioTemp = serialization.CreateCopy<ProtoId<RadioChannelPrototype>?>(Radio, hookCtx, context, false);
		}
		target.Radio = RadioTemp;
		SpriteSpecifier BackgroundTemp = null;
		if (Background == null)
		{
			throw new NullNotAllowedException();
		}
		if (!serialization.TryCustomCopy<SpriteSpecifier>(Background, ref BackgroundTemp, hookCtx, true, context))
		{
			BackgroundTemp = serialization.CreateCopy<SpriteSpecifier>(Background, hookCtx, context, false);
		}
		target.Background = BackgroundTemp;
		Rsi MinimapBackgroundTemp = null;
		if (!serialization.TryCustomCopy<Rsi>(MinimapBackground, ref MinimapBackgroundTemp, hookCtx, false, context))
		{
			if (MinimapBackground == null)
			{
				MinimapBackgroundTemp = null;
			}
			else
			{
				serialization.CopyTo<Rsi>(MinimapBackground, ref MinimapBackgroundTemp, hookCtx, context, false);
			}
		}
		target.MinimapBackground = MinimapBackgroundTemp;
		ProtoId<AccessLevelPrototype>[] AccessLevelsTemp = null;
		if (AccessLevels == null)
		{
			throw new NullNotAllowedException();
		}
		if (!serialization.TryCustomCopy<ProtoId<AccessLevelPrototype>[]>(AccessLevels, ref AccessLevelsTemp, hookCtx, true, context))
		{
			AccessLevelsTemp = serialization.CreateCopy<ProtoId<AccessLevelPrototype>[]>(AccessLevels, hookCtx, context, false);
		}
		target.AccessLevels = AccessLevelsTemp;
		HashSet<EntityUid> MembersTemp = null;
		if (Members == null)
		{
			throw new NullNotAllowedException();
		}
		if (!serialization.TryCustomCopy<HashSet<EntityUid>>(Members, ref MembersTemp, hookCtx, true, context))
		{
			MembersTemp = serialization.CreateCopy<HashSet<EntityUid>>(Members, hookCtx, context, false);
		}
		target.Members = MembersTemp;
		Dictionary<ProtoId<JobPrototype>, int> RolesTemp = null;
		if (Roles == null)
		{
			throw new NullNotAllowedException();
		}
		if (!serialization.TryCustomCopy<Dictionary<ProtoId<JobPrototype>, int>>(Roles, ref RolesTemp, hookCtx, true, context))
		{
			RolesTemp = serialization.CreateCopy<Dictionary<ProtoId<JobPrototype>, int>>(Roles, hookCtx, context, false);
		}
		target.Roles = RolesTemp;
		Dictionary<ProtoId<JobPrototype>, int> MaxRolesTemp = null;
		if (MaxRoles == null)
		{
			throw new NullNotAllowedException();
		}
		if (!serialization.TryCustomCopy<Dictionary<ProtoId<JobPrototype>, int>>(MaxRoles, ref MaxRolesTemp, hookCtx, true, context))
		{
			MaxRolesTemp = serialization.CreateCopy<Dictionary<ProtoId<JobPrototype>, int>>(MaxRoles, hookCtx, context, false);
		}
		target.MaxRoles = MaxRolesTemp;
		bool CanSupplyDropTemp = false;
		if (!serialization.TryCustomCopy<bool>(CanSupplyDrop, ref CanSupplyDropTemp, hookCtx, false, context))
		{
			CanSupplyDropTemp = CanSupplyDrop;
		}
		target.CanSupplyDrop = CanSupplyDropTemp;
		List<SquadArmorLayers> BlacklistedSquadArmorTemp = null;
		if (BlacklistedSquadArmor == null)
		{
			throw new NullNotAllowedException();
		}
		if (!serialization.TryCustomCopy<List<SquadArmorLayers>>(BlacklistedSquadArmor, ref BlacklistedSquadArmorTemp, hookCtx, true, context))
		{
			BlacklistedSquadArmorTemp = serialization.CreateCopy<List<SquadArmorLayers>>(BlacklistedSquadArmor, hookCtx, context, false);
		}
		target.BlacklistedSquadArmor = BlacklistedSquadArmorTemp;
		FireteamData FireteamsTemp = null;
		if (Fireteams == null)
		{
			throw new NullNotAllowedException();
		}
		if (!serialization.TryCustomCopy<FireteamData>(Fireteams, ref FireteamsTemp, hookCtx, true, context))
		{
			if (Fireteams == null)
			{
				FireteamsTemp = null;
			}
			else
			{
				serialization.CopyTo<FireteamData>(Fireteams, ref FireteamsTemp, hookCtx, context, true);
			}
		}
		target.Fireteams = FireteamsTemp;
		string GroupTemp = null;
		if (Group == null)
		{
			throw new NullNotAllowedException();
		}
		if (!serialization.TryCustomCopy<string>(Group, ref GroupTemp, hookCtx, false, context))
		{
			GroupTemp = Group;
		}
		target.Group = GroupTemp;
		Rsi LeaderIconTemp = null;
		if (LeaderIcon == null)
		{
			throw new NullNotAllowedException();
		}
		if (!serialization.TryCustomCopy<Rsi>(LeaderIcon, ref LeaderIconTemp, hookCtx, false, context))
		{
			if (LeaderIcon == null)
			{
				LeaderIconTemp = null;
			}
			else
			{
				serialization.CopyTo<Rsi>(LeaderIcon, ref LeaderIconTemp, hookCtx, context, true);
			}
		}
		target.LeaderIcon = LeaderIconTemp;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref SquadTeamComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		SquadTeamComponent cast = (SquadTeamComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		SquadTeamComponent cast = (SquadTeamComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		SquadTeamComponent def = (SquadTeamComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override SquadTeamComponent Instantiate()
	{
		return new SquadTeamComponent();
	}
}
