// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Marines.Skills.ItemToggleDeactivateUnskilledComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Localization;
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
namespace Content.Shared._RMC14.Marines.Skills;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (SkillsSystem)})]
public sealed class ItemToggleDeactivateUnskilledComponent : 
  Component,
  ISerializationGenerated<ItemToggleDeactivateUnskilledComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, true, false, null)]
  [AutoNetworkedField]
  public Dictionary<EntProtoId<SkillDefinitionComponent>, int> Skills = new Dictionary<EntProtoId<SkillDefinitionComponent>, int>();
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public LocId? Popup;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref ItemToggleDeactivateUnskilledComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (ItemToggleDeactivateUnskilledComponent) target1;
    if (serialization.TryCustomCopy<ItemToggleDeactivateUnskilledComponent>(this, ref target, hookCtx, false, context))
      return;
    Dictionary<EntProtoId<SkillDefinitionComponent>, int> target2 = (Dictionary<EntProtoId<SkillDefinitionComponent>, int>) null;
    if (this.Skills == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<Dictionary<EntProtoId<SkillDefinitionComponent>, int>>(this.Skills, ref target2, hookCtx, true, context))
      target2 = serialization.CreateCopy<Dictionary<EntProtoId<SkillDefinitionComponent>, int>>(this.Skills, hookCtx, context);
    target.Skills = target2;
    LocId? target3 = new LocId?();
    if (!serialization.TryCustomCopy<LocId?>(this.Popup, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<LocId?>(this.Popup, hookCtx, context);
    target.Popup = target3;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref ItemToggleDeactivateUnskilledComponent target,
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
    ItemToggleDeactivateUnskilledComponent target1 = (ItemToggleDeactivateUnskilledComponent) target;
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
    ItemToggleDeactivateUnskilledComponent target1 = (ItemToggleDeactivateUnskilledComponent) target;
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
    ItemToggleDeactivateUnskilledComponent target1 = (ItemToggleDeactivateUnskilledComponent) target;
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
  virtual ItemToggleDeactivateUnskilledComponent Component.Instantiate()
  {
    return new ItemToggleDeactivateUnskilledComponent();
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class ItemToggleDeactivateUnskilledComponent_AutoState : IComponentState
  {
    public Dictionary<EntProtoId<SkillDefinitionComponent>, int> Skills;
    public LocId? Popup;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class ItemToggleDeactivateUnskilledComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<ItemToggleDeactivateUnskilledComponent, ComponentGetState>(new ComponentEventRefHandler<ItemToggleDeactivateUnskilledComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<ItemToggleDeactivateUnskilledComponent, ComponentHandleState>(new ComponentEventRefHandler<ItemToggleDeactivateUnskilledComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      ItemToggleDeactivateUnskilledComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new ItemToggleDeactivateUnskilledComponent.ItemToggleDeactivateUnskilledComponent_AutoState()
      {
        Skills = component.Skills,
        Popup = component.Popup
      };
    }

    private void OnHandleState(
      EntityUid uid,
      ItemToggleDeactivateUnskilledComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is ItemToggleDeactivateUnskilledComponent.ItemToggleDeactivateUnskilledComponent_AutoState current))
        return;
      component.Skills = current.Skills == null ? (Dictionary<EntProtoId<SkillDefinitionComponent>, int>) null : new Dictionary<EntProtoId<SkillDefinitionComponent>, int>((IDictionary<EntProtoId<SkillDefinitionComponent>, int>) current.Skills);
      component.Popup = current.Popup;
    }
  }
}
