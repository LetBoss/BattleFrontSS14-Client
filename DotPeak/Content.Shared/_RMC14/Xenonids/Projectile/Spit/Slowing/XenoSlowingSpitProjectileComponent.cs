// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Projectile.Spit.Slowing.XenoSlowingSpitProjectileComponent
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
namespace Content.Shared._RMC14.Xenonids.Projectile.Spit.Slowing;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (XenoSpitSystem), typeof (XenoProjectileSystem)})]
public sealed class XenoSlowingSpitProjectileComponent : 
  Component,
  ISerializationGenerated<XenoSlowingSpitProjectileComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool SuperSlow = true;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan Slow = TimeSpan.FromSeconds(8L);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan Paralyze = TimeSpan.FromSeconds(3.5);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool ArmorResistsKnockdown = true;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref XenoSlowingSpitProjectileComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (XenoSlowingSpitProjectileComponent) target1;
    if (serialization.TryCustomCopy<XenoSlowingSpitProjectileComponent>(this, ref target, hookCtx, false, context))
      return;
    bool target2 = false;
    if (!serialization.TryCustomCopy<bool>(this.SuperSlow, ref target2, hookCtx, false, context))
      target2 = this.SuperSlow;
    target.SuperSlow = target2;
    TimeSpan target3 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.Slow, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<TimeSpan>(this.Slow, hookCtx, context);
    target.Slow = target3;
    TimeSpan target4 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.Paralyze, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<TimeSpan>(this.Paralyze, hookCtx, context);
    target.Paralyze = target4;
    bool target5 = false;
    if (!serialization.TryCustomCopy<bool>(this.ArmorResistsKnockdown, ref target5, hookCtx, false, context))
      target5 = this.ArmorResistsKnockdown;
    target.ArmorResistsKnockdown = target5;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref XenoSlowingSpitProjectileComponent target,
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
    XenoSlowingSpitProjectileComponent target1 = (XenoSlowingSpitProjectileComponent) target;
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
    XenoSlowingSpitProjectileComponent target1 = (XenoSlowingSpitProjectileComponent) target;
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
    XenoSlowingSpitProjectileComponent target1 = (XenoSlowingSpitProjectileComponent) target;
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
  virtual XenoSlowingSpitProjectileComponent Component.Instantiate()
  {
    return new XenoSlowingSpitProjectileComponent();
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class XenoSlowingSpitProjectileComponent_AutoState : IComponentState
  {
    public bool SuperSlow;
    public TimeSpan Slow;
    public TimeSpan Paralyze;
    public bool ArmorResistsKnockdown;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class XenoSlowingSpitProjectileComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<XenoSlowingSpitProjectileComponent, ComponentGetState>(new ComponentEventRefHandler<XenoSlowingSpitProjectileComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<XenoSlowingSpitProjectileComponent, ComponentHandleState>(new ComponentEventRefHandler<XenoSlowingSpitProjectileComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      XenoSlowingSpitProjectileComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new XenoSlowingSpitProjectileComponent.XenoSlowingSpitProjectileComponent_AutoState()
      {
        SuperSlow = component.SuperSlow,
        Slow = component.Slow,
        Paralyze = component.Paralyze,
        ArmorResistsKnockdown = component.ArmorResistsKnockdown
      };
    }

    private void OnHandleState(
      EntityUid uid,
      XenoSlowingSpitProjectileComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is XenoSlowingSpitProjectileComponent.XenoSlowingSpitProjectileComponent_AutoState current))
        return;
      component.SuperSlow = current.SuperSlow;
      component.Slow = current.Slow;
      component.Paralyze = current.Paralyze;
      component.ArmorResistsKnockdown = current.ArmorResistsKnockdown;
    }
  }
}
