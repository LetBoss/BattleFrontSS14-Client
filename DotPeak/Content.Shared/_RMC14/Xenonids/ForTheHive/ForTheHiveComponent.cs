// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.ForTheHive.ForTheHiveComponent
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
namespace Content.Shared._RMC14.Xenonids.ForTheHive;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
public sealed class ForTheHiveComponent : 
  Component,
  ISerializationGenerated<ForTheHiveComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public string? BaseSprite;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public string? ActiveSprite;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan AnimationTimeBase = TimeSpan.FromSeconds(1.6);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan Duration = TimeSpan.FromSeconds(20L);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int Minimum = 200;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public DamageSpecifier BaseDamage = new DamageSpecifier();
  [DataField(null, false, 1, false, false, null)]
  public ComponentRegistry? Acid;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref ForTheHiveComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (ForTheHiveComponent) target1;
    if (serialization.TryCustomCopy<ForTheHiveComponent>(this, ref target, hookCtx, false, context))
      return;
    string target2 = (string) null;
    if (!serialization.TryCustomCopy<string>(this.BaseSprite, ref target2, hookCtx, false, context))
      target2 = this.BaseSprite;
    target.BaseSprite = target2;
    string target3 = (string) null;
    if (!serialization.TryCustomCopy<string>(this.ActiveSprite, ref target3, hookCtx, false, context))
      target3 = this.ActiveSprite;
    target.ActiveSprite = target3;
    TimeSpan target4 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.AnimationTimeBase, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<TimeSpan>(this.AnimationTimeBase, hookCtx, context);
    target.AnimationTimeBase = target4;
    TimeSpan target5 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.Duration, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<TimeSpan>(this.Duration, hookCtx, context);
    target.Duration = target5;
    int target6 = 0;
    if (!serialization.TryCustomCopy<int>(this.Minimum, ref target6, hookCtx, false, context))
      target6 = this.Minimum;
    target.Minimum = target6;
    DamageSpecifier target7 = (DamageSpecifier) null;
    if (this.BaseDamage == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<DamageSpecifier>(this.BaseDamage, ref target7, hookCtx, false, context))
    {
      if (this.BaseDamage == null)
        target7 = (DamageSpecifier) null;
      else
        serialization.CopyTo<DamageSpecifier>(this.BaseDamage, ref target7, hookCtx, context, true);
    }
    target.BaseDamage = target7;
    ComponentRegistry target8 = (ComponentRegistry) null;
    if (!serialization.TryCustomCopy<ComponentRegistry>(this.Acid, ref target8, hookCtx, false, context))
      target8 = serialization.CreateCopy<ComponentRegistry>(this.Acid, hookCtx, context);
    target.Acid = target8;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref ForTheHiveComponent target,
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
    ForTheHiveComponent target1 = (ForTheHiveComponent) target;
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
    ForTheHiveComponent target1 = (ForTheHiveComponent) target;
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
    ForTheHiveComponent target1 = (ForTheHiveComponent) target;
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
  virtual ForTheHiveComponent Component.Instantiate() => new ForTheHiveComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class ForTheHiveComponent_AutoState : IComponentState
  {
    public string? BaseSprite;
    public string? ActiveSprite;
    public TimeSpan AnimationTimeBase;
    public TimeSpan Duration;
    public int Minimum;
    public DamageSpecifier BaseDamage;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class ForTheHiveComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<ForTheHiveComponent, ComponentGetState>(new ComponentEventRefHandler<ForTheHiveComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<ForTheHiveComponent, ComponentHandleState>(new ComponentEventRefHandler<ForTheHiveComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      ForTheHiveComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new ForTheHiveComponent.ForTheHiveComponent_AutoState()
      {
        BaseSprite = component.BaseSprite,
        ActiveSprite = component.ActiveSprite,
        AnimationTimeBase = component.AnimationTimeBase,
        Duration = component.Duration,
        Minimum = component.Minimum,
        BaseDamage = component.BaseDamage
      };
    }

    private void OnHandleState(
      EntityUid uid,
      ForTheHiveComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is ForTheHiveComponent.ForTheHiveComponent_AutoState current))
        return;
      component.BaseSprite = current.BaseSprite;
      component.ActiveSprite = current.ActiveSprite;
      component.AnimationTimeBase = current.AnimationTimeBase;
      component.Duration = current.Duration;
      component.Minimum = current.Minimum;
      component.BaseDamage = current.BaseDamage;
    }
  }
}
