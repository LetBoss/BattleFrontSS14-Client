// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Atmos.RMCIgniteOnCollideComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Damage;
using Content.Shared.Physics;
using Content.Shared.Whitelist;
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
namespace Content.Shared._RMC14.Atmos;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (SharedRMCFlammableSystem)})]
public sealed class RMCIgniteOnCollideComponent : 
  Component,
  ISerializationGenerated<RMCIgniteOnCollideComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int? MaxStacks;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int Intensity = 15;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int Duration = 55;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool InitDamaged;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public DamageSpecifier? TileDamage;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public double ArmorMultiplier = 1.0;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntityWhitelist? ArmorWhitelist;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public CollisionGroup Collision = CollisionGroup.FullTileLayer;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public Color BurnColor = Color.Orange;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref RMCIgniteOnCollideComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (RMCIgniteOnCollideComponent) target1;
    if (serialization.TryCustomCopy<RMCIgniteOnCollideComponent>(this, ref target, hookCtx, false, context))
      return;
    int? target2 = new int?();
    if (!serialization.TryCustomCopy<int?>(this.MaxStacks, ref target2, hookCtx, false, context))
      target2 = this.MaxStacks;
    target.MaxStacks = target2;
    int target3 = 0;
    if (!serialization.TryCustomCopy<int>(this.Intensity, ref target3, hookCtx, false, context))
      target3 = this.Intensity;
    target.Intensity = target3;
    int target4 = 0;
    if (!serialization.TryCustomCopy<int>(this.Duration, ref target4, hookCtx, false, context))
      target4 = this.Duration;
    target.Duration = target4;
    bool target5 = false;
    if (!serialization.TryCustomCopy<bool>(this.InitDamaged, ref target5, hookCtx, false, context))
      target5 = this.InitDamaged;
    target.InitDamaged = target5;
    DamageSpecifier target6 = (DamageSpecifier) null;
    if (!serialization.TryCustomCopy<DamageSpecifier>(this.TileDamage, ref target6, hookCtx, false, context))
    {
      if (this.TileDamage == null)
        target6 = (DamageSpecifier) null;
      else
        serialization.CopyTo<DamageSpecifier>(this.TileDamage, ref target6, hookCtx, context);
    }
    target.TileDamage = target6;
    double target7 = 0.0;
    if (!serialization.TryCustomCopy<double>(this.ArmorMultiplier, ref target7, hookCtx, false, context))
      target7 = this.ArmorMultiplier;
    target.ArmorMultiplier = target7;
    EntityWhitelist target8 = (EntityWhitelist) null;
    if (!serialization.TryCustomCopy<EntityWhitelist>(this.ArmorWhitelist, ref target8, hookCtx, false, context))
    {
      if (this.ArmorWhitelist == null)
        target8 = (EntityWhitelist) null;
      else
        serialization.CopyTo<EntityWhitelist>(this.ArmorWhitelist, ref target8, hookCtx, context);
    }
    target.ArmorWhitelist = target8;
    CollisionGroup target9 = CollisionGroup.None;
    if (!serialization.TryCustomCopy<CollisionGroup>(this.Collision, ref target9, hookCtx, false, context))
      target9 = this.Collision;
    target.Collision = target9;
    Color target10 = new Color();
    if (!serialization.TryCustomCopy<Color>(this.BurnColor, ref target10, hookCtx, false, context))
      target10 = serialization.CreateCopy<Color>(this.BurnColor, hookCtx, context);
    target.BurnColor = target10;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref RMCIgniteOnCollideComponent target,
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
    RMCIgniteOnCollideComponent target1 = (RMCIgniteOnCollideComponent) target;
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
    RMCIgniteOnCollideComponent target1 = (RMCIgniteOnCollideComponent) target;
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
    RMCIgniteOnCollideComponent target1 = (RMCIgniteOnCollideComponent) target;
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
  virtual RMCIgniteOnCollideComponent Component.Instantiate() => new RMCIgniteOnCollideComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class RMCIgniteOnCollideComponent_AutoState : IComponentState
  {
    public int? MaxStacks;
    public int Intensity;
    public int Duration;
    public bool InitDamaged;
    public DamageSpecifier? TileDamage;
    public double ArmorMultiplier;
    public EntityWhitelist? ArmorWhitelist;
    public CollisionGroup Collision;
    public Color BurnColor;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class RMCIgniteOnCollideComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<RMCIgniteOnCollideComponent, ComponentGetState>(new ComponentEventRefHandler<RMCIgniteOnCollideComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<RMCIgniteOnCollideComponent, ComponentHandleState>(new ComponentEventRefHandler<RMCIgniteOnCollideComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      RMCIgniteOnCollideComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new RMCIgniteOnCollideComponent.RMCIgniteOnCollideComponent_AutoState()
      {
        MaxStacks = component.MaxStacks,
        Intensity = component.Intensity,
        Duration = component.Duration,
        InitDamaged = component.InitDamaged,
        TileDamage = component.TileDamage,
        ArmorMultiplier = component.ArmorMultiplier,
        ArmorWhitelist = component.ArmorWhitelist,
        Collision = component.Collision,
        BurnColor = component.BurnColor
      };
    }

    private void OnHandleState(
      EntityUid uid,
      RMCIgniteOnCollideComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is RMCIgniteOnCollideComponent.RMCIgniteOnCollideComponent_AutoState current))
        return;
      component.MaxStacks = current.MaxStacks;
      component.Intensity = current.Intensity;
      component.Duration = current.Duration;
      component.InitDamaged = current.InitDamaged;
      component.TileDamage = current.TileDamage;
      component.ArmorMultiplier = current.ArmorMultiplier;
      component.ArmorWhitelist = current.ArmorWhitelist;
      component.Collision = current.Collision;
      component.BurnColor = current.BurnColor;
    }
  }
}
