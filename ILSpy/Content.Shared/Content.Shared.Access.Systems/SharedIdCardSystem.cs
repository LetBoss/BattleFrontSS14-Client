using System;
using System.Collections.Generic;
using System.Globalization;
using Content.Shared.Access.Components;
using Content.Shared.Administration.Logs;
using Content.Shared.CCVar;
using Content.Shared.Database;
using Content.Shared.Hands.Components;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.IdentityManagement;
using Content.Shared.Inventory;
using Content.Shared.PDA;
using Content.Shared.Roles;
using Content.Shared.StatusIcon;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;

namespace Content.Shared.Access.Systems;

public abstract class SharedIdCardSystem : EntitySystem
{
	[Dependency]
	private IConfigurationManager _cfgManager;

	[Dependency]
	private ISharedAdminLogManager _adminLogger;

	[Dependency]
	private IGameTiming _timing;

	[Dependency]
	private SharedAccessSystem _access;

	[Dependency]
	private SharedHandsSystem _hands;

	[Dependency]
	private InventorySystem _inventorySystem;

	[Dependency]
	private MetaDataSystem _metaSystem;

	[Dependency]
	private IPrototypeManager _prototypeManager;

	private int _maxNameLength;

	private int _maxIdJobLength;

	private readonly List<EntityUid> _toRename = new List<EntityUid>();

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<IdCardComponent, MapInitEvent>((ComponentEventHandler<IdCardComponent, MapInitEvent>)OnMapInit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<TryGetIdentityShortInfoEvent>((EntityEventHandler<TryGetIdentityShortInfoEvent>)OnTryGetIdentityShortInfo, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<EntityRenamedEvent>((EntityEventRefHandler<EntityRenamedEvent>)OnRename, (Type[])null, (Type[])null);
		EntitySystemSubscriptionExt.CVar<int>(((EntitySystem)this).Subs, _cfgManager, CCVars.MaxNameLength, (Action<int>)delegate(int value)
		{
			_maxNameLength = value;
		}, true);
		EntitySystemSubscriptionExt.CVar<int>(((EntitySystem)this).Subs, _cfgManager, CCVars.MaxIdJobLength, (Action<int>)delegate(int value)
		{
			_maxIdJobLength = value;
		}, true);
	}

	private void OnRename(ref EntityRenamedEvent ev)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).HasComp<IdCardComponent>(((EntityRenamedEvent)(ref ev)).Uid) && !((EntitySystem)this).HasComp<PdaComponent>(((EntityRenamedEvent)(ref ev)).Uid))
		{
			_toRename.Add(((EntityRenamedEvent)(ref ev)).Uid);
		}
	}

	private void OnMapInit(EntityUid uid, IdCardComponent id, MapInitEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		UpdateEntityName(uid, id);
	}

	private void OnTryGetIdentityShortInfo(TryGetIdentityShortInfoEvent ev)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		if (!((HandledEntityEventArgs)ev).Handled)
		{
			string title = null;
			if (TryFindIdCard(ev.ForActor, out Entity<IdCardComponent> idCard) && (!ev.RequestForAccessLogging || !idCard.Comp.BypassLogging))
			{
				title = ExtractFullTitle(Entity<IdCardComponent>.op_Implicit(idCard));
			}
			ev.Title = title;
			((HandledEntityEventArgs)ev).Handled = true;
		}
	}

	public bool TryFindIdCard(EntityUid uid, out Entity<IdCardComponent> idCard)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? activeItem = _hands.GetActiveItem(Entity<HandsComponent>.op_Implicit(uid));
		if (activeItem.HasValue)
		{
			EntityUid heldItem = activeItem.GetValueOrDefault();
			if (TryGetIdCard(heldItem, out idCard))
			{
				return true;
			}
		}
		if (TryGetIdCard(uid, out idCard))
		{
			return true;
		}
		if (_inventorySystem.TryGetSlotEntity(uid, "id", out var idUid) && TryGetIdCard(idUid.Value, out idCard))
		{
			return true;
		}
		return false;
	}

	public bool TryGetIdCard(EntityUid uid, out Entity<IdCardComponent> idCard)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		IdCardComponent idCardComp = default(IdCardComponent);
		if (((EntitySystem)this).TryComp<IdCardComponent>(uid, ref idCardComp))
		{
			idCard = Entity<IdCardComponent>.op_Implicit((uid, idCardComp));
			return true;
		}
		PdaComponent pda = default(PdaComponent);
		if (((EntitySystem)this).TryComp<PdaComponent>(uid, ref pda) && ((EntitySystem)this).TryComp<IdCardComponent>(pda.ContainedId, ref idCardComp))
		{
			idCard = Entity<IdCardComponent>.op_Implicit((pda.ContainedId.Value, idCardComp));
			return true;
		}
		idCard = default(Entity<IdCardComponent>);
		return false;
	}

	public bool TryChangeJobTitle(EntityUid uid, string? jobTitle, IdCardComponent? id = null, EntityUid? player = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<IdCardComponent>(uid, ref id, true))
		{
			return false;
		}
		if (!string.IsNullOrWhiteSpace(jobTitle))
		{
			jobTitle = jobTitle.Trim();
			if (jobTitle.Length > _maxIdJobLength)
			{
				jobTitle = jobTitle.Substring(0, _maxIdJobLength);
			}
		}
		else
		{
			jobTitle = null;
		}
		if (id.LocalizedJobTitle == jobTitle)
		{
			return true;
		}
		id.LocalizedJobTitle = jobTitle;
		((EntitySystem)this).Dirty(uid, (IComponent)(object)id, (MetaDataComponent)null);
		UpdateEntityName(uid, id);
		if (player.HasValue)
		{
			ISharedAdminLogManager adminLogger = _adminLogger;
			LogStringHandler handler = new LogStringHandler(35, 3);
			handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(player.Value)), "player", "ToPrettyString(player.Value)");
			handler.AppendLiteral(" has changed the job title of ");
			handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(uid)), "entity", "ToPrettyString(uid)");
			handler.AppendLiteral(" to ");
			handler.AppendFormatted(jobTitle);
			handler.AppendLiteral(" ");
			adminLogger.Add(LogType.Identity, LogImpact.Low, ref handler);
		}
		return true;
	}

	public bool TryChangeJobIcon(EntityUid uid, JobIconPrototype jobIcon, IdCardComponent? id = null, EntityUid? player = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<IdCardComponent>(uid, ref id, true))
		{
			return false;
		}
		if (id.JobIcon == ProtoId<JobIconPrototype>.op_Implicit(jobIcon.ID))
		{
			return true;
		}
		id.JobIcon = ProtoId<JobIconPrototype>.op_Implicit(jobIcon.ID);
		((EntitySystem)this).Dirty(uid, (IComponent)(object)id, (MetaDataComponent)null);
		if (player.HasValue)
		{
			ISharedAdminLogManager adminLogger = _adminLogger;
			LogStringHandler handler = new LogStringHandler(34, 3);
			handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(player.Value)), "player", "ToPrettyString(player.Value)");
			handler.AppendLiteral(" has changed the job icon of ");
			handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(uid)), "entity", "ToPrettyString(uid)");
			handler.AppendLiteral(" to ");
			handler.AppendFormatted(jobIcon, "jobIcon");
			handler.AppendLiteral(" ");
			adminLogger.Add(LogType.Identity, LogImpact.Low, ref handler);
		}
		return true;
	}

	public bool TryChangeJobDepartment(EntityUid uid, JobPrototype job, IdCardComponent? id = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<IdCardComponent>(uid, ref id, true))
		{
			return false;
		}
		id.JobDepartments.Clear();
		foreach (DepartmentPrototype department in _prototypeManager.EnumeratePrototypes<DepartmentPrototype>())
		{
			if (department.Roles.Contains(ProtoId<JobPrototype>.op_Implicit(job.ID)))
			{
				id.JobDepartments.Add(ProtoId<DepartmentPrototype>.op_Implicit(department.ID));
			}
		}
		((EntitySystem)this).Dirty(uid, (IComponent)(object)id, (MetaDataComponent)null);
		return true;
	}

	public bool TryChangeJobDepartment(EntityUid uid, List<ProtoId<DepartmentPrototype>> departments, IdCardComponent? id = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<IdCardComponent>(uid, ref id, true))
		{
			return false;
		}
		id.JobDepartments.Clear();
		foreach (ProtoId<DepartmentPrototype> department in departments)
		{
			id.JobDepartments.Add(department);
		}
		((EntitySystem)this).Dirty(uid, (IComponent)(object)id, (MetaDataComponent)null);
		return true;
	}

	public bool TryChangeFullName(EntityUid uid, string? fullName, IdCardComponent? id = null, EntityUid? player = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<IdCardComponent>(uid, ref id, true))
		{
			return false;
		}
		if (!string.IsNullOrWhiteSpace(fullName))
		{
			fullName = fullName.Trim();
			if (fullName.Length > _maxNameLength)
			{
				fullName = fullName.Substring(0, _maxNameLength);
			}
		}
		else
		{
			fullName = null;
		}
		if (id.FullName == fullName)
		{
			return true;
		}
		id.FullName = fullName;
		((EntitySystem)this).Dirty(uid, (IComponent)(object)id, (MetaDataComponent)null);
		UpdateEntityName(uid, id);
		if (player.HasValue)
		{
			ISharedAdminLogManager adminLogger = _adminLogger;
			LogStringHandler handler = new LogStringHandler(30, 3);
			handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(player.Value)), "player", "ToPrettyString(player.Value)");
			handler.AppendLiteral(" has changed the name of ");
			handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(uid)), "entity", "ToPrettyString(uid)");
			handler.AppendLiteral(" to ");
			handler.AppendFormatted(fullName);
			handler.AppendLiteral(" ");
			adminLogger.Add(LogType.Identity, LogImpact.Low, ref handler);
		}
		return true;
	}

	private void UpdateEntityName(EntityUid uid, IdCardComponent? id = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<IdCardComponent>(uid, ref id, true))
		{
			string jobSuffix = (string.IsNullOrWhiteSpace(id.LocalizedJobTitle) ? string.Empty : (" (" + id.LocalizedJobTitle + ")"));
			string val = (string.IsNullOrWhiteSpace(id.FullName) ? base.Loc.GetString(LocId.op_Implicit(id.NameLocId), (ValueTuple<string, object>)("jobSuffix", jobSuffix)) : base.Loc.GetString(LocId.op_Implicit(id.FullNameLocId), (ValueTuple<string, object>)("fullName", id.FullName), (ValueTuple<string, object>)("jobSuffix", jobSuffix)));
			_metaSystem.SetEntityName(uid, val, (MetaDataComponent)null, true);
		}
	}

	private static string ExtractFullTitle(IdCardComponent idCardComponent)
	{
		return (idCardComponent.FullName + " (" + CultureInfo.CurrentCulture.TextInfo.ToTitleCase(idCardComponent.LocalizedJobTitle ?? string.Empty) + ")").Trim();
	}

	public void SetExpireTime(Entity<ExpireIdCardComponent?> ent, TimeSpan time)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<ExpireIdCardComponent>(Entity<ExpireIdCardComponent>.op_Implicit(ent), ref ent.Comp, true))
		{
			ent.Comp.ExpireTime = time;
			((EntitySystem)this).Dirty<ExpireIdCardComponent>(ent, (MetaDataComponent)null);
		}
	}

	public void SetPermanent(Entity<ExpireIdCardComponent?> ent, bool val)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<ExpireIdCardComponent>(Entity<ExpireIdCardComponent>.op_Implicit(ent), ref ent.Comp, true))
		{
			ent.Comp.Permanent = val;
			((EntitySystem)this).Dirty<ExpireIdCardComponent>(ent, (MetaDataComponent)null);
		}
	}

	public virtual void ExpireId(Entity<ExpireIdCardComponent> ent)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		if (!ent.Comp.Expired)
		{
			_access.TrySetTags(Entity<ExpireIdCardComponent>.op_Implicit(ent), ent.Comp.ExpiredAccess);
			ent.Comp.Expired = true;
			((EntitySystem)this).Dirty<ExpireIdCardComponent>(ent, (MetaDataComponent)null);
		}
	}

	public override void Update(float frameTime)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).Update(frameTime);
		EntityQueryEnumerator<ExpireIdCardComponent> query = ((EntitySystem)this).EntityQueryEnumerator<ExpireIdCardComponent>();
		EntityUid uid = default(EntityUid);
		ExpireIdCardComponent comp = default(ExpireIdCardComponent);
		while (query.MoveNext(ref uid, ref comp))
		{
			if (!comp.Expired && !comp.Permanent && !(_timing.CurTime < comp.ExpireTime))
			{
				ExpireId(Entity<ExpireIdCardComponent>.op_Implicit((uid, comp)));
			}
		}
		try
		{
			foreach (EntityUid rename in _toRename)
			{
				if (!((EntitySystem)this).TerminatingOrDeleted(rename, (MetaDataComponent)null) && TryFindIdCard(rename, out Entity<IdCardComponent> idCard))
				{
					TryChangeFullName(Entity<IdCardComponent>.op_Implicit(idCard), ((EntitySystem)this).Name(rename, (MetaDataComponent)null), Entity<IdCardComponent>.op_Implicit(idCard));
				}
			}
		}
		finally
		{
			_toRename.Clear();
		}
	}

	public bool TryChangeOriginalOwner(EntityUid uid, EntityUid? player, IdCardComponent? id = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<IdCardComponent>(uid, ref id, true))
		{
			return false;
		}
		if (!player.HasValue)
		{
			return false;
		}
		EntityUid? originalOwner = id.OriginalOwner;
		EntityUid? val = player;
		if (originalOwner.HasValue == val.HasValue && (!originalOwner.HasValue || originalOwner.GetValueOrDefault() == val.GetValueOrDefault()))
		{
			return true;
		}
		id.OriginalOwner = player.Value;
		((EntitySystem)this).Dirty(uid, (IComponent)(object)id, (MetaDataComponent)null);
		UpdateEntityName(uid, id);
		return true;
	}
}
