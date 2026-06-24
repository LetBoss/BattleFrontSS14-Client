// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Brutalize.XenoBrutalizeComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Damage;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Xenonids.Brutalize;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (XenoBrutalizeSystem)})]
public sealed class XenoBrutalizeComponent : 
  Component,
  ISerializationGenerated<XenoBrutalizeComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int? MaxTargets;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public DamageSpecifier Damage = new DamageSpecifier();
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntProtoId Effect = (EntProtoId) "RMCEffectExtraSlash";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float Range = 1.5f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan BaseCooldownReduction = TimeSpan.FromSeconds(1.5);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan AddtionalCooldownReductions = TimeSpan.FromSeconds(0.5);

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref XenoBrutalizeComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (XenoBrutalizeComponent) target1;
    if (serialization.TryCustomCopy<XenoBrutalizeComponent>(this, ref target, hookCtx, false, context))
      return;
    int? target2 = new int?();
    if (!serialization.TryCustomCopy<int?>(this.MaxTargets, ref target2, hookCtx, false, context))
      target2 = this.MaxTargets;
    target.MaxTargets = target2;
    DamageSpecifier target3 = (DamageSpecifier) null;
    if (this.Damage == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<DamageSpecifier>(this.Damage, ref target3, hookCtx, false, context))
    {
      if (this.Damage == null)
        target3 = (DamageSpecifier) null;
      else
        serialization.CopyTo<DamageSpecifier>(this.Damage, ref target3, hookCtx, context, true);
    }
    target.Damage = target3;
    EntProtoId target4 = new EntProtoId();
    if (!serialization.TryCustomCopy<EntProtoId>(this.Effect, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<EntProtoId>(this.Effect, hookCtx, context);
    target.Effect = target4;
    float target5 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.Range, ref target5, hookCtx, false, context))
      target5 = this.Range;
    target.Range = target5;
    TimeSpan target6 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.BaseCooldownReduction, ref target6, hookCtx, false, context))
      target6 = serialization.CreateCopy<TimeSpan>(this.BaseCooldownReduction, hookCtx, context);
    target.BaseCooldownReduction = target6;
    TimeSpan target7 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.AddtionalCooldownReductions, ref target7, hookCtx, false, context))
      target7 = serialization.CreateCopy<TimeSpan>(this.AddtionalCooldownReductions, hookCtx, context);
    target.AddtionalCooldownReductions = target7;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref XenoBrutalizeComponent target,
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
    XenoBrutalizeComponent target1 = (XenoBrutalizeComponent) target;
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
    XenoBrutalizeComponent target1 = (XenoBrutalizeComponent) target;
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
    XenoBrutalizeComponent target1 = (XenoBrutalizeComponent) target;
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
  virtual XenoBrutalizeComponent Component.Instantiate() => new XenoBrutalizeComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class XenoBrutalizeComponent_AutoState : IComponentState
  {
    public int? MaxTargets;
    public DamageSpecifier Damage;
    public EntProtoId Effect;
    public float Range;
    public TimeSpan BaseCooldownReduction;
    public TimeSpan AddtionalCooldownReductions;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class XenoBrutalizeComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<XenoBrutalizeComponent, ComponentGetState>(new ComponentEventRefHandler<XenoBrutalizeComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<XenoBrutalizeComponent, ComponentHandleState>(new ComponentEventRefHandler<XenoBrutalizeComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      XenoBrutalizeComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new XenoBrutalizeComponent.XenoBrutalizeComponent_AutoState()
      {
        MaxTargets = component.MaxTargets,
        Damage = component.Damage,
        Effect = component.Effect,
        Range = component.Range,
        BaseCooldownReduction = component.BaseCooldownReduction,
        AddtionalCooldownReductions = component.AddtionalCooldownReductions
      };
    }

    private void OnHandleState(
      EntityUid uid,
      XenoBrutalizeComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is XenoBrutalizeComponent.XenoBrutalizeComponent_AutoState current))
        return;
      component.MaxTargets = current.MaxTargets;
      component.Damage = current.Damage;
      component.Effect = current.Effect;
      component.Range = current.Range;
      component.BaseCooldownReduction = current.BaseCooldownReduction;
      component.AddtionalCooldownReductions = current.AddtionalCooldownReductions;
    }
  }
}
