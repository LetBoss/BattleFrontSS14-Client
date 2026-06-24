// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.UniformAccessories.UniformAccessoryComponent
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
namespace Content.Shared._RMC14.UniformAccessories;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
public sealed class UniformAccessoryComponent : 
  Component,
  ISerializationGenerated<UniformAccessoryComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SpriteSpecifier.Rsi? PlayerSprite;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public NetEntity? User;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public string Category;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int Limit;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool Hidden;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool HiddenByJacketRolling;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public string? LayerKey;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool HasIconSprite;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref UniformAccessoryComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (UniformAccessoryComponent) target1;
    if (serialization.TryCustomCopy<UniformAccessoryComponent>(this, ref target, hookCtx, false, context))
      return;
    SpriteSpecifier.Rsi target2 = (SpriteSpecifier.Rsi) null;
    if (!serialization.TryCustomCopy<SpriteSpecifier.Rsi>(this.PlayerSprite, ref target2, hookCtx, false, context))
    {
      if (this.PlayerSprite == null)
        target2 = (SpriteSpecifier.Rsi) null;
      else
        serialization.CopyTo<SpriteSpecifier.Rsi>(this.PlayerSprite, ref target2, hookCtx, context);
    }
    target.PlayerSprite = target2;
    NetEntity? target3 = new NetEntity?();
    if (!serialization.TryCustomCopy<NetEntity?>(this.User, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<NetEntity?>(this.User, hookCtx, context);
    target.User = target3;
    string target4 = (string) null;
    if (this.Category == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.Category, ref target4, hookCtx, false, context))
      target4 = this.Category;
    target.Category = target4;
    int target5 = 0;
    if (!serialization.TryCustomCopy<int>(this.Limit, ref target5, hookCtx, false, context))
      target5 = this.Limit;
    target.Limit = target5;
    bool target6 = false;
    if (!serialization.TryCustomCopy<bool>(this.Hidden, ref target6, hookCtx, false, context))
      target6 = this.Hidden;
    target.Hidden = target6;
    bool target7 = false;
    if (!serialization.TryCustomCopy<bool>(this.HiddenByJacketRolling, ref target7, hookCtx, false, context))
      target7 = this.HiddenByJacketRolling;
    target.HiddenByJacketRolling = target7;
    string target8 = (string) null;
    if (!serialization.TryCustomCopy<string>(this.LayerKey, ref target8, hookCtx, false, context))
      target8 = this.LayerKey;
    target.LayerKey = target8;
    bool target9 = false;
    if (!serialization.TryCustomCopy<bool>(this.HasIconSprite, ref target9, hookCtx, false, context))
      target9 = this.HasIconSprite;
    target.HasIconSprite = target9;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref UniformAccessoryComponent target,
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
    UniformAccessoryComponent target1 = (UniformAccessoryComponent) target;
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
    UniformAccessoryComponent target1 = (UniformAccessoryComponent) target;
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
    UniformAccessoryComponent target1 = (UniformAccessoryComponent) target;
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
  virtual UniformAccessoryComponent Component.Instantiate() => new UniformAccessoryComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class UniformAccessoryComponent_AutoState : IComponentState
  {
    public SpriteSpecifier.Rsi? PlayerSprite;
    public NetEntity? User;
    public string Category;
    public int Limit;
    public bool Hidden;
    public bool HiddenByJacketRolling;
    public string? LayerKey;
    public bool HasIconSprite;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class UniformAccessoryComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<UniformAccessoryComponent, ComponentGetState>(new ComponentEventRefHandler<UniformAccessoryComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<UniformAccessoryComponent, ComponentHandleState>(new ComponentEventRefHandler<UniformAccessoryComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      UniformAccessoryComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new UniformAccessoryComponent.UniformAccessoryComponent_AutoState()
      {
        PlayerSprite = component.PlayerSprite,
        User = component.User,
        Category = component.Category,
        Limit = component.Limit,
        Hidden = component.Hidden,
        HiddenByJacketRolling = component.HiddenByJacketRolling,
        LayerKey = component.LayerKey,
        HasIconSprite = component.HasIconSprite
      };
    }

    private void OnHandleState(
      EntityUid uid,
      UniformAccessoryComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is UniformAccessoryComponent.UniformAccessoryComponent_AutoState current))
        return;
      component.PlayerSprite = current.PlayerSprite;
      component.User = current.User;
      component.Category = current.Category;
      component.Limit = current.Limit;
      component.Hidden = current.Hidden;
      component.HiddenByJacketRolling = current.HiddenByJacketRolling;
      component.LayerKey = current.LayerKey;
      component.HasIconSprite = current.HasIconSprite;
    }
  }
}
