// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Chemistry.RMCHyposprayComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Marines.Skills;
using Content.Shared.FixedPoint;
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
namespace Content.Shared._RMC14.Chemistry;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
public sealed class RMCHyposprayComponent : 
  Component,
  ISerializationGenerated<RMCHyposprayComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, true, false, null)]
  [AutoNetworkedField]
  public string SlotId = string.Empty;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public string VialName = "beaker";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public string SolutionName = "vial";
  [DataField(null, false, 1, false, false, null)]
  public SoundSpecifier InjectSound = (SoundSpecifier) new SoundPathSpecifier("/Audio/_RMC14/Medical/hypospray.ogg");
  [DataField(null, false, 1, false, false, null)]
  public bool OnlyAffectsMobs = true;
  [DataField(null, false, 1, false, false, null)]
  public FixedPoint2[] TransferAmounts = new FixedPoint2[5]
  {
    FixedPoint2.New(3),
    FixedPoint2.New(5),
    FixedPoint2.New(10),
    FixedPoint2.New(15),
    FixedPoint2.New(30)
  };
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public FixedPoint2 TransferAmount = FixedPoint2.New(5);
  [DataField(null, false, 1, false, false, null)]
  public TimeSpan TacticalReloadTime = TimeSpan.FromSeconds(1.25);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SkillWhitelist TacticalSkills;
  [DataField(null, false, 1, false, false, null)]
  public bool NeedHand = true;
  [DataField(null, false, 1, false, false, null)]
  public bool BreakOnHandChange = true;
  [DataField(null, false, 1, false, false, null)]
  public float MovementThreshold = 0.1f;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref RMCHyposprayComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (RMCHyposprayComponent) target1;
    if (serialization.TryCustomCopy<RMCHyposprayComponent>(this, ref target, hookCtx, false, context))
      return;
    string target2 = (string) null;
    if (this.SlotId == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.SlotId, ref target2, hookCtx, false, context))
      target2 = this.SlotId;
    target.SlotId = target2;
    string target3 = (string) null;
    if (this.VialName == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.VialName, ref target3, hookCtx, false, context))
      target3 = this.VialName;
    target.VialName = target3;
    string target4 = (string) null;
    if (this.SolutionName == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.SolutionName, ref target4, hookCtx, false, context))
      target4 = this.SolutionName;
    target.SolutionName = target4;
    SoundSpecifier target5 = (SoundSpecifier) null;
    if (this.InjectSound == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.InjectSound, ref target5, hookCtx, true, context))
      target5 = serialization.CreateCopy<SoundSpecifier>(this.InjectSound, hookCtx, context);
    target.InjectSound = target5;
    bool target6 = false;
    if (!serialization.TryCustomCopy<bool>(this.OnlyAffectsMobs, ref target6, hookCtx, false, context))
      target6 = this.OnlyAffectsMobs;
    target.OnlyAffectsMobs = target6;
    FixedPoint2[] target7 = (FixedPoint2[]) null;
    if (this.TransferAmounts == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<FixedPoint2[]>(this.TransferAmounts, ref target7, hookCtx, true, context))
      target7 = serialization.CreateCopy<FixedPoint2[]>(this.TransferAmounts, hookCtx, context);
    target.TransferAmounts = target7;
    FixedPoint2 target8 = new FixedPoint2();
    if (!serialization.TryCustomCopy<FixedPoint2>(this.TransferAmount, ref target8, hookCtx, false, context))
      target8 = serialization.CreateCopy<FixedPoint2>(this.TransferAmount, hookCtx, context);
    target.TransferAmount = target8;
    TimeSpan target9 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.TacticalReloadTime, ref target9, hookCtx, false, context))
      target9 = serialization.CreateCopy<TimeSpan>(this.TacticalReloadTime, hookCtx, context);
    target.TacticalReloadTime = target9;
    SkillWhitelist target10 = (SkillWhitelist) null;
    if (this.TacticalSkills == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SkillWhitelist>(this.TacticalSkills, ref target10, hookCtx, false, context))
    {
      if (this.TacticalSkills == null)
        target10 = (SkillWhitelist) null;
      else
        serialization.CopyTo<SkillWhitelist>(this.TacticalSkills, ref target10, hookCtx, context, true);
    }
    target.TacticalSkills = target10;
    bool target11 = false;
    if (!serialization.TryCustomCopy<bool>(this.NeedHand, ref target11, hookCtx, false, context))
      target11 = this.NeedHand;
    target.NeedHand = target11;
    bool target12 = false;
    if (!serialization.TryCustomCopy<bool>(this.BreakOnHandChange, ref target12, hookCtx, false, context))
      target12 = this.BreakOnHandChange;
    target.BreakOnHandChange = target12;
    float target13 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.MovementThreshold, ref target13, hookCtx, false, context))
      target13 = this.MovementThreshold;
    target.MovementThreshold = target13;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref RMCHyposprayComponent target,
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
    RMCHyposprayComponent target1 = (RMCHyposprayComponent) target;
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
    RMCHyposprayComponent target1 = (RMCHyposprayComponent) target;
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
    RMCHyposprayComponent target1 = (RMCHyposprayComponent) target;
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
  virtual RMCHyposprayComponent Component.Instantiate() => new RMCHyposprayComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class RMCHyposprayComponent_AutoState : IComponentState
  {
    public string SlotId;
    public string VialName;
    public string SolutionName;
    public FixedPoint2 TransferAmount;
    public SkillWhitelist TacticalSkills;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class RMCHyposprayComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<RMCHyposprayComponent, ComponentGetState>(new ComponentEventRefHandler<RMCHyposprayComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<RMCHyposprayComponent, ComponentHandleState>(new ComponentEventRefHandler<RMCHyposprayComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      RMCHyposprayComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new RMCHyposprayComponent.RMCHyposprayComponent_AutoState()
      {
        SlotId = component.SlotId,
        VialName = component.VialName,
        SolutionName = component.SolutionName,
        TransferAmount = component.TransferAmount,
        TacticalSkills = component.TacticalSkills
      };
    }

    private void OnHandleState(
      EntityUid uid,
      RMCHyposprayComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is RMCHyposprayComponent.RMCHyposprayComponent_AutoState current))
        return;
      component.SlotId = current.SlotId;
      component.VialName = current.VialName;
      component.SolutionName = current.SolutionName;
      component.TransferAmount = current.TransferAmount;
      component.TacticalSkills = current.TacticalSkills;
    }
  }
}
