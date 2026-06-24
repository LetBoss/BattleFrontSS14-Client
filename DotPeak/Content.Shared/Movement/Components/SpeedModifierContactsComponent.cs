// Decompiled with JetBrains decompiler
// Type: Content.Shared.Movement.Components.SpeedModifierContactsComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Movement.Systems;
using Content.Shared.Whitelist;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Movement.Components;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (SpeedModifierContactsSystem)})]
public sealed class SpeedModifierContactsComponent : 
  Component,
  ISerializationGenerated<SpeedModifierContactsComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float WalkSpeedModifier = 1f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float SprintSpeedModifier = 1f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool AffectAirborne;
  [DataField(null, false, 1, false, false, null)]
  public EntityWhitelist? IgnoreWhitelist;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref SpeedModifierContactsComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (SpeedModifierContactsComponent) target1;
    if (serialization.TryCustomCopy<SpeedModifierContactsComponent>(this, ref target, hookCtx, false, context))
      return;
    float target2 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.WalkSpeedModifier, ref target2, hookCtx, false, context))
      target2 = this.WalkSpeedModifier;
    target.WalkSpeedModifier = target2;
    float target3 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.SprintSpeedModifier, ref target3, hookCtx, false, context))
      target3 = this.SprintSpeedModifier;
    target.SprintSpeedModifier = target3;
    bool target4 = false;
    if (!serialization.TryCustomCopy<bool>(this.AffectAirborne, ref target4, hookCtx, false, context))
      target4 = this.AffectAirborne;
    target.AffectAirborne = target4;
    EntityWhitelist target5 = (EntityWhitelist) null;
    if (!serialization.TryCustomCopy<EntityWhitelist>(this.IgnoreWhitelist, ref target5, hookCtx, false, context))
    {
      if (this.IgnoreWhitelist == null)
        target5 = (EntityWhitelist) null;
      else
        serialization.CopyTo<EntityWhitelist>(this.IgnoreWhitelist, ref target5, hookCtx, context);
    }
    target.IgnoreWhitelist = target5;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref SpeedModifierContactsComponent target,
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
    SpeedModifierContactsComponent target1 = (SpeedModifierContactsComponent) target;
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
    SpeedModifierContactsComponent target1 = (SpeedModifierContactsComponent) target;
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
    SpeedModifierContactsComponent target1 = (SpeedModifierContactsComponent) target;
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
  virtual SpeedModifierContactsComponent Component.Instantiate()
  {
    return new SpeedModifierContactsComponent();
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class SpeedModifierContactsComponent_AutoState : IComponentState
  {
    public float WalkSpeedModifier;
    public float SprintSpeedModifier;
    public bool AffectAirborne;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class SpeedModifierContactsComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<SpeedModifierContactsComponent, ComponentGetState>(new ComponentEventRefHandler<SpeedModifierContactsComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<SpeedModifierContactsComponent, ComponentHandleState>(new ComponentEventRefHandler<SpeedModifierContactsComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      SpeedModifierContactsComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new SpeedModifierContactsComponent.SpeedModifierContactsComponent_AutoState()
      {
        WalkSpeedModifier = component.WalkSpeedModifier,
        SprintSpeedModifier = component.SprintSpeedModifier,
        AffectAirborne = component.AffectAirborne
      };
    }

    private void OnHandleState(
      EntityUid uid,
      SpeedModifierContactsComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is SpeedModifierContactsComponent.SpeedModifierContactsComponent_AutoState current))
        return;
      component.WalkSpeedModifier = current.WalkSpeedModifier;
      component.SprintSpeedModifier = current.SprintSpeedModifier;
      component.AffectAirborne = current.AffectAirborne;
    }
  }
}
