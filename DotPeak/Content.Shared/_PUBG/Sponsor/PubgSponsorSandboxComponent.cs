// Decompiled with JetBrains decompiler
// Type: Content.Shared._PUBG.Sponsor.PubgSponsorSandboxComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._PUBG.Sponsor;

[RegisterComponent]
[NetworkedComponent]
public sealed class PubgSponsorSandboxComponent : 
  Component,
  ISerializationGenerated<PubgSponsorSandboxComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public List<string> Ckeys = new List<string>();
  [DataField(null, false, 1, false, false, null)]
  public Dictionary<string, List<string>> Permissions = new Dictionary<string, List<string>>();
  [DataField(null, false, 1, false, false, null)]
  public List<string> DisallowedEntityIds = new List<string>();
  [DataField(null, false, 1, false, false, null)]
  public bool BlockEraseMinds;
  [DataField(null, false, 1, false, false, null)]
  public bool IsMiniGameSandbox;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref PubgSponsorSandboxComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (PubgSponsorSandboxComponent) target1;
    if (serialization.TryCustomCopy<PubgSponsorSandboxComponent>(this, ref target, hookCtx, false, context))
      return;
    List<string> target2 = (List<string>) null;
    if (this.Ckeys == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<List<string>>(this.Ckeys, ref target2, hookCtx, true, context))
      target2 = serialization.CreateCopy<List<string>>(this.Ckeys, hookCtx, context);
    target.Ckeys = target2;
    Dictionary<string, List<string>> target3 = (Dictionary<string, List<string>>) null;
    if (this.Permissions == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<Dictionary<string, List<string>>>(this.Permissions, ref target3, hookCtx, true, context))
      target3 = serialization.CreateCopy<Dictionary<string, List<string>>>(this.Permissions, hookCtx, context);
    target.Permissions = target3;
    List<string> target4 = (List<string>) null;
    if (this.DisallowedEntityIds == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<List<string>>(this.DisallowedEntityIds, ref target4, hookCtx, true, context))
      target4 = serialization.CreateCopy<List<string>>(this.DisallowedEntityIds, hookCtx, context);
    target.DisallowedEntityIds = target4;
    bool target5 = false;
    if (!serialization.TryCustomCopy<bool>(this.BlockEraseMinds, ref target5, hookCtx, false, context))
      target5 = this.BlockEraseMinds;
    target.BlockEraseMinds = target5;
    bool target6 = false;
    if (!serialization.TryCustomCopy<bool>(this.IsMiniGameSandbox, ref target6, hookCtx, false, context))
      target6 = this.IsMiniGameSandbox;
    target.IsMiniGameSandbox = target6;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref PubgSponsorSandboxComponent target,
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
    PubgSponsorSandboxComponent target1 = (PubgSponsorSandboxComponent) target;
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
    PubgSponsorSandboxComponent target1 = (PubgSponsorSandboxComponent) target;
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
    PubgSponsorSandboxComponent target1 = (PubgSponsorSandboxComponent) target;
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
  virtual PubgSponsorSandboxComponent Component.Instantiate() => new PubgSponsorSandboxComponent();
}
