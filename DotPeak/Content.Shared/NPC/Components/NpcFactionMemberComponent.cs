// Decompiled with JetBrains decompiler
// Type: Content.Shared.NPC.Components.NpcFactionMemberComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.NPC.Prototypes;
using Content.Shared.NPC.Systems;
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
namespace Content.Shared.NPC.Components;

[RegisterComponent]
[NetworkedComponent]
[Access(new Type[] {typeof (NpcFactionSystem)})]
[AutoGenerateComponentState(false, false)]
public sealed class NpcFactionMemberComponent : 
  Component,
  ISerializationGenerated<NpcFactionMemberComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public HashSet<ProtoId<NpcFactionPrototype>> Factions = new HashSet<ProtoId<NpcFactionPrototype>>();
  [Robust.Shared.ViewVariables.ViewVariables]
  [AutoNetworkedField]
  public HashSet<ProtoId<NpcFactionPrototype>> FriendlyFactions = new HashSet<ProtoId<NpcFactionPrototype>>();
  [Robust.Shared.ViewVariables.ViewVariables]
  [AutoNetworkedField]
  public HashSet<ProtoId<NpcFactionPrototype>> HostileFactions = new HashSet<ProtoId<NpcFactionPrototype>>();
  [DataField(null, false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables]
  [AutoNetworkedField]
  public HashSet<ProtoId<NpcFactionPrototype>>? AddFriendlyFactions;
  [DataField(null, false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables]
  [AutoNetworkedField]
  public HashSet<ProtoId<NpcFactionPrototype>>? AddHostileFactions;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref NpcFactionMemberComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (NpcFactionMemberComponent) target1;
    if (serialization.TryCustomCopy<NpcFactionMemberComponent>(this, ref target, hookCtx, false, context))
      return;
    HashSet<ProtoId<NpcFactionPrototype>> target2 = (HashSet<ProtoId<NpcFactionPrototype>>) null;
    if (this.Factions == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<HashSet<ProtoId<NpcFactionPrototype>>>(this.Factions, ref target2, hookCtx, true, context))
      target2 = serialization.CreateCopy<HashSet<ProtoId<NpcFactionPrototype>>>(this.Factions, hookCtx, context);
    target.Factions = target2;
    HashSet<ProtoId<NpcFactionPrototype>> target3 = (HashSet<ProtoId<NpcFactionPrototype>>) null;
    if (!serialization.TryCustomCopy<HashSet<ProtoId<NpcFactionPrototype>>>(this.AddFriendlyFactions, ref target3, hookCtx, true, context))
      target3 = serialization.CreateCopy<HashSet<ProtoId<NpcFactionPrototype>>>(this.AddFriendlyFactions, hookCtx, context);
    target.AddFriendlyFactions = target3;
    HashSet<ProtoId<NpcFactionPrototype>> target4 = (HashSet<ProtoId<NpcFactionPrototype>>) null;
    if (!serialization.TryCustomCopy<HashSet<ProtoId<NpcFactionPrototype>>>(this.AddHostileFactions, ref target4, hookCtx, true, context))
      target4 = serialization.CreateCopy<HashSet<ProtoId<NpcFactionPrototype>>>(this.AddHostileFactions, hookCtx, context);
    target.AddHostileFactions = target4;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref NpcFactionMemberComponent target,
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
    NpcFactionMemberComponent target1 = (NpcFactionMemberComponent) target;
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
    NpcFactionMemberComponent target1 = (NpcFactionMemberComponent) target;
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
    NpcFactionMemberComponent target1 = (NpcFactionMemberComponent) target;
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
  virtual NpcFactionMemberComponent Component.Instantiate() => new NpcFactionMemberComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class NpcFactionMemberComponent_AutoState : IComponentState
  {
    public HashSet<ProtoId<NpcFactionPrototype>> Factions;
    public HashSet<ProtoId<NpcFactionPrototype>> FriendlyFactions;
    public HashSet<ProtoId<NpcFactionPrototype>> HostileFactions;
    public HashSet<ProtoId<NpcFactionPrototype>>? AddFriendlyFactions;
    public HashSet<ProtoId<NpcFactionPrototype>>? AddHostileFactions;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class NpcFactionMemberComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<NpcFactionMemberComponent, ComponentGetState>(new ComponentEventRefHandler<NpcFactionMemberComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<NpcFactionMemberComponent, ComponentHandleState>(new ComponentEventRefHandler<NpcFactionMemberComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      NpcFactionMemberComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new NpcFactionMemberComponent.NpcFactionMemberComponent_AutoState()
      {
        Factions = component.Factions,
        FriendlyFactions = component.FriendlyFactions,
        HostileFactions = component.HostileFactions,
        AddFriendlyFactions = component.AddFriendlyFactions,
        AddHostileFactions = component.AddHostileFactions
      };
    }

    private void OnHandleState(
      EntityUid uid,
      NpcFactionMemberComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is NpcFactionMemberComponent.NpcFactionMemberComponent_AutoState current))
        return;
      component.Factions = current.Factions == null ? (HashSet<ProtoId<NpcFactionPrototype>>) null : new HashSet<ProtoId<NpcFactionPrototype>>((IEnumerable<ProtoId<NpcFactionPrototype>>) current.Factions);
      component.FriendlyFactions = current.FriendlyFactions == null ? (HashSet<ProtoId<NpcFactionPrototype>>) null : new HashSet<ProtoId<NpcFactionPrototype>>((IEnumerable<ProtoId<NpcFactionPrototype>>) current.FriendlyFactions);
      component.HostileFactions = current.HostileFactions == null ? (HashSet<ProtoId<NpcFactionPrototype>>) null : new HashSet<ProtoId<NpcFactionPrototype>>((IEnumerable<ProtoId<NpcFactionPrototype>>) current.HostileFactions);
      component.AddFriendlyFactions = current.AddFriendlyFactions == null ? (HashSet<ProtoId<NpcFactionPrototype>>) null : new HashSet<ProtoId<NpcFactionPrototype>>((IEnumerable<ProtoId<NpcFactionPrototype>>) current.AddFriendlyFactions);
      component.AddHostileFactions = current.AddHostileFactions == null ? (HashSet<ProtoId<NpcFactionPrototype>>) null : new HashSet<ProtoId<NpcFactionPrototype>>((IEnumerable<ProtoId<NpcFactionPrototype>>) current.AddHostileFactions);
    }
  }
}
