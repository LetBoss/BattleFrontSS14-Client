using System;
using Content.Shared._RMC14.Marines.Squads;
using Content.Shared._RMC14.Rangefinder.Spotting;
using Content.Shared._RMC14.Weapons.Ranged.AimedShot.FocusedShooting;
using Robust.Client.GameObjects;
using Robust.Client.Player;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Player;

namespace Content.Client._RMC14.Weapons.Ranged.Sniper.Focused;

public sealed class RMCFocusingSystem : EntitySystem
{
	private const string FocusedKey = "focused";

	[Dependency]
	private IPlayerManager _player;

	[Dependency]
	private SharedAppearanceSystem _appearance;

	[Dependency]
	private SpriteSystem _sprite;

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<RMCFocusingComponent, ComponentRemove>((EntityEventRefHandler<RMCFocusingComponent, ComponentRemove>)OnComponentRemove, (Type[])null, (Type[])null);
	}

	private void OnComponentRemove(Entity<RMCFocusingComponent> ent, ref ComponentRemove args)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? localEntity = ((ISharedPlayerManager)_player).LocalEntity;
		EntityUid owner = ent.Owner;
		EntityUid? val = localEntity;
		if (val.HasValue && !(owner != val.GetValueOrDefault()))
		{
			_appearance.SetData(ent.Comp.FocusTarget, (Enum)FocusedVisuals.Focused, (object)false, (AppearanceComponent)null);
		}
	}

	public override void Update(float frameTime)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		EntityQueryEnumerator<RMCFocusingComponent> val = ((EntitySystem)this).EntityQueryEnumerator<RMCFocusingComponent>();
		EntityUid val2 = default(EntityUid);
		RMCFocusingComponent rMCFocusingComponent = default(RMCFocusingComponent);
		SpriteComponent item = default(SpriteComponent);
		int num = default(int);
		Layer val5 = default(Layer);
		SquadMemberComponent squadMemberComponent = default(SquadMemberComponent);
		while (val.MoveNext(ref val2, ref rMCFocusingComponent))
		{
			EntityUid? localEntity = ((ISharedPlayerManager)_player).LocalEntity;
			if (rMCFocusingComponent.OldTarget.HasValue)
			{
				_appearance.SetData(rMCFocusingComponent.OldTarget.Value, (Enum)FocusedVisuals.Focused, (object)false, (AppearanceComponent)null);
				rMCFocusingComponent.OldTarget = null;
			}
			EntityUid val3 = val2;
			EntityUid? val4 = localEntity;
			if (((!val4.HasValue || val3 != val4.GetValueOrDefault()) && !((EntitySystem)this).HasComp<SpotterWhitelistComponent>(val2)) || !((EntitySystem)this).TryComp<SpriteComponent>(rMCFocusingComponent.FocusTarget, ref item) || !_sprite.LayerExists(Entity<SpriteComponent>.op_Implicit((rMCFocusingComponent.FocusTarget, item)), "focused"))
			{
				break;
			}
			_sprite.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((rMCFocusingComponent.FocusTarget, item)), "focused", ref num, false);
			_sprite.TryGetLayer(Entity<SpriteComponent>.op_Implicit((rMCFocusingComponent.FocusTarget, item)), num, ref val5, false);
			if (val5 != null && val5.Visible)
			{
				break;
			}
			_appearance.SetData(rMCFocusingComponent.FocusTarget, (Enum)FocusedVisuals.Focused, (object)true, (AppearanceComponent)null);
			if (!((EntitySystem)this).TryComp<SquadMemberComponent>(val2, ref squadMemberComponent))
			{
				break;
			}
			_sprite.LayerSetColor(Entity<SpriteComponent>.op_Implicit((rMCFocusingComponent.FocusTarget, item)), "focused", squadMemberComponent.BackgroundColor);
		}
	}
}
