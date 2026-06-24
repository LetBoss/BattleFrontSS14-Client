// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Fortify.XenoFortifyComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Stun;
using Content.Shared.Damage;
using Content.Shared.FixedPoint;
using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Physics.Collision.Shapes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Xenonids.Fortify;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (XenoFortifySystem)})]
public sealed class XenoFortifyComponent : 
  Component,
  ISerializationGenerated<XenoFortifyComponent>,
  ISerializationGenerated
{
  public const string FixtureId = "cm-xeno-fortify";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool Fortified;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int Armor = 30;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int FrontalArmor = 5;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int ExplosionArmor = 60;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public string[] ImmuneToStatuses = new string[1]
  {
    "KnockedDown"
  };
  [DataField(null, false, 1, false, false, null)]
  public IPhysShape Shape = (IPhysShape) new PhysShapeCircle(0.49f);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public RMCSizes FortifySize = RMCSizes.Immobile;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public RMCSizes? OriginalSize;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool ChangeExplosionWeakness = true;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool BaseWeakToExplosionStuns = true;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool CanMoveFortified;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool CanHeadbuttFortified;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public FixedPoint2 MoveSpeedModifier = FixedPoint2.New(0.45);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public DamageSpecifier DamageAddedFortified = new DamageSpecifier();
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SoundSpecifier FortifySound = (SoundSpecifier) new SoundPathSpecifier("/Audio/Effects/stonedoor_openclose.ogg", new AudioParams?(AudioParams.Default.WithVariation(new float?(0.2f))));

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref XenoFortifyComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (XenoFortifyComponent) target1;
    if (serialization.TryCustomCopy<XenoFortifyComponent>(this, ref target, hookCtx, false, context))
      return;
    bool target2 = false;
    if (!serialization.TryCustomCopy<bool>(this.Fortified, ref target2, hookCtx, false, context))
      target2 = this.Fortified;
    target.Fortified = target2;
    int target3 = 0;
    if (!serialization.TryCustomCopy<int>(this.Armor, ref target3, hookCtx, false, context))
      target3 = this.Armor;
    target.Armor = target3;
    int target4 = 0;
    if (!serialization.TryCustomCopy<int>(this.FrontalArmor, ref target4, hookCtx, false, context))
      target4 = this.FrontalArmor;
    target.FrontalArmor = target4;
    int target5 = 0;
    if (!serialization.TryCustomCopy<int>(this.ExplosionArmor, ref target5, hookCtx, false, context))
      target5 = this.ExplosionArmor;
    target.ExplosionArmor = target5;
    string[] target6 = (string[]) null;
    if (this.ImmuneToStatuses == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string[]>(this.ImmuneToStatuses, ref target6, hookCtx, true, context))
      target6 = serialization.CreateCopy<string[]>(this.ImmuneToStatuses, hookCtx, context);
    target.ImmuneToStatuses = target6;
    IPhysShape target7 = (IPhysShape) null;
    if (this.Shape == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<IPhysShape>(this.Shape, ref target7, hookCtx, true, context))
      target7 = serialization.CreateCopy<IPhysShape>(this.Shape, hookCtx, context);
    target.Shape = target7;
    RMCSizes target8 = RMCSizes.Small;
    if (!serialization.TryCustomCopy<RMCSizes>(this.FortifySize, ref target8, hookCtx, false, context))
      target8 = this.FortifySize;
    target.FortifySize = target8;
    RMCSizes? target9 = new RMCSizes?();
    if (!serialization.TryCustomCopy<RMCSizes?>(this.OriginalSize, ref target9, hookCtx, false, context))
      target9 = this.OriginalSize;
    target.OriginalSize = target9;
    bool target10 = false;
    if (!serialization.TryCustomCopy<bool>(this.ChangeExplosionWeakness, ref target10, hookCtx, false, context))
      target10 = this.ChangeExplosionWeakness;
    target.ChangeExplosionWeakness = target10;
    bool target11 = false;
    if (!serialization.TryCustomCopy<bool>(this.BaseWeakToExplosionStuns, ref target11, hookCtx, false, context))
      target11 = this.BaseWeakToExplosionStuns;
    target.BaseWeakToExplosionStuns = target11;
    bool target12 = false;
    if (!serialization.TryCustomCopy<bool>(this.CanMoveFortified, ref target12, hookCtx, false, context))
      target12 = this.CanMoveFortified;
    target.CanMoveFortified = target12;
    bool target13 = false;
    if (!serialization.TryCustomCopy<bool>(this.CanHeadbuttFortified, ref target13, hookCtx, false, context))
      target13 = this.CanHeadbuttFortified;
    target.CanHeadbuttFortified = target13;
    FixedPoint2 target14 = new FixedPoint2();
    if (!serialization.TryCustomCopy<FixedPoint2>(this.MoveSpeedModifier, ref target14, hookCtx, false, context))
      target14 = serialization.CreateCopy<FixedPoint2>(this.MoveSpeedModifier, hookCtx, context);
    target.MoveSpeedModifier = target14;
    DamageSpecifier target15 = (DamageSpecifier) null;
    if (this.DamageAddedFortified == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<DamageSpecifier>(this.DamageAddedFortified, ref target15, hookCtx, false, context))
    {
      if (this.DamageAddedFortified == null)
        target15 = (DamageSpecifier) null;
      else
        serialization.CopyTo<DamageSpecifier>(this.DamageAddedFortified, ref target15, hookCtx, context, true);
    }
    target.DamageAddedFortified = target15;
    SoundSpecifier target16 = (SoundSpecifier) null;
    if (this.FortifySound == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.FortifySound, ref target16, hookCtx, true, context))
      target16 = serialization.CreateCopy<SoundSpecifier>(this.FortifySound, hookCtx, context);
    target.FortifySound = target16;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref XenoFortifyComponent target,
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
    XenoFortifyComponent target1 = (XenoFortifyComponent) target;
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
    XenoFortifyComponent target1 = (XenoFortifyComponent) target;
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
    XenoFortifyComponent target1 = (XenoFortifyComponent) target;
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
  virtual XenoFortifyComponent Component.Instantiate() => new XenoFortifyComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class XenoFortifyComponent_AutoState : IComponentState
  {
    public bool Fortified;
    public int Armor;
    public int FrontalArmor;
    public int ExplosionArmor;
    public string[] ImmuneToStatuses;
    public RMCSizes FortifySize;
    public RMCSizes? OriginalSize;
    public bool ChangeExplosionWeakness;
    public bool BaseWeakToExplosionStuns;
    public bool CanMoveFortified;
    public bool CanHeadbuttFortified;
    public FixedPoint2 MoveSpeedModifier;
    public DamageSpecifier DamageAddedFortified;
    public SoundSpecifier FortifySound;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class XenoFortifyComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<XenoFortifyComponent, ComponentGetState>(new ComponentEventRefHandler<XenoFortifyComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<XenoFortifyComponent, ComponentHandleState>(new ComponentEventRefHandler<XenoFortifyComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      XenoFortifyComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new XenoFortifyComponent.XenoFortifyComponent_AutoState()
      {
        Fortified = component.Fortified,
        Armor = component.Armor,
        FrontalArmor = component.FrontalArmor,
        ExplosionArmor = component.ExplosionArmor,
        ImmuneToStatuses = component.ImmuneToStatuses,
        FortifySize = component.FortifySize,
        OriginalSize = component.OriginalSize,
        ChangeExplosionWeakness = component.ChangeExplosionWeakness,
        BaseWeakToExplosionStuns = component.BaseWeakToExplosionStuns,
        CanMoveFortified = component.CanMoveFortified,
        CanHeadbuttFortified = component.CanHeadbuttFortified,
        MoveSpeedModifier = component.MoveSpeedModifier,
        DamageAddedFortified = component.DamageAddedFortified,
        FortifySound = component.FortifySound
      };
    }

    private void OnHandleState(
      EntityUid uid,
      XenoFortifyComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is XenoFortifyComponent.XenoFortifyComponent_AutoState current))
        return;
      component.Fortified = current.Fortified;
      component.Armor = current.Armor;
      component.FrontalArmor = current.FrontalArmor;
      component.ExplosionArmor = current.ExplosionArmor;
      component.ImmuneToStatuses = current.ImmuneToStatuses;
      component.FortifySize = current.FortifySize;
      component.OriginalSize = current.OriginalSize;
      component.ChangeExplosionWeakness = current.ChangeExplosionWeakness;
      component.BaseWeakToExplosionStuns = current.BaseWeakToExplosionStuns;
      component.CanMoveFortified = current.CanMoveFortified;
      component.CanHeadbuttFortified = current.CanHeadbuttFortified;
      component.MoveSpeedModifier = current.MoveSpeedModifier;
      component.DamageAddedFortified = current.DamageAddedFortified;
      component.FortifySound = current.FortifySound;
    }
  }
}
