// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Vents.VentCrawlableComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Atmos;
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
namespace Content.Shared._RMC14.Vents;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
public sealed class VentCrawlableComponent : 
  Component,
  ISerializationGenerated<VentCrawlableComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public string ContainerId = "rmc_vent_container";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public string LayerId = "default_vent";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int? MaxEntities;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public PipeDirection TravelDirection;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SoundSpecifier? TravelSound = (SoundSpecifier) new SoundCollectionSpecifier("XenoVentCrawl");

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref VentCrawlableComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (VentCrawlableComponent) target1;
    if (serialization.TryCustomCopy<VentCrawlableComponent>(this, ref target, hookCtx, false, context))
      return;
    string target2 = (string) null;
    if (this.ContainerId == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.ContainerId, ref target2, hookCtx, false, context))
      target2 = this.ContainerId;
    target.ContainerId = target2;
    string target3 = (string) null;
    if (this.LayerId == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.LayerId, ref target3, hookCtx, false, context))
      target3 = this.LayerId;
    target.LayerId = target3;
    int? target4 = new int?();
    if (!serialization.TryCustomCopy<int?>(this.MaxEntities, ref target4, hookCtx, false, context))
      target4 = this.MaxEntities;
    target.MaxEntities = target4;
    PipeDirection target5 = PipeDirection.None;
    if (!serialization.TryCustomCopy<PipeDirection>(this.TravelDirection, ref target5, hookCtx, false, context))
      target5 = this.TravelDirection;
    target.TravelDirection = target5;
    SoundSpecifier target6 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.TravelSound, ref target6, hookCtx, true, context))
      target6 = serialization.CreateCopy<SoundSpecifier>(this.TravelSound, hookCtx, context);
    target.TravelSound = target6;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref VentCrawlableComponent target,
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
    VentCrawlableComponent target1 = (VentCrawlableComponent) target;
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
    VentCrawlableComponent target1 = (VentCrawlableComponent) target;
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
    VentCrawlableComponent target1 = (VentCrawlableComponent) target;
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
  virtual VentCrawlableComponent Component.Instantiate() => new VentCrawlableComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class VentCrawlableComponent_AutoState : IComponentState
  {
    public string ContainerId;
    public string LayerId;
    public int? MaxEntities;
    public PipeDirection TravelDirection;
    public SoundSpecifier? TravelSound;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class VentCrawlableComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<VentCrawlableComponent, ComponentGetState>(new ComponentEventRefHandler<VentCrawlableComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<VentCrawlableComponent, ComponentHandleState>(new ComponentEventRefHandler<VentCrawlableComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      VentCrawlableComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new VentCrawlableComponent.VentCrawlableComponent_AutoState()
      {
        ContainerId = component.ContainerId,
        LayerId = component.LayerId,
        MaxEntities = component.MaxEntities,
        TravelDirection = component.TravelDirection,
        TravelSound = component.TravelSound
      };
    }

    private void OnHandleState(
      EntityUid uid,
      VentCrawlableComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is VentCrawlableComponent.VentCrawlableComponent_AutoState current))
        return;
      component.ContainerId = current.ContainerId;
      component.LayerId = current.LayerId;
      component.MaxEntities = current.MaxEntities;
      component.TravelDirection = current.TravelDirection;
      component.TravelSound = current.TravelSound;
    }
  }
}
