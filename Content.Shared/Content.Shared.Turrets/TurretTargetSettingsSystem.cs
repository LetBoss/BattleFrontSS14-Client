using System.Collections.Generic;
using Content.Shared.Access;
using Content.Shared.Access.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;

namespace Content.Shared.Turrets;

public sealed class TurretTargetSettingsSystem : EntitySystem
{
	[Dependency]
	private AccessReaderSystem _accessReader;

	private ProtoId<AccessLevelPrototype> _accessLevelBorg = ProtoId<AccessLevelPrototype>.op_Implicit("Borg");

	private ProtoId<AccessLevelPrototype> _accessLevelBasicSilicon = ProtoId<AccessLevelPrototype>.op_Implicit("BasicSilicon");

	public void SetAccessLevelExemption(Entity<TurretTargetSettingsComponent> ent, ProtoId<AccessLevelPrototype> exemption, bool enabled, bool dirty = true)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		if (enabled)
		{
			ent.Comp.ExemptAccessLevels.Add(exemption);
		}
		else
		{
			ent.Comp.ExemptAccessLevels.Remove(exemption);
		}
		if (dirty)
		{
			((EntitySystem)this).Dirty<TurretTargetSettingsComponent>(ent, (MetaDataComponent)null);
		}
	}

	public void SetAccessLevelExemptions(Entity<TurretTargetSettingsComponent> ent, ICollection<ProtoId<AccessLevelPrototype>> exemptions, bool enabled)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		foreach (ProtoId<AccessLevelPrototype> exemption in exemptions)
		{
			SetAccessLevelExemption(ent, exemption, enabled, dirty: false);
		}
		((EntitySystem)this).Dirty<TurretTargetSettingsComponent>(ent, (MetaDataComponent)null);
	}

	public void SyncAccessLevelExemptions(Entity<TurretTargetSettingsComponent> ent, ICollection<ProtoId<AccessLevelPrototype>> exemptions)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		ent.Comp.ExemptAccessLevels.Clear();
		SetAccessLevelExemptions(ent, exemptions, enabled: true);
	}

	public void SyncAccessLevelExemptions(Entity<TurretTargetSettingsComponent> target, Entity<TurretTargetSettingsComponent> source)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		SyncAccessLevelExemptions(target, (ICollection<ProtoId<AccessLevelPrototype>>)source.Comp.ExemptAccessLevels);
	}

	public bool HasAccessLevelExemption(Entity<TurretTargetSettingsComponent> ent, ProtoId<AccessLevelPrototype> exemption)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		if (ent.Comp.ExemptAccessLevels.Count == 0)
		{
			return false;
		}
		return ent.Comp.ExemptAccessLevels.Contains(exemption);
	}

	public bool HasAnyAccessLevelExemption(Entity<TurretTargetSettingsComponent> ent, ICollection<ProtoId<AccessLevelPrototype>> exemptions)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		if (ent.Comp.ExemptAccessLevels.Count == 0)
		{
			return false;
		}
		foreach (ProtoId<AccessLevelPrototype> exemption in exemptions)
		{
			if (HasAccessLevelExemption(ent, exemption))
			{
				return true;
			}
		}
		return false;
	}

	public bool EntityIsTargetForTurret(Entity<TurretTargetSettingsComponent> ent, EntityUid target)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		ICollection<ProtoId<AccessLevelPrototype>> accessLevels = _accessReader.FindAccessTags(target);
		if (accessLevels.Contains(_accessLevelBorg))
		{
			return !HasAccessLevelExemption(ent, _accessLevelBorg);
		}
		if (accessLevels.Contains(_accessLevelBasicSilicon))
		{
			return !HasAccessLevelExemption(ent, _accessLevelBasicSilicon);
		}
		return !HasAnyAccessLevelExemption(ent, accessLevels);
	}
}
