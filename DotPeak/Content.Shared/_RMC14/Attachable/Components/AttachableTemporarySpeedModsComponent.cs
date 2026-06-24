// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Attachable.Components.AttachableTemporarySpeedModsComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Attachable.Events;
using Content.Shared._RMC14.Attachable.Systems;
using Content.Shared._RMC14.Movement;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Attachable.Components;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (AttachableTemporarySpeedModsSystem)})]
public sealed class AttachableTemporarySpeedModsComponent : 
  Component,
  ISerializationGenerated<AttachableTemporarySpeedModsComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public AttachableAlteredType Alteration = AttachableAlteredType.Interrupted;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public List<TemporarySpeedModifierSet> Modifiers = new List<TemporarySpeedModifierSet>();
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan SlowDuration = TimeSpan.FromSeconds(4L);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan SuperSlowDuration = TimeSpan.FromSeconds(2L);

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref AttachableTemporarySpeedModsComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (AttachableTemporarySpeedModsComponent) target1;
    if (serialization.TryCustomCopy<AttachableTemporarySpeedModsComponent>(this, ref target, hookCtx, false, context))
      return;
    AttachableAlteredType target2 = ~(AttachableAlteredType.DetachedDeactivated | AttachableAlteredType.Attached | AttachableAlteredType.Wielded | AttachableAlteredType.Unwielded | AttachableAlteredType.Activated | AttachableAlteredType.Interrupted | AttachableAlteredType.AppearanceChanged);
    if (!serialization.TryCustomCopy<AttachableAlteredType>(this.Alteration, ref target2, hookCtx, false, context))
      target2 = this.Alteration;
    target.Alteration = target2;
    List<TemporarySpeedModifierSet> target3 = (List<TemporarySpeedModifierSet>) null;
    if (this.Modifiers == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<List<TemporarySpeedModifierSet>>(this.Modifiers, ref target3, hookCtx, true, context))
      target3 = serialization.CreateCopy<List<TemporarySpeedModifierSet>>(this.Modifiers, hookCtx, context);
    target.Modifiers = target3;
    TimeSpan target4 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.SlowDuration, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<TimeSpan>(this.SlowDuration, hookCtx, context);
    target.SlowDuration = target4;
    TimeSpan target5 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.SuperSlowDuration, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<TimeSpan>(this.SuperSlowDuration, hookCtx, context);
    target.SuperSlowDuration = target5;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref AttachableTemporarySpeedModsComponent target,
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
    AttachableTemporarySpeedModsComponent target1 = (AttachableTemporarySpeedModsComponent) target;
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
    AttachableTemporarySpeedModsComponent target1 = (AttachableTemporarySpeedModsComponent) target;
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
    AttachableTemporarySpeedModsComponent target1 = (AttachableTemporarySpeedModsComponent) target;
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
  virtual AttachableTemporarySpeedModsComponent Component.Instantiate()
  {
    return new AttachableTemporarySpeedModsComponent();
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class AttachableTemporarySpeedModsComponent_AutoState : IComponentState
  {
    public AttachableAlteredType Alteration;
    public List<TemporarySpeedModifierSet> Modifiers;
    public TimeSpan SlowDuration;
    public TimeSpan SuperSlowDuration;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class AttachableTemporarySpeedModsComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<AttachableTemporarySpeedModsComponent, ComponentGetState>(new ComponentEventRefHandler<AttachableTemporarySpeedModsComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<AttachableTemporarySpeedModsComponent, ComponentHandleState>(new ComponentEventRefHandler<AttachableTemporarySpeedModsComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      AttachableTemporarySpeedModsComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new AttachableTemporarySpeedModsComponent.AttachableTemporarySpeedModsComponent_AutoState()
      {
        Alteration = component.Alteration,
        Modifiers = component.Modifiers,
        SlowDuration = component.SlowDuration,
        SuperSlowDuration = component.SuperSlowDuration
      };
    }

    private void OnHandleState(
      EntityUid uid,
      AttachableTemporarySpeedModsComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is AttachableTemporarySpeedModsComponent.AttachableTemporarySpeedModsComponent_AutoState current))
        return;
      component.Alteration = current.Alteration;
      component.Modifiers = current.Modifiers == null ? (List<TemporarySpeedModifierSet>) null : new List<TemporarySpeedModifierSet>((IEnumerable<TemporarySpeedModifierSet>) current.Modifiers);
      component.SlowDuration = current.SlowDuration;
      component.SuperSlowDuration = current.SuperSlowDuration;
    }
  }
}
