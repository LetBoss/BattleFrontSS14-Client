// Decompiled with JetBrains decompiler
// Type: Content.Shared._PUBG.Emote.PubgEmoteComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._PUBG.Emote;

[RegisterComponent]
public sealed class PubgEmoteComponent : 
  Component,
  ISerializationGenerated<PubgEmoteComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, true, false, null)]
  public string EmoteId = string.Empty;
  [DataField(null, false, 1, false, false, null)]
  public string Description = string.Empty;
  [DataField(null, false, 1, false, false, null)]
  public EmotePlayMode PlayMode;
  [DataField(null, false, 1, false, false, null)]
  public float? Duration;
  [DataField(null, false, 1, false, false, null)]
  public int? RepeatCount;
  [DataField(null, false, 1, true, false, null)]
  public string RsiPath = string.Empty;
  [DataField(null, false, 1, true, false, null)]
  public string StateName = string.Empty;
  [DataField(null, false, 1, false, false, null)]
  public SoundSpecifier? Sound;
  [DataField(null, false, 1, false, false, null)]
  public bool Disabled;
  [DataField(null, false, 1, false, false, null)]
  public float Cooldown = 20f;
  [DataField(null, false, 1, false, false, null)]
  public float Scale = 1f;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref PubgEmoteComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (PubgEmoteComponent) target1;
    if (serialization.TryCustomCopy<PubgEmoteComponent>(this, ref target, hookCtx, false, context))
      return;
    string target2 = (string) null;
    if (this.EmoteId == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.EmoteId, ref target2, hookCtx, false, context))
      target2 = this.EmoteId;
    target.EmoteId = target2;
    string target3 = (string) null;
    if (this.Description == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.Description, ref target3, hookCtx, false, context))
      target3 = this.Description;
    target.Description = target3;
    EmotePlayMode target4 = EmotePlayMode.Once;
    if (!serialization.TryCustomCopy<EmotePlayMode>(this.PlayMode, ref target4, hookCtx, false, context))
      target4 = this.PlayMode;
    target.PlayMode = target4;
    float? target5 = new float?();
    if (!serialization.TryCustomCopy<float?>(this.Duration, ref target5, hookCtx, false, context))
      target5 = this.Duration;
    target.Duration = target5;
    int? target6 = new int?();
    if (!serialization.TryCustomCopy<int?>(this.RepeatCount, ref target6, hookCtx, false, context))
      target6 = this.RepeatCount;
    target.RepeatCount = target6;
    string target7 = (string) null;
    if (this.RsiPath == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.RsiPath, ref target7, hookCtx, false, context))
      target7 = this.RsiPath;
    target.RsiPath = target7;
    string target8 = (string) null;
    if (this.StateName == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.StateName, ref target8, hookCtx, false, context))
      target8 = this.StateName;
    target.StateName = target8;
    SoundSpecifier target9 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.Sound, ref target9, hookCtx, true, context))
      target9 = serialization.CreateCopy<SoundSpecifier>(this.Sound, hookCtx, context);
    target.Sound = target9;
    bool target10 = false;
    if (!serialization.TryCustomCopy<bool>(this.Disabled, ref target10, hookCtx, false, context))
      target10 = this.Disabled;
    target.Disabled = target10;
    float target11 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.Cooldown, ref target11, hookCtx, false, context))
      target11 = this.Cooldown;
    target.Cooldown = target11;
    float target12 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.Scale, ref target12, hookCtx, false, context))
      target12 = this.Scale;
    target.Scale = target12;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref PubgEmoteComponent target,
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
    PubgEmoteComponent target1 = (PubgEmoteComponent) target;
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
    PubgEmoteComponent target1 = (PubgEmoteComponent) target;
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
    PubgEmoteComponent target1 = (PubgEmoteComponent) target;
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
  virtual PubgEmoteComponent Component.Instantiate() => new PubgEmoteComponent();
}
