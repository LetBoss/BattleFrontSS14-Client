// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Medical.Defibrillator.RMCDefibrillatorBlockedComponent
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
namespace Content.Shared._RMC14.Medical.Defibrillator;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (RMCDefibrillatorSystem)})]
public sealed class RMCDefibrillatorBlockedComponent : 
  Component,
  ISerializationGenerated<RMCDefibrillatorBlockedComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public LocId Popup = (LocId) "rmc-defibrillator-unrevivable";
  [DataField(null, false, 1, false, false, null)]
  public LocId Examine = (LocId) "rmc-defibrillator-unrevivable";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool ShowOnExamine;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref RMCDefibrillatorBlockedComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (RMCDefibrillatorBlockedComponent) target1;
    if (serialization.TryCustomCopy<RMCDefibrillatorBlockedComponent>(this, ref target, hookCtx, false, context))
      return;
    LocId target2 = new LocId();
    if (!serialization.TryCustomCopy<LocId>(this.Popup, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<LocId>(this.Popup, hookCtx, context);
    target.Popup = target2;
    LocId target3 = new LocId();
    if (!serialization.TryCustomCopy<LocId>(this.Examine, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<LocId>(this.Examine, hookCtx, context);
    target.Examine = target3;
    bool target4 = false;
    if (!serialization.TryCustomCopy<bool>(this.ShowOnExamine, ref target4, hookCtx, false, context))
      target4 = this.ShowOnExamine;
    target.ShowOnExamine = target4;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref RMCDefibrillatorBlockedComponent target,
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
    RMCDefibrillatorBlockedComponent target1 = (RMCDefibrillatorBlockedComponent) target;
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
    RMCDefibrillatorBlockedComponent target1 = (RMCDefibrillatorBlockedComponent) target;
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
    RMCDefibrillatorBlockedComponent target1 = (RMCDefibrillatorBlockedComponent) target;
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
  virtual RMCDefibrillatorBlockedComponent Component.Instantiate()
  {
    return new RMCDefibrillatorBlockedComponent();
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class RMCDefibrillatorBlockedComponent_AutoState : IComponentState
  {
    public bool ShowOnExamine;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class RMCDefibrillatorBlockedComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<RMCDefibrillatorBlockedComponent, ComponentGetState>(new ComponentEventRefHandler<RMCDefibrillatorBlockedComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<RMCDefibrillatorBlockedComponent, ComponentHandleState>(new ComponentEventRefHandler<RMCDefibrillatorBlockedComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      RMCDefibrillatorBlockedComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new RMCDefibrillatorBlockedComponent.RMCDefibrillatorBlockedComponent_AutoState()
      {
        ShowOnExamine = component.ShowOnExamine
      };
    }

    private void OnHandleState(
      EntityUid uid,
      RMCDefibrillatorBlockedComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is RMCDefibrillatorBlockedComponent.RMCDefibrillatorBlockedComponent_AutoState current))
        return;
      component.ShowOnExamine = current.ShowOnExamine;
    }
  }
}
