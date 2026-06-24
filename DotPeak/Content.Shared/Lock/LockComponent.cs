// Decompiled with JetBrains decompiler
// Type: Content.Shared.Lock.LockComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.ViewVariables;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Lock;

[RegisterComponent]
[NetworkedComponent]
[Access(new Type[] {typeof (LockSystem)})]
[AutoGenerateComponentState(false, false)]
public sealed class LockComponent : 
  Component,
  ISerializationGenerated<LockComponent>,
  ISerializationGenerated
{
  [DataField("locked", false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [AutoNetworkedField]
  public bool Locked;
  [DataField("lockOnClick", false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [AutoNetworkedField]
  public bool LockOnClick;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool UnlockOnClick;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool UseAccess;
  [DataField("unlockingSound", false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public SoundSpecifier UnlockSound;
  [DataField("lockingSound", false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public SoundSpecifier LockSound;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool BreakOnAccessBreaker;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan LockTime;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan UnlockTime;

  public LockComponent()
  {
    SoundPathSpecifier soundPathSpecifier1 = new SoundPathSpecifier("/Audio/Machines/door_lock_off.ogg");
    soundPathSpecifier1.Params = AudioParams.Default.WithVolume(-5f);
    this.UnlockSound = (SoundSpecifier) soundPathSpecifier1;
    SoundPathSpecifier soundPathSpecifier2 = new SoundPathSpecifier("/Audio/Machines/door_lock_on.ogg");
    soundPathSpecifier2.Params = AudioParams.Default.WithVolume(-5f);
    this.LockSound = (SoundSpecifier) soundPathSpecifier2;
    this.BreakOnAccessBreaker = true;
    // ISSUE: explicit constructor call
    base.\u002Ector();
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref LockComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (LockComponent) target1;
    if (serialization.TryCustomCopy<LockComponent>(this, ref target, hookCtx, false, context))
      return;
    bool target2 = false;
    if (!serialization.TryCustomCopy<bool>(this.Locked, ref target2, hookCtx, false, context))
      target2 = this.Locked;
    target.Locked = target2;
    bool target3 = false;
    if (!serialization.TryCustomCopy<bool>(this.LockOnClick, ref target3, hookCtx, false, context))
      target3 = this.LockOnClick;
    target.LockOnClick = target3;
    bool target4 = false;
    if (!serialization.TryCustomCopy<bool>(this.UnlockOnClick, ref target4, hookCtx, false, context))
      target4 = this.UnlockOnClick;
    target.UnlockOnClick = target4;
    bool target5 = false;
    if (!serialization.TryCustomCopy<bool>(this.UseAccess, ref target5, hookCtx, false, context))
      target5 = this.UseAccess;
    target.UseAccess = target5;
    SoundSpecifier target6 = (SoundSpecifier) null;
    if (this.UnlockSound == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.UnlockSound, ref target6, hookCtx, true, context))
      target6 = serialization.CreateCopy<SoundSpecifier>(this.UnlockSound, hookCtx, context);
    target.UnlockSound = target6;
    SoundSpecifier target7 = (SoundSpecifier) null;
    if (this.LockSound == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.LockSound, ref target7, hookCtx, true, context))
      target7 = serialization.CreateCopy<SoundSpecifier>(this.LockSound, hookCtx, context);
    target.LockSound = target7;
    bool target8 = false;
    if (!serialization.TryCustomCopy<bool>(this.BreakOnAccessBreaker, ref target8, hookCtx, false, context))
      target8 = this.BreakOnAccessBreaker;
    target.BreakOnAccessBreaker = target8;
    TimeSpan target9 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.LockTime, ref target9, hookCtx, false, context))
      target9 = serialization.CreateCopy<TimeSpan>(this.LockTime, hookCtx, context);
    target.LockTime = target9;
    TimeSpan target10 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.UnlockTime, ref target10, hookCtx, false, context))
      target10 = serialization.CreateCopy<TimeSpan>(this.UnlockTime, hookCtx, context);
    target.UnlockTime = target10;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref LockComponent target,
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
    LockComponent target1 = (LockComponent) target;
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
    LockComponent target1 = (LockComponent) target;
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
    LockComponent target1 = (LockComponent) target;
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
  virtual LockComponent Component.Instantiate() => new LockComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class LockComponent_AutoState : IComponentState
  {
    public bool Locked;
    public bool LockOnClick;
    public bool UnlockOnClick;
    public bool UseAccess;
    public bool BreakOnAccessBreaker;
    public TimeSpan LockTime;
    public TimeSpan UnlockTime;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class LockComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<LockComponent, ComponentGetState>(new ComponentEventRefHandler<LockComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<LockComponent, ComponentHandleState>(new ComponentEventRefHandler<LockComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(EntityUid uid, LockComponent component, ref ComponentGetState args)
    {
      args.State = (IComponentState) new LockComponent.LockComponent_AutoState()
      {
        Locked = component.Locked,
        LockOnClick = component.LockOnClick,
        UnlockOnClick = component.UnlockOnClick,
        UseAccess = component.UseAccess,
        BreakOnAccessBreaker = component.BreakOnAccessBreaker,
        LockTime = component.LockTime,
        UnlockTime = component.UnlockTime
      };
    }

    private void OnHandleState(
      EntityUid uid,
      LockComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is LockComponent.LockComponent_AutoState current))
        return;
      component.Locked = current.Locked;
      component.LockOnClick = current.LockOnClick;
      component.UnlockOnClick = current.UnlockOnClick;
      component.UseAccess = current.UseAccess;
      component.BreakOnAccessBreaker = current.BreakOnAccessBreaker;
      component.LockTime = current.LockTime;
      component.UnlockTime = current.UnlockTime;
    }
  }
}
