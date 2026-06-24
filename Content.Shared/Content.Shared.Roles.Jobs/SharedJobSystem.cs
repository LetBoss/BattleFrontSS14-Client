using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Content.Shared.Mind;
using Content.Shared.Players;
using Content.Shared.Players.PlayTimeTracking;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;

namespace Content.Shared.Roles.Jobs;

public abstract class SharedJobSystem : EntitySystem
{
	[Dependency]
	private SharedPlayerSystem _playerSystem;

	[Dependency]
	private IPrototypeManager _prototypes;

	[Dependency]
	private SharedRoleSystem _roles;

	private readonly Dictionary<ProtoId<PlayTimeTrackerPrototype>, List<ProtoId<JobPrototype>>> _inverseTrackerLookup = new Dictionary<ProtoId<PlayTimeTrackerPrototype>, List<ProtoId<JobPrototype>>>();

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<PrototypesReloadedEventArgs>((EntityEventHandler<PrototypesReloadedEventArgs>)OnProtoReload, (Type[])null, (Type[])null);
		SetupTrackerLookup();
	}

	private void OnProtoReload(PrototypesReloadedEventArgs obj)
	{
		if (obj.WasModified<JobPrototype>())
		{
			SetupTrackerLookup();
		}
	}

	private void SetupTrackerLookup()
	{
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		_inverseTrackerLookup.Clear();
		foreach (JobPrototype job in _prototypes.EnumeratePrototypes<JobPrototype>())
		{
			Extensions.GetOrNew<ProtoId<PlayTimeTrackerPrototype>, List<ProtoId<JobPrototype>>>(_inverseTrackerLookup, ProtoId<PlayTimeTrackerPrototype>.op_Implicit(job.PlayTimeTracker)).Add(ProtoId<JobPrototype>.op_Implicit(job.ID));
		}
	}

	[Obsolete("Use SharedJobSystem.GetJobPrototypes, a tracker prototype can now have multiple jobs")]
	public ProtoId<JobPrototype> GetJobPrototype(ProtoId<PlayTimeTrackerPrototype> trackerProto)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		return _inverseTrackerLookup[trackerProto].FirstOrDefault();
	}

	public List<ProtoId<JobPrototype>> GetJobPrototypes(ProtoId<PlayTimeTrackerPrototype> trackerProto)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		return _inverseTrackerLookup[trackerProto];
	}

	public bool TryGetDepartment(string jobProto, [NotNullWhen(true)] out DepartmentPrototype? departmentPrototype)
	{
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		List<DepartmentPrototype> list = _prototypes.EnumeratePrototypes<DepartmentPrototype>().ToList();
		list.Sort((DepartmentPrototype x, DepartmentPrototype y) => string.Compare(x.ID, y.ID, StringComparison.Ordinal));
		foreach (DepartmentPrototype department in list)
		{
			if (department.Roles.Contains(ProtoId<JobPrototype>.op_Implicit(jobProto)))
			{
				departmentPrototype = department;
				return true;
			}
		}
		departmentPrototype = null;
		return false;
	}

	public bool TryGetPrimaryDepartment(string jobProto, [NotNullWhen(true)] out DepartmentPrototype? departmentPrototype)
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		foreach (DepartmentPrototype department in _prototypes.EnumeratePrototypes<DepartmentPrototype>())
		{
			if (department.Primary && department.Roles.Contains(ProtoId<JobPrototype>.op_Implicit(jobProto)))
			{
				departmentPrototype = department;
				return true;
			}
		}
		departmentPrototype = null;
		return false;
	}

	public bool TryGetAllDepartments(string jobProto, out List<DepartmentPrototype> departmentPrototypes)
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		IEnumerable<DepartmentPrototype> enumerable = _prototypes.EnumeratePrototypes<DepartmentPrototype>();
		departmentPrototypes = new List<DepartmentPrototype>();
		bool found = false;
		foreach (DepartmentPrototype department in enumerable)
		{
			if (department.Roles.Contains(ProtoId<JobPrototype>.op_Implicit(jobProto)))
			{
				departmentPrototypes.Add(department);
				found = true;
			}
		}
		return found;
	}

	public bool TryGetListHighestWeightDepartment(List<ProtoId<JobPrototype>> jobList, [NotNullWhen(true)] out DepartmentPrototype? chosenDepartment)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		chosenDepartment = null;
		foreach (ProtoId<JobPrototype> jobId in jobList)
		{
			if (TryGetAllDepartments(ProtoId<JobPrototype>.op_Implicit(jobId), out List<DepartmentPrototype> departmentPrototypes) && departmentPrototypes.Count != 0)
			{
				departmentPrototypes.Sort((DepartmentPrototype x, DepartmentPrototype y) => y.Weight.CompareTo(x.Weight));
				DepartmentPrototype newDepartment = departmentPrototypes[0];
				if (chosenDepartment == null || chosenDepartment.Weight <= newDepartment.Weight)
				{
					chosenDepartment = newDepartment;
				}
			}
		}
		if (chosenDepartment == null)
		{
			return false;
		}
		return true;
	}

	public bool TryGetLowestWeightDepartment(string jobProto, [NotNullWhen(true)] out DepartmentPrototype? departmentPrototype)
	{
		departmentPrototype = null;
		if (!TryGetAllDepartments(jobProto, out List<DepartmentPrototype> departmentPrototypes) || departmentPrototypes.Count == 0)
		{
			return false;
		}
		departmentPrototypes.Sort((DepartmentPrototype x, DepartmentPrototype y) => y.Weight.CompareTo(x.Weight));
		departmentPrototype = departmentPrototypes[0];
		return true;
	}

	public bool MindHasJobWithId(EntityUid? mindId, string prototypeId)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		if (!mindId.HasValue)
		{
			return false;
		}
		_roles.MindHasRole<JobRoleComponent>(Entity<MindComponent>.op_Implicit(mindId.Value), out Entity<MindRoleComponent, JobRoleComponent>? role);
		if (!role.HasValue)
		{
			return false;
		}
		ProtoId<JobPrototype>? jobPrototype = role.Value.Comp1.JobPrototype;
		ProtoId<JobPrototype>? val = ProtoId<JobPrototype>.op_Implicit(prototypeId);
		if (jobPrototype.HasValue != val.HasValue)
		{
			return false;
		}
		if (!jobPrototype.HasValue)
		{
			return true;
		}
		return jobPrototype.GetValueOrDefault() == val.GetValueOrDefault();
	}

	public bool MindTryGetJob([NotNullWhen(true)] EntityUid? mindId, [NotNullWhen(true)] out JobPrototype? prototype)
	{
		prototype = null;
		MindTryGetJobId(mindId, out ProtoId<JobPrototype>? protoId);
		if (!_prototypes.TryIndex<JobPrototype>(protoId, ref prototype))
		{
			return prototype != null;
		}
		return true;
	}

	public bool MindTryGetJobId([NotNullWhen(true)] EntityUid? mindId, out ProtoId<JobPrototype>? job)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		job = null;
		if (!mindId.HasValue)
		{
			return false;
		}
		if (_roles.MindHasRole<JobRoleComponent>(Entity<MindComponent>.op_Implicit(mindId.Value), out Entity<MindRoleComponent, JobRoleComponent>? role))
		{
			job = role.Value.Comp1.JobPrototype;
		}
		ProtoId<JobPrototype>? val = job;
		return val.HasValue;
	}

	public bool MindTryGetJobName([NotNullWhen(true)] EntityUid? mindId, out string name)
	{
		if (MindTryGetJob(mindId, out JobPrototype prototype))
		{
			name = prototype.LocalizedName;
			return true;
		}
		name = base.Loc.GetString("generic-unknown-title");
		return false;
	}

	public string MindTryGetJobName([NotNullWhen(true)] EntityUid? mindId)
	{
		MindTryGetJobName(mindId, out string name);
		return name;
	}

	public bool CanBeAntag(ICommonSession player)
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		ContentPlayerData contentPlayerData = _playerSystem.ContentData(player);
		if (contentPlayerData != null)
		{
			EntityUid? mind = contentPlayerData.Mind;
			if (mind.HasValue)
			{
				EntityUid mindId = mind.GetValueOrDefault();
				if (!MindTryGetJob(mindId, out JobPrototype prototype))
				{
					return true;
				}
				return prototype.CanBeAntag;
			}
		}
		return true;
	}
}
