// Decompiled with JetBrains decompiler
// Type: Content.Shared._CIV14merka.Teams.CivTeamMemberComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Analyzers;
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
namespace Content.Shared._CIV14merka.Teams;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(true, false)]
public sealed class CivTeamMemberComponent : 
  Component,
  ISerializationGenerated<CivTeamMemberComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int TeamId;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public string SideId = string.Empty;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int SquadId;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool IsSquadLeader;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool IsCommander;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public CivTdmClass Class;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float WalkSpeedModifier = 0.4f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float SprintSpeedModifier = 0.58f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float RunSpeedModifier = 1f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float RunStaminaThresholdRatio = 0.2f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool RunHeld;
  public bool RunActive;
  [DataField(null, false, 1, false, false, null)]
  public float StaminaDrainPerSecond = 12f;
  [DataField(null, false, 1, false, false, null)]
  public float StaminaRecoveryPerSecond = 18f;
  [DataField(null, false, 1, false, false, null)]
  public float StaminaRecoveryDelaySeconds = 1.25f;
  public TimeSpan NextStaminaRecoveryTime = TimeSpan.Zero;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref CivTeamMemberComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (CivTeamMemberComponent) target1;
    if (serialization.TryCustomCopy<CivTeamMemberComponent>(this, ref target, hookCtx, false, context))
      return;
    int target2 = 0;
    if (!serialization.TryCustomCopy<int>(this.TeamId, ref target2, hookCtx, false, context))
      target2 = this.TeamId;
    target.TeamId = target2;
    string target3 = (string) null;
    if (this.SideId == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.SideId, ref target3, hookCtx, false, context))
      target3 = this.SideId;
    target.SideId = target3;
    int target4 = 0;
    if (!serialization.TryCustomCopy<int>(this.SquadId, ref target4, hookCtx, false, context))
      target4 = this.SquadId;
    target.SquadId = target4;
    bool target5 = false;
    if (!serialization.TryCustomCopy<bool>(this.IsSquadLeader, ref target5, hookCtx, false, context))
      target5 = this.IsSquadLeader;
    target.IsSquadLeader = target5;
    bool target6 = false;
    if (!serialization.TryCustomCopy<bool>(this.IsCommander, ref target6, hookCtx, false, context))
      target6 = this.IsCommander;
    target.IsCommander = target6;
    CivTdmClass target7 = CivTdmClass.Rifleman;
    if (!serialization.TryCustomCopy<CivTdmClass>(this.Class, ref target7, hookCtx, false, context))
      target7 = this.Class;
    target.Class = target7;
    float target8 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.WalkSpeedModifier, ref target8, hookCtx, false, context))
      target8 = this.WalkSpeedModifier;
    target.WalkSpeedModifier = target8;
    float target9 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.SprintSpeedModifier, ref target9, hookCtx, false, context))
      target9 = this.SprintSpeedModifier;
    target.SprintSpeedModifier = target9;
    float target10 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.RunSpeedModifier, ref target10, hookCtx, false, context))
      target10 = this.RunSpeedModifier;
    target.RunSpeedModifier = target10;
    float target11 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.RunStaminaThresholdRatio, ref target11, hookCtx, false, context))
      target11 = this.RunStaminaThresholdRatio;
    target.RunStaminaThresholdRatio = target11;
    bool target12 = false;
    if (!serialization.TryCustomCopy<bool>(this.RunHeld, ref target12, hookCtx, false, context))
      target12 = this.RunHeld;
    target.RunHeld = target12;
    float target13 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.StaminaDrainPerSecond, ref target13, hookCtx, false, context))
      target13 = this.StaminaDrainPerSecond;
    target.StaminaDrainPerSecond = target13;
    float target14 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.StaminaRecoveryPerSecond, ref target14, hookCtx, false, context))
      target14 = this.StaminaRecoveryPerSecond;
    target.StaminaRecoveryPerSecond = target14;
    float target15 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.StaminaRecoveryDelaySeconds, ref target15, hookCtx, false, context))
      target15 = this.StaminaRecoveryDelaySeconds;
    target.StaminaRecoveryDelaySeconds = target15;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref CivTeamMemberComponent target,
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
    CivTeamMemberComponent target1 = (CivTeamMemberComponent) target;
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
    CivTeamMemberComponent target1 = (CivTeamMemberComponent) target;
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
    CivTeamMemberComponent target1 = (CivTeamMemberComponent) target;
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
  virtual CivTeamMemberComponent Component.Instantiate() => new CivTeamMemberComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class CivTeamMemberComponent_AutoState : IComponentState
  {
    public int TeamId;
    public string SideId;
    public int SquadId;
    public bool IsSquadLeader;
    public bool IsCommander;
    public CivTdmClass Class;
    public float WalkSpeedModifier;
    public float SprintSpeedModifier;
    public float RunSpeedModifier;
    public float RunStaminaThresholdRatio;
    public bool RunHeld;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class CivTeamMemberComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<CivTeamMemberComponent, ComponentGetState>(new ComponentEventRefHandler<CivTeamMemberComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<CivTeamMemberComponent, ComponentHandleState>(new ComponentEventRefHandler<CivTeamMemberComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      CivTeamMemberComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new CivTeamMemberComponent.CivTeamMemberComponent_AutoState()
      {
        TeamId = component.TeamId,
        SideId = component.SideId,
        SquadId = component.SquadId,
        IsSquadLeader = component.IsSquadLeader,
        IsCommander = component.IsCommander,
        Class = component.Class,
        WalkSpeedModifier = component.WalkSpeedModifier,
        SprintSpeedModifier = component.SprintSpeedModifier,
        RunSpeedModifier = component.RunSpeedModifier,
        RunStaminaThresholdRatio = component.RunStaminaThresholdRatio,
        RunHeld = component.RunHeld
      };
    }

    private void OnHandleState(
      EntityUid uid,
      CivTeamMemberComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is CivTeamMemberComponent.CivTeamMemberComponent_AutoState current))
        return;
      component.TeamId = current.TeamId;
      component.SideId = current.SideId;
      component.SquadId = current.SquadId;
      component.IsSquadLeader = current.IsSquadLeader;
      component.IsCommander = current.IsCommander;
      component.Class = current.Class;
      component.WalkSpeedModifier = current.WalkSpeedModifier;
      component.SprintSpeedModifier = current.SprintSpeedModifier;
      component.RunSpeedModifier = current.RunSpeedModifier;
      component.RunStaminaThresholdRatio = current.RunStaminaThresholdRatio;
      component.RunHeld = current.RunHeld;
      AfterAutoHandleStateEvent args1 = new AfterAutoHandleStateEvent(args.Current);
      this.EntityManager.EventBus.RaiseComponentEvent<AfterAutoHandleStateEvent, CivTeamMemberComponent>(uid, component, ref args1);
    }
  }
}
