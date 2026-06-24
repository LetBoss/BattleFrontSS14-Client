// Decompiled with JetBrains decompiler
// Type: Content.Shared.Clothing.Components.MaskComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Clothing.EntitySystems;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Clothing.Components;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (MaskSystem)})]
public sealed class MaskComponent : 
  Component,
  ISerializationGenerated<MaskComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntProtoId ToggleAction = EntProtoId.op_Implicit("ActionToggleMask");
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntityUid? ToggleActionEntity;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool IsToggled;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public string EquippedPrefix = "up";
  [DataField("enabled", false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool IsToggleable = true;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool DisableOnFolded;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref MaskComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component component = (Component) target;
    this.InternalCopy(ref component, serialization, hookCtx, context);
    target = (MaskComponent) component;
    if (serialization.TryCustomCopy<MaskComponent>(this, ref target, hookCtx, false, context))
      return;
    EntProtoId entProtoId = new EntProtoId();
    if (!serialization.TryCustomCopy<EntProtoId>(this.ToggleAction, ref entProtoId, hookCtx, false, context))
      entProtoId = serialization.CreateCopy<EntProtoId>(this.ToggleAction, hookCtx, context, false);
    target.ToggleAction = entProtoId;
    EntityUid? nullable = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.ToggleActionEntity, ref nullable, hookCtx, false, context))
      nullable = serialization.CreateCopy<EntityUid?>(this.ToggleActionEntity, hookCtx, context, false);
    target.ToggleActionEntity = nullable;
    bool flag1 = false;
    if (!serialization.TryCustomCopy<bool>(this.IsToggled, ref flag1, hookCtx, false, context))
      flag1 = this.IsToggled;
    target.IsToggled = flag1;
    string str = (string) null;
    if (this.EquippedPrefix == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.EquippedPrefix, ref str, hookCtx, false, context))
      str = this.EquippedPrefix;
    target.EquippedPrefix = str;
    bool flag2 = false;
    if (!serialization.TryCustomCopy<bool>(this.IsToggleable, ref flag2, hookCtx, false, context))
      flag2 = this.IsToggleable;
    target.IsToggleable = flag2;
    bool flag3 = false;
    if (!serialization.TryCustomCopy<bool>(this.DisableOnFolded, ref flag3, hookCtx, false, context))
      flag3 = this.DisableOnFolded;
    target.DisableOnFolded = flag3;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref MaskComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void Copy(
    ref Component target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    MaskComponent target1 = (MaskComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (Component) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    MaskComponent target1 = (MaskComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void InternalCopy(
    ref IComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    MaskComponent target1 = (MaskComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (IComponent) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void Copy(
    ref IComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    base.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual MaskComponent Component.Instantiate() => new MaskComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class MaskComponent_AutoState : IComponentState
  {
    public EntProtoId ToggleAction;
    public NetEntity? ToggleActionEntity;
    public bool IsToggled;
    public string EquippedPrefix;
    public bool IsToggleable;
    public bool DisableOnFolded;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class MaskComponent_AutoNetworkSystem : EntitySystem
  {
    public virtual void Initialize()
    {
      // ISSUE: method pointer
      this.SubscribeLocalEvent<MaskComponent, ComponentGetState>(new ComponentEventRefHandler<MaskComponent, ComponentGetState>((object) this, __methodptr(OnGetState)), (Type[]) null, (Type[]) null);
      // ISSUE: method pointer
      this.SubscribeLocalEvent<MaskComponent, ComponentHandleState>(new ComponentEventRefHandler<MaskComponent, ComponentHandleState>((object) this, __methodptr(OnHandleState)), (Type[]) null, (Type[]) null);
    }

    private void OnGetState(EntityUid uid, MaskComponent component, ref ComponentGetState args)
    {
      ((ComponentGetState) ref args).State = (IComponentState) new MaskComponent.MaskComponent_AutoState()
      {
        ToggleAction = component.ToggleAction,
        ToggleActionEntity = this.GetNetEntity(component.ToggleActionEntity, (MetaDataComponent) null),
        IsToggled = component.IsToggled,
        EquippedPrefix = component.EquippedPrefix,
        IsToggleable = component.IsToggleable,
        DisableOnFolded = component.DisableOnFolded
      };
    }

    private void OnHandleState(
      EntityUid uid,
      MaskComponent component,
      ref ComponentHandleState args)
    {
      if (!(((ComponentHandleState) ref args).Current is MaskComponent.MaskComponent_AutoState current))
        return;
      component.ToggleAction = current.ToggleAction;
      component.ToggleActionEntity = this.EnsureEntity<MaskComponent>(current.ToggleActionEntity, uid);
      component.IsToggled = current.IsToggled;
      component.EquippedPrefix = current.EquippedPrefix;
      component.IsToggleable = current.IsToggleable;
      component.DisableOnFolded = current.DisableOnFolded;
    }
  }
}
