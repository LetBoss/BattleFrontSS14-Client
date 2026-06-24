// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Marines.Squads.SquadMemberComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Marines.Squads;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (SquadSystem)})]
public sealed class SquadMemberComponent : 
  Component,
  ISerializationGenerated<SquadMemberComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntityUid? Squad;
  [DataField(null, false, 1, true, false, null)]
  [AutoNetworkedField]
  public SpriteSpecifier Background;
  [DataField(null, false, 1, true, false, null)]
  [AutoNetworkedField]
  public Color BackgroundColor;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public Color? AccessibleBackgroundColor;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public List<SquadArmorLayers> BlacklistedSquadArmor = new List<SquadArmorLayers>();

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref SquadMemberComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (SquadMemberComponent) target1;
    if (serialization.TryCustomCopy<SquadMemberComponent>(this, ref target, hookCtx, false, context))
      return;
    EntityUid? target2 = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.Squad, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<EntityUid?>(this.Squad, hookCtx, context);
    target.Squad = target2;
    SpriteSpecifier target3 = (SpriteSpecifier) null;
    if (this.Background == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SpriteSpecifier>(this.Background, ref target3, hookCtx, true, context))
      target3 = serialization.CreateCopy<SpriteSpecifier>(this.Background, hookCtx, context);
    target.Background = target3;
    Color target4 = new Color();
    if (!serialization.TryCustomCopy<Color>(this.BackgroundColor, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<Color>(this.BackgroundColor, hookCtx, context);
    target.BackgroundColor = target4;
    Color? target5 = new Color?();
    if (!serialization.TryCustomCopy<Color?>(this.AccessibleBackgroundColor, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<Color?>(this.AccessibleBackgroundColor, hookCtx, context);
    target.AccessibleBackgroundColor = target5;
    List<SquadArmorLayers> target6 = (List<SquadArmorLayers>) null;
    if (this.BlacklistedSquadArmor == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<List<SquadArmorLayers>>(this.BlacklistedSquadArmor, ref target6, hookCtx, true, context))
      target6 = serialization.CreateCopy<List<SquadArmorLayers>>(this.BlacklistedSquadArmor, hookCtx, context);
    target.BlacklistedSquadArmor = target6;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref SquadMemberComponent target,
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
    SquadMemberComponent target1 = (SquadMemberComponent) target;
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
    SquadMemberComponent target1 = (SquadMemberComponent) target;
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
    SquadMemberComponent target1 = (SquadMemberComponent) target;
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
  virtual SquadMemberComponent Component.Instantiate() => new SquadMemberComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class SquadMemberComponent_AutoState : IComponentState
  {
    public NetEntity? Squad;
    public SpriteSpecifier Background;
    public Color BackgroundColor;
    public Color? AccessibleBackgroundColor;
    public List<SquadArmorLayers> BlacklistedSquadArmor;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class SquadMemberComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<SquadMemberComponent, ComponentGetState>(new ComponentEventRefHandler<SquadMemberComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<SquadMemberComponent, ComponentHandleState>(new ComponentEventRefHandler<SquadMemberComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      SquadMemberComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new SquadMemberComponent.SquadMemberComponent_AutoState()
      {
        Squad = this.GetNetEntity(component.Squad),
        Background = component.Background,
        BackgroundColor = component.BackgroundColor,
        AccessibleBackgroundColor = component.AccessibleBackgroundColor,
        BlacklistedSquadArmor = component.BlacklistedSquadArmor
      };
    }

    private void OnHandleState(
      EntityUid uid,
      SquadMemberComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is SquadMemberComponent.SquadMemberComponent_AutoState current))
        return;
      component.Squad = this.EnsureEntity<SquadMemberComponent>(current.Squad, uid);
      component.Background = current.Background;
      component.BackgroundColor = current.BackgroundColor;
      component.AccessibleBackgroundColor = current.AccessibleBackgroundColor;
      component.BlacklistedSquadArmor = current.BlacklistedSquadArmor == null ? (List<SquadArmorLayers>) null : new List<SquadArmorLayers>((IEnumerable<SquadArmorLayers>) current.BlacklistedSquadArmor);
    }
  }
}
