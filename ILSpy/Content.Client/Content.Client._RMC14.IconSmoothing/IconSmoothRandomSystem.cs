using System;
using System.Collections.Generic;
using Content.Client.IconSmoothing;
using Content.Shared.Sprite;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Reflection;

namespace Content.Client._RMC14.IconSmoothing;

public sealed class IconSmoothRandomSystem : EntitySystem
{
	[Dependency]
	private IReflectionManager _reflection;

	[Dependency]
	private SpriteSystem _sprite;

	private EntityQuery<RandomSpriteComponent> _randomSpriteQuery;

	private EntityQuery<SpriteComponent> _spriteQuery;

	public override void Initialize()
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).Initialize();
		_randomSpriteQuery = ((EntitySystem)this).GetEntityQuery<RandomSpriteComponent>();
		_spriteQuery = ((EntitySystem)this).GetEntityQuery<SpriteComponent>();
		((EntitySystem)this).SubscribeLocalEvent<IconSmoothRandomComponent, IconSmoothingUpdatedEvent>((EntityEventRefHandler<IconSmoothRandomComponent, IconSmoothingUpdatedEvent>)OnOverrideIconSmoothingUpdated, (Type[])null, (Type[])null);
	}

	private void OnOverrideIconSmoothingUpdated(Entity<IconSmoothRandomComponent> ent, ref IconSmoothingUpdatedEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
		RandomSpriteComponent randomSpriteComponent = default(RandomSpriteComponent);
		SpriteComponent item = default(SpriteComponent);
		if (!_randomSpriteQuery.TryGetComponent(Entity<IconSmoothRandomComponent>.op_Implicit(ent), ref randomSpriteComponent) || !_spriteQuery.TryGetComponent(Entity<IconSmoothRandomComponent>.op_Implicit(ent), ref item))
		{
			return;
		}
		Enum obj = default(Enum);
		int result = default(int);
		foreach (KeyValuePair<string, (string, Color?)> item2 in randomSpriteComponent.Selected)
		{
			if (_reflection.TryParseEnumReference(item2.Key, ref obj, true))
			{
				if (!_sprite.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((ent.Owner, item)), obj, ref result, true))
				{
					continue;
				}
			}
			else if (!_sprite.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((ent.Owner, item)), item2.Key, ref result, false))
			{
				string key = item2.Key;
				if (key == null || !int.TryParse(key, out result))
				{
					((EntitySystem)this).Log.Error($"Invalid key `{item2.Key}` for entity with random sprite {((EntitySystem)this).ToPrettyString((EntityUid?)Entity<IconSmoothRandomComponent>.op_Implicit(ent), (MetaDataComponent)null)}");
					continue;
				}
			}
			string name = _sprite.LayerGetRsiState(Entity<SpriteComponent>.op_Implicit((ent.Owner, item)), result).Name;
			if (name != null && ent.Comp.Overrides.Contains(name))
			{
				_sprite.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((ent.Owner, item)), result, StateId.op_Implicit(item2.Value.Item1));
				_sprite.LayerSetColor(Entity<SpriteComponent>.op_Implicit((ent.Owner, item)), result, (Color)(((_003F?)item2.Value.Item2) ?? Color.White));
			}
		}
	}
}
