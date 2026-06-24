// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Dropship.Utility.Components.DropshipAttachedSpriteComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Dropship.Utility.Systems;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Utility;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Dropship.Utility.Components;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (DropshipUtilitySystem)})]
public sealed class DropshipAttachedSpriteComponent : 
  Component,
  ISerializationGenerated<DropshipAttachedSpriteComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SpriteSpecifier.Rsi? Sprite;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SpriteSpecifier.Rsi? WeaponSlotSprite;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref DropshipAttachedSpriteComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (DropshipAttachedSpriteComponent) target1;
    if (serialization.TryCustomCopy<DropshipAttachedSpriteComponent>(this, ref target, hookCtx, false, context))
      return;
    SpriteSpecifier.Rsi target2 = (SpriteSpecifier.Rsi) null;
    if (!serialization.TryCustomCopy<SpriteSpecifier.Rsi>(this.Sprite, ref target2, hookCtx, false, context))
    {
      if (this.Sprite == null)
        target2 = (SpriteSpecifier.Rsi) null;
      else
        serialization.CopyTo<SpriteSpecifier.Rsi>(this.Sprite, ref target2, hookCtx, context);
    }
    target.Sprite = target2;
    SpriteSpecifier.Rsi target3 = (SpriteSpecifier.Rsi) null;
    if (!serialization.TryCustomCopy<SpriteSpecifier.Rsi>(this.WeaponSlotSprite, ref target3, hookCtx, false, context))
    {
      if (this.WeaponSlotSprite == null)
        target3 = (SpriteSpecifier.Rsi) null;
      else
        serialization.CopyTo<SpriteSpecifier.Rsi>(this.WeaponSlotSprite, ref target3, hookCtx, context);
    }
    target.WeaponSlotSprite = target3;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref DropshipAttachedSpriteComponent target,
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
    DropshipAttachedSpriteComponent target1 = (DropshipAttachedSpriteComponent) target;
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
    DropshipAttachedSpriteComponent target1 = (DropshipAttachedSpriteComponent) target;
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
    DropshipAttachedSpriteComponent target1 = (DropshipAttachedSpriteComponent) target;
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
  virtual DropshipAttachedSpriteComponent Component.Instantiate()
  {
    return new DropshipAttachedSpriteComponent();
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class DropshipAttachedSpriteComponent_AutoState : IComponentState
  {
    public SpriteSpecifier.Rsi? Sprite;
    public SpriteSpecifier.Rsi? WeaponSlotSprite;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class DropshipAttachedSpriteComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<DropshipAttachedSpriteComponent, ComponentGetState>(new ComponentEventRefHandler<DropshipAttachedSpriteComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<DropshipAttachedSpriteComponent, ComponentHandleState>(new ComponentEventRefHandler<DropshipAttachedSpriteComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      DropshipAttachedSpriteComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new DropshipAttachedSpriteComponent.DropshipAttachedSpriteComponent_AutoState()
      {
        Sprite = component.Sprite,
        WeaponSlotSprite = component.WeaponSlotSprite
      };
    }

    private void OnHandleState(
      EntityUid uid,
      DropshipAttachedSpriteComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is DropshipAttachedSpriteComponent.DropshipAttachedSpriteComponent_AutoState current))
        return;
      component.Sprite = current.Sprite;
      component.WeaponSlotSprite = current.WeaponSlotSprite;
    }
  }
}
