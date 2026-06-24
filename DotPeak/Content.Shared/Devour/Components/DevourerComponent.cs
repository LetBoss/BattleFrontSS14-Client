// Decompiled with JetBrains decompiler
// Type: Content.Shared.Devour.Components.DevourerComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Chemistry.Reagent;
using Content.Shared.Whitelist;
using Robust.Shared.Analyzers;
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
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Devour.Components;

[RegisterComponent]
[NetworkedComponent]
[Access(new Type[] {typeof (SharedDevourSystem)})]
public sealed class DevourerComponent : 
  Component,
  ISerializationGenerated<DevourerComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, typeof (PrototypeIdSerializer<EntityPrototype>))]
  public string? DevourAction;
  [DataField(null, false, 1, false, false, null)]
  public EntityUid? DevourActionEntity;
  [DataField(null, false, 1, false, false, null)]
  public SoundSpecifier? SoundDevour;
  [DataField(null, false, 1, false, false, null)]
  public float DevourTime;
  [DataField(null, false, 1, false, false, null)]
  public float StructureDevourTime;
  [DataField(null, false, 1, false, false, null)]
  public SoundSpecifier? SoundStructureDevour;
  public Container Stomach;
  [DataField(null, false, 1, false, false, null)]
  public EntityWhitelist? Whitelist;
  [DataField(null, false, 1, false, false, null)]
  public EntityWhitelist? StomachStorageWhitelist;
  [DataField(null, false, 1, false, false, null)]
  public EntityWhitelist? FoodPreferenceWhitelist;
  [DataField(null, false, 1, false, false, typeof (PrototypeIdSerializer<ReagentPrototype>))]
  public string Chemical;
  [DataField(null, false, 1, false, false, null)]
  public float HealRate;

  public DevourerComponent()
  {
    SoundPathSpecifier soundPathSpecifier1 = new SoundPathSpecifier("/Audio/Effects/demon_consume.ogg", new AudioParams?());
    ((SoundSpecifier) soundPathSpecifier1).Params = ((AudioParams) ref AudioParams.Default).WithVolume(-3f);
    this.SoundDevour = (SoundSpecifier) soundPathSpecifier1;
    this.DevourTime = 3f;
    this.StructureDevourTime = 10f;
    SoundPathSpecifier soundPathSpecifier2 = new SoundPathSpecifier("/Audio/Machines/airlock_creaking.ogg", new AudioParams?());
    ((SoundSpecifier) soundPathSpecifier2).Params = ((AudioParams) ref AudioParams.Default).WithVolume(-3f);
    this.SoundStructureDevour = (SoundSpecifier) soundPathSpecifier2;
    this.Whitelist = new EntityWhitelist()
    {
      Components = new string[1]{ "MobState" }
    };
    this.Chemical = "Ichor";
    this.HealRate = 15f;
    // ISSUE: explicit constructor call
    base.\u002Ector();
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref DevourerComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component component = (Component) target;
    this.InternalCopy(ref component, serialization, hookCtx, context);
    target = (DevourerComponent) component;
    if (serialization.TryCustomCopy<DevourerComponent>(this, ref target, hookCtx, false, context))
      return;
    string str1 = (string) null;
    if (!serialization.TryCustomCopy<string>(this.DevourAction, ref str1, hookCtx, false, context))
      str1 = this.DevourAction;
    target.DevourAction = str1;
    EntityUid? nullable = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.DevourActionEntity, ref nullable, hookCtx, false, context))
      nullable = serialization.CreateCopy<EntityUid?>(this.DevourActionEntity, hookCtx, context, false);
    target.DevourActionEntity = nullable;
    SoundSpecifier soundSpecifier1 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.SoundDevour, ref soundSpecifier1, hookCtx, true, context))
      soundSpecifier1 = serialization.CreateCopy<SoundSpecifier>(this.SoundDevour, hookCtx, context, false);
    target.SoundDevour = soundSpecifier1;
    float num1 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.DevourTime, ref num1, hookCtx, false, context))
      num1 = this.DevourTime;
    target.DevourTime = num1;
    float num2 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.StructureDevourTime, ref num2, hookCtx, false, context))
      num2 = this.StructureDevourTime;
    target.StructureDevourTime = num2;
    SoundSpecifier soundSpecifier2 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.SoundStructureDevour, ref soundSpecifier2, hookCtx, true, context))
      soundSpecifier2 = serialization.CreateCopy<SoundSpecifier>(this.SoundStructureDevour, hookCtx, context, false);
    target.SoundStructureDevour = soundSpecifier2;
    EntityWhitelist entityWhitelist1 = (EntityWhitelist) null;
    if (!serialization.TryCustomCopy<EntityWhitelist>(this.Whitelist, ref entityWhitelist1, hookCtx, false, context))
    {
      if (this.Whitelist == null)
        entityWhitelist1 = (EntityWhitelist) null;
      else
        serialization.CopyTo<EntityWhitelist>(this.Whitelist, ref entityWhitelist1, hookCtx, context, false);
    }
    target.Whitelist = entityWhitelist1;
    EntityWhitelist entityWhitelist2 = (EntityWhitelist) null;
    if (!serialization.TryCustomCopy<EntityWhitelist>(this.StomachStorageWhitelist, ref entityWhitelist2, hookCtx, false, context))
    {
      if (this.StomachStorageWhitelist == null)
        entityWhitelist2 = (EntityWhitelist) null;
      else
        serialization.CopyTo<EntityWhitelist>(this.StomachStorageWhitelist, ref entityWhitelist2, hookCtx, context, false);
    }
    target.StomachStorageWhitelist = entityWhitelist2;
    EntityWhitelist entityWhitelist3 = (EntityWhitelist) null;
    if (!serialization.TryCustomCopy<EntityWhitelist>(this.FoodPreferenceWhitelist, ref entityWhitelist3, hookCtx, false, context))
    {
      if (this.FoodPreferenceWhitelist == null)
        entityWhitelist3 = (EntityWhitelist) null;
      else
        serialization.CopyTo<EntityWhitelist>(this.FoodPreferenceWhitelist, ref entityWhitelist3, hookCtx, context, false);
    }
    target.FoodPreferenceWhitelist = entityWhitelist3;
    string str2 = (string) null;
    if (this.Chemical == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.Chemical, ref str2, hookCtx, false, context))
      str2 = this.Chemical;
    target.Chemical = str2;
    float num3 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.HealRate, ref num3, hookCtx, false, context))
      num3 = this.HealRate;
    target.HealRate = num3;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref DevourerComponent target,
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
    DevourerComponent target1 = (DevourerComponent) target;
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
    DevourerComponent target1 = (DevourerComponent) target;
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
    DevourerComponent target1 = (DevourerComponent) target;
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
  virtual DevourerComponent Component.Instantiate() => new DevourerComponent();
}
