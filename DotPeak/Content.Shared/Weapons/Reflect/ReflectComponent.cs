// Decompiled with JetBrains decompiler
// Type: Content.Shared.Weapons.Reflect.ReflectComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Inventory;
using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Weapons.Reflect;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
public sealed class ReflectComponent : 
  Component,
  ISerializationGenerated<ReflectComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public ReflectType Reflects = ReflectType.NonEnergy | ReflectType.Energy;
  [DataField(null, false, 1, false, false, null)]
  public SlotFlags SlotFlags = SlotFlags.WITHOUT_POCKET;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool ReflectingInHands = true;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool InRightPlace;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float ReflectProb = 0.25f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public Angle Spread = Angle.FromDegrees(45.0);
  [DataField(null, false, 1, false, false, null)]
  public SoundSpecifier? SoundOnReflect = (SoundSpecifier) new SoundPathSpecifier("/Audio/Weapons/Guns/Hits/laser_sear_wall.ogg", new AudioParams?(AudioParams.Default.WithVariation(new float?(0.05f))));

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref ReflectComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (ReflectComponent) target1;
    if (serialization.TryCustomCopy<ReflectComponent>(this, ref target, hookCtx, false, context))
      return;
    ReflectType target2 = ReflectType.None;
    if (!serialization.TryCustomCopy<ReflectType>(this.Reflects, ref target2, hookCtx, false, context))
      target2 = this.Reflects;
    target.Reflects = target2;
    SlotFlags target3 = SlotFlags.NONE;
    if (!serialization.TryCustomCopy<SlotFlags>(this.SlotFlags, ref target3, hookCtx, false, context))
      target3 = this.SlotFlags;
    target.SlotFlags = target3;
    bool target4 = false;
    if (!serialization.TryCustomCopy<bool>(this.ReflectingInHands, ref target4, hookCtx, false, context))
      target4 = this.ReflectingInHands;
    target.ReflectingInHands = target4;
    bool target5 = false;
    if (!serialization.TryCustomCopy<bool>(this.InRightPlace, ref target5, hookCtx, false, context))
      target5 = this.InRightPlace;
    target.InRightPlace = target5;
    float target6 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.ReflectProb, ref target6, hookCtx, false, context))
      target6 = this.ReflectProb;
    target.ReflectProb = target6;
    Angle target7 = new Angle();
    if (!serialization.TryCustomCopy<Angle>(this.Spread, ref target7, hookCtx, false, context))
      target7 = serialization.CreateCopy<Angle>(this.Spread, hookCtx, context);
    target.Spread = target7;
    SoundSpecifier target8 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.SoundOnReflect, ref target8, hookCtx, true, context))
      target8 = serialization.CreateCopy<SoundSpecifier>(this.SoundOnReflect, hookCtx, context);
    target.SoundOnReflect = target8;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref ReflectComponent target,
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
    ReflectComponent target1 = (ReflectComponent) target;
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
    ReflectComponent target1 = (ReflectComponent) target;
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
    ReflectComponent target1 = (ReflectComponent) target;
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
  virtual ReflectComponent Component.Instantiate() => new ReflectComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class ReflectComponent_AutoState : IComponentState
  {
    public bool ReflectingInHands;
    public bool InRightPlace;
    public float ReflectProb;
    public Angle Spread;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class ReflectComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<ReflectComponent, ComponentGetState>(new ComponentEventRefHandler<ReflectComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<ReflectComponent, ComponentHandleState>(new ComponentEventRefHandler<ReflectComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(EntityUid uid, ReflectComponent component, ref ComponentGetState args)
    {
      args.State = (IComponentState) new ReflectComponent.ReflectComponent_AutoState()
      {
        ReflectingInHands = component.ReflectingInHands,
        InRightPlace = component.InRightPlace,
        ReflectProb = component.ReflectProb,
        Spread = component.Spread
      };
    }

    private void OnHandleState(
      EntityUid uid,
      ReflectComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is ReflectComponent.ReflectComponent_AutoState current))
        return;
      component.ReflectingInHands = current.ReflectingInHands;
      component.InRightPlace = current.InRightPlace;
      component.ReflectProb = current.ReflectProb;
      component.Spread = current.Spread;
    }
  }
}
