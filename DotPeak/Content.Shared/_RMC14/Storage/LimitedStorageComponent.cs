// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Storage.LimitedStorageComponent
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
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Storage;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (RMCStorageSystem)})]
public sealed class LimitedStorageComponent : 
  Component,
  ISerializationGenerated<LimitedStorageComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public List<LimitedStorageComponent.Limit> Limits = new List<LimitedStorageComponent.Limit>();

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref LimitedStorageComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (LimitedStorageComponent) target1;
    if (serialization.TryCustomCopy<LimitedStorageComponent>(this, ref target, hookCtx, false, context))
      return;
    List<LimitedStorageComponent.Limit> target2 = (List<LimitedStorageComponent.Limit>) null;
    if (this.Limits == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<List<LimitedStorageComponent.Limit>>(this.Limits, ref target2, hookCtx, true, context))
      target2 = serialization.CreateCopy<List<LimitedStorageComponent.Limit>>(this.Limits, hookCtx, context);
    target.Limits = target2;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref LimitedStorageComponent target,
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
    LimitedStorageComponent target1 = (LimitedStorageComponent) target;
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
    LimitedStorageComponent target1 = (LimitedStorageComponent) target;
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
    LimitedStorageComponent target1 = (LimitedStorageComponent) target;
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
  virtual LimitedStorageComponent Component.Instantiate() => new LimitedStorageComponent();

  [DataDefinition]
  [NetSerializable]
  [Serializable]
  public struct Limit : 
    ISerializationGenerated<LimitedStorageComponent.Limit>,
    ISerializationGenerated
  {
    [DataField(null, false, 1, false, false, null)]
    public int Count;
    [DataField(null, false, 1, false, false, null)]
    public EntityWhitelist? Blacklist;
    [DataField(null, false, 1, true, false, null)]
    public EntityWhitelist? Whitelist;
    [DataField(null, false, 1, true, false, null)]
    public LocId Popup;

    public Limit()
    {
      this.Popup = new LocId();
      this.Count = 1;
      this.Blacklist = new EntityWhitelist();
      this.Whitelist = new EntityWhitelist();
    }

    [Obsolete("Use ISerializationManager.CopyTo instead")]
    public void InternalCopy(
      ref LimitedStorageComponent.Limit target,
      ISerializationManager serialization,
      SerializationHookContext hookCtx,
      ISerializationContext? context = null)
    {
      if (serialization.TryCustomCopy<LimitedStorageComponent.Limit>(this, ref target, hookCtx, false, context))
        return;
      int target1 = 0;
      if (!serialization.TryCustomCopy<int>(this.Count, ref target1, hookCtx, false, context))
        target1 = this.Count;
      EntityWhitelist target2 = (EntityWhitelist) null;
      if (!serialization.TryCustomCopy<EntityWhitelist>(this.Blacklist, ref target2, hookCtx, false, context))
      {
        if (this.Blacklist == null)
          target2 = (EntityWhitelist) null;
        else
          serialization.CopyTo<EntityWhitelist>(this.Blacklist, ref target2, hookCtx, context);
      }
      EntityWhitelist target3 = (EntityWhitelist) null;
      if (!serialization.TryCustomCopy<EntityWhitelist>(this.Whitelist, ref target3, hookCtx, false, context))
      {
        if (this.Whitelist == null)
          target3 = (EntityWhitelist) null;
        else
          serialization.CopyTo<EntityWhitelist>(this.Whitelist, ref target3, hookCtx, context);
      }
      LocId target4 = new LocId();
      if (!serialization.TryCustomCopy<LocId>(this.Popup, ref target4, hookCtx, false, context))
        target4 = serialization.CreateCopy<LocId>(this.Popup, hookCtx, context);
      target = target with
      {
        Count = target1,
        Blacklist = target2,
        Whitelist = target3,
        Popup = target4
      };
    }

    [Obsolete("Use ISerializationManager.CopyTo instead")]
    public void Copy(
      ref LimitedStorageComponent.Limit target,
      ISerializationManager serialization,
      SerializationHookContext hookCtx,
      ISerializationContext? context = null)
    {
      this.InternalCopy(ref target, serialization, hookCtx, context);
    }

    [Obsolete("Use ISerializationManager.CopyTo instead")]
    public void Copy(
      ref object target,
      ISerializationManager serialization,
      SerializationHookContext hookCtx,
      ISerializationContext? context = null)
    {
      LimitedStorageComponent.Limit target1 = (LimitedStorageComponent.Limit) target;
      this.Copy(ref target1, serialization, hookCtx, context);
      target = (object) target1;
    }

    [Obsolete("Use ISerializationManager.CreateCopy instead")]
    public LimitedStorageComponent.Limit Instantiate() => new LimitedStorageComponent.Limit();
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class LimitedStorageComponent_AutoState : IComponentState
  {
    public List<LimitedStorageComponent.Limit> Limits;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class LimitedStorageComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<LimitedStorageComponent, ComponentGetState>(new ComponentEventRefHandler<LimitedStorageComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<LimitedStorageComponent, ComponentHandleState>(new ComponentEventRefHandler<LimitedStorageComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      LimitedStorageComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new LimitedStorageComponent.LimitedStorageComponent_AutoState()
      {
        Limits = component.Limits
      };
    }

    private void OnHandleState(
      EntityUid uid,
      LimitedStorageComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is LimitedStorageComponent.LimitedStorageComponent_AutoState current))
        return;
      component.Limits = current.Limits == null ? (List<LimitedStorageComponent.Limit>) null : new List<LimitedStorageComponent.Limit>((IEnumerable<LimitedStorageComponent.Limit>) current.Limits);
    }
  }
}
