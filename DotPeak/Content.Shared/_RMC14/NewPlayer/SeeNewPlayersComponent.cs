// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.NewPlayer.SeeNewPlayersComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

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
namespace Content.Shared._RMC14.NewPlayer;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
public sealed class SeeNewPlayersComponent : 
  Component,
  ISerializationGenerated<SeeNewPlayersComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SpriteSpecifier.Rsi OneLabel = new SpriteSpecifier.Rsi(new ResPath("_RMC14/Interface/new_player.rsi"), "new_player_marker_1");
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SpriteSpecifier.Rsi TwoLabel = new SpriteSpecifier.Rsi(new ResPath("_RMC14/Interface/new_player.rsi"), "new_player_marker_2");
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SpriteSpecifier.Rsi ThreeLabel = new SpriteSpecifier.Rsi(new ResPath("_RMC14/Interface/new_player.rsi"), "new_player_marker_3");
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SpriteSpecifier.Rsi FourLabel = new SpriteSpecifier.Rsi(new ResPath("_RMC14/Interface/new_player.rsi"), "new_player_marker_4");

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref SeeNewPlayersComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (SeeNewPlayersComponent) target1;
    if (serialization.TryCustomCopy<SeeNewPlayersComponent>(this, ref target, hookCtx, false, context))
      return;
    SpriteSpecifier.Rsi target2 = (SpriteSpecifier.Rsi) null;
    if (this.OneLabel == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SpriteSpecifier.Rsi>(this.OneLabel, ref target2, hookCtx, false, context))
    {
      if (this.OneLabel == null)
        target2 = (SpriteSpecifier.Rsi) null;
      else
        serialization.CopyTo<SpriteSpecifier.Rsi>(this.OneLabel, ref target2, hookCtx, context, true);
    }
    target.OneLabel = target2;
    SpriteSpecifier.Rsi target3 = (SpriteSpecifier.Rsi) null;
    if (this.TwoLabel == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SpriteSpecifier.Rsi>(this.TwoLabel, ref target3, hookCtx, false, context))
    {
      if (this.TwoLabel == null)
        target3 = (SpriteSpecifier.Rsi) null;
      else
        serialization.CopyTo<SpriteSpecifier.Rsi>(this.TwoLabel, ref target3, hookCtx, context, true);
    }
    target.TwoLabel = target3;
    SpriteSpecifier.Rsi target4 = (SpriteSpecifier.Rsi) null;
    if (this.ThreeLabel == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SpriteSpecifier.Rsi>(this.ThreeLabel, ref target4, hookCtx, false, context))
    {
      if (this.ThreeLabel == null)
        target4 = (SpriteSpecifier.Rsi) null;
      else
        serialization.CopyTo<SpriteSpecifier.Rsi>(this.ThreeLabel, ref target4, hookCtx, context, true);
    }
    target.ThreeLabel = target4;
    SpriteSpecifier.Rsi target5 = (SpriteSpecifier.Rsi) null;
    if (this.FourLabel == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SpriteSpecifier.Rsi>(this.FourLabel, ref target5, hookCtx, false, context))
    {
      if (this.FourLabel == null)
        target5 = (SpriteSpecifier.Rsi) null;
      else
        serialization.CopyTo<SpriteSpecifier.Rsi>(this.FourLabel, ref target5, hookCtx, context, true);
    }
    target.FourLabel = target5;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref SeeNewPlayersComponent target,
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
    SeeNewPlayersComponent target1 = (SeeNewPlayersComponent) target;
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
    SeeNewPlayersComponent target1 = (SeeNewPlayersComponent) target;
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
    SeeNewPlayersComponent target1 = (SeeNewPlayersComponent) target;
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
  virtual SeeNewPlayersComponent Component.Instantiate() => new SeeNewPlayersComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class SeeNewPlayersComponent_AutoState : IComponentState
  {
    public SpriteSpecifier.Rsi OneLabel;
    public SpriteSpecifier.Rsi TwoLabel;
    public SpriteSpecifier.Rsi ThreeLabel;
    public SpriteSpecifier.Rsi FourLabel;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class SeeNewPlayersComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<SeeNewPlayersComponent, ComponentGetState>(new ComponentEventRefHandler<SeeNewPlayersComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<SeeNewPlayersComponent, ComponentHandleState>(new ComponentEventRefHandler<SeeNewPlayersComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      SeeNewPlayersComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new SeeNewPlayersComponent.SeeNewPlayersComponent_AutoState()
      {
        OneLabel = component.OneLabel,
        TwoLabel = component.TwoLabel,
        ThreeLabel = component.ThreeLabel,
        FourLabel = component.FourLabel
      };
    }

    private void OnHandleState(
      EntityUid uid,
      SeeNewPlayersComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is SeeNewPlayersComponent.SeeNewPlayersComponent_AutoState current))
        return;
      component.OneLabel = current.OneLabel;
      component.TwoLabel = current.TwoLabel;
      component.ThreeLabel = current.ThreeLabel;
      component.FourLabel = current.FourLabel;
    }
  }
}
