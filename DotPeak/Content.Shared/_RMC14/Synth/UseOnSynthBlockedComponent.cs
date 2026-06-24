// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Synth.UseOnSynthBlockedComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Whitelist;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Localization;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Synth;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (SharedSynthSystem)})]
public sealed class UseOnSynthBlockedComponent : 
  Component,
  ISerializationGenerated<UseOnSynthBlockedComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public LocId Popup = (LocId) "rmc-species-synth-defib-attempt";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool Reversed;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntityWhitelist? Whitelist;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntityWhitelist? Blacklist;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref UseOnSynthBlockedComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (UseOnSynthBlockedComponent) target1;
    if (serialization.TryCustomCopy<UseOnSynthBlockedComponent>(this, ref target, hookCtx, false, context))
      return;
    LocId target2 = new LocId();
    if (!serialization.TryCustomCopy<LocId>(this.Popup, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<LocId>(this.Popup, hookCtx, context);
    target.Popup = target2;
    bool target3 = false;
    if (!serialization.TryCustomCopy<bool>(this.Reversed, ref target3, hookCtx, false, context))
      target3 = this.Reversed;
    target.Reversed = target3;
    EntityWhitelist target4 = (EntityWhitelist) null;
    if (!serialization.TryCustomCopy<EntityWhitelist>(this.Whitelist, ref target4, hookCtx, false, context))
    {
      if (this.Whitelist == null)
        target4 = (EntityWhitelist) null;
      else
        serialization.CopyTo<EntityWhitelist>(this.Whitelist, ref target4, hookCtx, context);
    }
    target.Whitelist = target4;
    EntityWhitelist target5 = (EntityWhitelist) null;
    if (!serialization.TryCustomCopy<EntityWhitelist>(this.Blacklist, ref target5, hookCtx, false, context))
    {
      if (this.Blacklist == null)
        target5 = (EntityWhitelist) null;
      else
        serialization.CopyTo<EntityWhitelist>(this.Blacklist, ref target5, hookCtx, context);
    }
    target.Blacklist = target5;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref UseOnSynthBlockedComponent target,
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
    UseOnSynthBlockedComponent target1 = (UseOnSynthBlockedComponent) target;
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
    UseOnSynthBlockedComponent target1 = (UseOnSynthBlockedComponent) target;
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
    UseOnSynthBlockedComponent target1 = (UseOnSynthBlockedComponent) target;
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
  virtual UseOnSynthBlockedComponent Component.Instantiate() => new UseOnSynthBlockedComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class UseOnSynthBlockedComponent_AutoState : IComponentState
  {
    public bool Reversed;
    public EntityWhitelist? Whitelist;
    public EntityWhitelist? Blacklist;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class UseOnSynthBlockedComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<UseOnSynthBlockedComponent, ComponentGetState>(new ComponentEventRefHandler<UseOnSynthBlockedComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<UseOnSynthBlockedComponent, ComponentHandleState>(new ComponentEventRefHandler<UseOnSynthBlockedComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      UseOnSynthBlockedComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new UseOnSynthBlockedComponent.UseOnSynthBlockedComponent_AutoState()
      {
        Reversed = component.Reversed,
        Whitelist = component.Whitelist,
        Blacklist = component.Blacklist
      };
    }

    private void OnHandleState(
      EntityUid uid,
      UseOnSynthBlockedComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is UseOnSynthBlockedComponent.UseOnSynthBlockedComponent_AutoState current))
        return;
      component.Reversed = current.Reversed;
      component.Whitelist = current.Whitelist;
      component.Blacklist = current.Blacklist;
    }
  }
}
