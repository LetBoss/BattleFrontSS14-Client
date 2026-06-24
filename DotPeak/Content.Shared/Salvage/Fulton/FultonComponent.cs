// Decompiled with JetBrains decompiler
// Type: Content.Shared.Salvage.Fulton.FultonComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Whitelist;
using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Salvage.Fulton;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
public sealed class FultonComponent : 
  Component,
  ISerializationGenerated<FultonComponent>,
  ISerializationGenerated
{
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [DataField("applyDuration", false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan ApplyFultonDuration = TimeSpan.FromSeconds(3L);
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [DataField("beacon", false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntityUid? Beacon;
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [DataField("removeable", false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool Removeable = true;
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [DataField("duration", false, 1, false, false, null)]
  public TimeSpan FultonDuration = TimeSpan.FromSeconds(45L);
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [DataField("whitelist", false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntityWhitelist? Whitelist = new EntityWhitelist()
  {
    Components = new string[2]{ "Item", "Anchorable" }
  };
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [DataField("soundFulton", false, 1, false, false, null)]
  [AutoNetworkedField]
  public SoundSpecifier? FultonSound = (SoundSpecifier) new SoundPathSpecifier("/Audio/Items/Mining/fultext_deploy.ogg");

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref FultonComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (FultonComponent) target1;
    if (serialization.TryCustomCopy<FultonComponent>(this, ref target, hookCtx, false, context))
      return;
    TimeSpan target2 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.ApplyFultonDuration, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<TimeSpan>(this.ApplyFultonDuration, hookCtx, context);
    target.ApplyFultonDuration = target2;
    EntityUid? target3 = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.Beacon, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<EntityUid?>(this.Beacon, hookCtx, context);
    target.Beacon = target3;
    bool target4 = false;
    if (!serialization.TryCustomCopy<bool>(this.Removeable, ref target4, hookCtx, false, context))
      target4 = this.Removeable;
    target.Removeable = target4;
    TimeSpan target5 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.FultonDuration, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<TimeSpan>(this.FultonDuration, hookCtx, context);
    target.FultonDuration = target5;
    EntityWhitelist target6 = (EntityWhitelist) null;
    if (!serialization.TryCustomCopy<EntityWhitelist>(this.Whitelist, ref target6, hookCtx, false, context))
    {
      if (this.Whitelist == null)
        target6 = (EntityWhitelist) null;
      else
        serialization.CopyTo<EntityWhitelist>(this.Whitelist, ref target6, hookCtx, context);
    }
    target.Whitelist = target6;
    SoundSpecifier target7 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.FultonSound, ref target7, hookCtx, true, context))
      target7 = serialization.CreateCopy<SoundSpecifier>(this.FultonSound, hookCtx, context);
    target.FultonSound = target7;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref FultonComponent target,
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
    FultonComponent target1 = (FultonComponent) target;
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
    FultonComponent target1 = (FultonComponent) target;
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
    FultonComponent target1 = (FultonComponent) target;
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
  virtual FultonComponent Component.Instantiate() => new FultonComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class FultonComponent_AutoState : IComponentState
  {
    public TimeSpan ApplyFultonDuration;
    public NetEntity? Beacon;
    public bool Removeable;
    public EntityWhitelist? Whitelist;
    public SoundSpecifier? FultonSound;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class FultonComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<FultonComponent, ComponentGetState>(new ComponentEventRefHandler<FultonComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<FultonComponent, ComponentHandleState>(new ComponentEventRefHandler<FultonComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(EntityUid uid, FultonComponent component, ref ComponentGetState args)
    {
      args.State = (IComponentState) new FultonComponent.FultonComponent_AutoState()
      {
        ApplyFultonDuration = component.ApplyFultonDuration,
        Beacon = this.GetNetEntity(component.Beacon),
        Removeable = component.Removeable,
        Whitelist = component.Whitelist,
        FultonSound = component.FultonSound
      };
    }

    private void OnHandleState(
      EntityUid uid,
      FultonComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is FultonComponent.FultonComponent_AutoState current))
        return;
      component.ApplyFultonDuration = current.ApplyFultonDuration;
      component.Beacon = this.EnsureEntity<FultonComponent>(current.Beacon, uid);
      component.Removeable = current.Removeable;
      component.Whitelist = current.Whitelist;
      component.FultonSound = current.FultonSound;
    }
  }
}
