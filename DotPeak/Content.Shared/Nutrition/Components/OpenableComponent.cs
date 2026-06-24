// Decompiled with JetBrains decompiler
// Type: Content.Shared.Nutrition.Components.OpenableComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Nutrition.EntitySystems;
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
namespace Content.Shared.Nutrition.Components;

[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[RegisterComponent]
[Access(new Type[] {typeof (OpenableSystem)})]
public sealed class OpenableComponent : 
  Component,
  ISerializationGenerated<OpenableComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool Opened;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool OpenableByHand = true;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool OpenOnActivate;
  [DataField(null, false, 1, false, false, null)]
  public LocId ExamineText = (LocId) "drink-component-on-examine-is-opened";
  [DataField(null, false, 1, false, false, null)]
  public LocId ClosedPopup = (LocId) "drink-component-try-use-drink-not-open";
  [DataField(null, false, 1, false, false, null)]
  public LocId OpenVerbText = (LocId) "openable-component-verb-open";
  [DataField(null, false, 1, false, false, null)]
  public LocId CloseVerbText = (LocId) "openable-component-verb-close";
  [DataField(null, false, 1, false, false, null)]
  public SoundSpecifier? Sound = (SoundSpecifier) new SoundCollectionSpecifier("canOpenSounds");
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool Closeable;
  [DataField(null, false, 1, false, false, null)]
  public SoundSpecifier? CloseSound;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref OpenableComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (OpenableComponent) target1;
    if (serialization.TryCustomCopy<OpenableComponent>(this, ref target, hookCtx, false, context))
      return;
    bool target2 = false;
    if (!serialization.TryCustomCopy<bool>(this.Opened, ref target2, hookCtx, false, context))
      target2 = this.Opened;
    target.Opened = target2;
    bool target3 = false;
    if (!serialization.TryCustomCopy<bool>(this.OpenableByHand, ref target3, hookCtx, false, context))
      target3 = this.OpenableByHand;
    target.OpenableByHand = target3;
    bool target4 = false;
    if (!serialization.TryCustomCopy<bool>(this.OpenOnActivate, ref target4, hookCtx, false, context))
      target4 = this.OpenOnActivate;
    target.OpenOnActivate = target4;
    LocId target5 = new LocId();
    if (!serialization.TryCustomCopy<LocId>(this.ExamineText, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<LocId>(this.ExamineText, hookCtx, context);
    target.ExamineText = target5;
    LocId target6 = new LocId();
    if (!serialization.TryCustomCopy<LocId>(this.ClosedPopup, ref target6, hookCtx, false, context))
      target6 = serialization.CreateCopy<LocId>(this.ClosedPopup, hookCtx, context);
    target.ClosedPopup = target6;
    LocId target7 = new LocId();
    if (!serialization.TryCustomCopy<LocId>(this.OpenVerbText, ref target7, hookCtx, false, context))
      target7 = serialization.CreateCopy<LocId>(this.OpenVerbText, hookCtx, context);
    target.OpenVerbText = target7;
    LocId target8 = new LocId();
    if (!serialization.TryCustomCopy<LocId>(this.CloseVerbText, ref target8, hookCtx, false, context))
      target8 = serialization.CreateCopy<LocId>(this.CloseVerbText, hookCtx, context);
    target.CloseVerbText = target8;
    SoundSpecifier target9 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.Sound, ref target9, hookCtx, true, context))
      target9 = serialization.CreateCopy<SoundSpecifier>(this.Sound, hookCtx, context);
    target.Sound = target9;
    bool target10 = false;
    if (!serialization.TryCustomCopy<bool>(this.Closeable, ref target10, hookCtx, false, context))
      target10 = this.Closeable;
    target.Closeable = target10;
    SoundSpecifier target11 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.CloseSound, ref target11, hookCtx, true, context))
      target11 = serialization.CreateCopy<SoundSpecifier>(this.CloseSound, hookCtx, context);
    target.CloseSound = target11;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref OpenableComponent target,
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
    OpenableComponent target1 = (OpenableComponent) target;
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
    OpenableComponent target1 = (OpenableComponent) target;
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
    OpenableComponent target1 = (OpenableComponent) target;
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
  virtual OpenableComponent Component.Instantiate() => new OpenableComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class OpenableComponent_AutoState : IComponentState
  {
    public bool Opened;
    public bool OpenableByHand;
    public bool OpenOnActivate;
    public bool Closeable;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class OpenableComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<OpenableComponent, ComponentGetState>(new ComponentEventRefHandler<OpenableComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<OpenableComponent, ComponentHandleState>(new ComponentEventRefHandler<OpenableComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(EntityUid uid, OpenableComponent component, ref ComponentGetState args)
    {
      args.State = (IComponentState) new OpenableComponent.OpenableComponent_AutoState()
      {
        Opened = component.Opened,
        OpenableByHand = component.OpenableByHand,
        OpenOnActivate = component.OpenOnActivate,
        Closeable = component.Closeable
      };
    }

    private void OnHandleState(
      EntityUid uid,
      OpenableComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is OpenableComponent.OpenableComponent_AutoState current))
        return;
      component.Opened = current.Opened;
      component.OpenableByHand = current.OpenableByHand;
      component.OpenOnActivate = current.OpenOnActivate;
      component.Closeable = current.Closeable;
    }
  }
}
