// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Anchor.DeployableItemComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.FixedPoint;
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
namespace Content.Shared._RMC14.Anchor;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (DeployableItemSystem)})]
public sealed class DeployableItemComponent : 
  Component,
  ISerializationGenerated<DeployableItemComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public DeployableItemPosition Position;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public FixedPoint2 AlmostEmptyThreshold = FixedPoint2.New(0.33);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public FixedPoint2 HalfFullThreshold = FixedPoint2.New(0.66);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool LeftClickPickup;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool MagazineExamine;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref DeployableItemComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (DeployableItemComponent) target1;
    if (serialization.TryCustomCopy<DeployableItemComponent>(this, ref target, hookCtx, false, context))
      return;
    DeployableItemPosition target2 = DeployableItemPosition.None;
    if (!serialization.TryCustomCopy<DeployableItemPosition>(this.Position, ref target2, hookCtx, false, context))
      target2 = this.Position;
    target.Position = target2;
    FixedPoint2 target3 = new FixedPoint2();
    if (!serialization.TryCustomCopy<FixedPoint2>(this.AlmostEmptyThreshold, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<FixedPoint2>(this.AlmostEmptyThreshold, hookCtx, context);
    target.AlmostEmptyThreshold = target3;
    FixedPoint2 target4 = new FixedPoint2();
    if (!serialization.TryCustomCopy<FixedPoint2>(this.HalfFullThreshold, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<FixedPoint2>(this.HalfFullThreshold, hookCtx, context);
    target.HalfFullThreshold = target4;
    bool target5 = false;
    if (!serialization.TryCustomCopy<bool>(this.LeftClickPickup, ref target5, hookCtx, false, context))
      target5 = this.LeftClickPickup;
    target.LeftClickPickup = target5;
    bool target6 = false;
    if (!serialization.TryCustomCopy<bool>(this.MagazineExamine, ref target6, hookCtx, false, context))
      target6 = this.MagazineExamine;
    target.MagazineExamine = target6;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref DeployableItemComponent target,
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
    DeployableItemComponent target1 = (DeployableItemComponent) target;
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
    DeployableItemComponent target1 = (DeployableItemComponent) target;
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
    DeployableItemComponent target1 = (DeployableItemComponent) target;
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
  virtual DeployableItemComponent Component.Instantiate() => new DeployableItemComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class DeployableItemComponent_AutoState : IComponentState
  {
    public DeployableItemPosition Position;
    public FixedPoint2 AlmostEmptyThreshold;
    public FixedPoint2 HalfFullThreshold;
    public bool LeftClickPickup;
    public bool MagazineExamine;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class DeployableItemComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<DeployableItemComponent, ComponentGetState>(new ComponentEventRefHandler<DeployableItemComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<DeployableItemComponent, ComponentHandleState>(new ComponentEventRefHandler<DeployableItemComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      DeployableItemComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new DeployableItemComponent.DeployableItemComponent_AutoState()
      {
        Position = component.Position,
        AlmostEmptyThreshold = component.AlmostEmptyThreshold,
        HalfFullThreshold = component.HalfFullThreshold,
        LeftClickPickup = component.LeftClickPickup,
        MagazineExamine = component.MagazineExamine
      };
    }

    private void OnHandleState(
      EntityUid uid,
      DeployableItemComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is DeployableItemComponent.DeployableItemComponent_AutoState current))
        return;
      component.Position = current.Position;
      component.AlmostEmptyThreshold = current.AlmostEmptyThreshold;
      component.HalfFullThreshold = current.HalfFullThreshold;
      component.LeftClickPickup = current.LeftClickPickup;
      component.MagazineExamine = current.MagazineExamine;
    }
  }
}
