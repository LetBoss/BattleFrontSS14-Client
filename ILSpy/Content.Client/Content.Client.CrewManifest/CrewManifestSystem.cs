using System;
using System.Collections.Generic;
using Content.Shared.CrewManifest;
using Content.Shared.Roles;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;

namespace Content.Client.CrewManifest;

public sealed class CrewManifestSystem : EntitySystem
{
	[Dependency]
	private IPrototypeManager _prototypeManager;

	private Dictionary<string, Dictionary<string, int>> _jobDepartmentLookup = new Dictionary<string, Dictionary<string, int>>();

	private HashSet<string> _departments = new HashSet<string>();

	public IReadOnlySet<string> Departments => _departments;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		BuildDepartmentLookup();
		((EntitySystem)this).SubscribeLocalEvent<PrototypesReloadedEventArgs>((EntityEventHandler<PrototypesReloadedEventArgs>)OnPrototypesReload, (Type[])null, (Type[])null);
	}

	public void RequestCrewManifest(NetEntity netEntity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).RaiseNetworkEvent((EntityEventArgs)(object)new RequestCrewManifestMessage(netEntity));
	}

	private void OnPrototypesReload(PrototypesReloadedEventArgs args)
	{
		if (args.WasModified<DepartmentPrototype>())
		{
			BuildDepartmentLookup();
		}
	}

	private void BuildDepartmentLookup()
	{
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		_jobDepartmentLookup.Clear();
		_departments.Clear();
		foreach (DepartmentPrototype item in _prototypeManager.EnumeratePrototypes<DepartmentPrototype>())
		{
			_departments.Add(item.ID);
			for (int i = 1; i <= item.Roles.Count; i++)
			{
				if (!_jobDepartmentLookup.TryGetValue(ProtoId<JobPrototype>.op_Implicit(item.Roles[i - 1]), out Dictionary<string, int> value))
				{
					value = new Dictionary<string, int>();
					_jobDepartmentLookup.Add(ProtoId<JobPrototype>.op_Implicit(item.Roles[i - 1]), value);
				}
				value.Add(item.ID, i);
			}
		}
	}

	public int GetDepartmentOrder(string department, string jobPrototype)
	{
		if (!Departments.Contains(department))
		{
			return -1;
		}
		if (!_jobDepartmentLookup.TryGetValue(jobPrototype, out Dictionary<string, int> value))
		{
			return -1;
		}
		if (!value.TryGetValue(department, out var value2))
		{
			return -1;
		}
		return value2;
	}
}
