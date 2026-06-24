// Decompiled with JetBrains decompiler
// Type: Content.Shared.PAI.PAIComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.ViewVariables;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.PAI;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
public sealed class PAIComponent : 
  Component,
  ISerializationGenerated<PAIComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public EntityUid? LastUser;
  [DataField(null, false, 1, false, false, null)]
  public EntProtoId ShopActionId = (EntProtoId) "ActionPAIOpenShop";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntityUid? ShopAction;
  [DataField(null, false, 1, false, false, null)]
  public float BrickChance = 0.5f;
  [DataField(null, false, 1, false, false, null)]
  public string BrickPopup = "pai-system-brick-popup";
  [DataField(null, false, 1, false, false, null)]
  public string ScramblePopup = "pai-system-scramble-popup";

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref PAIComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (PAIComponent) target1;
    if (serialization.TryCustomCopy<PAIComponent>(this, ref target, hookCtx, false, context))
      return;
    EntityUid? target2 = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.LastUser, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<EntityUid?>(this.LastUser, hookCtx, context);
    target.LastUser = target2;
    EntProtoId target3 = new EntProtoId();
    if (!serialization.TryCustomCopy<EntProtoId>(this.ShopActionId, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<EntProtoId>(this.ShopActionId, hookCtx, context);
    target.ShopActionId = target3;
    EntityUid? target4 = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.ShopAction, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<EntityUid?>(this.ShopAction, hookCtx, context);
    target.ShopAction = target4;
    float target5 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.BrickChance, ref target5, hookCtx, false, context))
      target5 = this.BrickChance;
    target.BrickChance = target5;
    string target6 = (string) null;
    if (this.BrickPopup == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.BrickPopup, ref target6, hookCtx, false, context))
      target6 = this.BrickPopup;
    target.BrickPopup = target6;
    string target7 = (string) null;
    if (this.ScramblePopup == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.ScramblePopup, ref target7, hookCtx, false, context))
      target7 = this.ScramblePopup;
    target.ScramblePopup = target7;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref PAIComponent target,
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
    PAIComponent target1 = (PAIComponent) target;
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
    PAIComponent target1 = (PAIComponent) target;
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
    PAIComponent target1 = (PAIComponent) target;
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
  virtual PAIComponent Component.Instantiate() => new PAIComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class PAIComponent_AutoState : IComponentState
  {
    public NetEntity? ShopAction;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class PAIComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<PAIComponent, ComponentGetState>(new ComponentEventRefHandler<PAIComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<PAIComponent, ComponentHandleState>(new ComponentEventRefHandler<PAIComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(EntityUid uid, PAIComponent component, ref ComponentGetState args)
    {
      args.State = (IComponentState) new PAIComponent.PAIComponent_AutoState()
      {
        ShopAction = this.GetNetEntity(component.ShopAction)
      };
    }

    private void OnHandleState(
      EntityUid uid,
      PAIComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is PAIComponent.PAIComponent_AutoState current))
        return;
      component.ShopAction = this.EnsureEntity<PAIComponent>(current.ShopAction, uid);
    }
  }
}
