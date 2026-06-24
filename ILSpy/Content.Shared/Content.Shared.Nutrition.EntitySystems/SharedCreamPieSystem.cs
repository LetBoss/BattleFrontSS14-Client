using System;
using Content.Shared.Nutrition.Components;
using Content.Shared.Stunnable;
using Content.Shared.Throwing;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Shared.Nutrition.EntitySystems;

public abstract class SharedCreamPieSystem : EntitySystem
{
	[Dependency]
	private SharedStunSystem _stunSystem;

	[Dependency]
	private SharedAppearanceSystem _appearance;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<CreamPieComponent, ThrowDoHitEvent>((ComponentEventHandler<CreamPieComponent, ThrowDoHitEvent>)OnCreamPieHit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<CreamPieComponent, LandEvent>((ComponentEventRefHandler<CreamPieComponent, LandEvent>)OnCreamPieLand, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<CreamPiedComponent, ThrowHitByEvent>((ComponentEventHandler<CreamPiedComponent, ThrowHitByEvent>)OnCreamPiedHitBy, (Type[])null, (Type[])null);
	}

	public void SplatCreamPie(EntityUid uid, CreamPieComponent creamPie)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		if (!creamPie.Splatted)
		{
			creamPie.Splatted = true;
			SplattedCreamPie(uid, creamPie);
		}
	}

	protected virtual void SplattedCreamPie(EntityUid uid, CreamPieComponent creamPie)
	{
	}

	public void SetCreamPied(EntityUid uid, CreamPiedComponent creamPied, bool value)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		if (value != creamPied.CreamPied)
		{
			creamPied.CreamPied = value;
			AppearanceComponent appearance = default(AppearanceComponent);
			if (((EntitySystem)this).TryComp<AppearanceComponent>(uid, ref appearance))
			{
				_appearance.SetData(uid, (Enum)CreamPiedVisuals.Creamed, (object)value, appearance);
			}
		}
	}

	private void OnCreamPieLand(EntityUid uid, CreamPieComponent component, ref LandEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		SplatCreamPie(uid, component);
	}

	private void OnCreamPieHit(EntityUid uid, CreamPieComponent component, ThrowDoHitEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		SplatCreamPie(uid, component);
	}

	private void OnCreamPiedHitBy(EntityUid uid, CreamPiedComponent creamPied, ThrowHitByEvent args)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		CreamPieComponent creamPie = default(CreamPieComponent);
		if (((EntitySystem)this).Exists(args.Thrown) && ((EntitySystem)this).TryComp<CreamPieComponent>(args.Thrown, ref creamPie))
		{
			SetCreamPied(uid, creamPied, value: true);
			CreamedEntity(uid, creamPied, args);
			_stunSystem.TryParalyze(uid, TimeSpan.FromSeconds(creamPie.ParalyzeTime), refresh: true);
		}
	}

	protected virtual void CreamedEntity(EntityUid uid, CreamPiedComponent creamPied, ThrowHitByEvent args)
	{
	}
}
