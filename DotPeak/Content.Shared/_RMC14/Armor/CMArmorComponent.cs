// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Armor.CMArmorComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Armor;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (CMArmorSystem)})]
public sealed class CMArmorComponent : 
  Component,
  ISerializationGenerated<CMArmorComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int XenoArmor;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int FrontalArmor;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int SideArmor;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int Melee;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int Bullet;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int Bio;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int ExplosionArmor;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool ImmuneToAP;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref CMArmorComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (CMArmorComponent) target1;
    if (serialization.TryCustomCopy<CMArmorComponent>(this, ref target, hookCtx, false, context))
      return;
    int target2 = 0;
    if (!serialization.TryCustomCopy<int>(this.XenoArmor, ref target2, hookCtx, false, context))
      target2 = this.XenoArmor;
    target.XenoArmor = target2;
    int target3 = 0;
    if (!serialization.TryCustomCopy<int>(this.FrontalArmor, ref target3, hookCtx, false, context))
      target3 = this.FrontalArmor;
    target.FrontalArmor = target3;
    int target4 = 0;
    if (!serialization.TryCustomCopy<int>(this.SideArmor, ref target4, hookCtx, false, context))
      target4 = this.SideArmor;
    target.SideArmor = target4;
    int target5 = 0;
    if (!serialization.TryCustomCopy<int>(this.Melee, ref target5, hookCtx, false, context))
      target5 = this.Melee;
    target.Melee = target5;
    int target6 = 0;
    if (!serialization.TryCustomCopy<int>(this.Bullet, ref target6, hookCtx, false, context))
      target6 = this.Bullet;
    target.Bullet = target6;
    int target7 = 0;
    if (!serialization.TryCustomCopy<int>(this.Bio, ref target7, hookCtx, false, context))
      target7 = this.Bio;
    target.Bio = target7;
    int target8 = 0;
    if (!serialization.TryCustomCopy<int>(this.ExplosionArmor, ref target8, hookCtx, false, context))
      target8 = this.ExplosionArmor;
    target.ExplosionArmor = target8;
    bool target9 = false;
    if (!serialization.TryCustomCopy<bool>(this.ImmuneToAP, ref target9, hookCtx, false, context))
      target9 = this.ImmuneToAP;
    target.ImmuneToAP = target9;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref CMArmorComponent target,
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
    CMArmorComponent target1 = (CMArmorComponent) target;
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
    CMArmorComponent target1 = (CMArmorComponent) target;
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
    CMArmorComponent target1 = (CMArmorComponent) target;
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
  virtual CMArmorComponent Component.Instantiate() => new CMArmorComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class CMArmorComponent_AutoState : IComponentState
  {
    public int XenoArmor;
    public int FrontalArmor;
    public int SideArmor;
    public int Melee;
    public int Bullet;
    public int Bio;
    public int ExplosionArmor;
    public bool ImmuneToAP;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class CMArmorComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<CMArmorComponent, ComponentGetState>(new ComponentEventRefHandler<CMArmorComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<CMArmorComponent, ComponentHandleState>(new ComponentEventRefHandler<CMArmorComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(EntityUid uid, CMArmorComponent component, ref ComponentGetState args)
    {
      args.State = (IComponentState) new CMArmorComponent.CMArmorComponent_AutoState()
      {
        XenoArmor = component.XenoArmor,
        FrontalArmor = component.FrontalArmor,
        SideArmor = component.SideArmor,
        Melee = component.Melee,
        Bullet = component.Bullet,
        Bio = component.Bio,
        ExplosionArmor = component.ExplosionArmor,
        ImmuneToAP = component.ImmuneToAP
      };
    }

    private void OnHandleState(
      EntityUid uid,
      CMArmorComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is CMArmorComponent.CMArmorComponent_AutoState current))
        return;
      component.XenoArmor = current.XenoArmor;
      component.FrontalArmor = current.FrontalArmor;
      component.SideArmor = current.SideArmor;
      component.Melee = current.Melee;
      component.Bullet = current.Bullet;
      component.Bio = current.Bio;
      component.ExplosionArmor = current.ExplosionArmor;
      component.ImmuneToAP = current.ImmuneToAP;
    }
  }
}
