// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Evasion.EvasionComponent
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
namespace Content.Shared._RMC14.Evasion;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (EvasionSystem)})]
public sealed class EvasionComponent : 
  Component,
  ISerializationGenerated<EvasionComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public FixedPoint2 Evasion = (FixedPoint2) 0;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public FixedPoint2 ModifiedEvasion = (FixedPoint2) 0;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public FixedPoint2 EvasionFriendly = (FixedPoint2) 15;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public FixedPoint2 ModifiedEvasionFriendly = (FixedPoint2) 0;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref EvasionComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (EvasionComponent) target1;
    if (serialization.TryCustomCopy<EvasionComponent>(this, ref target, hookCtx, false, context))
      return;
    FixedPoint2 target2 = new FixedPoint2();
    if (!serialization.TryCustomCopy<FixedPoint2>(this.Evasion, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<FixedPoint2>(this.Evasion, hookCtx, context);
    target.Evasion = target2;
    FixedPoint2 target3 = new FixedPoint2();
    if (!serialization.TryCustomCopy<FixedPoint2>(this.ModifiedEvasion, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<FixedPoint2>(this.ModifiedEvasion, hookCtx, context);
    target.ModifiedEvasion = target3;
    FixedPoint2 target4 = new FixedPoint2();
    if (!serialization.TryCustomCopy<FixedPoint2>(this.EvasionFriendly, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<FixedPoint2>(this.EvasionFriendly, hookCtx, context);
    target.EvasionFriendly = target4;
    FixedPoint2 target5 = new FixedPoint2();
    if (!serialization.TryCustomCopy<FixedPoint2>(this.ModifiedEvasionFriendly, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<FixedPoint2>(this.ModifiedEvasionFriendly, hookCtx, context);
    target.ModifiedEvasionFriendly = target5;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref EvasionComponent target,
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
    EvasionComponent target1 = (EvasionComponent) target;
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
    EvasionComponent target1 = (EvasionComponent) target;
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
    EvasionComponent target1 = (EvasionComponent) target;
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
  virtual EvasionComponent Component.Instantiate() => new EvasionComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class EvasionComponent_AutoState : IComponentState
  {
    public FixedPoint2 Evasion;
    public FixedPoint2 ModifiedEvasion;
    public FixedPoint2 EvasionFriendly;
    public FixedPoint2 ModifiedEvasionFriendly;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class EvasionComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<EvasionComponent, ComponentGetState>(new ComponentEventRefHandler<EvasionComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<EvasionComponent, ComponentHandleState>(new ComponentEventRefHandler<EvasionComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(EntityUid uid, EvasionComponent component, ref ComponentGetState args)
    {
      args.State = (IComponentState) new EvasionComponent.EvasionComponent_AutoState()
      {
        Evasion = component.Evasion,
        ModifiedEvasion = component.ModifiedEvasion,
        EvasionFriendly = component.EvasionFriendly,
        ModifiedEvasionFriendly = component.ModifiedEvasionFriendly
      };
    }

    private void OnHandleState(
      EntityUid uid,
      EvasionComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is EvasionComponent.EvasionComponent_AutoState current))
        return;
      component.Evasion = current.Evasion;
      component.ModifiedEvasion = current.ModifiedEvasion;
      component.EvasionFriendly = current.EvasionFriendly;
      component.ModifiedEvasionFriendly = current.ModifiedEvasionFriendly;
    }
  }
}
