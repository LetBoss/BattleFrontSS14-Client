using System;
using System.Collections.Generic;
using System.Numerics;
using Content.Client._PUBG.Party;
using Content.Shared._PUBG.Ammo.Components;
using Content.Shared._PUBG.FogOfWar;
using Content.Shared._PUBG.Medicine;
using Content.Shared._PUBG.Party;
using Content.Shared._PUBG.Skin;
using Content.Shared.Armor;
using Content.Shared.Clothing.Components;
using Content.Shared.Humanoid;
using Content.Shared.Storage;
using Content.Shared.Tag;
using Content.Shared.Weapons.Ranged.Components;
using Robust.Client.GameObjects;
using Robust.Client.Player;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;

namespace Content.Client._PUBG.FogOfWar;

public sealed class PubgFogOfWarHideSystem : EntitySystem
{
	[Dependency]
	private IPlayerManager _player;

	[Dependency]
	private TagSystem _tags;

	[Dependency]
	private EntityLookupSystem _lookup;

	[Dependency]
	private SharedTransformSystem _xform;

	[Dependency]
	private PubgFogOfWarSystem _fogSystem;

	[Dependency]
	private PubgFovModifierSystem _fovModifiers;

	[Dependency]
	private IGameTiming _timing;

	[Dependency]
	private PubgPartyClientSystem _party;

	public readonly List<(Entity<SpriteComponent?> Ent, float BaseAlpha)> CachedBaseAlphas = new List<(Entity<SpriteComponent>, float)>(128);

	private readonly HashSet<EntityUid> _revealed = new HashSet<EntityUid>();

	private readonly HashSet<EntityUid> _shouldHide = new HashSet<EntityUid>();

	private readonly Dictionary<EntityUid, TimeSpan> _lastSeen = new Dictionary<EntityUid, TimeSpan>();

	private readonly HashSet<Entity<OccluderComponent>> _occluders = new HashSet<Entity<OccluderComponent>>();

	private readonly List<Box2> _occluderBoxes = new List<Box2>();

	private Vector2 _origin;

	private float _range;

	private float _cosHalfFov;

	private Vector2 _forward;

	private MapId _mapId;

	private EntityUid _playerEntity;

	private bool _prepared;

	private TimeSpan _nextOccludableRefresh;

	private static readonly TimeSpan OccludableRefreshInterval = TimeSpan.FromSeconds(1L);

	private static readonly TimeSpan MemoryFadeDuration = TimeSpan.FromSeconds(2L);

