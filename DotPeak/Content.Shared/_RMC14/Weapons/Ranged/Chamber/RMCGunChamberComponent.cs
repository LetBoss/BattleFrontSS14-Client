// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Weapons.Ranged.Chamber.RMCGunChamberComponent
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
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Weapons.Ranged.Chamber;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[AutoGenerateComponentPause]
[Access(new Type[] {typeof (RMCGunChamberSystem)})]
public sealed class RMCGunChamberComponent : 
  Component,
  ISerializationGenerated<RMCGunChamberComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool Enabled = true;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public string ContainerId = "rmc_gun_chamber";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SoundSpecifier? Sound = (SoundSpecifier) new SoundPathSpecifier("/Audio/_RMC14/Weapons/Guns/Cock/gun_cocked2.ogg", new AudioParams?(AudioParams.Default.WithMaxDistance(3f)));
  [DataField(null, false, 1, false, false, typeof (TimeOffsetSerializer))]
  [AutoNetworkedField]
  [AutoPausedField]
  public TimeSpan? LastChamberedAt;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan ChamberCooldown = TimeSpan.FromSeconds(1L);

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref RMCGunChamberComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (RMCGunChamberComponent) target1;
    if (serialization.TryCustomCopy<RMCGunChamberComponent>(this, ref target, hookCtx, false, context))
      return;
    bool target2 = false;
    if (!serialization.TryCustomCopy<bool>(this.Enabled, ref target2, hookCtx, false, context))
      target2 = this.Enabled;
    target.Enabled = target2;
    string target3 = (string) null;
    if (this.ContainerId == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.ContainerId, ref target3, hookCtx, false, context))
      target3 = this.ContainerId;
    target.ContainerId = target3;
    SoundSpecifier target4 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.Sound, ref target4, hookCtx, true, context))
      target4 = serialization.CreateCopy<SoundSpecifier>(this.Sound, hookCtx, context);
    target.Sound = target4;
    TimeSpan? target5 = new TimeSpan?();
    if (!serialization.TryCustomCopy<TimeSpan?>(this.LastChamberedAt, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<TimeSpan?>(this.LastChamberedAt, hookCtx, context);
    target.LastChamberedAt = target5;
    TimeSpan target6 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.ChamberCooldown, ref target6, hookCtx, false, context))
      target6 = serialization.CreateCopy<TimeSpan>(this.ChamberCooldown, hookCtx, context);
    target.ChamberCooldown = target6;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref RMCGunChamberComponent target,
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
    RMCGunChamberComponent target1 = (RMCGunChamberComponent) target;
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
    RMCGunChamberComponent target1 = (RMCGunChamberComponent) target;
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
    RMCGunChamberComponent target1 = (RMCGunChamberComponent) target;
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
  virtual RMCGunChamberComponent Component.Instantiate() => new RMCGunChamberComponent();

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class RMCGunChamberComponent_AutoPauseSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<RMCGunChamberComponent, EntityUnpausedEvent>(new ComponentEventRefHandler<RMCGunChamberComponent, EntityUnpausedEvent>(this.OnEntityUnpaused));
    }

    private void OnEntityUnpaused(
      EntityUid uid,
      #nullable disable
      RMCGunChamberComponent component,
      ref EntityUnpausedEvent args)
    {
      if (component.LastChamberedAt.HasValue)
        component.LastChamberedAt = new TimeSpan?(component.LastChamberedAt.Value + args.PausedTime);
      this.Dirty(uid, (IComponent) component);
    }
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class RMCGunChamberComponent_AutoState : IComponentState
  {
    public bool Enabled;
    public 
    #nullable enable
    string ContainerId;
    public SoundSpecifier? Sound;
    public TimeSpan? LastChamberedAt;
    public TimeSpan ChamberCooldown;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class RMCGunChamberComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<RMCGunChamberComponent, ComponentGetState>(new ComponentEventRefHandler<RMCGunChamberComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<RMCGunChamberComponent, ComponentHandleState>(new ComponentEventRefHandler<RMCGunChamberComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      RMCGunChamberComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new RMCGunChamberComponent.RMCGunChamberComponent_AutoState()
      {
        Enabled = component.Enabled,
        ContainerId = component.ContainerId,
        Sound = component.Sound,
        LastChamberedAt = component.LastChamberedAt,
        ChamberCooldown = component.ChamberCooldown
      };
    }

    private void OnHandleState(
      EntityUid uid,
      RMCGunChamberComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is RMCGunChamberComponent.RMCGunChamberComponent_AutoState current))
        return;
      component.Enabled = current.Enabled;
      component.ContainerId = current.ContainerId;
      component.Sound = current.Sound;
      component.LastChamberedAt = current.LastChamberedAt;
      component.ChamberCooldown = current.ChamberCooldown;
    }
  }
}
