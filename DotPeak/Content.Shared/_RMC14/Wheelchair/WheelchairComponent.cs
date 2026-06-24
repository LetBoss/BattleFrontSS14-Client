// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Wheelchair.WheelchairComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Wheelchair;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (WheelchairSystem)})]
public sealed class WheelchairComponent : 
  Component,
  ISerializationGenerated<WheelchairComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float SpeedMultiplier = 1f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntProtoId? BellAction;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SoundSpecifier BellSound = (SoundSpecifier) new SoundPathSpecifier("/Audio/Items/desk_bell_ring.ogg");

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref WheelchairComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (WheelchairComponent) target1;
    if (serialization.TryCustomCopy<WheelchairComponent>(this, ref target, hookCtx, false, context))
      return;
    float target2 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.SpeedMultiplier, ref target2, hookCtx, false, context))
      target2 = this.SpeedMultiplier;
    target.SpeedMultiplier = target2;
    EntProtoId? target3 = new EntProtoId?();
    if (!serialization.TryCustomCopy<EntProtoId?>(this.BellAction, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<EntProtoId?>(this.BellAction, hookCtx, context);
    target.BellAction = target3;
    SoundSpecifier target4 = (SoundSpecifier) null;
    if (this.BellSound == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.BellSound, ref target4, hookCtx, true, context))
      target4 = serialization.CreateCopy<SoundSpecifier>(this.BellSound, hookCtx, context);
    target.BellSound = target4;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref WheelchairComponent target,
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
    WheelchairComponent target1 = (WheelchairComponent) target;
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
    WheelchairComponent target1 = (WheelchairComponent) target;
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
    WheelchairComponent target1 = (WheelchairComponent) target;
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
  virtual WheelchairComponent Component.Instantiate() => new WheelchairComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class WheelchairComponent_AutoState : IComponentState
  {
    public float SpeedMultiplier;
    public EntProtoId? BellAction;
    public SoundSpecifier BellSound;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class WheelchairComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<WheelchairComponent, ComponentGetState>(new ComponentEventRefHandler<WheelchairComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<WheelchairComponent, ComponentHandleState>(new ComponentEventRefHandler<WheelchairComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      WheelchairComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new WheelchairComponent.WheelchairComponent_AutoState()
      {
        SpeedMultiplier = component.SpeedMultiplier,
        BellAction = component.BellAction,
        BellSound = component.BellSound
      };
    }

    private void OnHandleState(
      EntityUid uid,
      WheelchairComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is WheelchairComponent.WheelchairComponent_AutoState current))
        return;
      component.SpeedMultiplier = current.SpeedMultiplier;
      component.BellAction = current.BellAction;
      component.BellSound = current.BellSound;
    }
  }
}
