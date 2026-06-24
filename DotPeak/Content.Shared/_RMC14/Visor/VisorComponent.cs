// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Visor.VisorComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Marines.Skills;
using Content.Shared.Inventory;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Utility;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Visor;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (VisorSystem)})]
public sealed class VisorComponent : 
  Component,
  ISerializationGenerated<VisorComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SpriteSpecifier.Rsi? ToggledSprite;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SlotFlags Slot = SlotFlags.HEAD;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SkillWhitelist? SkillsRequired;
  [DataField(null, false, 1, true, false, null)]
  [AutoNetworkedField]
  public SpriteSpecifier.Rsi OnIcon;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref VisorComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (VisorComponent) target1;
    if (serialization.TryCustomCopy<VisorComponent>(this, ref target, hookCtx, false, context))
      return;
    SpriteSpecifier.Rsi target2 = (SpriteSpecifier.Rsi) null;
    if (!serialization.TryCustomCopy<SpriteSpecifier.Rsi>(this.ToggledSprite, ref target2, hookCtx, false, context))
    {
      if (this.ToggledSprite == null)
        target2 = (SpriteSpecifier.Rsi) null;
      else
        serialization.CopyTo<SpriteSpecifier.Rsi>(this.ToggledSprite, ref target2, hookCtx, context);
    }
    target.ToggledSprite = target2;
    SlotFlags target3 = SlotFlags.NONE;
    if (!serialization.TryCustomCopy<SlotFlags>(this.Slot, ref target3, hookCtx, false, context))
      target3 = this.Slot;
    target.Slot = target3;
    SkillWhitelist target4 = (SkillWhitelist) null;
    if (!serialization.TryCustomCopy<SkillWhitelist>(this.SkillsRequired, ref target4, hookCtx, false, context))
    {
      if (this.SkillsRequired == null)
        target4 = (SkillWhitelist) null;
      else
        serialization.CopyTo<SkillWhitelist>(this.SkillsRequired, ref target4, hookCtx, context);
    }
    target.SkillsRequired = target4;
    SpriteSpecifier.Rsi target5 = (SpriteSpecifier.Rsi) null;
    if (this.OnIcon == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SpriteSpecifier.Rsi>(this.OnIcon, ref target5, hookCtx, false, context))
    {
      if (this.OnIcon == null)
        target5 = (SpriteSpecifier.Rsi) null;
      else
        serialization.CopyTo<SpriteSpecifier.Rsi>(this.OnIcon, ref target5, hookCtx, context, true);
    }
    target.OnIcon = target5;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref VisorComponent target,
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
    VisorComponent target1 = (VisorComponent) target;
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
    VisorComponent target1 = (VisorComponent) target;
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
    VisorComponent target1 = (VisorComponent) target;
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
  virtual VisorComponent Component.Instantiate() => new VisorComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class VisorComponent_AutoState : IComponentState
  {
    public SpriteSpecifier.Rsi? ToggledSprite;
    public SlotFlags Slot;
    public SkillWhitelist? SkillsRequired;
    public SpriteSpecifier.Rsi OnIcon;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class VisorComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<VisorComponent, ComponentGetState>(new ComponentEventRefHandler<VisorComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<VisorComponent, ComponentHandleState>(new ComponentEventRefHandler<VisorComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(EntityUid uid, VisorComponent component, ref ComponentGetState args)
    {
      args.State = (IComponentState) new VisorComponent.VisorComponent_AutoState()
      {
        ToggledSprite = component.ToggledSprite,
        Slot = component.Slot,
        SkillsRequired = component.SkillsRequired,
        OnIcon = component.OnIcon
      };
    }

    private void OnHandleState(
      EntityUid uid,
      VisorComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is VisorComponent.VisorComponent_AutoState current))
        return;
      component.ToggledSprite = current.ToggledSprite;
      component.Slot = current.Slot;
      component.SkillsRequired = current.SkillsRequired;
      component.OnIcon = current.OnIcon;
    }
  }
}
