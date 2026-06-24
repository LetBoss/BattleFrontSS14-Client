using System;
using Content.Shared.Anomaly.Components;
using Content.Shared.Anomaly.Effects;
using Content.Shared.Body.Components;
using Content.Shared.Body.Prototypes;
using Robust.Client.GameObjects;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;

namespace Content.Client.Anomaly.Effects;

public sealed class ClientInnerBodyAnomalySystem : SharedInnerBodyAnomalySystem
{
	[Dependency]
	private SpriteSystem _sprite;

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<InnerBodyAnomalyComponent, AfterAutoHandleStateEvent>((EntityEventRefHandler<InnerBodyAnomalyComponent, AfterAutoHandleStateEvent>)OnAfterHandleState, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<InnerBodyAnomalyComponent, ComponentShutdown>((EntityEventRefHandler<InnerBodyAnomalyComponent, ComponentShutdown>)OnCompShutdown, (Type[])null, (Type[])null);
	}

	private void OnAfterHandleState(Entity<InnerBodyAnomalyComponent> ent, ref AfterAutoHandleStateEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		SpriteComponent val = default(SpriteComponent);
		if (!((EntitySystem)this).TryComp<SpriteComponent>(Entity<InnerBodyAnomalyComponent>.op_Implicit(ent), ref val) || ent.Comp.FallbackSprite == null)
		{
			return;
		}
		int num = _sprite.LayerMapReserve(Entity<SpriteComponent>.op_Implicit((ent.Owner, val)), ent.Comp.LayerMap);
		BodyComponent bodyComponent = default(BodyComponent);
		if (((EntitySystem)this).TryComp<BodyComponent>(Entity<InnerBodyAnomalyComponent>.op_Implicit(ent), ref bodyComponent))
		{
			ProtoId<BodyPrototype>? prototype = bodyComponent.Prototype;
			if (prototype.HasValue && ent.Comp.SpeciesSprites.TryGetValue(bodyComponent.Prototype.Value, out SpriteSpecifier value))
			{
				_sprite.LayerSetSprite(Entity<SpriteComponent>.op_Implicit((ent.Owner, val)), num, value);
				goto IL_00cf;
			}
		}
		_sprite.LayerSetSprite(Entity<SpriteComponent>.op_Implicit((ent.Owner, val)), num, ent.Comp.FallbackSprite);
		goto IL_00cf;
		IL_00cf:
		_sprite.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((ent.Owner, val)), num, true);
		val.LayerSetShader(num, "unshaded");
	}

	private void OnCompShutdown(Entity<InnerBodyAnomalyComponent> ent, ref ComponentShutdown args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		SpriteComponent item = default(SpriteComponent);
		if (((EntitySystem)this).TryComp<SpriteComponent>(Entity<InnerBodyAnomalyComponent>.op_Implicit(ent), ref item))
		{
			int num = _sprite.LayerMapGet(Entity<SpriteComponent>.op_Implicit((ent.Owner, item)), ent.Comp.LayerMap);
			_sprite.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((ent.Owner, item)), num, false);
		}
	}
}
