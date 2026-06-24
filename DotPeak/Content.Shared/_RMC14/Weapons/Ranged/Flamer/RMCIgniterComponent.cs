// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Weapons.Ranged.Flamer.RMCIgniterComponent
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
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Weapons.Ranged.Flamer;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (SharedRMCFlamerSystem)})]
public sealed class RMCIgniterComponent : 
  Component,
  ISerializationGenerated<RMCIgniterComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool Enabled;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool Locked;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SoundPathSpecifier? Sound = new SoundPathSpecifier("/Audio/_RMC14/Weapons/Handling/flamer_ignition.ogg");
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public LocId Popup = (LocId) "rmc-flamer-ignite-first";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public LocId PopupKey = (LocId) "rmc-flamer-ignite-first-with";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public LocId ExamineText = (LocId) "rmc-flamer-ignite-action-examine";

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref RMCIgniterComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (RMCIgniterComponent) target1;
    if (serialization.TryCustomCopy<RMCIgniterComponent>(this, ref target, hookCtx, false, context))
      return;
    bool target2 = false;
    if (!serialization.TryCustomCopy<bool>(this.Enabled, ref target2, hookCtx, false, context))
      target2 = this.Enabled;
    target.Enabled = target2;
    bool target3 = false;
    if (!serialization.TryCustomCopy<bool>(this.Locked, ref target3, hookCtx, false, context))
      target3 = this.Locked;
    target.Locked = target3;
    SoundPathSpecifier target4 = (SoundPathSpecifier) null;
    if (!serialization.TryCustomCopy<SoundPathSpecifier>(this.Sound, ref target4, hookCtx, false, context))
    {
      if (this.Sound == null)
        target4 = (SoundPathSpecifier) null;
      else
        serialization.CopyTo<SoundPathSpecifier>(this.Sound, ref target4, hookCtx, context);
    }
    target.Sound = target4;
    LocId target5 = new LocId();
    if (!serialization.TryCustomCopy<LocId>(this.Popup, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<LocId>(this.Popup, hookCtx, context);
    target.Popup = target5;
    LocId target6 = new LocId();
    if (!serialization.TryCustomCopy<LocId>(this.PopupKey, ref target6, hookCtx, false, context))
      target6 = serialization.CreateCopy<LocId>(this.PopupKey, hookCtx, context);
    target.PopupKey = target6;
    LocId target7 = new LocId();
    if (!serialization.TryCustomCopy<LocId>(this.ExamineText, ref target7, hookCtx, false, context))
      target7 = serialization.CreateCopy<LocId>(this.ExamineText, hookCtx, context);
    target.ExamineText = target7;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref RMCIgniterComponent target,
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
    RMCIgniterComponent target1 = (RMCIgniterComponent) target;
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
    RMCIgniterComponent target1 = (RMCIgniterComponent) target;
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
    RMCIgniterComponent target1 = (RMCIgniterComponent) target;
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
  virtual RMCIgniterComponent Component.Instantiate() => new RMCIgniterComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class RMCIgniterComponent_AutoState : IComponentState
  {
    public bool Enabled;
    public bool Locked;
    public SoundPathSpecifier? Sound;
    public LocId Popup;
    public LocId PopupKey;
    public LocId ExamineText;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class RMCIgniterComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<RMCIgniterComponent, ComponentGetState>(new ComponentEventRefHandler<RMCIgniterComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<RMCIgniterComponent, ComponentHandleState>(new ComponentEventRefHandler<RMCIgniterComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      RMCIgniterComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new RMCIgniterComponent.RMCIgniterComponent_AutoState()
      {
        Enabled = component.Enabled,
        Locked = component.Locked,
        Sound = component.Sound,
        Popup = component.Popup,
        PopupKey = component.PopupKey,
        ExamineText = component.ExamineText
      };
    }

    private void OnHandleState(
      EntityUid uid,
      RMCIgniterComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is RMCIgniterComponent.RMCIgniterComponent_AutoState current))
        return;
      component.Enabled = current.Enabled;
      component.Locked = current.Locked;
      component.Sound = current.Sound;
      component.Popup = current.Popup;
      component.PopupKey = current.PopupKey;
      component.ExamineText = current.ExamineText;
    }
  }
}
