using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Content.Shared._RMC14.GameStates;
using Content.Shared.Administration.Logs;
using Content.Shared.CCVar;
using Content.Shared.Database;
using Content.Shared.GameTicking;
using Content.Shared.Mind;
using Content.Shared.Roles.Jobs;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;

namespace Content.Shared.Roles;

public abstract class SharedRoleSystem : EntitySystem
{
	[Dependency]
	private IConfigurationManager _cfg;

	[Dependency]
	private IEntityManager _entityManager;

	[Dependency]
	private IPrototypeManager _prototypes;

	[Dependency]
	private ISharedAdminLogManager _adminLogger;

	[Dependency]
	protected ISharedPlayerManager Player;

	[Dependency]
	private SharedAudioSystem _audio;

	[Dependency]
	private SharedMindSystem _minds;

	[Dependency]
	private SharedRMCPvsSystem _rmcPvs;

	private JobRequirementOverridePrototype? _requirementOverride;

	public override void Initialize()
	{
		EntitySystemSubscriptionExt.CVar<string>(((EntitySystem)this).Subs, _cfg, CCVars.GameRoleTimerOverride, (Action<string>)SetRequirementOverride, true);
		((EntitySystem)this).SubscribeLocalEvent<MindRoleComponent, ComponentShutdown>((EntityEventRefHandler<MindRoleComponent, ComponentShutdown>)OnComponentShutdown, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<StartingMindRoleComponent, PlayerSpawnCompleteEvent>((ComponentEventHandler<StartingMindRoleComponent, PlayerSpawnCompleteEvent>)OnSpawn, (Type[])null, (Type[])null);
	}

	private void OnSpawn(EntityUid uid, StartingMindRoleComponent component, PlayerSpawnCompleteEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		if (_minds.TryGetMind(uid, out EntityUid mindId, out MindComponent mindComp))
		{
			MindAddRole(mindId, component.MindRole, mindComp, component.Silent);
		}
	}

	private void SetRequirementOverride(string value)
	{
		if (string.IsNullOrEmpty(value))
		{
			_requirementOverride = null;
		}
		else if (!_prototypes.TryIndex<JobRequirementOverridePrototype>(value, ref _requirementOverride))
		{
			((EntitySystem)this).Log.Error("Unknown JobRequirementOverridePrototype: " + value);
		}
	}

	public void MindAddRoles(EntityUid mindId, List<EntProtoId>? roles, MindComponent? mind = null, bool silent = false)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		if (roles == null || roles.Count == 0)
		{
			return;
		}
		foreach (EntProtoId proto in roles)
		{
			MindAddRole(mindId, proto, mind, silent);
		}
	}

