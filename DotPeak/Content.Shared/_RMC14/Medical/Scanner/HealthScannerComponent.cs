// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Medical.Scanner.HealthScannerComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Medical.Scanner;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[AutoGenerateComponentPause]
public sealed class HealthScannerComponent : 
  Component,
  ISerializationGenerated<HealthScannerComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SoundSpecifier? Sound;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntityUid? Target;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  [AutoPausedField]
  public TimeSpan UpdateAt;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan UpdateCooldown = TimeSpan.FromSeconds(1L);

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref HealthScannerComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (HealthScannerComponent) target1;
    if (serialization.TryCustomCopy<HealthScannerComponent>(this, ref target, hookCtx, false, context))
      return;
    SoundSpecifier target2 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.Sound, ref target2, hookCtx, true, context))
      target2 = serialization.CreateCopy<SoundSpecifier>(this.Sound, hookCtx, context);
    target.Sound = target2;
    EntityUid? target3 = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.Target, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<EntityUid?>(this.Target, hookCtx, context);
    target.Target = target3;
    TimeSpan target4 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.UpdateAt, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<TimeSpan>(this.UpdateAt, hookCtx, context);
    target.UpdateAt = target4;
    TimeSpan target5 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.UpdateCooldown, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<TimeSpan>(this.UpdateCooldown, hookCtx, context);
    target.UpdateCooldown = target5;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref HealthScannerComponent target,
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
    HealthScannerComponent target1 = (HealthScannerComponent) target;
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
    HealthScannerComponent target1 = (HealthScannerComponent) target;
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
    HealthScannerComponent target1 = (HealthScannerComponent) target;
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
  virtual HealthScannerComponent Component.Instantiate() => new HealthScannerComponent();

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class HealthScannerComponent_AutoPauseSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<HealthScannerComponent, EntityUnpausedEvent>(new ComponentEventRefHandler<HealthScannerComponent, EntityUnpausedEvent>(this.OnEntityUnpaused));
    }

    private void OnEntityUnpaused(
      EntityUid uid,
      #nullable disable
      HealthScannerComponent component,
      ref EntityUnpausedEvent args)
    {
      component.UpdateAt += args.PausedTime;
      this.Dirty(uid, (IComponent) component);
    }
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class HealthScannerComponent_AutoState : IComponentState
  {
    public 
    #nullable enable
    SoundSpecifier? Sound;
    public NetEntity? Target;
    public TimeSpan UpdateAt;
    public TimeSpan UpdateCooldown;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class HealthScannerComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<HealthScannerComponent, ComponentGetState>(new ComponentEventRefHandler<HealthScannerComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<HealthScannerComponent, ComponentHandleState>(new ComponentEventRefHandler<HealthScannerComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      HealthScannerComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new HealthScannerComponent.HealthScannerComponent_AutoState()
      {
        Sound = component.Sound,
        Target = this.GetNetEntity(component.Target),
        UpdateAt = component.UpdateAt,
        UpdateCooldown = component.UpdateCooldown
      };
    }

    private void OnHandleState(
      EntityUid uid,
      HealthScannerComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is HealthScannerComponent.HealthScannerComponent_AutoState current))
        return;
      component.Sound = current.Sound;
      component.Target = this.EnsureEntity<HealthScannerComponent>(current.Target, uid);
      component.UpdateAt = current.UpdateAt;
      component.UpdateCooldown = current.UpdateCooldown;
    }
  }
}
