using System;
using System.Collections.Generic;
using System.Linq;
using Content.Shared.Access.Components;
using Content.Shared.Access.Systems;
using Content.Shared.CCVar;
using Content.Shared.Examine;
using Content.Shared.Localizations;
using Content.Shared.Roles;
using Content.Shared.Verbs;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;

namespace Content.Shared.Contraband;

public sealed class ContrabandSystem : EntitySystem
{
	[Dependency]
	private IConfigurationManager _configuration;

	[Dependency]
	private IPrototypeManager _proto;

	[Dependency]
	private SharedIdCardSystem _id;

	[Dependency]
	private ExamineSystemShared _examine;

	private bool _contrabandExamineEnabled;

	private bool _contrabandExamineOnlyInHudEnabled;

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<ContrabandComponent, GetVerbsEvent<ExamineVerb>>((ComponentEventRefHandler<ContrabandComponent, GetVerbsEvent<ExamineVerb>>)OnDetailedExamine, (Type[])null, (Type[])null);
		EntitySystemSubscriptionExt.CVar<bool>(((EntitySystem)this).Subs, _configuration, CCVars.ContrabandExamine, (Action<bool>)SetContrabandExamine, true);
		EntitySystemSubscriptionExt.CVar<bool>(((EntitySystem)this).Subs, _configuration, CCVars.ContrabandExamineOnlyInHUD, (Action<bool>)SetContrabandExamineOnlyInHUD, true);
	}

	public void CopyDetails(EntityUid uid, ContrabandComponent other, ContrabandComponent? contraband = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<ContrabandComponent>(uid, ref contraband, true))
		{
			contraband.Severity = other.Severity;
			contraband.AllowedDepartments = other.AllowedDepartments;
			contraband.AllowedJobs = other.AllowedJobs;
			((EntitySystem)this).Dirty(uid, (IComponent)(object)contraband, (MetaDataComponent)null);
		}
	}

	private void OnDetailedExamine(EntityUid ent, ContrabandComponent component, ref GetVerbsEvent<ExamineVerb> args)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		if (!_contrabandExamineEnabled)
		{
			return;
		}
		if (_contrabandExamineOnlyInHudEnabled)
		{
			GetContrabandDetailsEvent ev = default(GetContrabandDetailsEvent);
			((EntitySystem)this).RaiseLocalEvent<GetContrabandDetailsEvent>(args.User, ref ev, false);
			if (!ev.CanShowContraband)
			{
				return;
			}
		}
		if (!args.CanInteract)
		{
			return;
		}
		IEnumerable<string> localizedDepartments = component.AllowedDepartments.Select<ProtoId<DepartmentPrototype>, string>((ProtoId<DepartmentPrototype> p) => base.Loc.GetString("contraband-department-plural", (ValueTuple<string, object>)("department", base.Loc.GetString(LocId.op_Implicit(_proto.Index<DepartmentPrototype>(p).Name)))));
		string[] jobs = component.AllowedJobs.Select<ProtoId<JobPrototype>, string>((ProtoId<JobPrototype> p) => _proto.Index<JobPrototype>(p).LocalizedName).ToArray();
		IEnumerable<string> localizedJobs = jobs.Select((string p) => base.Loc.GetString("contraband-job-plural", (ValueTuple<string, object>)("job", p)));
		ContrabandSeverityPrototype severity = _proto.Index<ContrabandSeverityPrototype>(component.Severity);
		string departmentExamineMessage;
		if (severity.ShowDepartmentsAndJobs)
		{
			string list = ContentLocalizationManager.FormatList(localizedDepartments.Concat(localizedJobs).ToList());
			departmentExamineMessage = base.Loc.GetString("contraband-examine-text-Restricted-department", (ValueTuple<string, object>)("departments", list));
		}
		else
		{
			departmentExamineMessage = base.Loc.GetString(LocId.op_Implicit(severity.ExamineText));
		}
		List<ProtoId<DepartmentPrototype>> departments = new List<ProtoId<DepartmentPrototype>>();
		string jobId = "";
		if (_id.TryFindIdCard(args.User, out Entity<IdCardComponent> id))
		{
			departments = id.Comp.JobDepartments;
			if (id.Comp.LocalizedJobTitle != null)
			{
				jobId = id.Comp.LocalizedJobTitle;
			}
		}
		string carryingMessage = base.Loc.GetString("contraband-examine-text-avoid-carrying-around");
		string iconTexture = "/Textures/Interface/VerbIcons/lock-red.svg.192dpi.png";
		if (departments.Intersect<ProtoId<DepartmentPrototype>>(component.AllowedDepartments).Any() || Enumerable.Contains(jobs, jobId))
		{
			carryingMessage = base.Loc.GetString("contraband-examine-text-in-the-clear");
			iconTexture = "/Textures/Interface/VerbIcons/unlock-green.svg.192dpi.png";
		}
		FormattedMessage examineMarkup = GetContrabandExamine(departmentExamineMessage, carryingMessage);
		_examine.AddHoverExamineVerb(args, (Component)(object)component, base.Loc.GetString("contraband-examinable-verb-text"), examineMarkup.ToMarkup(), iconTexture);
	}

	private FormattedMessage GetContrabandExamine(string deptMessage, string carryMessage)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Expected O, but got Unknown
		FormattedMessage val = new FormattedMessage();
		val.AddMarkupOrThrow(deptMessage);
		val.PushNewline();
		val.AddMarkupOrThrow(carryMessage);
		return val;
	}

	private void SetContrabandExamine(bool val)
	{
		_contrabandExamineEnabled = val;
	}

	private void SetContrabandExamineOnlyInHUD(bool val)
	{
		_contrabandExamineOnlyInHudEnabled = val;
	}
}
