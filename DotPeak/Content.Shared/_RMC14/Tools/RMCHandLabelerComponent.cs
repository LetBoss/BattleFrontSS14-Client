// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Tools.RMCHandLabelerComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Tools;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
public sealed class RMCHandLabelerComponent : 
  Component,
  ISerializationGenerated<RMCHandLabelerComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public SoundSpecifier LabelSound = (SoundSpecifier) new SoundPathSpecifier("/Audio/_RMC14/Items/component_pickup.ogg");
  [DataField(null, false, 1, false, false, null)]
  public SoundSpecifier RemoveLabelSound = (SoundSpecifier) new SoundPathSpecifier("/Audio/_RMC14/Items/paper_ripped.ogg");
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntityUid? CurrentPillBottle;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref RMCHandLabelerComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (RMCHandLabelerComponent) target1;
    if (serialization.TryCustomCopy<RMCHandLabelerComponent>(this, ref target, hookCtx, false, context))
      return;
    SoundSpecifier target2 = (SoundSpecifier) null;
    if (this.LabelSound == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.LabelSound, ref target2, hookCtx, true, context))
      target2 = serialization.CreateCopy<SoundSpecifier>(this.LabelSound, hookCtx, context);
    target.LabelSound = target2;
    SoundSpecifier target3 = (SoundSpecifier) null;
    if (this.RemoveLabelSound == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.RemoveLabelSound, ref target3, hookCtx, true, context))
      target3 = serialization.CreateCopy<SoundSpecifier>(this.RemoveLabelSound, hookCtx, context);
    target.RemoveLabelSound = target3;
    EntityUid? target4 = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.CurrentPillBottle, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<EntityUid?>(this.CurrentPillBottle, hookCtx, context);
    target.CurrentPillBottle = target4;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref RMCHandLabelerComponent target,
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
    RMCHandLabelerComponent target1 = (RMCHandLabelerComponent) target;
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
    RMCHandLabelerComponent target1 = (RMCHandLabelerComponent) target;
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
    RMCHandLabelerComponent target1 = (RMCHandLabelerComponent) target;
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
  virtual RMCHandLabelerComponent Component.Instantiate() => new RMCHandLabelerComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class RMCHandLabelerComponent_AutoState : IComponentState
  {
    public NetEntity? CurrentPillBottle;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class RMCHandLabelerComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<RMCHandLabelerComponent, ComponentGetState>(new ComponentEventRefHandler<RMCHandLabelerComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<RMCHandLabelerComponent, ComponentHandleState>(new ComponentEventRefHandler<RMCHandLabelerComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      RMCHandLabelerComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new RMCHandLabelerComponent.RMCHandLabelerComponent_AutoState()
      {
        CurrentPillBottle = this.GetNetEntity(component.CurrentPillBottle)
      };
    }

    private void OnHandleState(
      EntityUid uid,
      RMCHandLabelerComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is RMCHandLabelerComponent.RMCHandLabelerComponent_AutoState current))
        return;
      component.CurrentPillBottle = this.EnsureEntity<RMCHandLabelerComponent>(current.CurrentPillBottle, uid);
    }
  }
}
