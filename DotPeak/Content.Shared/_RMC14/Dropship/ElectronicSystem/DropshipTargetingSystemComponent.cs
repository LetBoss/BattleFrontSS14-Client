// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Dropship.ElectronicSystem.DropshipTargetingSystemComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

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
namespace Content.Shared._RMC14.Dropship.ElectronicSystem;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
public sealed class DropshipTargetingSystemComponent : 
  Component,
  ISerializationGenerated<DropshipTargetingSystemComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int SpreadModifier = -2;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int BulletSpreadModifier = -3;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan TravelingTimeModifier = TimeSpan.FromSeconds(-2L);

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref DropshipTargetingSystemComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (DropshipTargetingSystemComponent) target1;
    if (serialization.TryCustomCopy<DropshipTargetingSystemComponent>(this, ref target, hookCtx, false, context))
      return;
    int target2 = 0;
    if (!serialization.TryCustomCopy<int>(this.SpreadModifier, ref target2, hookCtx, false, context))
      target2 = this.SpreadModifier;
    target.SpreadModifier = target2;
    int target3 = 0;
    if (!serialization.TryCustomCopy<int>(this.BulletSpreadModifier, ref target3, hookCtx, false, context))
      target3 = this.BulletSpreadModifier;
    target.BulletSpreadModifier = target3;
    TimeSpan target4 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.TravelingTimeModifier, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<TimeSpan>(this.TravelingTimeModifier, hookCtx, context);
    target.TravelingTimeModifier = target4;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref DropshipTargetingSystemComponent target,
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
    DropshipTargetingSystemComponent target1 = (DropshipTargetingSystemComponent) target;
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
    DropshipTargetingSystemComponent target1 = (DropshipTargetingSystemComponent) target;
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
    DropshipTargetingSystemComponent target1 = (DropshipTargetingSystemComponent) target;
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
  virtual DropshipTargetingSystemComponent Component.Instantiate()
  {
    return new DropshipTargetingSystemComponent();
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class DropshipTargetingSystemComponent_AutoState : IComponentState
  {
    public int SpreadModifier;
    public int BulletSpreadModifier;
    public TimeSpan TravelingTimeModifier;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class DropshipTargetingSystemComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<DropshipTargetingSystemComponent, ComponentGetState>(new ComponentEventRefHandler<DropshipTargetingSystemComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<DropshipTargetingSystemComponent, ComponentHandleState>(new ComponentEventRefHandler<DropshipTargetingSystemComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      DropshipTargetingSystemComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new DropshipTargetingSystemComponent.DropshipTargetingSystemComponent_AutoState()
      {
        SpreadModifier = component.SpreadModifier,
        BulletSpreadModifier = component.BulletSpreadModifier,
        TravelingTimeModifier = component.TravelingTimeModifier
      };
    }

    private void OnHandleState(
      EntityUid uid,
      DropshipTargetingSystemComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is DropshipTargetingSystemComponent.DropshipTargetingSystemComponent_AutoState current))
        return;
      component.SpreadModifier = current.SpreadModifier;
      component.BulletSpreadModifier = current.BulletSpreadModifier;
      component.TravelingTimeModifier = current.TravelingTimeModifier;
    }
  }
}
