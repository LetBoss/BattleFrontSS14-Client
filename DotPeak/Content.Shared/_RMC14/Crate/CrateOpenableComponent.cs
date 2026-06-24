// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Crate.CrateOpenableComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Storage;
using Content.Shared.Tools;
using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Localization;
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
namespace Content.Shared._RMC14.Crate;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (CrateOpenableSystem)})]
public sealed class CrateOpenableComponent : 
  Component,
  ISerializationGenerated<CrateOpenableComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public ProtoId<ToolQualityPrototype> Tool = (ProtoId<ToolQualityPrototype>) "Prying";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public List<EntitySpawnEntry> Spawn = new List<EntitySpawnEntry>()
  {
    new EntitySpawnEntry()
    {
      PrototypeId = (EntProtoId?) "CMSheetMetal2"
    }
  };
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public LocId WrongToolPopup = (LocId) "rmc-crate-openable-need-crowbar";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SoundSpecifier? Sound = (SoundSpecifier) new SoundPathSpecifier("/Audio/_RMC14/Structures/metalhit.ogg");

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref CrateOpenableComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (CrateOpenableComponent) target1;
    if (serialization.TryCustomCopy<CrateOpenableComponent>(this, ref target, hookCtx, false, context))
      return;
    ProtoId<ToolQualityPrototype> target2 = new ProtoId<ToolQualityPrototype>();
    if (!serialization.TryCustomCopy<ProtoId<ToolQualityPrototype>>(this.Tool, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<ProtoId<ToolQualityPrototype>>(this.Tool, hookCtx, context);
    target.Tool = target2;
    List<EntitySpawnEntry> target3 = (List<EntitySpawnEntry>) null;
    if (this.Spawn == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<List<EntitySpawnEntry>>(this.Spawn, ref target3, hookCtx, true, context))
      target3 = serialization.CreateCopy<List<EntitySpawnEntry>>(this.Spawn, hookCtx, context);
    target.Spawn = target3;
    LocId target4 = new LocId();
    if (!serialization.TryCustomCopy<LocId>(this.WrongToolPopup, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<LocId>(this.WrongToolPopup, hookCtx, context);
    target.WrongToolPopup = target4;
    SoundSpecifier target5 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.Sound, ref target5, hookCtx, true, context))
      target5 = serialization.CreateCopy<SoundSpecifier>(this.Sound, hookCtx, context);
    target.Sound = target5;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref CrateOpenableComponent target,
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
    CrateOpenableComponent target1 = (CrateOpenableComponent) target;
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
    CrateOpenableComponent target1 = (CrateOpenableComponent) target;
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
    CrateOpenableComponent target1 = (CrateOpenableComponent) target;
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
  virtual CrateOpenableComponent Component.Instantiate() => new CrateOpenableComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class CrateOpenableComponent_AutoState : IComponentState
  {
    public ProtoId<ToolQualityPrototype> Tool;
    public List<EntitySpawnEntry> Spawn;
    public LocId WrongToolPopup;
    public SoundSpecifier? Sound;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class CrateOpenableComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<CrateOpenableComponent, ComponentGetState>(new ComponentEventRefHandler<CrateOpenableComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<CrateOpenableComponent, ComponentHandleState>(new ComponentEventRefHandler<CrateOpenableComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      CrateOpenableComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new CrateOpenableComponent.CrateOpenableComponent_AutoState()
      {
        Tool = component.Tool,
        Spawn = component.Spawn,
        WrongToolPopup = component.WrongToolPopup,
        Sound = component.Sound
      };
    }

    private void OnHandleState(
      EntityUid uid,
      CrateOpenableComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is CrateOpenableComponent.CrateOpenableComponent_AutoState current))
        return;
      component.Tool = current.Tool;
      component.Spawn = current.Spawn == null ? (List<EntitySpawnEntry>) null : new List<EntitySpawnEntry>((IEnumerable<EntitySpawnEntry>) current.Spawn);
      component.WrongToolPopup = current.WrongToolPopup;
      component.Sound = current.Sound;
    }
  }
}
