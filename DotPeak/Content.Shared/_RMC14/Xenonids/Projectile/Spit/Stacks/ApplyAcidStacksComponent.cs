// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Projectile.Spit.Stacks.ApplyAcidStacksComponent
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
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Xenonids.Projectile.Spit.Stacks;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (XenoSpitSystem)})]
public sealed class ApplyAcidStacksComponent : 
  Component,
  ISerializationGenerated<ApplyAcidStacksComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int Amount = 1;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int Max = 5;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public DamageSpecifier? Damage;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntityWhitelist? Whitelist = new EntityWhitelist()
  {
    Components = new string[1]{ "Marine" }
  };

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref ApplyAcidStacksComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (ApplyAcidStacksComponent) target1;
    if (serialization.TryCustomCopy<ApplyAcidStacksComponent>(this, ref target, hookCtx, false, context))
      return;
    int target2 = 0;
    if (!serialization.TryCustomCopy<int>(this.Amount, ref target2, hookCtx, false, context))
      target2 = this.Amount;
    target.Amount = target2;
    int target3 = 0;
    if (!serialization.TryCustomCopy<int>(this.Max, ref target3, hookCtx, false, context))
      target3 = this.Max;
    target.Max = target3;
    DamageSpecifier target4 = (DamageSpecifier) null;
    if (!serialization.TryCustomCopy<DamageSpecifier>(this.Damage, ref target4, hookCtx, false, context))
    {
      if (this.Damage == null)
        target4 = (DamageSpecifier) null;
      else
        serialization.CopyTo<DamageSpecifier>(this.Damage, ref target4, hookCtx, context);
    }
    target.Damage = target4;
    EntityWhitelist target5 = (EntityWhitelist) null;
    if (!serialization.TryCustomCopy<EntityWhitelist>(this.Whitelist, ref target5, hookCtx, false, context))
    {
      if (this.Whitelist == null)
        target5 = (EntityWhitelist) null;
      else
        serialization.CopyTo<EntityWhitelist>(this.Whitelist, ref target5, hookCtx, context);
    }
    target.Whitelist = target5;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref ApplyAcidStacksComponent target,
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
    ApplyAcidStacksComponent target1 = (ApplyAcidStacksComponent) target;
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
    ApplyAcidStacksComponent target1 = (ApplyAcidStacksComponent) target;
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
    ApplyAcidStacksComponent target1 = (ApplyAcidStacksComponent) target;
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
  virtual ApplyAcidStacksComponent Component.Instantiate() => new ApplyAcidStacksComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class ApplyAcidStacksComponent_AutoState : IComponentState
  {
    public int Amount;
    public int Max;
    public DamageSpecifier? Damage;
    public EntityWhitelist? Whitelist;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class ApplyAcidStacksComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<ApplyAcidStacksComponent, ComponentGetState>(new ComponentEventRefHandler<ApplyAcidStacksComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<ApplyAcidStacksComponent, ComponentHandleState>(new ComponentEventRefHandler<ApplyAcidStacksComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      ApplyAcidStacksComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new ApplyAcidStacksComponent.ApplyAcidStacksComponent_AutoState()
      {
        Amount = component.Amount,
        Max = component.Max,
        Damage = component.Damage,
        Whitelist = component.Whitelist
      };
    }

    private void OnHandleState(
      EntityUid uid,
      ApplyAcidStacksComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is ApplyAcidStacksComponent.ApplyAcidStacksComponent_AutoState current))
        return;
      component.Amount = current.Amount;
      component.Max = current.Max;
      component.Damage = current.Damage;
      component.Whitelist = current.Whitelist;
    }
  }
}
