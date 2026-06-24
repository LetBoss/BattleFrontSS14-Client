// Decompiled with JetBrains decompiler
// Type: Content.Shared.Nutrition.Components.ShakeableComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Localization;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Nutrition.Components;

[RegisterComponent]
[NetworkedComponent]
public sealed class ShakeableComponent : 
  Component,
  ISerializationGenerated<ShakeableComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public TimeSpan ShakeDuration = TimeSpan.FromSeconds(1.0);
  [DataField(null, false, 1, false, false, null)]
  public bool RequireInHand;
  [DataField(null, false, 1, false, false, null)]
  public LocId ShakeVerbText = (LocId) "shakeable-verb";
  [DataField(null, false, 1, false, false, null)]
  public LocId ShakePopupMessageSelf = (LocId) "shakeable-popup-message-self";
  [DataField(null, false, 1, false, false, null)]
  public LocId ShakePopupMessageOthers = (LocId) "shakeable-popup-message-others";
  [DataField(null, false, 1, false, false, null)]
  public SoundSpecifier ShakeSound = (SoundSpecifier) new SoundPathSpecifier("/Audio/Items/soda_shake.ogg");

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref ShakeableComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (ShakeableComponent) target1;
    if (serialization.TryCustomCopy<ShakeableComponent>(this, ref target, hookCtx, false, context))
      return;
    TimeSpan target2 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.ShakeDuration, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<TimeSpan>(this.ShakeDuration, hookCtx, context);
    target.ShakeDuration = target2;
    bool target3 = false;
    if (!serialization.TryCustomCopy<bool>(this.RequireInHand, ref target3, hookCtx, false, context))
      target3 = this.RequireInHand;
    target.RequireInHand = target3;
    LocId target4 = new LocId();
    if (!serialization.TryCustomCopy<LocId>(this.ShakeVerbText, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<LocId>(this.ShakeVerbText, hookCtx, context);
    target.ShakeVerbText = target4;
    LocId target5 = new LocId();
    if (!serialization.TryCustomCopy<LocId>(this.ShakePopupMessageSelf, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<LocId>(this.ShakePopupMessageSelf, hookCtx, context);
    target.ShakePopupMessageSelf = target5;
    LocId target6 = new LocId();
    if (!serialization.TryCustomCopy<LocId>(this.ShakePopupMessageOthers, ref target6, hookCtx, false, context))
      target6 = serialization.CreateCopy<LocId>(this.ShakePopupMessageOthers, hookCtx, context);
    target.ShakePopupMessageOthers = target6;
    SoundSpecifier target7 = (SoundSpecifier) null;
    if (this.ShakeSound == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.ShakeSound, ref target7, hookCtx, true, context))
      target7 = serialization.CreateCopy<SoundSpecifier>(this.ShakeSound, hookCtx, context);
    target.ShakeSound = target7;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref ShakeableComponent target,
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
    ShakeableComponent target1 = (ShakeableComponent) target;
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
    ShakeableComponent target1 = (ShakeableComponent) target;
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
    ShakeableComponent target1 = (ShakeableComponent) target;
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
  virtual ShakeableComponent Component.Instantiate() => new ShakeableComponent();
}
