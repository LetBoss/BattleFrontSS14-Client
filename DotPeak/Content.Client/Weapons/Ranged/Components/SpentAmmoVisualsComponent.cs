// Decompiled with JetBrains decompiler
// Type: Content.Client.Weapons.Ranged.Components.SpentAmmoVisualsComponent
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.Weapons.Ranged.Systems;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Client.Weapons.Ranged.Components;

[RegisterComponent]
[Access(new Type[] {typeof (GunSystem)})]
public sealed class SpentAmmoVisualsComponent : 
  Component,
  ISerializationGenerated<SpentAmmoVisualsComponent>,
  ISerializationGenerated
{
  [DataField("suffix", false, 1, false, false, null)]
  public bool Suffix = true;
  [DataField("state", false, 1, false, false, null)]
  public string State = "base";

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref SpentAmmoVisualsComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component component = (Component) target;
    this.InternalCopy(ref component, serialization, hookCtx, context);
    target = (SpentAmmoVisualsComponent) component;
    if (serialization.TryCustomCopy<SpentAmmoVisualsComponent>(this, ref target, hookCtx, false, context))
      return;
    bool flag = false;
    if (!serialization.TryCustomCopy<bool>(this.Suffix, ref flag, hookCtx, false, context))
      flag = this.Suffix;
    target.Suffix = flag;
    string str = (string) null;
    if (this.State == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.State, ref str, hookCtx, false, context))
      str = this.State;
    target.State = str;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref SpentAmmoVisualsComponent target,
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
    SpentAmmoVisualsComponent target1 = (SpentAmmoVisualsComponent) target;
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
    SpentAmmoVisualsComponent target1 = (SpentAmmoVisualsComponent) target;
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
    SpentAmmoVisualsComponent target1 = (SpentAmmoVisualsComponent) target;
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
  virtual SpentAmmoVisualsComponent Component.Instantiate() => new SpentAmmoVisualsComponent();
}
