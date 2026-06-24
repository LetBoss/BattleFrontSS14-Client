// Decompiled with JetBrains decompiler
// Type: Content.Shared.Turrets.DeployableTurretComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Damage.Prototypes;
using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Timing;
using Robust.Shared.ViewVariables;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Turrets;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, true)]
[AutoGenerateComponentPause]
[Access(new Type[] {typeof (SharedDeployableTurretSystem)})]
public sealed class DeployableTurretComponent : 
  Component,
  ISerializationGenerated<DeployableTurretComponent>,
  ISerializationGenerated,
  IComponentDelta,
  IComponent,
  ISerializationGenerated<IComponent>,
  ISerializationGenerated<IComponentDelta>
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool Enabled;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public DeployableTurretState CurrentState;
  [DataField(null, false, 1, false, false, null)]
  public DeployableTurretState VisualState;
  [DataField(null, false, 1, false, false, null)]
  public string? DeployedFixture = "turret";
  [DataField(null, false, 1, false, false, null)]
  public ProtoId<DamageModifierSetPrototype>? RetractedDamageModifierSetId;
  [DataField(null, false, 1, false, false, null)]
  public ProtoId<DamageModifierSetPrototype>? DeployedDamageModifierSetId;
  [DataField(null, false, 1, false, false, null)]
  public SoundSpecifier AccessDeniedSound = (SoundSpecifier) new SoundPathSpecifier("/Audio/Machines/custom_deny.ogg");
  [DataField(null, false, 1, false, false, null)]
  public SoundSpecifier DeploymentSound = (SoundSpecifier) new SoundPathSpecifier("/Audio/Machines/blastdoor.ogg");
  [DataField(null, false, 1, false, false, null)]
  public SoundSpecifier RetractionSound = (SoundSpecifier) new SoundPathSpecifier("/Audio/Machines/blastdoor.ogg");
  [DataField(null, false, 1, false, false, null)]
  public float DeploymentLength = 1.19f;
  [DataField(null, false, 1, false, false, null)]
  public float RetractionLength = 1.19f;
  [DataField(null, false, 1, false, false, null)]
  [AutoPausedField]
  public TimeSpan AnimationCompletionTime = TimeSpan.Zero;
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public object DeploymentAnimation;
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public object RetractionAnimation;
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadOnly)]
  public const string AnimationKey = "deployable_turret_animation";
  [DataField(null, false, 1, false, false, null)]
  public string DeployedState = "cover_open";
  [DataField(null, false, 1, false, false, null)]
  public string RetractedState = "cover_closed";
  [DataField(null, false, 1, false, false, null)]
  public string DeployingState = "cover_opening";
  [DataField(null, false, 1, false, false, null)]
  public string RetractingState = "cover_closing";

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref DeployableTurretComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (DeployableTurretComponent) target1;
    if (serialization.TryCustomCopy<DeployableTurretComponent>(this, ref target, hookCtx, false, context))
      return;
    bool target2 = false;
    if (!serialization.TryCustomCopy<bool>(this.Enabled, ref target2, hookCtx, false, context))
      target2 = this.Enabled;
    target.Enabled = target2;
    DeployableTurretState target3 = DeployableTurretState.Retracted;
    if (!serialization.TryCustomCopy<DeployableTurretState>(this.CurrentState, ref target3, hookCtx, false, context))
      target3 = this.CurrentState;
    target.CurrentState = target3;
    DeployableTurretState target4 = DeployableTurretState.Retracted;
    if (!serialization.TryCustomCopy<DeployableTurretState>(this.VisualState, ref target4, hookCtx, false, context))
      target4 = this.VisualState;
    target.VisualState = target4;
    string target5 = (string) null;
    if (!serialization.TryCustomCopy<string>(this.DeployedFixture, ref target5, hookCtx, false, context))
      target5 = this.DeployedFixture;
    target.DeployedFixture = target5;
    ProtoId<DamageModifierSetPrototype>? target6 = new ProtoId<DamageModifierSetPrototype>?();
    if (!serialization.TryCustomCopy<ProtoId<DamageModifierSetPrototype>?>(this.RetractedDamageModifierSetId, ref target6, hookCtx, false, context))
      target6 = serialization.CreateCopy<ProtoId<DamageModifierSetPrototype>?>(this.RetractedDamageModifierSetId, hookCtx, context);
    target.RetractedDamageModifierSetId = target6;
    ProtoId<DamageModifierSetPrototype>? target7 = new ProtoId<DamageModifierSetPrototype>?();
    if (!serialization.TryCustomCopy<ProtoId<DamageModifierSetPrototype>?>(this.DeployedDamageModifierSetId, ref target7, hookCtx, false, context))
      target7 = serialization.CreateCopy<ProtoId<DamageModifierSetPrototype>?>(this.DeployedDamageModifierSetId, hookCtx, context);
    target.DeployedDamageModifierSetId = target7;
    SoundSpecifier target8 = (SoundSpecifier) null;
    if (this.AccessDeniedSound == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.AccessDeniedSound, ref target8, hookCtx, true, context))
      target8 = serialization.CreateCopy<SoundSpecifier>(this.AccessDeniedSound, hookCtx, context);
    target.AccessDeniedSound = target8;
    SoundSpecifier target9 = (SoundSpecifier) null;
    if (this.DeploymentSound == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.DeploymentSound, ref target9, hookCtx, true, context))
      target9 = serialization.CreateCopy<SoundSpecifier>(this.DeploymentSound, hookCtx, context);
    target.DeploymentSound = target9;
    SoundSpecifier target10 = (SoundSpecifier) null;
    if (this.RetractionSound == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.RetractionSound, ref target10, hookCtx, true, context))
      target10 = serialization.CreateCopy<SoundSpecifier>(this.RetractionSound, hookCtx, context);
    target.RetractionSound = target10;
    float target11 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.DeploymentLength, ref target11, hookCtx, false, context))
      target11 = this.DeploymentLength;
    target.DeploymentLength = target11;
    float target12 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.RetractionLength, ref target12, hookCtx, false, context))
      target12 = this.RetractionLength;
    target.RetractionLength = target12;
    TimeSpan target13 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.AnimationCompletionTime, ref target13, hookCtx, false, context))
      target13 = serialization.CreateCopy<TimeSpan>(this.AnimationCompletionTime, hookCtx, context);
    target.AnimationCompletionTime = target13;
    string target14 = (string) null;
    if (this.DeployedState == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.DeployedState, ref target14, hookCtx, false, context))
      target14 = this.DeployedState;
    target.DeployedState = target14;
    string target15 = (string) null;
    if (this.RetractedState == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.RetractedState, ref target15, hookCtx, false, context))
      target15 = this.RetractedState;
    target.RetractedState = target15;
    string target16 = (string) null;
    if (this.DeployingState == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.DeployingState, ref target16, hookCtx, false, context))
      target16 = this.DeployingState;
    target.DeployingState = target16;
    string target17 = (string) null;
    if (this.RetractingState == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.RetractingState, ref target17, hookCtx, false, context))
      target17 = this.RetractingState;
    target.RetractingState = target17;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref DeployableTurretComponent target,
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
    DeployableTurretComponent target1 = (DeployableTurretComponent) target;
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
    DeployableTurretComponent target1 = (DeployableTurretComponent) target;
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
    DeployableTurretComponent target1 = (DeployableTurretComponent) target;
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

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref IComponentDelta target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    DeployableTurretComponent target1 = (DeployableTurretComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (IComponentDelta) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref IComponentDelta target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual DeployableTurretComponent Component.Instantiate() => new DeployableTurretComponent();

  IComponentDelta IComponentDelta.Instantiate() => (IComponentDelta) this.Instantiate();

  IComponentDelta ISerializationGenerated<IComponentDelta>.Instantiate()
  {
    return (IComponentDelta) this.Instantiate();
  }

  public GameTick LastFieldUpdate { get; set; } = GameTick.Zero;

  public GameTick[] LastModifiedFields { get; set; } = Array.Empty<GameTick>();

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class DeployableTurretComponent_AutoPauseSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<DeployableTurretComponent, EntityUnpausedEvent>(new ComponentEventRefHandler<DeployableTurretComponent, EntityUnpausedEvent>(this.OnEntityUnpaused));
    }

    private void OnEntityUnpaused(
      EntityUid uid,
      #nullable disable
      DeployableTurretComponent component,
      ref EntityUnpausedEvent args)
    {
      component.AnimationCompletionTime += args.PausedTime;
    }
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class DeployableTurretComponent_AutoState : IComponentState
  {
    public bool Enabled;
    public DeployableTurretState CurrentState;

    public 
    #nullable enable
    DeployableTurretComponent.DeployableTurretComponent_AutoState ShallowClone()
    {
      return new DeployableTurretComponent.DeployableTurretComponent_AutoState()
      {
        Enabled = this.Enabled,
        CurrentState = this.CurrentState
      };
    }
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class DeployableTurretComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.EntityManager.ComponentFactory.RegisterNetworkedFields<DeployableTurretComponent>("Enabled", "CurrentState");
      this.SubscribeLocalEvent<DeployableTurretComponent, ComponentGetState>(new ComponentEventRefHandler<DeployableTurretComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<DeployableTurretComponent, ComponentHandleState>(new ComponentEventRefHandler<DeployableTurretComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      DeployableTurretComponent component,
      ref ComponentGetState args)
    {
      IComponentDelta componentDelta = (IComponentDelta) component;
      if (componentDelta != null && args.FromTick > component.CreationTick && componentDelta.LastFieldUpdate >= args.FromTick)
      {
        switch (this.EntityManager.GetModifiedFields((IComponentDelta) component, args.FromTick))
        {
          case 1:
            args.State = (IComponentState) new DeployableTurretComponent.Enabled_FieldComponentState()
            {
              Enabled = component.Enabled
            };
            return;
          case 2:
            args.State = (IComponentState) new DeployableTurretComponent.CurrentState_FieldComponentState()
            {
              CurrentState = component.CurrentState
            };
            return;
        }
      }
      args.State = (IComponentState) new DeployableTurretComponent.DeployableTurretComponent_AutoState()
      {
        Enabled = component.Enabled,
        CurrentState = component.CurrentState
      };
    }

    private void OnHandleState(
      EntityUid uid,
      DeployableTurretComponent component,
      ref ComponentHandleState args)
    {
      switch (args.Current)
      {
        case DeployableTurretComponent.Enabled_FieldComponentState fieldComponentState1:
          component.Enabled = fieldComponentState1.Enabled;
          break;
        case DeployableTurretComponent.CurrentState_FieldComponentState fieldComponentState2:
          component.CurrentState = fieldComponentState2.CurrentState;
          break;
        case DeployableTurretComponent.DeployableTurretComponent_AutoState componentAutoState:
          component.Enabled = componentAutoState.Enabled;
          component.CurrentState = componentAutoState.CurrentState;
          break;
      }
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class Enabled_FieldComponentState : 
    IComponentDeltaState<DeployableTurretComponent.DeployableTurretComponent_AutoState>,
    IComponentDeltaState,
    IComponentState
  {
    public bool Enabled;

    public void ApplyToFullState(
      DeployableTurretComponent.DeployableTurretComponent_AutoState fullState)
    {
      fullState.Enabled = this.Enabled;
    }

    public DeployableTurretComponent.DeployableTurretComponent_AutoState CreateNewFullState(
      DeployableTurretComponent.DeployableTurretComponent_AutoState fullState)
    {
      DeployableTurretComponent.DeployableTurretComponent_AutoState fullState1 = fullState.ShallowClone();
      this.ApplyToFullState(fullState1);
      return fullState1;
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class CurrentState_FieldComponentState : 
    IComponentDeltaState<DeployableTurretComponent.DeployableTurretComponent_AutoState>,
    IComponentDeltaState,
    IComponentState
  {
    public DeployableTurretState CurrentState;

    public void ApplyToFullState(
      DeployableTurretComponent.DeployableTurretComponent_AutoState fullState)
    {
      fullState.CurrentState = this.CurrentState;
    }

    public DeployableTurretComponent.DeployableTurretComponent_AutoState CreateNewFullState(
      DeployableTurretComponent.DeployableTurretComponent_AutoState fullState)
    {
      DeployableTurretComponent.DeployableTurretComponent_AutoState fullState1 = fullState.ShallowClone();
      this.ApplyToFullState(fullState1);
      return fullState1;
    }
  }
}
