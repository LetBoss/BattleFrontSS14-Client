// Decompiled with JetBrains decompiler
// Type: Content.Shared.Weapons.Misc.ForceGunComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Weapons.Misc;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(true, false)]
public sealed class ForceGunComponent : 
  BaseForceGunComponent,
  ISerializationGenerated<ForceGunComponent>,
  ISerializationGenerated
{
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [DataField("throwDistance", false, 1, false, false, null)]
  [AutoNetworkedField]
  public float ThrowDistance;
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [DataField("throwForce", false, 1, false, false, null)]
  [AutoNetworkedField]
  public float ThrowForce;
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [DataField("soundLaunch", false, 1, false, false, null)]
  public SoundSpecifier? LaunchSound;

  public ForceGunComponent()
  {
    SoundPathSpecifier soundPathSpecifier = new SoundPathSpecifier("/Audio/Weapons/soup.ogg");
    soundPathSpecifier.Params = AudioParams.Default.WithVolume(5f);
    this.LaunchSound = (SoundSpecifier) soundPathSpecifier;
    // ISSUE: explicit constructor call
    base.\u002Ector();
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref ForceGunComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    BaseForceGunComponent target1 = (BaseForceGunComponent) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (ForceGunComponent) target1;
    if (serialization.TryCustomCopy<ForceGunComponent>(this, ref target, hookCtx, false, context))
      return;
    float target2 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.ThrowDistance, ref target2, hookCtx, false, context))
      target2 = this.ThrowDistance;
    target.ThrowDistance = target2;
    float target3 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.ThrowForce, ref target3, hookCtx, false, context))
      target3 = this.ThrowForce;
    target.ThrowForce = target3;
    SoundSpecifier target4 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.LaunchSound, ref target4, hookCtx, true, context))
      target4 = serialization.CreateCopy<SoundSpecifier>(this.LaunchSound, hookCtx, context);
    target.LaunchSound = target4;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref ForceGunComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref BaseForceGunComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    ForceGunComponent target1 = (ForceGunComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (BaseForceGunComponent) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    ForceGunComponent target1 = (ForceGunComponent) target;
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
    ForceGunComponent target1 = (ForceGunComponent) target;
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
  virtual ForceGunComponent BaseForceGunComponent.Instantiate() => new ForceGunComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class ForceGunComponent_AutoState : IComponentState
  {
    public float ThrowDistance;
    public float ThrowForce;
    public Color LineColor;
    public NetEntity? TetherEntity;
    public NetEntity? Tethered;
    public bool CanUnanchor;
    public bool CanTetherAlive;
    public float MaxForce;
    public float Frequency;
    public float DampingRatio;
    public float MassLimit;
    public SoundSpecifier? Sound;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class ForceGunComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<ForceGunComponent, ComponentGetState>(new ComponentEventRefHandler<ForceGunComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<ForceGunComponent, ComponentHandleState>(new ComponentEventRefHandler<ForceGunComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(EntityUid uid, ForceGunComponent component, ref ComponentGetState args)
    {
      args.State = (IComponentState) new ForceGunComponent.ForceGunComponent_AutoState()
      {
        ThrowDistance = component.ThrowDistance,
        ThrowForce = component.ThrowForce,
        LineColor = component.LineColor,
        TetherEntity = this.GetNetEntity(component.TetherEntity),
        Tethered = this.GetNetEntity(component.Tethered),
        CanUnanchor = component.CanUnanchor,
        CanTetherAlive = component.CanTetherAlive,
        MaxForce = component.MaxForce,
        Frequency = component.Frequency,
        DampingRatio = component.DampingRatio,
        MassLimit = component.MassLimit,
        Sound = component.Sound
      };
    }

    private void OnHandleState(
      EntityUid uid,
      ForceGunComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is ForceGunComponent.ForceGunComponent_AutoState current))
        return;
      component.ThrowDistance = current.ThrowDistance;
      component.ThrowForce = current.ThrowForce;
      component.LineColor = current.LineColor;
      component.TetherEntity = this.EnsureEntity<ForceGunComponent>(current.TetherEntity, uid);
      component.Tethered = this.EnsureEntity<ForceGunComponent>(current.Tethered, uid);
      component.CanUnanchor = current.CanUnanchor;
      component.CanTetherAlive = current.CanTetherAlive;
      component.MaxForce = current.MaxForce;
      component.Frequency = current.Frequency;
      component.DampingRatio = current.DampingRatio;
      component.MassLimit = current.MassLimit;
      component.Sound = current.Sound;
      AfterAutoHandleStateEvent args1 = new AfterAutoHandleStateEvent(args.Current);
      this.EntityManager.EventBus.RaiseComponentEvent<AfterAutoHandleStateEvent, ForceGunComponent>(uid, component, ref args1);
    }
  }
}
