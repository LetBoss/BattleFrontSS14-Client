// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.OrbitalCannon.OrbitalCannonComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.OrbitalCannon;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[AutoGenerateComponentPause]
[Access(new Type[] {typeof (OrbitalCannonSystem)})]
public sealed class OrbitalCannonComponent : 
  Component,
  ISerializationGenerated<OrbitalCannonComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public string WarheadContainer = "rmc_orbital_cannon_warhead";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public string FuelContainer = "rmc_orbital_cannon_fuel";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntProtoId<OrbitalCannonWarheadComponent>[] WarheadTypes = new EntProtoId<OrbitalCannonWarheadComponent>[4]
  {
    (EntProtoId<OrbitalCannonWarheadComponent>) "RMCOrbitalCannonWarheadExplosive",
    (EntProtoId<OrbitalCannonWarheadComponent>) "RMCOrbitalCannonWarheadIncendiary",
    (EntProtoId<OrbitalCannonWarheadComponent>) "RMCOrbitalCannonWarheadCluster",
    (EntProtoId<OrbitalCannonWarheadComponent>) "RMCOrbitalCannonWarheadAegis"
  };
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int[] PossibleFuelRequirements = new int[4]
  {
    4,
    5,
    6,
    6
  };
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public List<WarheadFuelRequirement> FuelRequirements = new List<WarheadFuelRequirement>();
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public OrbitalCannonStatus Status;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int MaxFuel = 6;
  [DataField(null, false, 1, false, false, typeof (TimeOffsetSerializer))]
  [AutoNetworkedField]
  [AutoPausedField]
  public TimeSpan LastToggledAt;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan ToggleCooldown = TimeSpan.FromSeconds(4L);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SoundSpecifier? LoadItemSound = (SoundSpecifier) new SoundPathSpecifier("/Audio/_RMC14/Machines/hydraulics_1.ogg");
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SoundSpecifier? UnloadItemSound = (SoundSpecifier) new SoundPathSpecifier("/Audio/_RMC14/Machines/hydraulics_2.ogg");
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SoundSpecifier? LoadSound = (SoundSpecifier) new SoundPathSpecifier("/Audio/_RMC14/Mecha/powerloader_buckle.ogg");
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SoundSpecifier? UnloadSound = (SoundSpecifier) new SoundPathSpecifier("/Audio/_RMC14/Mecha/powerloader_unbuckle.ogg");
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SoundSpecifier? ChamberSound = (SoundSpecifier) new SoundPathSpecifier("/Audio/_RMC14/Machines/hydraulics_2.ogg");
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SoundSpecifier? FireSound = (SoundSpecifier) new SoundPathSpecifier("/Audio/_RMC14/Weapons/Vehicles/smokelauncher_fire.ogg");
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SoundSpecifier? GroundAlertSound = (SoundSpecifier) new SoundPathSpecifier("/Audio/_RMC14/Effects/ob_alert.ogg");
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SoundSpecifier? TravelSound = (SoundSpecifier) new SoundPathSpecifier("/Audio/_RMC14/Weapons/gun_orbital_travel.ogg");
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SoundSpecifier? AegisBoomSound = (SoundSpecifier) new SoundPathSpecifier("/Audio/_RMC14/Explosion/aegis-close.ogg");
  [DataField(null, false, 1, false, false, typeof (TimeOffsetSerializer))]
  [AutoNetworkedField]
  [AutoPausedField]
  public TimeSpan? LastFireAt;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan FireCooldown = TimeSpan.FromSeconds(500L);

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref OrbitalCannonComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (OrbitalCannonComponent) target1;
    if (serialization.TryCustomCopy<OrbitalCannonComponent>(this, ref target, hookCtx, false, context))
      return;
    string target2 = (string) null;
    if (this.WarheadContainer == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.WarheadContainer, ref target2, hookCtx, false, context))
      target2 = this.WarheadContainer;
    target.WarheadContainer = target2;
    string target3 = (string) null;
    if (this.FuelContainer == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.FuelContainer, ref target3, hookCtx, false, context))
      target3 = this.FuelContainer;
    target.FuelContainer = target3;
    EntProtoId<OrbitalCannonWarheadComponent>[] target4 = (EntProtoId<OrbitalCannonWarheadComponent>[]) null;
    if (this.WarheadTypes == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<EntProtoId<OrbitalCannonWarheadComponent>[]>(this.WarheadTypes, ref target4, hookCtx, true, context))
      target4 = serialization.CreateCopy<EntProtoId<OrbitalCannonWarheadComponent>[]>(this.WarheadTypes, hookCtx, context);
    target.WarheadTypes = target4;
    int[] target5 = (int[]) null;
    if (this.PossibleFuelRequirements == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<int[]>(this.PossibleFuelRequirements, ref target5, hookCtx, true, context))
      target5 = serialization.CreateCopy<int[]>(this.PossibleFuelRequirements, hookCtx, context);
    target.PossibleFuelRequirements = target5;
    List<WarheadFuelRequirement> target6 = (List<WarheadFuelRequirement>) null;
    if (this.FuelRequirements == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<List<WarheadFuelRequirement>>(this.FuelRequirements, ref target6, hookCtx, true, context))
      target6 = serialization.CreateCopy<List<WarheadFuelRequirement>>(this.FuelRequirements, hookCtx, context);
    target.FuelRequirements = target6;
    OrbitalCannonStatus target7 = OrbitalCannonStatus.Unloaded;
    if (!serialization.TryCustomCopy<OrbitalCannonStatus>(this.Status, ref target7, hookCtx, false, context))
      target7 = this.Status;
    target.Status = target7;
    int target8 = 0;
    if (!serialization.TryCustomCopy<int>(this.MaxFuel, ref target8, hookCtx, false, context))
      target8 = this.MaxFuel;
    target.MaxFuel = target8;
    TimeSpan target9 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.LastToggledAt, ref target9, hookCtx, false, context))
      target9 = serialization.CreateCopy<TimeSpan>(this.LastToggledAt, hookCtx, context);
    target.LastToggledAt = target9;
    TimeSpan target10 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.ToggleCooldown, ref target10, hookCtx, false, context))
      target10 = serialization.CreateCopy<TimeSpan>(this.ToggleCooldown, hookCtx, context);
    target.ToggleCooldown = target10;
    SoundSpecifier target11 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.LoadItemSound, ref target11, hookCtx, true, context))
      target11 = serialization.CreateCopy<SoundSpecifier>(this.LoadItemSound, hookCtx, context);
    target.LoadItemSound = target11;
    SoundSpecifier target12 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.UnloadItemSound, ref target12, hookCtx, true, context))
      target12 = serialization.CreateCopy<SoundSpecifier>(this.UnloadItemSound, hookCtx, context);
    target.UnloadItemSound = target12;
    SoundSpecifier target13 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.LoadSound, ref target13, hookCtx, true, context))
      target13 = serialization.CreateCopy<SoundSpecifier>(this.LoadSound, hookCtx, context);
    target.LoadSound = target13;
    SoundSpecifier target14 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.UnloadSound, ref target14, hookCtx, true, context))
      target14 = serialization.CreateCopy<SoundSpecifier>(this.UnloadSound, hookCtx, context);
    target.UnloadSound = target14;
    SoundSpecifier target15 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.ChamberSound, ref target15, hookCtx, true, context))
      target15 = serialization.CreateCopy<SoundSpecifier>(this.ChamberSound, hookCtx, context);
    target.ChamberSound = target15;
    SoundSpecifier target16 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.FireSound, ref target16, hookCtx, true, context))
      target16 = serialization.CreateCopy<SoundSpecifier>(this.FireSound, hookCtx, context);
    target.FireSound = target16;
    SoundSpecifier target17 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.GroundAlertSound, ref target17, hookCtx, true, context))
      target17 = serialization.CreateCopy<SoundSpecifier>(this.GroundAlertSound, hookCtx, context);
    target.GroundAlertSound = target17;
    SoundSpecifier target18 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.TravelSound, ref target18, hookCtx, true, context))
      target18 = serialization.CreateCopy<SoundSpecifier>(this.TravelSound, hookCtx, context);
    target.TravelSound = target18;
    SoundSpecifier target19 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.AegisBoomSound, ref target19, hookCtx, true, context))
      target19 = serialization.CreateCopy<SoundSpecifier>(this.AegisBoomSound, hookCtx, context);
    target.AegisBoomSound = target19;
    TimeSpan? target20 = new TimeSpan?();
    if (!serialization.TryCustomCopy<TimeSpan?>(this.LastFireAt, ref target20, hookCtx, false, context))
      target20 = serialization.CreateCopy<TimeSpan?>(this.LastFireAt, hookCtx, context);
    target.LastFireAt = target20;
    TimeSpan target21 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.FireCooldown, ref target21, hookCtx, false, context))
      target21 = serialization.CreateCopy<TimeSpan>(this.FireCooldown, hookCtx, context);
    target.FireCooldown = target21;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref OrbitalCannonComponent target,
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
    OrbitalCannonComponent target1 = (OrbitalCannonComponent) target;
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
    OrbitalCannonComponent target1 = (OrbitalCannonComponent) target;
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
    OrbitalCannonComponent target1 = (OrbitalCannonComponent) target;
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
  virtual OrbitalCannonComponent Component.Instantiate() => new OrbitalCannonComponent();

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class OrbitalCannonComponent_AutoPauseSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<OrbitalCannonComponent, EntityUnpausedEvent>(new ComponentEventRefHandler<OrbitalCannonComponent, EntityUnpausedEvent>(this.OnEntityUnpaused));
    }

    private void OnEntityUnpaused(
      EntityUid uid,
      #nullable disable
      OrbitalCannonComponent component,
      ref EntityUnpausedEvent args)
    {
      component.LastToggledAt += args.PausedTime;
      if (component.LastFireAt.HasValue)
        component.LastFireAt = new TimeSpan?(component.LastFireAt.Value + args.PausedTime);
      this.Dirty(uid, (IComponent) component);
    }
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class OrbitalCannonComponent_AutoState : IComponentState
  {
    public 
    #nullable enable
    string WarheadContainer;
    public string FuelContainer;
    public EntProtoId<OrbitalCannonWarheadComponent>[] WarheadTypes;
    public int[] PossibleFuelRequirements;
    public List<WarheadFuelRequirement> FuelRequirements;
    public OrbitalCannonStatus Status;
    public int MaxFuel;
    public TimeSpan LastToggledAt;
    public TimeSpan ToggleCooldown;
    public SoundSpecifier? LoadItemSound;
    public SoundSpecifier? UnloadItemSound;
    public SoundSpecifier? LoadSound;
    public SoundSpecifier? UnloadSound;
    public SoundSpecifier? ChamberSound;
    public SoundSpecifier? FireSound;
    public SoundSpecifier? GroundAlertSound;
    public SoundSpecifier? TravelSound;
    public SoundSpecifier? AegisBoomSound;
    public TimeSpan? LastFireAt;
    public TimeSpan FireCooldown;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class OrbitalCannonComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<OrbitalCannonComponent, ComponentGetState>(new ComponentEventRefHandler<OrbitalCannonComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<OrbitalCannonComponent, ComponentHandleState>(new ComponentEventRefHandler<OrbitalCannonComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      OrbitalCannonComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new OrbitalCannonComponent.OrbitalCannonComponent_AutoState()
      {
        WarheadContainer = component.WarheadContainer,
        FuelContainer = component.FuelContainer,
        WarheadTypes = component.WarheadTypes,
        PossibleFuelRequirements = component.PossibleFuelRequirements,
        FuelRequirements = component.FuelRequirements,
        Status = component.Status,
        MaxFuel = component.MaxFuel,
        LastToggledAt = component.LastToggledAt,
        ToggleCooldown = component.ToggleCooldown,
        LoadItemSound = component.LoadItemSound,
        UnloadItemSound = component.UnloadItemSound,
        LoadSound = component.LoadSound,
        UnloadSound = component.UnloadSound,
        ChamberSound = component.ChamberSound,
        FireSound = component.FireSound,
        GroundAlertSound = component.GroundAlertSound,
        TravelSound = component.TravelSound,
        AegisBoomSound = component.AegisBoomSound,
        LastFireAt = component.LastFireAt,
        FireCooldown = component.FireCooldown
      };
    }

    private void OnHandleState(
      EntityUid uid,
      OrbitalCannonComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is OrbitalCannonComponent.OrbitalCannonComponent_AutoState current))
        return;
      component.WarheadContainer = current.WarheadContainer;
      component.FuelContainer = current.FuelContainer;
      component.WarheadTypes = current.WarheadTypes;
      component.PossibleFuelRequirements = current.PossibleFuelRequirements;
      component.FuelRequirements = current.FuelRequirements == null ? (List<WarheadFuelRequirement>) null : new List<WarheadFuelRequirement>((IEnumerable<WarheadFuelRequirement>) current.FuelRequirements);
      component.Status = current.Status;
      component.MaxFuel = current.MaxFuel;
      component.LastToggledAt = current.LastToggledAt;
      component.ToggleCooldown = current.ToggleCooldown;
      component.LoadItemSound = current.LoadItemSound;
      component.UnloadItemSound = current.UnloadItemSound;
      component.LoadSound = current.LoadSound;
      component.UnloadSound = current.UnloadSound;
      component.ChamberSound = current.ChamberSound;
      component.FireSound = current.FireSound;
      component.GroundAlertSound = current.GroundAlertSound;
      component.TravelSound = current.TravelSound;
      component.AegisBoomSound = current.AegisBoomSound;
      component.LastFireAt = current.LastFireAt;
      component.FireCooldown = current.FireCooldown;
    }
  }
}
