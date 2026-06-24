// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Weapons.Ranged.Flamer.RMCSprayAmmoProviderComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.FixedPoint;
using Content.Shared.Weapons.Ranged;
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
namespace Content.Shared._RMC14.Weapons.Ranged.Flamer;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (SharedRMCFlamerSystem)})]
public sealed class RMCSprayAmmoProviderComponent : 
  Component,
  IShootable,
  ISerializationGenerated<RMCSprayAmmoProviderComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public string SolutionId = "spray";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public FixedPoint2 Cost = FixedPoint2.New(5);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntProtoId Spawn = (EntProtoId) "RMCExtinguisherSpray";

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref RMCSprayAmmoProviderComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (RMCSprayAmmoProviderComponent) target1;
    if (serialization.TryCustomCopy<RMCSprayAmmoProviderComponent>(this, ref target, hookCtx, false, context))
      return;
    string target2 = (string) null;
    if (this.SolutionId == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.SolutionId, ref target2, hookCtx, false, context))
      target2 = this.SolutionId;
    target.SolutionId = target2;
    FixedPoint2 target3 = new FixedPoint2();
    if (!serialization.TryCustomCopy<FixedPoint2>(this.Cost, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<FixedPoint2>(this.Cost, hookCtx, context);
    target.Cost = target3;
    EntProtoId target4 = new EntProtoId();
    if (!serialization.TryCustomCopy<EntProtoId>(this.Spawn, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<EntProtoId>(this.Spawn, hookCtx, context);
    target.Spawn = target4;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref RMCSprayAmmoProviderComponent target,
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
    RMCSprayAmmoProviderComponent target1 = (RMCSprayAmmoProviderComponent) target;
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
    RMCSprayAmmoProviderComponent target1 = (RMCSprayAmmoProviderComponent) target;
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
    RMCSprayAmmoProviderComponent target1 = (RMCSprayAmmoProviderComponent) target;
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
  virtual RMCSprayAmmoProviderComponent Component.Instantiate()
  {
    return new RMCSprayAmmoProviderComponent();
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class RMCSprayAmmoProviderComponent_AutoState : IComponentState
  {
    public string SolutionId;
    public FixedPoint2 Cost;
    public EntProtoId Spawn;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class RMCSprayAmmoProviderComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<RMCSprayAmmoProviderComponent, ComponentGetState>(new ComponentEventRefHandler<RMCSprayAmmoProviderComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<RMCSprayAmmoProviderComponent, ComponentHandleState>(new ComponentEventRefHandler<RMCSprayAmmoProviderComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      RMCSprayAmmoProviderComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new RMCSprayAmmoProviderComponent.RMCSprayAmmoProviderComponent_AutoState()
      {
        SolutionId = component.SolutionId,
        Cost = component.Cost,
        Spawn = component.Spawn
      };
    }

    private void OnHandleState(
      EntityUid uid,
      RMCSprayAmmoProviderComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is RMCSprayAmmoProviderComponent.RMCSprayAmmoProviderComponent_AutoState current))
        return;
      component.SolutionId = current.SolutionId;
      component.Cost = current.Cost;
      component.Spawn = current.Spawn;
    }
  }
}
