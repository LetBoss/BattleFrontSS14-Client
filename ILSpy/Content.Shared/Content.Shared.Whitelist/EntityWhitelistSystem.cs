using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Content.Shared._RMC14.Marines.Skills;
using Content.Shared._RMC14.Stun;
using Content.Shared.Item;
using Content.Shared.Tag;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Shared.Whitelist;

public sealed class EntityWhitelistSystem : EntitySystem
{
	[Dependency]
	private TagSystem _tag;

	private EntityQuery<ItemComponent> _itemQuery;

	[Dependency]
	private SkillsSystem _skills;

	[Dependency]
	private RMCSizeStunSystem _rmcSizeStun;

	public override void Initialize()
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).Initialize();
		_itemQuery = ((EntitySystem)this).GetEntityQuery<ItemComponent>();
	}

	public bool IsValid(EntityWhitelist list, [NotNullWhen(true)] EntityUid? uid)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		if (uid.HasValue)
		{
			return IsValid(list, uid.Value);
		}
		return false;
	}

	public bool CheckBoth([NotNullWhen(true)] EntityUid? uid, EntityWhitelist? blacklist = null, EntityWhitelist? whitelist = null)
	{
		if (!uid.HasValue)
		{
			return false;
		}
		if (blacklist != null && IsValid(blacklist, uid))
		{
			return false;
		}
		if (whitelist != null)
		{
			return IsValid(whitelist, uid);
		}
		return true;
	}

	public bool IsValid(EntityWhitelist list, EntityUid uid)
	{
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		if (list.Components != null && list.Registrations == null)
		{
			List<ComponentRegistration> regs = StringsToRegs(list.Components);
			list.Registrations = new List<ComponentRegistration>();
			list.Registrations.AddRange(regs);
		}
		if (list.Registrations != null && list.Registrations.Count > 0)
		{
			foreach (ComponentRegistration reg in list.Registrations)
			{
				if (((EntitySystem)this).HasComp(uid, reg.Type))
				{
					if (!list.RequireAll)
					{
						return true;
					}
				}
				else if (list.RequireAll)
				{
					return false;
				}
			}
		}
		ItemComponent itemComp = default(ItemComponent);
		if (list.Sizes != null && _itemQuery.TryComp(uid, ref itemComp) && list.Sizes.Contains(itemComp.Size))
		{
			return true;
		}
		if (list.Tags != null)
		{
			if (!list.RequireAll)
			{
				return _tag.HasAnyTag(uid, list.Tags);
			}
			return _tag.HasAllTags(uid, list.Tags);
		}
		if (list.Skills != null)
		{
			if (!list.RequireAll)
			{
				return _skills.HasAnySkills(Entity<SkillsComponent>.op_Implicit(uid), list.Skills);
			}
			return _skills.HasAllSkills(Entity<SkillsComponent>.op_Implicit(uid), list.Skills);
		}
		if (list.MinMobSize.HasValue)
		{
			return _rmcSizeStun.IsXenoSized(Entity<RMCSizeComponent>.op_Implicit(uid));
		}
		return list.RequireAll;
	}

	public bool IsWhitelistPass(EntityWhitelist? whitelist, EntityUid uid)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		if (whitelist == null)
		{
			return false;
		}
		return IsValid(whitelist, uid);
	}

	public bool IsWhitelistFail(EntityWhitelist? whitelist, EntityUid uid)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		if (whitelist == null)
		{
			return false;
		}
		return !IsValid(whitelist, uid);
	}

	public bool IsWhitelistPassOrNull(EntityWhitelist? whitelist, EntityUid uid)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		if (whitelist == null)
		{
			return true;
		}
		return IsValid(whitelist, uid);
	}

	public bool IsWhitelistFailOrNull(EntityWhitelist? whitelist, EntityUid uid)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		if (whitelist == null)
		{
			return true;
		}
		return !IsValid(whitelist, uid);
	}

	public bool IsBlacklistPass(EntityWhitelist? blacklist, EntityUid uid)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		return IsWhitelistPass(blacklist, uid);
	}

	public bool IsBlacklistFail(EntityWhitelist? blacklist, EntityUid uid)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		return IsWhitelistFail(blacklist, uid);
	}

	public bool IsBlacklistPassOrNull(EntityWhitelist? blacklist, EntityUid uid)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		return IsWhitelistPassOrNull(blacklist, uid);
	}

	public bool IsBlacklistFailOrNull(EntityWhitelist? blacklist, EntityUid uid)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		return IsWhitelistFailOrNull(blacklist, uid);
	}

	private List<ComponentRegistration> StringsToRegs(string[]? input)
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Invalid comparison between Unknown and I4
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		List<ComponentRegistration> list = new List<ComponentRegistration>();
		if (input == null || input.Length == 0)
		{
			return list;
		}
		ComponentRegistration registration = default(ComponentRegistration);
		foreach (string name in input)
		{
			ComponentAvailability availability = ((EntitySystem)this).Factory.GetComponentAvailability(name, false);
			if (((EntitySystem)this).Factory.TryGetRegistration(name, ref registration, false) && (int)availability == 0)
			{
				list.Add(registration);
			}
			else if ((int)availability == 2)
			{
				((EntitySystem)this).Log.Error("StringsToRegs failed: Unknown component name " + name + " passed to EntityWhitelist!");
			}
		}
		return list;
	}
}
