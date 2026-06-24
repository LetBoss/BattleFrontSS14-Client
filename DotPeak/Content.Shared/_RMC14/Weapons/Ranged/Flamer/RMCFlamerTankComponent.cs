// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Weapons.Ranged.Flamer.RMCFlamerTankComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Chemistry.Reagent;
using Content.Shared.Whitelist;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Weapons.Ranged.Flamer;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (SharedRMCFlamerSystem)})]
public sealed class RMCFlamerTankComponent : 
  Component,
  ISerializationGenerated<RMCFlamerTankComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public string SolutionId = "rmc_flamer_tank";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntityWhitelist? RefillWhitelist;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int MaxIntensity = 40;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int MaxDuration = 30;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int MaxRange = 5;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public string ExamineIcon = "/Textures/_RMC14/Structures/Storage/reagent_tank.rsi/weldtank.png";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public List<ProtoId<ReagentPrototype>>? ReagentWhitelist;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref RMCFlamerTankComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (RMCFlamerTankComponent) target1;
    if (serialization.TryCustomCopy<RMCFlamerTankComponent>(this, ref target, hookCtx, false, context))
      return;
    string target2 = (string) null;
    if (this.SolutionId == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.SolutionId, ref target2, hookCtx, false, context))
      target2 = this.SolutionId;
    target.SolutionId = target2;
    EntityWhitelist target3 = (EntityWhitelist) null;
    if (!serialization.TryCustomCopy<EntityWhitelist>(this.RefillWhitelist, ref target3, hookCtx, false, context))
    {
      if (this.RefillWhitelist == null)
        target3 = (EntityWhitelist) null;
      else
        serialization.CopyTo<EntityWhitelist>(this.RefillWhitelist, ref target3, hookCtx, context);
    }
    target.RefillWhitelist = target3;
    int target4 = 0;
    if (!serialization.TryCustomCopy<int>(this.MaxIntensity, ref target4, hookCtx, false, context))
      target4 = this.MaxIntensity;
    target.MaxIntensity = target4;
    int target5 = 0;
    if (!serialization.TryCustomCopy<int>(this.MaxDuration, ref target5, hookCtx, false, context))
      target5 = this.MaxDuration;
    target.MaxDuration = target5;
    int target6 = 0;
    if (!serialization.TryCustomCopy<int>(this.MaxRange, ref target6, hookCtx, false, context))
      target6 = this.MaxRange;
    target.MaxRange = target6;
    string target7 = (string) null;
    if (this.ExamineIcon == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.ExamineIcon, ref target7, hookCtx, false, context))
      target7 = this.ExamineIcon;
    target.ExamineIcon = target7;
    List<ProtoId<ReagentPrototype>> target8 = (List<ProtoId<ReagentPrototype>>) null;
    if (!serialization.TryCustomCopy<List<ProtoId<ReagentPrototype>>>(this.ReagentWhitelist, ref target8, hookCtx, true, context))
      target8 = serialization.CreateCopy<List<ProtoId<ReagentPrototype>>>(this.ReagentWhitelist, hookCtx, context);
    target.ReagentWhitelist = target8;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref RMCFlamerTankComponent target,
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
    RMCFlamerTankComponent target1 = (RMCFlamerTankComponent) target;
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
    RMCFlamerTankComponent target1 = (RMCFlamerTankComponent) target;
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
    RMCFlamerTankComponent target1 = (RMCFlamerTankComponent) target;
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
  virtual RMCFlamerTankComponent Component.Instantiate() => new RMCFlamerTankComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class RMCFlamerTankComponent_AutoState : IComponentState
  {
    public string SolutionId;
    public EntityWhitelist? RefillWhitelist;
    public int MaxIntensity;
    public int MaxDuration;
    public int MaxRange;
    public string ExamineIcon;
    public List<ProtoId<ReagentPrototype>>? ReagentWhitelist;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class RMCFlamerTankComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<RMCFlamerTankComponent, ComponentGetState>(new ComponentEventRefHandler<RMCFlamerTankComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<RMCFlamerTankComponent, ComponentHandleState>(new ComponentEventRefHandler<RMCFlamerTankComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      RMCFlamerTankComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new RMCFlamerTankComponent.RMCFlamerTankComponent_AutoState()
      {
        SolutionId = component.SolutionId,
        RefillWhitelist = component.RefillWhitelist,
        MaxIntensity = component.MaxIntensity,
        MaxDuration = component.MaxDuration,
        MaxRange = component.MaxRange,
        ExamineIcon = component.ExamineIcon,
        ReagentWhitelist = component.ReagentWhitelist
      };
    }

    private void OnHandleState(
      EntityUid uid,
      RMCFlamerTankComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is RMCFlamerTankComponent.RMCFlamerTankComponent_AutoState current))
        return;
      component.SolutionId = current.SolutionId;
      component.RefillWhitelist = current.RefillWhitelist;
      component.MaxIntensity = current.MaxIntensity;
      component.MaxDuration = current.MaxDuration;
      component.MaxRange = current.MaxRange;
      component.ExamineIcon = current.ExamineIcon;
      component.ReagentWhitelist = current.ReagentWhitelist == null ? (List<ProtoId<ReagentPrototype>>) null : new List<ProtoId<ReagentPrototype>>((IEnumerable<ProtoId<ReagentPrototype>>) current.ReagentWhitelist);
    }
  }
}
