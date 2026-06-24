// Decompiled with JetBrains decompiler
// Type: Content.Shared._PUBG.MiniGames.PubgCustomArenaComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Network;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._PUBG.MiniGames;

[RegisterComponent]
[NetworkedComponent]
public sealed class PubgCustomArenaComponent : 
  Component,
  ISerializationGenerated<PubgCustomArenaComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public string DisplayName { get; set; } = string.Empty;

  [DataField(null, false, 1, false, false, null)]
  public NetUserId AuthorUserId { get; set; }

  [DataField(null, false, 1, false, false, null)]
  public string AuthorCkey { get; set; } = string.Empty;

  [DataField(null, false, 1, false, false, null)]
  public DateTime DateCreated { get; set; }

  [DataField(null, false, 1, false, false, null)]
  public string FileName { get; set; } = string.Empty;

  [DataField(null, false, 1, false, false, null)]
  public bool IsInCustomizationMode { get; set; }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref PubgCustomArenaComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (PubgCustomArenaComponent) target1;
    if (serialization.TryCustomCopy<PubgCustomArenaComponent>(this, ref target, hookCtx, false, context))
      return;
    string target2 = (string) null;
    if (this.DisplayName == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.DisplayName, ref target2, hookCtx, false, context))
      target2 = this.DisplayName;
    target.DisplayName = target2;
    NetUserId target3 = new NetUserId();
    if (!serialization.TryCustomCopy<NetUserId>(this.AuthorUserId, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<NetUserId>(this.AuthorUserId, hookCtx, context);
    target.AuthorUserId = target3;
    string target4 = (string) null;
    if (this.AuthorCkey == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.AuthorCkey, ref target4, hookCtx, false, context))
      target4 = this.AuthorCkey;
    target.AuthorCkey = target4;
    DateTime target5 = new DateTime();
    if (!serialization.TryCustomCopy<DateTime>(this.DateCreated, ref target5, hookCtx, false, context))
      target5 = this.DateCreated;
    target.DateCreated = target5;
    string target6 = (string) null;
    if (this.FileName == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.FileName, ref target6, hookCtx, false, context))
      target6 = this.FileName;
    target.FileName = target6;
    bool target7 = false;
    if (!serialization.TryCustomCopy<bool>(this.IsInCustomizationMode, ref target7, hookCtx, false, context))
      target7 = this.IsInCustomizationMode;
    target.IsInCustomizationMode = target7;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref PubgCustomArenaComponent target,
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
    PubgCustomArenaComponent target1 = (PubgCustomArenaComponent) target;
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
    PubgCustomArenaComponent target1 = (PubgCustomArenaComponent) target;
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
    PubgCustomArenaComponent target1 = (PubgCustomArenaComponent) target;
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
  virtual PubgCustomArenaComponent Component.Instantiate() => new PubgCustomArenaComponent();
}
