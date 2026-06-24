// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Weapons.Ranged.PumpActionComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Localization;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Weapons.Ranged;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (SharedPumpActionSystem)})]
public sealed class PumpActionComponent : 
  Component,
  ISerializationGenerated<PumpActionComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool Pumped;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SoundSpecifier Sound = (SoundSpecifier) new SoundCollectionSpecifier("CMShotgunPump");
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public LocId Examine = (LocId) "cm-gun-pump-examine";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public LocId Popup = (LocId) "cm-gun-pump-first";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public LocId PopupKey = (LocId) "cm-gun-pump-first-with";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool Once;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public string ContainerId = "gun_magazine";

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref PumpActionComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (PumpActionComponent) target1;
    if (serialization.TryCustomCopy<PumpActionComponent>(this, ref target, hookCtx, false, context))
      return;
    bool target2 = false;
    if (!serialization.TryCustomCopy<bool>(this.Pumped, ref target2, hookCtx, false, context))
      target2 = this.Pumped;
    target.Pumped = target2;
    SoundSpecifier target3 = (SoundSpecifier) null;
    if (this.Sound == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.Sound, ref target3, hookCtx, true, context))
      target3 = serialization.CreateCopy<SoundSpecifier>(this.Sound, hookCtx, context);
    target.Sound = target3;
    LocId target4 = new LocId();
    if (!serialization.TryCustomCopy<LocId>(this.Examine, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<LocId>(this.Examine, hookCtx, context);
    target.Examine = target4;
    LocId target5 = new LocId();
    if (!serialization.TryCustomCopy<LocId>(this.Popup, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<LocId>(this.Popup, hookCtx, context);
    target.Popup = target5;
    LocId target6 = new LocId();
    if (!serialization.TryCustomCopy<LocId>(this.PopupKey, ref target6, hookCtx, false, context))
      target6 = serialization.CreateCopy<LocId>(this.PopupKey, hookCtx, context);
    target.PopupKey = target6;
    bool target7 = false;
    if (!serialization.TryCustomCopy<bool>(this.Once, ref target7, hookCtx, false, context))
      target7 = this.Once;
    target.Once = target7;
    string target8 = (string) null;
    if (this.ContainerId == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.ContainerId, ref target8, hookCtx, false, context))
      target8 = this.ContainerId;
    target.ContainerId = target8;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref PumpActionComponent target,
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
    PumpActionComponent target1 = (PumpActionComponent) target;
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
    PumpActionComponent target1 = (PumpActionComponent) target;
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
    PumpActionComponent target1 = (PumpActionComponent) target;
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
  virtual PumpActionComponent Component.Instantiate() => new PumpActionComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class PumpActionComponent_AutoState : IComponentState
  {
    public bool Pumped;
    public SoundSpecifier Sound;
    public LocId Examine;
    public LocId Popup;
    public LocId PopupKey;
    public bool Once;
    public string ContainerId;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class PumpActionComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<PumpActionComponent, ComponentGetState>(new ComponentEventRefHandler<PumpActionComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<PumpActionComponent, ComponentHandleState>(new ComponentEventRefHandler<PumpActionComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      PumpActionComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new PumpActionComponent.PumpActionComponent_AutoState()
      {
        Pumped = component.Pumped,
        Sound = component.Sound,
        Examine = component.Examine,
        Popup = component.Popup,
        PopupKey = component.PopupKey,
        Once = component.Once,
        ContainerId = component.ContainerId
      };
    }

    private void OnHandleState(
      EntityUid uid,
      PumpActionComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is PumpActionComponent.PumpActionComponent_AutoState current))
        return;
      component.Pumped = current.Pumped;
      component.Sound = current.Sound;
      component.Examine = current.Examine;
      component.Popup = current.Popup;
      component.PopupKey = current.PopupKey;
      component.Once = current.Once;
      component.ContainerId = current.ContainerId;
    }
  }
}
