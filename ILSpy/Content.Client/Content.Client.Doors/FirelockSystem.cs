using System;
using Content.Shared.Doors.Components;
using Content.Shared.Doors.Systems;
using Robust.Client.Animations;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Client.Doors;

public sealed class FirelockSystem : SharedFirelockSystem
{
	[Dependency]
	private SharedAppearanceSystem _appearanceSystem;

	[Dependency]
	private SpriteSystem _sprite;

	public override void Initialize()
	{
		base.Initialize();
		((EntitySystem)this).SubscribeLocalEvent<FirelockComponent, AppearanceChangeEvent>((ComponentEventRefHandler<FirelockComponent, AppearanceChangeEvent>)OnAppearanceChange, (Type[])null, (Type[])null);
	}

	protected override void OnComponentStartup(Entity<FirelockComponent> ent, ref ComponentStartup args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Expected O, but got Unknown
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Expected O, but got Unknown
		base.OnComponentStartup(ent, ref args);
		DoorComponent doorComponent = default(DoorComponent);
		if (((EntitySystem)this).TryComp<DoorComponent>(ent.Owner, ref doorComponent))
		{
			doorComponent.ClosedSpriteStates.Add((DoorVisualLayers.BaseUnlit, ent.Comp.WarningLightSpriteState));
			doorComponent.OpenSpriteStates.Add((DoorVisualLayers.BaseUnlit, ent.Comp.WarningLightSpriteState));
			((Animation)doorComponent.OpeningAnimation).AnimationTracks.Add((AnimationTrack)new AnimationTrackSpriteFlick
			{
				LayerKey = DoorVisualLayers.BaseUnlit,
				KeyFrames = 
				{
					new KeyFrame(StateId.op_Implicit(ent.Comp.OpeningLightSpriteState), 0f)
				}
			});
			((Animation)doorComponent.ClosingAnimation).AnimationTracks.Add((AnimationTrack)new AnimationTrackSpriteFlick
			{
				LayerKey = DoorVisualLayers.BaseUnlit,
				KeyFrames = 
				{
					new KeyFrame(StateId.op_Implicit(ent.Comp.ClosingLightSpriteState), 0f)
				}
			});
		}
	}

	private void OnAppearanceChange(EntityUid uid, FirelockComponent comp, ref AppearanceChangeEvent args)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		if (args.Sprite != null)
		{
			bool flag = false;
			bool flag2 = false;
			DoorState doorState = default(DoorState);
			if (!_appearanceSystem.TryGetData<DoorState>(uid, (Enum)DoorVisuals.State, ref doorState, args.Component))
			{
				doorState = DoorState.Closed;
			}
			bool flag3 = default(bool);
			flag = _appearanceSystem.TryGetData<bool>(uid, (Enum)DoorVisuals.BoltLights, ref flag3, args.Component) && flag3;
			bool flag4 = default(bool);
			flag2 = doorState == DoorState.Closing || doorState == DoorState.Opening || doorState == DoorState.Denying || (_appearanceSystem.TryGetData<bool>(uid, (Enum)DoorVisuals.ClosedLights, ref flag4, args.Component) && flag4);
			_sprite.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), (Enum)DoorVisualLayers.BaseUnlit, flag2 && !flag);
			_sprite.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), (Enum)DoorVisualLayers.BaseBolted, flag);
		}
	}
}
