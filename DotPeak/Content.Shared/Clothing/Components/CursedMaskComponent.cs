// Decompiled with JetBrains decompiler
// Type: Content.Shared.Clothing.Components.CursedMaskComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Damage;
using Content.Shared.NPC.Prototypes;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Clothing.Components;

[RegisterComponent]
[NetworkedComponent]
[Access(new Type[] {typeof (SharedCursedMaskSystem)})]
public sealed class CursedMaskComponent : 
  Component,
  ISerializationGenerated<CursedMaskComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public CursedMaskExpression CurrentState;
  [DataField(null, false, 1, false, false, null)]
  public float JoySpeedModifier = 1.15f;
  [DataField(null, false, 1, false, false, null)]
  public DamageModifierSet DespairDamageModifier = new DamageModifierSet();
  [DataField(null, false, 1, false, false, null)]
  public bool HasNpc;
  [DataField(null, false, 1, false, false, null)]
  public EntityUid? StolenMind;
  [DataField(null, false, 1, false, false, null)]
  public ProtoId<NpcFactionPrototype> CursedMaskFaction = ProtoId<NpcFactionPrototype>.op_Implicit("SimpleHostile");
  [DataField(null, false, 1, false, false, null)]
  public HashSet<ProtoId<NpcFactionPrototype>> OldFactions = new HashSet<ProtoId<NpcFactionPrototype>>();

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref CursedMaskComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component component = (Component) target;
    this.InternalCopy(ref component, serialization, hookCtx, context);
    target = (CursedMaskComponent) component;
    if (serialization.TryCustomCopy<CursedMaskComponent>(this, ref target, hookCtx, false, context))
      return;
    CursedMaskExpression cursedMaskExpression = CursedMaskExpression.Neutral;
    if (!serialization.TryCustomCopy<CursedMaskExpression>(this.CurrentState, ref cursedMaskExpression, hookCtx, false, context))
      cursedMaskExpression = this.CurrentState;
    target.CurrentState = cursedMaskExpression;
    float num = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.JoySpeedModifier, ref num, hookCtx, false, context))
      num = this.JoySpeedModifier;
    target.JoySpeedModifier = num;
    DamageModifierSet damageModifierSet = (DamageModifierSet) null;
    if (this.DespairDamageModifier == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<DamageModifierSet>(this.DespairDamageModifier, ref damageModifierSet, hookCtx, true, context))
    {
      if (this.DespairDamageModifier == null)
        damageModifierSet = (DamageModifierSet) null;
      else
        serialization.CopyTo<DamageModifierSet>(this.DespairDamageModifier, ref damageModifierSet, hookCtx, context, true);
    }
    target.DespairDamageModifier = damageModifierSet;
    bool flag = false;
    if (!serialization.TryCustomCopy<bool>(this.HasNpc, ref flag, hookCtx, false, context))
      flag = this.HasNpc;
    target.HasNpc = flag;
    EntityUid? nullable = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.StolenMind, ref nullable, hookCtx, false, context))
      nullable = serialization.CreateCopy<EntityUid?>(this.StolenMind, hookCtx, context, false);
    target.StolenMind = nullable;
    ProtoId<NpcFactionPrototype> protoId = new ProtoId<NpcFactionPrototype>();
    if (!serialization.TryCustomCopy<ProtoId<NpcFactionPrototype>>(this.CursedMaskFaction, ref protoId, hookCtx, false, context))
      protoId = serialization.CreateCopy<ProtoId<NpcFactionPrototype>>(this.CursedMaskFaction, hookCtx, context, false);
    target.CursedMaskFaction = protoId;
    HashSet<ProtoId<NpcFactionPrototype>> protoIdSet = (HashSet<ProtoId<NpcFactionPrototype>>) null;
    if (this.OldFactions == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<HashSet<ProtoId<NpcFactionPrototype>>>(this.OldFactions, ref protoIdSet, hookCtx, true, context))
      protoIdSet = serialization.CreateCopy<HashSet<ProtoId<NpcFactionPrototype>>>(this.OldFactions, hookCtx, context, false);
    target.OldFactions = protoIdSet;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref CursedMaskComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void Copy(
    ref Component target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    CursedMaskComponent target1 = (CursedMaskComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (Component) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    CursedMaskComponent target1 = (CursedMaskComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void InternalCopy(
    ref IComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    CursedMaskComponent target1 = (CursedMaskComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (IComponent) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void Copy(
    ref IComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    base.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual CursedMaskComponent Component.Instantiate() => new CursedMaskComponent();
}
