// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Intel.Tech.TechControlConsoleComponent
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
namespace Content.Shared._RMC14.Intel.Tech;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(true, false)]
[Access(new Type[] {typeof (IntelSystem), typeof (TechSystem)})]
public sealed class TechControlConsoleComponent : 
  Component,
  ISerializationGenerated<TechControlConsoleComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public IntelTechTree Tree = new IntelTechTree();
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SpriteSpecifier.Rsi LockedRsi = new SpriteSpecifier.Rsi(new ResPath("_RMC14/Interface/tech_64.rsi"), "marine_locked");
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SpriteSpecifier.Rsi UnlockedRsi = new SpriteSpecifier.Rsi(new ResPath("_RMC14/Interface/tech_64.rsi"), "marine");

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref TechControlConsoleComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (TechControlConsoleComponent) target1;
    if (serialization.TryCustomCopy<TechControlConsoleComponent>(this, ref target, hookCtx, false, context))
      return;
    IntelTechTree target2 = (IntelTechTree) null;
    if (this.Tree == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<IntelTechTree>(this.Tree, ref target2, hookCtx, false, context))
    {
      if (this.Tree == null)
        target2 = (IntelTechTree) null;
      else
        serialization.CopyTo<IntelTechTree>(this.Tree, ref target2, hookCtx, context, true);
    }
    target.Tree = target2;
    SpriteSpecifier.Rsi target3 = (SpriteSpecifier.Rsi) null;
    if (this.LockedRsi == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SpriteSpecifier.Rsi>(this.LockedRsi, ref target3, hookCtx, false, context))
    {
      if (this.LockedRsi == null)
        target3 = (SpriteSpecifier.Rsi) null;
      else
        serialization.CopyTo<SpriteSpecifier.Rsi>(this.LockedRsi, ref target3, hookCtx, context, true);
    }
    target.LockedRsi = target3;
    SpriteSpecifier.Rsi target4 = (SpriteSpecifier.Rsi) null;
    if (this.UnlockedRsi == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SpriteSpecifier.Rsi>(this.UnlockedRsi, ref target4, hookCtx, false, context))
    {
      if (this.UnlockedRsi == null)
        target4 = (SpriteSpecifier.Rsi) null;
      else
        serialization.CopyTo<SpriteSpecifier.Rsi>(this.UnlockedRsi, ref target4, hookCtx, context, true);
    }
    target.UnlockedRsi = target4;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref TechControlConsoleComponent target,
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
    TechControlConsoleComponent target1 = (TechControlConsoleComponent) target;
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
    TechControlConsoleComponent target1 = (TechControlConsoleComponent) target;
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
    TechControlConsoleComponent target1 = (TechControlConsoleComponent) target;
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
  virtual TechControlConsoleComponent Component.Instantiate() => new TechControlConsoleComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class TechControlConsoleComponent_AutoState : IComponentState
  {
    public IntelTechTree Tree;
    public SpriteSpecifier.Rsi LockedRsi;
    public SpriteSpecifier.Rsi UnlockedRsi;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class TechControlConsoleComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<TechControlConsoleComponent, ComponentGetState>(new ComponentEventRefHandler<TechControlConsoleComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<TechControlConsoleComponent, ComponentHandleState>(new ComponentEventRefHandler<TechControlConsoleComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      TechControlConsoleComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new TechControlConsoleComponent.TechControlConsoleComponent_AutoState()
      {
        Tree = component.Tree,
        LockedRsi = component.LockedRsi,
        UnlockedRsi = component.UnlockedRsi
      };
    }

    private void OnHandleState(
      EntityUid uid,
      TechControlConsoleComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is TechControlConsoleComponent.TechControlConsoleComponent_AutoState current))
        return;
      component.Tree = current.Tree;
      component.LockedRsi = current.LockedRsi;
      component.UnlockedRsi = current.UnlockedRsi;
      AfterAutoHandleStateEvent args1 = new AfterAutoHandleStateEvent(args.Current);
      this.EntityManager.EventBus.RaiseComponentEvent<AfterAutoHandleStateEvent, TechControlConsoleComponent>(uid, component, ref args1);
    }
  }
}
