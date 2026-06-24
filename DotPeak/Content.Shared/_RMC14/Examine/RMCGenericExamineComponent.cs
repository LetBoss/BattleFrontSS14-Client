// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Examine.RMCGenericExamineComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Marines.Skills;
using Content.Shared.Whitelist;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Localization;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Examine;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (CMExamineSystem)})]
public sealed class RMCGenericExamineComponent : 
  Component,
  ISerializationGenerated<RMCGenericExamineComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, true, false, null)]
  [AutoNetworkedField]
  public LocId MessageId;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int ExaminePriority;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SkillWhitelist? SkillsRequired;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntityWhitelist? Blacklist;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntityWhitelist? Whitelist;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref RMCGenericExamineComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (RMCGenericExamineComponent) target1;
    if (serialization.TryCustomCopy<RMCGenericExamineComponent>(this, ref target, hookCtx, false, context))
      return;
    LocId target2 = new LocId();
    if (!serialization.TryCustomCopy<LocId>(this.MessageId, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<LocId>(this.MessageId, hookCtx, context);
    target.MessageId = target2;
    int target3 = 0;
    if (!serialization.TryCustomCopy<int>(this.ExaminePriority, ref target3, hookCtx, false, context))
      target3 = this.ExaminePriority;
    target.ExaminePriority = target3;
    SkillWhitelist target4 = (SkillWhitelist) null;
    if (!serialization.TryCustomCopy<SkillWhitelist>(this.SkillsRequired, ref target4, hookCtx, false, context))
    {
      if (this.SkillsRequired == null)
        target4 = (SkillWhitelist) null;
      else
        serialization.CopyTo<SkillWhitelist>(this.SkillsRequired, ref target4, hookCtx, context);
    }
    target.SkillsRequired = target4;
    EntityWhitelist target5 = (EntityWhitelist) null;
    if (!serialization.TryCustomCopy<EntityWhitelist>(this.Blacklist, ref target5, hookCtx, false, context))
    {
      if (this.Blacklist == null)
        target5 = (EntityWhitelist) null;
      else
        serialization.CopyTo<EntityWhitelist>(this.Blacklist, ref target5, hookCtx, context);
    }
    target.Blacklist = target5;
    EntityWhitelist target6 = (EntityWhitelist) null;
    if (!serialization.TryCustomCopy<EntityWhitelist>(this.Whitelist, ref target6, hookCtx, false, context))
    {
      if (this.Whitelist == null)
        target6 = (EntityWhitelist) null;
      else
        serialization.CopyTo<EntityWhitelist>(this.Whitelist, ref target6, hookCtx, context);
    }
    target.Whitelist = target6;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref RMCGenericExamineComponent target,
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
    RMCGenericExamineComponent target1 = (RMCGenericExamineComponent) target;
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
    RMCGenericExamineComponent target1 = (RMCGenericExamineComponent) target;
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
    RMCGenericExamineComponent target1 = (RMCGenericExamineComponent) target;
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
  virtual RMCGenericExamineComponent Component.Instantiate() => new RMCGenericExamineComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class RMCGenericExamineComponent_AutoState : IComponentState
  {
    public LocId MessageId;
    public int ExaminePriority;
    public SkillWhitelist? SkillsRequired;
    public EntityWhitelist? Blacklist;
    public EntityWhitelist? Whitelist;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class RMCGenericExamineComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<RMCGenericExamineComponent, ComponentGetState>(new ComponentEventRefHandler<RMCGenericExamineComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<RMCGenericExamineComponent, ComponentHandleState>(new ComponentEventRefHandler<RMCGenericExamineComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      RMCGenericExamineComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new RMCGenericExamineComponent.RMCGenericExamineComponent_AutoState()
      {
        MessageId = component.MessageId,
        ExaminePriority = component.ExaminePriority,
        SkillsRequired = component.SkillsRequired,
        Blacklist = component.Blacklist,
        Whitelist = component.Whitelist
      };
    }

    private void OnHandleState(
      EntityUid uid,
      RMCGenericExamineComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is RMCGenericExamineComponent.RMCGenericExamineComponent_AutoState current))
        return;
      component.MessageId = current.MessageId;
      component.ExaminePriority = current.ExaminePriority;
      component.SkillsRequired = current.SkillsRequired;
      component.Blacklist = current.Blacklist;
      component.Whitelist = current.Whitelist;
    }
  }
}
