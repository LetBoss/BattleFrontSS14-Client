// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Crippling.XenoActiveCripplingStrikeComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Localization;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Xenonids.Crippling;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[AutoGenerateComponentPause]
[Access(new Type[] {typeof (XenoCripplingStrikeSystem)})]
public sealed class XenoActiveCripplingStrikeComponent : 
  Component,
  ISerializationGenerated<XenoActiveCripplingStrikeComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, typeof (TimeOffsetSerializer))]
  [AutoNetworkedField]
  [AutoPausedField]
  public TimeSpan ExpireAt;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool NextSlashBuffed = true;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan SlowDuration = TimeSpan.FromSeconds(5L);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float DamageMult = 1.2f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public LocId HitText = (LocId) "cm-xeno-crippling-strike-hit";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public LocId? DeactivateText;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public LocId ExpireText = (LocId) "cm-xeno-crippling-strike-expire";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float? Speed;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool RemoveOnHit;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool PreventTackle;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref XenoActiveCripplingStrikeComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (XenoActiveCripplingStrikeComponent) target1;
    if (serialization.TryCustomCopy<XenoActiveCripplingStrikeComponent>(this, ref target, hookCtx, false, context))
      return;
    TimeSpan target2 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.ExpireAt, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<TimeSpan>(this.ExpireAt, hookCtx, context);
    target.ExpireAt = target2;
    bool target3 = false;
    if (!serialization.TryCustomCopy<bool>(this.NextSlashBuffed, ref target3, hookCtx, false, context))
      target3 = this.NextSlashBuffed;
    target.NextSlashBuffed = target3;
    TimeSpan target4 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.SlowDuration, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<TimeSpan>(this.SlowDuration, hookCtx, context);
    target.SlowDuration = target4;
    float target5 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.DamageMult, ref target5, hookCtx, false, context))
      target5 = this.DamageMult;
    target.DamageMult = target5;
    LocId target6 = new LocId();
    if (!serialization.TryCustomCopy<LocId>(this.HitText, ref target6, hookCtx, false, context))
      target6 = serialization.CreateCopy<LocId>(this.HitText, hookCtx, context);
    target.HitText = target6;
    LocId? target7 = new LocId?();
    if (!serialization.TryCustomCopy<LocId?>(this.DeactivateText, ref target7, hookCtx, false, context))
      target7 = serialization.CreateCopy<LocId?>(this.DeactivateText, hookCtx, context);
    target.DeactivateText = target7;
    LocId target8 = new LocId();
    if (!serialization.TryCustomCopy<LocId>(this.ExpireText, ref target8, hookCtx, false, context))
      target8 = serialization.CreateCopy<LocId>(this.ExpireText, hookCtx, context);
    target.ExpireText = target8;
    float? target9 = new float?();
    if (!serialization.TryCustomCopy<float?>(this.Speed, ref target9, hookCtx, false, context))
      target9 = this.Speed;
    target.Speed = target9;
    bool target10 = false;
    if (!serialization.TryCustomCopy<bool>(this.RemoveOnHit, ref target10, hookCtx, false, context))
      target10 = this.RemoveOnHit;
    target.RemoveOnHit = target10;
    bool target11 = false;
    if (!serialization.TryCustomCopy<bool>(this.PreventTackle, ref target11, hookCtx, false, context))
      target11 = this.PreventTackle;
    target.PreventTackle = target11;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref XenoActiveCripplingStrikeComponent target,
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
    XenoActiveCripplingStrikeComponent target1 = (XenoActiveCripplingStrikeComponent) target;
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
    XenoActiveCripplingStrikeComponent target1 = (XenoActiveCripplingStrikeComponent) target;
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
    XenoActiveCripplingStrikeComponent target1 = (XenoActiveCripplingStrikeComponent) target;
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
  virtual XenoActiveCripplingStrikeComponent Component.Instantiate()
  {
    return new XenoActiveCripplingStrikeComponent();
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class XenoActiveCripplingStrikeComponent_AutoPauseSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<XenoActiveCripplingStrikeComponent, EntityUnpausedEvent>(new ComponentEventRefHandler<XenoActiveCripplingStrikeComponent, EntityUnpausedEvent>(this.OnEntityUnpaused));
    }

    private void OnEntityUnpaused(
      EntityUid uid,
      #nullable disable
      XenoActiveCripplingStrikeComponent component,
      ref EntityUnpausedEvent args)
    {
      component.ExpireAt += args.PausedTime;
      this.Dirty(uid, (IComponent) component);
    }
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class XenoActiveCripplingStrikeComponent_AutoState : IComponentState
  {
    public TimeSpan ExpireAt;
    public bool NextSlashBuffed;
    public TimeSpan SlowDuration;
    public float DamageMult;
    public LocId HitText;
    public LocId? DeactivateText;
    public LocId ExpireText;
    public float? Speed;
    public bool RemoveOnHit;
    public bool PreventTackle;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class XenoActiveCripplingStrikeComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<XenoActiveCripplingStrikeComponent, ComponentGetState>(new ComponentEventRefHandler<XenoActiveCripplingStrikeComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<XenoActiveCripplingStrikeComponent, ComponentHandleState>(new ComponentEventRefHandler<XenoActiveCripplingStrikeComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      #nullable enable
      XenoActiveCripplingStrikeComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new XenoActiveCripplingStrikeComponent.XenoActiveCripplingStrikeComponent_AutoState()
      {
        ExpireAt = component.ExpireAt,
        NextSlashBuffed = component.NextSlashBuffed,
        SlowDuration = component.SlowDuration,
        DamageMult = component.DamageMult,
        HitText = component.HitText,
        DeactivateText = component.DeactivateText,
        ExpireText = component.ExpireText,
        Speed = component.Speed,
        RemoveOnHit = component.RemoveOnHit,
        PreventTackle = component.PreventTackle
      };
    }

    private void OnHandleState(
      EntityUid uid,
      XenoActiveCripplingStrikeComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is XenoActiveCripplingStrikeComponent.XenoActiveCripplingStrikeComponent_AutoState current))
        return;
      component.ExpireAt = current.ExpireAt;
      component.NextSlashBuffed = current.NextSlashBuffed;
      component.SlowDuration = current.SlowDuration;
      component.DamageMult = current.DamageMult;
      component.HitText = current.HitText;
      component.DeactivateText = current.DeactivateText;
      component.ExpireText = current.ExpireText;
      component.Speed = current.Speed;
      component.RemoveOnHit = current.RemoveOnHit;
      component.PreventTackle = current.PreventTackle;
    }
  }
}
