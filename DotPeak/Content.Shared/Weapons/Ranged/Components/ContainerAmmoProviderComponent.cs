// Decompiled with JetBrains decompiler
// Type: Content.Shared.Weapons.Ranged.Components.ContainerAmmoProviderComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Weapons.Ranged.Systems;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Weapons.Ranged.Components;

[RegisterComponent]
[Access(new Type[] {typeof (SharedGunSystem)})]
public sealed class ContainerAmmoProviderComponent : 
  AmmoProviderComponent,
  ISerializationGenerated<ContainerAmmoProviderComponent>,
  ISerializationGenerated
{
  [DataField("container", false, 1, true, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables]
  public string Container;
  [DataField("provider", false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables]
  public EntityUid? ProviderUid;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref ContainerAmmoProviderComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    AmmoProviderComponent target1 = (AmmoProviderComponent) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (ContainerAmmoProviderComponent) target1;
    if (serialization.TryCustomCopy<ContainerAmmoProviderComponent>(this, ref target, hookCtx, false, context))
      return;
    string target2 = (string) null;
    if (this.Container == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.Container, ref target2, hookCtx, false, context))
      target2 = this.Container;
    target.Container = target2;
    EntityUid? target3 = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.ProviderUid, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<EntityUid?>(this.ProviderUid, hookCtx, context);
    target.ProviderUid = target3;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref ContainerAmmoProviderComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref AmmoProviderComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    ContainerAmmoProviderComponent target1 = (ContainerAmmoProviderComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (AmmoProviderComponent) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    ContainerAmmoProviderComponent target1 = (ContainerAmmoProviderComponent) target;
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
    ContainerAmmoProviderComponent target1 = (ContainerAmmoProviderComponent) target;
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
  virtual ContainerAmmoProviderComponent AmmoProviderComponent.Instantiate()
  {
    return new ContainerAmmoProviderComponent();
  }
}
