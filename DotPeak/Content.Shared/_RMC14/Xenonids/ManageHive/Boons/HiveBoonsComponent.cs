// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.ManageHive.Boons.HiveBoonsComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

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
namespace Content.Shared._RMC14.Xenonids.ManageHive.Boons;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (HiveBoonSystem)})]
public sealed class HiveBoonsComponent : 
  Component,
  ISerializationGenerated<HiveBoonsComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int RoyalResin;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int RoyalResinMax = 10;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public Dictionary<EntProtoId<HiveBoonDefinitionComponent>, TimeSpan> UnlockAt = new Dictionary<EntProtoId<HiveBoonDefinitionComponent>, TimeSpan>();
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public Dictionary<EntProtoId<HiveBoonDefinitionComponent>, TimeSpan> UsedAt = new Dictionary<EntProtoId<HiveBoonDefinitionComponent>, TimeSpan>();
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public Dictionary<EntProtoId<HiveBoonDefinitionComponent>, EntityUid> Active = new Dictionary<EntProtoId<HiveBoonDefinitionComponent>, EntityUid>();
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool KingAnnounced;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref HiveBoonsComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (HiveBoonsComponent) target1;
    if (serialization.TryCustomCopy<HiveBoonsComponent>(this, ref target, hookCtx, false, context))
      return;
    int target2 = 0;
    if (!serialization.TryCustomCopy<int>(this.RoyalResin, ref target2, hookCtx, false, context))
      target2 = this.RoyalResin;
    target.RoyalResin = target2;
    int target3 = 0;
    if (!serialization.TryCustomCopy<int>(this.RoyalResinMax, ref target3, hookCtx, false, context))
      target3 = this.RoyalResinMax;
    target.RoyalResinMax = target3;
    Dictionary<EntProtoId<HiveBoonDefinitionComponent>, TimeSpan> target4 = (Dictionary<EntProtoId<HiveBoonDefinitionComponent>, TimeSpan>) null;
    if (this.UnlockAt == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<Dictionary<EntProtoId<HiveBoonDefinitionComponent>, TimeSpan>>(this.UnlockAt, ref target4, hookCtx, true, context))
      target4 = serialization.CreateCopy<Dictionary<EntProtoId<HiveBoonDefinitionComponent>, TimeSpan>>(this.UnlockAt, hookCtx, context);
    target.UnlockAt = target4;
    Dictionary<EntProtoId<HiveBoonDefinitionComponent>, TimeSpan> target5 = (Dictionary<EntProtoId<HiveBoonDefinitionComponent>, TimeSpan>) null;
    if (this.UsedAt == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<Dictionary<EntProtoId<HiveBoonDefinitionComponent>, TimeSpan>>(this.UsedAt, ref target5, hookCtx, true, context))
      target5 = serialization.CreateCopy<Dictionary<EntProtoId<HiveBoonDefinitionComponent>, TimeSpan>>(this.UsedAt, hookCtx, context);
    target.UsedAt = target5;
    Dictionary<EntProtoId<HiveBoonDefinitionComponent>, EntityUid> target6 = (Dictionary<EntProtoId<HiveBoonDefinitionComponent>, EntityUid>) null;
    if (this.Active == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<Dictionary<EntProtoId<HiveBoonDefinitionComponent>, EntityUid>>(this.Active, ref target6, hookCtx, true, context))
      target6 = serialization.CreateCopy<Dictionary<EntProtoId<HiveBoonDefinitionComponent>, EntityUid>>(this.Active, hookCtx, context);
    target.Active = target6;
    bool target7 = false;
    if (!serialization.TryCustomCopy<bool>(this.KingAnnounced, ref target7, hookCtx, false, context))
      target7 = this.KingAnnounced;
    target.KingAnnounced = target7;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref HiveBoonsComponent target,
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
    HiveBoonsComponent target1 = (HiveBoonsComponent) target;
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
    HiveBoonsComponent target1 = (HiveBoonsComponent) target;
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
    HiveBoonsComponent target1 = (HiveBoonsComponent) target;
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
  virtual HiveBoonsComponent Component.Instantiate() => new HiveBoonsComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class HiveBoonsComponent_AutoState : IComponentState
  {
    public int RoyalResin;
    public int RoyalResinMax;
    public Dictionary<EntProtoId<HiveBoonDefinitionComponent>, TimeSpan> UnlockAt;
    public Dictionary<EntProtoId<HiveBoonDefinitionComponent>, TimeSpan> UsedAt;
    public Dictionary<EntProtoId<HiveBoonDefinitionComponent>, NetEntity> Active;
    public bool KingAnnounced;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class HiveBoonsComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<HiveBoonsComponent, ComponentGetState>(new ComponentEventRefHandler<HiveBoonsComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<HiveBoonsComponent, ComponentHandleState>(new ComponentEventRefHandler<HiveBoonsComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      HiveBoonsComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new HiveBoonsComponent.HiveBoonsComponent_AutoState()
      {
        RoyalResin = component.RoyalResin,
        RoyalResinMax = component.RoyalResinMax,
        UnlockAt = component.UnlockAt,
        UsedAt = component.UsedAt,
        Active = this.GetNetEntityDictionary<EntProtoId<HiveBoonDefinitionComponent>>(component.Active),
        KingAnnounced = component.KingAnnounced
      };
    }

    private void OnHandleState(
      EntityUid uid,
      HiveBoonsComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is HiveBoonsComponent.HiveBoonsComponent_AutoState current))
        return;
      component.RoyalResin = current.RoyalResin;
      component.RoyalResinMax = current.RoyalResinMax;
      component.UnlockAt = current.UnlockAt == null ? (Dictionary<EntProtoId<HiveBoonDefinitionComponent>, TimeSpan>) null : new Dictionary<EntProtoId<HiveBoonDefinitionComponent>, TimeSpan>((IDictionary<EntProtoId<HiveBoonDefinitionComponent>, TimeSpan>) current.UnlockAt);
      component.UsedAt = current.UsedAt == null ? (Dictionary<EntProtoId<HiveBoonDefinitionComponent>, TimeSpan>) null : new Dictionary<EntProtoId<HiveBoonDefinitionComponent>, TimeSpan>((IDictionary<EntProtoId<HiveBoonDefinitionComponent>, TimeSpan>) current.UsedAt);
      this.EnsureEntityDictionary<HiveBoonsComponent, EntProtoId<HiveBoonDefinitionComponent>>(current.Active, uid, component.Active);
      component.KingAnnounced = current.KingAnnounced;
    }
  }
}
