using System;
using System.Numerics;
using Content.Shared.Salvage.Fulton;
using Robust.Client.Animations;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Spawners;
using Robust.Shared.Utility;

namespace Content.Client.Salvage;

public sealed class FultonSystem : SharedFultonSystem
{
	public enum FultonVisualLayers : byte
	{
		Base
	}

	[Dependency]
	private ISerializationManager _serManager;

	[Dependency]
	private AnimationPlayerSystem _player;

	[Dependency]
	private SpriteSystem _sprite;

	private static readonly TimeSpan AnimationDuration = TimeSpan.FromSeconds(0.4);

	private static readonly Animation InitialAnimation = new Animation
	{
		Length = AnimationDuration,
		AnimationTracks = { (AnimationTrack)new AnimationTrackSpriteFlick
		{
			LayerKey = FultonVisualLayers.Base,
			KeyFrames = 
			{
				new KeyFrame(new StateId("fulton_expand"), 0f)
			},
			KeyFrames = 
			{
				new KeyFrame(new StateId("fulton_balloon"), 0.4f)
			}
		} }
	};

	private static readonly Animation FultonAnimation = new Animation
	{
		Length = TimeSpan.FromSeconds(0.800000011920929),
		AnimationTracks = { (AnimationTrack)new AnimationTrackComponentProperty
		{
			ComponentType = typeof(SpriteComponent),
			Property = "Offset",
			KeyFrames = 
			{
				new KeyFrame((object)Vector2.Zero, 0f, (Func<float, float>)null)
			},
			KeyFrames = 
			{
				new KeyFrame((object)new Vector2(0f, -0.3f), 0.3f, (Func<float, float>)null)
			},
			KeyFrames = 
			{
				new KeyFrame((object)new Vector2(0f, 20f), 0.5f, (Func<float, float>)null)
			}
		} }
	};

	public override void Initialize()
	{
		base.Initialize();
		((EntitySystem)this).SubscribeLocalEvent<FultonedComponent, AfterAutoHandleStateEvent>((ComponentEventRefHandler<FultonedComponent, AfterAutoHandleStateEvent>)OnHandleState, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeNetworkEvent<FultonAnimationMessage>((EntityEventHandler<FultonAnimationMessage>)OnFultonMessage, (Type[])null, (Type[])null);
	}

	private void OnFultonMessage(FultonAnimationMessage ev)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Expected O, but got Unknown
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		EntityUid entity = ((EntitySystem)this).GetEntity(ev.Entity);
		EntityCoordinates coordinates = ((EntitySystem)this).GetCoordinates(ev.Coordinates);
		SpriteComponent val = default(SpriteComponent);
		if (!((EntitySystem)this).Deleted(entity, (MetaDataComponent)null) && ((EntitySystem)this).TryComp<SpriteComponent>(entity, ref val))
		{
			EntityUid val2 = ((EntitySystem)this).Spawn((string)null, coordinates);
			SpriteComponent val3 = ((EntitySystem)this).AddComp<SpriteComponent>(val2);
			_serManager.CopyTo<SpriteComponent>(val, ref val3, (ISerializationContext)null, false, true);
			AppearanceComponent val4 = default(AppearanceComponent);
			if (((EntitySystem)this).TryComp<AppearanceComponent>(entity, ref val4))
			{
				AppearanceComponent val5 = ((EntitySystem)this).AddComp<AppearanceComponent>(val2);
				_serManager.CopyTo<AppearanceComponent>(val4, ref val5, (ISerializationContext)null, false, true);
			}
			val3.NoRotation = true;
			int num = _sprite.AddLayer(Entity<SpriteComponent>.op_Implicit((val2, val3)), (SpriteSpecifier)new Rsi(new ResPath("Objects/Tools/fulton_balloon.rsi"), "fulton_balloon"), (int?)null);
			_sprite.LayerSetOffset(Entity<SpriteComponent>.op_Implicit((val2, val3)), num, SharedFultonSystem.EffectOffset + new Vector2(0f, 0.5f));
			((EntitySystem)this).AddComp<TimedDespawnComponent>(val2).Lifetime = 1.5f;
			_player.Play(val2, FultonAnimation, "fulton-animation");
		}
	}

	private void OnHandleState(EntityUid uid, FultonedComponent component, ref AfterAutoHandleStateEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		UpdateAppearance(uid, component);
	}

	protected override void UpdateAppearance(EntityUid uid, FultonedComponent component)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		EntityUid effect = component.Effect;
		if (((EntityUid)(ref effect)).IsValid())
		{
			TimeSpan timeSpan = component.NextFulton - component.FultonDuration;
			if (!(Timing.CurTime - timeSpan >= AnimationDuration))
			{
				_player.Play(component.Effect, InitialAnimation, "fulton");
			}
		}
	}
}
