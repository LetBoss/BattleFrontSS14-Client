// Decompiled with JetBrains decompiler
// Type: Content.Shared.Damage.Systems.DamageProtectionBuffSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Damage.Components;
using Content.Shared.Damage.Prototypes;
using Robust.Shared.GameObjects;
using System;

#nullable enable
namespace Content.Shared.Damage.Systems;

public sealed class DamageProtectionBuffSystem : EntitySystem
{
  public virtual void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<DamageProtectionBuffComponent, DamageModifyEvent>(new ComponentEventHandler<DamageProtectionBuffComponent, DamageModifyEvent>((object) this, __methodptr(OnDamageModify)), (Type[]) null, (Type[]) null);
  }

  private void OnDamageModify(
    EntityUid uid,
    DamageProtectionBuffComponent component,
    DamageModifyEvent args)
  {
    foreach (DamageModifierSetPrototype modifierSet in component.Modifiers.Values)
      args.Damage = DamageSpecifier.ApplyModifierSet(args.Damage, (DamageModifierSet) modifierSet);
  }
}
