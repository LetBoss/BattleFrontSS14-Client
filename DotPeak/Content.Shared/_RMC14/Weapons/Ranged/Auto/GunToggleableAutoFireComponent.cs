// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Weapons.Ranged.Auto.GunToggleableAutoFireComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Weapons.Ranged.Auto;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (GunToggleableAutoFireSystem)})]
public sealed class GunToggleableAutoFireComponent : 
  Component,
  ISerializationGenerated<GunToggleableAutoFireComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public Vector2i Range = new Vector2i(20, 10);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float MaxRange = 10f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float BatteryDrain = 5f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntProtoId ActionId = (EntProtoId) "RMCActionToggleAutoFire";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntityUid? Action;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SoundSpecifier ToggleSound = (SoundSpecifier) new SoundPathSpecifier("/Audio/_RMC14/Machines/click.ogg");

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref GunToggleableAutoFireComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (GunToggleableAutoFireComponent) target1;
    if (serialization.TryCustomCopy<GunToggleableAutoFireComponent>(this, ref target, hookCtx, false, context))
      return;
    Vector2i target2 = new Vector2i();
    if (!serialization.TryCustomCopy<Vector2i>(this.Range, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<Vector2i>(this.Range, hookCtx, context);
    target.Range = target2;
    float target3 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.MaxRange, ref target3, hookCtx, false, context))
      target3 = this.MaxRange;
    target.MaxRange = target3;
    float target4 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.BatteryDrain, ref target4, hookCtx, false, context))
      target4 = this.BatteryDrain;
    target.BatteryDrain = target4;
    EntProtoId target5 = new EntProtoId();
    if (!serialization.TryCustomCopy<EntProtoId>(this.ActionId, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<EntProtoId>(this.ActionId, hookCtx, context);
    target.ActionId = target5;
    EntityUid? target6 = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.Action, ref target6, hookCtx, false, context))
      target6 = serialization.CreateCopy<EntityUid?>(this.Action, hookCtx, context);
    target.Action = target6;
    SoundSpecifier target7 = (SoundSpecifier) null;
    if (this.ToggleSound == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.ToggleSound, ref target7, hookCtx, true, context))
      target7 = serialization.CreateCopy<SoundSpecifier>(this.ToggleSound, hookCtx, context);
    target.ToggleSound = target7;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref GunToggleableAutoFireComponent target,
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
    GunToggleableAutoFireComponent target1 = (GunToggleableAutoFireComponent) target;
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
    GunToggleableAutoFireComponent target1 = (GunToggleableAutoFireComponent) target;
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
    GunToggleableAutoFireComponent target1 = (GunToggleableAutoFireComponent) target;
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
  virtual GunToggleableAutoFireComponent Component.Instantiate()
  {
    return new GunToggleableAutoFireComponent();
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class GunToggleableAutoFireComponent_AutoState : IComponentState
  {
    public Vector2i Range;
    public float MaxRange;
    public float BatteryDrain;
    public EntProtoId ActionId;
    public NetEntity? Action;
    public SoundSpecifier ToggleSound;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class GunToggleableAutoFireComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<GunToggleableAutoFireComponent, ComponentGetState>(new ComponentEventRefHandler<GunToggleableAutoFireComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<GunToggleableAutoFireComponent, ComponentHandleState>(new ComponentEventRefHandler<GunToggleableAutoFireComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      GunToggleableAutoFireComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new GunToggleableAutoFireComponent.GunToggleableAutoFireComponent_AutoState()
      {
        Range = component.Range,
        MaxRange = component.MaxRange,
        BatteryDrain = component.BatteryDrain,
        ActionId = component.ActionId,
        Action = this.GetNetEntity(component.Action),
        ToggleSound = component.ToggleSound
      };
    }

    private void OnHandleState(
      EntityUid uid,
      GunToggleableAutoFireComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is GunToggleableAutoFireComponent.GunToggleableAutoFireComponent_AutoState current))
        return;
      component.Range = current.Range;
      component.MaxRange = current.MaxRange;
      component.BatteryDrain = current.BatteryDrain;
      component.ActionId = current.ActionId;
      component.Action = this.EnsureEntity<GunToggleableAutoFireComponent>(current.Action, uid);
      component.ToggleSound = current.ToggleSound;
    }
  }
}
