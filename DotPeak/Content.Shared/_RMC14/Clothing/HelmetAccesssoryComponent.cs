// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Clothing.HelmetAccessoryComponent
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
namespace Content.Shared._RMC14.Clothing;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (HelmetAccessoriesSystem)})]
public sealed class HelmetAccessoryComponent : 
  Component,
  ISerializationGenerated<HelmetAccessoryComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SpriteSpecifier.Rsi Rsi;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SpriteSpecifier.Rsi? HatRsi;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SpriteSpecifier.Rsi? ToggledRsi;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SpriteSpecifier.Rsi? HatToggledRsi;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref HelmetAccessoryComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (HelmetAccessoryComponent) target1;
    if (serialization.TryCustomCopy<HelmetAccessoryComponent>(this, ref target, hookCtx, false, context))
      return;
    SpriteSpecifier.Rsi target2 = (SpriteSpecifier.Rsi) null;
    if (this.Rsi == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SpriteSpecifier.Rsi>(this.Rsi, ref target2, hookCtx, false, context))
    {
      if (this.Rsi == null)
        target2 = (SpriteSpecifier.Rsi) null;
      else
        serialization.CopyTo<SpriteSpecifier.Rsi>(this.Rsi, ref target2, hookCtx, context, true);
    }
    target.Rsi = target2;
    SpriteSpecifier.Rsi target3 = (SpriteSpecifier.Rsi) null;
    if (!serialization.TryCustomCopy<SpriteSpecifier.Rsi>(this.HatRsi, ref target3, hookCtx, false, context))
    {
      if (this.HatRsi == null)
        target3 = (SpriteSpecifier.Rsi) null;
      else
        serialization.CopyTo<SpriteSpecifier.Rsi>(this.HatRsi, ref target3, hookCtx, context);
    }
    target.HatRsi = target3;
    SpriteSpecifier.Rsi target4 = (SpriteSpecifier.Rsi) null;
    if (!serialization.TryCustomCopy<SpriteSpecifier.Rsi>(this.ToggledRsi, ref target4, hookCtx, false, context))
    {
      if (this.ToggledRsi == null)
        target4 = (SpriteSpecifier.Rsi) null;
      else
        serialization.CopyTo<SpriteSpecifier.Rsi>(this.ToggledRsi, ref target4, hookCtx, context);
    }
    target.ToggledRsi = target4;
    SpriteSpecifier.Rsi target5 = (SpriteSpecifier.Rsi) null;
    if (!serialization.TryCustomCopy<SpriteSpecifier.Rsi>(this.HatToggledRsi, ref target5, hookCtx, false, context))
    {
      if (this.HatToggledRsi == null)
        target5 = (SpriteSpecifier.Rsi) null;
      else
        serialization.CopyTo<SpriteSpecifier.Rsi>(this.HatToggledRsi, ref target5, hookCtx, context);
    }
    target.HatToggledRsi = target5;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref HelmetAccessoryComponent target,
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
    HelmetAccessoryComponent target1 = (HelmetAccessoryComponent) target;
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
    HelmetAccessoryComponent target1 = (HelmetAccessoryComponent) target;
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
    HelmetAccessoryComponent target1 = (HelmetAccessoryComponent) target;
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
  virtual HelmetAccessoryComponent Component.Instantiate() => new HelmetAccessoryComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class HelmetAccessoryComponent_AutoState : IComponentState
  {
    public SpriteSpecifier.Rsi Rsi;
    public SpriteSpecifier.Rsi? HatRsi;
    public SpriteSpecifier.Rsi? ToggledRsi;
    public SpriteSpecifier.Rsi? HatToggledRsi;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class HelmetAccessoryComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<HelmetAccessoryComponent, ComponentGetState>(new ComponentEventRefHandler<HelmetAccessoryComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<HelmetAccessoryComponent, ComponentHandleState>(new ComponentEventRefHandler<HelmetAccessoryComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      HelmetAccessoryComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new HelmetAccessoryComponent.HelmetAccessoryComponent_AutoState()
      {
        Rsi = component.Rsi,
        HatRsi = component.HatRsi,
        ToggledRsi = component.ToggledRsi,
        HatToggledRsi = component.HatToggledRsi
      };
    }

    private void OnHandleState(
      EntityUid uid,
      HelmetAccessoryComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is HelmetAccessoryComponent.HelmetAccessoryComponent_AutoState current))
        return;
      component.Rsi = current.Rsi;
      component.HatRsi = current.HatRsi;
      component.ToggledRsi = current.ToggledRsi;
      component.HatToggledRsi = current.HatToggledRsi;
    }
  }
}
