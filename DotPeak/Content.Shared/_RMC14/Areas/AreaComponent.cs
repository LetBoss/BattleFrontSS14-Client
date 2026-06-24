// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Areas.AreaComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.WeedKiller;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Areas;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (AreaSystem)})]
public sealed class AreaComponent : 
  Component,
  ISerializationGenerated<AreaComponent>,
  ISerializationGenerated
{
  [DataField("CAS", false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool CAS;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool Fulton;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool Lasing;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool MortarPlacement;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool MortarFire;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool Medevac;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool Paradropping;
  [DataField("OB", false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool OB;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool SupplyDrop;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool AvoidBioscan;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool NoTunnel;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool Unweedable;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool BuildSpecial;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool ResinAllowed = true;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool ResinConstructionAllowed = true;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool WeatherEnabled = true;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool HijackEvacuationArea;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool AlwaysPowered;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public double HijackEvacuationWeight;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public AreaHijackEvacuationType HijackEvacuationType;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public string? PowerNet;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public Color MinimapColor;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int ZLevel;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool LandingZone;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  [Access(new Type[] {typeof (AreaSystem), typeof (WeedKillerSystem)})]
  public string? LinkedLz;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  [Access(new Type[] {typeof (AreaSystem), typeof (WeedKillerSystem)})]
  public bool WeedKilling;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool RetrieveItemObjective;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int BuildableTiles;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int ResinConstructCount;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref AreaComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (AreaComponent) target1;
    if (serialization.TryCustomCopy<AreaComponent>(this, ref target, hookCtx, false, context))
      return;
    bool target2 = false;
    if (!serialization.TryCustomCopy<bool>(this.CAS, ref target2, hookCtx, false, context))
      target2 = this.CAS;
    target.CAS = target2;
    bool target3 = false;
    if (!serialization.TryCustomCopy<bool>(this.Fulton, ref target3, hookCtx, false, context))
      target3 = this.Fulton;
    target.Fulton = target3;
    bool target4 = false;
    if (!serialization.TryCustomCopy<bool>(this.Lasing, ref target4, hookCtx, false, context))
      target4 = this.Lasing;
    target.Lasing = target4;
    bool target5 = false;
    if (!serialization.TryCustomCopy<bool>(this.MortarPlacement, ref target5, hookCtx, false, context))
      target5 = this.MortarPlacement;
    target.MortarPlacement = target5;
    bool target6 = false;
    if (!serialization.TryCustomCopy<bool>(this.MortarFire, ref target6, hookCtx, false, context))
      target6 = this.MortarFire;
    target.MortarFire = target6;
    bool target7 = false;
    if (!serialization.TryCustomCopy<bool>(this.Medevac, ref target7, hookCtx, false, context))
      target7 = this.Medevac;
    target.Medevac = target7;
    bool target8 = false;
    if (!serialization.TryCustomCopy<bool>(this.Paradropping, ref target8, hookCtx, false, context))
      target8 = this.Paradropping;
    target.Paradropping = target8;
    bool target9 = false;
    if (!serialization.TryCustomCopy<bool>(this.OB, ref target9, hookCtx, false, context))
      target9 = this.OB;
    target.OB = target9;
    bool target10 = false;
    if (!serialization.TryCustomCopy<bool>(this.SupplyDrop, ref target10, hookCtx, false, context))
      target10 = this.SupplyDrop;
    target.SupplyDrop = target10;
    bool target11 = false;
    if (!serialization.TryCustomCopy<bool>(this.AvoidBioscan, ref target11, hookCtx, false, context))
      target11 = this.AvoidBioscan;
    target.AvoidBioscan = target11;
    bool target12 = false;
    if (!serialization.TryCustomCopy<bool>(this.NoTunnel, ref target12, hookCtx, false, context))
      target12 = this.NoTunnel;
    target.NoTunnel = target12;
    bool target13 = false;
    if (!serialization.TryCustomCopy<bool>(this.Unweedable, ref target13, hookCtx, false, context))
      target13 = this.Unweedable;
    target.Unweedable = target13;
    bool target14 = false;
    if (!serialization.TryCustomCopy<bool>(this.BuildSpecial, ref target14, hookCtx, false, context))
      target14 = this.BuildSpecial;
    target.BuildSpecial = target14;
    bool target15 = false;
    if (!serialization.TryCustomCopy<bool>(this.ResinAllowed, ref target15, hookCtx, false, context))
      target15 = this.ResinAllowed;
    target.ResinAllowed = target15;
    bool target16 = false;
    if (!serialization.TryCustomCopy<bool>(this.ResinConstructionAllowed, ref target16, hookCtx, false, context))
      target16 = this.ResinConstructionAllowed;
    target.ResinConstructionAllowed = target16;
    bool target17 = false;
    if (!serialization.TryCustomCopy<bool>(this.WeatherEnabled, ref target17, hookCtx, false, context))
      target17 = this.WeatherEnabled;
    target.WeatherEnabled = target17;
    bool target18 = false;
    if (!serialization.TryCustomCopy<bool>(this.HijackEvacuationArea, ref target18, hookCtx, false, context))
      target18 = this.HijackEvacuationArea;
    target.HijackEvacuationArea = target18;
    bool target19 = false;
    if (!serialization.TryCustomCopy<bool>(this.AlwaysPowered, ref target19, hookCtx, false, context))
      target19 = this.AlwaysPowered;
    target.AlwaysPowered = target19;
    double target20 = 0.0;
    if (!serialization.TryCustomCopy<double>(this.HijackEvacuationWeight, ref target20, hookCtx, false, context))
      target20 = this.HijackEvacuationWeight;
    target.HijackEvacuationWeight = target20;
    AreaHijackEvacuationType target21 = AreaHijackEvacuationType.None;
    if (!serialization.TryCustomCopy<AreaHijackEvacuationType>(this.HijackEvacuationType, ref target21, hookCtx, false, context))
      target21 = this.HijackEvacuationType;
    target.HijackEvacuationType = target21;
    string target22 = (string) null;
    if (!serialization.TryCustomCopy<string>(this.PowerNet, ref target22, hookCtx, false, context))
      target22 = this.PowerNet;
    target.PowerNet = target22;
    Color target23 = new Color();
    if (!serialization.TryCustomCopy<Color>(this.MinimapColor, ref target23, hookCtx, false, context))
      target23 = serialization.CreateCopy<Color>(this.MinimapColor, hookCtx, context);
    target.MinimapColor = target23;
    int target24 = 0;
    if (!serialization.TryCustomCopy<int>(this.ZLevel, ref target24, hookCtx, false, context))
      target24 = this.ZLevel;
    target.ZLevel = target24;
    bool target25 = false;
    if (!serialization.TryCustomCopy<bool>(this.LandingZone, ref target25, hookCtx, false, context))
      target25 = this.LandingZone;
    target.LandingZone = target25;
    string target26 = (string) null;
    if (!serialization.TryCustomCopy<string>(this.LinkedLz, ref target26, hookCtx, false, context))
      target26 = this.LinkedLz;
    target.LinkedLz = target26;
    bool target27 = false;
    if (!serialization.TryCustomCopy<bool>(this.WeedKilling, ref target27, hookCtx, false, context))
      target27 = this.WeedKilling;
    target.WeedKilling = target27;
    bool target28 = false;
    if (!serialization.TryCustomCopy<bool>(this.RetrieveItemObjective, ref target28, hookCtx, false, context))
      target28 = this.RetrieveItemObjective;
    target.RetrieveItemObjective = target28;
    int target29 = 0;
    if (!serialization.TryCustomCopy<int>(this.BuildableTiles, ref target29, hookCtx, false, context))
      target29 = this.BuildableTiles;
    target.BuildableTiles = target29;
    int target30 = 0;
    if (!serialization.TryCustomCopy<int>(this.ResinConstructCount, ref target30, hookCtx, false, context))
      target30 = this.ResinConstructCount;
    target.ResinConstructCount = target30;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref AreaComponent target,
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
    AreaComponent target1 = (AreaComponent) target;
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
    AreaComponent target1 = (AreaComponent) target;
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
    AreaComponent target1 = (AreaComponent) target;
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
  virtual AreaComponent Component.Instantiate() => new AreaComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class AreaComponent_AutoState : IComponentState
  {
    public bool CAS;
    public bool Fulton;
    public bool Lasing;
    public bool MortarPlacement;
    public bool MortarFire;
    public bool Medevac;
    public bool Paradropping;
    public bool OB;
    public bool SupplyDrop;
    public bool AvoidBioscan;
    public bool NoTunnel;
    public bool Unweedable;
    public bool BuildSpecial;
    public bool ResinAllowed;
    public bool ResinConstructionAllowed;
    public bool WeatherEnabled;
    public bool HijackEvacuationArea;
    public bool AlwaysPowered;
    public double HijackEvacuationWeight;
    public AreaHijackEvacuationType HijackEvacuationType;
    public string? PowerNet;
    public Color MinimapColor;
    public int ZLevel;
    public bool LandingZone;
    public string? LinkedLz;
    public bool WeedKilling;
    public bool RetrieveItemObjective;
    public int BuildableTiles;
    public int ResinConstructCount;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class AreaComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<AreaComponent, ComponentGetState>(new ComponentEventRefHandler<AreaComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<AreaComponent, ComponentHandleState>(new ComponentEventRefHandler<AreaComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(EntityUid uid, AreaComponent component, ref ComponentGetState args)
    {
      args.State = (IComponentState) new AreaComponent.AreaComponent_AutoState()
      {
        CAS = component.CAS,
        Fulton = component.Fulton,
        Lasing = component.Lasing,
        MortarPlacement = component.MortarPlacement,
        MortarFire = component.MortarFire,
        Medevac = component.Medevac,
        Paradropping = component.Paradropping,
        OB = component.OB,
        SupplyDrop = component.SupplyDrop,
        AvoidBioscan = component.AvoidBioscan,
        NoTunnel = component.NoTunnel,
        Unweedable = component.Unweedable,
        BuildSpecial = component.BuildSpecial,
        ResinAllowed = component.ResinAllowed,
        ResinConstructionAllowed = component.ResinConstructionAllowed,
        WeatherEnabled = component.WeatherEnabled,
        HijackEvacuationArea = component.HijackEvacuationArea,
        AlwaysPowered = component.AlwaysPowered,
        HijackEvacuationWeight = component.HijackEvacuationWeight,
        HijackEvacuationType = component.HijackEvacuationType,
        PowerNet = component.PowerNet,
        MinimapColor = component.MinimapColor,
        ZLevel = component.ZLevel,
        LandingZone = component.LandingZone,
        LinkedLz = component.LinkedLz,
        WeedKilling = component.WeedKilling,
        RetrieveItemObjective = component.RetrieveItemObjective,
        BuildableTiles = component.BuildableTiles,
        ResinConstructCount = component.ResinConstructCount
      };
    }

    private void OnHandleState(
      EntityUid uid,
      AreaComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is AreaComponent.AreaComponent_AutoState current))
        return;
      component.CAS = current.CAS;
      component.Fulton = current.Fulton;
      component.Lasing = current.Lasing;
      component.MortarPlacement = current.MortarPlacement;
      component.MortarFire = current.MortarFire;
      component.Medevac = current.Medevac;
      component.Paradropping = current.Paradropping;
      component.OB = current.OB;
      component.SupplyDrop = current.SupplyDrop;
      component.AvoidBioscan = current.AvoidBioscan;
      component.NoTunnel = current.NoTunnel;
      component.Unweedable = current.Unweedable;
      component.BuildSpecial = current.BuildSpecial;
      component.ResinAllowed = current.ResinAllowed;
      component.ResinConstructionAllowed = current.ResinConstructionAllowed;
      component.WeatherEnabled = current.WeatherEnabled;
      component.HijackEvacuationArea = current.HijackEvacuationArea;
      component.AlwaysPowered = current.AlwaysPowered;
      component.HijackEvacuationWeight = current.HijackEvacuationWeight;
      component.HijackEvacuationType = current.HijackEvacuationType;
      component.PowerNet = current.PowerNet;
      component.MinimapColor = current.MinimapColor;
      component.ZLevel = current.ZLevel;
      component.LandingZone = current.LandingZone;
      component.LinkedLz = current.LinkedLz;
      component.WeedKilling = current.WeedKilling;
      component.RetrieveItemObjective = current.RetrieveItemObjective;
      component.BuildableTiles = current.BuildableTiles;
      component.ResinConstructCount = current.ResinConstructCount;
    }
  }
}
