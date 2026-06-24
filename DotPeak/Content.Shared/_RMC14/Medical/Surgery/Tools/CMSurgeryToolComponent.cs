// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Medical.Surgery.Tools.CMSurgeryToolComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Marines.Skills;
using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
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
namespace Content.Shared._RMC14.Medical.Surgery.Tools;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (SharedCMSurgerySystem)})]
public sealed class CMSurgeryToolComponent : 
  Component,
  ISerializationGenerated<CMSurgeryToolComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntProtoId<SkillDefinitionComponent> SkillType = (EntProtoId<SkillDefinitionComponent>) "RMCSkillSurgery";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int Skill = 1;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SoundSpecifier? StartSound;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SoundSpecifier? EndSound;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref CMSurgeryToolComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (CMSurgeryToolComponent) target1;
    if (serialization.TryCustomCopy<CMSurgeryToolComponent>(this, ref target, hookCtx, false, context))
      return;
    EntProtoId<SkillDefinitionComponent> target2 = new EntProtoId<SkillDefinitionComponent>();
    if (!serialization.TryCustomCopy<EntProtoId<SkillDefinitionComponent>>(this.SkillType, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<EntProtoId<SkillDefinitionComponent>>(this.SkillType, hookCtx, context);
    target.SkillType = target2;
    int target3 = 0;
    if (!serialization.TryCustomCopy<int>(this.Skill, ref target3, hookCtx, false, context))
      target3 = this.Skill;
    target.Skill = target3;
    SoundSpecifier target4 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.StartSound, ref target4, hookCtx, true, context))
      target4 = serialization.CreateCopy<SoundSpecifier>(this.StartSound, hookCtx, context);
    target.StartSound = target4;
    SoundSpecifier target5 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.EndSound, ref target5, hookCtx, true, context))
      target5 = serialization.CreateCopy<SoundSpecifier>(this.EndSound, hookCtx, context);
    target.EndSound = target5;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref CMSurgeryToolComponent target,
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
    CMSurgeryToolComponent target1 = (CMSurgeryToolComponent) target;
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
    CMSurgeryToolComponent target1 = (CMSurgeryToolComponent) target;
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
    CMSurgeryToolComponent target1 = (CMSurgeryToolComponent) target;
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
  virtual CMSurgeryToolComponent Component.Instantiate() => new CMSurgeryToolComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class CMSurgeryToolComponent_AutoState : IComponentState
  {
    public EntProtoId<SkillDefinitionComponent> SkillType;
    public int Skill;
    public SoundSpecifier? StartSound;
    public SoundSpecifier? EndSound;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class CMSurgeryToolComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<CMSurgeryToolComponent, ComponentGetState>(new ComponentEventRefHandler<CMSurgeryToolComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<CMSurgeryToolComponent, ComponentHandleState>(new ComponentEventRefHandler<CMSurgeryToolComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      CMSurgeryToolComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new CMSurgeryToolComponent.CMSurgeryToolComponent_AutoState()
      {
        SkillType = component.SkillType,
        Skill = component.Skill,
        StartSound = component.StartSound,
        EndSound = component.EndSound
      };
    }

    private void OnHandleState(
      EntityUid uid,
      CMSurgeryToolComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is CMSurgeryToolComponent.CMSurgeryToolComponent_AutoState current))
        return;
      component.SkillType = current.SkillType;
      component.Skill = current.Skill;
      component.StartSound = current.StartSound;
      component.EndSound = current.EndSound;
    }
  }
}
