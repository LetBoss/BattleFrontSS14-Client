// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Rules.RMCPlanetMapPrototypeComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Item;
using Content.Shared.Roles;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Rules;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (RMCPlanetSystem)})]
public sealed class RMCPlanetMapPrototypeComponent : 
  Component,
  ISerializationGenerated<RMCPlanetMapPrototypeComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, true, false, null)]
  [AutoNetworkedField]
  [Access(new Type[] {}, Other = AccessPermissions.ReadExecute)]
  public ResPath Map;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public CamouflageType Camouflage = CamouflageType.Jungle;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int MinPlayers;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int MaxPlayers;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public string? Announcement;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public List<(ProtoId<JobPrototype> Job, int Amount)>? SurvivorJobs;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public Dictionary<ProtoId<JobPrototype>, List<(ProtoId<JobPrototype> Insert, int Amount)>>? SurvivorJobInserts;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public Dictionary<ProtoId<JobPrototype>, ProtoId<JobPrototype>>? SurvivorJobOverrides;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool SelectRandomSurvivorInsert = true;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public Dictionary<string, Dictionary<ProtoId<JobPrototype>, List<(ProtoId<JobPrototype> Special, int Amount)>>>? SurvivorJobScenarios;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public List<RMCNightmareScenario>? NightmareScenarios;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool InRotation = true;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref RMCPlanetMapPrototypeComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (RMCPlanetMapPrototypeComponent) target1;
    if (serialization.TryCustomCopy<RMCPlanetMapPrototypeComponent>(this, ref target, hookCtx, false, context))
      return;
    ResPath target2 = new ResPath();
    if (!serialization.TryCustomCopy<ResPath>(this.Map, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<ResPath>(this.Map, hookCtx, context);
    target.Map = target2;
    CamouflageType target3 = (CamouflageType) 0;
    if (!serialization.TryCustomCopy<CamouflageType>(this.Camouflage, ref target3, hookCtx, false, context))
      target3 = this.Camouflage;
    target.Camouflage = target3;
    int target4 = 0;
    if (!serialization.TryCustomCopy<int>(this.MinPlayers, ref target4, hookCtx, false, context))
      target4 = this.MinPlayers;
    target.MinPlayers = target4;
    int target5 = 0;
    if (!serialization.TryCustomCopy<int>(this.MaxPlayers, ref target5, hookCtx, false, context))
      target5 = this.MaxPlayers;
    target.MaxPlayers = target5;
    string target6 = (string) null;
    if (!serialization.TryCustomCopy<string>(this.Announcement, ref target6, hookCtx, false, context))
      target6 = this.Announcement;
    target.Announcement = target6;
    List<(ProtoId<JobPrototype>, int)> target7 = (List<(ProtoId<JobPrototype>, int)>) null;
    if (!serialization.TryCustomCopy<List<(ProtoId<JobPrototype>, int)>>(this.SurvivorJobs, ref target7, hookCtx, true, context))
      target7 = serialization.CreateCopy<List<(ProtoId<JobPrototype>, int)>>(this.SurvivorJobs, hookCtx, context);
    target.SurvivorJobs = target7;
    Dictionary<ProtoId<JobPrototype>, List<(ProtoId<JobPrototype>, int)>> target8 = (Dictionary<ProtoId<JobPrototype>, List<(ProtoId<JobPrototype>, int)>>) null;
    if (!serialization.TryCustomCopy<Dictionary<ProtoId<JobPrototype>, List<(ProtoId<JobPrototype>, int)>>>(this.SurvivorJobInserts, ref target8, hookCtx, true, context))
      target8 = serialization.CreateCopy<Dictionary<ProtoId<JobPrototype>, List<(ProtoId<JobPrototype>, int)>>>(this.SurvivorJobInserts, hookCtx, context);
    target.SurvivorJobInserts = target8;
    Dictionary<ProtoId<JobPrototype>, ProtoId<JobPrototype>> target9 = (Dictionary<ProtoId<JobPrototype>, ProtoId<JobPrototype>>) null;
    if (!serialization.TryCustomCopy<Dictionary<ProtoId<JobPrototype>, ProtoId<JobPrototype>>>(this.SurvivorJobOverrides, ref target9, hookCtx, true, context))
      target9 = serialization.CreateCopy<Dictionary<ProtoId<JobPrototype>, ProtoId<JobPrototype>>>(this.SurvivorJobOverrides, hookCtx, context);
    target.SurvivorJobOverrides = target9;
    bool target10 = false;
    if (!serialization.TryCustomCopy<bool>(this.SelectRandomSurvivorInsert, ref target10, hookCtx, false, context))
      target10 = this.SelectRandomSurvivorInsert;
    target.SelectRandomSurvivorInsert = target10;
    Dictionary<string, Dictionary<ProtoId<JobPrototype>, List<(ProtoId<JobPrototype>, int)>>> target11 = (Dictionary<string, Dictionary<ProtoId<JobPrototype>, List<(ProtoId<JobPrototype>, int)>>>) null;
    if (!serialization.TryCustomCopy<Dictionary<string, Dictionary<ProtoId<JobPrototype>, List<(ProtoId<JobPrototype>, int)>>>>(this.SurvivorJobScenarios, ref target11, hookCtx, true, context))
      target11 = serialization.CreateCopy<Dictionary<string, Dictionary<ProtoId<JobPrototype>, List<(ProtoId<JobPrototype>, int)>>>>(this.SurvivorJobScenarios, hookCtx, context);
    target.SurvivorJobScenarios = target11;
    List<RMCNightmareScenario> target12 = (List<RMCNightmareScenario>) null;
    if (!serialization.TryCustomCopy<List<RMCNightmareScenario>>(this.NightmareScenarios, ref target12, hookCtx, true, context))
      target12 = serialization.CreateCopy<List<RMCNightmareScenario>>(this.NightmareScenarios, hookCtx, context);
    target.NightmareScenarios = target12;
    bool target13 = false;
    if (!serialization.TryCustomCopy<bool>(this.InRotation, ref target13, hookCtx, false, context))
      target13 = this.InRotation;
    target.InRotation = target13;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref RMCPlanetMapPrototypeComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref Component target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    RMCPlanetMapPrototypeComponent target1 = (RMCPlanetMapPrototypeComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (Component) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    RMCPlanetMapPrototypeComponent target1 = (RMCPlanetMapPrototypeComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void InternalCopy(
    ref IComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    RMCPlanetMapPrototypeComponent target1 = (RMCPlanetMapPrototypeComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (IComponent) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref IComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual RMCPlanetMapPrototypeComponent Component.Instantiate()
  {
    return new RMCPlanetMapPrototypeComponent();
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class RMCPlanetMapPrototypeComponent_AutoState : IComponentState
  {
    public ResPath Map;
    public CamouflageType Camouflage;
    public int MinPlayers;
    public int MaxPlayers;
    public string? Announcement;
    public List<(ProtoId<JobPrototype> Job, int Amount)>? SurvivorJobs;
    public Dictionary<ProtoId<JobPrototype>, List<(ProtoId<JobPrototype> Insert, int Amount)>>? SurvivorJobInserts;
    public Dictionary<ProtoId<JobPrototype>, ProtoId<JobPrototype>>? SurvivorJobOverrides;
    public bool SelectRandomSurvivorInsert;
    public Dictionary<string, Dictionary<ProtoId<JobPrototype>, List<(ProtoId<JobPrototype> Special, int Amount)>>>? SurvivorJobScenarios;
    public List<RMCNightmareScenario>? NightmareScenarios;
    public bool InRotation;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class RMCPlanetMapPrototypeComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<RMCPlanetMapPrototypeComponent, ComponentGetState>(new ComponentEventRefHandler<RMCPlanetMapPrototypeComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<RMCPlanetMapPrototypeComponent, ComponentHandleState>(new ComponentEventRefHandler<RMCPlanetMapPrototypeComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      RMCPlanetMapPrototypeComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new RMCPlanetMapPrototypeComponent.RMCPlanetMapPrototypeComponent_AutoState()
      {
        Map = component.Map,
        Camouflage = component.Camouflage,
        MinPlayers = component.MinPlayers,
        MaxPlayers = component.MaxPlayers,
        Announcement = component.Announcement,
        SurvivorJobs = component.SurvivorJobs,
        SurvivorJobInserts = component.SurvivorJobInserts,
        SurvivorJobOverrides = component.SurvivorJobOverrides,
        SelectRandomSurvivorInsert = component.SelectRandomSurvivorInsert,
        SurvivorJobScenarios = component.SurvivorJobScenarios,
        NightmareScenarios = component.NightmareScenarios,
        InRotation = component.InRotation
      };
    }

    private void OnHandleState(
      EntityUid uid,
      RMCPlanetMapPrototypeComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is RMCPlanetMapPrototypeComponent.RMCPlanetMapPrototypeComponent_AutoState current))
        return;
      component.Map = current.Map;
      component.Camouflage = current.Camouflage;
      component.MinPlayers = current.MinPlayers;
      component.MaxPlayers = current.MaxPlayers;
      component.Announcement = current.Announcement;
      component.SurvivorJobs = current.SurvivorJobs == null ? (List<(ProtoId<JobPrototype>, int)>) null : new List<(ProtoId<JobPrototype>, int)>((IEnumerable<(ProtoId<JobPrototype>, int)>) current.SurvivorJobs);
      component.SurvivorJobInserts = current.SurvivorJobInserts == null ? (Dictionary<ProtoId<JobPrototype>, List<(ProtoId<JobPrototype>, int)>>) null : new Dictionary<ProtoId<JobPrototype>, List<(ProtoId<JobPrototype>, int)>>((IDictionary<ProtoId<JobPrototype>, List<(ProtoId<JobPrototype>, int)>>) current.SurvivorJobInserts);
      component.SurvivorJobOverrides = current.SurvivorJobOverrides == null ? (Dictionary<ProtoId<JobPrototype>, ProtoId<JobPrototype>>) null : new Dictionary<ProtoId<JobPrototype>, ProtoId<JobPrototype>>((IDictionary<ProtoId<JobPrototype>, ProtoId<JobPrototype>>) current.SurvivorJobOverrides);
      component.SelectRandomSurvivorInsert = current.SelectRandomSurvivorInsert;
      component.SurvivorJobScenarios = current.SurvivorJobScenarios == null ? (Dictionary<string, Dictionary<ProtoId<JobPrototype>, List<(ProtoId<JobPrototype>, int)>>>) null : new Dictionary<string, Dictionary<ProtoId<JobPrototype>, List<(ProtoId<JobPrototype>, int)>>>((IDictionary<string, Dictionary<ProtoId<JobPrototype>, List<(ProtoId<JobPrototype>, int)>>>) current.SurvivorJobScenarios);
      component.NightmareScenarios = current.NightmareScenarios == null ? (List<RMCNightmareScenario>) null : new List<RMCNightmareScenario>((IEnumerable<RMCNightmareScenario>) current.NightmareScenarios);
      component.InRotation = current.InRotation;
    }
  }
}