	public void MindAddRole(EntityUid mindId, EntProtoId protoId, MindComponent? mind = null, bool silent = false)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		if (protoId == EntProtoId.op_Implicit("MindRoleJob"))
		{
			MindAddJobRole(mindId, mind, silent, "");
		}
		else
		{
			MindAddRoleDo(mindId, protoId, mind, silent);
		}
	}

	public void MindAddJobRole(EntityUid mindId, MindComponent? mind = null, bool silent = false, string? jobPrototype = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<MindComponent>(mindId, ref mind, true))
		{
			return;
		}
		if (this.MindHasRole<JobRoleComponent>(Entity<MindComponent>.op_Implicit((mindId, mind)), out Entity<MindRoleComponent, JobRoleComponent>? jobRole))
		{
			ProtoId<JobPrototype>? jobPrototype2 = jobRole.Value.Comp1.JobPrototype;
			ProtoId<JobPrototype>? val = ProtoId<JobPrototype>.op_Implicit(jobPrototype);
			if (jobPrototype2.HasValue != val.HasValue || (jobPrototype2.HasValue && jobPrototype2.GetValueOrDefault() != val.GetValueOrDefault()))
			{
				ISharedAdminLogManager adminLogger = _adminLogger;
				LogStringHandler handler = new LogStringHandler(34, 3);
				handler.AppendLiteral("Job Role of ");
				handler.AppendFormatted(((EntitySystem)this).ToPrettyString(mind.OwnedEntity, (MetaDataComponent)null), "ToPrettyString(mind.OwnedEntity)");
				handler.AppendLiteral(" changed from '");
				handler.AppendFormatted(jobRole.Value.Comp1.JobPrototype, "jobRole.Value.Comp1.JobPrototype");
				handler.AppendLiteral("' to '");
				handler.AppendFormatted(jobPrototype);
				handler.AppendLiteral("'");
				adminLogger.Add(LogType.Mind, LogImpact.Low, ref handler);
				jobRole.Value.Comp1.JobPrototype = ProtoId<JobPrototype>.op_Implicit(jobPrototype);
				return;
			}
		}
		MindAddRoleDo(mindId, EntProtoId.op_Implicit("MindRoleJob"), mind, silent, jobPrototype);
	}

	private void MindAddRoleDo(EntityUid mindId, EntProtoId protoId, MindComponent? mind = null, bool silent = false, string? jobPrototype = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_024a: Unknown result type (might be due to invalid IL or missing references)
		//IL_024b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0250: Unknown result type (might be due to invalid IL or missing references)
		//IL_0279: Unknown result type (might be due to invalid IL or missing references)
		//IL_027e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0286: Unknown result type (might be due to invalid IL or missing references)
		//IL_0287: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<MindComponent>(mindId, ref mind, true))
		{
			((EntitySystem)this).Log.Error($"Failed to add role {protoId} to {((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(mindId))} : Mind does not match provided mind component");
			return;
		}
		EntityPrototype protoEnt = default(EntityPrototype);
		if (!_prototypes.TryIndex(protoId, ref protoEnt))
		{
			((EntitySystem)this).Log.Error($"Failed to add role {protoId} to {((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(mindId))} : Role prototype does not exist");
			return;
		}
		EntityUid mindRoleId = ((EntitySystem)this).Spawn(EntProtoId.op_Implicit(protoId), MapCoordinates.Nullspace, (ComponentRegistry)null, default(Angle));
		((EntitySystem)this).EnsureComp<MindRoleComponent>(mindRoleId);
		MindRoleComponent mindRoleComp = ((EntitySystem)this).Comp<MindRoleComponent>(mindRoleId);
		mindRoleComp.Mind = Entity<MindComponent>.op_Implicit((mindId, mind));
		if (jobPrototype != null)
		{
			mindRoleComp.JobPrototype = ProtoId<JobPrototype>.op_Implicit(jobPrototype);
			((EntitySystem)this).EnsureComp<JobRoleComponent>(mindRoleId);
		}
		((EntitySystem)this).Dirty(mindRoleId, (IComponent)(object)mindRoleComp, (MetaDataComponent)null);
		mind.MindRoles.Add(mindRoleId);
		((EntitySystem)this).Dirty(mindId, (IComponent)(object)mind, (MetaDataComponent)null);
		bool update = MindRolesUpdate(Entity<MindComponent>.op_Implicit((mindId, mind)));
		RoleAddedEvent message = new RoleAddedEvent(mindId, mind, update, silent);
		((EntitySystem)this).RaiseLocalEvent<RoleAddedEvent>(mindId, message, true);
		string name = base.Loc.GetString(protoEnt.Name);
		if (mind.OwnedEntity.HasValue)
		{
			ISharedAdminLogManager adminLogger = _adminLogger;
			LogStringHandler handler = new LogStringHandler(18, 2);
			handler.AppendFormatted(name);
			handler.AppendLiteral(" added to mind of ");
			handler.AppendFormatted(((EntitySystem)this).ToPrettyString(mind.OwnedEntity, (MetaDataComponent)null), "ToPrettyString(mind.OwnedEntity)");
			adminLogger.Add(LogType.Mind, LogImpact.Low, ref handler);
		}
		else
		{
			((EntitySystem)this).Log.Error($"{((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(mindId))} does not have an OwnedEntity!");
			ISharedAdminLogManager adminLogger2 = _adminLogger;
			LogStringHandler handler2 = new LogStringHandler(10, 2);
			handler2.AppendFormatted(name);
			handler2.AppendLiteral(" added to ");
			handler2.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(mindId)), "ToPrettyString(mindId)");
			adminLogger2.Add(LogType.Mind, LogImpact.Low, ref handler2);
		}
		NetUserId? originalOwnerUserId = mind.OriginalOwnerUserId;
		if (originalOwnerUserId.HasValue)
		{
			NetUserId session = originalOwnerUserId.GetValueOrDefault();
			_rmcPvs.AddSessionOverride(mindRoleId, session);
		}
	}

	private bool MindRolesUpdate(Entity<MindComponent?> ent)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<MindComponent>(ent.Owner, ref ent.Comp, true))
		{
			return false;
		}
		var (roleType, subtype) = GetRoleTypeByTime(ent.Comp);
		if (ent.Comp.RoleType == roleType)
		{
			LocId? subtype2 = ent.Comp.Subtype;
			LocId? val = subtype;
			if (subtype2.HasValue == val.HasValue && (!subtype2.HasValue || subtype2.GetValueOrDefault() == val.GetValueOrDefault()))
			{
				return false;
			}
		}
		SetRoleType(ent.Owner, roleType, subtype);
		return true;
	}

	private (ProtoId<RoleTypePrototype>, LocId?) GetRoleTypeByTime(MindComponent mind)
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		Entity<MindRoleComponent>? role = GetRoleCompByTime(mind);
		return ((ProtoId<RoleTypePrototype>)(((_003F?)role?.Comp?.RoleType) ?? ProtoId<RoleTypePrototype>.op_Implicit("Neutral")), role?.Comp?.Subtype);
	}

	public Entity<MindRoleComponent>? GetRoleCompByTime(MindComponent mind)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		List<Entity<MindRoleComponent>> roles = new List<Entity<MindRoleComponent>>();
		foreach (EntityUid role in mind.MindRoles)
		{
			MindRoleComponent comp = ((EntitySystem)this).Comp<MindRoleComponent>(role);
			ProtoId<RoleTypePrototype>? roleType = comp.RoleType;
			if (roleType.HasValue)
			{
				roles.Add(Entity<MindRoleComponent>.op_Implicit((role, comp)));
			}
		}
		if (roles.Count <= 0)
		{
			return null;
		}
		return roles.LastOrDefault();
	}

	private void SetRoleType(EntityUid mind, ProtoId<RoleTypePrototype> roleTypeId, LocId? subtype)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_027d: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_020b: Unknown result type (might be due to invalid IL or missing references)
		MindComponent comp = default(MindComponent);
		if (!((EntitySystem)this).TryComp<MindComponent>(mind, ref comp))
		{
			((EntitySystem)this).Log.Error($"Failed to update Role Type of mind entity {((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(mind))} to {roleTypeId}, {subtype}. MindComponent not found.");
			return;
		}
		if (!_prototypes.HasIndex<RoleTypePrototype>(roleTypeId))
		{
			((EntitySystem)this).Log.Error($"Failed to change Role Type of {_minds.MindOwnerLoggingString(comp)} to {roleTypeId}, {subtype}. Invalid role");
			return;
		}
		comp.RoleType = roleTypeId;
		comp.Subtype = subtype;
		((EntitySystem)this).Dirty(mind, (IComponent)(object)comp, (MetaDataComponent)null);
		ICommonSession session = default(ICommonSession);
		if (Player.TryGetSessionById(comp.UserId, ref session))
		{
			((EntitySystem)this).RaiseNetworkEvent((EntityEventArgs)(object)new MindRoleTypeChangedEvent(), session.Channel);
		}
		else
		{
			string error = "The Character Window of " + _minds.MindOwnerLoggingString(comp) + " potentially did not update immediately : session error";
			ISharedAdminLogManager adminLogger = _adminLogger;
			LogStringHandler handler = new LogStringHandler(0, 1);
			handler.AppendFormatted(error);
			adminLogger.Add(LogType.Mind, LogImpact.Medium, ref handler);
		}
		if (!comp.OwnedEntity.HasValue)
		{
			((EntitySystem)this).Log.Error($"{((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(mind))} does not have an OwnedEntity!");
			ISharedAdminLogManager adminLogger2 = _adminLogger;
			LogStringHandler handler2 = new LogStringHandler(27, 3);
			handler2.AppendLiteral("Role Type of ");
			handler2.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(mind)), "ToPrettyString(mind)");
			handler2.AppendLiteral(" changed to ");
			handler2.AppendFormatted<ProtoId<RoleTypePrototype>>(roleTypeId, "roleTypeId");
			handler2.AppendLiteral(", ");
			handler2.AppendFormatted(subtype, "subtype");
			adminLogger2.Add(LogType.Mind, LogImpact.Medium, ref handler2);
		}
		else
		{
			ISharedAdminLogManager adminLogger3 = _adminLogger;
			LogStringHandler handler3 = new LogStringHandler(27, 3);
			handler3.AppendLiteral("Role Type of ");
			handler3.AppendFormatted(((EntitySystem)this).ToPrettyString(comp.OwnedEntity, (MetaDataComponent)null), "ToPrettyString(comp.OwnedEntity)");
			handler3.AppendLiteral(" changed to ");
			handler3.AppendFormatted<ProtoId<RoleTypePrototype>>(roleTypeId, "roleTypeId");
			handler3.AppendLiteral(", ");
			handler3.AppendFormatted(subtype, "subtype");
			adminLogger3.Add(LogType.Mind, LogImpact.High, ref handler3);
		}
	}

	public bool MindRemoveRole<T>(Entity<MindComponent?> mind) where T : IComponent
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		if (typeof(T) == typeof(MindRoleComponent))
		{
			throw new InvalidOperationException();
		}
		if (!((EntitySystem)this).Resolve<MindComponent>(mind.Owner, ref mind.Comp, true))
		{
			return false;
		}
		List<EntityUid> delete = new List<EntityUid>();
		string original = "'" + typeof(T).Name + "'";
		string deleteName = original;
		foreach (EntityUid role in mind.Comp.MindRoles)
		{
			if (!((EntitySystem)this).HasComp<MindRoleComponent>(role))
			{
				((EntitySystem)this).Log.Error($"Encountered mind role entity {((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(role))} without a {"MindRoleComponent"}");
			}
			else if (((EntitySystem)this).HasComp<T>(role))
			{
				delete.Add(role);
				deleteName = RemoveRoleLogNameGeneration(deleteName, ((EntitySystem)this).MetaData(role).EntityName, original);
			}
		}
		return MindRemoveRoleDo(mind, delete, deleteName);
	}

	private string RemoveRoleLogNameGeneration(string name, string newName, string original)
	{
		if (name == original)
		{
			name = "'" + newName + "'";
		}
		else if (!name.Contains(newName))
		{
			name = name + ", '" + newName + "'";
		}
		return name;
	}

	public bool MindRemoveRole<T>(EntityUid mindId) where T : IComponent
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		MindComponent mind = default(MindComponent);
		if (!((EntitySystem)this).TryComp<MindComponent>(mindId, ref mind))
		{
			((EntitySystem)this).Log.Error($"The specified mind entity '{((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(mindId))}' does not have a {"MindComponent"}");
			return false;
		}
		return MindRemoveRole<T>(Entity<MindComponent>.op_Implicit((mindId, mind)));
	}

	public bool MindRemoveRole(Entity<MindComponent?> mind, EntProtoId<MindRoleComponent> protoId)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<MindComponent>(mind.Owner, ref mind.Comp, true))
		{
			return false;
		}
		string original = "'" + EntProtoId<MindRoleComponent>.op_Implicit(protoId) + "'";
		string deleteName = original;
		List<EntityUid> delete = new List<EntityUid>();
		foreach (EntityUid role in mind.Comp.MindRoles)
		{
			if (!((EntitySystem)this).HasComp<MindRoleComponent>(role))
			{
				((EntitySystem)this).Log.Error($"Encountered mind role entity {((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(role))} without a {"MindRoleComponent"}");
				continue;
			}
			EntityPrototype entityPrototype = ((EntitySystem)this).MetaData(role).EntityPrototype;
			string id = ((entityPrototype != null) ? entityPrototype.ID : null);
			if (id != null && !(EntProtoId<MindRoleComponent>.op_Implicit(id) != protoId))
			{
				delete.Add(role);
				deleteName = RemoveRoleLogNameGeneration(deleteName, ((EntitySystem)this).MetaData(role).EntityName, original);
			}
		}
		return MindRemoveRoleDo(mind, delete, deleteName);
	}

	private bool MindRemoveRoleDo(Entity<MindComponent?> mind, List<EntityUid> delete, string? logName = "")
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<MindComponent>(mind.Owner, ref mind.Comp, true))
		{
			return false;
		}
		if (delete.Count <= 0)
		{
			((EntitySystem)this).Log.Warning($"Failed to remove mind role {logName} from {((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(mind.Owner))} : mind does not have this role ");
			return false;
		}
		foreach (EntityUid role in delete)
		{
			_entityManager.DeleteEntity((EntityUid?)role);
		}
		bool update = MindRolesUpdate(mind);
		RoleRemovedEvent message = new RoleRemovedEvent(mind.Owner, mind.Comp, update);
		((EntitySystem)this).RaiseLocalEvent<RoleRemovedEvent>(Entity<MindComponent>.op_Implicit(mind), message, true);
		ISharedAdminLogManager adminLogger = _adminLogger;
		LogStringHandler handler = new LogStringHandler(40, 2);
		handler.AppendLiteral("All roles of type ");
		handler.AppendFormatted(logName);
		handler.AppendLiteral(" removed from mind of ");
		handler.AppendFormatted(((EntitySystem)this).ToPrettyString(mind.Comp.OwnedEntity, (MetaDataComponent)null), "ToPrettyString(mind.Comp.OwnedEntity)");
		adminLogger.Add(LogType.Mind, LogImpact.Low, ref handler);
		return true;
	}

	private void OnComponentShutdown(Entity<MindRoleComponent> ent, ref ComponentShutdown args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		if (ent.Comp.Mind.Comp != null)
		{
			ent.Comp.Mind.Comp.MindRoles.Remove(ent.Owner);
		}
	}

	public bool MindHasRole<T>(Entity<MindComponent?> mind, [NotNullWhen(true)] out Entity<MindRoleComponent, T>? role) where T : IComponent
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		role = null;
		if (!((EntitySystem)this).Resolve<MindComponent>(mind.Owner, ref mind.Comp, true))
		{
			return false;
		}
		T tcomp = default(T);
		MindRoleComponent roleComp = default(MindRoleComponent);
		foreach (EntityUid roleEnt in mind.Comp.MindRoles)
		{
			if (((EntitySystem)this).TryComp<T>(roleEnt, ref tcomp))
			{
				if (((EntitySystem)this).TryComp<MindRoleComponent>(roleEnt, ref roleComp))
				{
					role = Entity<MindRoleComponent, T>.op_Implicit((roleEnt, roleComp, tcomp));
					return true;
				}
				((EntitySystem)this).Log.Error($"Encountered mind role entity {((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(roleEnt))} without a {"MindRoleComponent"}");
			}
		}
		return false;
	}

	public bool MindHasRole(EntityUid mindId, Type type, [NotNullWhen(true)] out Entity<MindRoleComponent>? role)
	{
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		role = null;
		if (type == Type.GetType("MindRoleComponent"))
		{
			((EntitySystem)this).Log.Error($"Something attempted to query mind role 'MindRoleComponent' on mind {mindId}. This component is present on every single mind role.");
			return false;
		}
		MindComponent mind = default(MindComponent);
		if (!((EntitySystem)this).TryComp<MindComponent>(mindId, ref mind))
		{
			return false;
		}
		bool found = false;
		MindRoleComponent roleComp = default(MindRoleComponent);
		foreach (EntityUid roleEnt in mind.MindRoles)
		{
			if (((EntitySystem)this).HasComp(roleEnt, type))
			{
				if (((EntitySystem)this).TryComp<MindRoleComponent>(roleEnt, ref roleComp))
				{
					role = Entity<MindRoleComponent>.op_Implicit((roleEnt, roleComp));
					found = true;
					break;
				}
				((EntitySystem)this).Log.Error($"Encountered mind role entity {((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(roleEnt))} without a {"MindRoleComponent"}");
			}
		}
		return found;
	}

	public bool MindHasRole<T>(EntityUid mindId) where T : IComponent
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		Entity<MindRoleComponent, T>? role;
		return this.MindHasRole<T>(Entity<MindComponent>.op_Implicit(mindId), out role);
	}

	[Obsolete("Use MindHasRole's output value")]
	public Entity<MindRoleComponent>? MindGetRole<T>(EntityUid mindId) where T : IComponent
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		Entity<MindRoleComponent>? result = null;
		MindRoleComponent comp = default(MindRoleComponent);
		foreach (EntityUid uid in ((EntitySystem)this).Comp<MindComponent>(mindId).MindRoles)
		{
			if (((EntitySystem)this).HasComp<T>(uid) && ((EntitySystem)this).TryComp<MindRoleComponent>(uid, ref comp))
			{
				result = Entity<MindRoleComponent>.op_Implicit((uid, comp));
			}
		}
		return result;
	}

	public List<RoleInfo> MindGetAllRoleInfo(Entity<MindComponent?> mind)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0204: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_0254: Unknown result type (might be due to invalid IL or missing references)
		List<RoleInfo> roleInfo = new List<RoleInfo>();
		if (!((EntitySystem)this).Resolve<MindComponent>(mind.Owner, ref mind.Comp, true))
		{
			return roleInfo;
		}
		MindRoleComponent comp = default(MindRoleComponent);
		JobPrototype job = default(JobPrototype);
		AntagPrototype antag = default(AntagPrototype);
		foreach (EntityUid role in mind.Comp.MindRoles)
		{
			bool valid = false;
			string name = "game-ticker-unknown-role";
			string prototype = "";
			string playTimeTracker = null;
			if (!((EntitySystem)this).TryComp<MindRoleComponent>(role, ref comp))
			{
				((EntitySystem)this).Log.Error($"Encountered mind role entity {((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(role))} without a {"MindRoleComponent"}");
				continue;
			}
			if (comp.AntagPrototype.HasValue)
			{
				ProtoId<AntagPrototype>? antagPrototype = comp.AntagPrototype;
				prototype = (antagPrototype.HasValue ? ProtoId<AntagPrototype>.op_Implicit(antagPrototype.GetValueOrDefault()) : null);
			}
			if (comp.JobPrototype.HasValue && !comp.AntagPrototype.HasValue)
			{
				ProtoId<JobPrototype>? jobPrototype = comp.JobPrototype;
				prototype = (jobPrototype.HasValue ? ProtoId<JobPrototype>.op_Implicit(jobPrototype.GetValueOrDefault()) : null);
				if (_prototypes.TryIndex<JobPrototype>(comp.JobPrototype, ref job))
				{
					playTimeTracker = job.PlayTimeTracker;
					name = job.Name;
					valid = true;
				}
				else
				{
					((EntitySystem)this).Log.Error($" Mind Role Prototype '{role.Id}' contains invalid Job prototype: '{comp.JobPrototype}'");
				}
			}
			else if (comp.AntagPrototype.HasValue && !comp.JobPrototype.HasValue)
			{
				ProtoId<AntagPrototype>? antagPrototype = comp.AntagPrototype;
				prototype = (antagPrototype.HasValue ? ProtoId<AntagPrototype>.op_Implicit(antagPrototype.GetValueOrDefault()) : null);
				if (_prototypes.TryIndex<AntagPrototype>(comp.AntagPrototype, ref antag))
				{
					name = antag.Name;
					valid = true;
				}
				else
				{
					((EntitySystem)this).Log.Error($" Mind Role Prototype '{role.Id}' contains invalid Antagonist prototype: '{comp.AntagPrototype}'");
				}
			}
			else if (comp.JobPrototype.HasValue && comp.AntagPrototype.HasValue)
			{
				((EntitySystem)this).Log.Error($" Mind Role Prototype '{role.Id}' contains both Job and Antagonist prototypes");
			}
			if (valid)
			{
				roleInfo.Add(new RoleInfo(name, comp.Antag, playTimeTracker, prototype));
			}
		}
		return roleInfo;
	}

	public bool MindIsAntagonist(EntityUid? mindId)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		if (!mindId.HasValue)
		{
			return false;
		}
		return CheckAntagonistStatus(Entity<MindComponent>.op_Implicit(mindId.Value)).Antag;
	}

	public bool MindIsExclusiveAntagonist(EntityUid? mindId)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		if (!mindId.HasValue)
		{
			return false;
		}
		return CheckAntagonistStatus(Entity<MindComponent>.op_Implicit(mindId.Value)).ExclusiveAntag;
	}

	private (bool Antag, bool ExclusiveAntag) CheckAntagonistStatus(Entity<MindComponent?> mind)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<MindComponent>(mind.Owner, ref mind.Comp, true))
		{
			return (Antag: false, ExclusiveAntag: false);
		}
		bool antagonist = false;
		bool exclusiveAntag = false;
		MindRoleComponent roleComp = default(MindRoleComponent);
		foreach (EntityUid role in mind.Comp.MindRoles)
		{
			if (!((EntitySystem)this).TryComp<MindRoleComponent>(role, ref roleComp))
			{
				((EntitySystem)this).Log.Error($"Mind Role Entity {((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(role))} does not have a MindRoleComponent, despite being listed as a role belonging to {((EntitySystem)this).ToPrettyString((EntityUid?)Entity<MindComponent>.op_Implicit(mind), (MetaDataComponent)null)}|");
			}
			else
			{
				antagonist |= roleComp.Antag;
				exclusiveAntag |= roleComp.ExclusiveAntag;
			}
		}
		return (Antag: antagonist, ExclusiveAntag: exclusiveAntag);
	}

	public void MindPlaySound(EntityUid mindId, SoundSpecifier? sound, MindComponent? mind = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		ICommonSession session = default(ICommonSession);
		if (((EntitySystem)this).Resolve<MindComponent>(mindId, ref mind, true) && Player.TryGetSessionById(mind.UserId, ref session))
		{
			_audio.PlayGlobal(sound, session, (AudioParams?)null);
		}
	}

	public HashSet<JobRequirement>? GetJobRequirement(JobPrototype job)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		if (_requirementOverride != null && _requirementOverride.Jobs.TryGetValue(ProtoId<JobPrototype>.op_Implicit(job.ID), out HashSet<JobRequirement> req))
		{
			return req;
		}
		return job.Requirements;
	}

	public HashSet<JobRequirement>? GetJobRequirement(ProtoId<JobPrototype> job)
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		if (_requirementOverride != null && _requirementOverride.Jobs.TryGetValue(job, out HashSet<JobRequirement> req))
		{
			return req;
		}
		return _prototypes.Index<JobPrototype>(job).Requirements;
	}

	public HashSet<JobRequirement>? GetAntagRequirement(ProtoId<AntagPrototype> antag)
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		if (_requirementOverride != null && _requirementOverride.Antags.TryGetValue(antag, out HashSet<JobRequirement> req))
		{
			return req;
		}
		return _prototypes.Index<AntagPrototype>(antag).Requirements;
	}

	public HashSet<JobRequirement>? GetAntagRequirement(AntagPrototype antag)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		if (_requirementOverride != null && _requirementOverride.Antags.TryGetValue(ProtoId<AntagPrototype>.op_Implicit(antag.ID), out HashSet<JobRequirement> req))
		{
			return req;
		}
		return antag.Requirements;
	}

	public string GetRoleSubtypeLabel(LocId roleType, LocId? subtype)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		LocId? val = subtype;
		if (!string.IsNullOrEmpty(val.HasValue ? LocId.op_Implicit(val.GetValueOrDefault()) : null))
		{
			ILocalizationManager loc = base.Loc;
			val = subtype;
			return loc.GetString(val.HasValue ? LocId.op_Implicit(val.GetValueOrDefault()) : null);
		}
		return base.Loc.GetString(LocId.op_Implicit(roleType));
	}
}
