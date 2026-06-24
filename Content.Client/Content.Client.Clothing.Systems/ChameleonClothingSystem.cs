using System;
using Content.Client.PDA;
using Content.Shared.Clothing.Components;
using Content.Shared.Clothing.EntitySystems;
using Robust.Client.GameObjects;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;

namespace Content.Client.Clothing.Systems;

public sealed class ChameleonClothingSystem : SharedChameleonClothingSystem
{
	[Dependency]
	private SpriteSystem _sprite;

	public override void Initialize()
	{
		base.Initialize();
		((EntitySystem)this).SubscribeLocalEvent<ChameleonClothingComponent, AfterAutoHandleStateEvent>((ComponentEventRefHandler<ChameleonClothingComponent, AfterAutoHandleStateEvent>)HandleState, (Type[])null, (Type[])null);
		PrepareAllVariants();
		((EntitySystem)this).SubscribeLocalEvent<PrototypesReloadedEventArgs>((EntityEventHandler<PrototypesReloadedEventArgs>)OnProtoReloaded, (Type[])null, (Type[])null);
	}

	private void OnProtoReloaded(PrototypesReloadedEventArgs args)
	{
		if (args.WasModified<EntityPrototype>())
		{
			PrepareAllVariants();
		}
	}

	private void HandleState(EntityUid uid, ChameleonClothingComponent component, ref AfterAutoHandleStateEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		UpdateVisuals(uid, component);
	}

	protected override void UpdateSprite(EntityUid uid, EntityPrototype proto)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		base.UpdateSprite(uid, proto);
		SpriteComponent item = default(SpriteComponent);
		SpriteComponent item2 = default(SpriteComponent);
		if (((EntitySystem)this).TryComp<SpriteComponent>(uid, ref item) && proto.TryGetComponent<SpriteComponent>(ref item2, ((EntitySystem)this).Factory))
		{
			_sprite.CopySprite(Entity<SpriteComponent>.op_Implicit((EntityUid.Invalid, item2)), Entity<SpriteComponent>.op_Implicit((uid, item)));
		}
		PdaBorderColorComponent pdaBorderColorComponent = default(PdaBorderColorComponent);
		PdaBorderColorComponent pdaBorderColorComponent2 = default(PdaBorderColorComponent);
		if (((EntitySystem)this).TryComp<PdaBorderColorComponent>(uid, ref pdaBorderColorComponent) && proto.TryGetComponent<PdaBorderColorComponent>(ref pdaBorderColorComponent2, ((EntitySystem)this).Factory))
		{
			pdaBorderColorComponent.BorderColor = pdaBorderColorComponent2.BorderColor;
			pdaBorderColorComponent.AccentHColor = pdaBorderColorComponent2.AccentHColor;
			pdaBorderColorComponent.AccentVColor = pdaBorderColorComponent2.AccentVColor;
		}
	}
}
