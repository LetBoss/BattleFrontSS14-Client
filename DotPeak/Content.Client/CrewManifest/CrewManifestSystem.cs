// Decompiled with JetBrains decompiler
// Type: Content.Client.CrewManifest.CrewManifestSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.CrewManifest;
using Content.Shared.Roles;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Client.CrewManifest;

public sealed class CrewManifestSystem : EntitySystem
{
  [Dependency]
  private IPrototypeManager _prototypeManager;
  private Dictionary<string, Dictionary<string, int>> _jobDepartmentLookup = new Dictionary<string, Dictionary<string, int>>();
  private HashSet<string> _departments = new HashSet<string>();

  public IReadOnlySet<string> Departments => (IReadOnlySet<string>) this._departments;

  public virtual void Initialize()
  {
    base.Initialize();
    this.BuildDepartmentLookup();
    this.SubscribeLocalEvent<PrototypesReloadedEventArgs>(new EntityEventHandler<PrototypesReloadedEventArgs>(this.OnPrototypesReload), (Type[]) null, (Type[]) null);
  }

  public void RequestCrewManifest(NetEntity netEntity)
  {
    this.RaiseNetworkEvent((EntityEventArgs) new RequestCrewManifestMessage(netEntity));
  }

  private void OnPrototypesReload(PrototypesReloadedEventArgs args)
  {
    if (!args.WasModified<DepartmentPrototype>())
      return;
    this.BuildDepartmentLookup();
  }

  private void BuildDepartmentLookup()
  {
    this._jobDepartmentLookup.Clear();
    this._departments.Clear();
    foreach (DepartmentPrototype enumeratePrototype in this._prototypeManager.EnumeratePrototypes<DepartmentPrototype>())
    {
      this._departments.Add(enumeratePrototype.ID);
      for (int index = 1; index <= enumeratePrototype.Roles.Count; ++index)
      {
        Dictionary<string, int> dictionary;
        if (!this._jobDepartmentLookup.TryGetValue(ProtoId<JobPrototype>.op_Implicit(enumeratePrototype.Roles[index - 1]), out dictionary))
        {
          dictionary = new Dictionary<string, int>();
          this._jobDepartmentLookup.Add(ProtoId<JobPrototype>.op_Implicit(enumeratePrototype.Roles[index - 1]), dictionary);
        }
        dictionary.Add(enumeratePrototype.ID, index);
      }
    }
  }

  public int GetDepartmentOrder(string department, string jobPrototype)
  {
    Dictionary<string, int> dictionary;
    int num;
    return !this.Departments.Contains(department) || !this._jobDepartmentLookup.TryGetValue(jobPrototype, out dictionary) || !dictionary.TryGetValue(department, out num) ? -1 : num;
  }
}
