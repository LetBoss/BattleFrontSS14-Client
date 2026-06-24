// Decompiled with JetBrains decompiler
// Type: Content.Shared.Polymorph.Components.ChameleonProjectorComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Polymorph.Systems;
using Content.Shared.Whitelist;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Polymorph.Components;

[RegisterComponent]
[Access(new Type[] {typeof (SharedChameleonProjectorSystem)})]
public sealed class ChameleonProjectorComponent : 
  Component,
  ISerializationGenerated<ChameleonProjectorComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, true, false, null)]
  public EntityWhitelist? Whitelist;
  [DataField(null, false, 1, true, false, null)]
  public EntityWhitelist? Blacklist;
  [DataField(null, false, 1, true, false, null)]
  public EntProtoId DisguiseProto = (EntProtoId) string.Empty;
  [DataField(null, false, 1, false, false, null)]
  public EntProtoId NoRotAction = (EntProtoId) "ActionDisguiseNoRot";
  [DataField(null, false, 1, false, false, null)]
  public EntityUid? NoRotActionEntity;
  [DataField(null, false, 1, false, false, null)]
  public EntProtoId AnchorAction = (EntProtoId) "ActionDisguiseAnchor";
  [DataField(null, false, 1, false, false, null)]
  public EntityUid? AnchorActionEntity;
  [DataField(null, false, 1, false, false, null)]
  public float MinHealth = 1f;
  [DataField(null, false, 1, false, false, null)]
  public float MaxHealth = 100f;
  [DataField(null, false, 1, false, false, null)]
  public EntityUid? Disguised;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref ChameleonProjectorComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (ChameleonProjectorComponent) target1;
    if (serialization.TryCustomCopy<ChameleonProjectorComponent>(this, ref target, hookCtx, false, context))
      return;
    EntityWhitelist target2 = (EntityWhitelist) null;
    if (!serialization.TryCustomCopy<EntityWhitelist>(this.Whitelist, ref target2, hookCtx, false, context))
    {
      if (this.Whitelist == null)
        target2 = (EntityWhitelist) null;
      else
        serialization.CopyTo<EntityWhitelist>(this.Whitelist, ref target2, hookCtx, context);
    }
    target.Whitelist = target2;
    EntityWhitelist target3 = (EntityWhitelist) null;
    if (!serialization.TryCustomCopy<EntityWhitelist>(this.Blacklist, ref target3, hookCtx, false, context))
    {
      if (this.Blacklist == null)
        target3 = (EntityWhitelist) null;
      else
        serialization.CopyTo<EntityWhitelist>(this.Blacklist, ref target3, hookCtx, context);
    }
    target.Blacklist = target3;
    EntProtoId target4 = new EntProtoId();
    if (!serialization.TryCustomCopy<EntProtoId>(this.DisguiseProto, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<EntProtoId>(this.DisguiseProto, hookCtx, context);
    target.DisguiseProto = target4;
    EntProtoId target5 = new EntProtoId();
    if (!serialization.TryCustomCopy<EntProtoId>(this.NoRotAction, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<EntProtoId>(this.NoRotAction, hookCtx, context);
    target.NoRotAction = target5;
    EntityUid? target6 = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.NoRotActionEntity, ref target6, hookCtx, false, context))
      target6 = serialization.CreateCopy<EntityUid?>(this.NoRotActionEntity, hookCtx, context);
    target.NoRotActionEntity = target6;
    EntProtoId target7 = new EntProtoId();
    if (!serialization.TryCustomCopy<EntProtoId>(this.AnchorAction, ref target7, hookCtx, false, context))
      target7 = serialization.CreateCopy<EntProtoId>(this.AnchorAction, hookCtx, context);
    target.AnchorAction = target7;
    EntityUid? target8 = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.AnchorActionEntity, ref target8, hookCtx, false, context))
      target8 = serialization.CreateCopy<EntityUid?>(this.AnchorActionEntity, hookCtx, context);
    target.AnchorActionEntity = target8;
    float target9 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.MinHealth, ref target9, hookCtx, false, context))
      target9 = this.MinHealth;
    target.MinHealth = target9;
    float target10 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.MaxHealth, ref target10, hookCtx, false, context))
      target10 = this.MaxHealth;
    target.MaxHealth = target10;
    EntityUid? target11 = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.Disguised, ref target11, hookCtx, false, context))
      target11 = serialization.CreateCopy<EntityUid?>(this.Disguised, hookCtx, context);
    target.Disguised = target11;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref ChameleonProjectorComponent target,
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
    ChameleonProjectorComponent target1 = (ChameleonProjectorComponent) target;
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
    ChameleonProjectorComponent target1 = (ChameleonProjectorComponent) target;
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
    ChameleonProjectorComponent target1 = (ChameleonProjectorComponent) target;
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
  virtual ChameleonProjectorComponent Component.Instantiate() => new ChameleonProjectorComponent();
}
