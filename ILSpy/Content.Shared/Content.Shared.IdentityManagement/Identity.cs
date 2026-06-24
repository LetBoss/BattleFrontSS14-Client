using Content.Shared._RMC14.IdentityManagement;
using Content.Shared.Ghost;
using Content.Shared.IdentityManagement.Components;
using Content.Shared.Whitelist;
using Robust.Shared.GameObjects;
using Robust.Shared.Localization;

namespace Content.Shared.IdentityManagement;

public static class Identity
{
	public static IdentityEntity Name(EntityUid uid, IEntityManager ent, EntityUid? viewer = null)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Invalid comparison between Unknown and I4
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntityUid)(ref uid)).IsValid())
		{
			return new IdentityEntity(uid, string.Empty);
		}
		MetaDataComponent meta = ent.GetComponent<MetaDataComponent>(uid);
		if ((int)meta.EntityLifeStage <= 1)
		{
			return new IdentityEntity(uid, meta.EntityName);
		}
		string uidName = meta.EntityName;
		EntityWhitelistSystem whitelistSystem = ent.System<EntityWhitelistSystem>();
		FixedIdentityComponent fixedIdentity = default(FixedIdentityComponent);
		if (viewer.HasValue && ent.TryGetComponent<FixedIdentityComponent>(uid, ref fixedIdentity))
		{
			LocId? name = fixedIdentity.Name;
			if (name.HasValue)
			{
				LocId nameId = name.GetValueOrDefault();
				if (whitelistSystem.IsWhitelistPass(fixedIdentity.Whitelist, viewer.Value))
				{
					string name2 = Loc.GetString(LocId.op_Implicit(nameId));
					RMCGetFixedIdentityEvent ev = new RMCGetFixedIdentityEvent(name2);
					((IDirectedEventBus)ent.EventBus).RaiseLocalEvent<RMCGetFixedIdentityEvent>(uid, ref ev, false);
					return new IdentityEntity(uid, ev.Name);
				}
			}
		}
		IdentityComponent identity = default(IdentityComponent);
		if (!ent.TryGetComponent<IdentityComponent>(uid, ref identity))
		{
			return new IdentityEntity(uid, uidName);
		}
		EntityUid? ident = identity.IdentityEntitySlot.ContainedEntity;
		if (!ident.HasValue)
		{
			return new IdentityEntity(uid, uidName);
		}
		string identName = ent.GetComponent<MetaDataComponent>(ident.Value).EntityName;
		if (!viewer.HasValue || !CanSeeThroughIdentity(uid, viewer.Value, ent))
		{
			return new IdentityEntity(uid, identName);
		}
		if (uidName == identName)
		{
			return new IdentityEntity(uid, uidName);
		}
		return new IdentityEntity(uid, uidName + " (" + identName + ")");
	}

	public static EntityUid Entity(EntityUid uid, IEntityManager ent, EntityUid? viewer = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		IdentityComponent identity = default(IdentityComponent);
		if (!ent.TryGetComponent<IdentityComponent>(uid, ref identity))
		{
			return uid;
		}
		if (viewer.HasValue && CanSeeThroughIdentity(uid, viewer.Value, ent))
		{
			return uid;
		}
		return identity.IdentityEntitySlot.ContainedEntity ?? uid;
	}

	public static bool CanSeeThroughIdentity(EntityUid uid, EntityUid viewer, IEntityManager ent)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		return ent.HasComponent<GhostComponent>(viewer);
	}
}
