// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Marines.MarineComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.NPC.Prototypes;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
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
namespace Content.Shared._RMC14.Marines;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (SharedMarineSystem)})]
public sealed class MarineComponent : 
  Component,
  ISerializationGenerated<MarineComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SpriteSpecifier? Icon;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public Dictionary<ProtoId<NpcFactionPrototype>, SpriteSpecifier> GenericFactionIcons = new Dictionary<ProtoId<NpcFactionPrototype>, SpriteSpecifier>()
  {
    {
      (ProtoId<NpcFactionPrototype>) "UNMC",
      (SpriteSpecifier) new SpriteSpecifier.Rsi(new ResPath("/Textures/_RMC14/Interface/faction_icons.rsi"), "unmc")
    },
    {
      (ProtoId<NpcFactionPrototype>) "SPP",
      (SpriteSpecifier) new SpriteSpecifier.Rsi(new ResPath("/Textures/_RMC14/Interface/faction_icons.rsi"), "spp")
    },
    {
      (ProtoId<NpcFactionPrototype>) "WeYa",
      (SpriteSpecifier) new SpriteSpecifier.Rsi(new ResPath("/Textures/_RMC14/Interface/faction_icons.rsi"), "weya")
    },
    {
      (ProtoId<NpcFactionPrototype>) "RoyalMarines",
      (SpriteSpecifier) new SpriteSpecifier.Rsi(new ResPath("/Textures/_RMC14/Interface/faction_icons.rsi"), "tse")
    },
    {
      (ProtoId<NpcFactionPrototype>) "TSE",
      (SpriteSpecifier) new SpriteSpecifier.Rsi(new ResPath("/Textures/_RMC14/Interface/faction_icons.rsi"), "tse")
    },
    {
      (ProtoId<NpcFactionPrototype>) "CLF",
      (SpriteSpecifier) new SpriteSpecifier.Rsi(new ResPath("/Textures/_RMC14/Interface/faction_icons.rsi"), "clf")
    }
  };

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref MarineComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (MarineComponent) target1;
    if (serialization.TryCustomCopy<MarineComponent>(this, ref target, hookCtx, false, context))
      return;
    SpriteSpecifier target2 = (SpriteSpecifier) null;
    if (!serialization.TryCustomCopy<SpriteSpecifier>(this.Icon, ref target2, hookCtx, true, context))
      target2 = serialization.CreateCopy<SpriteSpecifier>(this.Icon, hookCtx, context);
    target.Icon = target2;
    Dictionary<ProtoId<NpcFactionPrototype>, SpriteSpecifier> target3 = (Dictionary<ProtoId<NpcFactionPrototype>, SpriteSpecifier>) null;
    if (this.GenericFactionIcons == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<Dictionary<ProtoId<NpcFactionPrototype>, SpriteSpecifier>>(this.GenericFactionIcons, ref target3, hookCtx, true, context))
      target3 = serialization.CreateCopy<Dictionary<ProtoId<NpcFactionPrototype>, SpriteSpecifier>>(this.GenericFactionIcons, hookCtx, context);
    target.GenericFactionIcons = target3;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref MarineComponent target,
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
    MarineComponent target1 = (MarineComponent) target;
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
    MarineComponent target1 = (MarineComponent) target;
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
    MarineComponent target1 = (MarineComponent) target;
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
  virtual MarineComponent Component.Instantiate() => new MarineComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class MarineComponent_AutoState : IComponentState
  {
    public SpriteSpecifier? Icon;
    public Dictionary<ProtoId<NpcFactionPrototype>, SpriteSpecifier> GenericFactionIcons;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class MarineComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<MarineComponent, ComponentGetState>(new ComponentEventRefHandler<MarineComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<MarineComponent, ComponentHandleState>(new ComponentEventRefHandler<MarineComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(EntityUid uid, MarineComponent component, ref ComponentGetState args)
    {
      args.State = (IComponentState) new MarineComponent.MarineComponent_AutoState()
      {
        Icon = component.Icon,
        GenericFactionIcons = component.GenericFactionIcons
      };
    }

    private void OnHandleState(
      EntityUid uid,
      MarineComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is MarineComponent.MarineComponent_AutoState current))
        return;
      component.Icon = current.Icon;
      component.GenericFactionIcons = current.GenericFactionIcons == null ? (Dictionary<ProtoId<NpcFactionPrototype>, SpriteSpecifier>) null : new Dictionary<ProtoId<NpcFactionPrototype>, SpriteSpecifier>((IDictionary<ProtoId<NpcFactionPrototype>, SpriteSpecifier>) current.GenericFactionIcons);
    }
  }
}
