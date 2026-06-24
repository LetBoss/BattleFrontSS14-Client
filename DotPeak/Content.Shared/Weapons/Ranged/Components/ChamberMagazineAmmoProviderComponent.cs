// Decompiled with JetBrains decompiler
// Type: Content.Shared.Weapons.Ranged.Components.ChamberMagazineAmmoProviderComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Weapons.Ranged.Systems;
using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Weapons.Ranged.Components;

[RegisterComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (SharedGunSystem)})]
public sealed class ChamberMagazineAmmoProviderComponent : 
  MagazineAmmoProviderComponent,
  ISerializationGenerated<ChamberMagazineAmmoProviderComponent>,
  ISerializationGenerated
{
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [DataField("boltClosed", false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool? BoltClosed = new bool?(false);
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [DataField("autoCycle", false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool AutoCycle = true;
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [DataField("canRack", false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool CanRack = true;
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [DataField("soundBoltClosed", false, 1, false, false, null)]
  [AutoNetworkedField]
  public SoundSpecifier? BoltClosedSound = (SoundSpecifier) new SoundPathSpecifier("/Audio/Weapons/Guns/Bolt/rifle_bolt_closed.ogg");
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [DataField("soundBoltOpened", false, 1, false, false, null)]
  [AutoNetworkedField]
  public SoundSpecifier? BoltOpenedSound = (SoundSpecifier) new SoundPathSpecifier("/Audio/Weapons/Guns/Bolt/rifle_bolt_open.ogg");
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [DataField("soundRack", false, 1, false, false, null)]
  [AutoNetworkedField]
  public SoundSpecifier? RackSound = (SoundSpecifier) new SoundPathSpecifier("/Audio/Weapons/Guns/Cock/ltrifle_cock.ogg");

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref ChamberMagazineAmmoProviderComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    MagazineAmmoProviderComponent target1 = (MagazineAmmoProviderComponent) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (ChamberMagazineAmmoProviderComponent) target1;
    if (serialization.TryCustomCopy<ChamberMagazineAmmoProviderComponent>(this, ref target, hookCtx, false, context))
      return;
    bool? target2 = new bool?();
    if (!serialization.TryCustomCopy<bool?>(this.BoltClosed, ref target2, hookCtx, false, context))
      target2 = this.BoltClosed;
    target.BoltClosed = target2;
    bool target3 = false;
    if (!serialization.TryCustomCopy<bool>(this.AutoCycle, ref target3, hookCtx, false, context))
      target3 = this.AutoCycle;
    target.AutoCycle = target3;
    bool target4 = false;
    if (!serialization.TryCustomCopy<bool>(this.CanRack, ref target4, hookCtx, false, context))
      target4 = this.CanRack;
    target.CanRack = target4;
    SoundSpecifier target5 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.BoltClosedSound, ref target5, hookCtx, true, context))
      target5 = serialization.CreateCopy<SoundSpecifier>(this.BoltClosedSound, hookCtx, context);
    target.BoltClosedSound = target5;
    SoundSpecifier target6 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.BoltOpenedSound, ref target6, hookCtx, true, context))
      target6 = serialization.CreateCopy<SoundSpecifier>(this.BoltOpenedSound, hookCtx, context);
    target.BoltOpenedSound = target6;
    SoundSpecifier target7 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.RackSound, ref target7, hookCtx, true, context))
      target7 = serialization.CreateCopy<SoundSpecifier>(this.RackSound, hookCtx, context);
    target.RackSound = target7;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref ChamberMagazineAmmoProviderComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref MagazineAmmoProviderComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    ChamberMagazineAmmoProviderComponent target1 = (ChamberMagazineAmmoProviderComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (MagazineAmmoProviderComponent) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    ChamberMagazineAmmoProviderComponent target1 = (ChamberMagazineAmmoProviderComponent) target;
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
    ChamberMagazineAmmoProviderComponent target1 = (ChamberMagazineAmmoProviderComponent) target;
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
  virtual ChamberMagazineAmmoProviderComponent MagazineAmmoProviderComponent.Instantiate()
  {
    return new ChamberMagazineAmmoProviderComponent();
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class ChamberMagazineAmmoProviderComponent_AutoState : IComponentState
  {
    public bool? BoltClosed;
    public bool AutoCycle;
    public bool CanRack;
    public SoundSpecifier? BoltClosedSound;
    public SoundSpecifier? BoltOpenedSound;
    public SoundSpecifier? RackSound;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class ChamberMagazineAmmoProviderComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<ChamberMagazineAmmoProviderComponent, ComponentGetState>(new ComponentEventRefHandler<ChamberMagazineAmmoProviderComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<ChamberMagazineAmmoProviderComponent, ComponentHandleState>(new ComponentEventRefHandler<ChamberMagazineAmmoProviderComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      ChamberMagazineAmmoProviderComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new ChamberMagazineAmmoProviderComponent.ChamberMagazineAmmoProviderComponent_AutoState()
      {
        BoltClosed = component.BoltClosed,
        AutoCycle = component.AutoCycle,
        CanRack = component.CanRack,
        BoltClosedSound = component.BoltClosedSound,
        BoltOpenedSound = component.BoltOpenedSound,
        RackSound = component.RackSound
      };
    }

    private void OnHandleState(
      EntityUid uid,
      ChamberMagazineAmmoProviderComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is ChamberMagazineAmmoProviderComponent.ChamberMagazineAmmoProviderComponent_AutoState current))
        return;
      component.BoltClosed = current.BoltClosed;
      component.AutoCycle = current.AutoCycle;
      component.CanRack = current.CanRack;
      component.BoltClosedSound = current.BoltClosedSound;
      component.BoltOpenedSound = current.BoltOpenedSound;
      component.RackSound = current.RackSound;
    }
  }
}
