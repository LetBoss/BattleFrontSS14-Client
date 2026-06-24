// Decompiled with JetBrains decompiler
// Type: Content.Shared.Weapons.Ranged.MagazineAmmoProviderComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Weapons.Ranged.Components;
using Content.Shared.Weapons.Ranged.Systems;
using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Weapons.Ranged;

[RegisterComponent]
[Virtual]
[Access(new Type[] {typeof (SharedGunSystem)})]
public class MagazineAmmoProviderComponent : 
  AmmoProviderComponent,
  ISerializationGenerated<MagazineAmmoProviderComponent>,
  ISerializationGenerated
{
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [DataField("soundAutoEject", false, 1, false, false, null)]
  public SoundSpecifier? SoundAutoEject = (SoundSpecifier) new SoundPathSpecifier("/Audio/Weapons/Guns/EmptyAlarm/smg_empty_alarm.ogg");
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [DataField("autoEject", false, 1, false, false, null)]
  public bool AutoEject;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void InternalCopy(
    ref MagazineAmmoProviderComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    AmmoProviderComponent target1 = (AmmoProviderComponent) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (MagazineAmmoProviderComponent) target1;
    if (serialization.TryCustomCopy<MagazineAmmoProviderComponent>(this, ref target, hookCtx, false, context))
      return;
    SoundSpecifier target2 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.SoundAutoEject, ref target2, hookCtx, true, context))
      target2 = serialization.CreateCopy<SoundSpecifier>(this.SoundAutoEject, hookCtx, context);
    target.SoundAutoEject = target2;
    bool target3 = false;
    if (!serialization.TryCustomCopy<bool>(this.AutoEject, ref target3, hookCtx, false, context))
      target3 = this.AutoEject;
    target.AutoEject = target3;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void Copy(
    ref MagazineAmmoProviderComponent target,
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
    MagazineAmmoProviderComponent target1 = (MagazineAmmoProviderComponent) target;
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
    MagazineAmmoProviderComponent target1 = (MagazineAmmoProviderComponent) target;
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
    MagazineAmmoProviderComponent target1 = (MagazineAmmoProviderComponent) target;
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
  virtual MagazineAmmoProviderComponent AmmoProviderComponent.Instantiate()
  {
    return new MagazineAmmoProviderComponent();
  }
}
