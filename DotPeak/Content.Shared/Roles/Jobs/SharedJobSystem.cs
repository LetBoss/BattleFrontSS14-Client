// Decompiled with JetBrains decompiler
// Type: Content.Shared.Roles.Jobs.SharedJobSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Mind;
using Content.Shared.Players;
using Content.Shared.Players.PlayTimeTracking;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

#nullable enable
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
    base.Initialize();
    this.SubscribeLocalEvent<PrototypesReloadedEventArgs>(new EntityEventHandler<PrototypesReloadedEventArgs>(this.OnProtoReload));
    this.SetupTrackerLookup();
  }

  private void OnProtoReload(PrototypesReloadedEventArgs obj)
  {
    if (!obj.WasModified<JobPrototype>())
      return;
    this.SetupTrackerLookup();
  }

  private void SetupTrackerLookup()
  {
    this._inverseTrackerLookup.Clear();
    foreach (JobPrototype enumeratePrototype in this._prototypes.EnumeratePrototypes<JobPrototype>())
      this._inverseTrackerLookup.GetOrNew<ProtoId<PlayTimeTrackerPrototype>, List<ProtoId<JobPrototype>>>((ProtoId<PlayTimeTrackerPrototype>) enumeratePrototype.PlayTimeTracker).Add((ProtoId<JobPrototype>) enumeratePrototype.ID);
  }

  [Obsolete("Use SharedJobSystem.GetJobPrototypes, a tracker prototype can now have multiple jobs")]
  public ProtoId<JobPrototype> GetJobPrototype(ProtoId<PlayTimeTrackerPrototype> trackerProto)
  {
    return this._inverseTrackerLookup[trackerProto].FirstOrDefault<ProtoId<JobPrototype>>();
  }

  public List<ProtoId<JobPrototype>> GetJobPrototypes(ProtoId<PlayTimeTrackerPrototype> trackerProto)
  {
    return this._inverseTrackerLookup[trackerProto];
  }

  public bool TryGetDepartment(string jobProto, [NotNullWhen(true)] out DepartmentPrototype? departmentPrototype)
  {
    List<DepartmentPrototype> list = this._prototypes.EnumeratePrototypes<DepartmentPrototype>().ToList<DepartmentPrototype>();
    list.Sort((Comparison<DepartmentPrototype>) ((x, y) => string.Compare(x.ID, y.ID, StringComparison.Ordinal)));
    foreach (DepartmentPrototype departmentPrototype1 in list)
    {
      if (departmentPrototype1.Roles.Contains((ProtoId<JobPrototype>) jobProto))
      {
        departmentPrototype = departmentPrototype1;
        return true;
      }
    }
    departmentPrototype = (DepartmentPrototype) null;
    return false;
  }

  public bool TryGetPrimaryDepartment(string jobProto, [NotNullWhen(true)] out DepartmentPrototype? departmentPrototype)
  {
    foreach (DepartmentPrototype enumeratePrototype in this._prototypes.EnumeratePrototypes<DepartmentPrototype>())
    {
      if (enumeratePrototype.Primary && enumeratePrototype.Roles.Contains((ProtoId<JobPrototype>) jobProto))
      {
        departmentPrototype = enumeratePrototype;
        return true;
      }
    }
    departmentPrototype = (DepartmentPrototype) null;
    return false;
  }

  public bool TryGetAllDepartments(
    string jobProto,
    out List<DepartmentPrototype> departmentPrototypes)
  {
    IEnumerable<DepartmentPrototype> departmentPrototypes1 = this._prototypes.EnumeratePrototypes<DepartmentPrototype>();
    departmentPrototypes = new List<DepartmentPrototype>();
    bool allDepartments = false;
    foreach (DepartmentPrototype departmentPrototype in departmentPrototypes1)
    {
      if (departmentPrototype.Roles.Contains((ProtoId<JobPrototype>) jobProto))
      {
        departmentPrototypes.Add(departmentPrototype);
        allDepartments = true;
      }
    }
    return allDepartments;
  }

  public bool TryGetListHighestWeightDepartment(
    List<ProtoId<JobPrototype>> jobList,
    [NotNullWhen(true)] out DepartmentPrototype? chosenDepartment)
  {
    chosenDepartment = (DepartmentPrototype) null;
    foreach (ProtoId<JobPrototype> job in jobList)
    {
      List<DepartmentPrototype> departmentPrototypes;
      if (this.TryGetAllDepartments((string) job, out departmentPrototypes) && departmentPrototypes.Count != 0)
      {
        departmentPrototypes.Sort((Comparison<DepartmentPrototype>) ((x, y) => y.Weight.CompareTo(x.Weight)));
        DepartmentPrototype departmentPrototype = departmentPrototypes[0];
        if (chosenDepartment == null || chosenDepartment.Weight <= departmentPrototype.Weight)
          chosenDepartment = departmentPrototype;
      }
    }
    return chosenDepartment != null;
  }

  public bool TryGetLowestWeightDepartment(
    string jobProto,
    [NotNullWhen(true)] out DepartmentPrototype? departmentPrototype)
  {
    departmentPrototype = (DepartmentPrototype) null;
    List<DepartmentPrototype> departmentPrototypes;
    if (!this.TryGetAllDepartments(jobProto, out departmentPrototypes) || departmentPrototypes.Count == 0)
      return false;
    departmentPrototypes.Sort((Comparison<DepartmentPrototype>) ((x, y) => y.Weight.CompareTo(x.Weight)));
    departmentPrototype = departmentPrototypes[0];
    return true;
  }

  public bool MindHasJobWithId(EntityUid? mindId, string prototypeId)
  {
    if (!mindId.HasValue)
      return false;
    Entity<MindRoleComponent, JobRoleComponent>? role;
    this._roles.MindHasRole<JobRoleComponent>((Entity<MindComponent>) mindId.Value, out role);
    if (!role.HasValue)
      return false;
    ProtoId<JobPrototype>? jobPrototype = role.Value.Comp1.JobPrototype;
    ProtoId<JobPrototype>? nullable = (ProtoId<JobPrototype>?) prototypeId;
    if (jobPrototype.HasValue != nullable.HasValue)
      return false;
    return !jobPrototype.HasValue || jobPrototype.GetValueOrDefault() == nullable.GetValueOrDefault();
  }

  public bool MindTryGetJob([NotNullWhen(true)] EntityUid? mindId, [NotNullWhen(true)] out JobPrototype? prototype)
  {
    prototype = (JobPrototype) null;
    ProtoId<JobPrototype>? job;
    this.MindTryGetJobId(mindId, out job);
    return this._prototypes.TryIndex<JobPrototype>(job, out prototype) || prototype != null;
  }

  public bool MindTryGetJobId([NotNullWhen(true)] EntityUid? mindId, out ProtoId<JobPrototype>? job)
  {
    job = new ProtoId<JobPrototype>?();
    if (!mindId.HasValue)
      return false;
    Entity<MindRoleComponent, JobRoleComponent>? role;
    if (this._roles.MindHasRole<JobRoleComponent>((Entity<MindComponent>) mindId.Value, out role))
      job = role.Value.Comp1.JobPrototype;
    return job.HasValue;
  }

  public bool MindTryGetJobName([NotNullWhen(true)] EntityUid? mindId, out string name)
  {
    JobPrototype prototype;
    if (this.MindTryGetJob(mindId, out prototype))
    {
      name = prototype.LocalizedName;
      return true;
    }
    name = this.Loc.GetString("generic-unknown-title");
    return false;
  }

  public string MindTryGetJobName([NotNullWhen(true)] EntityUid? mindId)
  {
    string name;
    this.MindTryGetJobName(mindId, out name);
    return name;
  }

  public bool CanBeAntag(ICommonSession player)
  {
    ContentPlayerData contentPlayerData = this._playerSystem.ContentData(player);
    if (contentPlayerData != null)
    {
      EntityUid? mind = contentPlayerData.Mind;
      JobPrototype prototype;
      if (mind.HasValue && this.MindTryGetJob(new EntityUid?(mind.GetValueOrDefault()), out prototype))
        return prototype.CanBeAntag;
    }
    return true;
  }
}
