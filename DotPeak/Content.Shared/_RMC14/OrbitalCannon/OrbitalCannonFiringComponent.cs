// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.OrbitalCannon.OrbitalCannonFiringComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.OrbitalCannon;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[AutoGenerateComponentPause]
[Access(new Type[] {typeof (OrbitalCannonSystem)})]
public sealed class OrbitalCannonFiringComponent : 
  Component,
  ISerializationGenerated<OrbitalCannonFiringComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public Vector2i Coordinates;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public string WarheadName;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntityUid Squad;
  [DataField(null, false, 1, false, false, typeof (TimeOffsetSerializer))]
  [AutoNetworkedField]
  [AutoPausedField]
  public TimeSpan StartedAt;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan AlertDelay = TimeSpan.FromSeconds(2L);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool Alerted;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan BeginFireDelay = TimeSpan.FromSeconds(6L);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool BegunFire;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan FireDelay = TimeSpan.FromSeconds(12L);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool Fired;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan WarnOneDelay = TimeSpan.FromSeconds(16L /*0x10*/);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool WarnedOne;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan WarnTwoDelay = TimeSpan.FromSeconds(20L);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool WarnedTwo;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan ImpactDelay = TimeSpan.FromSeconds(24L);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool AegisBoomed;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool Impacted;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int FirstWarningRange = 30;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int SecondWarningRange = 25;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int ThirdWarningRange = 15;

  public TimeSpan AegisBoomDelay => TimeSpan.FromSeconds(19.5);

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref OrbitalCannonFiringComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (OrbitalCannonFiringComponent) target1;
    if (serialization.TryCustomCopy<OrbitalCannonFiringComponent>(this, ref target, hookCtx, false, context))
      return;
    Vector2i target2 = new Vector2i();
    if (!serialization.TryCustomCopy<Vector2i>(this.Coordinates, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<Vector2i>(this.Coordinates, hookCtx, context);
    target.Coordinates = target2;
    string target3 = (string) null;
    if (this.WarheadName == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.WarheadName, ref target3, hookCtx, false, context))
      target3 = this.WarheadName;
    target.WarheadName = target3;
    EntityUid target4 = new EntityUid();
    if (!serialization.TryCustomCopy<EntityUid>(this.Squad, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<EntityUid>(this.Squad, hookCtx, context);
    target.Squad = target4;
    TimeSpan target5 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.StartedAt, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<TimeSpan>(this.StartedAt, hookCtx, context);
    target.StartedAt = target5;
    TimeSpan target6 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.AlertDelay, ref target6, hookCtx, false, context))
      target6 = serialization.CreateCopy<TimeSpan>(this.AlertDelay, hookCtx, context);
    target.AlertDelay = target6;
    bool target7 = false;
    if (!serialization.TryCustomCopy<bool>(this.Alerted, ref target7, hookCtx, false, context))
      target7 = this.Alerted;
    target.Alerted = target7;
    TimeSpan target8 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.BeginFireDelay, ref target8, hookCtx, false, context))
      target8 = serialization.CreateCopy<TimeSpan>(this.BeginFireDelay, hookCtx, context);
    target.BeginFireDelay = target8;
    bool target9 = false;
    if (!serialization.TryCustomCopy<bool>(this.BegunFire, ref target9, hookCtx, false, context))
      target9 = this.BegunFire;
    target.BegunFire = target9;
    TimeSpan target10 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.FireDelay, ref target10, hookCtx, false, context))
      target10 = serialization.CreateCopy<TimeSpan>(this.FireDelay, hookCtx, context);
    target.FireDelay = target10;
    bool target11 = false;
    if (!serialization.TryCustomCopy<bool>(this.Fired, ref target11, hookCtx, false, context))
      target11 = this.Fired;
    target.Fired = target11;
    TimeSpan target12 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.WarnOneDelay, ref target12, hookCtx, false, context))
      target12 = serialization.CreateCopy<TimeSpan>(this.WarnOneDelay, hookCtx, context);
    target.WarnOneDelay = target12;
    bool target13 = false;
    if (!serialization.TryCustomCopy<bool>(this.WarnedOne, ref target13, hookCtx, false, context))
      target13 = this.WarnedOne;
    target.WarnedOne = target13;
    TimeSpan target14 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.WarnTwoDelay, ref target14, hookCtx, false, context))
      target14 = serialization.CreateCopy<TimeSpan>(this.WarnTwoDelay, hookCtx, context);
    target.WarnTwoDelay = target14;
    bool target15 = false;
    if (!serialization.TryCustomCopy<bool>(this.WarnedTwo, ref target15, hookCtx, false, context))
      target15 = this.WarnedTwo;
    target.WarnedTwo = target15;
    TimeSpan target16 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.ImpactDelay, ref target16, hookCtx, false, context))
      target16 = serialization.CreateCopy<TimeSpan>(this.ImpactDelay, hookCtx, context);
    target.ImpactDelay = target16;
    bool target17 = false;
    if (!serialization.TryCustomCopy<bool>(this.AegisBoomed, ref target17, hookCtx, false, context))
      target17 = this.AegisBoomed;
    target.AegisBoomed = target17;
    bool target18 = false;
    if (!serialization.TryCustomCopy<bool>(this.Impacted, ref target18, hookCtx, false, context))
      target18 = this.Impacted;
    target.Impacted = target18;
    int target19 = 0;
    if (!serialization.TryCustomCopy<int>(this.FirstWarningRange, ref target19, hookCtx, false, context))
      target19 = this.FirstWarningRange;
    target.FirstWarningRange = target19;
    int target20 = 0;
    if (!serialization.TryCustomCopy<int>(this.SecondWarningRange, ref target20, hookCtx, false, context))
      target20 = this.SecondWarningRange;
    target.SecondWarningRange = target20;
    int target21 = 0;
    if (!serialization.TryCustomCopy<int>(this.ThirdWarningRange, ref target21, hookCtx, false, context))
      target21 = this.ThirdWarningRange;
    target.ThirdWarningRange = target21;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref OrbitalCannonFiringComponent target,
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
    OrbitalCannonFiringComponent target1 = (OrbitalCannonFiringComponent) target;
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
    OrbitalCannonFiringComponent target1 = (OrbitalCannonFiringComponent) target;
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
    OrbitalCannonFiringComponent target1 = (OrbitalCannonFiringComponent) target;
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
  virtual OrbitalCannonFiringComponent Component.Instantiate()
  {
    return new OrbitalCannonFiringComponent();
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class OrbitalCannonFiringComponent_AutoPauseSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<OrbitalCannonFiringComponent, EntityUnpausedEvent>(new ComponentEventRefHandler<OrbitalCannonFiringComponent, EntityUnpausedEvent>(this.OnEntityUnpaused));
    }

    private void OnEntityUnpaused(
      EntityUid uid,
      #nullable disable
      OrbitalCannonFiringComponent component,
      ref EntityUnpausedEvent args)
    {
      component.StartedAt += args.PausedTime;
      this.Dirty(uid, (IComponent) component);
    }
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class OrbitalCannonFiringComponent_AutoState : IComponentState
  {
    public Vector2i Coordinates;
    public 
    #nullable enable
    string WarheadName;
    public NetEntity Squad;
    public TimeSpan StartedAt;
    public TimeSpan AlertDelay;
    public bool Alerted;
    public TimeSpan BeginFireDelay;
    public bool BegunFire;
    public TimeSpan FireDelay;
    public bool Fired;
    public TimeSpan WarnOneDelay;
    public bool WarnedOne;
    public TimeSpan WarnTwoDelay;
    public bool WarnedTwo;
    public TimeSpan ImpactDelay;
    public bool AegisBoomed;
    public bool Impacted;
    public int FirstWarningRange;
    public int SecondWarningRange;
    public int ThirdWarningRange;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class OrbitalCannonFiringComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<OrbitalCannonFiringComponent, ComponentGetState>(new ComponentEventRefHandler<OrbitalCannonFiringComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<OrbitalCannonFiringComponent, ComponentHandleState>(new ComponentEventRefHandler<OrbitalCannonFiringComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      OrbitalCannonFiringComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new OrbitalCannonFiringComponent.OrbitalCannonFiringComponent_AutoState()
      {
        Coordinates = component.Coordinates,
        WarheadName = component.WarheadName,
        Squad = this.GetNetEntity(component.Squad),
        StartedAt = component.StartedAt,
        AlertDelay = component.AlertDelay,
        Alerted = component.Alerted,
        BeginFireDelay = component.BeginFireDelay,
        BegunFire = component.BegunFire,
        FireDelay = component.FireDelay,
        Fired = component.Fired,
        WarnOneDelay = component.WarnOneDelay,
        WarnedOne = component.WarnedOne,
        WarnTwoDelay = component.WarnTwoDelay,
        WarnedTwo = component.WarnedTwo,
        ImpactDelay = component.ImpactDelay,
        AegisBoomed = component.AegisBoomed,
        Impacted = component.Impacted,
        FirstWarningRange = component.FirstWarningRange,
        SecondWarningRange = component.SecondWarningRange,
        ThirdWarningRange = component.ThirdWarningRange
      };
    }

    private void OnHandleState(
      EntityUid uid,
      OrbitalCannonFiringComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is OrbitalCannonFiringComponent.OrbitalCannonFiringComponent_AutoState current))
        return;
      component.Coordinates = current.Coordinates;
      component.WarheadName = current.WarheadName;
      component.Squad = this.EnsureEntity<OrbitalCannonFiringComponent>(current.Squad, uid);
      component.StartedAt = current.StartedAt;
      component.AlertDelay = current.AlertDelay;
      component.Alerted = current.Alerted;
      component.BeginFireDelay = current.BeginFireDelay;
      component.BegunFire = current.BegunFire;
      component.FireDelay = current.FireDelay;
      component.Fired = current.Fired;
      component.WarnOneDelay = current.WarnOneDelay;
      component.WarnedOne = current.WarnedOne;
      component.WarnTwoDelay = current.WarnTwoDelay;
      component.WarnedTwo = current.WarnedTwo;
      component.ImpactDelay = current.ImpactDelay;
      component.AegisBoomed = current.AegisBoomed;
      component.Impacted = current.Impacted;
      component.FirstWarningRange = current.FirstWarningRange;
      component.SecondWarningRange = current.SecondWarningRange;
      component.ThirdWarningRange = current.ThirdWarningRange;
    }
  }
}
