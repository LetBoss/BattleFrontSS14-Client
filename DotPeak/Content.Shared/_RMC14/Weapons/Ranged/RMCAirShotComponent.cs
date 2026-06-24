// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Weapons.Ranged.RMCAirShotComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Marines.Skills;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Weapons.Ranged;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
public sealed class RMCAirShotComponent : 
  Component,
  ISerializationGenerated<RMCAirShotComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool IgnoreRoof;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan PreparationTime = TimeSpan.FromSeconds(1L);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool RequiresCombat;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool DoAfterBreakOnMove = true;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int ShakeAmount;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int ShakeStrength = 30;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public Dictionary<EntProtoId<SkillDefinitionComponent>, int>? RequiredSkills;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public string? LastFlareId;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref RMCAirShotComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (RMCAirShotComponent) target1;
    if (serialization.TryCustomCopy<RMCAirShotComponent>(this, ref target, hookCtx, false, context))
      return;
    bool target2 = false;
    if (!serialization.TryCustomCopy<bool>(this.IgnoreRoof, ref target2, hookCtx, false, context))
      target2 = this.IgnoreRoof;
    target.IgnoreRoof = target2;
    TimeSpan target3 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.PreparationTime, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<TimeSpan>(this.PreparationTime, hookCtx, context);
    target.PreparationTime = target3;
    bool target4 = false;
    if (!serialization.TryCustomCopy<bool>(this.RequiresCombat, ref target4, hookCtx, false, context))
      target4 = this.RequiresCombat;
    target.RequiresCombat = target4;
    bool target5 = false;
    if (!serialization.TryCustomCopy<bool>(this.DoAfterBreakOnMove, ref target5, hookCtx, false, context))
      target5 = this.DoAfterBreakOnMove;
    target.DoAfterBreakOnMove = target5;
    int target6 = 0;
    if (!serialization.TryCustomCopy<int>(this.ShakeAmount, ref target6, hookCtx, false, context))
      target6 = this.ShakeAmount;
    target.ShakeAmount = target6;
    int target7 = 0;
    if (!serialization.TryCustomCopy<int>(this.ShakeStrength, ref target7, hookCtx, false, context))
      target7 = this.ShakeStrength;
    target.ShakeStrength = target7;
    Dictionary<EntProtoId<SkillDefinitionComponent>, int> target8 = (Dictionary<EntProtoId<SkillDefinitionComponent>, int>) null;
    if (!serialization.TryCustomCopy<Dictionary<EntProtoId<SkillDefinitionComponent>, int>>(this.RequiredSkills, ref target8, hookCtx, true, context))
      target8 = serialization.CreateCopy<Dictionary<EntProtoId<SkillDefinitionComponent>, int>>(this.RequiredSkills, hookCtx, context);
    target.RequiredSkills = target8;
    string target9 = (string) null;
    if (!serialization.TryCustomCopy<string>(this.LastFlareId, ref target9, hookCtx, false, context))
      target9 = this.LastFlareId;
    target.LastFlareId = target9;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref RMCAirShotComponent target,
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
    RMCAirShotComponent target1 = (RMCAirShotComponent) target;
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
    RMCAirShotComponent target1 = (RMCAirShotComponent) target;
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
    RMCAirShotComponent target1 = (RMCAirShotComponent) target;
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
  virtual RMCAirShotComponent Component.Instantiate() => new RMCAirShotComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class RMCAirShotComponent_AutoState : IComponentState
  {
    public bool IgnoreRoof;
    public TimeSpan PreparationTime;
    public bool RequiresCombat;
    public bool DoAfterBreakOnMove;
    public int ShakeAmount;
    public int ShakeStrength;
    public Dictionary<EntProtoId<SkillDefinitionComponent>, int>? RequiredSkills;
    public string? LastFlareId;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class RMCAirShotComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<RMCAirShotComponent, ComponentGetState>(new ComponentEventRefHandler<RMCAirShotComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<RMCAirShotComponent, ComponentHandleState>(new ComponentEventRefHandler<RMCAirShotComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      RMCAirShotComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new RMCAirShotComponent.RMCAirShotComponent_AutoState()
      {
        IgnoreRoof = component.IgnoreRoof,
        PreparationTime = component.PreparationTime,
        RequiresCombat = component.RequiresCombat,
        DoAfterBreakOnMove = component.DoAfterBreakOnMove,
        ShakeAmount = component.ShakeAmount,
        ShakeStrength = component.ShakeStrength,
        RequiredSkills = component.RequiredSkills,
        LastFlareId = component.LastFlareId
      };
    }

    private void OnHandleState(
      EntityUid uid,
      RMCAirShotComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is RMCAirShotComponent.RMCAirShotComponent_AutoState current))
        return;
      component.IgnoreRoof = current.IgnoreRoof;
      component.PreparationTime = current.PreparationTime;
      component.RequiresCombat = current.RequiresCombat;
      component.DoAfterBreakOnMove = current.DoAfterBreakOnMove;
      component.ShakeAmount = current.ShakeAmount;
      component.ShakeStrength = current.ShakeStrength;
      component.RequiredSkills = current.RequiredSkills == null ? (Dictionary<EntProtoId<SkillDefinitionComponent>, int>) null : new Dictionary<EntProtoId<SkillDefinitionComponent>, int>((IDictionary<EntProtoId<SkillDefinitionComponent>, int>) current.RequiredSkills);
      component.LastFlareId = current.LastFlareId;
    }
  }
}
