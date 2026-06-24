// Decompiled with JetBrains decompiler
// Type: Content.Shared.Objectives.Systems.SharedObjectivesSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Mind;
using Content.Shared.Objectives.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;
using System.Diagnostics.CodeAnalysis;

#nullable enable
namespace Content.Shared.Objectives.Systems;

public abstract class SharedObjectivesSystem : EntitySystem
{
  [Dependency]
  private SharedMindSystem _mind;
  [Dependency]
  private IPrototypeManager _protoMan;
  private Robust.Shared.GameObjects.EntityQuery<MetaDataComponent> _metaQuery;

  public override void Initialize()
  {
    base.Initialize();
    this._metaQuery = this.GetEntityQuery<MetaDataComponent>();
  }

  public bool CanBeAssigned(
    EntityUid uid,
    EntityUid mindId,
    MindComponent mind,
    ObjectiveComponent? comp = null)
  {
    if (!this.Resolve<ObjectiveComponent>(uid, ref comp))
      return false;
    RequirementCheckEvent args = new RequirementCheckEvent(mindId, mind);
    this.RaiseLocalEvent<RequirementCheckEvent>(uid, ref args);
    if (args.Cancelled)
      return false;
    if (comp.Unique)
    {
      string id = this._metaQuery.GetComponent(uid).EntityPrototype?.ID;
      foreach (EntityUid objective in mind.Objectives)
      {
        if (this._metaQuery.GetComponent(objective).EntityPrototype?.ID == id)
          return false;
      }
    }
    return true;
  }

  public EntityUid? TryCreateObjective(EntityUid mindId, MindComponent mind, string proto)
  {
    if (!this._protoMan.HasIndex<Robust.Shared.Prototypes.EntityPrototype>(proto))
      return new EntityUid?();
    EntityUid uid = this.Spawn(proto);
    ObjectiveComponent comp;
    if (!this.TryComp<ObjectiveComponent>(uid, out comp))
    {
      this.Del(new EntityUid?(uid));
      this.Log.Error($"Invalid objective prototype {proto}, missing ObjectiveComponent");
      return new EntityUid?();
    }
    if (!this.CanBeAssigned(uid, mindId, mind, comp))
    {
      this.Log.Warning($"Objective {proto} did not match the requirements for {this._mind.MindOwnerLoggingString(mind)}, deleted it");
      return new EntityUid?();
    }
    ObjectiveAssignedEvent args1 = new ObjectiveAssignedEvent(mindId, mind);
    this.RaiseLocalEvent<ObjectiveAssignedEvent>(uid, ref args1);
    if (args1.Cancelled)
    {
      this.Del(new EntityUid?(uid));
      this.Log.Warning($"Could not assign objective {proto}, deleted it");
      return new EntityUid?();
    }
    ObjectiveAfterAssignEvent args2 = new ObjectiveAfterAssignEvent(mindId, mind, comp, this.MetaData(uid));
    this.RaiseLocalEvent<ObjectiveAfterAssignEvent>(uid, ref args2);
    this.Log.Debug($"Created objective {this.ToPrettyString((Entity<MetaDataComponent>) uid):objective}");
    return new EntityUid?(uid);
  }

  public bool TryCreateObjective(
    Entity<MindComponent> mind,
    EntProtoId proto,
    [NotNullWhen(true)] out EntityUid? objective)
  {
    objective = this.TryCreateObjective(mind.Owner, mind.Comp, (string) proto);
    return objective.HasValue;
  }

  public ObjectiveInfo? GetInfo(EntityUid uid, EntityUid mindId, MindComponent? mind = null)
  {
    if (!this.Resolve<MindComponent>(mindId, ref mind))
      return new ObjectiveInfo?();
    float? progress = this.GetProgress(uid, (Entity<MindComponent>) (mindId, mind));
    if (!progress.HasValue)
      return new ObjectiveInfo?();
    float valueOrDefault = progress.GetValueOrDefault();
    ObjectiveComponent objectiveComponent = this.Comp<ObjectiveComponent>(uid);
    MetaDataComponent metaDataComponent = this.MetaData(uid);
    string entityName = metaDataComponent.EntityName;
    string entityDescription = metaDataComponent.EntityDescription;
    if (objectiveComponent.Icon != null)
      return new ObjectiveInfo?(new ObjectiveInfo(entityName, entityDescription, objectiveComponent.Icon, valueOrDefault));
    this.Log.Error($"An objective {this.ToPrettyString((Entity<MetaDataComponent>) uid):objective} of {this._mind.MindOwnerLoggingString(mind)} is missing an icon!");
    return new ObjectiveInfo?();
  }

  public float? GetProgress(EntityUid uid, Entity<MindComponent> mind)
  {
    ObjectiveGetProgressEvent args = new ObjectiveGetProgressEvent((EntityUid) mind, mind.Comp);
    this.RaiseLocalEvent<ObjectiveGetProgressEvent>(uid, ref args);
    if (args.Progress.HasValue)
      return args.Progress;
    this.Log.Error($"Objective {this.ToPrettyString((Entity<MetaDataComponent>) uid):objective} of {this._mind.MindOwnerLoggingString(mind.Comp)} didn't set a progress value!");
    return new float?();
  }

  public bool IsCompleted(EntityUid uid, Entity<MindComponent> mind)
  {
    return (double) this.GetProgress(uid, mind).GetValueOrDefault() >= 0.99900001287460327;
  }

  public void SetIcon(EntityUid uid, SpriteSpecifier icon, ObjectiveComponent? comp = null)
  {
    if (!this.Resolve<ObjectiveComponent>(uid, ref comp))
      return;
    comp.Icon = icon;
  }
}
