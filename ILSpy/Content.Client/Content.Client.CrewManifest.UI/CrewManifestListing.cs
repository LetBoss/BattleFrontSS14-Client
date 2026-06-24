using System.Collections.Generic;
using System.Linq;
using Content.Shared.CrewManifest;
using Content.Shared.Roles;
using Robust.Client.GameObjects;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;

namespace Content.Client.CrewManifest.UI;

public sealed class CrewManifestListing : BoxContainer
{
	[Dependency]
	private IEntitySystemManager _entitySystem;

	[Dependency]
	private IPrototypeManager _prototypeManager;

	private readonly SpriteSystem _spriteSystem;

	public CrewManifestListing()
	{
		IoCManager.InjectDependencies<CrewManifestListing>(this);
		_spriteSystem = _entitySystem.GetEntitySystem<SpriteSystem>();
	}

	public void AddCrewManifestEntries(CrewManifestEntries entries)
	{
		RMCAddCrewManifestEntries(entries);
	}

	public void RMCAddCrewManifestEntries(CrewManifestEntries entries)
	{
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		Dictionary<ProtoId<JobPrototype>, List<DepartmentPrototype>> dictionary = new Dictionary<ProtoId<JobPrototype>, List<DepartmentPrototype>>();
		foreach (DepartmentPrototype item in _prototypeManager.EnumeratePrototypes<DepartmentPrototype>())
		{
			foreach (ProtoId<JobPrototype> item2 in item.Roles.Where<ProtoId<JobPrototype>>((ProtoId<JobPrototype> roleId) => _prototypeManager.HasIndex<JobPrototype>(roleId)))
			{
				Extensions.GetOrNew<ProtoId<JobPrototype>, List<DepartmentPrototype>>(dictionary, item2).Add(item);
			}
		}
		Dictionary<DepartmentPrototype, List<CrewManifestEntry>> dictionary2 = new Dictionary<DepartmentPrototype, List<CrewManifestEntry>>();
		Dictionary<string, List<CrewManifestEntry>> dictionary3 = new Dictionary<string, List<CrewManifestEntry>>();
		CrewManifestEntry[] entries2 = entries.Entries;
		foreach (CrewManifestEntry crewManifestEntry in entries2)
		{
			if (!dictionary.TryGetValue(ProtoId<JobPrototype>.op_Implicit(crewManifestEntry.JobPrototype), out var value))
			{
				continue;
			}
			if (!string.IsNullOrEmpty(crewManifestEntry.Squad))
			{
				Extensions.GetOrNew<string, List<CrewManifestEntry>>(dictionary3, crewManifestEntry.Squad).Add(crewManifestEntry);
				continue;
			}
			foreach (DepartmentPrototype item3 in value)
			{
				Extensions.GetOrNew<DepartmentPrototype, List<CrewManifestEntry>>(dictionary2, item3).Add(crewManifestEntry);
			}
		}
		List<CrewManifestEntry> value2;
		foreach (KeyValuePair<DepartmentPrototype, List<CrewManifestEntry>> item4 in dictionary2.OrderBy((KeyValuePair<DepartmentPrototype, List<CrewManifestEntry>> kvp) => kvp.Key, DepartmentUIComparer.Instance))
		{
			item4.Deconstruct(out var key, out value2);
			DepartmentPrototype departmentPrototype = key;
			List<CrewManifestEntry> entries3 = value2;
			((Control)this).AddChild((Control)(object)new CrewManifestSection(_prototypeManager, _spriteSystem, LocId.op_Implicit(departmentPrototype.Name), entries3));
		}
		foreach (KeyValuePair<string, List<CrewManifestEntry>> item5 in dictionary3.OrderBy((KeyValuePair<string, List<CrewManifestEntry>> kvp) => kvp.Key))
		{
			item5.Deconstruct(out var key2, out value2);
			string sectionName = key2;
			List<CrewManifestEntry> entries4 = value2;
			((Control)this).AddChild((Control)(object)new CrewManifestSection(_prototypeManager, _spriteSystem, sectionName, entries4));
		}
	}
}
