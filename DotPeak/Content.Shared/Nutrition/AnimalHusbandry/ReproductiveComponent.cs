// Decompiled with JetBrains decompiler
// Type: Content.Shared.Nutrition.AnimalHusbandry.ReproductiveComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Storage;
using Content.Shared.Whitelist;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.Localization;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;
using Robust.Shared.ViewVariables;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Nutrition.AnimalHusbandry;

[RegisterComponent]
[AutoGenerateComponentPause]
public sealed class ReproductiveComponent : 
  Component,
  ISerializationGenerated<ReproductiveComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, typeof (TimeOffsetSerializer))]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [AutoPausedField]
  public TimeSpan NextBreedAttempt;
  [DataField(null, false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public TimeSpan MinBreedAttemptInterval = TimeSpan.FromSeconds(45L);
  [DataField(null, false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public TimeSpan MaxBreedAttemptInterval = TimeSpan.FromSeconds(60L);
  [DataField(null, false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public float BreedRange = 3f;
  [DataField(null, false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public int Capacity = 6;
  [DataField(null, false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public float BreedChance = 0.15f;
  [DataField(null, false, 1, true, false, null)]
  public List<EntitySpawnEntry> Offspring;
  [DataField(null, false, 1, false, false, null)]
  public bool Gestating;
  [DataField(null, false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public TimeSpan? GestationEndTime;
  [DataField(null, false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public TimeSpan GestationDuration = TimeSpan.FromMinutes(1.5);
  [DataField(null, false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public float HungerPerBirth = 75f;
  [DataField(null, false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public LocId BirthPopup = (LocId) "reproductive-birth-popup";
  [DataField(null, false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public bool MakeOffspringInfant = true;
  [DataField(null, false, 1, true, false, null)]
  public EntityWhitelist PartnerWhitelist;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref ReproductiveComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (ReproductiveComponent) target1;
    if (serialization.TryCustomCopy<ReproductiveComponent>(this, ref target, hookCtx, false, context))
      return;
    TimeSpan target2 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.NextBreedAttempt, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<TimeSpan>(this.NextBreedAttempt, hookCtx, context);
    target.NextBreedAttempt = target2;
    TimeSpan target3 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.MinBreedAttemptInterval, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<TimeSpan>(this.MinBreedAttemptInterval, hookCtx, context);
    target.MinBreedAttemptInterval = target3;
    TimeSpan target4 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.MaxBreedAttemptInterval, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<TimeSpan>(this.MaxBreedAttemptInterval, hookCtx, context);
    target.MaxBreedAttemptInterval = target4;
    float target5 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.BreedRange, ref target5, hookCtx, false, context))
      target5 = this.BreedRange;
    target.BreedRange = target5;
    int target6 = 0;
    if (!serialization.TryCustomCopy<int>(this.Capacity, ref target6, hookCtx, false, context))
      target6 = this.Capacity;
    target.Capacity = target6;
    float target7 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.BreedChance, ref target7, hookCtx, false, context))
      target7 = this.BreedChance;
    target.BreedChance = target7;
    List<EntitySpawnEntry> target8 = (List<EntitySpawnEntry>) null;
    if (this.Offspring == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<List<EntitySpawnEntry>>(this.Offspring, ref target8, hookCtx, true, context))
      target8 = serialization.CreateCopy<List<EntitySpawnEntry>>(this.Offspring, hookCtx, context);
    target.Offspring = target8;
    bool target9 = false;
    if (!serialization.TryCustomCopy<bool>(this.Gestating, ref target9, hookCtx, false, context))
      target9 = this.Gestating;
    target.Gestating = target9;
    TimeSpan? target10 = new TimeSpan?();
    if (!serialization.TryCustomCopy<TimeSpan?>(this.GestationEndTime, ref target10, hookCtx, false, context))
      target10 = serialization.CreateCopy<TimeSpan?>(this.GestationEndTime, hookCtx, context);
    target.GestationEndTime = target10;
    TimeSpan target11 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.GestationDuration, ref target11, hookCtx, false, context))
      target11 = serialization.CreateCopy<TimeSpan>(this.GestationDuration, hookCtx, context);
    target.GestationDuration = target11;
    float target12 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.HungerPerBirth, ref target12, hookCtx, false, context))
      target12 = this.HungerPerBirth;
    target.HungerPerBirth = target12;
    LocId target13 = new LocId();
    if (!serialization.TryCustomCopy<LocId>(this.BirthPopup, ref target13, hookCtx, false, context))
      target13 = serialization.CreateCopy<LocId>(this.BirthPopup, hookCtx, context);
    target.BirthPopup = target13;
    bool target14 = false;
    if (!serialization.TryCustomCopy<bool>(this.MakeOffspringInfant, ref target14, hookCtx, false, context))
      target14 = this.MakeOffspringInfant;
    target.MakeOffspringInfant = target14;
    EntityWhitelist target15 = (EntityWhitelist) null;
    if (this.PartnerWhitelist == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<EntityWhitelist>(this.PartnerWhitelist, ref target15, hookCtx, false, context))
    {
      if (this.PartnerWhitelist == null)
        target15 = (EntityWhitelist) null;
      else
        serialization.CopyTo<EntityWhitelist>(this.PartnerWhitelist, ref target15, hookCtx, context, true);
    }
    target.PartnerWhitelist = target15;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref ReproductiveComponent target,
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
    ReproductiveComponent target1 = (ReproductiveComponent) target;
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
    ReproductiveComponent target1 = (ReproductiveComponent) target;
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
    ReproductiveComponent target1 = (ReproductiveComponent) target;
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
  virtual ReproductiveComponent Component.Instantiate() => new ReproductiveComponent();

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class ReproductiveComponent_AutoPauseSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<ReproductiveComponent, EntityUnpausedEvent>(new ComponentEventRefHandler<ReproductiveComponent, EntityUnpausedEvent>(this.OnEntityUnpaused));
    }

    private void OnEntityUnpaused(
      EntityUid uid,
      #nullable disable
      ReproductiveComponent component,
      ref EntityUnpausedEvent args)
    {
      component.NextBreedAttempt += args.PausedTime;
    }
  }
}
