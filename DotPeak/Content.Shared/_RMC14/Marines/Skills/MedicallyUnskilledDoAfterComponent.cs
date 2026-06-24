// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Marines.Skills.MedicallyUnskilledDoAfterComponent
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
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Marines.Skills;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (SkillsSystem)})]
public sealed class MedicallyUnskilledDoAfterComponent : 
  Component,
  ISerializationGenerated<MedicallyUnskilledDoAfterComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int Min = 1;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan DoAfter = TimeSpan.FromSeconds(3L);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntProtoId<SkillDefinitionComponent> Skill = (EntProtoId<SkillDefinitionComponent>) "RMCSkillMedical";

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref MedicallyUnskilledDoAfterComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (MedicallyUnskilledDoAfterComponent) target1;
    if (serialization.TryCustomCopy<MedicallyUnskilledDoAfterComponent>(this, ref target, hookCtx, false, context))
      return;
    int target2 = 0;
    if (!serialization.TryCustomCopy<int>(this.Min, ref target2, hookCtx, false, context))
      target2 = this.Min;
    target.Min = target2;
    TimeSpan target3 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.DoAfter, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<TimeSpan>(this.DoAfter, hookCtx, context);
    target.DoAfter = target3;
    EntProtoId<SkillDefinitionComponent> target4 = new EntProtoId<SkillDefinitionComponent>();
    if (!serialization.TryCustomCopy<EntProtoId<SkillDefinitionComponent>>(this.Skill, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<EntProtoId<SkillDefinitionComponent>>(this.Skill, hookCtx, context);
    target.Skill = target4;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref MedicallyUnskilledDoAfterComponent target,
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
    MedicallyUnskilledDoAfterComponent target1 = (MedicallyUnskilledDoAfterComponent) target;
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
    MedicallyUnskilledDoAfterComponent target1 = (MedicallyUnskilledDoAfterComponent) target;
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
    MedicallyUnskilledDoAfterComponent target1 = (MedicallyUnskilledDoAfterComponent) target;
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
  virtual MedicallyUnskilledDoAfterComponent Component.Instantiate()
  {
    return new MedicallyUnskilledDoAfterComponent();
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class MedicallyUnskilledDoAfterComponent_AutoState : IComponentState
  {
    public int Min;
    public TimeSpan DoAfter;
    public EntProtoId<SkillDefinitionComponent> Skill;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class MedicallyUnskilledDoAfterComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<MedicallyUnskilledDoAfterComponent, ComponentGetState>(new ComponentEventRefHandler<MedicallyUnskilledDoAfterComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<MedicallyUnskilledDoAfterComponent, ComponentHandleState>(new ComponentEventRefHandler<MedicallyUnskilledDoAfterComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      MedicallyUnskilledDoAfterComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new MedicallyUnskilledDoAfterComponent.MedicallyUnskilledDoAfterComponent_AutoState()
      {
        Min = component.Min,
        DoAfter = component.DoAfter,
        Skill = component.Skill
      };
    }

    private void OnHandleState(
      EntityUid uid,
      MedicallyUnskilledDoAfterComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is MedicallyUnskilledDoAfterComponent.MedicallyUnskilledDoAfterComponent_AutoState current))
        return;
      component.Min = current.Min;
      component.DoAfter = current.DoAfter;
      component.Skill = current.Skill;
    }
  }
}
