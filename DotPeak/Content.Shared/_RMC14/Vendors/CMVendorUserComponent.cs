// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Vendors.CMVendorUserComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Roles;
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
namespace Content.Shared._RMC14.Vendors;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (SharedCMAutomatedVendorSystem)})]
public sealed class CMVendorUserComponent : 
  Component,
  ISerializationGenerated<CMVendorUserComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public ProtoId<JobPrototype>? Id;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public Dictionary<string, int> Choices = new Dictionary<string, int>();
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public HashSet<(string Category, EntProtoId Ent)> TakeAll = new HashSet<(string, EntProtoId)>();
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public HashSet<string> TakeOne = new HashSet<string>();
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int Points;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public Dictionary<string, int>? ExtraPoints;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref CMVendorUserComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (CMVendorUserComponent) target1;
    if (serialization.TryCustomCopy<CMVendorUserComponent>(this, ref target, hookCtx, false, context))
      return;
    ProtoId<JobPrototype>? target2 = new ProtoId<JobPrototype>?();
    if (!serialization.TryCustomCopy<ProtoId<JobPrototype>?>(this.Id, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<ProtoId<JobPrototype>?>(this.Id, hookCtx, context);
    target.Id = target2;
    Dictionary<string, int> target3 = (Dictionary<string, int>) null;
    if (this.Choices == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<Dictionary<string, int>>(this.Choices, ref target3, hookCtx, true, context))
      target3 = serialization.CreateCopy<Dictionary<string, int>>(this.Choices, hookCtx, context);
    target.Choices = target3;
    HashSet<(string, EntProtoId)> target4 = (HashSet<(string, EntProtoId)>) null;
    if (this.TakeAll == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<HashSet<(string, EntProtoId)>>(this.TakeAll, ref target4, hookCtx, true, context))
      target4 = serialization.CreateCopy<HashSet<(string, EntProtoId)>>(this.TakeAll, hookCtx, context);
    target.TakeAll = target4;
    HashSet<string> target5 = (HashSet<string>) null;
    if (this.TakeOne == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<HashSet<string>>(this.TakeOne, ref target5, hookCtx, true, context))
      target5 = serialization.CreateCopy<HashSet<string>>(this.TakeOne, hookCtx, context);
    target.TakeOne = target5;
    int target6 = 0;
    if (!serialization.TryCustomCopy<int>(this.Points, ref target6, hookCtx, false, context))
      target6 = this.Points;
    target.Points = target6;
    Dictionary<string, int> target7 = (Dictionary<string, int>) null;
    if (!serialization.TryCustomCopy<Dictionary<string, int>>(this.ExtraPoints, ref target7, hookCtx, true, context))
      target7 = serialization.CreateCopy<Dictionary<string, int>>(this.ExtraPoints, hookCtx, context);
    target.ExtraPoints = target7;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref CMVendorUserComponent target,
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
    CMVendorUserComponent target1 = (CMVendorUserComponent) target;
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
    CMVendorUserComponent target1 = (CMVendorUserComponent) target;
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
    CMVendorUserComponent target1 = (CMVendorUserComponent) target;
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
  virtual CMVendorUserComponent Component.Instantiate() => new CMVendorUserComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class CMVendorUserComponent_AutoState : IComponentState
  {
    public ProtoId<JobPrototype>? Id;
    public Dictionary<string, int> Choices;
    public HashSet<(string Category, EntProtoId Ent)> TakeAll;
    public HashSet<string> TakeOne;
    public int Points;
    public Dictionary<string, int>? ExtraPoints;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class CMVendorUserComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<CMVendorUserComponent, ComponentGetState>(new ComponentEventRefHandler<CMVendorUserComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<CMVendorUserComponent, ComponentHandleState>(new ComponentEventRefHandler<CMVendorUserComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      CMVendorUserComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new CMVendorUserComponent.CMVendorUserComponent_AutoState()
      {
        Id = component.Id,
        Choices = component.Choices,
        TakeAll = component.TakeAll,
        TakeOne = component.TakeOne,
        Points = component.Points,
        ExtraPoints = component.ExtraPoints
      };
    }

    private void OnHandleState(
      EntityUid uid,
      CMVendorUserComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is CMVendorUserComponent.CMVendorUserComponent_AutoState current))
        return;
      component.Id = current.Id;
      component.Choices = current.Choices == null ? (Dictionary<string, int>) null : new Dictionary<string, int>((IDictionary<string, int>) current.Choices);
      component.TakeAll = current.TakeAll == null ? (HashSet<(string, EntProtoId)>) null : new HashSet<(string, EntProtoId)>((IEnumerable<(string, EntProtoId)>) current.TakeAll);
      component.TakeOne = current.TakeOne == null ? (HashSet<string>) null : new HashSet<string>((IEnumerable<string>) current.TakeOne);
      component.Points = current.Points;
      component.ExtraPoints = current.ExtraPoints == null ? (Dictionary<string, int>) null : new Dictionary<string, int>((IDictionary<string, int>) current.ExtraPoints);
    }
  }
}
