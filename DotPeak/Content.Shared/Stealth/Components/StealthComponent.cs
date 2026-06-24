// Decompiled with JetBrains decompiler
// Type: Content.Shared.Stealth.Components.StealthComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Stealth.Components;

[RegisterComponent]
[NetworkedComponent]
[Access(new Type[] {typeof (SharedStealthSystem)})]
public sealed class StealthComponent : 
  Component,
  ISerializationGenerated<StealthComponent>,
  ISerializationGenerated
{
  [DataField("enabled", false, 1, false, false, null)]
  public bool Enabled = true;
  [DataField("enabledOnDeath", false, 1, false, false, null)]
  public bool EnabledOnDeath = true;
  [DataField("hadOutline", false, 1, false, false, null)]
  public bool HadOutline;
  [DataField("examineThreshold", false, 1, false, false, null)]
  public float ExamineThreshold = 0.5f;
  [DataField("lastVisibility", false, 1, false, false, null)]
  [Access(new Type[] {typeof (SharedStealthSystem)}, Other = AccessPermissions.None)]
  public float LastVisibility = 1f;
  [DataField("lastUpdate", false, 1, false, false, typeof (TimeOffsetSerializer))]
  public TimeSpan? LastUpdated;
  [DataField("minVisibility", false, 1, false, false, null)]
  public float MinVisibility = -1f;
  [DataField("maxVisibility", false, 1, false, false, null)]
  public float MaxVisibility = 1.5f;
  [DataField("examinedDesc", false, 1, false, false, null)]
  public string ExaminedDesc = "stealth-visual-effect";

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref StealthComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (StealthComponent) target1;
    if (serialization.TryCustomCopy<StealthComponent>(this, ref target, hookCtx, false, context))
      return;
    bool target2 = false;
    if (!serialization.TryCustomCopy<bool>(this.Enabled, ref target2, hookCtx, false, context))
      target2 = this.Enabled;
    target.Enabled = target2;
    bool target3 = false;
    if (!serialization.TryCustomCopy<bool>(this.EnabledOnDeath, ref target3, hookCtx, false, context))
      target3 = this.EnabledOnDeath;
    target.EnabledOnDeath = target3;
    bool target4 = false;
    if (!serialization.TryCustomCopy<bool>(this.HadOutline, ref target4, hookCtx, false, context))
      target4 = this.HadOutline;
    target.HadOutline = target4;
    float target5 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.ExamineThreshold, ref target5, hookCtx, false, context))
      target5 = this.ExamineThreshold;
    target.ExamineThreshold = target5;
    float target6 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.LastVisibility, ref target6, hookCtx, false, context))
      target6 = this.LastVisibility;
    target.LastVisibility = target6;
    TimeSpan? target7 = new TimeSpan?();
    if (!serialization.TryCustomCopy<TimeSpan?>(this.LastUpdated, ref target7, hookCtx, false, context))
      target7 = serialization.CreateCopy<TimeSpan?>(this.LastUpdated, hookCtx, context);
    target.LastUpdated = target7;
    float target8 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.MinVisibility, ref target8, hookCtx, false, context))
      target8 = this.MinVisibility;
    target.MinVisibility = target8;
    float target9 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.MaxVisibility, ref target9, hookCtx, false, context))
      target9 = this.MaxVisibility;
    target.MaxVisibility = target9;
    string target10 = (string) null;
    if (this.ExaminedDesc == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.ExaminedDesc, ref target10, hookCtx, false, context))
      target10 = this.ExaminedDesc;
    target.ExaminedDesc = target10;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref StealthComponent target,
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
    StealthComponent target1 = (StealthComponent) target;
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
    StealthComponent target1 = (StealthComponent) target;
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
    StealthComponent target1 = (StealthComponent) target;
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
  virtual StealthComponent Component.Instantiate() => new StealthComponent();
}
