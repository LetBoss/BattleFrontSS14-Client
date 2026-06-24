// Decompiled with JetBrains decompiler
// Type: Content.Shared.Stunnable.StunOnContactComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Whitelist;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Stunnable;

[RegisterComponent]
[NetworkedComponent]
[Access(new Type[] {typeof (SharedStunSystem)})]
public sealed class StunOnContactComponent : 
  Component,
  ISerializationGenerated<StunOnContactComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public string FixtureId = "fix";
  [DataField(null, false, 1, false, false, null)]
  public TimeSpan Duration = TimeSpan.FromSeconds(5L);
  [DataField(null, false, 1, false, false, null)]
  public EntityWhitelist Blacklist = new EntityWhitelist();

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref StunOnContactComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (StunOnContactComponent) target1;
    if (serialization.TryCustomCopy<StunOnContactComponent>(this, ref target, hookCtx, false, context))
      return;
    string target2 = (string) null;
    if (this.FixtureId == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.FixtureId, ref target2, hookCtx, false, context))
      target2 = this.FixtureId;
    target.FixtureId = target2;
    TimeSpan target3 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.Duration, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<TimeSpan>(this.Duration, hookCtx, context);
    target.Duration = target3;
    EntityWhitelist target4 = (EntityWhitelist) null;
    if (this.Blacklist == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<EntityWhitelist>(this.Blacklist, ref target4, hookCtx, false, context))
    {
      if (this.Blacklist == null)
        target4 = (EntityWhitelist) null;
      else
        serialization.CopyTo<EntityWhitelist>(this.Blacklist, ref target4, hookCtx, context, true);
    }
    target.Blacklist = target4;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref StunOnContactComponent target,
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
    StunOnContactComponent target1 = (StunOnContactComponent) target;
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
    StunOnContactComponent target1 = (StunOnContactComponent) target;
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
    StunOnContactComponent target1 = (StunOnContactComponent) target;
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
  virtual StunOnContactComponent Component.Instantiate() => new StunOnContactComponent();
}
