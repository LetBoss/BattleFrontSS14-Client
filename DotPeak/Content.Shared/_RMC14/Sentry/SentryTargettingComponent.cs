// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Sentry.SentryTargetingComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Sentry;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (SharedSentryTargetingSystem)})]
public sealed class SentryTargetingComponent : 
  Component,
  ISerializationGenerated<SentryTargetingComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public HashSet<string> FriendlyFactions = new HashSet<string>();
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public HashSet<string> DeployedFriendlyFactions = new HashSet<string>();
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public string OriginalFaction = "UNMC";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public HashSet<string> TargetedFactions = new HashSet<string>();
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public HashSet<string> HumanoidAdded = new HashSet<string>();

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref SentryTargetingComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (SentryTargetingComponent) target1;
    if (serialization.TryCustomCopy<SentryTargetingComponent>(this, ref target, hookCtx, false, context))
      return;
    HashSet<string> target2 = (HashSet<string>) null;
    if (this.FriendlyFactions == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<HashSet<string>>(this.FriendlyFactions, ref target2, hookCtx, true, context))
      target2 = serialization.CreateCopy<HashSet<string>>(this.FriendlyFactions, hookCtx, context);
    target.FriendlyFactions = target2;
    HashSet<string> target3 = (HashSet<string>) null;
    if (this.DeployedFriendlyFactions == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<HashSet<string>>(this.DeployedFriendlyFactions, ref target3, hookCtx, true, context))
      target3 = serialization.CreateCopy<HashSet<string>>(this.DeployedFriendlyFactions, hookCtx, context);
    target.DeployedFriendlyFactions = target3;
    string target4 = (string) null;
    if (this.OriginalFaction == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.OriginalFaction, ref target4, hookCtx, false, context))
      target4 = this.OriginalFaction;
    target.OriginalFaction = target4;
    HashSet<string> target5 = (HashSet<string>) null;
    if (this.TargetedFactions == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<HashSet<string>>(this.TargetedFactions, ref target5, hookCtx, true, context))
      target5 = serialization.CreateCopy<HashSet<string>>(this.TargetedFactions, hookCtx, context);
    target.TargetedFactions = target5;
    HashSet<string> target6 = (HashSet<string>) null;
    if (this.HumanoidAdded == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<HashSet<string>>(this.HumanoidAdded, ref target6, hookCtx, true, context))
      target6 = serialization.CreateCopy<HashSet<string>>(this.HumanoidAdded, hookCtx, context);
    target.HumanoidAdded = target6;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref SentryTargetingComponent target,
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
    SentryTargetingComponent target1 = (SentryTargetingComponent) target;
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
    SentryTargetingComponent target1 = (SentryTargetingComponent) target;
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
    SentryTargetingComponent target1 = (SentryTargetingComponent) target;
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
  virtual SentryTargetingComponent Component.Instantiate() => new SentryTargetingComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class SentryTargetingComponent_AutoState : IComponentState
  {
    public HashSet<string> FriendlyFactions;
    public HashSet<string> DeployedFriendlyFactions;
    public string OriginalFaction;
    public HashSet<string> TargetedFactions;
    public HashSet<string> HumanoidAdded;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class SentryTargetingComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<SentryTargetingComponent, ComponentGetState>(new ComponentEventRefHandler<SentryTargetingComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<SentryTargetingComponent, ComponentHandleState>(new ComponentEventRefHandler<SentryTargetingComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      SentryTargetingComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new SentryTargetingComponent.SentryTargetingComponent_AutoState()
      {
        FriendlyFactions = component.FriendlyFactions,
        DeployedFriendlyFactions = component.DeployedFriendlyFactions,
        OriginalFaction = component.OriginalFaction,
        TargetedFactions = component.TargetedFactions,
        HumanoidAdded = component.HumanoidAdded
      };
    }

    private void OnHandleState(
      EntityUid uid,
      SentryTargetingComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is SentryTargetingComponent.SentryTargetingComponent_AutoState current))
        return;
      component.FriendlyFactions = current.FriendlyFactions == null ? (HashSet<string>) null : new HashSet<string>((IEnumerable<string>) current.FriendlyFactions);
      component.DeployedFriendlyFactions = current.DeployedFriendlyFactions == null ? (HashSet<string>) null : new HashSet<string>((IEnumerable<string>) current.DeployedFriendlyFactions);
      component.OriginalFaction = current.OriginalFaction;
      component.TargetedFactions = current.TargetedFactions == null ? (HashSet<string>) null : new HashSet<string>((IEnumerable<string>) current.TargetedFactions);
      component.HumanoidAdded = current.HumanoidAdded == null ? (HashSet<string>) null : new HashSet<string>((IEnumerable<string>) current.HumanoidAdded);
    }
  }
}
