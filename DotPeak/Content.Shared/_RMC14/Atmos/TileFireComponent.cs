// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Atmos.TileFireComponent
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
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Atmos;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[AutoGenerateComponentPause]
[Access(new Type[] {typeof (SharedRMCFlammableSystem)})]
public sealed class TileFireComponent : 
  Component,
  ISerializationGenerated<TileFireComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, true, false, null)]
  [AutoNetworkedField]
  public EntProtoId<TileFireComponent>? Id;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool ExtinguishInstantly = true;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float PatExtinguishMultiplier = 1f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float SprayExtinguishMultiplier = 1f;
  [DataField(null, false, 1, false, false, typeof (TimeOffsetSerializer))]
  [AutoNetworkedField]
  [AutoPausedField]
  public TimeSpan SpawnedAt;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan Duration = TimeSpan.FromMinutes(1L);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan BigFireDuration = TimeSpan.FromSeconds(0.5);

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref TileFireComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (TileFireComponent) target1;
    if (serialization.TryCustomCopy<TileFireComponent>(this, ref target, hookCtx, false, context))
      return;
    EntProtoId<TileFireComponent>? target2 = new EntProtoId<TileFireComponent>?();
    if (!serialization.TryCustomCopy<EntProtoId<TileFireComponent>?>(this.Id, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<EntProtoId<TileFireComponent>?>(this.Id, hookCtx, context);
    target.Id = target2;
    bool target3 = false;
    if (!serialization.TryCustomCopy<bool>(this.ExtinguishInstantly, ref target3, hookCtx, false, context))
      target3 = this.ExtinguishInstantly;
    target.ExtinguishInstantly = target3;
    float target4 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.PatExtinguishMultiplier, ref target4, hookCtx, false, context))
      target4 = this.PatExtinguishMultiplier;
    target.PatExtinguishMultiplier = target4;
    float target5 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.SprayExtinguishMultiplier, ref target5, hookCtx, false, context))
      target5 = this.SprayExtinguishMultiplier;
    target.SprayExtinguishMultiplier = target5;
    TimeSpan target6 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.SpawnedAt, ref target6, hookCtx, false, context))
      target6 = serialization.CreateCopy<TimeSpan>(this.SpawnedAt, hookCtx, context);
    target.SpawnedAt = target6;
    TimeSpan target7 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.Duration, ref target7, hookCtx, false, context))
      target7 = serialization.CreateCopy<TimeSpan>(this.Duration, hookCtx, context);
    target.Duration = target7;
    TimeSpan target8 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.BigFireDuration, ref target8, hookCtx, false, context))
      target8 = serialization.CreateCopy<TimeSpan>(this.BigFireDuration, hookCtx, context);
    target.BigFireDuration = target8;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref TileFireComponent target,
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
    TileFireComponent target1 = (TileFireComponent) target;
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
    TileFireComponent target1 = (TileFireComponent) target;
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
    TileFireComponent target1 = (TileFireComponent) target;
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
  virtual TileFireComponent Component.Instantiate() => new TileFireComponent();

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class TileFireComponent_AutoPauseSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<TileFireComponent, EntityUnpausedEvent>(new ComponentEventRefHandler<TileFireComponent, EntityUnpausedEvent>(this.OnEntityUnpaused));
    }

    private void OnEntityUnpaused(
      EntityUid uid,
      #nullable disable
      TileFireComponent component,
      ref EntityUnpausedEvent args)
    {
      component.SpawnedAt += args.PausedTime;
      this.Dirty(uid, (IComponent) component);
    }
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class TileFireComponent_AutoState : IComponentState
  {
    public EntProtoId<
    #nullable enable
    TileFireComponent>? Id;
    public bool ExtinguishInstantly;
    public float PatExtinguishMultiplier;
    public float SprayExtinguishMultiplier;
    public TimeSpan SpawnedAt;
    public TimeSpan Duration;
    public TimeSpan BigFireDuration;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class TileFireComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<TileFireComponent, ComponentGetState>(new ComponentEventRefHandler<TileFireComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<TileFireComponent, ComponentHandleState>(new ComponentEventRefHandler<TileFireComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(EntityUid uid, TileFireComponent component, ref ComponentGetState args)
    {
      args.State = (IComponentState) new TileFireComponent.TileFireComponent_AutoState()
      {
        Id = component.Id,
        ExtinguishInstantly = component.ExtinguishInstantly,
        PatExtinguishMultiplier = component.PatExtinguishMultiplier,
        SprayExtinguishMultiplier = component.SprayExtinguishMultiplier,
        SpawnedAt = component.SpawnedAt,
        Duration = component.Duration,
        BigFireDuration = component.BigFireDuration
      };
    }

    private void OnHandleState(
      EntityUid uid,
      TileFireComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is TileFireComponent.TileFireComponent_AutoState current))
        return;
      component.Id = current.Id;
      component.ExtinguishInstantly = current.ExtinguishInstantly;
      component.PatExtinguishMultiplier = current.PatExtinguishMultiplier;
      component.SprayExtinguishMultiplier = current.SprayExtinguishMultiplier;
      component.SpawnedAt = current.SpawnedAt;
      component.Duration = current.Duration;
      component.BigFireDuration = current.BigFireDuration;
    }
  }
}
