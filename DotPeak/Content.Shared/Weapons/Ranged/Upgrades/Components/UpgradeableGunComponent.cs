// Decompiled with JetBrains decompiler
// Type: Content.Shared.Weapons.Ranged.Upgrades.Components.UpgradeableGunComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Whitelist;
using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Weapons.Ranged.Upgrades.Components;

[RegisterComponent]
[NetworkedComponent]
[Access(new Type[] {typeof (GunUpgradeSystem)})]
public sealed class UpgradeableGunComponent : 
  Component,
  ISerializationGenerated<UpgradeableGunComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public string UpgradesContainerId = "upgrades";
  [DataField(null, false, 1, false, false, null)]
  public EntityWhitelist Whitelist = new EntityWhitelist();
  [DataField(null, false, 1, false, false, null)]
  public SoundSpecifier? InsertSound = (SoundSpecifier) new SoundPathSpecifier("/Audio/Effects/thunk.ogg");
  [DataField(null, false, 1, false, false, null)]
  public int MaxUpgradeCount = 2;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref UpgradeableGunComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (UpgradeableGunComponent) target1;
    if (serialization.TryCustomCopy<UpgradeableGunComponent>(this, ref target, hookCtx, false, context))
      return;
    string target2 = (string) null;
    if (this.UpgradesContainerId == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.UpgradesContainerId, ref target2, hookCtx, false, context))
      target2 = this.UpgradesContainerId;
    target.UpgradesContainerId = target2;
    EntityWhitelist target3 = (EntityWhitelist) null;
    if (this.Whitelist == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<EntityWhitelist>(this.Whitelist, ref target3, hookCtx, false, context))
    {
      if (this.Whitelist == null)
        target3 = (EntityWhitelist) null;
      else
        serialization.CopyTo<EntityWhitelist>(this.Whitelist, ref target3, hookCtx, context, true);
    }
    target.Whitelist = target3;
    SoundSpecifier target4 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.InsertSound, ref target4, hookCtx, true, context))
      target4 = serialization.CreateCopy<SoundSpecifier>(this.InsertSound, hookCtx, context);
    target.InsertSound = target4;
    int target5 = 0;
    if (!serialization.TryCustomCopy<int>(this.MaxUpgradeCount, ref target5, hookCtx, false, context))
      target5 = this.MaxUpgradeCount;
    target.MaxUpgradeCount = target5;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref UpgradeableGunComponent target,
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
    UpgradeableGunComponent target1 = (UpgradeableGunComponent) target;
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
    UpgradeableGunComponent target1 = (UpgradeableGunComponent) target;
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
    UpgradeableGunComponent target1 = (UpgradeableGunComponent) target;
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
  virtual UpgradeableGunComponent Component.Instantiate() => new UpgradeableGunComponent();
}
