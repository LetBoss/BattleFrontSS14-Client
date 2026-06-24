// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Marines.Skills.Pamphlets.SkillPamphletComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Roles;
using Content.Shared.Whitelist;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Marines.Skills.Pamphlets;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
public sealed class SkillPamphletComponent : 
  Component,
  ISerializationGenerated<SkillPamphletComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public ComponentRegistry AddComps = new ComponentRegistry();
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public Dictionary<EntProtoId<SkillDefinitionComponent>, int> AddSkills = new Dictionary<EntProtoId<SkillDefinitionComponent>, int>();
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public Dictionary<EntProtoId<SkillDefinitionComponent>, int> SkillCap = new Dictionary<EntProtoId<SkillDefinitionComponent>, int>();
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SpriteSpecifier.Rsi? GiveIcon;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SpriteSpecifier.Rsi? GiveMapBlip;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public LocId? GiveJobTitle;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public LocId? GivePrefix;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool IsAppendPrefix;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool BypassLimit;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public List<SkillPamphletComponent.PamphletWhitelist> Whitelists = new List<SkillPamphletComponent.PamphletWhitelist>();
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public List<SkillPamphletComponent.JobWhitelist> JobWhitelists = new List<SkillPamphletComponent.JobWhitelist>();
  public bool GaveSkill;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool BypassSkill;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref SkillPamphletComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (SkillPamphletComponent) target1;
    if (serialization.TryCustomCopy<SkillPamphletComponent>(this, ref target, hookCtx, false, context))
      return;
    ComponentRegistry target2 = (ComponentRegistry) null;
    if (this.AddComps == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<ComponentRegistry>(this.AddComps, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<ComponentRegistry>(this.AddComps, hookCtx, context);
    target.AddComps = target2;
    Dictionary<EntProtoId<SkillDefinitionComponent>, int> target3 = (Dictionary<EntProtoId<SkillDefinitionComponent>, int>) null;
    if (this.AddSkills == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<Dictionary<EntProtoId<SkillDefinitionComponent>, int>>(this.AddSkills, ref target3, hookCtx, true, context))
      target3 = serialization.CreateCopy<Dictionary<EntProtoId<SkillDefinitionComponent>, int>>(this.AddSkills, hookCtx, context);
    target.AddSkills = target3;
    Dictionary<EntProtoId<SkillDefinitionComponent>, int> target4 = (Dictionary<EntProtoId<SkillDefinitionComponent>, int>) null;
    if (this.SkillCap == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<Dictionary<EntProtoId<SkillDefinitionComponent>, int>>(this.SkillCap, ref target4, hookCtx, true, context))
      target4 = serialization.CreateCopy<Dictionary<EntProtoId<SkillDefinitionComponent>, int>>(this.SkillCap, hookCtx, context);
    target.SkillCap = target4;
    SpriteSpecifier.Rsi target5 = (SpriteSpecifier.Rsi) null;
    if (!serialization.TryCustomCopy<SpriteSpecifier.Rsi>(this.GiveIcon, ref target5, hookCtx, false, context))
    {
      if (this.GiveIcon == null)
        target5 = (SpriteSpecifier.Rsi) null;
      else
        serialization.CopyTo<SpriteSpecifier.Rsi>(this.GiveIcon, ref target5, hookCtx, context);
    }
    target.GiveIcon = target5;
    SpriteSpecifier.Rsi target6 = (SpriteSpecifier.Rsi) null;
    if (!serialization.TryCustomCopy<SpriteSpecifier.Rsi>(this.GiveMapBlip, ref target6, hookCtx, false, context))
    {
      if (this.GiveMapBlip == null)
        target6 = (SpriteSpecifier.Rsi) null;
      else
        serialization.CopyTo<SpriteSpecifier.Rsi>(this.GiveMapBlip, ref target6, hookCtx, context);
    }
    target.GiveMapBlip = target6;
    LocId? target7 = new LocId?();
    if (!serialization.TryCustomCopy<LocId?>(this.GiveJobTitle, ref target7, hookCtx, false, context))
      target7 = serialization.CreateCopy<LocId?>(this.GiveJobTitle, hookCtx, context);
    target.GiveJobTitle = target7;
    LocId? target8 = new LocId?();
    if (!serialization.TryCustomCopy<LocId?>(this.GivePrefix, ref target8, hookCtx, false, context))
      target8 = serialization.CreateCopy<LocId?>(this.GivePrefix, hookCtx, context);
    target.GivePrefix = target8;
    bool target9 = false;
    if (!serialization.TryCustomCopy<bool>(this.IsAppendPrefix, ref target9, hookCtx, false, context))
      target9 = this.IsAppendPrefix;
    target.IsAppendPrefix = target9;
    bool target10 = false;
    if (!serialization.TryCustomCopy<bool>(this.BypassLimit, ref target10, hookCtx, false, context))
      target10 = this.BypassLimit;
    target.BypassLimit = target10;
    List<SkillPamphletComponent.PamphletWhitelist> target11 = (List<SkillPamphletComponent.PamphletWhitelist>) null;
    if (this.Whitelists == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<List<SkillPamphletComponent.PamphletWhitelist>>(this.Whitelists, ref target11, hookCtx, true, context))
      target11 = serialization.CreateCopy<List<SkillPamphletComponent.PamphletWhitelist>>(this.Whitelists, hookCtx, context);
    target.Whitelists = target11;
    List<SkillPamphletComponent.JobWhitelist> target12 = (List<SkillPamphletComponent.JobWhitelist>) null;
    if (this.JobWhitelists == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<List<SkillPamphletComponent.JobWhitelist>>(this.JobWhitelists, ref target12, hookCtx, true, context))
      target12 = serialization.CreateCopy<List<SkillPamphletComponent.JobWhitelist>>(this.JobWhitelists, hookCtx, context);
    target.JobWhitelists = target12;
    bool target13 = false;
    if (!serialization.TryCustomCopy<bool>(this.BypassSkill, ref target13, hookCtx, false, context))
      target13 = this.BypassSkill;
    target.BypassSkill = target13;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref SkillPamphletComponent target,
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
    SkillPamphletComponent target1 = (SkillPamphletComponent) target;
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
    SkillPamphletComponent target1 = (SkillPamphletComponent) target;
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
    SkillPamphletComponent target1 = (SkillPamphletComponent) target;
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
  virtual SkillPamphletComponent Component.Instantiate() => new SkillPamphletComponent();

  [DataRecord]
  [NetSerializable]
  [Serializable]
  public readonly record struct PamphletWhitelist(string Popup, EntityWhitelist Restrictions);

  [DataRecord]
  [NetSerializable]
  [Serializable]
  public readonly record struct JobWhitelist(LocId Popup, ProtoId<JobPrototype> JobProto);

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class SkillPamphletComponent_AutoState : IComponentState
  {
    public Dictionary<EntProtoId<SkillDefinitionComponent>, int> AddSkills;
    public Dictionary<EntProtoId<SkillDefinitionComponent>, int> SkillCap;
    public SpriteSpecifier.Rsi? GiveIcon;
    public SpriteSpecifier.Rsi? GiveMapBlip;
    public LocId? GiveJobTitle;
    public LocId? GivePrefix;
    public bool IsAppendPrefix;
    public bool BypassLimit;
    public List<SkillPamphletComponent.PamphletWhitelist> Whitelists;
    public List<SkillPamphletComponent.JobWhitelist> JobWhitelists;
    public bool BypassSkill;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class SkillPamphletComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<SkillPamphletComponent, ComponentGetState>(new ComponentEventRefHandler<SkillPamphletComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<SkillPamphletComponent, ComponentHandleState>(new ComponentEventRefHandler<SkillPamphletComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      SkillPamphletComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new SkillPamphletComponent.SkillPamphletComponent_AutoState()
      {
        AddSkills = component.AddSkills,
        SkillCap = component.SkillCap,
        GiveIcon = component.GiveIcon,
        GiveMapBlip = component.GiveMapBlip,
        GiveJobTitle = component.GiveJobTitle,
        GivePrefix = component.GivePrefix,
        IsAppendPrefix = component.IsAppendPrefix,
        BypassLimit = component.BypassLimit,
        Whitelists = component.Whitelists,
        JobWhitelists = component.JobWhitelists,
        BypassSkill = component.BypassSkill
      };
    }

    private void OnHandleState(
      EntityUid uid,
      SkillPamphletComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is SkillPamphletComponent.SkillPamphletComponent_AutoState current))
        return;
      component.AddSkills = current.AddSkills == null ? (Dictionary<EntProtoId<SkillDefinitionComponent>, int>) null : new Dictionary<EntProtoId<SkillDefinitionComponent>, int>((IDictionary<EntProtoId<SkillDefinitionComponent>, int>) current.AddSkills);
      component.SkillCap = current.SkillCap == null ? (Dictionary<EntProtoId<SkillDefinitionComponent>, int>) null : new Dictionary<EntProtoId<SkillDefinitionComponent>, int>((IDictionary<EntProtoId<SkillDefinitionComponent>, int>) current.SkillCap);
      component.GiveIcon = current.GiveIcon;
      component.GiveMapBlip = current.GiveMapBlip;
      component.GiveJobTitle = current.GiveJobTitle;
      component.GivePrefix = current.GivePrefix;
      component.IsAppendPrefix = current.IsAppendPrefix;
      component.BypassLimit = current.BypassLimit;
      component.Whitelists = current.Whitelists == null ? (List<SkillPamphletComponent.PamphletWhitelist>) null : new List<SkillPamphletComponent.PamphletWhitelist>((IEnumerable<SkillPamphletComponent.PamphletWhitelist>) current.Whitelists);
      component.JobWhitelists = current.JobWhitelists == null ? (List<SkillPamphletComponent.JobWhitelist>) null : new List<SkillPamphletComponent.JobWhitelist>((IEnumerable<SkillPamphletComponent.JobWhitelist>) current.JobWhitelists);
      component.BypassSkill = current.BypassSkill;
    }
  }
}
