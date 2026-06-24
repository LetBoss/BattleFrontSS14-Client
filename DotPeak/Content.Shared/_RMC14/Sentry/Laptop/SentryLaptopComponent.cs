// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Sentry.Laptop.SentryLaptopComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

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
namespace Content.Shared._RMC14.Sentry.Laptop;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (SharedSentryLaptopSystem)})]
public sealed class SentryLaptopComponent : 
  Component,
  ISerializationGenerated<SentryLaptopComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool IsOpen;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool IsPowered;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float Range = 20f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public HashSet<EntityUid> LinkedSentries = new HashSet<EntityUid>();
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int MaxLinkedSentries = 99;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public Dictionary<EntityUid, string> SentryCustomNames = new Dictionary<EntityUid, string>();
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public List<EntityUid> Watchers = new List<EntityUid>();
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntityUid? CurrentCamera;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref SentryLaptopComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (SentryLaptopComponent) target1;
    if (serialization.TryCustomCopy<SentryLaptopComponent>(this, ref target, hookCtx, false, context))
      return;
    bool target2 = false;
    if (!serialization.TryCustomCopy<bool>(this.IsOpen, ref target2, hookCtx, false, context))
      target2 = this.IsOpen;
    target.IsOpen = target2;
    bool target3 = false;
    if (!serialization.TryCustomCopy<bool>(this.IsPowered, ref target3, hookCtx, false, context))
      target3 = this.IsPowered;
    target.IsPowered = target3;
    float target4 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.Range, ref target4, hookCtx, false, context))
      target4 = this.Range;
    target.Range = target4;
    HashSet<EntityUid> target5 = (HashSet<EntityUid>) null;
    if (this.LinkedSentries == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<HashSet<EntityUid>>(this.LinkedSentries, ref target5, hookCtx, true, context))
      target5 = serialization.CreateCopy<HashSet<EntityUid>>(this.LinkedSentries, hookCtx, context);
    target.LinkedSentries = target5;
    int target6 = 0;
    if (!serialization.TryCustomCopy<int>(this.MaxLinkedSentries, ref target6, hookCtx, false, context))
      target6 = this.MaxLinkedSentries;
    target.MaxLinkedSentries = target6;
    Dictionary<EntityUid, string> target7 = (Dictionary<EntityUid, string>) null;
    if (this.SentryCustomNames == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<Dictionary<EntityUid, string>>(this.SentryCustomNames, ref target7, hookCtx, true, context))
      target7 = serialization.CreateCopy<Dictionary<EntityUid, string>>(this.SentryCustomNames, hookCtx, context);
    target.SentryCustomNames = target7;
    List<EntityUid> target8 = (List<EntityUid>) null;
    if (this.Watchers == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<List<EntityUid>>(this.Watchers, ref target8, hookCtx, true, context))
      target8 = serialization.CreateCopy<List<EntityUid>>(this.Watchers, hookCtx, context);
    target.Watchers = target8;
    EntityUid? target9 = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.CurrentCamera, ref target9, hookCtx, false, context))
      target9 = serialization.CreateCopy<EntityUid?>(this.CurrentCamera, hookCtx, context);
    target.CurrentCamera = target9;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref SentryLaptopComponent target,
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
    SentryLaptopComponent target1 = (SentryLaptopComponent) target;
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
    SentryLaptopComponent target1 = (SentryLaptopComponent) target;
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
    SentryLaptopComponent target1 = (SentryLaptopComponent) target;
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
  virtual SentryLaptopComponent Component.Instantiate() => new SentryLaptopComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class SentryLaptopComponent_AutoState : IComponentState
  {
    public bool IsOpen;
    public bool IsPowered;
    public float Range;
    public HashSet<NetEntity> LinkedSentries;
    public int MaxLinkedSentries;
    public Dictionary<NetEntity, string> SentryCustomNames;
    public List<NetEntity> Watchers;
    public NetEntity? CurrentCamera;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class SentryLaptopComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<SentryLaptopComponent, ComponentGetState>(new ComponentEventRefHandler<SentryLaptopComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<SentryLaptopComponent, ComponentHandleState>(new ComponentEventRefHandler<SentryLaptopComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      SentryLaptopComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new SentryLaptopComponent.SentryLaptopComponent_AutoState()
      {
        IsOpen = component.IsOpen,
        IsPowered = component.IsPowered,
        Range = component.Range,
        LinkedSentries = this.GetNetEntitySet(component.LinkedSentries),
        MaxLinkedSentries = component.MaxLinkedSentries,
        SentryCustomNames = this.GetNetEntityDictionary<string>(component.SentryCustomNames),
        Watchers = this.GetNetEntityList(component.Watchers),
        CurrentCamera = this.GetNetEntity(component.CurrentCamera)
      };
    }

    private void OnHandleState(
      EntityUid uid,
      SentryLaptopComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is SentryLaptopComponent.SentryLaptopComponent_AutoState current))
        return;
      component.IsOpen = current.IsOpen;
      component.IsPowered = current.IsPowered;
      component.Range = current.Range;
      this.EnsureEntitySet<SentryLaptopComponent>(current.LinkedSentries, uid, component.LinkedSentries);
      component.MaxLinkedSentries = current.MaxLinkedSentries;
      this.EnsureEntityDictionary<SentryLaptopComponent, string>(current.SentryCustomNames, uid, component.SentryCustomNames);
      this.EnsureEntityList<SentryLaptopComponent>(current.Watchers, uid, component.Watchers);
      component.CurrentCamera = this.EnsureEntity<SentryLaptopComponent>(current.CurrentCamera, uid);
    }
  }
}
