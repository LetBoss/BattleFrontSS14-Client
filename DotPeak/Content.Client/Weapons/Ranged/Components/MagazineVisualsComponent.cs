// Decompiled with JetBrains decompiler
// Type: Content.Client.Weapons.Ranged.Components.MagazineVisualsComponent
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.Weapons.Ranged.Systems;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Client.Weapons.Ranged.Components;

[RegisterComponent]
[Access(new Type[] {typeof (GunSystem)})]
public sealed class MagazineVisualsComponent : 
  Component,
  ISerializationGenerated<MagazineVisualsComponent>,
  ISerializationGenerated
{
  [DataField("magState", false, 1, false, false, null)]
  public string? MagState;
  [DataField("steps", false, 1, false, false, null)]
  public int MagSteps;
  [DataField("zeroVisible", false, 1, false, false, null)]
  public bool ZeroVisible;
  [DataField(null, false, 1, false, false, null)]
  public bool ZeroOnlyOnEmpty;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref MagazineVisualsComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component component = (Component) target;
    this.InternalCopy(ref component, serialization, hookCtx, context);
    target = (MagazineVisualsComponent) component;
    if (serialization.TryCustomCopy<MagazineVisualsComponent>(this, ref target, hookCtx, false, context))
      return;
    string str = (string) null;
    if (!serialization.TryCustomCopy<string>(this.MagState, ref str, hookCtx, false, context))
      str = this.MagState;
    target.MagState = str;
    int num = 0;
    if (!serialization.TryCustomCopy<int>(this.MagSteps, ref num, hookCtx, false, context))
      num = this.MagSteps;
    target.MagSteps = num;
    bool flag1 = false;
    if (!serialization.TryCustomCopy<bool>(this.ZeroVisible, ref flag1, hookCtx, false, context))
      flag1 = this.ZeroVisible;
    target.ZeroVisible = flag1;
    bool flag2 = false;
    if (!serialization.TryCustomCopy<bool>(this.ZeroOnlyOnEmpty, ref flag2, hookCtx, false, context))
      flag2 = this.ZeroOnlyOnEmpty;
    target.ZeroOnlyOnEmpty = flag2;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref MagazineVisualsComponent target,
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
    MagazineVisualsComponent target1 = (MagazineVisualsComponent) target;
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
    MagazineVisualsComponent target1 = (MagazineVisualsComponent) target;
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
    MagazineVisualsComponent target1 = (MagazineVisualsComponent) target;
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
  virtual MagazineVisualsComponent Component.Instantiate() => new MagazineVisualsComponent();
}
