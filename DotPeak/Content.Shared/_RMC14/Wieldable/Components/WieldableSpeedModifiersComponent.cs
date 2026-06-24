// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Wieldable.Components.WieldableSpeedModifiersComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

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
namespace Content.Shared._RMC14.Wieldable.Components;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (RMCWieldableSystem)})]
public sealed class WieldableSpeedModifiersComponent : 
  Component,
  ISerializationGenerated<WieldableSpeedModifiersComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float Base = 1f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float Light = 1f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float Medium = 1f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float Heavy = 1f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float ModifiedSprint = 1f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float ModifiedWalk = 1f;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref WieldableSpeedModifiersComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (WieldableSpeedModifiersComponent) target1;
    if (serialization.TryCustomCopy<WieldableSpeedModifiersComponent>(this, ref target, hookCtx, false, context))
      return;
    float target2 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.Base, ref target2, hookCtx, false, context))
      target2 = this.Base;
    target.Base = target2;
    float target3 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.Light, ref target3, hookCtx, false, context))
      target3 = this.Light;
    target.Light = target3;
    float target4 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.Medium, ref target4, hookCtx, false, context))
      target4 = this.Medium;
    target.Medium = target4;
    float target5 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.Heavy, ref target5, hookCtx, false, context))
      target5 = this.Heavy;
    target.Heavy = target5;
    float target6 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.ModifiedSprint, ref target6, hookCtx, false, context))
      target6 = this.ModifiedSprint;
    target.ModifiedSprint = target6;
    float target7 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.ModifiedWalk, ref target7, hookCtx, false, context))
      target7 = this.ModifiedWalk;
    target.ModifiedWalk = target7;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref WieldableSpeedModifiersComponent target,
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
    WieldableSpeedModifiersComponent target1 = (WieldableSpeedModifiersComponent) target;
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
    WieldableSpeedModifiersComponent target1 = (WieldableSpeedModifiersComponent) target;
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
    WieldableSpeedModifiersComponent target1 = (WieldableSpeedModifiersComponent) target;
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
  virtual WieldableSpeedModifiersComponent Component.Instantiate()
  {
    return new WieldableSpeedModifiersComponent();
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class WieldableSpeedModifiersComponent_AutoState : IComponentState
  {
    public float Base;
    public float Light;
    public float Medium;
    public float Heavy;
    public float ModifiedSprint;
    public float ModifiedWalk;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class WieldableSpeedModifiersComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<WieldableSpeedModifiersComponent, ComponentGetState>(new ComponentEventRefHandler<WieldableSpeedModifiersComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<WieldableSpeedModifiersComponent, ComponentHandleState>(new ComponentEventRefHandler<WieldableSpeedModifiersComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      WieldableSpeedModifiersComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new WieldableSpeedModifiersComponent.WieldableSpeedModifiersComponent_AutoState()
      {
        Base = component.Base,
        Light = component.Light,
        Medium = component.Medium,
        Heavy = component.Heavy,
        ModifiedSprint = component.ModifiedSprint,
        ModifiedWalk = component.ModifiedWalk
      };
    }

    private void OnHandleState(
      EntityUid uid,
      WieldableSpeedModifiersComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is WieldableSpeedModifiersComponent.WieldableSpeedModifiersComponent_AutoState current))
        return;
      component.Base = current.Base;
      component.Light = current.Light;
      component.Medium = current.Medium;
      component.Heavy = current.Heavy;
      component.ModifiedSprint = current.ModifiedSprint;
      component.ModifiedWalk = current.ModifiedWalk;
    }
  }
}
