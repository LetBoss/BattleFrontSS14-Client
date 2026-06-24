// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Debilitate.RMCDebilitateComponent
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
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Debilitate;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (RMCDebilitateSystem)})]
public sealed class RMCDebilitateComponent : 
  Component,
  ISerializationGenerated<RMCDebilitateComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntityWhitelist? Blacklist;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntityWhitelist? Whitelist;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan Knockdown;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref RMCDebilitateComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (RMCDebilitateComponent) target1;
    if (serialization.TryCustomCopy<RMCDebilitateComponent>(this, ref target, hookCtx, false, context))
      return;
    EntityWhitelist target2 = (EntityWhitelist) null;
    if (!serialization.TryCustomCopy<EntityWhitelist>(this.Blacklist, ref target2, hookCtx, false, context))
    {
      if (this.Blacklist == null)
        target2 = (EntityWhitelist) null;
      else
        serialization.CopyTo<EntityWhitelist>(this.Blacklist, ref target2, hookCtx, context);
    }
    target.Blacklist = target2;
    EntityWhitelist target3 = (EntityWhitelist) null;
    if (!serialization.TryCustomCopy<EntityWhitelist>(this.Whitelist, ref target3, hookCtx, false, context))
    {
      if (this.Whitelist == null)
        target3 = (EntityWhitelist) null;
      else
        serialization.CopyTo<EntityWhitelist>(this.Whitelist, ref target3, hookCtx, context);
    }
    target.Whitelist = target3;
    TimeSpan target4 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.Knockdown, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<TimeSpan>(this.Knockdown, hookCtx, context);
    target.Knockdown = target4;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref RMCDebilitateComponent target,
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
    RMCDebilitateComponent target1 = (RMCDebilitateComponent) target;
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
    RMCDebilitateComponent target1 = (RMCDebilitateComponent) target;
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
    RMCDebilitateComponent target1 = (RMCDebilitateComponent) target;
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
  virtual RMCDebilitateComponent Component.Instantiate() => new RMCDebilitateComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class RMCDebilitateComponent_AutoState : IComponentState
  {
    public EntityWhitelist? Blacklist;
    public EntityWhitelist? Whitelist;
    public TimeSpan Knockdown;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class RMCDebilitateComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<RMCDebilitateComponent, ComponentGetState>(new ComponentEventRefHandler<RMCDebilitateComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<RMCDebilitateComponent, ComponentHandleState>(new ComponentEventRefHandler<RMCDebilitateComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      RMCDebilitateComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new RMCDebilitateComponent.RMCDebilitateComponent_AutoState()
      {
        Blacklist = component.Blacklist,
        Whitelist = component.Whitelist,
        Knockdown = component.Knockdown
      };
    }

    private void OnHandleState(
      EntityUid uid,
      RMCDebilitateComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is RMCDebilitateComponent.RMCDebilitateComponent_AutoState current))
        return;
      component.Blacklist = current.Blacklist;
      component.Whitelist = current.Whitelist;
      component.Knockdown = current.Knockdown;
    }
  }
}
