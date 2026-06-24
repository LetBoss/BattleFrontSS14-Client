// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Telephone.RotaryPhoneDialingComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Telephone;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[AutoGenerateComponentPause]
[Access(new Type[] {typeof (SharedRMCTelephoneSystem)})]
public sealed class RotaryPhoneDialingComponent : 
  Component,
  ISerializationGenerated<RotaryPhoneDialingComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntityUid? Other;
  [DataField(null, false, 1, false, false, typeof (TimeOffsetSerializer))]
  [AutoNetworkedField]
  [AutoPausedField]
  public TimeSpan LastVoicemail;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool DidVoicemail;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool DidVoicemailTimeout;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref RotaryPhoneDialingComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (RotaryPhoneDialingComponent) target1;
    if (serialization.TryCustomCopy<RotaryPhoneDialingComponent>(this, ref target, hookCtx, false, context))
      return;
    EntityUid? target2 = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.Other, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<EntityUid?>(this.Other, hookCtx, context);
    target.Other = target2;
    TimeSpan target3 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.LastVoicemail, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<TimeSpan>(this.LastVoicemail, hookCtx, context);
    target.LastVoicemail = target3;
    bool target4 = false;
    if (!serialization.TryCustomCopy<bool>(this.DidVoicemail, ref target4, hookCtx, false, context))
      target4 = this.DidVoicemail;
    target.DidVoicemail = target4;
    bool target5 = false;
    if (!serialization.TryCustomCopy<bool>(this.DidVoicemailTimeout, ref target5, hookCtx, false, context))
      target5 = this.DidVoicemailTimeout;
    target.DidVoicemailTimeout = target5;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref RotaryPhoneDialingComponent target,
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
    RotaryPhoneDialingComponent target1 = (RotaryPhoneDialingComponent) target;
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
    RotaryPhoneDialingComponent target1 = (RotaryPhoneDialingComponent) target;
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
    RotaryPhoneDialingComponent target1 = (RotaryPhoneDialingComponent) target;
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
  virtual RotaryPhoneDialingComponent Component.Instantiate() => new RotaryPhoneDialingComponent();

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class RotaryPhoneDialingComponent_AutoPauseSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<RotaryPhoneDialingComponent, EntityUnpausedEvent>(new ComponentEventRefHandler<RotaryPhoneDialingComponent, EntityUnpausedEvent>(this.OnEntityUnpaused));
    }

    private void OnEntityUnpaused(
      EntityUid uid,
      #nullable disable
      RotaryPhoneDialingComponent component,
      ref EntityUnpausedEvent args)
    {
      component.LastVoicemail += args.PausedTime;
      this.Dirty(uid, (IComponent) component);
    }
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class RotaryPhoneDialingComponent_AutoState : IComponentState
  {
    public NetEntity? Other;
    public TimeSpan LastVoicemail;
    public bool DidVoicemail;
    public bool DidVoicemailTimeout;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class RotaryPhoneDialingComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<RotaryPhoneDialingComponent, ComponentGetState>(new ComponentEventRefHandler<RotaryPhoneDialingComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<RotaryPhoneDialingComponent, ComponentHandleState>(new ComponentEventRefHandler<RotaryPhoneDialingComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      #nullable enable
      RotaryPhoneDialingComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new RotaryPhoneDialingComponent.RotaryPhoneDialingComponent_AutoState()
      {
        Other = this.GetNetEntity(component.Other),
        LastVoicemail = component.LastVoicemail,
        DidVoicemail = component.DidVoicemail,
        DidVoicemailTimeout = component.DidVoicemailTimeout
      };
    }

    private void OnHandleState(
      EntityUid uid,
      RotaryPhoneDialingComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is RotaryPhoneDialingComponent.RotaryPhoneDialingComponent_AutoState current))
        return;
      component.Other = this.EnsureEntity<RotaryPhoneDialingComponent>(current.Other, uid);
      component.LastVoicemail = current.LastVoicemail;
      component.DidVoicemail = current.DidVoicemail;
      component.DidVoicemailTimeout = current.DidVoicemailTimeout;
    }
  }
}
