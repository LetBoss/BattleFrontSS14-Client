// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Stealth.RMCPassiveStealthComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Whitelist;
using Robust.Shared.Analyzers;
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
namespace Content.Shared._RMC14.Stealth;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
public sealed class RMCPassiveStealthComponent : 
  Component,
  ISerializationGenerated<RMCPassiveStealthComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float MinOpacity = 0.2f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float MaxOpacity = 1f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool? Enabled;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan Delay = TimeSpan.Zero;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan UnCloakDelay = TimeSpan.Zero;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan ToggleTime;
  [DataField(null, false, 1, false, false, null)]
  public bool Toggleable;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntityWhitelist Whitelist = new EntityWhitelist();

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref RMCPassiveStealthComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (RMCPassiveStealthComponent) target1;
    if (serialization.TryCustomCopy<RMCPassiveStealthComponent>(this, ref target, hookCtx, false, context))
      return;
    float target2 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.MinOpacity, ref target2, hookCtx, false, context))
      target2 = this.MinOpacity;
    target.MinOpacity = target2;
    float target3 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.MaxOpacity, ref target3, hookCtx, false, context))
      target3 = this.MaxOpacity;
    target.MaxOpacity = target3;
    bool? target4 = new bool?();
    if (!serialization.TryCustomCopy<bool?>(this.Enabled, ref target4, hookCtx, false, context))
      target4 = this.Enabled;
    target.Enabled = target4;
    TimeSpan target5 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.Delay, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<TimeSpan>(this.Delay, hookCtx, context);
    target.Delay = target5;
    TimeSpan target6 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.UnCloakDelay, ref target6, hookCtx, false, context))
      target6 = serialization.CreateCopy<TimeSpan>(this.UnCloakDelay, hookCtx, context);
    target.UnCloakDelay = target6;
    TimeSpan target7 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.ToggleTime, ref target7, hookCtx, false, context))
      target7 = serialization.CreateCopy<TimeSpan>(this.ToggleTime, hookCtx, context);
    target.ToggleTime = target7;
    bool target8 = false;
    if (!serialization.TryCustomCopy<bool>(this.Toggleable, ref target8, hookCtx, false, context))
      target8 = this.Toggleable;
    target.Toggleable = target8;
    EntityWhitelist target9 = (EntityWhitelist) null;
    if (this.Whitelist == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<EntityWhitelist>(this.Whitelist, ref target9, hookCtx, false, context))
    {
      if (this.Whitelist == null)
        target9 = (EntityWhitelist) null;
      else
        serialization.CopyTo<EntityWhitelist>(this.Whitelist, ref target9, hookCtx, context, true);
    }
    target.Whitelist = target9;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref RMCPassiveStealthComponent target,
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
    RMCPassiveStealthComponent target1 = (RMCPassiveStealthComponent) target;
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
    RMCPassiveStealthComponent target1 = (RMCPassiveStealthComponent) target;
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
    RMCPassiveStealthComponent target1 = (RMCPassiveStealthComponent) target;
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
  virtual RMCPassiveStealthComponent Component.Instantiate() => new RMCPassiveStealthComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class RMCPassiveStealthComponent_AutoState : IComponentState
  {
    public float MinOpacity;
    public float MaxOpacity;
    public bool? Enabled;
    public TimeSpan Delay;
    public TimeSpan UnCloakDelay;
    public TimeSpan ToggleTime;
    public EntityWhitelist Whitelist;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class RMCPassiveStealthComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<RMCPassiveStealthComponent, ComponentGetState>(new ComponentEventRefHandler<RMCPassiveStealthComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<RMCPassiveStealthComponent, ComponentHandleState>(new ComponentEventRefHandler<RMCPassiveStealthComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      RMCPassiveStealthComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new RMCPassiveStealthComponent.RMCPassiveStealthComponent_AutoState()
      {
        MinOpacity = component.MinOpacity,
        MaxOpacity = component.MaxOpacity,
        Enabled = component.Enabled,
        Delay = component.Delay,
        UnCloakDelay = component.UnCloakDelay,
        ToggleTime = component.ToggleTime,
        Whitelist = component.Whitelist
      };
    }

    private void OnHandleState(
      EntityUid uid,
      RMCPassiveStealthComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is RMCPassiveStealthComponent.RMCPassiveStealthComponent_AutoState current))
        return;
      component.MinOpacity = current.MinOpacity;
      component.MaxOpacity = current.MaxOpacity;
      component.Enabled = current.Enabled;
      component.Delay = current.Delay;
      component.UnCloakDelay = current.UnCloakDelay;
      component.ToggleTime = current.ToggleTime;
      component.Whitelist = current.Whitelist;
    }
  }
}
