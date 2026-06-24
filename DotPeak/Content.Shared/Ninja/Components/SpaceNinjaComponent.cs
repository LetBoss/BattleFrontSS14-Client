// Decompiled with JetBrains decompiler
// Type: Content.Shared.Ninja.Components.SpaceNinjaComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Alert;
using Content.Shared.Ninja.Systems;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Ninja.Components;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (SharedSpaceNinjaSystem)})]
public sealed class SpaceNinjaComponent : 
  Component,
  ISerializationGenerated<SpaceNinjaComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntityUid? Suit;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntityUid? Gloves;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntityUid? Katana;
  [DataField(null, false, 1, false, false, null)]
  public EntProtoId TerrorObjective = (EntProtoId) nameof (TerrorObjective);
  [DataField(null, false, 1, false, false, null)]
  public EntProtoId MassArrestObjective = (EntProtoId) nameof (MassArrestObjective);
  [DataField(null, false, 1, false, false, null)]
  public EntProtoId SpiderChargeObjective = (EntProtoId) nameof (SpiderChargeObjective);
  [DataField(null, false, 1, false, false, null)]
  public ProtoId<AlertPrototype> SuitPowerAlert = (ProtoId<AlertPrototype>) "SuitPower";

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref SpaceNinjaComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (SpaceNinjaComponent) target1;
    if (serialization.TryCustomCopy<SpaceNinjaComponent>(this, ref target, hookCtx, false, context))
      return;
    EntityUid? target2 = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.Suit, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<EntityUid?>(this.Suit, hookCtx, context);
    target.Suit = target2;
    EntityUid? target3 = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.Gloves, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<EntityUid?>(this.Gloves, hookCtx, context);
    target.Gloves = target3;
    EntityUid? target4 = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.Katana, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<EntityUid?>(this.Katana, hookCtx, context);
    target.Katana = target4;
    EntProtoId target5 = new EntProtoId();
    if (!serialization.TryCustomCopy<EntProtoId>(this.TerrorObjective, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<EntProtoId>(this.TerrorObjective, hookCtx, context);
    target.TerrorObjective = target5;
    EntProtoId target6 = new EntProtoId();
    if (!serialization.TryCustomCopy<EntProtoId>(this.MassArrestObjective, ref target6, hookCtx, false, context))
      target6 = serialization.CreateCopy<EntProtoId>(this.MassArrestObjective, hookCtx, context);
    target.MassArrestObjective = target6;
    EntProtoId target7 = new EntProtoId();
    if (!serialization.TryCustomCopy<EntProtoId>(this.SpiderChargeObjective, ref target7, hookCtx, false, context))
      target7 = serialization.CreateCopy<EntProtoId>(this.SpiderChargeObjective, hookCtx, context);
    target.SpiderChargeObjective = target7;
    ProtoId<AlertPrototype> target8 = new ProtoId<AlertPrototype>();
    if (!serialization.TryCustomCopy<ProtoId<AlertPrototype>>(this.SuitPowerAlert, ref target8, hookCtx, false, context))
      target8 = serialization.CreateCopy<ProtoId<AlertPrototype>>(this.SuitPowerAlert, hookCtx, context);
    target.SuitPowerAlert = target8;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref SpaceNinjaComponent target,
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
    SpaceNinjaComponent target1 = (SpaceNinjaComponent) target;
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
    SpaceNinjaComponent target1 = (SpaceNinjaComponent) target;
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
    SpaceNinjaComponent target1 = (SpaceNinjaComponent) target;
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
  virtual SpaceNinjaComponent Component.Instantiate() => new SpaceNinjaComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class SpaceNinjaComponent_AutoState : IComponentState
  {
    public NetEntity? Suit;
    public NetEntity? Gloves;
    public NetEntity? Katana;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class SpaceNinjaComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<SpaceNinjaComponent, ComponentGetState>(new ComponentEventRefHandler<SpaceNinjaComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<SpaceNinjaComponent, ComponentHandleState>(new ComponentEventRefHandler<SpaceNinjaComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      SpaceNinjaComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new SpaceNinjaComponent.SpaceNinjaComponent_AutoState()
      {
        Suit = this.GetNetEntity(component.Suit),
        Gloves = this.GetNetEntity(component.Gloves),
        Katana = this.GetNetEntity(component.Katana)
      };
    }

    private void OnHandleState(
      EntityUid uid,
      SpaceNinjaComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is SpaceNinjaComponent.SpaceNinjaComponent_AutoState current))
        return;
      component.Suit = this.EnsureEntity<SpaceNinjaComponent>(current.Suit, uid);
      component.Gloves = this.EnsureEntity<SpaceNinjaComponent>(current.Gloves, uid);
      component.Katana = this.EnsureEntity<SpaceNinjaComponent>(current.Katana, uid);
    }
  }
}
