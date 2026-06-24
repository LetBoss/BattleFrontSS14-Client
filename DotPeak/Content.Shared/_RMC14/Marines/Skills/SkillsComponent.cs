// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Marines.Skills.SkillsComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

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
namespace Content.Shared._RMC14.Marines.Skills;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (SkillsSystem)})]
public sealed class SkillsComponent : 
  Component,
  ISerializationGenerated<SkillsComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public Dictionary<EntProtoId<SkillDefinitionComponent>, int> Skills = new Dictionary<EntProtoId<SkillDefinitionComponent>, int>();
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public ProtoId<SkillPresetPrototype>? Preset;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref SkillsComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (SkillsComponent) target1;
    if (serialization.TryCustomCopy<SkillsComponent>(this, ref target, hookCtx, false, context))
      return;
    Dictionary<EntProtoId<SkillDefinitionComponent>, int> target2 = (Dictionary<EntProtoId<SkillDefinitionComponent>, int>) null;
    if (this.Skills == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<Dictionary<EntProtoId<SkillDefinitionComponent>, int>>(this.Skills, ref target2, hookCtx, true, context))
      target2 = serialization.CreateCopy<Dictionary<EntProtoId<SkillDefinitionComponent>, int>>(this.Skills, hookCtx, context);
    target.Skills = target2;
    ProtoId<SkillPresetPrototype>? target3 = new ProtoId<SkillPresetPrototype>?();
    if (!serialization.TryCustomCopy<ProtoId<SkillPresetPrototype>?>(this.Preset, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<ProtoId<SkillPresetPrototype>?>(this.Preset, hookCtx, context);
    target.Preset = target3;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref SkillsComponent target,
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
    SkillsComponent target1 = (SkillsComponent) target;
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
    SkillsComponent target1 = (SkillsComponent) target;
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
    SkillsComponent target1 = (SkillsComponent) target;
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
  virtual SkillsComponent Component.Instantiate() => new SkillsComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class SkillsComponent_AutoState : IComponentState
  {
    public Dictionary<EntProtoId<SkillDefinitionComponent>, int> Skills;
    public ProtoId<SkillPresetPrototype>? Preset;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class SkillsComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<SkillsComponent, ComponentGetState>(new ComponentEventRefHandler<SkillsComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<SkillsComponent, ComponentHandleState>(new ComponentEventRefHandler<SkillsComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(EntityUid uid, SkillsComponent component, ref ComponentGetState args)
    {
      args.State = (IComponentState) new SkillsComponent.SkillsComponent_AutoState()
      {
        Skills = component.Skills,
        Preset = component.Preset
      };
    }

    private void OnHandleState(
      EntityUid uid,
      SkillsComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is SkillsComponent.SkillsComponent_AutoState current))
        return;
      component.Skills = current.Skills == null ? (Dictionary<EntProtoId<SkillDefinitionComponent>, int>) null : new Dictionary<EntProtoId<SkillDefinitionComponent>, int>((IDictionary<EntProtoId<SkillDefinitionComponent>, int>) current.Skills);
      component.Preset = current.Preset;
    }
  }
}
