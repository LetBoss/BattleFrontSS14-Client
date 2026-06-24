// Decompiled with JetBrains decompiler
// Type: Content.Client.CrewManifest.UI.CrewManifestListing
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

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
using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable
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
    this._spriteSystem = this._entitySystem.GetEntitySystem<SpriteSystem>();
  }

  public void AddCrewManifestEntries(CrewManifestEntries entries)
  {
    this.RMCAddCrewManifestEntries(entries);
  }

  public void RMCAddCrewManifestEntries(CrewManifestEntries entries)
  {
    Dictionary<ProtoId<JobPrototype>, List<DepartmentPrototype>> dictionary = new Dictionary<ProtoId<JobPrototype>, List<DepartmentPrototype>>();
    foreach (DepartmentPrototype enumeratePrototype in this._prototypeManager.EnumeratePrototypes<DepartmentPrototype>())
    {
      foreach (ProtoId<JobPrototype> protoId in enumeratePrototype.Roles.Where<ProtoId<JobPrototype>>((Func<ProtoId<JobPrototype>, bool>) (roleId => this._prototypeManager.HasIndex<JobPrototype>(roleId))))
        Extensions.GetOrNew<ProtoId<JobPrototype>, List<DepartmentPrototype>>(dictionary, protoId).Add(enumeratePrototype);
    }
    Dictionary<DepartmentPrototype, List<CrewManifestEntry>> source1 = new Dictionary<DepartmentPrototype, List<CrewManifestEntry>>();
    Dictionary<string, List<CrewManifestEntry>> source2 = new Dictionary<string, List<CrewManifestEntry>>();
    foreach (CrewManifestEntry entry in entries.Entries)
    {
      List<DepartmentPrototype> departmentPrototypeList;
      if (dictionary.TryGetValue(ProtoId<JobPrototype>.op_Implicit(entry.JobPrototype), out departmentPrototypeList))
      {
        if (!string.IsNullOrEmpty(entry.Squad))
        {
          Extensions.GetOrNew<string, List<CrewManifestEntry>>(source2, entry.Squad).Add(entry);
        }
        else
        {
          foreach (DepartmentPrototype departmentPrototype in departmentPrototypeList)
            Extensions.GetOrNew<DepartmentPrototype, List<CrewManifestEntry>>(source1, departmentPrototype).Add(entry);
        }
      }
    }
    foreach ((DepartmentPrototype key, List<CrewManifestEntry> entries3) in (IEnumerable<KeyValuePair<DepartmentPrototype, List<CrewManifestEntry>>>) source1.OrderBy<KeyValuePair<DepartmentPrototype, List<CrewManifestEntry>>, DepartmentPrototype>((Func<KeyValuePair<DepartmentPrototype, List<CrewManifestEntry>>, DepartmentPrototype>) (kvp => kvp.Key), (IComparer<DepartmentPrototype>) DepartmentUIComparer.Instance))
    {
      List<CrewManifestEntry> entries2 = entries3;
      ((Control) this).AddChild((Control) new CrewManifestSection(this._prototypeManager, this._spriteSystem, LocId.op_Implicit(key.Name), entries2));
    }
    string str2;
    foreach ((str2, entries3) in (IEnumerable<KeyValuePair<string, List<CrewManifestEntry>>>) source2.OrderBy<KeyValuePair<string, List<CrewManifestEntry>>, string>((Func<KeyValuePair<string, List<CrewManifestEntry>>, string>) (kvp => kvp.Key)))
      ((Control) this).AddChild((Control) new CrewManifestSection(this._prototypeManager, this._spriteSystem, str2, entries3));
  }
}
