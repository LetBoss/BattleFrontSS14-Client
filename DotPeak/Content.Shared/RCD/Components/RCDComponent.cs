// Decompiled with JetBrains decompiler
// Type: Content.Shared.RCD.Components.RCDComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.RCD.Systems;
using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Maths;
using Robust.Shared.Physics;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.ViewVariables;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.RCD.Components;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (RCDSystem)})]
public sealed class RCDComponent : 
  Component,
  ISerializationGenerated<RCDComponent>,
  ISerializationGenerated
{
  private Direction _constructionDirection;

  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public HashSet<Robust.Shared.Prototypes.ProtoId<RCDPrototype>> AvailablePrototypes { get; set; } = new HashSet<Robust.Shared.Prototypes.ProtoId<RCDPrototype>>();

  [DataField(null, false, 1, false, false, null)]
  public SoundSpecifier SuccessSound { get; set; } = (SoundSpecifier) new SoundPathSpecifier("/Audio/Items/deconstruct.ogg");

  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public Robust.Shared.Prototypes.ProtoId<RCDPrototype> ProtoId { get; set; } = (Robust.Shared.Prototypes.ProtoId<RCDPrototype>) "Invalid";

  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public Direction ConstructionDirection
  {
    get => this._constructionDirection;
    set
    {
      this._constructionDirection = value;
      this.ConstructionTransform = new Transform(new Vector2(), DirectionExtensions.ToAngle(this._constructionDirection));
    }
  }

  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadOnly)]
  public Transform ConstructionTransform { get; private set; }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref RCDComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (RCDComponent) target1;
    if (serialization.TryCustomCopy<RCDComponent>(this, ref target, hookCtx, false, context))
      return;
    HashSet<Robust.Shared.Prototypes.ProtoId<RCDPrototype>> target2 = (HashSet<Robust.Shared.Prototypes.ProtoId<RCDPrototype>>) null;
    if (this.AvailablePrototypes == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<HashSet<Robust.Shared.Prototypes.ProtoId<RCDPrototype>>>(this.AvailablePrototypes, ref target2, hookCtx, true, context))
      target2 = serialization.CreateCopy<HashSet<Robust.Shared.Prototypes.ProtoId<RCDPrototype>>>(this.AvailablePrototypes, hookCtx, context);
    target.AvailablePrototypes = target2;
    SoundSpecifier target3 = (SoundSpecifier) null;
    if (this.SuccessSound == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.SuccessSound, ref target3, hookCtx, true, context))
      target3 = serialization.CreateCopy<SoundSpecifier>(this.SuccessSound, hookCtx, context);
    target.SuccessSound = target3;
    Robust.Shared.Prototypes.ProtoId<RCDPrototype> target4 = new Robust.Shared.Prototypes.ProtoId<RCDPrototype>();
    if (!serialization.TryCustomCopy<Robust.Shared.Prototypes.ProtoId<RCDPrototype>>(this.ProtoId, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<Robust.Shared.Prototypes.ProtoId<RCDPrototype>>(this.ProtoId, hookCtx, context);
    target.ProtoId = target4;
    Direction target5 = (Direction) 0;
    if (!serialization.TryCustomCopy<Direction>(this.ConstructionDirection, ref target5, hookCtx, false, context))
      target5 = this.ConstructionDirection;
    target.ConstructionDirection = target5;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref RCDComponent target,
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
    RCDComponent target1 = (RCDComponent) target;
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
    RCDComponent target1 = (RCDComponent) target;
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
    RCDComponent target1 = (RCDComponent) target;
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
  virtual RCDComponent Component.Instantiate() => new RCDComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class RCDComponent_AutoState : IComponentState
  {
    public HashSet<Robust.Shared.Prototypes.ProtoId<RCDPrototype>> AvailablePrototypes;
    public Robust.Shared.Prototypes.ProtoId<RCDPrototype> ProtoId;
    public Direction ConstructionDirection;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class RCDComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<RCDComponent, ComponentGetState>(new ComponentEventRefHandler<RCDComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<RCDComponent, ComponentHandleState>(new ComponentEventRefHandler<RCDComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(EntityUid uid, RCDComponent component, ref ComponentGetState args)
    {
      args.State = (IComponentState) new RCDComponent.RCDComponent_AutoState()
      {
        AvailablePrototypes = component.AvailablePrototypes,
        ProtoId = component.ProtoId,
        ConstructionDirection = component.ConstructionDirection
      };
    }

    private void OnHandleState(
      EntityUid uid,
      RCDComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is RCDComponent.RCDComponent_AutoState current))
        return;
      component.AvailablePrototypes = current.AvailablePrototypes == null ? (HashSet<Robust.Shared.Prototypes.ProtoId<RCDPrototype>>) null : new HashSet<Robust.Shared.Prototypes.ProtoId<RCDPrototype>>((IEnumerable<Robust.Shared.Prototypes.ProtoId<RCDPrototype>>) current.AvailablePrototypes);
      component.ProtoId = current.ProtoId;
      component.ConstructionDirection = current.ConstructionDirection;
    }
  }
}
