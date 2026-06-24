// Decompiled with JetBrains decompiler
// Type: Content.Shared.Singularity.Components.SingularityGeneratorComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Singularity.Components;

[RegisterComponent]
[AutoGenerateComponentPause]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
public sealed class SingularityGeneratorComponent : 
  Component,
  ISerializationGenerated<SingularityGeneratorComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public float Power;
  [DataField(null, false, 1, false, false, null)]
  public float Threshold = 16f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool FailsafeDisabled;
  [DataField(null, false, 1, false, false, null)]
  public float FailsafeDistance = 16f;
  [DataField("spawnId", false, 1, false, false, typeof (PrototypeIdSerializer<EntityPrototype>))]
  public string? SpawnPrototype = "Singularity";
  [DataField(null, false, 1, false, false, null)]
  public int CollisionMask = 158;
  [DataField(null, false, 1, false, false, null)]
  public LocId ContainmentFailsafeMessage = (LocId) "comp-generator-failsafe";
  [DataField(null, false, 1, false, false, null)]
  public TimeSpan FailsafeCooldown = TimeSpan.FromSeconds(10L);
  [DataField(null, false, 1, false, false, typeof (TimeOffsetSerializer))]
  [AutoPausedField]
  public TimeSpan NextFailsafe = TimeSpan.Zero;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref SingularityGeneratorComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (SingularityGeneratorComponent) target1;
    if (serialization.TryCustomCopy<SingularityGeneratorComponent>(this, ref target, hookCtx, false, context))
      return;
    float target2 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.Power, ref target2, hookCtx, false, context))
      target2 = this.Power;
    target.Power = target2;
    float target3 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.Threshold, ref target3, hookCtx, false, context))
      target3 = this.Threshold;
    target.Threshold = target3;
    bool target4 = false;
    if (!serialization.TryCustomCopy<bool>(this.FailsafeDisabled, ref target4, hookCtx, false, context))
      target4 = this.FailsafeDisabled;
    target.FailsafeDisabled = target4;
    float target5 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.FailsafeDistance, ref target5, hookCtx, false, context))
      target5 = this.FailsafeDistance;
    target.FailsafeDistance = target5;
    string target6 = (string) null;
    if (!serialization.TryCustomCopy<string>(this.SpawnPrototype, ref target6, hookCtx, false, context))
      target6 = this.SpawnPrototype;
    target.SpawnPrototype = target6;
    int target7 = 0;
    if (!serialization.TryCustomCopy<int>(this.CollisionMask, ref target7, hookCtx, false, context))
      target7 = this.CollisionMask;
    target.CollisionMask = target7;
    LocId target8 = new LocId();
    if (!serialization.TryCustomCopy<LocId>(this.ContainmentFailsafeMessage, ref target8, hookCtx, false, context))
      target8 = serialization.CreateCopy<LocId>(this.ContainmentFailsafeMessage, hookCtx, context);
    target.ContainmentFailsafeMessage = target8;
    TimeSpan target9 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.FailsafeCooldown, ref target9, hookCtx, false, context))
      target9 = serialization.CreateCopy<TimeSpan>(this.FailsafeCooldown, hookCtx, context);
    target.FailsafeCooldown = target9;
    TimeSpan target10 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.NextFailsafe, ref target10, hookCtx, false, context))
      target10 = serialization.CreateCopy<TimeSpan>(this.NextFailsafe, hookCtx, context);
    target.NextFailsafe = target10;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref SingularityGeneratorComponent target,
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
    SingularityGeneratorComponent target1 = (SingularityGeneratorComponent) target;
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
    SingularityGeneratorComponent target1 = (SingularityGeneratorComponent) target;
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
    SingularityGeneratorComponent target1 = (SingularityGeneratorComponent) target;
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
  virtual SingularityGeneratorComponent Component.Instantiate()
  {
    return new SingularityGeneratorComponent();
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class SingularityGeneratorComponent_AutoPauseSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<SingularityGeneratorComponent, EntityUnpausedEvent>(new ComponentEventRefHandler<SingularityGeneratorComponent, EntityUnpausedEvent>(this.OnEntityUnpaused));
    }

    private void OnEntityUnpaused(
      EntityUid uid,
      #nullable disable
      SingularityGeneratorComponent component,
      ref EntityUnpausedEvent args)
    {
      component.NextFailsafe += args.PausedTime;
    }
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class SingularityGeneratorComponent_AutoState : IComponentState
  {
    public bool FailsafeDisabled;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class SingularityGeneratorComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<SingularityGeneratorComponent, ComponentGetState>(new ComponentEventRefHandler<SingularityGeneratorComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<SingularityGeneratorComponent, ComponentHandleState>(new ComponentEventRefHandler<SingularityGeneratorComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      #nullable enable
      SingularityGeneratorComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new SingularityGeneratorComponent.SingularityGeneratorComponent_AutoState()
      {
        FailsafeDisabled = component.FailsafeDisabled
      };
    }

    private void OnHandleState(
      EntityUid uid,
      SingularityGeneratorComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is SingularityGeneratorComponent.SingularityGeneratorComponent_AutoState current))
        return;
      component.FailsafeDisabled = current.FailsafeDisabled;
    }
  }
}
