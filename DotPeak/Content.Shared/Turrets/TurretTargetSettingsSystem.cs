// Decompiled with JetBrains decompiler
// Type: Content.Shared.Turrets.TurretTargetSettingsSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Access;
using Content.Shared.Access.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared.Turrets;

public sealed class TurretTargetSettingsSystem : EntitySystem
{
  [Dependency]
  private AccessReaderSystem _accessReader;
  private ProtoId<AccessLevelPrototype> _accessLevelBorg = (ProtoId<AccessLevelPrototype>) "Borg";
  private ProtoId<AccessLevelPrototype> _accessLevelBasicSilicon = (ProtoId<AccessLevelPrototype>) "BasicSilicon";

  public void SetAccessLevelExemption(
    Entity<TurretTargetSettingsComponent> ent,
    ProtoId<AccessLevelPrototype> exemption,
    bool enabled,
    bool dirty = true)
  {
    if (enabled)
      ent.Comp.ExemptAccessLevels.Add(exemption);
    else
      ent.Comp.ExemptAccessLevels.Remove(exemption);
    if (!dirty)
      return;
    this.Dirty<TurretTargetSettingsComponent>(ent);
  }

  public void SetAccessLevelExemptions(
    Entity<TurretTargetSettingsComponent> ent,
    ICollection<ProtoId<AccessLevelPrototype>> exemptions,
    bool enabled)
  {
    foreach (ProtoId<AccessLevelPrototype> exemption in (IEnumerable<ProtoId<AccessLevelPrototype>>) exemptions)
      this.SetAccessLevelExemption(ent, exemption, enabled, false);
    this.Dirty<TurretTargetSettingsComponent>(ent);
  }

  public void SyncAccessLevelExemptions(
    Entity<TurretTargetSettingsComponent> ent,
    ICollection<ProtoId<AccessLevelPrototype>> exemptions)
  {
    ent.Comp.ExemptAccessLevels.Clear();
    this.SetAccessLevelExemptions(ent, exemptions, true);
  }

  public void SyncAccessLevelExemptions(
    Entity<TurretTargetSettingsComponent> target,
    Entity<TurretTargetSettingsComponent> source)
  {
    this.SyncAccessLevelExemptions(target, (ICollection<ProtoId<AccessLevelPrototype>>) source.Comp.ExemptAccessLevels);
  }

  public bool HasAccessLevelExemption(
    Entity<TurretTargetSettingsComponent> ent,
    ProtoId<AccessLevelPrototype> exemption)
  {
    return ent.Comp.ExemptAccessLevels.Count != 0 && ent.Comp.ExemptAccessLevels.Contains(exemption);
  }

  public bool HasAnyAccessLevelExemption(
    Entity<TurretTargetSettingsComponent> ent,
    ICollection<ProtoId<AccessLevelPrototype>> exemptions)
  {
    if (ent.Comp.ExemptAccessLevels.Count == 0)
      return false;
    foreach (ProtoId<AccessLevelPrototype> exemption in (IEnumerable<ProtoId<AccessLevelPrototype>>) exemptions)
    {
      if (this.HasAccessLevelExemption(ent, exemption))
        return true;
    }
    return false;
  }

  public bool EntityIsTargetForTurret(Entity<TurretTargetSettingsComponent> ent, EntityUid target)
  {
    ICollection<ProtoId<AccessLevelPrototype>> accessTags = this._accessReader.FindAccessTags(target);
    if (accessTags.Contains(this._accessLevelBorg))
      return !this.HasAccessLevelExemption(ent, this._accessLevelBorg);
    return accessTags.Contains(this._accessLevelBasicSilicon) ? !this.HasAccessLevelExemption(ent, this._accessLevelBasicSilicon) : !this.HasAnyAccessLevelExemption(ent, accessTags);
  }
}
