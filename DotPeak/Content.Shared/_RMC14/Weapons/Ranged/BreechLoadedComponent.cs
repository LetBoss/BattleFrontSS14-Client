// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Weapons.Ranged.BreechLoadedComponent
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
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Weapons.Ranged;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (BreechLoadedSystem)})]
public sealed class BreechLoadedComponent : 
  Component,
  ISerializationGenerated<BreechLoadedComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool Open;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool NeedOpenClose;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool Ready;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SoundSpecifier OpenSound = (SoundSpecifier) new SoundPathSpecifier("/Audio/_RMC14/Weapons/Guns/Breech/ugl_open.ogg", new AudioParams?(AudioParams.Default.WithVolume(-6.5f)));
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SoundSpecifier CloseSound = (SoundSpecifier) new SoundPathSpecifier("/Audio/_RMC14/Weapons/Guns/Breech/ugl_close.ogg", new AudioParams?(AudioParams.Default.WithVolume(-6.5f)));
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool ShowBreechOpen = true;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public string DelayId = "breech-toggle-delay";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan ToggleDelay = TimeSpan.FromSeconds(0L);

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref BreechLoadedComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (BreechLoadedComponent) target1;
    if (serialization.TryCustomCopy<BreechLoadedComponent>(this, ref target, hookCtx, false, context))
      return;
    bool target2 = false;
    if (!serialization.TryCustomCopy<bool>(this.Open, ref target2, hookCtx, false, context))
      target2 = this.Open;
    target.Open = target2;
    bool target3 = false;
    if (!serialization.TryCustomCopy<bool>(this.NeedOpenClose, ref target3, hookCtx, false, context))
      target3 = this.NeedOpenClose;
    target.NeedOpenClose = target3;
    bool target4 = false;
    if (!serialization.TryCustomCopy<bool>(this.Ready, ref target4, hookCtx, false, context))
      target4 = this.Ready;
    target.Ready = target4;
    SoundSpecifier target5 = (SoundSpecifier) null;
    if (this.OpenSound == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.OpenSound, ref target5, hookCtx, true, context))
      target5 = serialization.CreateCopy<SoundSpecifier>(this.OpenSound, hookCtx, context);
    target.OpenSound = target5;
    SoundSpecifier target6 = (SoundSpecifier) null;
    if (this.CloseSound == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.CloseSound, ref target6, hookCtx, true, context))
      target6 = serialization.CreateCopy<SoundSpecifier>(this.CloseSound, hookCtx, context);
    target.CloseSound = target6;
    bool target7 = false;
    if (!serialization.TryCustomCopy<bool>(this.ShowBreechOpen, ref target7, hookCtx, false, context))
      target7 = this.ShowBreechOpen;
    target.ShowBreechOpen = target7;
    string target8 = (string) null;
    if (this.DelayId == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.DelayId, ref target8, hookCtx, false, context))
      target8 = this.DelayId;
    target.DelayId = target8;
    TimeSpan target9 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.ToggleDelay, ref target9, hookCtx, false, context))
      target9 = serialization.CreateCopy<TimeSpan>(this.ToggleDelay, hookCtx, context);
    target.ToggleDelay = target9;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref BreechLoadedComponent target,
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
    BreechLoadedComponent target1 = (BreechLoadedComponent) target;
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
    BreechLoadedComponent target1 = (BreechLoadedComponent) target;
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
    BreechLoadedComponent target1 = (BreechLoadedComponent) target;
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
  virtual BreechLoadedComponent Component.Instantiate() => new BreechLoadedComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class BreechLoadedComponent_AutoState : IComponentState
  {
    public bool Open;
    public bool NeedOpenClose;
    public bool Ready;
    public SoundSpecifier OpenSound;
    public SoundSpecifier CloseSound;
    public bool ShowBreechOpen;
    public string DelayId;
    public TimeSpan ToggleDelay;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class BreechLoadedComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<BreechLoadedComponent, ComponentGetState>(new ComponentEventRefHandler<BreechLoadedComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<BreechLoadedComponent, ComponentHandleState>(new ComponentEventRefHandler<BreechLoadedComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      BreechLoadedComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new BreechLoadedComponent.BreechLoadedComponent_AutoState()
      {
        Open = component.Open,
        NeedOpenClose = component.NeedOpenClose,
        Ready = component.Ready,
        OpenSound = component.OpenSound,
        CloseSound = component.CloseSound,
        ShowBreechOpen = component.ShowBreechOpen,
        DelayId = component.DelayId,
        ToggleDelay = component.ToggleDelay
      };
    }

    private void OnHandleState(
      EntityUid uid,
      BreechLoadedComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is BreechLoadedComponent.BreechLoadedComponent_AutoState current))
        return;
      component.Open = current.Open;
      component.NeedOpenClose = current.NeedOpenClose;
      component.Ready = current.Ready;
      component.OpenSound = current.OpenSound;
      component.CloseSound = current.CloseSound;
      component.ShowBreechOpen = current.ShowBreechOpen;
      component.DelayId = current.DelayId;
      component.ToggleDelay = current.ToggleDelay;
    }
  }
}
