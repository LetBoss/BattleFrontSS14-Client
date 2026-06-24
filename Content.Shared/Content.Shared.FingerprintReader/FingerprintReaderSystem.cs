using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Content.Shared.Forensics.Components;
using Content.Shared.Inventory;
using Content.Shared.Popups;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;

namespace Content.Shared.FingerprintReader;

public sealed class FingerprintReaderSystem : EntitySystem
{
	[Dependency]
	private InventorySystem _inventory;

	[Dependency]
	private SharedPopupSystem _popup;

	public bool IsAllowed(Entity<FingerprintReaderComponent?> target, EntityUid user, bool showPopup = true)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<FingerprintReaderComponent>(Entity<FingerprintReaderComponent>.op_Implicit(target), ref target.Comp, false))
		{
			return true;
		}
		if (target.Comp.AllowedFingerprints.Count == 0)
		{
			return true;
		}
		if (!target.Comp.IgnoreGloves && TryGetBlockingGloves(user, out var gloves))
		{
			if (target.Comp.FailGlovesPopup.HasValue && showPopup)
			{
				SharedPopupSystem popup = _popup;
				ILocalizationManager loc = base.Loc;
				LocId? failGlovesPopup = target.Comp.FailGlovesPopup;
				popup.PopupClient(loc.GetString(failGlovesPopup.HasValue ? LocId.op_Implicit(failGlovesPopup.GetValueOrDefault()) : null, (ValueTuple<string, object>)("blocker", gloves)), Entity<FingerprintReaderComponent>.op_Implicit(target), user);
			}
			return false;
		}
		FingerprintComponent fingerprint = default(FingerprintComponent);
		if (!((EntitySystem)this).TryComp<FingerprintComponent>(user, ref fingerprint) || fingerprint.Fingerprint == null || !target.Comp.AllowedFingerprints.Contains(fingerprint.Fingerprint))
		{
			if (target.Comp.FailPopup.HasValue && showPopup)
			{
				SharedPopupSystem popup2 = _popup;
				ILocalizationManager loc2 = base.Loc;
				LocId? failGlovesPopup = target.Comp.FailPopup;
				popup2.PopupClient(loc2.GetString(failGlovesPopup.HasValue ? LocId.op_Implicit(failGlovesPopup.GetValueOrDefault()) : null), Entity<FingerprintReaderComponent>.op_Implicit(target), user);
			}
			return false;
		}
		return true;
	}

	public bool TryGetBlockingGloves(EntityUid user, [NotNullWhen(true)] out EntityUid? blocker)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		blocker = null;
		if (_inventory.TryGetSlotEntity(user, "gloves", out var gloves) && ((EntitySystem)this).HasComp<FingerprintMaskComponent>(gloves))
		{
			blocker = gloves;
			return true;
		}
		return false;
	}

	public void SetAllowedFingerprints(Entity<FingerprintReaderComponent> target, HashSet<string> fingerprints)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		target.Comp.AllowedFingerprints = fingerprints;
		((EntitySystem)this).Dirty<FingerprintReaderComponent>(target, (MetaDataComponent)null);
	}

	public void AddAllowedFingerprint(Entity<FingerprintReaderComponent> target, string fingerprint)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		target.Comp.AllowedFingerprints.Add(fingerprint);
		((EntitySystem)this).Dirty<FingerprintReaderComponent>(target, (MetaDataComponent)null);
	}

	public void RemoveAllowedFingerprint(Entity<FingerprintReaderComponent> target, string fingerprint)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		target.Comp.AllowedFingerprints.Remove(fingerprint);
		((EntitySystem)this).Dirty<FingerprintReaderComponent>(target, (MetaDataComponent)null);
	}
}
