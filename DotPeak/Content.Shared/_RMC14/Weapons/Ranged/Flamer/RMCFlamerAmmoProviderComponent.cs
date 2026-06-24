// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Weapons.Ranged.Flamer.RMCFlamerAmmoProviderComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.FixedPoint;
using Content.Shared.Weapons.Ranged;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Weapons.Ranged.Flamer;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[AutoGenerateComponentPause]
[Access(new Type[] {typeof (SharedRMCFlamerSystem)})]
public sealed class RMCFlamerAmmoProviderComponent : 
  Component,
  IShootable,
  ISerializationGenerated<RMCFlamerAmmoProviderComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public string ContainerId = "gun_magazine";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan DelayPer = TimeSpan.FromSeconds(0.05);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public FixedPoint2 CostPer = FixedPoint2.New(1);
  [DataField(null, false, 1, false, false, typeof (TimeOffsetSerializer))]
  [AutoNetworkedField]
  [AutoPausedField]
  public TimeSpan CantShootPopupLast;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan CantShootPopupCooldown = TimeSpan.FromSeconds(0.25);

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref RMCFlamerAmmoProviderComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (RMCFlamerAmmoProviderComponent) target1;
    if (serialization.TryCustomCopy<RMCFlamerAmmoProviderComponent>(this, ref target, hookCtx, false, context))
      return;
    string target2 = (string) null;
    if (this.ContainerId == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.ContainerId, ref target2, hookCtx, false, context))
      target2 = this.ContainerId;
    target.ContainerId = target2;
    TimeSpan target3 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.DelayPer, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<TimeSpan>(this.DelayPer, hookCtx, context);
    target.DelayPer = target3;
    FixedPoint2 target4 = new FixedPoint2();
    if (!serialization.TryCustomCopy<FixedPoint2>(this.CostPer, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<FixedPoint2>(this.CostPer, hookCtx, context);
    target.CostPer = target4;
    TimeSpan target5 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.CantShootPopupLast, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<TimeSpan>(this.CantShootPopupLast, hookCtx, context);
    target.CantShootPopupLast = target5;
    TimeSpan target6 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.CantShootPopupCooldown, ref target6, hookCtx, false, context))
      target6 = serialization.CreateCopy<TimeSpan>(this.CantShootPopupCooldown, hookCtx, context);
    target.CantShootPopupCooldown = target6;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref RMCFlamerAmmoProviderComponent target,
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
    RMCFlamerAmmoProviderComponent target1 = (RMCFlamerAmmoProviderComponent) target;
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
    RMCFlamerAmmoProviderComponent target1 = (RMCFlamerAmmoProviderComponent) target;
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
    RMCFlamerAmmoProviderComponent target1 = (RMCFlamerAmmoProviderComponent) target;
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
  virtual RMCFlamerAmmoProviderComponent Component.Instantiate()
  {
    return new RMCFlamerAmmoProviderComponent();
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class RMCFlamerAmmoProviderComponent_AutoPauseSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<RMCFlamerAmmoProviderComponent, EntityUnpausedEvent>(new ComponentEventRefHandler<RMCFlamerAmmoProviderComponent, EntityUnpausedEvent>(this.OnEntityUnpaused));
    }

    private void OnEntityUnpaused(
      EntityUid uid,
      #nullable disable
      RMCFlamerAmmoProviderComponent component,
      ref EntityUnpausedEvent args)
    {
      component.CantShootPopupLast += args.PausedTime;
      this.Dirty(uid, (IComponent) component);
    }
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class RMCFlamerAmmoProviderComponent_AutoState : IComponentState
  {
    public 
    #nullable enable
    string ContainerId;
    public TimeSpan DelayPer;
    public FixedPoint2 CostPer;
    public TimeSpan CantShootPopupLast;
    public TimeSpan CantShootPopupCooldown;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class RMCFlamerAmmoProviderComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<RMCFlamerAmmoProviderComponent, ComponentGetState>(new ComponentEventRefHandler<RMCFlamerAmmoProviderComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<RMCFlamerAmmoProviderComponent, ComponentHandleState>(new ComponentEventRefHandler<RMCFlamerAmmoProviderComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      RMCFlamerAmmoProviderComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new RMCFlamerAmmoProviderComponent.RMCFlamerAmmoProviderComponent_AutoState()
      {
        ContainerId = component.ContainerId,
        DelayPer = component.DelayPer,
        CostPer = component.CostPer,
        CantShootPopupLast = component.CantShootPopupLast,
        CantShootPopupCooldown = component.CantShootPopupCooldown
      };
    }

    private void OnHandleState(
      EntityUid uid,
      RMCFlamerAmmoProviderComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is RMCFlamerAmmoProviderComponent.RMCFlamerAmmoProviderComponent_AutoState current))
        return;
      component.ContainerId = current.ContainerId;
      component.DelayPer = current.DelayPer;
      component.CostPer = current.CostPer;
      component.CantShootPopupLast = current.CantShootPopupLast;
      component.CantShootPopupCooldown = current.CantShootPopupCooldown;
    }
  }
}
