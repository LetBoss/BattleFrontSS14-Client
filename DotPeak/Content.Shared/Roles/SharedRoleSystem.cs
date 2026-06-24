// Decompiled with JetBrains decompiler
// Type: Content.Shared.Roles.SharedRoleSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

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
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

#nullable enable
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
    this.Subs.CVar<string>(this._cfg, CCVars.GameRoleTimerOverride, new Action<string>(this.SetRequirementOverride), true);
    this.SubscribeLocalEvent<MindRoleComponent, ComponentShutdown>(new EntityEventRefHandler<MindRoleComponent, ComponentShutdown>(this.OnComponentShutdown));
    this.SubscribeLocalEvent<StartingMindRoleComponent, PlayerSpawnCompleteEvent>(new ComponentEventHandler<StartingMindRoleComponent, PlayerSpawnCompleteEvent>(this.OnSpawn));
  }

  private void OnSpawn(
    EntityUid uid,
    StartingMindRoleComponent component,
    PlayerSpawnCompleteEvent args)
  {
    EntityUid mindId;
    MindComponent mind;
    if (!this._minds.TryGetMind(uid, out mindId, out mind))
      return;
    this.MindAddRole(mindId, component.MindRole, mind, component.Silent);
  }

  private void SetRequirementOverride(string value)
  {
    if (string.IsNullOrEmpty(value))
    {
      this._requirementOverride = (JobRequirementOverridePrototype) null;
    }
    else
    {
      if (this._prototypes.TryIndex<JobRequirementOverridePrototype>(value, out this._requirementOverride))
        return;
      this.Log.Error("Unknown JobRequirementOverridePrototype: " + value);
    }
  }

  public void MindAddRoles(
    EntityUid mindId,
    List<EntProtoId>? roles,
    MindComponent? mind = null,
    bool silent = false)
  {
    if (roles == null || roles.Count == 0)
      return;
    foreach (EntProtoId role in roles)
      this.MindAddRole(mindId, role, mind, silent);
  }

  public void MindAddRole(EntityUid mindId, EntProtoId protoId, MindComponent? mind = null, bool silent = false)
  {
    if (protoId == (EntProtoId) "MindRoleJob")
      this.MindAddJobRole(mindId, mind, silent, "");
    else
      this.MindAddRoleDo(mindId, protoId, mind, silent);
  }

  public void MindAddJobRole(
    EntityUid mindId,
    MindComponent? mind = null,
    bool silent = false,
    string? jobPrototype = null)
  {
    if (!this.Resolve<MindComponent>(mindId, ref mind))
      return;
    Entity<MindRoleComponent, JobRoleComponent>? role;
    if (this.MindHasRole<JobRoleComponent>((Entity<MindComponent>) (mindId, mind), out role))
    {
      ProtoId<JobPrototype>? jobPrototype1 = role.Value.Comp1.JobPrototype;
      ProtoId<JobPrototype>? nullable = (ProtoId<JobPrototype>?) jobPrototype;
      if ((jobPrototype1.HasValue == nullable.HasValue ? (jobPrototype1.HasValue ? (jobPrototype1.GetValueOrDefault() != nullable.GetValueOrDefault() ? 1 : 0) : 0) : 1) != 0)
      {
        ISharedAdminLogManager adminLogger = this._adminLogger;
        LogStringHandler logStringHandler = new LogStringHandler(34, 3);
        logStringHandler.AppendLiteral("Job Role of ");
        logStringHandler.AppendFormatted<EntityStringRepresentation?>(this.ToPrettyString(mind.OwnedEntity), "ToPrettyString(mind.OwnedEntity)");
        logStringHandler.AppendLiteral(" changed from '");
        logStringHandler.AppendFormatted<ProtoId<JobPrototype>?>(role.Value.Comp1.JobPrototype, "jobRole.Value.Comp1.JobPrototype");
        logStringHandler.AppendLiteral("' to '");
        logStringHandler.AppendFormatted(jobPrototype);
        logStringHandler.AppendLiteral("'");
        ref LogStringHandler local = ref logStringHandler;
        adminLogger.Add(LogType.Mind, LogImpact.Low, ref local);
        role.Value.Comp1.JobPrototype = (ProtoId<JobPrototype>?) jobPrototype;
        return;
      }
    }
    this.MindAddRoleDo(mindId, (EntProtoId) "MindRoleJob", mind, silent, jobPrototype);
  }

  private void MindAddRoleDo(
    EntityUid mindId,
    EntProtoId protoId,
    MindComponent? mind = null,
    bool silent = false,
    string? jobPrototype = null)
  {
    if (!this.Resolve<MindComponent>(mindId, ref mind))
    {
      this.Log.Error($"Failed to add role {protoId} to {this.ToPrettyString((Entity<MetaDataComponent>) mindId)} : Mind does not match provided mind component");
    }
    else
    {
      EntityPrototype prototype;
      if (!this._prototypes.TryIndex(protoId, out prototype))
      {
        this.Log.Error($"Failed to add role {protoId} to {this.ToPrettyString((Entity<MetaDataComponent>) mindId)} : Role prototype does not exist");
      }
      else
      {
        EntityUid entityUid = this.Spawn((string) protoId, MapCoordinates.Nullspace, rotation: new Angle());
        this.EnsureComp<MindRoleComponent>(entityUid);
        MindRoleComponent mindRoleComponent = this.Comp<MindRoleComponent>(entityUid);
        mindRoleComponent.Mind = (Entity<MindComponent>) (mindId, mind);
        if (jobPrototype != null)
        {
          mindRoleComponent.JobPrototype = (ProtoId<JobPrototype>?) jobPrototype;
          this.EnsureComp<JobRoleComponent>(entityUid);
        }
        this.Dirty(entityUid, (IComponent) mindRoleComponent);
        mind.MindRoles.Add(entityUid);
        this.Dirty(mindId, (IComponent) mind);
        bool RoleTypeUpdate = this.MindRolesUpdate((Entity<MindComponent>) (mindId, mind));
        RoleAddedEvent args = new RoleAddedEvent(mindId, mind, RoleTypeUpdate, silent);
        this.RaiseLocalEvent<RoleAddedEvent>(mindId, args, true);
        string str = this.Loc.GetString(prototype.Name);
        if (mind.OwnedEntity.HasValue)
        {
          ISharedAdminLogManager adminLogger = this._adminLogger;
          LogStringHandler logStringHandler = new LogStringHandler(18, 2);
          logStringHandler.AppendFormatted(str);
          logStringHandler.AppendLiteral(" added to mind of ");
          logStringHandler.AppendFormatted<EntityStringRepresentation?>(this.ToPrettyString(mind.OwnedEntity), "ToPrettyString(mind.OwnedEntity)");
          ref LogStringHandler local = ref logStringHandler;
          adminLogger.Add(LogType.Mind, LogImpact.Low, ref local);
        }
        else
        {
          this.Log.Error($"{this.ToPrettyString((Entity<MetaDataComponent>) mindId)} does not have an OwnedEntity!");
          ISharedAdminLogManager adminLogger = this._adminLogger;
          LogStringHandler logStringHandler = new LogStringHandler(10, 2);
          logStringHandler.AppendFormatted(str);
          logStringHandler.AppendLiteral(" added to ");
          logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) mindId), "ToPrettyString(mindId)");
          ref LogStringHandler local = ref logStringHandler;
          adminLogger.Add(LogType.Mind, LogImpact.Low, ref local);
        }
        NetUserId? originalOwnerUserId = mind.OriginalOwnerUserId;
        if (!originalOwnerUserId.HasValue)
          return;
        NetUserId valueOrDefault = originalOwnerUserId.GetValueOrDefault();
        this._rmcPvs.AddSessionOverride(entityUid, valueOrDefault);
      }
    }
  }

  private bool MindRolesUpdate(Entity<MindComponent?> ent)
  {
    if (!this.Resolve<MindComponent>(ent.Owner, ref ent.Comp))
      return false;
    (ProtoId<RoleTypePrototype> roleTypeId, LocId? subtype1) = this.GetRoleTypeByTime(ent.Comp);
    if (ent.Comp.RoleType == roleTypeId)
    {
      LocId? subtype2 = ent.Comp.Subtype;
      LocId? nullable = subtype1;
      if ((subtype2.HasValue == nullable.HasValue ? (subtype2.HasValue ? (subtype2.GetValueOrDefault() == nullable.GetValueOrDefault() ? 1 : 0) : 1) : 0) != 0)
        return false;
    }
    this.SetRoleType(ent.Owner, roleTypeId, subtype1);
    return true;
  }

  private (ProtoId<RoleTypePrototype>, LocId?) GetRoleTypeByTime(MindComponent mind)
  {
    Entity<MindRoleComponent>? roleCompByTime = this.GetRoleCompByTime(mind);
    return ((ProtoId<RoleTypePrototype>?) roleCompByTime?.Comp?.RoleType ?? (ProtoId<RoleTypePrototype>) "Neutral", (LocId?) roleCompByTime?.Comp?.Subtype);
  }

  public Entity<MindRoleComponent>? GetRoleCompByTime(MindComponent mind)
  {
    List<Entity<MindRoleComponent>> source = new List<Entity<MindRoleComponent>>();
    foreach (EntityUid mindRole in mind.MindRoles)
    {
      MindRoleComponent mindRoleComponent = this.Comp<MindRoleComponent>(mindRole);
      if (mindRoleComponent.RoleType.HasValue)
        source.Add((Entity<MindRoleComponent>) (mindRole, mindRoleComponent));
    }
    return source.Count <= 0 ? new Entity<MindRoleComponent>?() : new Entity<MindRoleComponent>?(source.LastOrDefault<Entity<MindRoleComponent>>());
  }

  private void SetRoleType(EntityUid mind, ProtoId<RoleTypePrototype> roleTypeId, LocId? subtype)
  {
    MindComponent comp;
    if (!this.TryComp<MindComponent>(mind, out comp))
      this.Log.Error($"Failed to update Role Type of mind entity {this.ToPrettyString((Entity<MetaDataComponent>) mind)} to {roleTypeId}, {subtype}. MindComponent not found.");
    else if (!this._prototypes.HasIndex<RoleTypePrototype>(roleTypeId))
    {
      this.Log.Error($"Failed to change Role Type of {this._minds.MindOwnerLoggingString(comp)} to {roleTypeId}, {subtype}. Invalid role");
    }
    else
    {
      comp.RoleType = roleTypeId;
      comp.Subtype = subtype;
      this.Dirty(mind, (IComponent) comp);
      ICommonSession session;
      if (this.Player.TryGetSessionById(comp.UserId, out session))
      {
        this.RaiseNetworkEvent((EntityEventArgs) new MindRoleTypeChangedEvent(), session.Channel);
      }
      else
      {
        string str = $"The Character Window of {this._minds.MindOwnerLoggingString(comp)} potentially did not update immediately : session error";
        ISharedAdminLogManager adminLogger = this._adminLogger;
        LogStringHandler logStringHandler = new LogStringHandler(0, 1);
        logStringHandler.AppendFormatted(str);
        ref LogStringHandler local = ref logStringHandler;
        adminLogger.Add(LogType.Mind, LogImpact.Medium, ref local);
      }
      if (!comp.OwnedEntity.HasValue)
      {
        this.Log.Error($"{this.ToPrettyString((Entity<MetaDataComponent>) mind)} does not have an OwnedEntity!");
        ISharedAdminLogManager adminLogger = this._adminLogger;
        LogStringHandler logStringHandler = new LogStringHandler(27, 3);
        logStringHandler.AppendLiteral("Role Type of ");
        logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) mind), "ToPrettyString(mind)");
        logStringHandler.AppendLiteral(" changed to ");
        logStringHandler.AppendFormatted<ProtoId<RoleTypePrototype>>(roleTypeId, nameof (roleTypeId));
        logStringHandler.AppendLiteral(", ");
        logStringHandler.AppendFormatted<LocId?>(subtype, nameof (subtype));
        ref LogStringHandler local = ref logStringHandler;
        adminLogger.Add(LogType.Mind, LogImpact.Medium, ref local);
      }
      else
      {
        ISharedAdminLogManager adminLogger = this._adminLogger;
        LogStringHandler logStringHandler = new LogStringHandler(27, 3);
        logStringHandler.AppendLiteral("Role Type of ");
        logStringHandler.AppendFormatted<EntityStringRepresentation?>(this.ToPrettyString(comp.OwnedEntity), "ToPrettyString(comp.OwnedEntity)");
        logStringHandler.AppendLiteral(" changed to ");
        logStringHandler.AppendFormatted<ProtoId<RoleTypePrototype>>(roleTypeId, nameof (roleTypeId));
        logStringHandler.AppendLiteral(", ");
        logStringHandler.AppendFormatted<LocId?>(subtype, nameof (subtype));
        ref LogStringHandler local = ref logStringHandler;
        adminLogger.Add(LogType.Mind, LogImpact.High, ref local);
      }
    }
  }

  public bool MindRemoveRole<T>(Entity<MindComponent?> mind) where T : IComponent
  {
    if (typeof (T) == typeof (MindRoleComponent))
      throw new InvalidOperationException();
    if (!this.Resolve<MindComponent>(mind.Owner, ref mind.Comp))
      return false;
    List<EntityUid> delete = new List<EntityUid>();
    string original = $"'{typeof (T).Name}'";
    string str = original;
    foreach (EntityUid mindRole in mind.Comp.MindRoles)
    {
      if (!this.HasComp<MindRoleComponent>(mindRole))
        this.Log.Error($"Encountered mind role entity {this.ToPrettyString((Entity<MetaDataComponent>) mindRole)} without a {"MindRoleComponent"}");
      else if (this.HasComp<T>(mindRole))
      {
        delete.Add(mindRole);
        str = this.RemoveRoleLogNameGeneration(str, this.MetaData(mindRole).EntityName, original);
      }
    }
    return this.MindRemoveRoleDo(mind, delete, str);
  }

  private string RemoveRoleLogNameGeneration(string name, string newName, string original)
  {
    if (name == original)
      name = $"'{newName}'";
    else if (!name.Contains(newName))
      name = $"{name}, '{newName}'";
    return name;
  }

  public bool MindRemoveRole<T>(EntityUid mindId) where T : IComponent
  {
    MindComponent comp;
    if (this.TryComp<MindComponent>(mindId, out comp))
      return this.MindRemoveRole<T>((Entity<MindComponent>) (mindId, comp));
    this.Log.Error($"The specified mind entity '{this.ToPrettyString((Entity<MetaDataComponent>) mindId)}' does not have a {"MindComponent"}");
    return false;
  }

  public bool MindRemoveRole(Entity<MindComponent?> mind, EntProtoId<MindRoleComponent> protoId)
  {
    if (!this.Resolve<MindComponent>(mind.Owner, ref mind.Comp))
      return false;
    string original = $"'{(string) protoId}'";
    string str = original;
    List<EntityUid> delete = new List<EntityUid>();
    foreach (EntityUid mindRole in mind.Comp.MindRoles)
    {
      if (!this.HasComp<MindRoleComponent>(mindRole))
      {
        this.Log.Error($"Encountered mind role entity {this.ToPrettyString((Entity<MetaDataComponent>) mindRole)} without a {"MindRoleComponent"}");
      }
      else
      {
        string id = this.MetaData(mindRole).EntityPrototype?.ID;
        if (id != null && !((EntProtoId<MindRoleComponent>) id != protoId))
        {
          delete.Add(mindRole);
          str = this.RemoveRoleLogNameGeneration(str, this.MetaData(mindRole).EntityName, original);
        }
      }
    }
    return this.MindRemoveRoleDo(mind, delete, str);
  }

  private bool MindRemoveRoleDo(Entity<MindComponent?> mind, List<EntityUid> delete, string? logName = "")
  {
    if (!this.Resolve<MindComponent>(mind.Owner, ref mind.Comp))
      return false;
    if (delete.Count <= 0)
    {
      this.Log.Warning($"Failed to remove mind role {logName} from {this.ToPrettyString((Entity<MetaDataComponent>) mind.Owner)} : mind does not have this role ");
      return false;
    }
    foreach (EntityUid entityUid in delete)
      this._entityManager.DeleteEntity(new EntityUid?(entityUid));
    bool RoleTypeUpdate = this.MindRolesUpdate(mind);
    RoleRemovedEvent args = new RoleRemovedEvent(mind.Owner, mind.Comp, RoleTypeUpdate);
    this.RaiseLocalEvent<RoleRemovedEvent>((EntityUid) mind, args, true);
    ISharedAdminLogManager adminLogger = this._adminLogger;
    LogStringHandler logStringHandler = new LogStringHandler(40, 2);
    logStringHandler.AppendLiteral("All roles of type ");
    logStringHandler.AppendFormatted(logName);
    logStringHandler.AppendLiteral(" removed from mind of ");
    logStringHandler.AppendFormatted<EntityStringRepresentation?>(this.ToPrettyString(mind.Comp.OwnedEntity), "ToPrettyString(mind.Comp.OwnedEntity)");
    ref LogStringHandler local = ref logStringHandler;
    adminLogger.Add(LogType.Mind, LogImpact.Low, ref local);
    return true;
  }

  private void OnComponentShutdown(Entity<MindRoleComponent> ent, ref ComponentShutdown args)
  {
    if (ent.Comp.Mind.Comp == null)
      return;
    ent.Comp.Mind.Comp.MindRoles.Remove(ent.Owner);
  }

  public bool MindHasRole<T>(Entity<MindComponent?> mind, [NotNullWhen(true)] out Entity<MindRoleComponent, T>? role) where T : IComponent
  {
    role = new Entity<MindRoleComponent, T>?();
    if (!this.Resolve<MindComponent>(mind.Owner, ref mind.Comp))
      return false;
    foreach (EntityUid mindRole in mind.Comp.MindRoles)
    {
      T comp1;
      if (this.TryComp<T>(mindRole, out comp1))
      {
        MindRoleComponent comp2;
        if (!this.TryComp<MindRoleComponent>(mindRole, out comp2))
        {
          this.Log.Error($"Encountered mind role entity {this.ToPrettyString((Entity<MetaDataComponent>) mindRole)} without a {"MindRoleComponent"}");
        }
        else
        {
          role = new Entity<MindRoleComponent, T>?((Entity<MindRoleComponent, T>) (mindRole, comp2, comp1));
          return true;
        }
      }
    }
    return false;
  }

  public bool MindHasRole(EntityUid mindId, Type type, [NotNullWhen(true)] out Entity<MindRoleComponent>? role)
  {
    role = new Entity<MindRoleComponent>?();
    if (type == Type.GetType("MindRoleComponent"))
    {
      this.Log.Error($"Something attempted to query mind role 'MindRoleComponent' on mind {mindId}. This component is present on every single mind role.");
      return false;
    }
    MindComponent comp1;
    if (!this.TryComp<MindComponent>(mindId, out comp1))
      return false;
    bool flag = false;
    foreach (EntityUid mindRole in comp1.MindRoles)
    {
      if (this.HasComp(mindRole, type))
      {
        MindRoleComponent comp2;
        if (!this.TryComp<MindRoleComponent>(mindRole, out comp2))
        {
          this.Log.Error($"Encountered mind role entity {this.ToPrettyString((Entity<MetaDataComponent>) mindRole)} without a {"MindRoleComponent"}");
        }
        else
        {
          role = new Entity<MindRoleComponent>?((Entity<MindRoleComponent>) (mindRole, comp2));
          flag = true;
          break;
        }
      }
    }
    return flag;
  }

  public bool MindHasRole<T>(EntityUid mindId) where T : IComponent
  {
    return this.MindHasRole<T>((Entity<MindComponent>) mindId, out Entity<MindRoleComponent, T>? _);
  }

  [Obsolete("Use MindHasRole's output value")]
  public Entity<MindRoleComponent>? MindGetRole<T>(EntityUid mindId) where T : IComponent
  {
    Entity<MindRoleComponent>? role = new Entity<MindRoleComponent>?();
    foreach (EntityUid mindRole in this.Comp<MindComponent>(mindId).MindRoles)
    {
      MindRoleComponent comp;
      if (this.HasComp<T>(mindRole) && this.TryComp<MindRoleComponent>(mindRole, out comp))
        role = new Entity<MindRoleComponent>?((Entity<MindRoleComponent>) (mindRole, comp));
    }
    return role;
  }

  public List<RoleInfo> MindGetAllRoleInfo(Entity<MindComponent?> mind)
  {
    List<RoleInfo> allRoleInfo = new List<RoleInfo>();
    if (!this.Resolve<MindComponent>(mind.Owner, ref mind.Comp))
      return allRoleInfo;
    foreach (EntityUid mindRole in mind.Comp.MindRoles)
    {
      bool flag = false;
      string Name = "game-ticker-unknown-role";
      string Prototype = "";
      string PlayTimeTrackerId = (string) null;
      MindRoleComponent comp;
      if (!this.TryComp<MindRoleComponent>(mindRole, out comp))
      {
        this.Log.Error($"Encountered mind role entity {this.ToPrettyString((Entity<MetaDataComponent>) mindRole)} without a {"MindRoleComponent"}");
      }
      else
      {
        ProtoId<AntagPrototype>? antagPrototype = comp.AntagPrototype;
        if (antagPrototype.HasValue)
        {
          antagPrototype = comp.AntagPrototype;
          Prototype = antagPrototype.HasValue ? (string) antagPrototype.GetValueOrDefault() : (string) null;
        }
        ProtoId<JobPrototype>? jobPrototype = comp.JobPrototype;
        if (jobPrototype.HasValue)
        {
          antagPrototype = comp.AntagPrototype;
          if (!antagPrototype.HasValue)
          {
            jobPrototype = comp.JobPrototype;
            Prototype = jobPrototype.HasValue ? (string) jobPrototype.GetValueOrDefault() : (string) null;
            JobPrototype prototype;
            if (this._prototypes.TryIndex<JobPrototype>(comp.JobPrototype, out prototype))
            {
              PlayTimeTrackerId = prototype.PlayTimeTracker;
              Name = prototype.Name;
              flag = true;
              goto label_21;
            }
            this.Log.Error($" Mind Role Prototype '{mindRole.Id}' contains invalid Job prototype: '{comp.JobPrototype}'");
            goto label_21;
          }
        }
        antagPrototype = comp.AntagPrototype;
        if (antagPrototype.HasValue)
        {
          jobPrototype = comp.JobPrototype;
          if (!jobPrototype.HasValue)
          {
            antagPrototype = comp.AntagPrototype;
            Prototype = antagPrototype.HasValue ? (string) antagPrototype.GetValueOrDefault() : (string) null;
            AntagPrototype prototype;
            if (this._prototypes.TryIndex<AntagPrototype>(comp.AntagPrototype, out prototype))
            {
              Name = prototype.Name;
              flag = true;
              goto label_21;
            }
            this.Log.Error($" Mind Role Prototype '{mindRole.Id}' contains invalid Antagonist prototype: '{comp.AntagPrototype}'");
            goto label_21;
          }
        }
        jobPrototype = comp.JobPrototype;
        if (jobPrototype.HasValue)
        {
          antagPrototype = comp.AntagPrototype;
          if (antagPrototype.HasValue)
            this.Log.Error($" Mind Role Prototype '{mindRole.Id}' contains both Job and Antagonist prototypes");
        }
label_21:
        if (flag)
          allRoleInfo.Add(new RoleInfo(Name, comp.Antag, PlayTimeTrackerId, Prototype));
      }
    }
    return allRoleInfo;
  }

  public bool MindIsAntagonist(EntityUid? mindId)
  {
    return mindId.HasValue && this.CheckAntagonistStatus((Entity<MindComponent>) mindId.Value).Antag;
  }

  public bool MindIsExclusiveAntagonist(EntityUid? mindId)
  {
    return mindId.HasValue && this.CheckAntagonistStatus((Entity<MindComponent>) mindId.Value).ExclusiveAntag;
  }

  private (bool Antag, bool ExclusiveAntag) CheckAntagonistStatus(Entity<MindComponent?> mind)
  {
    if (!this.Resolve<MindComponent>(mind.Owner, ref mind.Comp))
      return (false, false);
    bool flag1 = false;
    bool flag2 = false;
    foreach (EntityUid mindRole in mind.Comp.MindRoles)
    {
      MindRoleComponent comp;
      if (!this.TryComp<MindRoleComponent>(mindRole, out comp))
      {
        this.Log.Error($"Mind Role Entity {this.ToPrettyString((Entity<MetaDataComponent>) mindRole)} does not have a MindRoleComponent, despite being listed as a role belonging to {this.ToPrettyString(new EntityUid?((EntityUid) mind))}|");
      }
      else
      {
        flag1 |= comp.Antag;
        flag2 |= comp.ExclusiveAntag;
      }
    }
    return (flag1, flag2);
  }

  public void MindPlaySound(EntityUid mindId, SoundSpecifier? sound, MindComponent? mind = null)
  {
    ICommonSession session;
    if (!this.Resolve<MindComponent>(mindId, ref mind) || !this.Player.TryGetSessionById(mind.UserId, out session))
      return;
    this._audio.PlayGlobal(sound, session);
  }

  public HashSet<JobRequirement>? GetJobRequirement(JobPrototype job)
  {
    HashSet<JobRequirement> jobRequirementSet;
    return this._requirementOverride != null && this._requirementOverride.Jobs.TryGetValue((ProtoId<JobPrototype>) job.ID, out jobRequirementSet) ? jobRequirementSet : job.Requirements;
  }

  public HashSet<JobRequirement>? GetJobRequirement(ProtoId<JobPrototype> job)
  {
    HashSet<JobRequirement> jobRequirementSet;
    return this._requirementOverride != null && this._requirementOverride.Jobs.TryGetValue(job, out jobRequirementSet) ? jobRequirementSet : this._prototypes.Index<JobPrototype>(job).Requirements;
  }

  public HashSet<JobRequirement>? GetAntagRequirement(ProtoId<AntagPrototype> antag)
  {
    HashSet<JobRequirement> jobRequirementSet;
    return this._requirementOverride != null && this._requirementOverride.Antags.TryGetValue(antag, out jobRequirementSet) ? jobRequirementSet : this._prototypes.Index<AntagPrototype>(antag).Requirements;
  }

  public HashSet<JobRequirement>? GetAntagRequirement(AntagPrototype antag)
  {
    HashSet<JobRequirement> jobRequirementSet;
    return this._requirementOverride != null && this._requirementOverride.Antags.TryGetValue((ProtoId<AntagPrototype>) antag.ID, out jobRequirementSet) ? jobRequirementSet : antag.Requirements;
  }

  public string GetRoleSubtypeLabel(LocId roleType, LocId? subtype)
  {
    LocId? nullable1 = subtype;
    if (string.IsNullOrEmpty(nullable1.HasValue ? (string) nullable1.GetValueOrDefault() : (string) null))
      return this.Loc.GetString((string) roleType);
    ILocalizationManager loc = this.Loc;
    LocId? nullable2 = subtype;
    string valueOrDefault = nullable2.HasValue ? (string) nullable2.GetValueOrDefault() : (string) null;
    return loc.GetString(valueOrDefault);
  }
}
