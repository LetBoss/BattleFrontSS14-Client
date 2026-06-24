// Decompiled with JetBrains decompiler
// Type: Content.Shared.Armable.ArmableComponent
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
namespace Content.Shared.Armable;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (ArmableSystem)})]
public sealed class ArmableComponent : 
  Component,
  ISerializationGenerated<ArmableComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool ShowStatusOnExamination = true;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool ChangeAppearance = true;
  [DataField(null, false, 1, false, false, null)]
  public LocId? ExamineTextArmed = LocId.op_Implicit("armable-examine-armed");
  [DataField(null, false, 1, false, false, null)]
  public LocId? ExamineTextNotArmed = LocId.op_Implicit("armable-examine-not-armed");

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref ArmableComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component component = (Component) target;
    this.InternalCopy(ref component, serialization, hookCtx, context);
    target = (ArmableComponent) component;
    if (serialization.TryCustomCopy<ArmableComponent>(this, ref target, hookCtx, false, context))
      return;
    bool flag1 = false;
    if (!serialization.TryCustomCopy<bool>(this.ShowStatusOnExamination, ref flag1, hookCtx, false, context))
      flag1 = this.ShowStatusOnExamination;
    target.ShowStatusOnExamination = flag1;
    bool flag2 = false;
    if (!serialization.TryCustomCopy<bool>(this.ChangeAppearance, ref flag2, hookCtx, false, context))
      flag2 = this.ChangeAppearance;
    target.ChangeAppearance = flag2;
    LocId? nullable1 = new LocId?();
    if (!serialization.TryCustomCopy<LocId?>(this.ExamineTextArmed, ref nullable1, hookCtx, false, context))
      nullable1 = serialization.CreateCopy<LocId?>(this.ExamineTextArmed, hookCtx, context, false);
    target.ExamineTextArmed = nullable1;
    LocId? nullable2 = new LocId?();
    if (!serialization.TryCustomCopy<LocId?>(this.ExamineTextNotArmed, ref nullable2, hookCtx, false, context))
      nullable2 = serialization.CreateCopy<LocId?>(this.ExamineTextNotArmed, hookCtx, context, false);
    target.ExamineTextNotArmed = nullable2;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref ArmableComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void Copy(
    ref Component target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    ArmableComponent target1 = (ArmableComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (Component) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    ArmableComponent target1 = (ArmableComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void InternalCopy(
    ref IComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    ArmableComponent target1 = (ArmableComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (IComponent) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void Copy(
    ref IComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    base.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual ArmableComponent Component.Instantiate() => new ArmableComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class ArmableComponent_AutoState : IComponentState
  {
    public bool ShowStatusOnExamination;
    public bool ChangeAppearance;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class ArmableComponent_AutoNetworkSystem : EntitySystem
  {
    public virtual void Initialize()
    {
      // ISSUE: method pointer
      this.SubscribeLocalEvent<ArmableComponent, ComponentGetState>(new ComponentEventRefHandler<ArmableComponent, ComponentGetState>((object) this, __methodptr(OnGetState)), (Type[]) null, (Type[]) null);
      // ISSUE: method pointer
      this.SubscribeLocalEvent<ArmableComponent, ComponentHandleState>(new ComponentEventRefHandler<ArmableComponent, ComponentHandleState>((object) this, __methodptr(OnHandleState)), (Type[]) null, (Type[]) null);
    }

    private void OnGetState(EntityUid uid, ArmableComponent component, ref ComponentGetState args)
    {
      ((ComponentGetState) ref args).State = (IComponentState) new ArmableComponent.ArmableComponent_AutoState()
      {
        ShowStatusOnExamination = component.ShowStatusOnExamination,
        ChangeAppearance = component.ChangeAppearance
      };
    }

    private void OnHandleState(
      EntityUid uid,
      ArmableComponent component,
      ref ComponentHandleState args)
    {
      if (!(((ComponentHandleState) ref args).Current is ArmableComponent.ArmableComponent_AutoState current))
        return;
      component.ShowStatusOnExamination = current.ShowStatusOnExamination;
      component.ChangeAppearance = current.ChangeAppearance;
    }
  }
}
