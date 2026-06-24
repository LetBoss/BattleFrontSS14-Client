// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Soak.XenoSoakingDamageComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.FixedPoint;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Xenonids.Soak;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
public sealed class XenoSoakingDamageComponent : 
  Component,
  ISerializationGenerated<XenoSoakingDamageComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float DamageAccumulated;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int DamageGoal = 140;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public FixedPoint2 Heal = FixedPoint2.New(75);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan EffectExpiresAt;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public Color SoakColor = Color.FromHex((ReadOnlySpan<char>) "#421313", new Color?());
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public Color RageColor = Color.FromHex((ReadOnlySpan<char>) "#AD1313", new Color?());
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan RageDuration = TimeSpan.FromSeconds(3L);

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref XenoSoakingDamageComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (XenoSoakingDamageComponent) target1;
    if (serialization.TryCustomCopy<XenoSoakingDamageComponent>(this, ref target, hookCtx, false, context))
      return;
    float target2 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.DamageAccumulated, ref target2, hookCtx, false, context))
      target2 = this.DamageAccumulated;
    target.DamageAccumulated = target2;
    int target3 = 0;
    if (!serialization.TryCustomCopy<int>(this.DamageGoal, ref target3, hookCtx, false, context))
      target3 = this.DamageGoal;
    target.DamageGoal = target3;
    FixedPoint2 target4 = new FixedPoint2();
    if (!serialization.TryCustomCopy<FixedPoint2>(this.Heal, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<FixedPoint2>(this.Heal, hookCtx, context);
    target.Heal = target4;
    TimeSpan target5 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.EffectExpiresAt, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<TimeSpan>(this.EffectExpiresAt, hookCtx, context);
    target.EffectExpiresAt = target5;
    Color target6 = new Color();
    if (!serialization.TryCustomCopy<Color>(this.SoakColor, ref target6, hookCtx, false, context))
      target6 = serialization.CreateCopy<Color>(this.SoakColor, hookCtx, context);
    target.SoakColor = target6;
    Color target7 = new Color();
    if (!serialization.TryCustomCopy<Color>(this.RageColor, ref target7, hookCtx, false, context))
      target7 = serialization.CreateCopy<Color>(this.RageColor, hookCtx, context);
    target.RageColor = target7;
    TimeSpan target8 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.RageDuration, ref target8, hookCtx, false, context))
      target8 = serialization.CreateCopy<TimeSpan>(this.RageDuration, hookCtx, context);
    target.RageDuration = target8;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref XenoSoakingDamageComponent target,
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
    XenoSoakingDamageComponent target1 = (XenoSoakingDamageComponent) target;
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
    XenoSoakingDamageComponent target1 = (XenoSoakingDamageComponent) target;
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
    XenoSoakingDamageComponent target1 = (XenoSoakingDamageComponent) target;
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
  virtual XenoSoakingDamageComponent Component.Instantiate() => new XenoSoakingDamageComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class XenoSoakingDamageComponent_AutoState : IComponentState
  {
    public float DamageAccumulated;
    public int DamageGoal;
    public FixedPoint2 Heal;
    public TimeSpan EffectExpiresAt;
    public Color SoakColor;
    public Color RageColor;
    public TimeSpan RageDuration;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class XenoSoakingDamageComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<XenoSoakingDamageComponent, ComponentGetState>(new ComponentEventRefHandler<XenoSoakingDamageComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<XenoSoakingDamageComponent, ComponentHandleState>(new ComponentEventRefHandler<XenoSoakingDamageComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      XenoSoakingDamageComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new XenoSoakingDamageComponent.XenoSoakingDamageComponent_AutoState()
      {
        DamageAccumulated = component.DamageAccumulated,
        DamageGoal = component.DamageGoal,
        Heal = component.Heal,
        EffectExpiresAt = component.EffectExpiresAt,
        SoakColor = component.SoakColor,
        RageColor = component.RageColor,
        RageDuration = component.RageDuration
      };
    }

    private void OnHandleState(
      EntityUid uid,
      XenoSoakingDamageComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is XenoSoakingDamageComponent.XenoSoakingDamageComponent_AutoState current))
        return;
      component.DamageAccumulated = current.DamageAccumulated;
      component.DamageGoal = current.DamageGoal;
      component.Heal = current.Heal;
      component.EffectExpiresAt = current.EffectExpiresAt;
      component.SoakColor = current.SoakColor;
      component.RageColor = current.RageColor;
      component.RageDuration = current.RageDuration;
    }
  }
}
