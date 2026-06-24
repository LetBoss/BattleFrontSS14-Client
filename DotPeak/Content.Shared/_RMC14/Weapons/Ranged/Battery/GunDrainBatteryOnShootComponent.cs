// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Weapons.Ranged.Battery.GunDrainBatteryOnShootComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

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
namespace Content.Shared._RMC14.Weapons.Ranged.Battery;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (RMCGunBatterySystem)})]
public sealed class GunDrainBatteryOnShootComponent : 
  Component,
  ISerializationGenerated<GunDrainBatteryOnShootComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float BaseDrain = 0.25f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float Drain = 0.25f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public string BatteryContainer = "cell_slot";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool Powered;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref GunDrainBatteryOnShootComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (GunDrainBatteryOnShootComponent) target1;
    if (serialization.TryCustomCopy<GunDrainBatteryOnShootComponent>(this, ref target, hookCtx, false, context))
      return;
    float target2 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.BaseDrain, ref target2, hookCtx, false, context))
      target2 = this.BaseDrain;
    target.BaseDrain = target2;
    float target3 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.Drain, ref target3, hookCtx, false, context))
      target3 = this.Drain;
    target.Drain = target3;
    string target4 = (string) null;
    if (this.BatteryContainer == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.BatteryContainer, ref target4, hookCtx, false, context))
      target4 = this.BatteryContainer;
    target.BatteryContainer = target4;
    bool target5 = false;
    if (!serialization.TryCustomCopy<bool>(this.Powered, ref target5, hookCtx, false, context))
      target5 = this.Powered;
    target.Powered = target5;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref GunDrainBatteryOnShootComponent target,
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
    GunDrainBatteryOnShootComponent target1 = (GunDrainBatteryOnShootComponent) target;
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
    GunDrainBatteryOnShootComponent target1 = (GunDrainBatteryOnShootComponent) target;
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
    GunDrainBatteryOnShootComponent target1 = (GunDrainBatteryOnShootComponent) target;
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
  virtual GunDrainBatteryOnShootComponent Component.Instantiate()
  {
    return new GunDrainBatteryOnShootComponent();
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class GunDrainBatteryOnShootComponent_AutoState : IComponentState
  {
    public float BaseDrain;
    public float Drain;
    public string BatteryContainer;
    public bool Powered;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class GunDrainBatteryOnShootComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<GunDrainBatteryOnShootComponent, ComponentGetState>(new ComponentEventRefHandler<GunDrainBatteryOnShootComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<GunDrainBatteryOnShootComponent, ComponentHandleState>(new ComponentEventRefHandler<GunDrainBatteryOnShootComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      GunDrainBatteryOnShootComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new GunDrainBatteryOnShootComponent.GunDrainBatteryOnShootComponent_AutoState()
      {
        BaseDrain = component.BaseDrain,
        Drain = component.Drain,
        BatteryContainer = component.BatteryContainer,
        Powered = component.Powered
      };
    }

    private void OnHandleState(
      EntityUid uid,
      GunDrainBatteryOnShootComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is GunDrainBatteryOnShootComponent.GunDrainBatteryOnShootComponent_AutoState current))
        return;
      component.BaseDrain = current.BaseDrain;
      component.Drain = current.Drain;
      component.BatteryContainer = current.BatteryContainer;
      component.Powered = current.Powered;
    }
  }
}
