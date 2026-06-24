using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Content.Shared.Hands;
using Content.Shared.Inventory.Events;
using Content.Shared.Item;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Client.ResourceManagement;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Serialization.TypeSerializers.Implementations;

namespace Content.Client.Items.Systems;

public sealed class ItemSystem : SharedItemSystem
{
	[Dependency]
	private IResourceCache _resCache;

	[Dependency]
	private SpriteSystem _sprite;

	public override void Initialize()
	{
		base.Initialize();
		((EntitySystem)this).SubscribeLocalEvent<ItemComponent, GetInhandVisualsEvent>((ComponentEventHandler<ItemComponent, GetInhandVisualsEvent>)OnGetVisuals, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<SpriteComponent, GotEquippedEvent>((ComponentEventHandler<SpriteComponent, GotEquippedEvent>)OnEquipped, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<SpriteComponent, GotUnequippedEvent>((ComponentEventHandler<SpriteComponent, GotUnequippedEvent>)OnUnequipped, (Type[])null, (Type[])null);
	}

	private void OnUnequipped(EntityUid uid, SpriteComponent component, GotUnequippedEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		_sprite.SetVisible(Entity<SpriteComponent>.op_Implicit((uid, component)), true);
	}

	private void OnEquipped(EntityUid uid, SpriteComponent component, GotEquippedEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		_sprite.SetVisible(Entity<SpriteComponent>.op_Implicit((uid, component)), false);
	}

	public override void VisualsChanged(EntityUid uid)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		BaseContainer val = default(BaseContainer);
		if (Container.TryGetContainingContainer(Entity<TransformComponent, MetaDataComponent>.op_Implicit((ValueTuple<EntityUid, TransformComponent, MetaDataComponent>)(uid, null, null)), ref val))
		{
			((EntitySystem)this).RaiseLocalEvent<VisualsChangedEvent>(val.Owner, new VisualsChangedEvent(((EntitySystem)this).GetNetEntity(uid, (MetaDataComponent)null), val.ID), false);
		}
	}

	private void OnGetVisuals(EntityUid uid, ItemComponent item, GetInhandVisualsEvent args)
	{
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		string text = "inhand-" + args.Location.ToString().ToLowerInvariant();
		if (!item.InhandVisuals.TryGetValue(args.Location, out List<PrototypeLayerData> value) && !TryGetDefaultVisuals(uid, item, text, out value))
		{
			return;
		}
		int num = 0;
		foreach (PrototypeLayerData item2 in value)
		{
			string text2 = item2.MapKeys?.FirstOrDefault();
			if (text2 == null)
			{
				text2 = ((num == 0) ? text : $"{text}-{num}");
				num++;
			}
			args.Layers.Add((text2, item2));
		}
	}

	private bool TryGetDefaultVisuals(EntityUid uid, ItemComponent item, string defaultKey, [NotNullWhen(true)] out List<PrototypeLayerData>? result)
	{
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Expected O, but got Unknown
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		result = null;
		RSI val = null;
		SpriteComponent val2 = default(SpriteComponent);
		if (item.RsiPath != null)
		{
			val = _resCache.GetResource<RSIResource>(SpriteSpecifierSerializer.TextureRoot / item.RsiPath, true).RSI;
		}
		else if (((EntitySystem)this).TryComp<SpriteComponent>(uid, ref val2))
		{
			val = val2.BaseRSI;
		}
		if (val == null)
		{
			return false;
		}
		string text = ((item.HeldPrefix == null) ? defaultKey : (item.HeldPrefix + "-" + defaultKey));
		State val3 = default(State);
		if (!val.TryGetState(StateId.op_Implicit(text), ref val3))
		{
			return false;
		}
		PrototypeLayerData val4 = new PrototypeLayerData();
		val4.RsiPath = ((object)val.Path/*cast due to constrained. prefix*/).ToString();
		val4.State = text;
		val4.MapKeys = new HashSet<string> { text };
		result = new List<PrototypeLayerData> { val4 };
		return true;
	}
}
