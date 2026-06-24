// Decompiled with JetBrains decompiler
// Type: Content.Shared.Weapons.Ranged.Components.GrapplingGunComponent
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
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Utility;
using Robust.Shared.ViewVariables;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Weapons.Ranged.Components;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
public sealed class GrapplingGunComponent : 
  Component,
  ISerializationGenerated<GrapplingGunComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float ReelRate;
  [DataField("jointId", false, 1, false, false, null)]
  [AutoNetworkedField]
  public string Joint;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntityUid? Projectile;
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [DataField("reeling", false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool Reeling;
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [DataField("reelSound", false, 1, false, false, null)]
  [AutoNetworkedField]
  public SoundSpecifier? ReelSound;
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [DataField("cycleSound", false, 1, false, false, null)]
  [AutoNetworkedField]
  public SoundSpecifier? CycleSound;
  [DataField(null, false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables]
  public SpriteSpecifier RopeSprite;
  public EntityUid? Stream;

  public GrapplingGunComponent()
  {
    SoundPathSpecifier soundPathSpecifier = new SoundPathSpecifier("/Audio/Weapons/reel.ogg");
    soundPathSpecifier.Params = AudioParams.Default.WithLoop(true);
    this.ReelSound = (SoundSpecifier) soundPathSpecifier;
    this.CycleSound = (SoundSpecifier) new SoundPathSpecifier("/Audio/Weapons/Guns/MagIn/kinetic_reload.ogg");
    this.RopeSprite = (SpriteSpecifier) new SpriteSpecifier.Rsi(new ResPath("Objects/Weapons/Guns/Launchers/grappling_gun.rsi"), "rope");
    // ISSUE: explicit constructor call
    base.\u002Ector();
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref GrapplingGunComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (GrapplingGunComponent) target1;
    if (serialization.TryCustomCopy<GrapplingGunComponent>(this, ref target, hookCtx, false, context))
      return;
    float target2 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.ReelRate, ref target2, hookCtx, false, context))
      target2 = this.ReelRate;
    target.ReelRate = target2;
    string target3 = (string) null;
    if (this.Joint == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.Joint, ref target3, hookCtx, false, context))
      target3 = this.Joint;
    target.Joint = target3;
    EntityUid? target4 = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.Projectile, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<EntityUid?>(this.Projectile, hookCtx, context);
    target.Projectile = target4;
    bool target5 = false;
    if (!serialization.TryCustomCopy<bool>(this.Reeling, ref target5, hookCtx, false, context))
      target5 = this.Reeling;
    target.Reeling = target5;
    SoundSpecifier target6 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.ReelSound, ref target6, hookCtx, true, context))
      target6 = serialization.CreateCopy<SoundSpecifier>(this.ReelSound, hookCtx, context);
    target.ReelSound = target6;
    SoundSpecifier target7 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.CycleSound, ref target7, hookCtx, true, context))
      target7 = serialization.CreateCopy<SoundSpecifier>(this.CycleSound, hookCtx, context);
    target.CycleSound = target7;
    SpriteSpecifier target8 = (SpriteSpecifier) null;
    if (this.RopeSprite == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SpriteSpecifier>(this.RopeSprite, ref target8, hookCtx, true, context))
      target8 = serialization.CreateCopy<SpriteSpecifier>(this.RopeSprite, hookCtx, context);
    target.RopeSprite = target8;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref GrapplingGunComponent target,
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
    GrapplingGunComponent target1 = (GrapplingGunComponent) target;
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
    GrapplingGunComponent target1 = (GrapplingGunComponent) target;
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
    GrapplingGunComponent target1 = (GrapplingGunComponent) target;
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
  virtual GrapplingGunComponent Component.Instantiate() => new GrapplingGunComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class GrapplingGunComponent_AutoState : IComponentState
  {
    public float ReelRate;
    public string Joint;
    public NetEntity? Projectile;
    public bool Reeling;
    public SoundSpecifier? ReelSound;
    public SoundSpecifier? CycleSound;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class GrapplingGunComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<GrapplingGunComponent, ComponentGetState>(new ComponentEventRefHandler<GrapplingGunComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<GrapplingGunComponent, ComponentHandleState>(new ComponentEventRefHandler<GrapplingGunComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      GrapplingGunComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new GrapplingGunComponent.GrapplingGunComponent_AutoState()
      {
        ReelRate = component.ReelRate,
        Joint = component.Joint,
        Projectile = this.GetNetEntity(component.Projectile),
        Reeling = component.Reeling,
        ReelSound = component.ReelSound,
        CycleSound = component.CycleSound
      };
    }

    private void OnHandleState(
      EntityUid uid,
      GrapplingGunComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is GrapplingGunComponent.GrapplingGunComponent_AutoState current))
        return;
      component.ReelRate = current.ReelRate;
      component.Joint = current.Joint;
      component.Projectile = this.EnsureEntity<GrapplingGunComponent>(current.Projectile, uid);
      component.Reeling = current.Reeling;
      component.ReelSound = current.ReelSound;
      component.CycleSound = current.CycleSound;
    }
  }
}
