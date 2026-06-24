using System;
using Content.Shared.Hands;
using Content.Shared.Hands.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Shared._RMC14.Xenonids.Inhands;

public sealed class XenoInhandsSystem : EntitySystem
{
	[Dependency]
	private SharedAppearanceSystem _appearance;

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<XenoInhandsComponent, DidEquipHandEvent>((EntityEventRefHandler<XenoInhandsComponent, DidEquipHandEvent>)OnXenoSpritePickedUp, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoInhandsComponent, DidUnequipHandEvent>((EntityEventRefHandler<XenoInhandsComponent, DidUnequipHandEvent>)OnXenoSpriteDropped, (Type[])null, (Type[])null);
	}

	public void OnXenoSpritePickedUp(Entity<XenoInhandsComponent> xeno, ref DidEquipHandEvent args)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		UpdateHand(args.User, args.Equipped, args.Hand, equipped: true);
	}

	public void OnXenoSpriteDropped(Entity<XenoInhandsComponent> xeno, ref DidUnequipHandEvent args)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		UpdateHand(args.User, args.Unequipped, args.Hand, equipped: false);
	}

	private void UpdateHand(EntityUid user, EntityUid item, Hand hand, bool equipped)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).HasComp<XenoInhandsComponent>(user) && hand.Location != HandLocation.Middle)
		{
			string held = string.Empty;
			XenoInhandSpriteComponent inhandSprite = default(XenoInhandSpriteComponent);
			if (equipped && ((EntitySystem)this).TryComp<XenoInhandSpriteComponent>(item, ref inhandSprite))
			{
				held = inhandSprite.StateName ?? string.Empty;
			}
			_appearance.SetData(user, (Enum)((hand.Location != HandLocation.Left) ? XenoInhandVisuals.RightHand : XenoInhandVisuals.LeftHand), (object)held, (AppearanceComponent)null);
		}
	}
}
