// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Fury.XenoFuryComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Maths;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Xenonids.Fury;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
public sealed class XenoFuryComponent : 
  Component,
  ISerializationGenerated<XenoFuryComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int Heal = 15;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int BoostedHeal = 25;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntProtoId Effect = (EntProtoId) "RMCEffectHeal";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float Range = RMCMathExtensions.CircleAreaFromSquareAbilityRange(3f);

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref XenoFuryComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (XenoFuryComponent) target1;
    if (serialization.TryCustomCopy<XenoFuryComponent>(this, ref target, hookCtx, false, context))
      return;
    int target2 = 0;
    if (!serialization.TryCustomCopy<int>(this.Heal, ref target2, hookCtx, false, context))
      target2 = this.Heal;
    target.Heal = target2;
    int target3 = 0;
    if (!serialization.TryCustomCopy<int>(this.BoostedHeal, ref target3, hookCtx, false, context))
      target3 = this.BoostedHeal;
    target.BoostedHeal = target3;
    EntProtoId target4 = new EntProtoId();
    if (!serialization.TryCustomCopy<EntProtoId>(this.Effect, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<EntProtoId>(this.Effect, hookCtx, context);
    target.Effect = target4;
    float target5 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.Range, ref target5, hookCtx, false, context))
      target5 = this.Range;
    target.Range = target5;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref XenoFuryComponent target,
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
    XenoFuryComponent target1 = (XenoFuryComponent) target;
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
    XenoFuryComponent target1 = (XenoFuryComponent) target;
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
    XenoFuryComponent target1 = (XenoFuryComponent) target;
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
  virtual XenoFuryComponent Component.Instantiate() => new XenoFuryComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class XenoFuryComponent_AutoState : IComponentState
  {
    public int Heal;
    public int BoostedHeal;
    public EntProtoId Effect;
    public float Range;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class XenoFuryComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<XenoFuryComponent, ComponentGetState>(new ComponentEventRefHandler<XenoFuryComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<XenoFuryComponent, ComponentHandleState>(new ComponentEventRefHandler<XenoFuryComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(EntityUid uid, XenoFuryComponent component, ref ComponentGetState args)
    {
      args.State = (IComponentState) new XenoFuryComponent.XenoFuryComponent_AutoState()
      {
        Heal = component.Heal,
        BoostedHeal = component.BoostedHeal,
        Effect = component.Effect,
        Range = component.Range
      };
    }

    private void OnHandleState(
      EntityUid uid,
      XenoFuryComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is XenoFuryComponent.XenoFuryComponent_AutoState current))
        return;
      component.Heal = current.Heal;
      component.BoostedHeal = current.BoostedHeal;
      component.Effect = current.Effect;
      component.Range = current.Range;
    }
  }
}
