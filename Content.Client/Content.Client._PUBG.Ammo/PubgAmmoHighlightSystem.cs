using System;
using System.Collections.Generic;
using Content.Client.Interactable.Components;
using Content.Shared._PUBG.Ammo.Components;
using Content.Shared.Hands.Components;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Inventory;
using Content.Shared.Storage;
using Content.Shared.Tag;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Client.Player;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;

namespace Content.Client._PUBG.Ammo;

public sealed class PubgAmmoHighlightSystem : EntitySystem
{
	[Dependency]
	private IPlayerManager _player;

	[Dependency]
	private SharedHandsSystem _hands;

	[Dependency]
	private InventorySystem _inventory;

	[Dependency]
	private TagSystem _tags;

	[Dependency]
	private SharedContainerSystem _containers;

	[Dependency]
	private IPrototypeManager _prototypes;

	[Dependency]
	private IGameTiming _timing;

	private readonly Dictionary<EntityUid, ShaderInstance> _highlightShaders = new Dictionary<EntityUid, ShaderInstance>();

	private readonly Dictionary<EntityUid, ShaderInstance?> _previousShaders = new Dictionary<EntityUid, ShaderInstance>();

	private readonly HashSet<string> _ammoTags = new HashSet<string>();

	private readonly List<EntityUid> _removeScratch = new List<EntityUid>();

	private TimeSpan _nextUpdate;

	private static readonly Color HighlightColor = Color.FromHex((ReadOnlySpan<char>)"#FFD54F", (Color?)null);

	private const float HighlightWidth = 2f;

	private const float UpdateInterval = 0.5f;

	private static ShaderInstance? GetValidShader(ShaderInstance? shader)
	{
		if (shader == null || shader.Disposed)
		{
			return null;
		}
		return shader;
	}

	public override void Shutdown()
	{
		((EntitySystem)this).Shutdown();
		ClearOutlines();
	}

