// Decompiled with JetBrains decompiler
// Type: Content.Shared.Weapons.Ranged.Components.RevolverAmmoProviderComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Whitelist;
using Robust.Shared.Audio;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Weapons.Ranged.Components;

[RegisterComponent]
[NetworkedComponent]
public sealed class RevolverAmmoProviderComponent : 
  AmmoProviderComponent,
  ISerializationGenerated<RevolverAmmoProviderComponent>,
  ISerializationGenerated
{
  [DataField("whitelist", false, 1, false, false, null)]
  public EntityWhitelist? Whitelist;
  public Container AmmoContainer;
  [DataField("currentSlot", false, 1, false, false, null)]
  public int CurrentIndex;
  [DataField("capacity", false, 1, false, false, null)]
  public int Capacity = 6;
  [DataField("ammoSlots", false, 1, false, false, null)]
  public List<EntityUid?> AmmoSlots = new List<EntityUid?>();
  [DataField("chambers", false, 1, false, false, null)]
  public bool?[] Chambers = Array.Empty<bool?>();
  [DataField("proto", false, 1, false, false, typeof (PrototypeIdSerializer<EntityPrototype>))]
  public string? FillPrototype = "CartridgeMagnum";
  [DataField("soundEject", false, 1, false, false, null)]
  public SoundSpecifier? SoundEject = (SoundSpecifier) new SoundPathSpecifier("/Audio/Weapons/Guns/MagOut/revolver_magout.ogg");
  [DataField("soundInsert", false, 1, false, false, null)]
  public SoundSpecifier? SoundInsert = (SoundSpecifier) new SoundPathSpecifier("/Audio/Weapons/Guns/MagIn/revolver_magin.ogg");
  [DataField("soundSpin", false, 1, false, false, null)]
  public SoundSpecifier? SoundSpin = (SoundSpecifier) new SoundPathSpecifier("/Audio/Weapons/Guns/Misc/revolver_spin.ogg");

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref RevolverAmmoProviderComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    AmmoProviderComponent target1 = (AmmoProviderComponent) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (RevolverAmmoProviderComponent) target1;
    if (serialization.TryCustomCopy<RevolverAmmoProviderComponent>(this, ref target, hookCtx, false, context))
      return;
    EntityWhitelist target2 = (EntityWhitelist) null;
    if (!serialization.TryCustomCopy<EntityWhitelist>(this.Whitelist, ref target2, hookCtx, false, context))
    {
      if (this.Whitelist == null)
        target2 = (EntityWhitelist) null;
      else
        serialization.CopyTo<EntityWhitelist>(this.Whitelist, ref target2, hookCtx, context);
    }
    target.Whitelist = target2;
    int target3 = 0;
    if (!serialization.TryCustomCopy<int>(this.CurrentIndex, ref target3, hookCtx, false, context))
      target3 = this.CurrentIndex;
    target.CurrentIndex = target3;
    int target4 = 0;
    if (!serialization.TryCustomCopy<int>(this.Capacity, ref target4, hookCtx, false, context))
      target4 = this.Capacity;
    target.Capacity = target4;
    List<EntityUid?> target5 = (List<EntityUid?>) null;
    if (this.AmmoSlots == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<List<EntityUid?>>(this.AmmoSlots, ref target5, hookCtx, true, context))
      target5 = serialization.CreateCopy<List<EntityUid?>>(this.AmmoSlots, hookCtx, context);
    target.AmmoSlots = target5;
    bool?[] target6 = (bool?[]) null;
    if (this.Chambers == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<bool?[]>(this.Chambers, ref target6, hookCtx, true, context))
      target6 = serialization.CreateCopy<bool?[]>(this.Chambers, hookCtx, context);
    target.Chambers = target6;
    string target7 = (string) null;
    if (!serialization.TryCustomCopy<string>(this.FillPrototype, ref target7, hookCtx, false, context))
      target7 = this.FillPrototype;
    target.FillPrototype = target7;
    SoundSpecifier target8 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.SoundEject, ref target8, hookCtx, true, context))
      target8 = serialization.CreateCopy<SoundSpecifier>(this.SoundEject, hookCtx, context);
    target.SoundEject = target8;
    SoundSpecifier target9 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.SoundInsert, ref target9, hookCtx, true, context))
      target9 = serialization.CreateCopy<SoundSpecifier>(this.SoundInsert, hookCtx, context);
    target.SoundInsert = target9;
    SoundSpecifier target10 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.SoundSpin, ref target10, hookCtx, true, context))
      target10 = serialization.CreateCopy<SoundSpecifier>(this.SoundSpin, hookCtx, context);
    target.SoundSpin = target10;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref RevolverAmmoProviderComponent target,
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
    RevolverAmmoProviderComponent target1 = (RevolverAmmoProviderComponent) target;
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
    RevolverAmmoProviderComponent target1 = (RevolverAmmoProviderComponent) target;
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
    RevolverAmmoProviderComponent target1 = (RevolverAmmoProviderComponent) target;
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
  virtual RevolverAmmoProviderComponent AmmoProviderComponent.Instantiate()
  {
    return new RevolverAmmoProviderComponent();
  }
}