	private static readonly HashSet<ProtoId<TagPrototype>> AmmoTags = new HashSet<ProtoId<TagPrototype>>
	{
		ProtoId<TagPrototype>.op_Implicit("Magazine762Pubg"),
		ProtoId<TagPrototype>.op_Implicit("Cartridge762Pubg"),
		ProtoId<TagPrototype>.op_Implicit("Ammo762Pubg"),
		ProtoId<TagPrototype>.op_Implicit("Ammo5-56x45mmPubg"),
		ProtoId<TagPrototype>.op_Implicit("CartridgeRifle5-56x45mm"),
		ProtoId<TagPrototype>.op_Implicit("Ammo7-62x51mmPubg"),
		ProtoId<TagPrototype>.op_Implicit("CartridgeRifle7-62x51mm"),
		ProtoId<TagPrototype>.op_Implicit("Ammo9x39mmPubg"),
		ProtoId<TagPrototype>.op_Implicit("CartridgeRifle9x39mm"),
		ProtoId<TagPrototype>.op_Implicit("Ammo7-62x39mmPubg"),
		ProtoId<TagPrototype>.op_Implicit("CartridgeRifle7-62x39mm"),
		ProtoId<TagPrototype>.op_Implicit("Ammo30-06sprPubg"),
		ProtoId<TagPrototype>.op_Implicit("CartridgeRifle30-06spr"),
		ProtoId<TagPrototype>.op_Implicit("Ammo12x55mmPubg"),
		ProtoId<TagPrototype>.op_Implicit("CartridgeRifle12x55mm"),
		ProtoId<TagPrototype>.op_Implicit("Ammo5-7x28mmPubg"),
		ProtoId<TagPrototype>.op_Implicit("CartridgeSmg5-7x28mm"),
		ProtoId<TagPrototype>.op_Implicit("Ammo9mmPubg"),
		ProtoId<TagPrototype>.op_Implicit("CartridgePistol9mm"),
		ProtoId<TagPrototype>.op_Implicit("Ammo7-65mmPubg"),
		ProtoId<TagPrototype>.op_Implicit("CartridgePistol7-65mm"),
		ProtoId<TagPrototype>.op_Implicit("Ammo12-7x33mmPubg"),
		ProtoId<TagPrototype>.op_Implicit("CartridgePistol12-7x33mm"),
		ProtoId<TagPrototype>.op_Implicit("Ammo357mmPubg"),
		ProtoId<TagPrototype>.op_Implicit("CartridgeRevolverMagnum357"),
		ProtoId<TagPrototype>.op_Implicit("Ammo500mmPubg"),
		ProtoId<TagPrototype>.op_Implicit("CartridgeRevolverMagnum500"),
		ProtoId<TagPrototype>.op_Implicit("Ammo12x70Pubg"),
		ProtoId<TagPrototype>.op_Implicit("ShellShotgun12x70"),
		ProtoId<TagPrototype>.op_Implicit("Ammo23x75Pubg"),
		ProtoId<TagPrototype>.op_Implicit("ShellShotgun23x75"),
		ProtoId<TagPrototype>.op_Implicit("CartridgeGL-40mm")
	};

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		RefreshOccludables();
	}

	public bool TryPrepare()
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		_prepared = false;
		_playerEntity = EntityUid.Invalid;
		if (_timing.RealTime >= _nextOccludableRefresh)
		{
			RefreshOccludables();
			_nextOccludableRefresh = _timing.RealTime + OccludableRefreshInterval;
		}
		EntityUid? localEntity = ((ISharedPlayerManager)_player).LocalEntity;
		if (!localEntity.HasValue || !_fogSystem.Active)
		{
			return false;
		}
		TransformComponent val = default(TransformComponent);
		PubgFogOfWarComponent pubgFogOfWarComponent = default(PubgFogOfWarComponent);
		if (!((EntitySystem)this).TryComp(localEntity.Value, ref val) || !((EntitySystem)this).TryComp<PubgFogOfWarComponent>(localEntity.Value, ref pubgFogOfWarComponent))
		{
			return false;
		}
		if (!pubgFogOfWarComponent.Enabled)
		{
			return false;
		}
		_playerEntity = localEntity.Value;
		_mapId = val.MapID;
		_origin = _xform.GetWorldPosition(val);
		_range = MathF.Max(0.1f, pubgFogOfWarComponent.Range);
		float effectiveFov = _fovModifiers.GetEffectiveFov(localEntity.Value, pubgFogOfWarComponent);
		_cosHalfFov = MathF.Cos(MathHelper.DegreesToRadians(effectiveFov * 0.5f));
		Angle val2 = (pubgFogOfWarComponent.DesiredViewAngle.HasValue ? pubgFogOfWarComponent.CurrentAngle : _xform.GetWorldRotation(val));
		_forward = ((Angle)(ref val2)).ToWorldVec();
		BuildOccluderCache(_mapId, _origin, _range);
		_prepared = true;
		return true;
	}

	private void RefreshOccludables()
	{
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		_shouldHide.Clear();
		MarkWithComponent<PubgSkinComponent>();
		MarkWithComponent<HumanoidAppearanceComponent>();
		MarkAmmoTags();
		MarkWithComponent<PubgAmmoProviderComponent>();
		MarkWithComponent<PubgMedicalComponent>();
		MarkWithComponent<PubgEnergyDrinkComponent>();
		MarkWithComponent<GunComponent>();
		MarkWithComponent<ArmorComponent>();
		MarkStorageClothing();
		EntityQueryEnumerator<PubgFogOfWarOccludableComponent> val = ((EntitySystem)this).EntityQueryEnumerator<PubgFogOfWarOccludableComponent>();
		EntityUid val2 = default(EntityUid);
		PubgFogOfWarOccludableComponent pubgFogOfWarOccludableComponent = default(PubgFogOfWarOccludableComponent);
		while (val.MoveNext(ref val2, ref pubgFogOfWarOccludableComponent))
		{
			if (!_shouldHide.Contains(val2) || !((EntitySystem)this).HasComp<SpriteComponent>(val2))
			{
				((EntitySystem)this).RemComp<PubgFogOfWarOccludableComponent>(val2);
			}
		}
		foreach (EntityUid item in _shouldHide)
		{
			if (((EntitySystem)this).HasComp<SpriteComponent>(item))
			{
				((EntitySystem)this).EnsureComp<PubgFogOfWarOccludableComponent>(item);
			}
		}
		CleanupRevealedEntries();
		CleanupMemoryEntries();
	}

	private void MarkWithComponent<T>() where T : Component
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		EntityQueryEnumerator<T, SpriteComponent> val = ((EntitySystem)this).EntityQueryEnumerator<T, SpriteComponent>();
		EntityUid item = default(EntityUid);
		T val2 = default(T);
		SpriteComponent val3 = default(SpriteComponent);
		while (val.MoveNext(ref item, ref val2, ref val3))
		{
			_shouldHide.Add(item);
		}
	}

	private void MarkAmmoTags()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		EntityQueryEnumerator<TagComponent, SpriteComponent> val = ((EntitySystem)this).EntityQueryEnumerator<TagComponent, SpriteComponent>();
		EntityUid item = default(EntityUid);
		TagComponent component = default(TagComponent);
		SpriteComponent val2 = default(SpriteComponent);
		while (val.MoveNext(ref item, ref component, ref val2))
		{
			if (_tags.HasAnyTag(component, AmmoTags))
			{
				_shouldHide.Add(item);
			}
		}
	}

	private void MarkStorageClothing()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		EntityQueryEnumerator<StorageComponent, ClothingComponent, SpriteComponent> val = ((EntitySystem)this).EntityQueryEnumerator<StorageComponent, ClothingComponent, SpriteComponent>();
		EntityUid item = default(EntityUid);
		StorageComponent storageComponent = default(StorageComponent);
		ClothingComponent clothingComponent = default(ClothingComponent);
		SpriteComponent val2 = default(SpriteComponent);
		while (val.MoveNext(ref item, ref storageComponent, ref clothingComponent, ref val2))
		{
			_shouldHide.Add(item);
		}
	}

	public float GetTargetAlpha(EntityUid uid, SpriteComponent sprite, TransformComponent xform)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		if (!_prepared)
		{
			return sprite.Color.A;
		}
		if (uid == _playerEntity)
		{
			return sprite.Color.A;
		}
		if (IsPartyMember(uid))
		{
			return sprite.Color.A;
		}
		if (xform.MapID != _mapId || xform.Anchored)
		{
			return sprite.Color.A;
		}
		if (!ShouldHide(uid))
		{
			return sprite.Color.A;
		}
		bool flag = ShouldRevealPersist(uid);
		if (flag && _revealed.Contains(uid))
		{
			return sprite.Color.A;
		}
		Vector2 worldPosition = _xform.GetWorldPosition(xform);
		if (!IsVisible(_origin, worldPosition, _forward, _cosHalfFov, _range))
		{
			if (!IsMemoryTarget(uid))
			{
				return 0f;
			}
			if (!_lastSeen.TryGetValue(uid, out var value))
			{
				return 0f;
			}
			TimeSpan timeSpan = _timing.CurTime - value;
			if (timeSpan <= TimeSpan.Zero)
			{
				return sprite.Color.A;
			}
			if (timeSpan >= MemoryFadeDuration)
			{
				return 0f;
			}
			float num = 1f - (float)(timeSpan.TotalSeconds / MemoryFadeDuration.TotalSeconds);
			return sprite.Color.A * num;
		}
		if (flag)
		{
			_revealed.Add(uid);
		}
		if (IsMemoryTarget(uid))
		{
			_lastSeen[uid] = _timing.CurTime;
		}
		return sprite.Color.A;
	}

	private bool IsPartyMember(EntityUid uid)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		if (_party.Members.Count == 0)
		{
			return false;
		}
		NetEntity netEntity = ((EntitySystem)this).GetNetEntity(uid, (MetaDataComponent)null);
		foreach (PubgPartyMemberState member in _party.Members)
		{
			if (member.Entity == netEntity)
			{
				return true;
			}
		}
		return false;
	}

	private void BuildOccluderCache(MapId mapId, Vector2 origin, float range)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		_occluders.Clear();
		_occluderBoxes.Clear();
		_lookup.GetEntitiesInRange<OccluderComponent>(mapId, origin, range, _occluders, (LookupFlags)5);
		EntityUid val = default(EntityUid);
		OccluderComponent val2 = default(OccluderComponent);
		TransformComponent val5 = default(TransformComponent);
		foreach (Entity<OccluderComponent> occluder in _occluders)
		{
			occluder.Deconstruct(ref val, ref val2);
			EntityUid val3 = val;
			OccluderComponent val4 = val2;
			if (val4.Enabled && ((EntitySystem)this).TryComp(val3, ref val5) && !(val5.MapID != mapId) && val5.Anchored)
			{
				Box2 item = Matrix3Helpers.TransformBox(_xform.GetWorldMatrix(val3), ref val4.BoundingBox);
				_occluderBoxes.Add(item);
			}
		}
	}

	private bool IsVisible(Vector2 origin, Vector2 target, Vector2 forward, float cosHalfFov, float range)
	{
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		Vector2 vector = target - origin;
		float num = vector.LengthSquared();
		if (num > range * range)
		{
			return false;
		}
		if (num > 0.0001f)
		{
			Vector2 value = vector / MathF.Sqrt(num);
			if (Vector2.Dot(forward, value) < cosHalfFov)
			{
				return false;
			}
		}
		float num2 = MathF.Sqrt(num);
		if (num2 <= 0.001f)
		{
			return true;
		}
		Vector2 dir = vector / num2;
		for (int i = 0; i < _occluderBoxes.Count; i++)
		{
			if (RayAabb(origin, dir, _occluderBoxes[i], out var distance) && distance >= 0f && distance < num2 - 0.05f)
			{
				return false;
			}
		}
		return true;
	}

	private bool ShouldHide(EntityUid uid)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).HasComp<PubgSkinComponent>(uid) || ((EntitySystem)this).HasComp<HumanoidAppearanceComponent>(uid))
		{
			return true;
		}
		TagComponent component = default(TagComponent);
		if (((EntitySystem)this).TryComp<TagComponent>(uid, ref component) && _tags.HasAnyTag(component, AmmoTags))
		{
			return true;
		}
		if (((EntitySystem)this).HasComp<PubgAmmoProviderComponent>(uid))
		{
			return true;
		}
		if (((EntitySystem)this).HasComp<PubgMedicalComponent>(uid) || ((EntitySystem)this).HasComp<PubgEnergyDrinkComponent>(uid))
		{
			return true;
		}
		if (((EntitySystem)this).HasComp<GunComponent>(uid))
		{
			return true;
		}
		if (((EntitySystem)this).HasComp<ArmorComponent>(uid))
		{
			return true;
		}
		if (((EntitySystem)this).HasComp<StorageComponent>(uid) && ((EntitySystem)this).HasComp<ClothingComponent>(uid))
		{
			return true;
		}
		return false;
	}

	private bool ShouldRevealPersist(EntityUid uid)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).HasComp<PubgSkinComponent>(uid) || ((EntitySystem)this).HasComp<HumanoidAppearanceComponent>(uid))
		{
			return false;
		}
		TagComponent component = default(TagComponent);
		if (((EntitySystem)this).TryComp<TagComponent>(uid, ref component) && _tags.HasAnyTag(component, AmmoTags))
		{
			return true;
		}
		((EntitySystem)this).HasComp<PubgAmmoProviderComponent>(uid);
		return true;
	}

	private bool IsMemoryTarget(EntityUid uid)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).HasComp<HumanoidAppearanceComponent>(uid))
		{
			return ((EntitySystem)this).HasComp<PubgSkinComponent>(uid);
		}
		return true;
	}

	private void CleanupMemoryEntries()
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		if (_lastSeen.Count == 0)
		{
			return;
		}
		List<EntityUid> list = new List<EntityUid>();
		foreach (EntityUid key in _lastSeen.Keys)
		{
			if (!((EntitySystem)this).Exists(key) || !IsMemoryTarget(key))
			{
				list.Add(key);
			}
		}
		for (int i = 0; i < list.Count; i++)
		{
			_lastSeen.Remove(list[i]);
		}
	}

	private void CleanupRevealedEntries()
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		if (_revealed.Count == 0)
		{
			return;
		}
		List<EntityUid> list = new List<EntityUid>();
		foreach (EntityUid item in _revealed)
		{
			if (!((EntitySystem)this).Exists(item) || !ShouldRevealPersist(item))
			{
				list.Add(item);
			}
		}
		for (int i = 0; i < list.Count; i++)
		{
			_revealed.Remove(list[i]);
		}
	}

	private static bool RayAabb(Vector2 origin, Vector2 dir, Box2 box, out float distance)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		distance = 0f;
		float tmin = 0f;
		float tmax = float.PositiveInfinity;
		if (!RaySlab(origin.X, dir.X, box.Left, box.Right, ref tmin, ref tmax))
		{
			return false;
		}
		if (!RaySlab(origin.Y, dir.Y, box.Bottom, box.Top, ref tmin, ref tmax))
		{
			return false;
		}
		if (tmax < 0f)
		{
			return false;
		}
		distance = ((tmin >= 0f) ? tmin : tmax);
		return distance >= 0f;
	}

	private static bool RaySlab(float origin, float dir, float min, float max, ref float tmin, ref float tmax)
	{
		if (MathF.Abs(dir) < 0.0001f)
		{
			if (origin < min || origin > max)
			{
				return false;
			}
			return true;
		}
		float num = 1f / dir;
		float num2 = (min - origin) * num;
		float num3 = (max - origin) * num;
		if (num2 > num3)
		{
			float num4 = num2;
			num2 = num3;
			num3 = num4;
		}
		if (num2 > tmin)
		{
			tmin = num2;
		}
		if (num3 < tmax)
		{
			tmax = num3;
		}
		return tmax >= tmin;
	}
}
