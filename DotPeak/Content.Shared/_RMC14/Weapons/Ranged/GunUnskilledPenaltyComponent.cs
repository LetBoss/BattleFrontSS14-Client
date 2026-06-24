// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Weapons.Ranged.GunUnskilledPenaltyComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Marines.Skills;
using Content.Shared.FixedPoint;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Weapons.Ranged;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (CMGunSystem)})]
public sealed class GunUnskilledPenaltyComponent : 
  Component,
  ISerializationGenerated<GunUnskilledPenaltyComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int Firearms = 1;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public Angle AngleIncrease = Angle.FromDegrees(75.0);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public FixedPoint2 AccuracyAddMult = (FixedPoint2) -0.15;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntProtoId<SkillDefinitionComponent> Skill = (EntProtoId<SkillDefinitionComponent>) "RMCSkillFirearms";

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref GunUnskilledPenaltyComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (GunUnskilledPenaltyComponent) target1;
    if (serialization.TryCustomCopy<GunUnskilledPenaltyComponent>(this, ref target, hookCtx, false, context))
      return;
    int target2 = 0;
    if (!serialization.TryCustomCopy<int>(this.Firearms, ref target2, hookCtx, false, context))
      target2 = this.Firearms;
    target.Firearms = target2;
    Angle target3 = new Angle();
    if (!serialization.TryCustomCopy<Angle>(this.AngleIncrease, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<Angle>(this.AngleIncrease, hookCtx, context);
    target.AngleIncrease = target3;
    FixedPoint2 target4 = new FixedPoint2();
    if (!serialization.TryCustomCopy<FixedPoint2>(this.AccuracyAddMult, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<FixedPoint2>(this.AccuracyAddMult, hookCtx, context);
    target.AccuracyAddMult = target4;
    EntProtoId<SkillDefinitionComponent> target5 = new EntProtoId<SkillDefinitionComponent>();
    if (!serialization.TryCustomCopy<EntProtoId<SkillDefinitionComponent>>(this.Skill, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<EntProtoId<SkillDefinitionComponent>>(this.Skill, hookCtx, context);
    target.Skill = target5;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref GunUnskilledPenaltyComponent target,
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
    GunUnskilledPenaltyComponent target1 = (GunUnskilledPenaltyComponent) target;
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
    GunUnskilledPenaltyComponent target1 = (GunUnskilledPenaltyComponent) target;
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
    GunUnskilledPenaltyComponent target1 = (GunUnskilledPenaltyComponent) target;
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
  virtual GunUnskilledPenaltyComponent Component.Instantiate()
  {
    return new GunUnskilledPenaltyComponent();
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class GunUnskilledPenaltyComponent_AutoState : IComponentState
  {
    public int Firearms;
    public Angle AngleIncrease;
    public FixedPoint2 AccuracyAddMult;
    public EntProtoId<SkillDefinitionComponent> Skill;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class GunUnskilledPenaltyComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<GunUnskilledPenaltyComponent, ComponentGetState>(new ComponentEventRefHandler<GunUnskilledPenaltyComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<GunUnskilledPenaltyComponent, ComponentHandleState>(new ComponentEventRefHandler<GunUnskilledPenaltyComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      GunUnskilledPenaltyComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new GunUnskilledPenaltyComponent.GunUnskilledPenaltyComponent_AutoState()
      {
        Firearms = component.Firearms,
        AngleIncrease = component.AngleIncrease,
        AccuracyAddMult = component.AccuracyAddMult,
        Skill = component.Skill
      };
    }

    private void OnHandleState(
      EntityUid uid,
      GunUnskilledPenaltyComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is GunUnskilledPenaltyComponent.GunUnskilledPenaltyComponent_AutoState current))
        return;
      component.Firearms = current.Firearms;
      component.AngleIncrease = current.AngleIncrease;
      component.AccuracyAddMult = current.AccuracyAddMult;
      component.Skill = current.Skill;
    }
  }
}
