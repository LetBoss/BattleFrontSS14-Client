// Decompiled with JetBrains decompiler
// Type: Content.Shared.Materials.MaterialReclaimerComponent
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
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;
using Robust.Shared.ViewVariables;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Materials;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[AutoGenerateComponentPause]
[Access(new Type[] {typeof (SharedMaterialReclaimerSystem)})]
public sealed class MaterialReclaimerComponent : 
  Component,
  ISerializationGenerated<MaterialReclaimerComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public bool Powered;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public bool Enabled = true;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool Broken;
  [DataField(null, false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public float Efficiency = 1f;
  [DataField(null, false, 1, false, false, null)]
  public bool ScaleProcessSpeed = true;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public float MaterialProcessRate = 100f;
  [DataField(null, false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public TimeSpan MinimumProcessDuration = TimeSpan.FromSeconds(0.5);
  [DataField(null, false, 1, false, false, null)]
  public string? SolutionContainerId;
  [DataField(null, false, 1, false, false, null)]
  public bool OnlyReclaimDrainable = true;
  [DataField(null, false, 1, false, false, null)]
  public EntityWhitelist? Whitelist;
  [DataField(null, false, 1, false, false, null)]
  public EntityWhitelist? Blacklist;
  [DataField(null, false, 1, false, false, null)]
  public SoundSpecifier? Sound;
  [DataField(null, false, 1, false, false, null)]
  public bool CutOffSound = true;
  [DataField(null, false, 1, false, false, typeof (TimeOffsetSerializer))]
  [AutoPausedField]
  public TimeSpan NextSound;
  [DataField(null, false, 1, false, false, null)]
  public TimeSpan SoundCooldown = TimeSpan.FromSeconds(0.800000011920929);
  public EntityUid? Stream;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int ItemsProcessed;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref MaterialReclaimerComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (MaterialReclaimerComponent) target1;
    if (serialization.TryCustomCopy<MaterialReclaimerComponent>(this, ref target, hookCtx, false, context))
      return;
    bool target2 = false;
    if (!serialization.TryCustomCopy<bool>(this.Powered, ref target2, hookCtx, false, context))
      target2 = this.Powered;
    target.Powered = target2;
    bool target3 = false;
    if (!serialization.TryCustomCopy<bool>(this.Enabled, ref target3, hookCtx, false, context))
      target3 = this.Enabled;
    target.Enabled = target3;
    bool target4 = false;
    if (!serialization.TryCustomCopy<bool>(this.Broken, ref target4, hookCtx, false, context))
      target4 = this.Broken;
    target.Broken = target4;
    float target5 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.Efficiency, ref target5, hookCtx, false, context))
      target5 = this.Efficiency;
    target.Efficiency = target5;
    bool target6 = false;
    if (!serialization.TryCustomCopy<bool>(this.ScaleProcessSpeed, ref target6, hookCtx, false, context))
      target6 = this.ScaleProcessSpeed;
    target.ScaleProcessSpeed = target6;
    float target7 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.MaterialProcessRate, ref target7, hookCtx, false, context))
      target7 = this.MaterialProcessRate;
    target.MaterialProcessRate = target7;
    TimeSpan target8 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.MinimumProcessDuration, ref target8, hookCtx, false, context))
      target8 = serialization.CreateCopy<TimeSpan>(this.MinimumProcessDuration, hookCtx, context);
    target.MinimumProcessDuration = target8;
    string target9 = (string) null;
    if (!serialization.TryCustomCopy<string>(this.SolutionContainerId, ref target9, hookCtx, false, context))
      target9 = this.SolutionContainerId;
    target.SolutionContainerId = target9;
    bool target10 = false;
    if (!serialization.TryCustomCopy<bool>(this.OnlyReclaimDrainable, ref target10, hookCtx, false, context))
      target10 = this.OnlyReclaimDrainable;
    target.OnlyReclaimDrainable = target10;
    EntityWhitelist target11 = (EntityWhitelist) null;
    if (!serialization.TryCustomCopy<EntityWhitelist>(this.Whitelist, ref target11, hookCtx, false, context))
    {
      if (this.Whitelist == null)
        target11 = (EntityWhitelist) null;
      else
        serialization.CopyTo<EntityWhitelist>(this.Whitelist, ref target11, hookCtx, context);
    }
    target.Whitelist = target11;
    EntityWhitelist target12 = (EntityWhitelist) null;
    if (!serialization.TryCustomCopy<EntityWhitelist>(this.Blacklist, ref target12, hookCtx, false, context))
    {
      if (this.Blacklist == null)
        target12 = (EntityWhitelist) null;
      else
        serialization.CopyTo<EntityWhitelist>(this.Blacklist, ref target12, hookCtx, context);
    }
    target.Blacklist = target12;
    SoundSpecifier target13 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.Sound, ref target13, hookCtx, true, context))
      target13 = serialization.CreateCopy<SoundSpecifier>(this.Sound, hookCtx, context);
    target.Sound = target13;
    bool target14 = false;
    if (!serialization.TryCustomCopy<bool>(this.CutOffSound, ref target14, hookCtx, false, context))
      target14 = this.CutOffSound;
    target.CutOffSound = target14;
    TimeSpan target15 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.NextSound, ref target15, hookCtx, false, context))
      target15 = serialization.CreateCopy<TimeSpan>(this.NextSound, hookCtx, context);
    target.NextSound = target15;
    TimeSpan target16 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.SoundCooldown, ref target16, hookCtx, false, context))
      target16 = serialization.CreateCopy<TimeSpan>(this.SoundCooldown, hookCtx, context);
    target.SoundCooldown = target16;
    int target17 = 0;
    if (!serialization.TryCustomCopy<int>(this.ItemsProcessed, ref target17, hookCtx, false, context))
      target17 = this.ItemsProcessed;
    target.ItemsProcessed = target17;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref MaterialReclaimerComponent target,
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
    MaterialReclaimerComponent target1 = (MaterialReclaimerComponent) target;
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
    MaterialReclaimerComponent target1 = (MaterialReclaimerComponent) target;
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
    MaterialReclaimerComponent target1 = (MaterialReclaimerComponent) target;
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
  virtual MaterialReclaimerComponent Component.Instantiate() => new MaterialReclaimerComponent();

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class MaterialReclaimerComponent_AutoPauseSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<MaterialReclaimerComponent, EntityUnpausedEvent>(new ComponentEventRefHandler<MaterialReclaimerComponent, EntityUnpausedEvent>(this.OnEntityUnpaused));
    }

    private void OnEntityUnpaused(
      EntityUid uid,
      #nullable disable
      MaterialReclaimerComponent component,
      ref EntityUnpausedEvent args)
    {
      component.NextSound += args.PausedTime;
    }
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class MaterialReclaimerComponent_AutoState : IComponentState
  {
    public bool Powered;
    public bool Enabled;
    public bool Broken;
    public float MaterialProcessRate;
    public int ItemsProcessed;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class MaterialReclaimerComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<MaterialReclaimerComponent, ComponentGetState>(new ComponentEventRefHandler<MaterialReclaimerComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<MaterialReclaimerComponent, ComponentHandleState>(new ComponentEventRefHandler<MaterialReclaimerComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      #nullable enable
      MaterialReclaimerComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new MaterialReclaimerComponent.MaterialReclaimerComponent_AutoState()
      {
        Powered = component.Powered,
        Enabled = component.Enabled,
        Broken = component.Broken,
        MaterialProcessRate = component.MaterialProcessRate,
        ItemsProcessed = component.ItemsProcessed
      };
    }

    private void OnHandleState(
      EntityUid uid,
      MaterialReclaimerComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is MaterialReclaimerComponent.MaterialReclaimerComponent_AutoState current))
        return;
      component.Powered = current.Powered;
      component.Enabled = current.Enabled;
      component.Broken = current.Broken;
      component.MaterialProcessRate = current.MaterialProcessRate;
      component.ItemsProcessed = current.ItemsProcessed;
    }
  }
}
