// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Pulling.PreventPulledWhileAliveComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.StatusEffect;
using Content.Shared.Whitelist;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Pulling;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (RMCPullingSystem)})]
public sealed class PreventPulledWhileAliveComponent : 
  Component,
  ISerializationGenerated<PreventPulledWhileAliveComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntityWhitelist? Whitelist;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public HashSet<ProtoId<StatusEffectPrototype>> ExceptEffects = new HashSet<ProtoId<StatusEffectPrototype>>();

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref PreventPulledWhileAliveComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (PreventPulledWhileAliveComponent) target1;
    if (serialization.TryCustomCopy<PreventPulledWhileAliveComponent>(this, ref target, hookCtx, false, context))
      return;
    EntityWhitelist target2 = (EntityWhitelist) null;
    if (!serialization.TryCustomCopy<EntityWhitelist>(this.Whitelist, ref target2, hookCtx, false, context))
    {
      if (this.Whitelist == null)
        target2 = (EntityWhitelist) null;
      else
        serialization.CopyTo<EntityWhitelist>(this.Whitelist, ref target2, hookCtx, context);
    }
    target.Whitelist = target2;
    HashSet<ProtoId<StatusEffectPrototype>> target3 = (HashSet<ProtoId<StatusEffectPrototype>>) null;
    if (this.ExceptEffects == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<HashSet<ProtoId<StatusEffectPrototype>>>(this.ExceptEffects, ref target3, hookCtx, true, context))
      target3 = serialization.CreateCopy<HashSet<ProtoId<StatusEffectPrototype>>>(this.ExceptEffects, hookCtx, context);
    target.ExceptEffects = target3;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref PreventPulledWhileAliveComponent target,
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
    PreventPulledWhileAliveComponent target1 = (PreventPulledWhileAliveComponent) target;
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
    PreventPulledWhileAliveComponent target1 = (PreventPulledWhileAliveComponent) target;
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
    PreventPulledWhileAliveComponent target1 = (PreventPulledWhileAliveComponent) target;
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
  virtual PreventPulledWhileAliveComponent Component.Instantiate()
  {
    return new PreventPulledWhileAliveComponent();
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class PreventPulledWhileAliveComponent_AutoState : IComponentState
  {
    public EntityWhitelist? Whitelist;
    public HashSet<ProtoId<StatusEffectPrototype>> ExceptEffects;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class PreventPulledWhileAliveComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<PreventPulledWhileAliveComponent, ComponentGetState>(new ComponentEventRefHandler<PreventPulledWhileAliveComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<PreventPulledWhileAliveComponent, ComponentHandleState>(new ComponentEventRefHandler<PreventPulledWhileAliveComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      PreventPulledWhileAliveComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new PreventPulledWhileAliveComponent.PreventPulledWhileAliveComponent_AutoState()
      {
        Whitelist = component.Whitelist,
        ExceptEffects = component.ExceptEffects
      };
    }

    private void OnHandleState(
      EntityUid uid,
      PreventPulledWhileAliveComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is PreventPulledWhileAliveComponent.PreventPulledWhileAliveComponent_AutoState current))
        return;
      component.Whitelist = current.Whitelist;
      component.ExceptEffects = current.ExceptEffects == null ? (HashSet<ProtoId<StatusEffectPrototype>>) null : new HashSet<ProtoId<StatusEffectPrototype>>((IEnumerable<ProtoId<StatusEffectPrototype>>) current.ExceptEffects);
    }
  }
}
