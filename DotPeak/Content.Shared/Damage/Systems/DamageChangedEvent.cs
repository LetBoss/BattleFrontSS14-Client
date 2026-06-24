// Decompiled with JetBrains decompiler
// Type: Content.Shared.Damage.DamageChangedEvent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.FixedPoint;
using Robust.Shared.GameObjects;

#nullable enable
namespace Content.Shared.Damage;

public sealed class DamageChangedEvent : EntityEventArgs
{
  public readonly DamageableComponent Damageable;
  public readonly DamageSpecifier? DamageDelta;
  public readonly bool DamageIncreased;
  public readonly bool InterruptsDoAfters;
  public readonly EntityUid? Origin;
  public readonly EntityUid? Tool;

  public DamageChangedEvent(
    DamageableComponent damageable,
    DamageSpecifier? damageDelta,
    bool interruptsDoAfters,
    EntityUid? origin,
    EntityUid? tool)
  {
    this.Damageable = damageable;
    this.DamageDelta = damageDelta;
    this.Origin = origin;
    this.Tool = tool;
    if (this.DamageDelta == null)
      return;
    foreach (FixedPoint2 fixedPoint2 in this.DamageDelta.DamageDict.Values)
    {
      if (fixedPoint2 > 0)
      {
        this.DamageIncreased = true;
        break;
      }
    }
    this.InterruptsDoAfters = interruptsDoAfters && this.DamageIncreased;
  }
}
