// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Stealth.EntityTurnInvisibleComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

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
namespace Content.Shared._RMC14.Stealth;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
public sealed class EntityTurnInvisibleComponent : 
  Component,
  ISerializationGenerated<EntityTurnInvisibleComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool Enabled;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float Opacity = 1f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool RestrictWeapons;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan UncloakTime;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan UncloakWeaponLock;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref EntityTurnInvisibleComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (EntityTurnInvisibleComponent) target1;
    if (serialization.TryCustomCopy<EntityTurnInvisibleComponent>(this, ref target, hookCtx, false, context))
      return;
    bool target2 = false;
    if (!serialization.TryCustomCopy<bool>(this.Enabled, ref target2, hookCtx, false, context))
      target2 = this.Enabled;
    target.Enabled = target2;
    float target3 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.Opacity, ref target3, hookCtx, false, context))
      target3 = this.Opacity;
    target.Opacity = target3;
    bool target4 = false;
    if (!serialization.TryCustomCopy<bool>(this.RestrictWeapons, ref target4, hookCtx, false, context))
      target4 = this.RestrictWeapons;
    target.RestrictWeapons = target4;
    TimeSpan target5 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.UncloakTime, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<TimeSpan>(this.UncloakTime, hookCtx, context);
    target.UncloakTime = target5;
    TimeSpan target6 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.UncloakWeaponLock, ref target6, hookCtx, false, context))
      target6 = serialization.CreateCopy<TimeSpan>(this.UncloakWeaponLock, hookCtx, context);
    target.UncloakWeaponLock = target6;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref EntityTurnInvisibleComponent target,
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
    EntityTurnInvisibleComponent target1 = (EntityTurnInvisibleComponent) target;
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
    EntityTurnInvisibleComponent target1 = (EntityTurnInvisibleComponent) target;
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
    EntityTurnInvisibleComponent target1 = (EntityTurnInvisibleComponent) target;
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
  virtual EntityTurnInvisibleComponent Component.Instantiate()
  {
    return new EntityTurnInvisibleComponent();
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class EntityTurnInvisibleComponent_AutoState : IComponentState
  {
    public bool Enabled;
    public float Opacity;
    public bool RestrictWeapons;
    public TimeSpan UncloakTime;
    public TimeSpan UncloakWeaponLock;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class EntityTurnInvisibleComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<EntityTurnInvisibleComponent, ComponentGetState>(new ComponentEventRefHandler<EntityTurnInvisibleComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<EntityTurnInvisibleComponent, ComponentHandleState>(new ComponentEventRefHandler<EntityTurnInvisibleComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      EntityTurnInvisibleComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new EntityTurnInvisibleComponent.EntityTurnInvisibleComponent_AutoState()
      {
        Enabled = component.Enabled,
        Opacity = component.Opacity,
        RestrictWeapons = component.RestrictWeapons,
        UncloakTime = component.UncloakTime,
        UncloakWeaponLock = component.UncloakWeaponLock
      };
    }

    private void OnHandleState(
      EntityUid uid,
      EntityTurnInvisibleComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is EntityTurnInvisibleComponent.EntityTurnInvisibleComponent_AutoState current))
        return;
      component.Enabled = current.Enabled;
      component.Opacity = current.Opacity;
      component.RestrictWeapons = current.RestrictWeapons;
      component.UncloakTime = current.UncloakTime;
      component.UncloakWeaponLock = current.UncloakWeaponLock;
    }
  }
}
