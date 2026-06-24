// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Dropship.Weapon.DropshipWeaponComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Marines.Skills;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;
using Robust.Shared.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Dropship.Weapon;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[AutoGenerateComponentPause]
[Access(new Type[] {typeof (SharedDropshipWeaponSystem)})]
public sealed class DropshipWeaponComponent : 
  Component,
  ISerializationGenerated<DropshipWeaponComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, true, false, null)]
  [AutoNetworkedField]
  public string Abbreviation = string.Empty;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan FireDelay = TimeSpan.FromSeconds(2L);
  [DataField(null, false, 1, false, false, typeof (TimeOffsetSerializer))]
  [AutoNetworkedField]
  [AutoPausedField]
  public TimeSpan? NextFireAt;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool FireInTransport;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SkillWhitelist? Skills;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SpriteSpecifier.Rsi? WeaponAttachedSprite;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SpriteSpecifier.Rsi? AmmoEmptyAttachedSprite;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SpriteSpecifier.Rsi? AmmoAttachedSprite;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public List<int> AmmoSpriteThresholds = new List<int>();

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref DropshipWeaponComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (DropshipWeaponComponent) target1;
    if (serialization.TryCustomCopy<DropshipWeaponComponent>(this, ref target, hookCtx, false, context))
      return;
    string target2 = (string) null;
    if (this.Abbreviation == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.Abbreviation, ref target2, hookCtx, false, context))
      target2 = this.Abbreviation;
    target.Abbreviation = target2;
    TimeSpan target3 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.FireDelay, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<TimeSpan>(this.FireDelay, hookCtx, context);
    target.FireDelay = target3;
    TimeSpan? target4 = new TimeSpan?();
    if (!serialization.TryCustomCopy<TimeSpan?>(this.NextFireAt, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<TimeSpan?>(this.NextFireAt, hookCtx, context);
    target.NextFireAt = target4;
    bool target5 = false;
    if (!serialization.TryCustomCopy<bool>(this.FireInTransport, ref target5, hookCtx, false, context))
      target5 = this.FireInTransport;
    target.FireInTransport = target5;
    SkillWhitelist target6 = (SkillWhitelist) null;
    if (!serialization.TryCustomCopy<SkillWhitelist>(this.Skills, ref target6, hookCtx, false, context))
    {
      if (this.Skills == null)
        target6 = (SkillWhitelist) null;
      else
        serialization.CopyTo<SkillWhitelist>(this.Skills, ref target6, hookCtx, context);
    }
    target.Skills = target6;
    SpriteSpecifier.Rsi target7 = (SpriteSpecifier.Rsi) null;
    if (!serialization.TryCustomCopy<SpriteSpecifier.Rsi>(this.WeaponAttachedSprite, ref target7, hookCtx, false, context))
    {
      if (this.WeaponAttachedSprite == null)
        target7 = (SpriteSpecifier.Rsi) null;
      else
        serialization.CopyTo<SpriteSpecifier.Rsi>(this.WeaponAttachedSprite, ref target7, hookCtx, context);
    }
    target.WeaponAttachedSprite = target7;
    SpriteSpecifier.Rsi target8 = (SpriteSpecifier.Rsi) null;
    if (!serialization.TryCustomCopy<SpriteSpecifier.Rsi>(this.AmmoEmptyAttachedSprite, ref target8, hookCtx, false, context))
    {
      if (this.AmmoEmptyAttachedSprite == null)
        target8 = (SpriteSpecifier.Rsi) null;
      else
        serialization.CopyTo<SpriteSpecifier.Rsi>(this.AmmoEmptyAttachedSprite, ref target8, hookCtx, context);
    }
    target.AmmoEmptyAttachedSprite = target8;
    SpriteSpecifier.Rsi target9 = (SpriteSpecifier.Rsi) null;
    if (!serialization.TryCustomCopy<SpriteSpecifier.Rsi>(this.AmmoAttachedSprite, ref target9, hookCtx, false, context))
    {
      if (this.AmmoAttachedSprite == null)
        target9 = (SpriteSpecifier.Rsi) null;
      else
        serialization.CopyTo<SpriteSpecifier.Rsi>(this.AmmoAttachedSprite, ref target9, hookCtx, context);
    }
    target.AmmoAttachedSprite = target9;
    List<int> target10 = (List<int>) null;
    if (this.AmmoSpriteThresholds == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<List<int>>(this.AmmoSpriteThresholds, ref target10, hookCtx, true, context))
      target10 = serialization.CreateCopy<List<int>>(this.AmmoSpriteThresholds, hookCtx, context);
    target.AmmoSpriteThresholds = target10;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref DropshipWeaponComponent target,
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
    DropshipWeaponComponent target1 = (DropshipWeaponComponent) target;
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
    DropshipWeaponComponent target1 = (DropshipWeaponComponent) target;
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
    DropshipWeaponComponent target1 = (DropshipWeaponComponent) target;
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
  virtual DropshipWeaponComponent Component.Instantiate() => new DropshipWeaponComponent();

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class DropshipWeaponComponent_AutoPauseSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<DropshipWeaponComponent, EntityUnpausedEvent>(new ComponentEventRefHandler<DropshipWeaponComponent, EntityUnpausedEvent>(this.OnEntityUnpaused));
    }

    private void OnEntityUnpaused(
      EntityUid uid,
      #nullable disable
      DropshipWeaponComponent component,
      ref EntityUnpausedEvent args)
    {
      if (component.NextFireAt.HasValue)
        component.NextFireAt = new TimeSpan?(component.NextFireAt.Value + args.PausedTime);
      this.Dirty(uid, (IComponent) component);
    }
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class DropshipWeaponComponent_AutoState : IComponentState
  {
    public 
    #nullable enable
    string Abbreviation;
    public TimeSpan FireDelay;
    public TimeSpan? NextFireAt;
    public bool FireInTransport;
    public SkillWhitelist? Skills;
    public SpriteSpecifier.Rsi? WeaponAttachedSprite;
    public SpriteSpecifier.Rsi? AmmoEmptyAttachedSprite;
    public SpriteSpecifier.Rsi? AmmoAttachedSprite;
    public List<int> AmmoSpriteThresholds;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class DropshipWeaponComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<DropshipWeaponComponent, ComponentGetState>(new ComponentEventRefHandler<DropshipWeaponComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<DropshipWeaponComponent, ComponentHandleState>(new ComponentEventRefHandler<DropshipWeaponComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      DropshipWeaponComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new DropshipWeaponComponent.DropshipWeaponComponent_AutoState()
      {
        Abbreviation = component.Abbreviation,
        FireDelay = component.FireDelay,
        NextFireAt = component.NextFireAt,
        FireInTransport = component.FireInTransport,
        Skills = component.Skills,
        WeaponAttachedSprite = component.WeaponAttachedSprite,
        AmmoEmptyAttachedSprite = component.AmmoEmptyAttachedSprite,
        AmmoAttachedSprite = component.AmmoAttachedSprite,
        AmmoSpriteThresholds = component.AmmoSpriteThresholds
      };
    }

    private void OnHandleState(
      EntityUid uid,
      DropshipWeaponComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is DropshipWeaponComponent.DropshipWeaponComponent_AutoState current))
        return;
      component.Abbreviation = current.Abbreviation;
      component.FireDelay = current.FireDelay;
      component.NextFireAt = current.NextFireAt;
      component.FireInTransport = current.FireInTransport;
      component.Skills = current.Skills;
      component.WeaponAttachedSprite = current.WeaponAttachedSprite;
      component.AmmoEmptyAttachedSprite = current.AmmoEmptyAttachedSprite;
      component.AmmoAttachedSprite = current.AmmoAttachedSprite;
      component.AmmoSpriteThresholds = current.AmmoSpriteThresholds == null ? (List<int>) null : new List<int>((IEnumerable<int>) current.AmmoSpriteThresholds);
    }
  }
}
