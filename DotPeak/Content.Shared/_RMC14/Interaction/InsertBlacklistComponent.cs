// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Interaction.InsertBlacklistComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Mobs;
using Content.Shared.Whitelist;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Interaction;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (RMCInteractionSystem)})]
public sealed class InsertBlacklistComponent : 
  Component,
  ISerializationGenerated<InsertBlacklistComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntityWhitelist? Whitelist;
  [DataField(null, false, 1, false, false, null)]
  public EntityWhitelist? Blacklist;
  [DataField(null, false, 1, false, false, null)]
  public HashSet<MobState>? BlacklistedMobStates;
  [DataField(null, false, 1, false, false, null)]
  public HashSet<MobState>? WhitelistedMobStates;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref InsertBlacklistComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (InsertBlacklistComponent) target1;
    if (serialization.TryCustomCopy<InsertBlacklistComponent>(this, ref target, hookCtx, false, context))
      return;
    EntityWhitelist target2 = (EntityWhitelist) null;
    if (!serialization.TryCustomCopy<EntityWhitelist>(this.Whitelist, ref target2, hookCtx, false, context))
    {
      if (this.Whitelist == null)
        target2 = (EntityWhitelist) null;
      else
        serialization.CopyTo<EntityWhitelist>(this.Whitelist, ref target2, hookCtx, context);
    }
    target.Whitelist = target2;
    EntityWhitelist target3 = (EntityWhitelist) null;
    if (!serialization.TryCustomCopy<EntityWhitelist>(this.Blacklist, ref target3, hookCtx, false, context))
    {
      if (this.Blacklist == null)
        target3 = (EntityWhitelist) null;
      else
        serialization.CopyTo<EntityWhitelist>(this.Blacklist, ref target3, hookCtx, context);
    }
    target.Blacklist = target3;
    HashSet<MobState> target4 = (HashSet<MobState>) null;
    if (!serialization.TryCustomCopy<HashSet<MobState>>(this.BlacklistedMobStates, ref target4, hookCtx, true, context))
      target4 = serialization.CreateCopy<HashSet<MobState>>(this.BlacklistedMobStates, hookCtx, context);
    target.BlacklistedMobStates = target4;
    HashSet<MobState> target5 = (HashSet<MobState>) null;
    if (!serialization.TryCustomCopy<HashSet<MobState>>(this.WhitelistedMobStates, ref target5, hookCtx, true, context))
      target5 = serialization.CreateCopy<HashSet<MobState>>(this.WhitelistedMobStates, hookCtx, context);
    target.WhitelistedMobStates = target5;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref InsertBlacklistComponent target,
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
    InsertBlacklistComponent target1 = (InsertBlacklistComponent) target;
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
    InsertBlacklistComponent target1 = (InsertBlacklistComponent) target;
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
    InsertBlacklistComponent target1 = (InsertBlacklistComponent) target;
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
  virtual InsertBlacklistComponent Component.Instantiate() => new InsertBlacklistComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class InsertBlacklistComponent_AutoState : IComponentState
  {
    public EntityWhitelist? Whitelist;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class InsertBlacklistComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<InsertBlacklistComponent, ComponentGetState>(new ComponentEventRefHandler<InsertBlacklistComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<InsertBlacklistComponent, ComponentHandleState>(new ComponentEventRefHandler<InsertBlacklistComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      InsertBlacklistComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new InsertBlacklistComponent.InsertBlacklistComponent_AutoState()
      {
        Whitelist = component.Whitelist
      };
    }

    private void OnHandleState(
      EntityUid uid,
      InsertBlacklistComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is InsertBlacklistComponent.InsertBlacklistComponent_AutoState current))
        return;
      component.Whitelist = current.Whitelist;
    }
  }
}
