// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.NightVision.NightVisionItemComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Marines.Skills;
using Content.Shared.Inventory;
using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.NightVision;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (SharedNightVisionSystem)})]
public sealed class NightVisionItemComponent : 
  Component,
  ISerializationGenerated<NightVisionItemComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntProtoId? ActionId = (EntProtoId?) "CMActionToggleScoutVision";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntityUid? Action;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntityUid? User;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool Toggleable = true;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public Dictionary<EntProtoId<SkillDefinitionComponent>, int>? Skills;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool Green;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool Mesons;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool BlockScopes;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SoundSpecifier? SoundOn = (SoundSpecifier) new SoundPathSpecifier("/Audio/_RMC14/Handling/toggle_nv1.ogg");
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SoundSpecifier? SoundOff = (SoundSpecifier) new SoundPathSpecifier("/Audio/_RMC14/Handling/toggle_nv2.ogg");

  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SlotFlags SlotFlags { get; set; } = SlotFlags.EYES;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref NightVisionItemComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (NightVisionItemComponent) target1;
    if (serialization.TryCustomCopy<NightVisionItemComponent>(this, ref target, hookCtx, false, context))
      return;
    EntProtoId? target2 = new EntProtoId?();
    if (!serialization.TryCustomCopy<EntProtoId?>(this.ActionId, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<EntProtoId?>(this.ActionId, hookCtx, context);
    target.ActionId = target2;
    EntityUid? target3 = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.Action, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<EntityUid?>(this.Action, hookCtx, context);
    target.Action = target3;
    EntityUid? target4 = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.User, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<EntityUid?>(this.User, hookCtx, context);
    target.User = target4;
    bool target5 = false;
    if (!serialization.TryCustomCopy<bool>(this.Toggleable, ref target5, hookCtx, false, context))
      target5 = this.Toggleable;
    target.Toggleable = target5;
    SlotFlags target6 = SlotFlags.NONE;
    if (!serialization.TryCustomCopy<SlotFlags>(this.SlotFlags, ref target6, hookCtx, false, context))
      target6 = this.SlotFlags;
    target.SlotFlags = target6;
    Dictionary<EntProtoId<SkillDefinitionComponent>, int> target7 = (Dictionary<EntProtoId<SkillDefinitionComponent>, int>) null;
    if (!serialization.TryCustomCopy<Dictionary<EntProtoId<SkillDefinitionComponent>, int>>(this.Skills, ref target7, hookCtx, true, context))
      target7 = serialization.CreateCopy<Dictionary<EntProtoId<SkillDefinitionComponent>, int>>(this.Skills, hookCtx, context);
    target.Skills = target7;
    bool target8 = false;
    if (!serialization.TryCustomCopy<bool>(this.Green, ref target8, hookCtx, false, context))
      target8 = this.Green;
    target.Green = target8;
    bool target9 = false;
    if (!serialization.TryCustomCopy<bool>(this.Mesons, ref target9, hookCtx, false, context))
      target9 = this.Mesons;
    target.Mesons = target9;
    bool target10 = false;
    if (!serialization.TryCustomCopy<bool>(this.BlockScopes, ref target10, hookCtx, false, context))
      target10 = this.BlockScopes;
    target.BlockScopes = target10;
    SoundSpecifier target11 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.SoundOn, ref target11, hookCtx, true, context))
      target11 = serialization.CreateCopy<SoundSpecifier>(this.SoundOn, hookCtx, context);
    target.SoundOn = target11;
    SoundSpecifier target12 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.SoundOff, ref target12, hookCtx, true, context))
      target12 = serialization.CreateCopy<SoundSpecifier>(this.SoundOff, hookCtx, context);
    target.SoundOff = target12;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref NightVisionItemComponent target,
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
    NightVisionItemComponent target1 = (NightVisionItemComponent) target;
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
    NightVisionItemComponent target1 = (NightVisionItemComponent) target;
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
    NightVisionItemComponent target1 = (NightVisionItemComponent) target;
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
  virtual NightVisionItemComponent Component.Instantiate() => new NightVisionItemComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class NightVisionItemComponent_AutoState : IComponentState
  {
    public EntProtoId? ActionId;
    public NetEntity? Action;
    public NetEntity? User;
    public bool Toggleable;
    public SlotFlags SlotFlags;
    public Dictionary<EntProtoId<SkillDefinitionComponent>, int>? Skills;
    public bool Green;
    public bool Mesons;
    public bool BlockScopes;
    public SoundSpecifier? SoundOn;
    public SoundSpecifier? SoundOff;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class NightVisionItemComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<NightVisionItemComponent, ComponentGetState>(new ComponentEventRefHandler<NightVisionItemComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<NightVisionItemComponent, ComponentHandleState>(new ComponentEventRefHandler<NightVisionItemComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      NightVisionItemComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new NightVisionItemComponent.NightVisionItemComponent_AutoState()
      {
        ActionId = component.ActionId,
        Action = this.GetNetEntity(component.Action),
        User = this.GetNetEntity(component.User),
        Toggleable = component.Toggleable,
        SlotFlags = component.SlotFlags,
        Skills = component.Skills,
        Green = component.Green,
        Mesons = component.Mesons,
        BlockScopes = component.BlockScopes,
        SoundOn = component.SoundOn,
        SoundOff = component.SoundOff
      };
    }

    private void OnHandleState(
      EntityUid uid,
      NightVisionItemComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is NightVisionItemComponent.NightVisionItemComponent_AutoState current))
        return;
      component.ActionId = current.ActionId;
      component.Action = this.EnsureEntity<NightVisionItemComponent>(current.Action, uid);
      component.User = this.EnsureEntity<NightVisionItemComponent>(current.User, uid);
      component.Toggleable = current.Toggleable;
      component.SlotFlags = current.SlotFlags;
      component.Skills = current.Skills == null ? (Dictionary<EntProtoId<SkillDefinitionComponent>, int>) null : new Dictionary<EntProtoId<SkillDefinitionComponent>, int>((IDictionary<EntProtoId<SkillDefinitionComponent>, int>) current.Skills);
      component.Green = current.Green;
      component.Mesons = current.Mesons;
      component.BlockScopes = current.BlockScopes;
      component.SoundOn = current.SoundOn;
      component.SoundOff = current.SoundOff;
    }
  }
}
