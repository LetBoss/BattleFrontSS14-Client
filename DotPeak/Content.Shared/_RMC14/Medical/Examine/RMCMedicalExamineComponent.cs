// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Medical.Examine.RMCMedicalExamineComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

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
namespace Content.Shared._RMC14.Medical.Examine;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (RMCMedicalExamineSystem)})]
public sealed class RMCMedicalExamineComponent : 
  Component,
  ISerializationGenerated<RMCMedicalExamineComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public LocId UnrevivableText = (LocId) "rmc-medical-examine-unrevivable";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public LocId AliveText = (LocId) "rmc-medical-examine-alive";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public LocId DeadText = (LocId) "rmc-medical-examine-dead";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public LocId CritText = (LocId) "rmc-medical-examine-unconscious";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public LocId BleedText = (LocId) "rmc-medical-examine-bleeding";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool Simple;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref RMCMedicalExamineComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (RMCMedicalExamineComponent) target1;
    if (serialization.TryCustomCopy<RMCMedicalExamineComponent>(this, ref target, hookCtx, false, context))
      return;
    LocId target2 = new LocId();
    if (!serialization.TryCustomCopy<LocId>(this.UnrevivableText, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<LocId>(this.UnrevivableText, hookCtx, context);
    target.UnrevivableText = target2;
    LocId target3 = new LocId();
    if (!serialization.TryCustomCopy<LocId>(this.AliveText, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<LocId>(this.AliveText, hookCtx, context);
    target.AliveText = target3;
    LocId target4 = new LocId();
    if (!serialization.TryCustomCopy<LocId>(this.DeadText, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<LocId>(this.DeadText, hookCtx, context);
    target.DeadText = target4;
    LocId target5 = new LocId();
    if (!serialization.TryCustomCopy<LocId>(this.CritText, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<LocId>(this.CritText, hookCtx, context);
    target.CritText = target5;
    LocId target6 = new LocId();
    if (!serialization.TryCustomCopy<LocId>(this.BleedText, ref target6, hookCtx, false, context))
      target6 = serialization.CreateCopy<LocId>(this.BleedText, hookCtx, context);
    target.BleedText = target6;
    bool target7 = false;
    if (!serialization.TryCustomCopy<bool>(this.Simple, ref target7, hookCtx, false, context))
      target7 = this.Simple;
    target.Simple = target7;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref RMCMedicalExamineComponent target,
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
    RMCMedicalExamineComponent target1 = (RMCMedicalExamineComponent) target;
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
    RMCMedicalExamineComponent target1 = (RMCMedicalExamineComponent) target;
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
    RMCMedicalExamineComponent target1 = (RMCMedicalExamineComponent) target;
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
  virtual RMCMedicalExamineComponent Component.Instantiate() => new RMCMedicalExamineComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class RMCMedicalExamineComponent_AutoState : IComponentState
  {
    public LocId UnrevivableText;
    public LocId AliveText;
    public LocId DeadText;
    public LocId CritText;
    public LocId BleedText;
    public bool Simple;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class RMCMedicalExamineComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<RMCMedicalExamineComponent, ComponentGetState>(new ComponentEventRefHandler<RMCMedicalExamineComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<RMCMedicalExamineComponent, ComponentHandleState>(new ComponentEventRefHandler<RMCMedicalExamineComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      RMCMedicalExamineComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new RMCMedicalExamineComponent.RMCMedicalExamineComponent_AutoState()
      {
        UnrevivableText = component.UnrevivableText,
        AliveText = component.AliveText,
        DeadText = component.DeadText,
        CritText = component.CritText,
        BleedText = component.BleedText,
        Simple = component.Simple
      };
    }

    private void OnHandleState(
      EntityUid uid,
      RMCMedicalExamineComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is RMCMedicalExamineComponent.RMCMedicalExamineComponent_AutoState current))
        return;
      component.UnrevivableText = current.UnrevivableText;
      component.AliveText = current.AliveText;
      component.DeadText = current.DeadText;
      component.CritText = current.CritText;
      component.BleedText = current.BleedText;
      component.Simple = current.Simple;
    }
  }
}
