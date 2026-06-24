// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Crest.XenoCrestComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Stun;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Xenonids.Crest;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (XenoCrestSystem)})]
public sealed class XenoCrestComponent : 
  Component,
  ISerializationGenerated<XenoCrestComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool Lowered;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int Armor = 5;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float SpeedMultiplier = 0.7f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public string[] ImmuneToStatuses = new string[1]
  {
    "KnockedDown"
  };
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public RMCSizes CrestSize = RMCSizes.Big;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public RMCSizes? OriginalSize;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref XenoCrestComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (XenoCrestComponent) target1;
    if (serialization.TryCustomCopy<XenoCrestComponent>(this, ref target, hookCtx, false, context))
      return;
    bool target2 = false;
    if (!serialization.TryCustomCopy<bool>(this.Lowered, ref target2, hookCtx, false, context))
      target2 = this.Lowered;
    target.Lowered = target2;
    int target3 = 0;
    if (!serialization.TryCustomCopy<int>(this.Armor, ref target3, hookCtx, false, context))
      target3 = this.Armor;
    target.Armor = target3;
    float target4 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.SpeedMultiplier, ref target4, hookCtx, false, context))
      target4 = this.SpeedMultiplier;
    target.SpeedMultiplier = target4;
    string[] target5 = (string[]) null;
    if (this.ImmuneToStatuses == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string[]>(this.ImmuneToStatuses, ref target5, hookCtx, true, context))
      target5 = serialization.CreateCopy<string[]>(this.ImmuneToStatuses, hookCtx, context);
    target.ImmuneToStatuses = target5;
    RMCSizes target6 = RMCSizes.Small;
    if (!serialization.TryCustomCopy<RMCSizes>(this.CrestSize, ref target6, hookCtx, false, context))
      target6 = this.CrestSize;
    target.CrestSize = target6;
    RMCSizes? target7 = new RMCSizes?();
    if (!serialization.TryCustomCopy<RMCSizes?>(this.OriginalSize, ref target7, hookCtx, false, context))
      target7 = this.OriginalSize;
    target.OriginalSize = target7;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref XenoCrestComponent target,
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
    XenoCrestComponent target1 = (XenoCrestComponent) target;
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
    XenoCrestComponent target1 = (XenoCrestComponent) target;
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
    XenoCrestComponent target1 = (XenoCrestComponent) target;
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
  virtual XenoCrestComponent Component.Instantiate() => new XenoCrestComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class XenoCrestComponent_AutoState : IComponentState
  {
    public bool Lowered;
    public int Armor;
    public float SpeedMultiplier;
    public string[] ImmuneToStatuses;
    public RMCSizes CrestSize;
    public RMCSizes? OriginalSize;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class XenoCrestComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<XenoCrestComponent, ComponentGetState>(new ComponentEventRefHandler<XenoCrestComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<XenoCrestComponent, ComponentHandleState>(new ComponentEventRefHandler<XenoCrestComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      XenoCrestComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new XenoCrestComponent.XenoCrestComponent_AutoState()
      {
        Lowered = component.Lowered,
        Armor = component.Armor,
        SpeedMultiplier = component.SpeedMultiplier,
        ImmuneToStatuses = component.ImmuneToStatuses,
        CrestSize = component.CrestSize,
        OriginalSize = component.OriginalSize
      };
    }

    private void OnHandleState(
      EntityUid uid,
      XenoCrestComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is XenoCrestComponent.XenoCrestComponent_AutoState current))
        return;
      component.Lowered = current.Lowered;
      component.Armor = current.Armor;
      component.SpeedMultiplier = current.SpeedMultiplier;
      component.ImmuneToStatuses = current.ImmuneToStatuses;
      component.CrestSize = current.CrestSize;
      component.OriginalSize = current.OriginalSize;
    }
  }
}
