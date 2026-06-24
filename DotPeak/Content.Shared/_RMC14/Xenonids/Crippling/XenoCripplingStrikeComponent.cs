// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Crippling.XenoCripplingStrikeComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Localization;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Xenonids.Crippling;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (XenoCripplingStrikeSystem)})]
public sealed class XenoCripplingStrikeComponent : 
  Component,
  ISerializationGenerated<XenoCripplingStrikeComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float DamageMult = 1.2f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan ActiveDuration = TimeSpan.FromSeconds(5L);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan SlowDuration = TimeSpan.FromSeconds(5L);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public Color? AuraColor;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public LocId ActivateText = (LocId) "cm-xeno-crippling-strike-activate";
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
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool ResetMeleeCooldown;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref XenoCripplingStrikeComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (XenoCripplingStrikeComponent) target1;
    if (serialization.TryCustomCopy<XenoCripplingStrikeComponent>(this, ref target, hookCtx, false, context))
      return;
    float target2 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.DamageMult, ref target2, hookCtx, false, context))
      target2 = this.DamageMult;
    target.DamageMult = target2;
    TimeSpan target3 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.ActiveDuration, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<TimeSpan>(this.ActiveDuration, hookCtx, context);
    target.ActiveDuration = target3;
    TimeSpan target4 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.SlowDuration, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<TimeSpan>(this.SlowDuration, hookCtx, context);
    target.SlowDuration = target4;
    Color? target5 = new Color?();
    if (!serialization.TryCustomCopy<Color?>(this.AuraColor, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<Color?>(this.AuraColor, hookCtx, context);
    target.AuraColor = target5;
    LocId target6 = new LocId();
    if (!serialization.TryCustomCopy<LocId>(this.ActivateText, ref target6, hookCtx, false, context))
      target6 = serialization.CreateCopy<LocId>(this.ActivateText, hookCtx, context);
    target.ActivateText = target6;
    LocId target7 = new LocId();
    if (!serialization.TryCustomCopy<LocId>(this.HitText, ref target7, hookCtx, false, context))
      target7 = serialization.CreateCopy<LocId>(this.HitText, hookCtx, context);
    target.HitText = target7;
    LocId? target8 = new LocId?();
    if (!serialization.TryCustomCopy<LocId?>(this.DeactivateText, ref target8, hookCtx, false, context))
      target8 = serialization.CreateCopy<LocId?>(this.DeactivateText, hookCtx, context);
    target.DeactivateText = target8;
    LocId target9 = new LocId();
    if (!serialization.TryCustomCopy<LocId>(this.ExpireText, ref target9, hookCtx, false, context))
      target9 = serialization.CreateCopy<LocId>(this.ExpireText, hookCtx, context);
    target.ExpireText = target9;
    float? target10 = new float?();
    if (!serialization.TryCustomCopy<float?>(this.Speed, ref target10, hookCtx, false, context))
      target10 = this.Speed;
    target.Speed = target10;
    bool target11 = false;
    if (!serialization.TryCustomCopy<bool>(this.RemoveOnHit, ref target11, hookCtx, false, context))
      target11 = this.RemoveOnHit;
    target.RemoveOnHit = target11;
    bool target12 = false;
    if (!serialization.TryCustomCopy<bool>(this.PreventTackle, ref target12, hookCtx, false, context))
      target12 = this.PreventTackle;
    target.PreventTackle = target12;
    bool target13 = false;
    if (!serialization.TryCustomCopy<bool>(this.ResetMeleeCooldown, ref target13, hookCtx, false, context))
      target13 = this.ResetMeleeCooldown;
    target.ResetMeleeCooldown = target13;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref XenoCripplingStrikeComponent target,
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
    XenoCripplingStrikeComponent target1 = (XenoCripplingStrikeComponent) target;
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
    XenoCripplingStrikeComponent target1 = (XenoCripplingStrikeComponent) target;
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
    XenoCripplingStrikeComponent target1 = (XenoCripplingStrikeComponent) target;
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
  virtual XenoCripplingStrikeComponent Component.Instantiate()
  {
    return new XenoCripplingStrikeComponent();
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class XenoCripplingStrikeComponent_AutoState : IComponentState
  {
    public float DamageMult;
    public TimeSpan ActiveDuration;
    public TimeSpan SlowDuration;
    public Color? AuraColor;
    public LocId ActivateText;
    public LocId HitText;
    public LocId? DeactivateText;
    public LocId ExpireText;
    public float? Speed;
    public bool RemoveOnHit;
    public bool PreventTackle;
    public bool ResetMeleeCooldown;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class XenoCripplingStrikeComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<XenoCripplingStrikeComponent, ComponentGetState>(new ComponentEventRefHandler<XenoCripplingStrikeComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<XenoCripplingStrikeComponent, ComponentHandleState>(new ComponentEventRefHandler<XenoCripplingStrikeComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      XenoCripplingStrikeComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new XenoCripplingStrikeComponent.XenoCripplingStrikeComponent_AutoState()
      {
        DamageMult = component.DamageMult,
        ActiveDuration = component.ActiveDuration,
        SlowDuration = component.SlowDuration,
        AuraColor = component.AuraColor,
        ActivateText = component.ActivateText,
        HitText = component.HitText,
        DeactivateText = component.DeactivateText,
        ExpireText = component.ExpireText,
        Speed = component.Speed,
        RemoveOnHit = component.RemoveOnHit,
        PreventTackle = component.PreventTackle,
        ResetMeleeCooldown = component.ResetMeleeCooldown
      };
    }

    private void OnHandleState(
      EntityUid uid,
      XenoCripplingStrikeComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is XenoCripplingStrikeComponent.XenoCripplingStrikeComponent_AutoState current))
        return;
      component.DamageMult = current.DamageMult;
      component.ActiveDuration = current.ActiveDuration;
      component.SlowDuration = current.SlowDuration;
      component.AuraColor = current.AuraColor;
      component.ActivateText = current.ActivateText;
      component.HitText = current.HitText;
      component.DeactivateText = current.DeactivateText;
      component.ExpireText = current.ExpireText;
      component.Speed = current.Speed;
      component.RemoveOnHit = current.RemoveOnHit;
      component.PreventTackle = current.PreventTackle;
      component.ResetMeleeCooldown = current.ResetMeleeCooldown;
    }
  }
}
