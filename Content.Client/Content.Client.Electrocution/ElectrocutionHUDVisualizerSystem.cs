using System;
using Content.Shared.Electrocution;
using Robust.Client.GameObjects;
using Robust.Client.Player;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Player;

namespace Content.Client.Electrocution;

public sealed class ElectrocutionHUDVisualizerSystem : VisualizerSystem<ElectrocutionHUDVisualsComponent>
{
	[Dependency]
	private IPlayerManager _playerMan;

	public override void Initialize()
	{
		base.Initialize();
		((EntitySystem)this).SubscribeLocalEvent<ShowElectrocutionHUDComponent, ComponentInit>((EntityEventRefHandler<ShowElectrocutionHUDComponent, ComponentInit>)OnInit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ShowElectrocutionHUDComponent, ComponentShutdown>((EntityEventRefHandler<ShowElectrocutionHUDComponent, ComponentShutdown>)OnShutdown, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ShowElectrocutionHUDComponent, LocalPlayerAttachedEvent>((EntityEventRefHandler<ShowElectrocutionHUDComponent, LocalPlayerAttachedEvent>)OnPlayerAttached, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ShowElectrocutionHUDComponent, LocalPlayerDetachedEvent>((EntityEventRefHandler<ShowElectrocutionHUDComponent, LocalPlayerDetachedEvent>)OnPlayerDetached, (Type[])null, (Type[])null);
	}

	private void OnPlayerAttached(Entity<ShowElectrocutionHUDComponent> ent, ref LocalPlayerAttachedEvent args)
	{
		ShowHUD();
	}

	private void OnPlayerDetached(Entity<ShowElectrocutionHUDComponent> ent, ref LocalPlayerDetachedEvent args)
	{
		RemoveHUD();
	}

	private void OnInit(Entity<ShowElectrocutionHUDComponent> ent, ref ComponentInit args)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? localEntity = ((ISharedPlayerManager)_playerMan).LocalEntity;
		EntityUid val = Entity<ShowElectrocutionHUDComponent>.op_Implicit(ent);
		if (localEntity.HasValue && localEntity.GetValueOrDefault() == val)
		{
			ShowHUD();
		}
	}

	private void OnShutdown(Entity<ShowElectrocutionHUDComponent> ent, ref ComponentShutdown args)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? localEntity = ((ISharedPlayerManager)_playerMan).LocalEntity;
		EntityUid val = Entity<ShowElectrocutionHUDComponent>.op_Implicit(ent);
		if (localEntity.HasValue && localEntity.GetValueOrDefault() == val)
		{
			RemoveHUD();
		}
	}

	private void ShowHUD()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		AllEntityQueryEnumerator<ElectrocutionHUDVisualsComponent, AppearanceComponent, SpriteComponent> val = ((EntitySystem)this).AllEntityQuery<ElectrocutionHUDVisualsComponent, AppearanceComponent, SpriteComponent>();
		EntityUid val2 = default(EntityUid);
		ElectrocutionHUDVisualsComponent electrocutionHUDVisualsComponent = default(ElectrocutionHUDVisualsComponent);
		AppearanceComponent val3 = default(AppearanceComponent);
		SpriteComponent item = default(SpriteComponent);
		bool flag = default(bool);
		while (val.MoveNext(ref val2, ref electrocutionHUDVisualsComponent, ref val3, ref item))
		{
			if (((SharedAppearanceSystem)base.AppearanceSystem).TryGetData<bool>(val2, (Enum)ElectrifiedVisuals.IsElectrified, ref flag, val3))
			{
				base.SpriteSystem.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((val2, item)), (Enum)ElectrifiedLayers.HUD, flag);
			}
		}
	}

	private void RemoveHUD()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		AllEntityQueryEnumerator<ElectrocutionHUDVisualsComponent, AppearanceComponent, SpriteComponent> val = ((EntitySystem)this).AllEntityQuery<ElectrocutionHUDVisualsComponent, AppearanceComponent, SpriteComponent>();
		EntityUid item = default(EntityUid);
		ElectrocutionHUDVisualsComponent electrocutionHUDVisualsComponent = default(ElectrocutionHUDVisualsComponent);
		AppearanceComponent val2 = default(AppearanceComponent);
		SpriteComponent item2 = default(SpriteComponent);
		while (val.MoveNext(ref item, ref electrocutionHUDVisualsComponent, ref val2, ref item2))
		{
			base.SpriteSystem.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((item, item2)), (Enum)ElectrifiedLayers.HUD, false);
		}
	}

	protected override void OnAppearanceChange(EntityUid uid, ElectrocutionHUDVisualsComponent comp, ref AppearanceChangeEvent args)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		bool flag = default(bool);
		if (args.Sprite != null && ((SharedAppearanceSystem)base.AppearanceSystem).TryGetData<bool>(uid, (Enum)ElectrifiedVisuals.IsElectrified, ref flag, args.Component))
		{
			EntityUid? localEntity = ((ISharedPlayerManager)_playerMan).LocalEntity;
			base.SpriteSystem.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), (Enum)ElectrifiedLayers.HUD, flag && ((EntitySystem)this).HasComp<ShowElectrocutionHUDComponent>(localEntity));
		}
	}
}
