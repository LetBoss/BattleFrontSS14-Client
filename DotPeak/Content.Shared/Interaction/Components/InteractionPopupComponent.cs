// Decompiled with JetBrains decompiler
// Type: Content.Shared.Interaction.Components.InteractionPopupComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Interaction.Components;

[RegisterComponent]
[Access(new Type[] {typeof (InteractionPopupSystem)})]
public sealed class InteractionPopupComponent : 
  Component,
  ISerializationGenerated<InteractionPopupComponent>,
  ISerializationGenerated
{
  [DataField("interactDelay", false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public TimeSpan InteractDelay = TimeSpan.FromSeconds(1.0);
  [DataField("interactSuccessString", false, 1, false, false, null)]
  public string? InteractSuccessString;
  [DataField("interactFailureString", false, 1, false, false, null)]
  public string? InteractFailureString;
  [DataField("interactSuccessSound", false, 1, false, false, null)]
  public SoundSpecifier? InteractSuccessSound;
  [DataField("interactFailureSound", false, 1, false, false, null)]
  public SoundSpecifier? InteractFailureSound;
  [DataField(null, false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public EntProtoId? InteractSuccessSpawn;
  [DataField(null, false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public EntProtoId? InteractFailureSpawn;
  [DataField("successChance", false, 1, false, false, null)]
  public float SuccessChance = 1f;
  [DataField("messagePerceivedByOthers", false, 1, false, false, null)]
  public string? MessagePerceivedByOthers;
  [DataField("soundPerceivedByOthers", false, 1, false, false, null)]
  public bool SoundPerceivedByOthers = true;
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public TimeSpan LastInteractTime;
  [DataField(null, false, 1, false, false, null)]
  public bool OnActivate;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref InteractionPopupComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (InteractionPopupComponent) target1;
    if (serialization.TryCustomCopy<InteractionPopupComponent>(this, ref target, hookCtx, false, context))
      return;
    TimeSpan target2 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.InteractDelay, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<TimeSpan>(this.InteractDelay, hookCtx, context);
    target.InteractDelay = target2;
    string target3 = (string) null;
    if (!serialization.TryCustomCopy<string>(this.InteractSuccessString, ref target3, hookCtx, false, context))
      target3 = this.InteractSuccessString;
    target.InteractSuccessString = target3;
    string target4 = (string) null;
    if (!serialization.TryCustomCopy<string>(this.InteractFailureString, ref target4, hookCtx, false, context))
      target4 = this.InteractFailureString;
    target.InteractFailureString = target4;
    SoundSpecifier target5 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.InteractSuccessSound, ref target5, hookCtx, true, context))
      target5 = serialization.CreateCopy<SoundSpecifier>(this.InteractSuccessSound, hookCtx, context);
    target.InteractSuccessSound = target5;
    SoundSpecifier target6 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.InteractFailureSound, ref target6, hookCtx, true, context))
      target6 = serialization.CreateCopy<SoundSpecifier>(this.InteractFailureSound, hookCtx, context);
    target.InteractFailureSound = target6;
    EntProtoId? target7 = new EntProtoId?();
    if (!serialization.TryCustomCopy<EntProtoId?>(this.InteractSuccessSpawn, ref target7, hookCtx, false, context))
      target7 = serialization.CreateCopy<EntProtoId?>(this.InteractSuccessSpawn, hookCtx, context);
    target.InteractSuccessSpawn = target7;
    EntProtoId? target8 = new EntProtoId?();
    if (!serialization.TryCustomCopy<EntProtoId?>(this.InteractFailureSpawn, ref target8, hookCtx, false, context))
      target8 = serialization.CreateCopy<EntProtoId?>(this.InteractFailureSpawn, hookCtx, context);
    target.InteractFailureSpawn = target8;
    float target9 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.SuccessChance, ref target9, hookCtx, false, context))
      target9 = this.SuccessChance;
    target.SuccessChance = target9;
    string target10 = (string) null;
    if (!serialization.TryCustomCopy<string>(this.MessagePerceivedByOthers, ref target10, hookCtx, false, context))
      target10 = this.MessagePerceivedByOthers;
    target.MessagePerceivedByOthers = target10;
    bool target11 = false;
    if (!serialization.TryCustomCopy<bool>(this.SoundPerceivedByOthers, ref target11, hookCtx, false, context))
      target11 = this.SoundPerceivedByOthers;
    target.SoundPerceivedByOthers = target11;
    bool target12 = false;
    if (!serialization.TryCustomCopy<bool>(this.OnActivate, ref target12, hookCtx, false, context))
      target12 = this.OnActivate;
    target.OnActivate = target12;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref InteractionPopupComponent target,
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
    InteractionPopupComponent target1 = (InteractionPopupComponent) target;
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
    InteractionPopupComponent target1 = (InteractionPopupComponent) target;
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
    InteractionPopupComponent target1 = (InteractionPopupComponent) target;
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
  virtual InteractionPopupComponent Component.Instantiate() => new InteractionPopupComponent();
}
