// Decompiled with JetBrains decompiler
// Type: Content.Shared._CIV14merka.Mortar.CivActiveMortarShellComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Map;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._CIV14merka.Mortar;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (SharedCivMortarSystem)})]
public sealed class CivActiveMortarShellComponent : 
  Component,
  ISerializationGenerated<CivActiveMortarShellComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntityCoordinates Coordinates;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan WarnAt;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool Warned;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float WarnRange = 15f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SoundSpecifier? WarnSound = (SoundSpecifier) new SoundPathSpecifier("/Audio/_RMC14/Weapons/gun_mortar_travel.ogg");
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan ImpactWarnAt;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool ImpactWarned;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float ImpactWarnRange = 10f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan LandAt;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool FiredFromRoof;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntityUid? MortarUid;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref CivActiveMortarShellComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (CivActiveMortarShellComponent) target1;
    if (serialization.TryCustomCopy<CivActiveMortarShellComponent>(this, ref target, hookCtx, false, context))
      return;
    EntityCoordinates target2 = new EntityCoordinates();
    if (!serialization.TryCustomCopy<EntityCoordinates>(this.Coordinates, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<EntityCoordinates>(this.Coordinates, hookCtx, context);
    target.Coordinates = target2;
    TimeSpan target3 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.WarnAt, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<TimeSpan>(this.WarnAt, hookCtx, context);
    target.WarnAt = target3;
    bool target4 = false;
    if (!serialization.TryCustomCopy<bool>(this.Warned, ref target4, hookCtx, false, context))
      target4 = this.Warned;
    target.Warned = target4;
    float target5 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.WarnRange, ref target5, hookCtx, false, context))
      target5 = this.WarnRange;
    target.WarnRange = target5;
    SoundSpecifier target6 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.WarnSound, ref target6, hookCtx, true, context))
      target6 = serialization.CreateCopy<SoundSpecifier>(this.WarnSound, hookCtx, context);
    target.WarnSound = target6;
    TimeSpan target7 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.ImpactWarnAt, ref target7, hookCtx, false, context))
      target7 = serialization.CreateCopy<TimeSpan>(this.ImpactWarnAt, hookCtx, context);
    target.ImpactWarnAt = target7;
    bool target8 = false;
    if (!serialization.TryCustomCopy<bool>(this.ImpactWarned, ref target8, hookCtx, false, context))
      target8 = this.ImpactWarned;
    target.ImpactWarned = target8;
    float target9 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.ImpactWarnRange, ref target9, hookCtx, false, context))
      target9 = this.ImpactWarnRange;
    target.ImpactWarnRange = target9;
    TimeSpan target10 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.LandAt, ref target10, hookCtx, false, context))
      target10 = serialization.CreateCopy<TimeSpan>(this.LandAt, hookCtx, context);
    target.LandAt = target10;
    bool target11 = false;
    if (!serialization.TryCustomCopy<bool>(this.FiredFromRoof, ref target11, hookCtx, false, context))
      target11 = this.FiredFromRoof;
    target.FiredFromRoof = target11;
    EntityUid? target12 = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.MortarUid, ref target12, hookCtx, false, context))
      target12 = serialization.CreateCopy<EntityUid?>(this.MortarUid, hookCtx, context);
    target.MortarUid = target12;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref CivActiveMortarShellComponent target,
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
    CivActiveMortarShellComponent target1 = (CivActiveMortarShellComponent) target;
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
    CivActiveMortarShellComponent target1 = (CivActiveMortarShellComponent) target;
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
    CivActiveMortarShellComponent target1 = (CivActiveMortarShellComponent) target;
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
  virtual CivActiveMortarShellComponent Component.Instantiate()
  {
    return new CivActiveMortarShellComponent();
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class CivActiveMortarShellComponent_AutoState : IComponentState
  {
    public NetCoordinates Coordinates;
    public TimeSpan WarnAt;
    public bool Warned;
    public float WarnRange;
    public SoundSpecifier? WarnSound;
    public TimeSpan ImpactWarnAt;
    public bool ImpactWarned;
    public float ImpactWarnRange;
    public TimeSpan LandAt;
    public bool FiredFromRoof;
    public NetEntity? MortarUid;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class CivActiveMortarShellComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<CivActiveMortarShellComponent, ComponentGetState>(new ComponentEventRefHandler<CivActiveMortarShellComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<CivActiveMortarShellComponent, ComponentHandleState>(new ComponentEventRefHandler<CivActiveMortarShellComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      CivActiveMortarShellComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new CivActiveMortarShellComponent.CivActiveMortarShellComponent_AutoState()
      {
        Coordinates = this.GetNetCoordinates(component.Coordinates),
        WarnAt = component.WarnAt,
        Warned = component.Warned,
        WarnRange = component.WarnRange,
        WarnSound = component.WarnSound,
        ImpactWarnAt = component.ImpactWarnAt,
        ImpactWarned = component.ImpactWarned,
        ImpactWarnRange = component.ImpactWarnRange,
        LandAt = component.LandAt,
        FiredFromRoof = component.FiredFromRoof,
        MortarUid = this.GetNetEntity(component.MortarUid)
      };
    }

    private void OnHandleState(
      EntityUid uid,
      CivActiveMortarShellComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is CivActiveMortarShellComponent.CivActiveMortarShellComponent_AutoState current))
        return;
      component.Coordinates = this.EnsureCoordinates<CivActiveMortarShellComponent>(current.Coordinates, uid);
      component.WarnAt = current.WarnAt;
      component.Warned = current.Warned;
      component.WarnRange = current.WarnRange;
      component.WarnSound = current.WarnSound;
      component.ImpactWarnAt = current.ImpactWarnAt;
      component.ImpactWarned = current.ImpactWarned;
      component.ImpactWarnRange = current.ImpactWarnRange;
      component.LandAt = current.LandAt;
      component.FiredFromRoof = current.FiredFromRoof;
      component.MortarUid = this.EnsureEntity<CivActiveMortarShellComponent>(current.MortarUid, uid);
    }
  }
}
