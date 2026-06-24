using System;
using Content.Shared.Camera;
using Content.Shared.Eye.Blinding.Components;
using Content.Shared.Rejuvenate;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Shared.Eye.Blinding.Systems;

public sealed class BlindableSystem : EntitySystem
{
	[Dependency]
	private BlurryVisionSystem _blurriness;

	[Dependency]
	private EyeClosingSystem _eyelids;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<BlindableComponent, RejuvenateEvent>((EntityEventRefHandler<BlindableComponent, RejuvenateEvent>)OnRejuvenate, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<BlindableComponent, EyeDamageChangedEvent>((EntityEventRefHandler<BlindableComponent, EyeDamageChangedEvent>)OnDamageChanged, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<BlindableComponent, GetEyePvsScaleAttemptEvent>((EntityEventRefHandler<BlindableComponent, GetEyePvsScaleAttemptEvent>)OnGetEyePvsScaleAttemptEvent, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<BlindableComponent, GetEyeOffsetAttemptEvent>((EntityEventRefHandler<BlindableComponent, GetEyeOffsetAttemptEvent>)OnGetEyeOffsetAttemptEvent, (Type[])null, (Type[])null);
	}

	private void OnRejuvenate(Entity<BlindableComponent> ent, ref RejuvenateEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		AdjustEyeDamage(Entity<BlindableComponent>.op_Implicit((ent.Owner, ent.Comp)), -ent.Comp.EyeDamage);
	}

	private void OnDamageChanged(Entity<BlindableComponent> ent, ref EyeDamageChangedEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		_blurriness.UpdateBlurMagnitude(Entity<BlindableComponent>.op_Implicit((ent.Owner, ent.Comp)));
		_eyelids.UpdateEyesClosable(Entity<BlindableComponent>.op_Implicit((ent.Owner, ent.Comp)));
	}

	private void OnGetEyePvsScaleAttemptEvent(Entity<BlindableComponent> ent, ref GetEyePvsScaleAttemptEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		if (ent.Comp.IsBlind)
		{
			args.Cancelled = true;
		}
	}

	private void OnGetEyeOffsetAttemptEvent(Entity<BlindableComponent> ent, ref GetEyeOffsetAttemptEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		if (ent.Comp.IsBlind)
		{
			args.Cancelled = true;
		}
	}

	public void UpdateIsBlind(Entity<BlindableComponent?> blindable)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<BlindableComponent>(Entity<BlindableComponent>.op_Implicit(blindable), ref blindable.Comp, false))
		{
			bool isBlind = blindable.Comp.IsBlind;
			if (blindable.Comp.EyeDamage >= blindable.Comp.MaxDamage)
			{
				blindable.Comp.IsBlind = true;
			}
			else
			{
				CanSeeAttemptEvent ev = new CanSeeAttemptEvent();
				((EntitySystem)this).RaiseLocalEvent<CanSeeAttemptEvent>(blindable.Owner, ev, false);
				blindable.Comp.IsBlind = ev.Blind;
			}
			if (isBlind != blindable.Comp.IsBlind)
			{
				BlindnessChangedEvent changeEv = new BlindnessChangedEvent(blindable.Comp.IsBlind);
				((EntitySystem)this).RaiseLocalEvent<BlindnessChangedEvent>(blindable.Owner, ref changeEv, false);
				((EntitySystem)this).Dirty<BlindableComponent>(blindable, (MetaDataComponent)null);
			}
		}
	}

	public void AdjustEyeDamage(Entity<BlindableComponent?> blindable, int amount)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<BlindableComponent>(Entity<BlindableComponent>.op_Implicit(blindable), ref blindable.Comp, false) && amount != 0)
		{
			blindable.Comp.EyeDamage += amount;
			UpdateEyeDamage(blindable, isDamageChanged: true);
		}
	}

	private void UpdateEyeDamage(Entity<BlindableComponent?> blindable, bool isDamageChanged)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<BlindableComponent>(Entity<BlindableComponent>.op_Implicit(blindable), ref blindable.Comp, false))
		{
			int previousDamage = blindable.Comp.EyeDamage;
			blindable.Comp.EyeDamage = Math.Clamp(blindable.Comp.EyeDamage, blindable.Comp.MinDamage, blindable.Comp.MaxDamage);
			((EntitySystem)this).Dirty<BlindableComponent>(blindable, (MetaDataComponent)null);
			if (isDamageChanged || previousDamage != blindable.Comp.EyeDamage)
			{
				UpdateIsBlind(blindable);
				EyeDamageChangedEvent ev = new EyeDamageChangedEvent(blindable.Comp.EyeDamage);
				((EntitySystem)this).RaiseLocalEvent<EyeDamageChangedEvent>(blindable.Owner, ref ev, false);
			}
		}
	}

	public void SetMinDamage(Entity<BlindableComponent?> blindable, int amount)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<BlindableComponent>(Entity<BlindableComponent>.op_Implicit(blindable), ref blindable.Comp, false))
		{
			blindable.Comp.MinDamage = amount;
			UpdateEyeDamage(blindable, isDamageChanged: false);
		}
	}
}
