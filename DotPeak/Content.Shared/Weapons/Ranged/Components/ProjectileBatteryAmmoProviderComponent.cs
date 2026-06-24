// Decompiled with JetBrains decompiler
// Type: Content.Shared.Weapons.Ranged.Components.ProjectileBatteryAmmoProviderComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using Robust.Shared.ViewVariables;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Weapons.Ranged.Components;

[RegisterComponent]
[NetworkedComponent]
public sealed class ProjectileBatteryAmmoProviderComponent : 
  BatteryAmmoProviderComponent,
  ISerializationGenerated<ProjectileBatteryAmmoProviderComponent>,
  ISerializationGenerated
{
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [DataField("proto", false, 1, true, false, typeof (PrototypeIdSerializer<EntityPrototype>))]
  public string Prototype;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref ProjectileBatteryAmmoProviderComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    BatteryAmmoProviderComponent target1 = (BatteryAmmoProviderComponent) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (ProjectileBatteryAmmoProviderComponent) target1;
    if (serialization.TryCustomCopy<ProjectileBatteryAmmoProviderComponent>(this, ref target, hookCtx, false, context))
      return;
    string target2 = (string) null;
    if (this.Prototype == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.Prototype, ref target2, hookCtx, false, context))
      target2 = this.Prototype;
    target.Prototype = target2;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref ProjectileBatteryAmmoProviderComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref BatteryAmmoProviderComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    ProjectileBatteryAmmoProviderComponent target1 = (ProjectileBatteryAmmoProviderComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (BatteryAmmoProviderComponent) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    ProjectileBatteryAmmoProviderComponent target1 = (ProjectileBatteryAmmoProviderComponent) target;
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
    ProjectileBatteryAmmoProviderComponent target1 = (ProjectileBatteryAmmoProviderComponent) target;
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
  virtual ProjectileBatteryAmmoProviderComponent BatteryAmmoProviderComponent.Instantiate()
  {
    return new ProjectileBatteryAmmoProviderComponent();
  }
}
