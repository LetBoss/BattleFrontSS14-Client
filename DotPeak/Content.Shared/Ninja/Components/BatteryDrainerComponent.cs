// Decompiled with JetBrains decompiler
// Type: Content.Shared.Ninja.Components.BatteryDrainerComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Ninja.Systems;
using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Ninja.Components;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (SharedBatteryDrainerSystem)})]
public sealed class BatteryDrainerComponent : 
  Component,
  ISerializationGenerated<BatteryDrainerComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntityUid? BatteryUid;
  [DataField(null, false, 1, false, false, null)]
  public float DrainEfficiency = 1f / 1000f;
  [DataField(null, false, 1, false, false, null)]
  public float DrainTime = 1f;
  [DataField(null, false, 1, false, false, null)]
  public SoundSpecifier SparkSound = (SoundSpecifier) new SoundCollectionSpecifier("sparks");

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref BatteryDrainerComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (BatteryDrainerComponent) target1;
    if (serialization.TryCustomCopy<BatteryDrainerComponent>(this, ref target, hookCtx, false, context))
      return;
    EntityUid? target2 = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.BatteryUid, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<EntityUid?>(this.BatteryUid, hookCtx, context);
    target.BatteryUid = target2;
    float target3 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.DrainEfficiency, ref target3, hookCtx, false, context))
      target3 = this.DrainEfficiency;
    target.DrainEfficiency = target3;
    float target4 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.DrainTime, ref target4, hookCtx, false, context))
      target4 = this.DrainTime;
    target.DrainTime = target4;
    SoundSpecifier target5 = (SoundSpecifier) null;
    if (this.SparkSound == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.SparkSound, ref target5, hookCtx, true, context))
      target5 = serialization.CreateCopy<SoundSpecifier>(this.SparkSound, hookCtx, context);
    target.SparkSound = target5;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref BatteryDrainerComponent target,
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
    BatteryDrainerComponent target1 = (BatteryDrainerComponent) target;
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
    BatteryDrainerComponent target1 = (BatteryDrainerComponent) target;
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
    BatteryDrainerComponent target1 = (BatteryDrainerComponent) target;
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
  virtual BatteryDrainerComponent Component.Instantiate() => new BatteryDrainerComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class BatteryDrainerComponent_AutoState : IComponentState
  {
    public NetEntity? BatteryUid;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class BatteryDrainerComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<BatteryDrainerComponent, ComponentGetState>(new ComponentEventRefHandler<BatteryDrainerComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<BatteryDrainerComponent, ComponentHandleState>(new ComponentEventRefHandler<BatteryDrainerComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      BatteryDrainerComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new BatteryDrainerComponent.BatteryDrainerComponent_AutoState()
      {
        BatteryUid = this.GetNetEntity(component.BatteryUid)
      };
    }

    private void OnHandleState(
      EntityUid uid,
      BatteryDrainerComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is BatteryDrainerComponent.BatteryDrainerComponent_AutoState current))
        return;
      component.BatteryUid = this.EnsureEntity<BatteryDrainerComponent>(current.BatteryUid, uid);
    }
  }
}
