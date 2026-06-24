// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Marines.Skills.Pamphlets.UsedSkillPamphletComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Localization;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Utility;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Marines.Skills.Pamphlets;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
public sealed class UsedSkillPamphletComponent : 
  Component,
  ISerializationGenerated<UsedSkillPamphletComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SpriteSpecifier.Rsi? Icon;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public LocId? JobTitle;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool Used;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref UsedSkillPamphletComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (UsedSkillPamphletComponent) target1;
    if (serialization.TryCustomCopy<UsedSkillPamphletComponent>(this, ref target, hookCtx, false, context))
      return;
    SpriteSpecifier.Rsi target2 = (SpriteSpecifier.Rsi) null;
    if (!serialization.TryCustomCopy<SpriteSpecifier.Rsi>(this.Icon, ref target2, hookCtx, false, context))
    {
      if (this.Icon == null)
        target2 = (SpriteSpecifier.Rsi) null;
      else
        serialization.CopyTo<SpriteSpecifier.Rsi>(this.Icon, ref target2, hookCtx, context);
    }
    target.Icon = target2;
    LocId? target3 = new LocId?();
    if (!serialization.TryCustomCopy<LocId?>(this.JobTitle, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<LocId?>(this.JobTitle, hookCtx, context);
    target.JobTitle = target3;
    bool target4 = false;
    if (!serialization.TryCustomCopy<bool>(this.Used, ref target4, hookCtx, false, context))
      target4 = this.Used;
    target.Used = target4;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref UsedSkillPamphletComponent target,
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
    UsedSkillPamphletComponent target1 = (UsedSkillPamphletComponent) target;
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
    UsedSkillPamphletComponent target1 = (UsedSkillPamphletComponent) target;
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
    UsedSkillPamphletComponent target1 = (UsedSkillPamphletComponent) target;
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
  virtual UsedSkillPamphletComponent Component.Instantiate() => new UsedSkillPamphletComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class UsedSkillPamphletComponent_AutoState : IComponentState
  {
    public SpriteSpecifier.Rsi? Icon;
    public LocId? JobTitle;
    public bool Used;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class UsedSkillPamphletComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<UsedSkillPamphletComponent, ComponentGetState>(new ComponentEventRefHandler<UsedSkillPamphletComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<UsedSkillPamphletComponent, ComponentHandleState>(new ComponentEventRefHandler<UsedSkillPamphletComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      UsedSkillPamphletComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new UsedSkillPamphletComponent.UsedSkillPamphletComponent_AutoState()
      {
        Icon = component.Icon,
        JobTitle = component.JobTitle,
        Used = component.Used
      };
    }

    private void OnHandleState(
      EntityUid uid,
      UsedSkillPamphletComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is UsedSkillPamphletComponent.UsedSkillPamphletComponent_AutoState current))
        return;
      component.Icon = current.Icon;
      component.JobTitle = current.JobTitle;
      component.Used = current.Used;
    }
  }
}