	public override void Update(float frameTime)
	{
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).Update(frameTime);
		if (_timing.CurTime < _nextUpdate)
		{
			return;
		}
		_nextUpdate = _timing.CurTime + TimeSpan.FromSeconds(0.5);
		EntityUid? localEntity = ((ISharedPlayerManager)_player).LocalEntity;
		if (!localEntity.HasValue)
		{
			ClearOutlines();
			return;
		}
		BuildAmmoTags(localEntity.Value);
		if (_ammoTags.Count == 0)
		{
			ClearOutlines();
		}
		else
		{
			ApplyOutlines(localEntity.Value);
		}
	}

	private void BuildAmmoTags(EntityUid local)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		_ammoTags.Clear();
		HandsComponent item = default(HandsComponent);
		if (((EntitySystem)this).TryComp<HandsComponent>(local, ref item))
		{
			foreach (EntityUid item2 in _hands.EnumerateHeld(Entity<HandsComponent>.op_Implicit((local, item))))
			{
				AddAmmoTag(item2);
			}
		}
		if (_inventory.TryGetSlotEntity(local, "back", out var entityUid))
		{
			AddAmmoTag(entityUid.Value);
			StorageComponent storageComponent = default(StorageComponent);
			if (((EntitySystem)this).TryComp<StorageComponent>(entityUid.Value, ref storageComponent) && storageComponent.Container != null)
			{
				foreach (EntityUid containedEntity in ((BaseContainer)storageComponent.Container).ContainedEntities)
				{
					AddAmmoTag(containedEntity);
				}
			}
		}
		if (_inventory.TryGetSlotEntity(local, "suitstorage", out var entityUid2))
		{
			AddAmmoTag(entityUid2.Value);
		}
	}

	private void AddAmmoTag(EntityUid entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		PubgAmmoProviderComponent pubgAmmoProviderComponent = default(PubgAmmoProviderComponent);
		if (((EntitySystem)this).TryComp<PubgAmmoProviderComponent>(entity, ref pubgAmmoProviderComponent) && !string.IsNullOrWhiteSpace(pubgAmmoProviderComponent.AmmoTag))
		{
			_ammoTags.Add(pubgAmmoProviderComponent.AmmoTag);
		}
	}

	private void ApplyOutlines(EntityUid local)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		HashSet<EntityUid> hashSet = new HashSet<EntityUid>();
		MapId mapID = ((EntitySystem)this).Transform(local).MapID;
		EntityQueryEnumerator<TagComponent, SpriteComponent, TransformComponent> val = ((EntitySystem)this).EntityQueryEnumerator<TagComponent, SpriteComponent, TransformComponent>();
		EntityUid val2 = default(EntityUid);
		TagComponent tagComponent = default(TagComponent);
		SpriteComponent val3 = default(SpriteComponent);
		TransformComponent val4 = default(TransformComponent);
		while (val.MoveNext(ref val2, ref tagComponent, ref val3, ref val4))
		{
			if (val4.MapID != mapID || _containers.IsEntityInContainer(val2, (MetaDataComponent)null) || !HasMatchingAmmoTag(val2))
			{
				continue;
			}
			hashSet.Add(val2);
			((EntitySystem)this).EnsureComp<InteractionOutlineComponent>(val2);
			ShaderInstance value = null;
			bool flag = !_highlightShaders.TryGetValue(val2, out value) || val3.PostShader != value || (value != null && value.Disposed);
			if (!flag && value != null)
			{
				try
				{
					value.SetParameter("outline_color", HighlightColor);
				}
				catch
				{
					flag = true;
				}
			}
			if (flag)
			{
				if (value != null)
				{
					try
					{
						value.Dispose();
					}
					catch
					{
					}
				}
				ShaderInstance validShader = GetValidShader(val3.PostShader);
				if (validShader != null)
				{
					_previousShaders.TryAdd(val2, validShader);
				}
				else
				{
					_previousShaders.Remove(val2);
				}
				value = _prototypes.Index<ShaderPrototype>(new ProtoId<ShaderPrototype>("RMCAuraOutline")).InstanceUnique();
				_highlightShaders[val2] = value;
				val3.PostShader = value;
				value.SetParameter("outline_color", HighlightColor);
				value.SetParameter("outline_width", 2f);
			}
			else if (value != null)
			{
				value.SetParameter("outline_width", 2f);
			}
		}
		ClearMissing(hashSet);
	}

	private bool HasMatchingAmmoTag(EntityUid uid)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		foreach (string ammoTag in _ammoTags)
		{
			if (_tags.HasTag(uid, ProtoId<TagPrototype>.op_Implicit(ammoTag)))
			{
				return true;
			}
		}
		return false;
	}

	private void ClearMissing(HashSet<EntityUid> keep)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		_removeScratch.Clear();
		foreach (EntityUid key in _highlightShaders.Keys)
		{
			if (!keep.Contains(key))
			{
				_removeScratch.Add(key);
			}
		}
		foreach (EntityUid item in _removeScratch)
		{
			RestoreOutline(item);
		}
	}

	private void ClearOutlines()
	{
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		_removeScratch.Clear();
		_removeScratch.AddRange(_highlightShaders.Keys);
		foreach (EntityUid item in _removeScratch)
		{
			RestoreOutline(item);
		}
	}

	private void RestoreOutline(EntityUid uid)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		if (_highlightShaders.TryGetValue(uid, out ShaderInstance value))
		{
			SpriteComponent val = default(SpriteComponent);
			if (((EntitySystem)this).Exists(uid) && ((EntitySystem)this).TryComp<SpriteComponent>(uid, ref val) && val.PostShader == value)
			{
				if (_previousShaders.TryGetValue(uid, out ShaderInstance value2) && GetValidShader(value2) != null)
				{
					val.PostShader = value2;
				}
				else
				{
					val.PostShader = null;
				}
			}
			try
			{
				value.Dispose();
			}
			catch
			{
			}
		}
		_highlightShaders.Remove(uid);
		_previousShaders.Remove(uid);
	}
}
