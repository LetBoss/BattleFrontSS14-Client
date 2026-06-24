using System;
using Content.Shared.Eye.Blinding.Components;
using Content.Shared.Inventory;
using Content.Shared.Inventory.Events;
using Robust.Shared.GameObjects;

namespace Content.Shared.Eye.Blinding.Systems;

public sealed class BlurryVisionSystem : EntitySystem
{
	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<VisionCorrectionComponent, GotEquippedEvent>((EntityEventRefHandler<VisionCorrectionComponent, GotEquippedEvent>)OnGlassesEquipped, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<VisionCorrectionComponent, GotUnequippedEvent>((EntityEventRefHandler<VisionCorrectionComponent, GotUnequippedEvent>)OnGlassesUnequipped, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<VisionCorrectionComponent, InventoryRelayedEvent<GetBlurEvent>>((EntityEventRefHandler<VisionCorrectionComponent, InventoryRelayedEvent<GetBlurEvent>>)OnGetBlur, (Type[])null, (Type[])null);
	}

	private void OnGetBlur(Entity<VisionCorrectionComponent> glasses, ref InventoryRelayedEvent<GetBlurEvent> args)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		args.Args.Blur += glasses.Comp.VisionBonus;
		args.Args.CorrectionPower *= glasses.Comp.CorrectionPower;
	}

	public void UpdateBlurMagnitude(Entity<BlindableComponent?> ent)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<BlindableComponent>(ent.Owner, ref ent.Comp, false))
		{
			GetBlurEvent ev = new GetBlurEvent(ent.Comp.EyeDamage);
			((EntitySystem)this).RaiseLocalEvent<GetBlurEvent>(Entity<BlindableComponent>.op_Implicit(ent), ev, false);
			float blur = Math.Clamp(ev.Blur, 0f, 6f);
			if (blur <= 0f)
			{
				((EntitySystem)this).RemCompDeferred<BlurryVisionComponent>(Entity<BlindableComponent>.op_Implicit(ent));
				return;
			}
			BlurryVisionComponent blurry = ((EntitySystem)this).EnsureComp<BlurryVisionComponent>(Entity<BlindableComponent>.op_Implicit(ent));
			blurry.Magnitude = blur;
			blurry.CorrectionPower = ev.CorrectionPower;
			((EntitySystem)this).Dirty(Entity<BlindableComponent>.op_Implicit(ent), (IComponent)(object)blurry, (MetaDataComponent)null);
		}
	}

	private void OnGlassesEquipped(Entity<VisionCorrectionComponent> glasses, ref GotEquippedEvent args)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		UpdateBlurMagnitude(Entity<BlindableComponent>.op_Implicit(args.Equipee));
	}

	private void OnGlassesUnequipped(Entity<VisionCorrectionComponent> glasses, ref GotUnequippedEvent args)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		UpdateBlurMagnitude(Entity<BlindableComponent>.op_Implicit(args.Equipee));
	}
}
