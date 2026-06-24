// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Marines.Squads.SquadArmorComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

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
namespace Content.Shared._RMC14.Marines.Squads;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (SquadSystem)})]
public sealed class SquadArmorComponent : 
  Component,
  ISerializationGenerated<SquadArmorComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, true, false, null)]
  [AutoNetworkedField]
  public SquadArmorLayers Layer;
  [DataField(null, false, 1, true, false, null)]
  [AutoNetworkedField]
  public SlotFlags Slot;
  [DataField(null, false, 1, true, false, null)]
  [AutoNetworkedField]
  public SpriteSpecifier.Rsi Rsi;
  [DataField(null, false, 1, true, false, null)]
  [AutoNetworkedField]
  public SpriteSpecifier.Rsi LeaderRsi;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref SquadArmorComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (SquadArmorComponent) target1;
    if (serialization.TryCustomCopy<SquadArmorComponent>(this, ref target, hookCtx, false, context))
      return;
    SquadArmorLayers target2 = SquadArmorLayers.Helmet;
    if (!serialization.TryCustomCopy<SquadArmorLayers>(this.Layer, ref target2, hookCtx, false, context))
      target2 = this.Layer;
    target.Layer = target2;
    SlotFlags target3 = SlotFlags.NONE;
    if (!serialization.TryCustomCopy<SlotFlags>(this.Slot, ref target3, hookCtx, false, context))
      target3 = this.Slot;
    target.Slot = target3;
    SpriteSpecifier.Rsi target4 = (SpriteSpecifier.Rsi) null;
    if (this.Rsi == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SpriteSpecifier.Rsi>(this.Rsi, ref target4, hookCtx, false, context))
    {
      if (this.Rsi == null)
        target4 = (SpriteSpecifier.Rsi) null;
      else
        serialization.CopyTo<SpriteSpecifier.Rsi>(this.Rsi, ref target4, hookCtx, context, true);
    }
    target.Rsi = target4;
    SpriteSpecifier.Rsi target5 = (SpriteSpecifier.Rsi) null;
    if (this.LeaderRsi == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SpriteSpecifier.Rsi>(this.LeaderRsi, ref target5, hookCtx, false, context))
    {
      if (this.LeaderRsi == null)
        target5 = (SpriteSpecifier.Rsi) null;
      else
        serialization.CopyTo<SpriteSpecifier.Rsi>(this.LeaderRsi, ref target5, hookCtx, context, true);
    }
    target.LeaderRsi = target5;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref SquadArmorComponent target,
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
    SquadArmorComponent target1 = (SquadArmorComponent) target;
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
    SquadArmorComponent target1 = (SquadArmorComponent) target;
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
    SquadArmorComponent target1 = (SquadArmorComponent) target;
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
  virtual SquadArmorComponent Component.Instantiate() => new SquadArmorComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class SquadArmorComponent_AutoState : IComponentState
  {
    public SquadArmorLayers Layer;
    public SlotFlags Slot;
    public SpriteSpecifier.Rsi Rsi;
    public SpriteSpecifier.Rsi LeaderRsi;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class SquadArmorComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<SquadArmorComponent, ComponentGetState>(new ComponentEventRefHandler<SquadArmorComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<SquadArmorComponent, ComponentHandleState>(new ComponentEventRefHandler<SquadArmorComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      SquadArmorComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new SquadArmorComponent.SquadArmorComponent_AutoState()
      {
        Layer = component.Layer,
        Slot = component.Slot,
        Rsi = component.Rsi,
        LeaderRsi = component.LeaderRsi
      };
    }

    private void OnHandleState(
      EntityUid uid,
      SquadArmorComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is SquadArmorComponent.SquadArmorComponent_AutoState current))
        return;
      component.Layer = current.Layer;
      component.Slot = current.Slot;
      component.Rsi = current.Rsi;
      component.LeaderRsi = current.LeaderRsi;
    }
  }
}
