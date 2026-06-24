using System;
using System.Numerics;
using Content.Shared._RMC14.NewPlayer;
using Robust.Client.GameObjects;
using Robust.Client.Player;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Player;
using Robust.Shared.Utility;

namespace Content.Client._RMC14.NewPlayer;

public sealed class NewPlayerVisualizerSystem : VisualizerSystem<NewPlayerLabelComponent>
{
	[Dependency]
	private IPlayerManager _player;

	private EntityQuery<SeeNewPlayersComponent> _seeNewPlayersQuery;

	public override void Initialize()
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		base.Initialize();
		_seeNewPlayersQuery = ((EntitySystem)this).GetEntityQuery<SeeNewPlayersComponent>();
		((EntitySystem)this).SubscribeLocalEvent<SeeNewPlayersComponent, LocalPlayerAttachedEvent>((EntityEventRefHandler<SeeNewPlayersComponent, LocalPlayerAttachedEvent>)OnSeeNewPlayersLocalAttached, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<SeeNewPlayersComponent, LocalPlayerDetachedEvent>((EntityEventRefHandler<SeeNewPlayersComponent, LocalPlayerDetachedEvent>)OnSeeNewPlayersLocalDetached, (Type[])null, (Type[])null);
	}

	private void OnSeeNewPlayersLocalAttached(Entity<SeeNewPlayersComponent> ent, ref LocalPlayerAttachedEvent args)
	{
		UpdateAllAppearance();
	}

	private void OnSeeNewPlayersLocalDetached(Entity<SeeNewPlayersComponent> ent, ref LocalPlayerDetachedEvent args)
	{
		UpdateAllAppearance();
	}

	protected override void OnAppearanceChange(EntityUid uid, NewPlayerLabelComponent component, ref AppearanceChangeEvent args)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		if (args.Sprite != null)
		{
			UpdateAppearance(Entity<AppearanceComponent, SpriteComponent>.op_Implicit((uid, args.Component, args.Sprite)));
		}
	}

	private void UpdateAllAppearance()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		AllEntityQueryEnumerator<NewPlayerLabelComponent, AppearanceComponent, SpriteComponent> val = ((EntitySystem)this).AllEntityQuery<NewPlayerLabelComponent, AppearanceComponent, SpriteComponent>();
		EntityUid item = default(EntityUid);
		NewPlayerLabelComponent newPlayerLabelComponent = default(NewPlayerLabelComponent);
		AppearanceComponent item2 = default(AppearanceComponent);
		SpriteComponent item3 = default(SpriteComponent);
		while (val.MoveNext(ref item, ref newPlayerLabelComponent, ref item2, ref item3))
		{
			UpdateAppearance(Entity<AppearanceComponent, SpriteComponent>.op_Implicit((item, item2, item3)));
		}
	}

	private void UpdateAppearance(Entity<AppearanceComponent, SpriteComponent> ent)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		int num = default(int);
		if (!base.SpriteSystem.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((ent.Owner, ent.Comp2)), (Enum)NewPlayerLayers.Layer, ref num, false))
		{
			return;
		}
		SeeNewPlayersComponent seeNewPlayersComponent = default(SeeNewPlayersComponent);
		NewPlayerVisuals newPlayerVisuals = default(NewPlayerVisuals);
		if (!_seeNewPlayersQuery.TryComp(((ISharedPlayerManager)_player).LocalEntity, ref seeNewPlayersComponent) || !((SharedAppearanceSystem)base.AppearanceSystem).TryGetData<NewPlayerVisuals>(Entity<AppearanceComponent, SpriteComponent>.op_Implicit(ent), (Enum)NewPlayerLayers.Layer, ref newPlayerVisuals, Entity<AppearanceComponent, SpriteComponent>.op_Implicit(ent)))
		{
			base.SpriteSystem.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((ent.Owner, ent.Comp2)), num, false);
			return;
		}
		Rsi val = (Rsi)(newPlayerVisuals switch
		{
			NewPlayerVisuals.One => seeNewPlayersComponent.OneLabel, 
			NewPlayerVisuals.Two => seeNewPlayersComponent.TwoLabel, 
			NewPlayerVisuals.Three => seeNewPlayersComponent.ThreeLabel, 
			NewPlayerVisuals.Four => seeNewPlayersComponent.FourLabel, 
			_ => null, 
		});
		if (val != null)
		{
			base.SpriteSystem.LayerSetSprite(Entity<SpriteComponent>.op_Implicit((ent.Owner, ent.Comp2)), num, (SpriteSpecifier)(object)val);
			base.SpriteSystem.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((ent.Owner, ent.Comp2)), num, true);
			base.SpriteSystem.LayerSetOffset(Entity<SpriteComponent>.op_Implicit((ent.Owner, ent.Comp2)), num, new Vector2(0f, 0.21f));
		}
	}
}
