// Decompiled with JetBrains decompiler
// Type: Content.Shared.Tools.Components.ToolComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Marines.Skills;
using Content.Shared.Tools.Systems;
using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Utility;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Tools.Components;

[RegisterComponent]
[NetworkedComponent]
[Access(new Type[] {typeof (SharedToolSystem)})]
public sealed class ToolComponent : 
  Component,
  ISerializationGenerated<ToolComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public EntProtoId<SkillDefinitionComponent> Skill = (EntProtoId<SkillDefinitionComponent>) "RMCSkillEngineer";
  [DataField(null, false, 1, false, false, null)]
  public PrototypeFlags<ToolQualityPrototype> Qualities = new PrototypeFlags<ToolQualityPrototype>();
  [DataField(null, false, 1, false, false, null)]
  public float SpeedModifier = 1f;
  [DataField(null, false, 1, false, false, null)]
  public SoundSpecifier? UseSound;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref ToolComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (ToolComponent) target1;
    if (serialization.TryCustomCopy<ToolComponent>(this, ref target, hookCtx, false, context))
      return;
    EntProtoId<SkillDefinitionComponent> target2 = new EntProtoId<SkillDefinitionComponent>();
    if (!serialization.TryCustomCopy<EntProtoId<SkillDefinitionComponent>>(this.Skill, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<EntProtoId<SkillDefinitionComponent>>(this.Skill, hookCtx, context);
    target.Skill = target2;
    PrototypeFlags<ToolQualityPrototype> target3 = (PrototypeFlags<ToolQualityPrototype>) null;
    if (this.Qualities == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<PrototypeFlags<ToolQualityPrototype>>(this.Qualities, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<PrototypeFlags<ToolQualityPrototype>>(this.Qualities, hookCtx, context);
    target.Qualities = target3;
    float target4 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.SpeedModifier, ref target4, hookCtx, false, context))
      target4 = this.SpeedModifier;
    target.SpeedModifier = target4;
    SoundSpecifier target5 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.UseSound, ref target5, hookCtx, true, context))
      target5 = serialization.CreateCopy<SoundSpecifier>(this.UseSound, hookCtx, context);
    target.UseSound = target5;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref ToolComponent target,
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
    ToolComponent target1 = (ToolComponent) target;
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
    ToolComponent target1 = (ToolComponent) target;
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
    ToolComponent target1 = (ToolComponent) target;
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
  virtual ToolComponent Component.Instantiate() => new ToolComponent();
}
