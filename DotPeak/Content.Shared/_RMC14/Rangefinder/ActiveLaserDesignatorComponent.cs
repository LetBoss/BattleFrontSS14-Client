// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Rangefinder.ActiveLaserDesignatorComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Map;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Rangefinder;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (RangefinderSystem)})]
public sealed class ActiveLaserDesignatorComponent : 
  Component,
  ISerializationGenerated<ActiveLaserDesignatorComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntityCoordinates Origin;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntityUid? Target;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float BreakRange = 0.5f;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref ActiveLaserDesignatorComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (ActiveLaserDesignatorComponent) target1;
    if (serialization.TryCustomCopy<ActiveLaserDesignatorComponent>(this, ref target, hookCtx, false, context))
      return;
    EntityCoordinates target2 = new EntityCoordinates();
    if (!serialization.TryCustomCopy<EntityCoordinates>(this.Origin, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<EntityCoordinates>(this.Origin, hookCtx, context);
    target.Origin = target2;
    EntityUid? target3 = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.Target, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<EntityUid?>(this.Target, hookCtx, context);
    target.Target = target3;
    float target4 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.BreakRange, ref target4, hookCtx, false, context))
      target4 = this.BreakRange;
    target.BreakRange = target4;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref ActiveLaserDesignatorComponent target,
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
    ActiveLaserDesignatorComponent target1 = (ActiveLaserDesignatorComponent) target;
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
    ActiveLaserDesignatorComponent target1 = (ActiveLaserDesignatorComponent) target;
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
    ActiveLaserDesignatorComponent target1 = (ActiveLaserDesignatorComponent) target;
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
  virtual ActiveLaserDesignatorComponent Component.Instantiate()
  {
    return new ActiveLaserDesignatorComponent();
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class ActiveLaserDesignatorComponent_AutoState : IComponentState
  {
    public NetCoordinates Origin;
    public NetEntity? Target;
    public float BreakRange;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class ActiveLaserDesignatorComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<ActiveLaserDesignatorComponent, ComponentGetState>(new ComponentEventRefHandler<ActiveLaserDesignatorComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<ActiveLaserDesignatorComponent, ComponentHandleState>(new ComponentEventRefHandler<ActiveLaserDesignatorComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      ActiveLaserDesignatorComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new ActiveLaserDesignatorComponent.ActiveLaserDesignatorComponent_AutoState()
      {
        Origin = this.GetNetCoordinates(component.Origin),
        Target = this.GetNetEntity(component.Target),
        BreakRange = component.BreakRange
      };
    }

    private void OnHandleState(
      EntityUid uid,
      ActiveLaserDesignatorComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is ActiveLaserDesignatorComponent.ActiveLaserDesignatorComponent_AutoState current))
        return;
      component.Origin = this.EnsureCoordinates<ActiveLaserDesignatorComponent>(current.Origin, uid);
      component.Target = this.EnsureEntity<ActiveLaserDesignatorComponent>(current.Target, uid);
      component.BreakRange = current.BreakRange;
    }
  }
}
