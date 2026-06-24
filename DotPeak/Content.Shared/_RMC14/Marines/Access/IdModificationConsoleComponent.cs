// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Marines.Access.IdModificationConsoleComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Weapons.Ranged.IFF;
using Content.Shared.Access;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Marines.Access;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(true, false)]
public sealed class IdModificationConsoleComponent : 
  Component,
  ISerializationGenerated<IdModificationConsoleComponent>,
  ISerializationGenerated
{
  public static string PrivilegedIdCardSlotId = "IdCardConsole-privilegedId";
  public static string TargetIdCardSlotId = "IdCardConsole-targetId";
  public ProtoId<AccessLevelPrototype> Access = (ProtoId<AccessLevelPrototype>) "RMCAccessDatabase";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public HashSet<ProtoId<AccessLevelPrototype>> AccessGroups = new HashSet<ProtoId<AccessLevelPrototype>>();
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public HashSet<ProtoId<AccessLevelPrototype>> AccessList = new HashSet<ProtoId<AccessLevelPrototype>>();
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool Authenticated;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntProtoId<IFFFactionComponent> Faction = (EntProtoId<IFFFactionComponent>) "FactionMarine";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool HasIFF;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public HashSet<ProtoId<AccessLevelPrototype>> HiddenAccessList = new HashSet<ProtoId<AccessLevelPrototype>>();
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public HashSet<ProtoId<AccessGroupPrototype>> JobGroups = new HashSet<ProtoId<AccessGroupPrototype>>();
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public HashSet<ProtoId<AccessGroupPrototype>> JobList = new HashSet<ProtoId<AccessGroupPrototype>>();
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public string PrivilegedIdSlot = nameof (PrivilegedIdSlot);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public string TargetIdSlot = nameof (TargetIdSlot);

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref IdModificationConsoleComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (IdModificationConsoleComponent) target1;
    if (serialization.TryCustomCopy<IdModificationConsoleComponent>(this, ref target, hookCtx, false, context))
      return;
    HashSet<ProtoId<AccessLevelPrototype>> target2 = (HashSet<ProtoId<AccessLevelPrototype>>) null;
    if (this.AccessGroups == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<HashSet<ProtoId<AccessLevelPrototype>>>(this.AccessGroups, ref target2, hookCtx, true, context))
      target2 = serialization.CreateCopy<HashSet<ProtoId<AccessLevelPrototype>>>(this.AccessGroups, hookCtx, context);
    target.AccessGroups = target2;
    HashSet<ProtoId<AccessLevelPrototype>> target3 = (HashSet<ProtoId<AccessLevelPrototype>>) null;
    if (this.AccessList == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<HashSet<ProtoId<AccessLevelPrototype>>>(this.AccessList, ref target3, hookCtx, true, context))
      target3 = serialization.CreateCopy<HashSet<ProtoId<AccessLevelPrototype>>>(this.AccessList, hookCtx, context);
    target.AccessList = target3;
    bool target4 = false;
    if (!serialization.TryCustomCopy<bool>(this.Authenticated, ref target4, hookCtx, false, context))
      target4 = this.Authenticated;
    target.Authenticated = target4;
    EntProtoId<IFFFactionComponent> target5 = new EntProtoId<IFFFactionComponent>();
    if (!serialization.TryCustomCopy<EntProtoId<IFFFactionComponent>>(this.Faction, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<EntProtoId<IFFFactionComponent>>(this.Faction, hookCtx, context);
    target.Faction = target5;
    bool target6 = false;
    if (!serialization.TryCustomCopy<bool>(this.HasIFF, ref target6, hookCtx, false, context))
      target6 = this.HasIFF;
    target.HasIFF = target6;
    HashSet<ProtoId<AccessLevelPrototype>> target7 = (HashSet<ProtoId<AccessLevelPrototype>>) null;
    if (this.HiddenAccessList == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<HashSet<ProtoId<AccessLevelPrototype>>>(this.HiddenAccessList, ref target7, hookCtx, true, context))
      target7 = serialization.CreateCopy<HashSet<ProtoId<AccessLevelPrototype>>>(this.HiddenAccessList, hookCtx, context);
    target.HiddenAccessList = target7;
    HashSet<ProtoId<AccessGroupPrototype>> target8 = (HashSet<ProtoId<AccessGroupPrototype>>) null;
    if (this.JobGroups == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<HashSet<ProtoId<AccessGroupPrototype>>>(this.JobGroups, ref target8, hookCtx, true, context))
      target8 = serialization.CreateCopy<HashSet<ProtoId<AccessGroupPrototype>>>(this.JobGroups, hookCtx, context);
    target.JobGroups = target8;
    HashSet<ProtoId<AccessGroupPrototype>> target9 = (HashSet<ProtoId<AccessGroupPrototype>>) null;
    if (this.JobList == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<HashSet<ProtoId<AccessGroupPrototype>>>(this.JobList, ref target9, hookCtx, true, context))
      target9 = serialization.CreateCopy<HashSet<ProtoId<AccessGroupPrototype>>>(this.JobList, hookCtx, context);
    target.JobList = target9;
    string target10 = (string) null;
    if (this.PrivilegedIdSlot == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.PrivilegedIdSlot, ref target10, hookCtx, false, context))
      target10 = this.PrivilegedIdSlot;
    target.PrivilegedIdSlot = target10;
    string target11 = (string) null;
    if (this.TargetIdSlot == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.TargetIdSlot, ref target11, hookCtx, false, context))
      target11 = this.TargetIdSlot;
    target.TargetIdSlot = target11;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref IdModificationConsoleComponent target,
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
    IdModificationConsoleComponent target1 = (IdModificationConsoleComponent) target;
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
    IdModificationConsoleComponent target1 = (IdModificationConsoleComponent) target;
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
    IdModificationConsoleComponent target1 = (IdModificationConsoleComponent) target;
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
  virtual IdModificationConsoleComponent Component.Instantiate()
  {
    return new IdModificationConsoleComponent();
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class IdModificationConsoleComponent_AutoState : IComponentState
  {
    public HashSet<ProtoId<AccessLevelPrototype>> AccessGroups;
    public HashSet<ProtoId<AccessLevelPrototype>> AccessList;
    public bool Authenticated;
    public EntProtoId<IFFFactionComponent> Faction;
    public bool HasIFF;
    public HashSet<ProtoId<AccessLevelPrototype>> HiddenAccessList;
    public HashSet<ProtoId<AccessGroupPrototype>> JobGroups;
    public HashSet<ProtoId<AccessGroupPrototype>> JobList;
    public string PrivilegedIdSlot;
    public string TargetIdSlot;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class IdModificationConsoleComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<IdModificationConsoleComponent, ComponentGetState>(new ComponentEventRefHandler<IdModificationConsoleComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<IdModificationConsoleComponent, ComponentHandleState>(new ComponentEventRefHandler<IdModificationConsoleComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      IdModificationConsoleComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new IdModificationConsoleComponent.IdModificationConsoleComponent_AutoState()
      {
        AccessGroups = component.AccessGroups,
        AccessList = component.AccessList,
        Authenticated = component.Authenticated,
        Faction = component.Faction,
        HasIFF = component.HasIFF,
        HiddenAccessList = component.HiddenAccessList,
        JobGroups = component.JobGroups,
        JobList = component.JobList,
        PrivilegedIdSlot = component.PrivilegedIdSlot,
        TargetIdSlot = component.TargetIdSlot
      };
    }

    private void OnHandleState(
      EntityUid uid,
      IdModificationConsoleComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is IdModificationConsoleComponent.IdModificationConsoleComponent_AutoState current))
        return;
      component.AccessGroups = current.AccessGroups == null ? (HashSet<ProtoId<AccessLevelPrototype>>) null : new HashSet<ProtoId<AccessLevelPrototype>>((IEnumerable<ProtoId<AccessLevelPrototype>>) current.AccessGroups);
      component.AccessList = current.AccessList == null ? (HashSet<ProtoId<AccessLevelPrototype>>) null : new HashSet<ProtoId<AccessLevelPrototype>>((IEnumerable<ProtoId<AccessLevelPrototype>>) current.AccessList);
      component.Authenticated = current.Authenticated;
      component.Faction = current.Faction;
      component.HasIFF = current.HasIFF;
      component.HiddenAccessList = current.HiddenAccessList == null ? (HashSet<ProtoId<AccessLevelPrototype>>) null : new HashSet<ProtoId<AccessLevelPrototype>>((IEnumerable<ProtoId<AccessLevelPrototype>>) current.HiddenAccessList);
      component.JobGroups = current.JobGroups == null ? (HashSet<ProtoId<AccessGroupPrototype>>) null : new HashSet<ProtoId<AccessGroupPrototype>>((IEnumerable<ProtoId<AccessGroupPrototype>>) current.JobGroups);
      component.JobList = current.JobList == null ? (HashSet<ProtoId<AccessGroupPrototype>>) null : new HashSet<ProtoId<AccessGroupPrototype>>((IEnumerable<ProtoId<AccessGroupPrototype>>) current.JobList);
      component.PrivilegedIdSlot = current.PrivilegedIdSlot;
      component.TargetIdSlot = current.TargetIdSlot;
      AfterAutoHandleStateEvent args1 = new AfterAutoHandleStateEvent(args.Current);
      this.EntityManager.EventBus.RaiseComponentEvent<AfterAutoHandleStateEvent, IdModificationConsoleComponent>(uid, component, ref args1);
    }
  }
}
