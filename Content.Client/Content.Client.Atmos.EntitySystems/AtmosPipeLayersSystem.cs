using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Content.Shared.Atmos.Components;
using Content.Shared.Atmos.EntitySystems;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Client.ResourceManagement;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Reflection;
using Robust.Shared.Serialization.TypeSerializers.Implementations;
using Robust.Shared.Utility;

namespace Content.Client.Atmos.EntitySystems;

public sealed class AtmosPipeLayersSystem : SharedAtmosPipeLayersSystem
{
	[Dependency]
	private SharedAppearanceSystem _appearance;

	[Dependency]
	private IReflectionManager _reflection;

	[Dependency]
	private IResourceCache _resourceCache;

	[Dependency]
	private SpriteSystem _sprite;

	public override void Initialize()
	{
		base.Initialize();
		((EntitySystem)this).SubscribeLocalEvent<AtmosPipeLayersComponent, AppearanceChangeEvent>((EntityEventRefHandler<AtmosPipeLayersComponent, AppearanceChangeEvent>)OnAppearanceChange, (Type[])null, (Type[])null);
	}

	private void OnAppearanceChange(Entity<AtmosPipeLayersComponent> ent, ref AppearanceChangeEvent ev)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		SpriteComponent item = default(SpriteComponent);
		if (!((EntitySystem)this).TryComp<SpriteComponent>(Entity<AtmosPipeLayersComponent>.op_Implicit(ent), ref item))
		{
			return;
		}
		string text = default(string);
		RSIResource val = default(RSIResource);
		if (_appearance.TryGetData<string>(Entity<AtmosPipeLayersComponent>.op_Implicit(ent), (Enum)AtmosPipeLayerVisuals.Sprite, ref text, (AppearanceComponent)null) && _resourceCache.TryGetResource<RSIResource>(SpriteSpecifierSerializer.TextureRoot / text, ref val))
		{
			_sprite.SetBaseRsi(Entity<SpriteComponent>.op_Implicit((Entity<AtmosPipeLayersComponent>.op_Implicit(ent), item)), val.RSI);
		}
		Dictionary<string, string> dictionary = default(Dictionary<string, string>);
		if (!_appearance.TryGetData<Dictionary<string, string>>(Entity<AtmosPipeLayersComponent>.op_Implicit(ent), (Enum)AtmosPipeLayerVisuals.SpriteLayers, ref dictionary, (AppearanceComponent)null))
		{
			return;
		}
		foreach (var (text4, text5) in dictionary)
		{
			if (TryParseKey(text4, out Enum @enum))
			{
				_sprite.LayerSetRsi(Entity<SpriteComponent>.op_Implicit((Entity<AtmosPipeLayersComponent>.op_Implicit(ent), item)), @enum, new ResPath(text5), (StateId?)null);
			}
			else
			{
				_sprite.LayerSetRsi(Entity<SpriteComponent>.op_Implicit((Entity<AtmosPipeLayersComponent>.op_Implicit(ent), item)), text4, new ResPath(text5), (StateId?)null);
			}
		}
	}

	private bool TryParseKey(string keyString, [NotNullWhen(true)] out Enum? @enum)
	{
		return _reflection.TryParseEnumReference(keyString, ref @enum, true);
	}
}
