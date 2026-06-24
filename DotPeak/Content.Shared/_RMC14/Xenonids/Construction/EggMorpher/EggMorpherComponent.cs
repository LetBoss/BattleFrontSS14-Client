// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Construction.EggMorpher.EggMorpherComponent
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
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Xenonids.Construction.EggMorpher;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
public sealed class EggMorpherComponent : 
  Component,
  ISerializationGenerated<EggMorpherComponent>,
  ISerializationGenerated
{
  public const string ParasitePrototype = "CMXenoParasite";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int CurParasites;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int MaxParasites = 12;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int GrowMaxParasites = 6;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int ReservedParasites;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan StandardSpawnCooldown = TimeSpan.FromSeconds(120L);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan OviSpawnCooldown = TimeSpan.FromSeconds(60L);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan? NextSpawnAt;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public string OverlayPrefix = "eggmorph";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int OverlayCount = 4;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref EggMorpherComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (EggMorpherComponent) target1;
    if (serialization.TryCustomCopy<EggMorpherComponent>(this, ref target, hookCtx, false, context))
      return;
    int target2 = 0;
    if (!serialization.TryCustomCopy<int>(this.CurParasites, ref target2, hookCtx, false, context))
      target2 = this.CurParasites;
    target.CurParasites = target2;
    int target3 = 0;
    if (!serialization.TryCustomCopy<int>(this.MaxParasites, ref target3, hookCtx, false, context))
      target3 = this.MaxParasites;
    target.MaxParasites = target3;
    int target4 = 0;
    if (!serialization.TryCustomCopy<int>(this.GrowMaxParasites, ref target4, hookCtx, false, context))
      target4 = this.GrowMaxParasites;
    target.GrowMaxParasites = target4;
    int target5 = 0;
    if (!serialization.TryCustomCopy<int>(this.ReservedParasites, ref target5, hookCtx, false, context))
      target5 = this.ReservedParasites;
    target.ReservedParasites = target5;
    TimeSpan target6 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.StandardSpawnCooldown, ref target6, hookCtx, false, context))
      target6 = serialization.CreateCopy<TimeSpan>(this.StandardSpawnCooldown, hookCtx, context);
    target.StandardSpawnCooldown = target6;
    TimeSpan target7 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.OviSpawnCooldown, ref target7, hookCtx, false, context))
      target7 = serialization.CreateCopy<TimeSpan>(this.OviSpawnCooldown, hookCtx, context);
    target.OviSpawnCooldown = target7;
    TimeSpan? target8 = new TimeSpan?();
    if (!serialization.TryCustomCopy<TimeSpan?>(this.NextSpawnAt, ref target8, hookCtx, false, context))
      target8 = serialization.CreateCopy<TimeSpan?>(this.NextSpawnAt, hookCtx, context);
    target.NextSpawnAt = target8;
    string target9 = (string) null;
    if (this.OverlayPrefix == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.OverlayPrefix, ref target9, hookCtx, false, context))
      target9 = this.OverlayPrefix;
    target.OverlayPrefix = target9;
    int target10 = 0;
    if (!serialization.TryCustomCopy<int>(this.OverlayCount, ref target10, hookCtx, false, context))
      target10 = this.OverlayCount;
    target.OverlayCount = target10;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref EggMorpherComponent target,
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
    EggMorpherComponent target1 = (EggMorpherComponent) target;
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
    EggMorpherComponent target1 = (EggMorpherComponent) target;
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
    EggMorpherComponent target1 = (EggMorpherComponent) target;
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
  virtual EggMorpherComponent Component.Instantiate() => new EggMorpherComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class EggMorpherComponent_AutoState : IComponentState
  {
    public int CurParasites;
    public int MaxParasites;
    public int GrowMaxParasites;
    public int ReservedParasites;
    public TimeSpan StandardSpawnCooldown;
    public TimeSpan OviSpawnCooldown;
    public TimeSpan? NextSpawnAt;
    public string OverlayPrefix;
    public int OverlayCount;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class EggMorpherComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<EggMorpherComponent, ComponentGetState>(new ComponentEventRefHandler<EggMorpherComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<EggMorpherComponent, ComponentHandleState>(new ComponentEventRefHandler<EggMorpherComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      EggMorpherComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new EggMorpherComponent.EggMorpherComponent_AutoState()
      {
        CurParasites = component.CurParasites,
        MaxParasites = component.MaxParasites,
        GrowMaxParasites = component.GrowMaxParasites,
        ReservedParasites = component.ReservedParasites,
        StandardSpawnCooldown = component.StandardSpawnCooldown,
        OviSpawnCooldown = component.OviSpawnCooldown,
        NextSpawnAt = component.NextSpawnAt,
        OverlayPrefix = component.OverlayPrefix,
        OverlayCount = component.OverlayCount
      };
    }

    private void OnHandleState(
      EntityUid uid,
      EggMorpherComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is EggMorpherComponent.EggMorpherComponent_AutoState current))
        return;
      component.CurParasites = current.CurParasites;
      component.MaxParasites = current.MaxParasites;
      component.GrowMaxParasites = current.GrowMaxParasites;
      component.ReservedParasites = current.ReservedParasites;
      component.StandardSpawnCooldown = current.StandardSpawnCooldown;
      component.OviSpawnCooldown = current.OviSpawnCooldown;
      component.NextSpawnAt = current.NextSpawnAt;
      component.OverlayPrefix = current.OverlayPrefix;
      component.OverlayCount = current.OverlayCount;
    }
  }
}
