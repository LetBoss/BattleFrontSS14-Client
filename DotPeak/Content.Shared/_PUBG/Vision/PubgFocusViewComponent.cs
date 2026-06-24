// Decompiled with JetBrains decompiler
// Type: Content.Shared._PUBG.Vision.PubgFocusViewComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._PUBG.Vision;

[RegisterComponent]
public sealed class PubgFocusViewComponent : 
  Component,
  ISerializationGenerated<PubgFocusViewComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public float OffsetTiles = 4f;
  [DataField(null, false, 1, false, false, null)]
  public float PvsIncrease = 0.4f;
  [DataField(null, false, 1, false, false, null)]
  public bool FixedOffset;
  [DataField(null, false, 1, false, false, null)]
  public bool AdjustShotCoordinates = true;
  [DataField(null, false, 1, false, false, null)]
  public bool Active;
  [DataField(null, false, 1, false, false, null)]
  public EntityUid? ActiveUser;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref PubgFocusViewComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (PubgFocusViewComponent) target1;
    if (serialization.TryCustomCopy<PubgFocusViewComponent>(this, ref target, hookCtx, false, context))
      return;
    float target2 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.OffsetTiles, ref target2, hookCtx, false, context))
      target2 = this.OffsetTiles;
    target.OffsetTiles = target2;
    float target3 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.PvsIncrease, ref target3, hookCtx, false, context))
      target3 = this.PvsIncrease;
    target.PvsIncrease = target3;
    bool target4 = false;
    if (!serialization.TryCustomCopy<bool>(this.FixedOffset, ref target4, hookCtx, false, context))
      target4 = this.FixedOffset;
    target.FixedOffset = target4;
    bool target5 = false;
    if (!serialization.TryCustomCopy<bool>(this.AdjustShotCoordinates, ref target5, hookCtx, false, context))
      target5 = this.AdjustShotCoordinates;
    target.AdjustShotCoordinates = target5;
    bool target6 = false;
    if (!serialization.TryCustomCopy<bool>(this.Active, ref target6, hookCtx, false, context))
      target6 = this.Active;
    target.Active = target6;
    EntityUid? target7 = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.ActiveUser, ref target7, hookCtx, false, context))
      target7 = serialization.CreateCopy<EntityUid?>(this.ActiveUser, hookCtx, context);
    target.ActiveUser = target7;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref PubgFocusViewComponent target,
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
    PubgFocusViewComponent target1 = (PubgFocusViewComponent) target;
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
    PubgFocusViewComponent target1 = (PubgFocusViewComponent) target;
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
    PubgFocusViewComponent target1 = (PubgFocusViewComponent) target;
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
  virtual PubgFocusViewComponent Component.Instantiate() => new PubgFocusViewComponent();
}
