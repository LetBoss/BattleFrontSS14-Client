// Decompiled with JetBrains decompiler
// Type: Content.Shared.Xenoarchaeology.Artifact.XAE.Components.XAEDamageInAreaComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Damage;
using Content.Shared.Whitelist;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Xenoarchaeology.Artifact.XAE.Components;

[RegisterComponent]
[Access(new Type[] {typeof (XAEDamageInAreaSystem)})]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
public sealed class XAEDamageInAreaComponent : 
  Component,
  ISerializationGenerated<XAEDamageInAreaComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float Radius = 3f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntityWhitelist? Whitelist;
  [DataField(null, false, 1, true, false, null)]
  [AutoNetworkedField]
  public DamageSpecifier Damage;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float DamageChance = 1f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool IgnoreResistances;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref XAEDamageInAreaComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (XAEDamageInAreaComponent) target1;
    if (serialization.TryCustomCopy<XAEDamageInAreaComponent>(this, ref target, hookCtx, false, context))
      return;
    float target2 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.Radius, ref target2, hookCtx, false, context))
      target2 = this.Radius;
    target.Radius = target2;
    EntityWhitelist target3 = (EntityWhitelist) null;
    if (!serialization.TryCustomCopy<EntityWhitelist>(this.Whitelist, ref target3, hookCtx, false, context))
    {
      if (this.Whitelist == null)
        target3 = (EntityWhitelist) null;
      else
        serialization.CopyTo<EntityWhitelist>(this.Whitelist, ref target3, hookCtx, context);
    }
    target.Whitelist = target3;
    DamageSpecifier target4 = (DamageSpecifier) null;
    if (this.Damage == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<DamageSpecifier>(this.Damage, ref target4, hookCtx, false, context))
    {
      if (this.Damage == null)
        target4 = (DamageSpecifier) null;
      else
        serialization.CopyTo<DamageSpecifier>(this.Damage, ref target4, hookCtx, context, true);
    }
    target.Damage = target4;
    float target5 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.DamageChance, ref target5, hookCtx, false, context))
      target5 = this.DamageChance;
    target.DamageChance = target5;
    bool target6 = false;
    if (!serialization.TryCustomCopy<bool>(this.IgnoreResistances, ref target6, hookCtx, false, context))
      target6 = this.IgnoreResistances;
    target.IgnoreResistances = target6;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref XAEDamageInAreaComponent target,
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
    XAEDamageInAreaComponent target1 = (XAEDamageInAreaComponent) target;
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
    XAEDamageInAreaComponent target1 = (XAEDamageInAreaComponent) target;
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
    XAEDamageInAreaComponent target1 = (XAEDamageInAreaComponent) target;
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
  virtual XAEDamageInAreaComponent Component.Instantiate() => new XAEDamageInAreaComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class XAEDamageInAreaComponent_AutoState : IComponentState
  {
    public float Radius;
    public EntityWhitelist? Whitelist;
    public DamageSpecifier Damage;
    public float DamageChance;
    public bool IgnoreResistances;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class XAEDamageInAreaComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<XAEDamageInAreaComponent, ComponentGetState>(new ComponentEventRefHandler<XAEDamageInAreaComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<XAEDamageInAreaComponent, ComponentHandleState>(new ComponentEventRefHandler<XAEDamageInAreaComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      XAEDamageInAreaComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new XAEDamageInAreaComponent.XAEDamageInAreaComponent_AutoState()
      {
        Radius = component.Radius,
        Whitelist = component.Whitelist,
        Damage = component.Damage,
        DamageChance = component.DamageChance,
        IgnoreResistances = component.IgnoreResistances
      };
    }

    private void OnHandleState(
      EntityUid uid,
      XAEDamageInAreaComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is XAEDamageInAreaComponent.XAEDamageInAreaComponent_AutoState current))
        return;
      component.Radius = current.Radius;
      component.Whitelist = current.Whitelist;
      component.Damage = current.Damage;
      component.DamageChance = current.DamageChance;
      component.IgnoreResistances = current.IgnoreResistances;
    }
  }
}
