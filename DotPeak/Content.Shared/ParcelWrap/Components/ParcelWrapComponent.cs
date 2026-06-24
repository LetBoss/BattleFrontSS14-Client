// Decompiled with JetBrains decompiler
// Type: Content.Shared.ParcelWrap.Components.ParcelWrapComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Item;
using Content.Shared.Whitelist;
using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.ParcelWrap.Components;

[RegisterComponent]
[NetworkedComponent]
[Access(new Type[] {})]
public sealed class ParcelWrapComponent : 
  Component,
  ISerializationGenerated<ParcelWrapComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, true, false, null)]
  public EntProtoId ParcelPrototype;
  [DataField(null, false, 1, false, false, null)]
  public bool WrappedItemsMaintainSize = true;
  [DataField(null, false, 1, false, false, null)]
  public ProtoId<ItemSizePrototype> FallbackItemSize = (ProtoId<ItemSizePrototype>) "Ginormous";
  [DataField(null, false, 1, false, false, null)]
  public bool WrappedItemsMaintainShape;
  [DataField(null, false, 1, true, false, null)]
  public TimeSpan WrapDelay = TimeSpan.FromSeconds(1L);
  [DataField(null, false, 1, false, false, null)]
  public SoundSpecifier? WrapSound;
  [DataField(null, false, 1, false, false, null)]
  public EntityWhitelist? Whitelist;
  [DataField(null, false, 1, false, false, null)]
  public EntityWhitelist? Blacklist;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref ParcelWrapComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (ParcelWrapComponent) target1;
    if (serialization.TryCustomCopy<ParcelWrapComponent>(this, ref target, hookCtx, false, context))
      return;
    EntProtoId target2 = new EntProtoId();
    if (!serialization.TryCustomCopy<EntProtoId>(this.ParcelPrototype, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<EntProtoId>(this.ParcelPrototype, hookCtx, context);
    target.ParcelPrototype = target2;
    bool target3 = false;
    if (!serialization.TryCustomCopy<bool>(this.WrappedItemsMaintainSize, ref target3, hookCtx, false, context))
      target3 = this.WrappedItemsMaintainSize;
    target.WrappedItemsMaintainSize = target3;
    ProtoId<ItemSizePrototype> target4 = new ProtoId<ItemSizePrototype>();
    if (!serialization.TryCustomCopy<ProtoId<ItemSizePrototype>>(this.FallbackItemSize, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<ProtoId<ItemSizePrototype>>(this.FallbackItemSize, hookCtx, context);
    target.FallbackItemSize = target4;
    bool target5 = false;
    if (!serialization.TryCustomCopy<bool>(this.WrappedItemsMaintainShape, ref target5, hookCtx, false, context))
      target5 = this.WrappedItemsMaintainShape;
    target.WrappedItemsMaintainShape = target5;
    TimeSpan target6 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.WrapDelay, ref target6, hookCtx, false, context))
      target6 = serialization.CreateCopy<TimeSpan>(this.WrapDelay, hookCtx, context);
    target.WrapDelay = target6;
    SoundSpecifier target7 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.WrapSound, ref target7, hookCtx, true, context))
      target7 = serialization.CreateCopy<SoundSpecifier>(this.WrapSound, hookCtx, context);
    target.WrapSound = target7;
    EntityWhitelist target8 = (EntityWhitelist) null;
    if (!serialization.TryCustomCopy<EntityWhitelist>(this.Whitelist, ref target8, hookCtx, false, context))
    {
      if (this.Whitelist == null)
        target8 = (EntityWhitelist) null;
      else
        serialization.CopyTo<EntityWhitelist>(this.Whitelist, ref target8, hookCtx, context);
    }
    target.Whitelist = target8;
    EntityWhitelist target9 = (EntityWhitelist) null;
    if (!serialization.TryCustomCopy<EntityWhitelist>(this.Blacklist, ref target9, hookCtx, false, context))
    {
      if (this.Blacklist == null)
        target9 = (EntityWhitelist) null;
      else
        serialization.CopyTo<EntityWhitelist>(this.Blacklist, ref target9, hookCtx, context);
    }
    target.Blacklist = target9;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref ParcelWrapComponent target,
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
    ParcelWrapComponent target1 = (ParcelWrapComponent) target;
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
    ParcelWrapComponent target1 = (ParcelWrapComponent) target;
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
    ParcelWrapComponent target1 = (ParcelWrapComponent) target;
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
  virtual ParcelWrapComponent Component.Instantiate() => new ParcelWrapComponent();
}
