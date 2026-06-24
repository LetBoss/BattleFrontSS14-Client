// Decompiled with JetBrains decompiler
// Type: Content.Shared.Clothing.Components.FactionClothingComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Clothing.EntitySystems;
using Content.Shared.NPC.Prototypes;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Clothing.Components;

[RegisterComponent]
[NetworkedComponent]
[Access(new Type[] {typeof (FactionClothingSystem)})]
public sealed class FactionClothingComponent : 
  Component,
  ISerializationGenerated<FactionClothingComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, true, false, null)]
  public ProtoId<NpcFactionPrototype> Faction = ProtoId<NpcFactionPrototype>.op_Implicit(string.Empty);
  [DataField(null, false, 1, false, false, null)]
  public bool AlreadyMember;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref FactionClothingComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component component = (Component) target;
    this.InternalCopy(ref component, serialization, hookCtx, context);
    target = (FactionClothingComponent) component;
    if (serialization.TryCustomCopy<FactionClothingComponent>(this, ref target, hookCtx, false, context))
      return;
    ProtoId<NpcFactionPrototype> protoId = new ProtoId<NpcFactionPrototype>();
    if (!serialization.TryCustomCopy<ProtoId<NpcFactionPrototype>>(this.Faction, ref protoId, hookCtx, false, context))
      protoId = serialization.CreateCopy<ProtoId<NpcFactionPrototype>>(this.Faction, hookCtx, context, false);
    target.Faction = protoId;
    bool flag = false;
    if (!serialization.TryCustomCopy<bool>(this.AlreadyMember, ref flag, hookCtx, false, context))
      flag = this.AlreadyMember;
    target.AlreadyMember = flag;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref FactionClothingComponent target,
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
    FactionClothingComponent target1 = (FactionClothingComponent) target;
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
    FactionClothingComponent target1 = (FactionClothingComponent) target;
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
    FactionClothingComponent target1 = (FactionClothingComponent) target;
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
  virtual FactionClothingComponent Component.Instantiate() => new FactionClothingComponent();
}
