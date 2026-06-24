// Decompiled with JetBrains decompiler
// Type: Content.Shared.Weapons.Marker.DamageMarkerOnCollideComponent
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
using Robust.Shared.ViewVariables;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Weapons.Marker;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (SharedDamageMarkerSystem)})]
public sealed class DamageMarkerOnCollideComponent : 
  Component,
  ISerializationGenerated<DamageMarkerOnCollideComponent>,
  ISerializationGenerated
{
  [DataField("whitelist", false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntityWhitelist? Whitelist = new EntityWhitelist();
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [DataField("duration", false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan Duration = TimeSpan.FromSeconds(5L);
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [DataField("damage", false, 1, false, false, null)]
  public DamageSpecifier Damage = new DamageSpecifier();
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [DataField("amount", false, 1, false, false, null)]
  [AutoNetworkedField]
  public int Amount = 1;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref DamageMarkerOnCollideComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (DamageMarkerOnCollideComponent) target1;
    if (serialization.TryCustomCopy<DamageMarkerOnCollideComponent>(this, ref target, hookCtx, false, context))
      return;
    EntityWhitelist target2 = (EntityWhitelist) null;
    if (!serialization.TryCustomCopy<EntityWhitelist>(this.Whitelist, ref target2, hookCtx, false, context))
    {
      if (this.Whitelist == null)
        target2 = (EntityWhitelist) null;
      else
        serialization.CopyTo<EntityWhitelist>(this.Whitelist, ref target2, hookCtx, context);
    }
    target.Whitelist = target2;
    TimeSpan target3 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.Duration, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<TimeSpan>(this.Duration, hookCtx, context);
    target.Duration = target3;
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
    int target5 = 0;
    if (!serialization.TryCustomCopy<int>(this.Amount, ref target5, hookCtx, false, context))
      target5 = this.Amount;
    target.Amount = target5;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref DamageMarkerOnCollideComponent target,
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
    DamageMarkerOnCollideComponent target1 = (DamageMarkerOnCollideComponent) target;
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
    DamageMarkerOnCollideComponent target1 = (DamageMarkerOnCollideComponent) target;
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
    DamageMarkerOnCollideComponent target1 = (DamageMarkerOnCollideComponent) target;
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
  virtual DamageMarkerOnCollideComponent Component.Instantiate()
  {
    return new DamageMarkerOnCollideComponent();
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class DamageMarkerOnCollideComponent_AutoState : IComponentState
  {
    public EntityWhitelist? Whitelist;
    public TimeSpan Duration;
    public int Amount;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class DamageMarkerOnCollideComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<DamageMarkerOnCollideComponent, ComponentGetState>(new ComponentEventRefHandler<DamageMarkerOnCollideComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<DamageMarkerOnCollideComponent, ComponentHandleState>(new ComponentEventRefHandler<DamageMarkerOnCollideComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      DamageMarkerOnCollideComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new DamageMarkerOnCollideComponent.DamageMarkerOnCollideComponent_AutoState()
      {
        Whitelist = component.Whitelist,
        Duration = component.Duration,
        Amount = component.Amount
      };
    }

    private void OnHandleState(
      EntityUid uid,
      DamageMarkerOnCollideComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is DamageMarkerOnCollideComponent.DamageMarkerOnCollideComponent_AutoState current))
        return;
      component.Whitelist = current.Whitelist;
      component.Duration = current.Duration;
      component.Amount = current.Amount;
    }
  }
}
