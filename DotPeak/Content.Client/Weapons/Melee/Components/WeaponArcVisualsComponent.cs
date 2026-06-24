// Decompiled with JetBrains decompiler
// Type: Content.Client.Weapons.Melee.Components.WeaponArcVisualsComponent
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Client.Weapons.Melee.Components;

[RegisterComponent]
public sealed class WeaponArcVisualsComponent : 
  Component,
  ISerializationGenerated<WeaponArcVisualsComponent>,
  ISerializationGenerated
{
  public EntityUid? User;
  [DataField("animation", false, 1, false, false, null)]
  public WeaponArcAnimation Animation;
  [Robust.Shared.ViewVariables.ViewVariables]
  [DataField("fadeOut", false, 1, false, false, null)]
  public bool Fadeout = true;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref WeaponArcVisualsComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component component = (Component) target;
    this.InternalCopy(ref component, serialization, hookCtx, context);
    target = (WeaponArcVisualsComponent) component;
    if (serialization.TryCustomCopy<WeaponArcVisualsComponent>(this, ref target, hookCtx, false, context))
      return;
    WeaponArcAnimation weaponArcAnimation = WeaponArcAnimation.None;
    if (!serialization.TryCustomCopy<WeaponArcAnimation>(this.Animation, ref weaponArcAnimation, hookCtx, false, context))
      weaponArcAnimation = this.Animation;
    target.Animation = weaponArcAnimation;
    bool flag = false;
    if (!serialization.TryCustomCopy<bool>(this.Fadeout, ref flag, hookCtx, false, context))
      flag = this.Fadeout;
    target.Fadeout = flag;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref WeaponArcVisualsComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void Copy(
    ref Component target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    WeaponArcVisualsComponent target1 = (WeaponArcVisualsComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (Component) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    WeaponArcVisualsComponent target1 = (WeaponArcVisualsComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void InternalCopy(
    ref IComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    WeaponArcVisualsComponent target1 = (WeaponArcVisualsComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (IComponent) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void Copy(
    ref IComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    base.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual WeaponArcVisualsComponent Component.Instantiate() => new WeaponArcVisualsComponent();
}
