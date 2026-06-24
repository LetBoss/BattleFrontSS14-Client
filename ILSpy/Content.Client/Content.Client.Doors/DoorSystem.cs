using System;
using System.Collections.Generic;
using Content.Shared.Doors.Components;
using Content.Shared.Doors.Systems;
using Robust.Client.Animations;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Client.ResourceManagement;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Serialization.TypeSerializers.Implementations;

namespace Content.Client.Doors;

public sealed class DoorSystem : SharedDoorSystem
{
	[Dependency]
	private AnimationPlayerSystem _animationSystem;

	[Dependency]
	private IResourceCache _resourceCache;

	[Dependency]
	private SpriteSystem _sprite;

	public override void Initialize()
	{
		base.Initialize();
		((EntitySystem)this).SubscribeLocalEvent<DoorComponent, AppearanceChangeEvent>((EntityEventRefHandler<DoorComponent, AppearanceChangeEvent>)OnAppearanceChange, (Type[])null, (Type[])null);
	}

	protected override void OnComponentInit(Entity<DoorComponent> ent, ref ComponentInit args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Expected O, but got Unknown
		//IL_00a6: Expected O, but got Unknown
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Expected O, but got Unknown
		//IL_00ff: Expected O, but got Unknown
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Expected O, but got Unknown
		//IL_0158: Expected O, but got Unknown
		DoorComponent comp = ent.Comp;
		comp.OpenSpriteStates = new List<(DoorVisualLayers, string)>(2);
		comp.ClosedSpriteStates = new List<(DoorVisualLayers, string)>(2);
		comp.OpenSpriteStates.Add((DoorVisualLayers.Base, comp.OpenSpriteState));
		comp.ClosedSpriteStates.Add((DoorVisualLayers.Base, comp.ClosedSpriteState));
		comp.OpeningAnimation = (object)new Animation
		{
			Length = TimeSpan.FromSeconds(comp.OpeningAnimationTime),
			AnimationTracks = { (AnimationTrack)new AnimationTrackSpriteFlick
			{
				LayerKey = DoorVisualLayers.Base,
				KeyFrames = 
				{
					new KeyFrame(StateId.op_Implicit(comp.OpeningSpriteState), 0f)
				}
			} }
		};
		comp.ClosingAnimation = (object)new Animation
		{
			Length = TimeSpan.FromSeconds(comp.ClosingAnimationTime),
			AnimationTracks = { (AnimationTrack)new AnimationTrackSpriteFlick
			{
				LayerKey = DoorVisualLayers.Base,
				KeyFrames = 
				{
					new KeyFrame(StateId.op_Implicit(comp.ClosingSpriteState), 0f)
				}
			} }
		};
		comp.EmaggingAnimation = (object)new Animation
		{
			Length = TimeSpan.FromSeconds(comp.EmaggingAnimationTime),
			AnimationTracks = { (AnimationTrack)new AnimationTrackSpriteFlick
			{
				LayerKey = DoorVisualLayers.BaseUnlit,
				KeyFrames = 
				{
					new KeyFrame(StateId.op_Implicit(comp.EmaggingSpriteState), 0f)
				}
			} }
		};
	}

	private void OnAppearanceChange(Entity<DoorComponent> entity, ref AppearanceChangeEvent args)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		if (args.Sprite != null)
		{
			DoorState state = default(DoorState);
			if (!AppearanceSystem.TryGetData<DoorState>(Entity<DoorComponent>.op_Implicit(entity), (Enum)DoorVisuals.State, ref state, args.Component))
			{
				state = DoorState.Closed;
			}
			string baseRsi = default(string);
			if (AppearanceSystem.TryGetData<string>(Entity<DoorComponent>.op_Implicit(entity), (Enum)DoorVisuals.BaseRSI, ref baseRsi, args.Component))
			{
				UpdateSpriteLayers(Entity<SpriteComponent>.op_Implicit((entity.Owner, args.Sprite)), baseRsi);
			}
			if (_animationSystem.HasRunningAnimation(Entity<DoorComponent>.op_Implicit(entity), "door_animation"))
			{
				_animationSystem.Stop(Entity<AnimationPlayerComponent>.op_Implicit(entity.Owner), "door_animation");
			}
			UpdateAppearanceForDoorState(entity, args.Sprite, state);
		}
	}

	private void UpdateAppearanceForDoorState(Entity<DoorComponent> entity, SpriteComponent sprite, DoorState state)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e5: Expected O, but got Unknown
		//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_020c: Expected O, but got Unknown
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Expected O, but got Unknown
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01af: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Expected O, but got Unknown
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		_sprite.SetDrawDepth(Entity<SpriteComponent>.op_Implicit((entity.Owner, sprite)), (state == DoorState.Open) ? entity.Comp.OpenDrawDepth : entity.Comp.ClosedDrawDepth);
		switch (state)
		{
		case DoorState.Open:
		{
			foreach (var (doorVisualLayers, text) in entity.Comp.OpenSpriteStates)
			{
				_sprite.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((entity.Owner, sprite)), (Enum)doorVisualLayers, StateId.op_Implicit(text));
			}
			break;
		}
		case DoorState.Closed:
		{
			foreach (var (doorVisualLayers2, text2) in entity.Comp.ClosedSpriteStates)
			{
				_sprite.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((entity.Owner, sprite)), (Enum)doorVisualLayers2, StateId.op_Implicit(text2));
			}
			break;
		}
		case DoorState.Opening:
			if ((double)entity.Comp.OpeningAnimationTime != 0.0)
			{
				_animationSystem.Play(Entity<DoorComponent>.op_Implicit(entity), (Animation)entity.Comp.OpeningAnimation, "door_animation");
			}
			break;
		case DoorState.Closing:
			if ((double)entity.Comp.ClosingAnimationTime != 0.0 && entity.Comp.CurrentlyCrushing.Count == 0)
			{
				_animationSystem.Play(Entity<DoorComponent>.op_Implicit(entity), (Animation)entity.Comp.ClosingAnimation, "door_animation");
			}
			break;
		case DoorState.Denying:
			_animationSystem.Play(Entity<DoorComponent>.op_Implicit(entity), (Animation)entity.Comp.DenyingAnimation, "door_animation");
			break;
		case DoorState.Emagging:
			_animationSystem.Play(Entity<DoorComponent>.op_Implicit(entity), (Animation)entity.Comp.EmaggingAnimation, "door_animation");
			break;
		case DoorState.Welded:
			break;
		}
	}

	private void UpdateSpriteLayers(Entity<SpriteComponent> sprite, string baseRsi)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		RSIResource val = default(RSIResource);
		if (!_resourceCache.TryGetResource<RSIResource>(SpriteSpecifierSerializer.TextureRoot / baseRsi, ref val))
		{
			((EntitySystem)this).Log.Error("Unable to load RSI '{0}'. Trace:\n{1}", new object[2]
			{
				baseRsi,
				Environment.StackTrace
			});
		}
		else
		{
			_sprite.SetBaseRsi(sprite.AsNullable(), val.RSI);
		}
	}
}
