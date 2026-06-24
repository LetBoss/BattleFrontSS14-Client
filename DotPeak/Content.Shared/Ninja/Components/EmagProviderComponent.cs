// Decompiled with JetBrains decompiler
// Type: Content.Shared.Ninja.Components.EmagProviderComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Emag.Systems;
using Content.Shared.Ninja.Systems;
using Content.Shared.Tag;
using Content.Shared.Whitelist;
using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Ninja.Components;

[RegisterComponent]
[NetworkedComponent]
[Access(new Type[] {typeof (EmagProviderSystem)})]
public sealed class EmagProviderComponent : 
  Component,
  ISerializationGenerated<EmagProviderComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public ProtoId<TagPrototype> AccessBreakerImmuneTag = (ProtoId<TagPrototype>) "AccessBreakerImmune";
  [DataField(null, false, 1, false, false, null)]
  public EntityWhitelist? Whitelist;
  [DataField(null, false, 1, false, false, null)]
  public EmagType EmagType = EmagType.Access;
  [DataField(null, false, 1, false, false, null)]
  public SoundSpecifier EmagSound = (SoundSpecifier) new SoundCollectionSpecifier("sparks");

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref EmagProviderComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (EmagProviderComponent) target1;
    if (serialization.TryCustomCopy<EmagProviderComponent>(this, ref target, hookCtx, false, context))
      return;
    ProtoId<TagPrototype> target2 = new ProtoId<TagPrototype>();
    if (!serialization.TryCustomCopy<ProtoId<TagPrototype>>(this.AccessBreakerImmuneTag, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<ProtoId<TagPrototype>>(this.AccessBreakerImmuneTag, hookCtx, context);
    target.AccessBreakerImmuneTag = target2;
    EntityWhitelist target3 = (EntityWhitelist) null;
    if (!serialization.TryCustomCopy<EntityWhitelist>(this.Whitelist, ref target3, hookCtx, false, context))
    {
      if (this.Whitelist == null)
        target3 = (EntityWhitelist) null;
      else
        serialization.CopyTo<EntityWhitelist>(this.Whitelist, ref target3, hookCtx, context);
    }
    target.Whitelist = target3;
    EmagType target4 = EmagType.None;
    if (!serialization.TryCustomCopy<EmagType>(this.EmagType, ref target4, hookCtx, false, context))
      target4 = this.EmagType;
    target.EmagType = target4;
    SoundSpecifier target5 = (SoundSpecifier) null;
    if (this.EmagSound == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.EmagSound, ref target5, hookCtx, true, context))
      target5 = serialization.CreateCopy<SoundSpecifier>(this.EmagSound, hookCtx, context);
    target.EmagSound = target5;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref EmagProviderComponent target,
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
    EmagProviderComponent target1 = (EmagProviderComponent) target;
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
    EmagProviderComponent target1 = (EmagProviderComponent) target;
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
    EmagProviderComponent target1 = (EmagProviderComponent) target;
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
  virtual EmagProviderComponent Component.Instantiate() => new EmagProviderComponent();
}
