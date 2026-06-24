// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Weapons.Ranged.Flamer.RMCFlamerChainComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Line;
using Content.Shared.Chemistry.Reagent;
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
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Weapons.Ranged.Flamer;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (SharedRMCFlamerSystem)})]
public sealed class RMCFlamerChainComponent : 
  Component,
  ISerializationGenerated<RMCFlamerChainComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntProtoId Spawn = (EntProtoId) "RMCTileFire";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public List<LineTile> Tiles = new List<LineTile>();
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public ProtoId<ReagentPrototype> Reagent = (ProtoId<ReagentPrototype>) "RMCNapalmUT";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int MaxIntensity = 20;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int MaxDuration = 24;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref RMCFlamerChainComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (RMCFlamerChainComponent) target1;
    if (serialization.TryCustomCopy<RMCFlamerChainComponent>(this, ref target, hookCtx, false, context))
      return;
    EntProtoId target2 = new EntProtoId();
    if (!serialization.TryCustomCopy<EntProtoId>(this.Spawn, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<EntProtoId>(this.Spawn, hookCtx, context);
    target.Spawn = target2;
    List<LineTile> target3 = (List<LineTile>) null;
    if (this.Tiles == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<List<LineTile>>(this.Tiles, ref target3, hookCtx, true, context))
      target3 = serialization.CreateCopy<List<LineTile>>(this.Tiles, hookCtx, context);
    target.Tiles = target3;
    ProtoId<ReagentPrototype> target4 = new ProtoId<ReagentPrototype>();
    if (!serialization.TryCustomCopy<ProtoId<ReagentPrototype>>(this.Reagent, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<ProtoId<ReagentPrototype>>(this.Reagent, hookCtx, context);
    target.Reagent = target4;
    int target5 = 0;
    if (!serialization.TryCustomCopy<int>(this.MaxIntensity, ref target5, hookCtx, false, context))
      target5 = this.MaxIntensity;
    target.MaxIntensity = target5;
    int target6 = 0;
    if (!serialization.TryCustomCopy<int>(this.MaxDuration, ref target6, hookCtx, false, context))
      target6 = this.MaxDuration;
    target.MaxDuration = target6;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref RMCFlamerChainComponent target,
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
    RMCFlamerChainComponent target1 = (RMCFlamerChainComponent) target;
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
    RMCFlamerChainComponent target1 = (RMCFlamerChainComponent) target;
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
    RMCFlamerChainComponent target1 = (RMCFlamerChainComponent) target;
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
  virtual RMCFlamerChainComponent Component.Instantiate() => new RMCFlamerChainComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class RMCFlamerChainComponent_AutoState : IComponentState
  {
    public EntProtoId Spawn;
    public List<LineTile> Tiles;
    public ProtoId<ReagentPrototype> Reagent;
    public int MaxIntensity;
    public int MaxDuration;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class RMCFlamerChainComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<RMCFlamerChainComponent, ComponentGetState>(new ComponentEventRefHandler<RMCFlamerChainComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<RMCFlamerChainComponent, ComponentHandleState>(new ComponentEventRefHandler<RMCFlamerChainComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      RMCFlamerChainComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new RMCFlamerChainComponent.RMCFlamerChainComponent_AutoState()
      {
        Spawn = component.Spawn,
        Tiles = component.Tiles,
        Reagent = component.Reagent,
        MaxIntensity = component.MaxIntensity,
        MaxDuration = component.MaxDuration
      };
    }

    private void OnHandleState(
      EntityUid uid,
      RMCFlamerChainComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is RMCFlamerChainComponent.RMCFlamerChainComponent_AutoState current))
        return;
      component.Spawn = current.Spawn;
      component.Tiles = current.Tiles == null ? (List<LineTile>) null : new List<LineTile>((IEnumerable<LineTile>) current.Tiles);
      component.Reagent = current.Reagent;
      component.MaxIntensity = current.MaxIntensity;
      component.MaxDuration = current.MaxDuration;
    }
  }
}
