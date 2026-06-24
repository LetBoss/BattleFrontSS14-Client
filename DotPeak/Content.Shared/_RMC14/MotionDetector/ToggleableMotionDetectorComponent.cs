// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.MotionDetector.ToggleableMotionDetectorComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Inventory;
using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.MotionDetector;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (MotionDetectorSystem)})]
public sealed class ToggleableMotionDetectorComponent : 
  Component,
  ISerializationGenerated<ToggleableMotionDetectorComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SlotFlags Slots = SlotFlags.All;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float BatteryDrain = 0.45f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntProtoId ActionId = (EntProtoId) "RMCActionToggleMotionDetector";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntityUid? Action;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SoundSpecifier? ToggleSound = (SoundSpecifier) new SoundPathSpecifier("/Audio/_RMC14/Machines/click.ogg");

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref ToggleableMotionDetectorComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (ToggleableMotionDetectorComponent) target1;
    if (serialization.TryCustomCopy<ToggleableMotionDetectorComponent>(this, ref target, hookCtx, false, context))
      return;
    SlotFlags target2 = SlotFlags.NONE;
    if (!serialization.TryCustomCopy<SlotFlags>(this.Slots, ref target2, hookCtx, false, context))
      target2 = this.Slots;
    target.Slots = target2;
    float target3 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.BatteryDrain, ref target3, hookCtx, false, context))
      target3 = this.BatteryDrain;
    target.BatteryDrain = target3;
    EntProtoId target4 = new EntProtoId();
    if (!serialization.TryCustomCopy<EntProtoId>(this.ActionId, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<EntProtoId>(this.ActionId, hookCtx, context);
    target.ActionId = target4;
    EntityUid? target5 = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.Action, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<EntityUid?>(this.Action, hookCtx, context);
    target.Action = target5;
    SoundSpecifier target6 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.ToggleSound, ref target6, hookCtx, true, context))
      target6 = serialization.CreateCopy<SoundSpecifier>(this.ToggleSound, hookCtx, context);
    target.ToggleSound = target6;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref ToggleableMotionDetectorComponent target,
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
    ToggleableMotionDetectorComponent target1 = (ToggleableMotionDetectorComponent) target;
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
    ToggleableMotionDetectorComponent target1 = (ToggleableMotionDetectorComponent) target;
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
    ToggleableMotionDetectorComponent target1 = (ToggleableMotionDetectorComponent) target;
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
  virtual ToggleableMotionDetectorComponent Component.Instantiate()
  {
    return new ToggleableMotionDetectorComponent();
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class ToggleableMotionDetectorComponent_AutoState : IComponentState
  {
    public SlotFlags Slots;
    public float BatteryDrain;
    public EntProtoId ActionId;
    public NetEntity? Action;
    public SoundSpecifier? ToggleSound;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class ToggleableMotionDetectorComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<ToggleableMotionDetectorComponent, ComponentGetState>(new ComponentEventRefHandler<ToggleableMotionDetectorComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<ToggleableMotionDetectorComponent, ComponentHandleState>(new ComponentEventRefHandler<ToggleableMotionDetectorComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      ToggleableMotionDetectorComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new ToggleableMotionDetectorComponent.ToggleableMotionDetectorComponent_AutoState()
      {
        Slots = component.Slots,
        BatteryDrain = component.BatteryDrain,
        ActionId = component.ActionId,
        Action = this.GetNetEntity(component.Action),
        ToggleSound = component.ToggleSound
      };
    }

    private void OnHandleState(
      EntityUid uid,
      ToggleableMotionDetectorComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is ToggleableMotionDetectorComponent.ToggleableMotionDetectorComponent_AutoState current))
        return;
      component.Slots = current.Slots;
      component.BatteryDrain = current.BatteryDrain;
      component.ActionId = current.ActionId;
      component.Action = this.EnsureEntity<ToggleableMotionDetectorComponent>(current.Action, uid);
      component.ToggleSound = current.ToggleSound;
    }
  }
}
