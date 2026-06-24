// Decompiled with JetBrains decompiler
// Type: Content.Shared.Teleportation.Components.HandTeleporterComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Audio;
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
namespace Content.Shared.Teleportation.Components;

[RegisterComponent]
[NetworkedComponent]
public sealed class HandTeleporterComponent : 
  Component,
  ISerializationGenerated<HandTeleporterComponent>,
  ISerializationGenerated
{
  [Robust.Shared.ViewVariables.ViewVariables]
  [DataField("firstPortal", false, 1, false, false, null)]
  public EntityUid? FirstPortal;
  [Robust.Shared.ViewVariables.ViewVariables]
  [DataField("secondPortal", false, 1, false, false, null)]
  public EntityUid? SecondPortal;
  [DataField(null, false, 1, false, false, null)]
  public bool AllowPortalsOnDifferentGrids;
  [DataField(null, false, 1, false, false, null)]
  public bool AllowPortalsOnDifferentMaps;
  [DataField("firstPortalPrototype", false, 1, false, false, typeof (PrototypeIdSerializer<EntityPrototype>))]
  public string FirstPortalPrototype;
  [DataField("secondPortalPrototype", false, 1, false, false, typeof (PrototypeIdSerializer<EntityPrototype>))]
  public string SecondPortalPrototype;
  [DataField("newPortalSound", false, 1, false, false, null)]
  public SoundSpecifier NewPortalSound;
  [DataField("clearPortalsSound", false, 1, false, false, null)]
  public SoundSpecifier ClearPortalsSound;
  [DataField("portalCreationDelay", false, 1, false, false, null)]
  public float PortalCreationDelay;

  public HandTeleporterComponent()
  {
    SoundPathSpecifier soundPathSpecifier = new SoundPathSpecifier("/Audio/Machines/high_tech_confirm.ogg");
    soundPathSpecifier.Params = AudioParams.Default.WithVolume(-2f);
    this.NewPortalSound = (SoundSpecifier) soundPathSpecifier;
    this.ClearPortalsSound = (SoundSpecifier) new SoundPathSpecifier("/Audio/Machines/button.ogg");
    this.PortalCreationDelay = 1f;
    // ISSUE: explicit constructor call
    base.\u002Ector();
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref HandTeleporterComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (HandTeleporterComponent) target1;
    if (serialization.TryCustomCopy<HandTeleporterComponent>(this, ref target, hookCtx, false, context))
      return;
    EntityUid? target2 = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.FirstPortal, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<EntityUid?>(this.FirstPortal, hookCtx, context);
    target.FirstPortal = target2;
    EntityUid? target3 = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.SecondPortal, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<EntityUid?>(this.SecondPortal, hookCtx, context);
    target.SecondPortal = target3;
    bool target4 = false;
    if (!serialization.TryCustomCopy<bool>(this.AllowPortalsOnDifferentGrids, ref target4, hookCtx, false, context))
      target4 = this.AllowPortalsOnDifferentGrids;
    target.AllowPortalsOnDifferentGrids = target4;
    bool target5 = false;
    if (!serialization.TryCustomCopy<bool>(this.AllowPortalsOnDifferentMaps, ref target5, hookCtx, false, context))
      target5 = this.AllowPortalsOnDifferentMaps;
    target.AllowPortalsOnDifferentMaps = target5;
    string target6 = (string) null;
    if (this.FirstPortalPrototype == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.FirstPortalPrototype, ref target6, hookCtx, false, context))
      target6 = this.FirstPortalPrototype;
    target.FirstPortalPrototype = target6;
    string target7 = (string) null;
    if (this.SecondPortalPrototype == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.SecondPortalPrototype, ref target7, hookCtx, false, context))
      target7 = this.SecondPortalPrototype;
    target.SecondPortalPrototype = target7;
    SoundSpecifier target8 = (SoundSpecifier) null;
    if (this.NewPortalSound == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.NewPortalSound, ref target8, hookCtx, true, context))
      target8 = serialization.CreateCopy<SoundSpecifier>(this.NewPortalSound, hookCtx, context);
    target.NewPortalSound = target8;
    SoundSpecifier target9 = (SoundSpecifier) null;
    if (this.ClearPortalsSound == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.ClearPortalsSound, ref target9, hookCtx, true, context))
      target9 = serialization.CreateCopy<SoundSpecifier>(this.ClearPortalsSound, hookCtx, context);
    target.ClearPortalsSound = target9;
    float target10 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.PortalCreationDelay, ref target10, hookCtx, false, context))
      target10 = this.PortalCreationDelay;
    target.PortalCreationDelay = target10;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref HandTeleporterComponent target,
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
    HandTeleporterComponent target1 = (HandTeleporterComponent) target;
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
    HandTeleporterComponent target1 = (HandTeleporterComponent) target;
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
    HandTeleporterComponent target1 = (HandTeleporterComponent) target;
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
  virtual HandTeleporterComponent Component.Instantiate() => new HandTeleporterComponent();
}
