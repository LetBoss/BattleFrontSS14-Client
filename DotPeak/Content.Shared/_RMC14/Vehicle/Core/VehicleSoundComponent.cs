// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Vehicle.VehicleSoundComponent
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
namespace Content.Shared._RMC14.Vehicle;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
public sealed class VehicleSoundComponent : 
  Component,
  ISerializationGenerated<VehicleSoundComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public SoundSpecifier? RunningSound;
  [DataField(null, false, 1, false, false, null)]
  public float RunningSoundCooldown = 1f;
  [DataField(null, false, 1, false, false, null)]
  public SoundSpecifier? CollisionSound;
  [DataField(null, false, 1, false, false, null)]
  public float CollisionSoundCooldown = 0.25f;
  [DataField(null, false, 1, false, false, null)]
  public SoundSpecifier? MobCollisionSound;
  [DataField(null, false, 1, false, false, null)]
  public SoundSpecifier? HornSound;
  [DataField(null, false, 1, false, false, null)]
  public float HornCooldown = 1f;
  [DataField(null, false, 1, false, false, null)]
  public bool RunLoop = true;
  [DataField(null, false, 1, false, false, null)]
  public float RunMinVolume = -6f;
  [DataField(null, false, 1, false, false, null)]
  public float RunMaxVolume;
  [DataField(null, false, 1, false, false, null)]
  public float RunMinPitch = 0.92f;
  [DataField(null, false, 1, false, false, null)]
  public float RunMaxPitch = 1.08f;
  [DataField(null, false, 1, false, false, null)]
  public float RunThreshold = 0.04f;
  [DataField(null, false, 1, false, false, null)]
  public SoundSpecifier? InsideRunSound;
  [DataField(null, false, 1, false, false, null)]
  public float InsideRunVolume = -2f;
  [DataField(null, false, 1, false, false, null)]
  public float InsideRunPitch = 0.95f;
  [DataField(null, false, 1, false, false, null)]
  public SoundSpecifier? InsideHornSound;
  [DataField(null, false, 1, false, false, null)]
  public float InsideHornVolume = -2f;
  [DataField(null, false, 1, false, false, null)]
  public SoundSpecifier? InsideHitSound;
  [DataField(null, false, 1, false, false, null)]
  public float InsideHitVolume = -1f;
  public EntityUid? RunAudio;
  public float LastRunVolume = float.NaN;
  public float LastRunPitch = float.NaN;
  [AutoNetworkedField]
  public TimeSpan NextRunningSound;
  [AutoNetworkedField]
  public TimeSpan NextCollisionSound;
  [AutoNetworkedField]
  public TimeSpan NextHornSound;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref VehicleSoundComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (VehicleSoundComponent) target1;
    if (serialization.TryCustomCopy<VehicleSoundComponent>(this, ref target, hookCtx, false, context))
      return;
    SoundSpecifier target2 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.RunningSound, ref target2, hookCtx, true, context))
      target2 = serialization.CreateCopy<SoundSpecifier>(this.RunningSound, hookCtx, context);
    target.RunningSound = target2;
    float target3 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.RunningSoundCooldown, ref target3, hookCtx, false, context))
      target3 = this.RunningSoundCooldown;
    target.RunningSoundCooldown = target3;
    SoundSpecifier target4 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.CollisionSound, ref target4, hookCtx, true, context))
      target4 = serialization.CreateCopy<SoundSpecifier>(this.CollisionSound, hookCtx, context);
    target.CollisionSound = target4;
    float target5 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.CollisionSoundCooldown, ref target5, hookCtx, false, context))
      target5 = this.CollisionSoundCooldown;
    target.CollisionSoundCooldown = target5;
    SoundSpecifier target6 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.MobCollisionSound, ref target6, hookCtx, true, context))
      target6 = serialization.CreateCopy<SoundSpecifier>(this.MobCollisionSound, hookCtx, context);
    target.MobCollisionSound = target6;
    SoundSpecifier target7 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.HornSound, ref target7, hookCtx, true, context))
      target7 = serialization.CreateCopy<SoundSpecifier>(this.HornSound, hookCtx, context);
    target.HornSound = target7;
    float target8 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.HornCooldown, ref target8, hookCtx, false, context))
      target8 = this.HornCooldown;
    target.HornCooldown = target8;
    bool target9 = false;
    if (!serialization.TryCustomCopy<bool>(this.RunLoop, ref target9, hookCtx, false, context))
      target9 = this.RunLoop;
    target.RunLoop = target9;
    float target10 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.RunMinVolume, ref target10, hookCtx, false, context))
      target10 = this.RunMinVolume;
    target.RunMinVolume = target10;
    float target11 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.RunMaxVolume, ref target11, hookCtx, false, context))
      target11 = this.RunMaxVolume;
    target.RunMaxVolume = target11;
    float target12 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.RunMinPitch, ref target12, hookCtx, false, context))
      target12 = this.RunMinPitch;
    target.RunMinPitch = target12;
    float target13 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.RunMaxPitch, ref target13, hookCtx, false, context))
      target13 = this.RunMaxPitch;
    target.RunMaxPitch = target13;
    float target14 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.RunThreshold, ref target14, hookCtx, false, context))
      target14 = this.RunThreshold;
    target.RunThreshold = target14;
    SoundSpecifier target15 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.InsideRunSound, ref target15, hookCtx, true, context))
      target15 = serialization.CreateCopy<SoundSpecifier>(this.InsideRunSound, hookCtx, context);
    target.InsideRunSound = target15;
    float target16 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.InsideRunVolume, ref target16, hookCtx, false, context))
      target16 = this.InsideRunVolume;
    target.InsideRunVolume = target16;
    float target17 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.InsideRunPitch, ref target17, hookCtx, false, context))
      target17 = this.InsideRunPitch;
    target.InsideRunPitch = target17;
    SoundSpecifier target18 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.InsideHornSound, ref target18, hookCtx, true, context))
      target18 = serialization.CreateCopy<SoundSpecifier>(this.InsideHornSound, hookCtx, context);
    target.InsideHornSound = target18;
    float target19 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.InsideHornVolume, ref target19, hookCtx, false, context))
      target19 = this.InsideHornVolume;
    target.InsideHornVolume = target19;
    SoundSpecifier target20 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.InsideHitSound, ref target20, hookCtx, true, context))
      target20 = serialization.CreateCopy<SoundSpecifier>(this.InsideHitSound, hookCtx, context);
    target.InsideHitSound = target20;
    float target21 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.InsideHitVolume, ref target21, hookCtx, false, context))
      target21 = this.InsideHitVolume;
    target.InsideHitVolume = target21;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref VehicleSoundComponent target,
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
    VehicleSoundComponent target1 = (VehicleSoundComponent) target;
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
    VehicleSoundComponent target1 = (VehicleSoundComponent) target;
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
    VehicleSoundComponent target1 = (VehicleSoundComponent) target;
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
  virtual VehicleSoundComponent Component.Instantiate() => new VehicleSoundComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class VehicleSoundComponent_AutoState : IComponentState
  {
    public TimeSpan NextRunningSound;
    public TimeSpan NextCollisionSound;
    public TimeSpan NextHornSound;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class VehicleSoundComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<VehicleSoundComponent, ComponentGetState>(new ComponentEventRefHandler<VehicleSoundComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<VehicleSoundComponent, ComponentHandleState>(new ComponentEventRefHandler<VehicleSoundComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      VehicleSoundComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new VehicleSoundComponent.VehicleSoundComponent_AutoState()
      {
        NextRunningSound = component.NextRunningSound,
        NextCollisionSound = component.NextCollisionSound,
        NextHornSound = component.NextHornSound
      };
    }

    private void OnHandleState(
      EntityUid uid,
      VehicleSoundComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is VehicleSoundComponent.VehicleSoundComponent_AutoState current))
        return;
      component.NextRunningSound = current.NextRunningSound;
      component.NextCollisionSound = current.NextCollisionSound;
      component.NextHornSound = current.NextHornSound;
    }
  }
}
