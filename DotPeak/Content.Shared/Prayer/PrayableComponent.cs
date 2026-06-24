// Decompiled with JetBrains decompiler
// Type: Content.Shared.Prayer.PrayableComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Utility;
using Robust.Shared.ViewVariables;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Prayer;

[RegisterComponent]
[NetworkedComponent]
public sealed class PrayableComponent : 
  Component,
  ISerializationGenerated<PrayableComponent>,
  ISerializationGenerated
{
  [DataField("bibleUserOnly", false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public bool BibleUserOnly;
  [DataField("sentMessage", false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public string SentMessage = "prayer-popup-notify-pray-sent";
  [DataField("notificationPrefix", false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public string NotificationPrefix = "prayer-chat-notify-pray";
  [DataField("verb", false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadOnly)]
  public string Verb = "prayer-verbs-pray";
  [DataField("verbImage", false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadOnly)]
  public SpriteSpecifier? VerbImage = (SpriteSpecifier) new SpriteSpecifier.Texture(new ResPath("/Textures/Interface/pray.svg.png"));

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref PrayableComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (PrayableComponent) target1;
    if (serialization.TryCustomCopy<PrayableComponent>(this, ref target, hookCtx, false, context))
      return;
    bool target2 = false;
    if (!serialization.TryCustomCopy<bool>(this.BibleUserOnly, ref target2, hookCtx, false, context))
      target2 = this.BibleUserOnly;
    target.BibleUserOnly = target2;
    string target3 = (string) null;
    if (this.SentMessage == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.SentMessage, ref target3, hookCtx, false, context))
      target3 = this.SentMessage;
    target.SentMessage = target3;
    string target4 = (string) null;
    if (this.NotificationPrefix == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.NotificationPrefix, ref target4, hookCtx, false, context))
      target4 = this.NotificationPrefix;
    target.NotificationPrefix = target4;
    string target5 = (string) null;
    if (this.Verb == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.Verb, ref target5, hookCtx, false, context))
      target5 = this.Verb;
    target.Verb = target5;
    SpriteSpecifier target6 = (SpriteSpecifier) null;
    if (!serialization.TryCustomCopy<SpriteSpecifier>(this.VerbImage, ref target6, hookCtx, true, context))
      target6 = serialization.CreateCopy<SpriteSpecifier>(this.VerbImage, hookCtx, context);
    target.VerbImage = target6;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref PrayableComponent target,
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
    PrayableComponent target1 = (PrayableComponent) target;
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
    PrayableComponent target1 = (PrayableComponent) target;
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
    PrayableComponent target1 = (PrayableComponent) target;
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
  virtual PrayableComponent Component.Instantiate() => new PrayableComponent();
}
