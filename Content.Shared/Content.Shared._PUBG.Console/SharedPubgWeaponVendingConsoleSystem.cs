using System.Collections.Generic;
using Content.Shared._PUBG.Ammo.Components;
using Content.Shared.Tag;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;

namespace Content.Shared._PUBG.Console;

public abstract class SharedPubgWeaponVendingConsoleSystem : EntitySystem
{
	[Dependency]
	private IPrototypeManager _protoManager;

	[Dependency]
	private IComponentFactory _compFactory;

	private Dictionary<string, string>? _ammoBoxCache;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
	}

	public string? GetAmmoBoxForWeapon(EntityPrototype weaponProto)
	{
		PubgAmmoProviderComponent ammoComp = default(PubgAmmoProviderComponent);
		if (!weaponProto.TryGetComponent<PubgAmmoProviderComponent>(ref ammoComp, _compFactory))
		{
			return null;
		}
		string ammoTag = ammoComp.AmmoTag;
		if (string.IsNullOrEmpty(ammoTag))
		{
			return null;
		}
		if (_ammoBoxCache == null)
		{
			_ammoBoxCache = BuildAmmoBoxCache();
		}
		if (!_ammoBoxCache.TryGetValue(ammoTag, out string boxProto))
		{
			return null;
		}
		return boxProto;
	}

	private Dictionary<string, string> BuildAmmoBoxCache()
	{
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		Dictionary<string, string> cache = new Dictionary<string, string>();
		TagComponent tagComp = default(TagComponent);
		foreach (EntityPrototype proto in _protoManager.EnumeratePrototypes<EntityPrototype>())
		{
			if (!proto.TryGetComponent<TagComponent>(ref tagComp, _compFactory))
			{
				continue;
			}
			foreach (ProtoId<TagPrototype> tag2 in tagComp.Tags)
			{
				string tagId = ((object)tag2/*cast due to constrained. prefix*/).ToString();
				if (tagId.StartsWith("Ammo") && tagId.EndsWith("Pubg"))
				{
					cache.TryAdd(tagId, proto.ID);
				}
			}
		}
		return cache;
	}
}
