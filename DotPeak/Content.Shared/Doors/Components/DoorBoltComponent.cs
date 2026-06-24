// Decompiled with JetBrains decompiler
// Type: Content.Shared.Doors.Components.DoorBoltComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Doors.Systems;
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
namespace Content.Shared.Doors.Components;

[RegisterComponent]
[NetworkedComponent]
[Access(new Type[] {typeof (SharedDoorSystem)})]
[AutoGenerateComponentState(false, false)]
public sealed class DoorBoltComponent : 
  Component,
  ISerializationGenerated<DoorBoltComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public SoundSpecifier BoltUpSound = (SoundSpecifier) new SoundPathSpecifier("/Audio/Machines/boltsup.ogg");
  [DataField(null, false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public SoundSpecifier BoltDownSound = (SoundSpecifier) new SoundPathSpecifier("/Audio/Machines/boltsdown.ogg");
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool BoltsDown;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool BoltLightsEnabled = true;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool BoltWireCut;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool Powered;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref DoorBoltComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (DoorBoltComponent) target1;
    if (serialization.TryCustomCopy<DoorBoltComponent>(this, ref target, hookCtx, false, context))
      return;
    SoundSpecifier target2 = (SoundSpecifier) null;
    if (this.BoltUpSound == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.BoltUpSound, ref target2, hookCtx, true, context))
      target2 = serialization.CreateCopy<SoundSpecifier>(this.BoltUpSound, hookCtx, context);
    target.BoltUpSound = target2;
    SoundSpecifier target3 = (SoundSpecifier) null;
    if (this.BoltDownSound == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.BoltDownSound, ref target3, hookCtx, true, context))
      target3 = serialization.CreateCopy<SoundSpecifier>(this.BoltDownSound, hookCtx, context);
    target.BoltDownSound = target3;
    bool target4 = false;
    if (!serialization.TryCustomCopy<bool>(this.BoltsDown, ref target4, hookCtx, false, context))
      target4 = this.BoltsDown;
    target.BoltsDown = target4;
    bool target5 = false;
    if (!serialization.TryCustomCopy<bool>(this.BoltLightsEnabled, ref target5, hookCtx, false, context))
      target5 = this.BoltLightsEnabled;
    target.BoltLightsEnabled = target5;
    bool target6 = false;
    if (!serialization.TryCustomCopy<bool>(this.BoltWireCut, ref target6, hookCtx, false, context))
      target6 = this.BoltWireCut;
    target.BoltWireCut = target6;
    bool target7 = false;
    if (!serialization.TryCustomCopy<bool>(this.Powered, ref target7, hookCtx, false, context))
      target7 = this.Powered;
    target.Powered = target7;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref DoorBoltComponent target,
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
    DoorBoltComponent target1 = (DoorBoltComponent) target;
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
    DoorBoltComponent target1 = (DoorBoltComponent) target;
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
    DoorBoltComponent target1 = (DoorBoltComponent) target;
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
  virtual DoorBoltComponent Component.Instantiate() => new DoorBoltComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class DoorBoltComponent_AutoState : IComponentState
  {
    public bool BoltsDown;
    public bool BoltLightsEnabled;
    public bool BoltWireCut;
    public bool Powered;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class DoorBoltComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<DoorBoltComponent, ComponentGetState>(new ComponentEventRefHandler<DoorBoltComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<DoorBoltComponent, ComponentHandleState>(new ComponentEventRefHandler<DoorBoltComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(EntityUid uid, DoorBoltComponent component, ref ComponentGetState args)
    {
      args.State = (IComponentState) new DoorBoltComponent.DoorBoltComponent_AutoState()
      {
        BoltsDown = component.BoltsDown,
        BoltLightsEnabled = component.BoltLightsEnabled,
        BoltWireCut = component.BoltWireCut,
        Powered = component.Powered
      };
    }

    private void OnHandleState(
      EntityUid uid,
      DoorBoltComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is DoorBoltComponent.DoorBoltComponent_AutoState current))
        return;
      component.BoltsDown = current.BoltsDown;
      component.BoltLightsEnabled = current.BoltLightsEnabled;
      component.BoltWireCut = current.BoltWireCut;
      component.Powered = current.Powered;
    }
  }
}
