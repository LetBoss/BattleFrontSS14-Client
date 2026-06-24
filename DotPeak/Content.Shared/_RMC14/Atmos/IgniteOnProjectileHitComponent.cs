// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Atmos.IgniteOnProjectileHitComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Projectiles.Aimed;
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
[Access(new Type[] {typeof (SharedRMCFlammableSystem), typeof (AimedProjectileSystem)})]
public sealed class IgniteOnProjectileHitComponent : 
  Component,
  ISerializationGenerated<IgniteOnProjectileHitComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int Intensity = 30;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int Duration = 20;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public Color BurnColor = Color.FromHex((ReadOnlySpan<char>) "#EE6515", new Color?());

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref IgniteOnProjectileHitComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (IgniteOnProjectileHitComponent) target1;
    if (serialization.TryCustomCopy<IgniteOnProjectileHitComponent>(this, ref target, hookCtx, false, context))
      return;
    int target2 = 0;
    if (!serialization.TryCustomCopy<int>(this.Intensity, ref target2, hookCtx, false, context))
      target2 = this.Intensity;
    target.Intensity = target2;
    int target3 = 0;
    if (!serialization.TryCustomCopy<int>(this.Duration, ref target3, hookCtx, false, context))
      target3 = this.Duration;
    target.Duration = target3;
    Color target4 = new Color();
    if (!serialization.TryCustomCopy<Color>(this.BurnColor, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<Color>(this.BurnColor, hookCtx, context);
    target.BurnColor = target4;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref IgniteOnProjectileHitComponent target,
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
    IgniteOnProjectileHitComponent target1 = (IgniteOnProjectileHitComponent) target;
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
    IgniteOnProjectileHitComponent target1 = (IgniteOnProjectileHitComponent) target;
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
    IgniteOnProjectileHitComponent target1 = (IgniteOnProjectileHitComponent) target;
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
  virtual IgniteOnProjectileHitComponent Component.Instantiate()
  {
    return new IgniteOnProjectileHitComponent();
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class IgniteOnProjectileHitComponent_AutoState : IComponentState
  {
    public int Intensity;
    public int Duration;
    public Color BurnColor;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class IgniteOnProjectileHitComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<IgniteOnProjectileHitComponent, ComponentGetState>(new ComponentEventRefHandler<IgniteOnProjectileHitComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<IgniteOnProjectileHitComponent, ComponentHandleState>(new ComponentEventRefHandler<IgniteOnProjectileHitComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      IgniteOnProjectileHitComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new IgniteOnProjectileHitComponent.IgniteOnProjectileHitComponent_AutoState()
      {
        Intensity = component.Intensity,
        Duration = component.Duration,
        BurnColor = component.BurnColor
      };
    }

    private void OnHandleState(
      EntityUid uid,
      IgniteOnProjectileHitComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is IgniteOnProjectileHitComponent.IgniteOnProjectileHitComponent_AutoState current))
        return;
      component.Intensity = current.Intensity;
      component.Duration = current.Duration;
      component.BurnColor = current.BurnColor;
    }
  }
}
