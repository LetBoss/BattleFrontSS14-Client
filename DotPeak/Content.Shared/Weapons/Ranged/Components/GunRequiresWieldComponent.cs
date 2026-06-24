// Decompiled with JetBrains decompiler
// Type: Content.Shared.Weapons.Ranged.Components.GunRequiresWieldComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Wieldable;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Localization;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Weapons.Ranged.Components;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (SharedWieldableSystem)})]
public sealed class GunRequiresWieldComponent : 
  Component,
  ISerializationGenerated<GunRequiresWieldComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan LastPopup;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan PopupCooldown = TimeSpan.FromSeconds(1L);
  [DataField(null, false, 1, false, false, null)]
  public LocId? WieldRequiresExamineMessage = (LocId?) "gunrequireswield-component-examine";

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref GunRequiresWieldComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (GunRequiresWieldComponent) target1;
    if (serialization.TryCustomCopy<GunRequiresWieldComponent>(this, ref target, hookCtx, false, context))
      return;
    TimeSpan target2 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.LastPopup, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<TimeSpan>(this.LastPopup, hookCtx, context);
    target.LastPopup = target2;
    TimeSpan target3 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.PopupCooldown, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<TimeSpan>(this.PopupCooldown, hookCtx, context);
    target.PopupCooldown = target3;
    LocId? target4 = new LocId?();
    if (!serialization.TryCustomCopy<LocId?>(this.WieldRequiresExamineMessage, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<LocId?>(this.WieldRequiresExamineMessage, hookCtx, context);
    target.WieldRequiresExamineMessage = target4;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref GunRequiresWieldComponent target,
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
    GunRequiresWieldComponent target1 = (GunRequiresWieldComponent) target;
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
    GunRequiresWieldComponent target1 = (GunRequiresWieldComponent) target;
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
    GunRequiresWieldComponent target1 = (GunRequiresWieldComponent) target;
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
  virtual GunRequiresWieldComponent Component.Instantiate() => new GunRequiresWieldComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class GunRequiresWieldComponent_AutoState : IComponentState
  {
    public TimeSpan LastPopup;
    public TimeSpan PopupCooldown;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class GunRequiresWieldComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<GunRequiresWieldComponent, ComponentGetState>(new ComponentEventRefHandler<GunRequiresWieldComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<GunRequiresWieldComponent, ComponentHandleState>(new ComponentEventRefHandler<GunRequiresWieldComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      GunRequiresWieldComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new GunRequiresWieldComponent.GunRequiresWieldComponent_AutoState()
      {
        LastPopup = component.LastPopup,
        PopupCooldown = component.PopupCooldown
      };
    }

    private void OnHandleState(
      EntityUid uid,
      GunRequiresWieldComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is GunRequiresWieldComponent.GunRequiresWieldComponent_AutoState current))
        return;
      component.LastPopup = current.LastPopup;
      component.PopupCooldown = current.PopupCooldown;
    }
  }
}
