using System;
using Content.Shared._RMC14.Xenonids.Egg;
using Robust.Client.Animations;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Client.ResourceManagement;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Serialization.TypeSerializers.Implementations;

namespace Content.Client._RMC14.Xenonids.Egg;

public sealed class XenoEggVisualizerSystem : EntitySystem
{
	[Dependency]
	private AnimationPlayerSystem _animation;

	[Dependency]
	private IResourceCache _resourceCache;

	[Dependency]
	private SpriteSystem _sprite;

	private const string AnimationKey = "rmc_egg_destroying";

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<XenoEggComponent, ComponentStartup>((EntityEventRefHandler<XenoEggComponent, ComponentStartup>)SetVisuals, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoEggComponent, XenoEggStateChangedEvent>((EntityEventRefHandler<XenoEggComponent, XenoEggStateChangedEvent>)SetVisuals, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<DestroyedXenoEggComponent, ComponentStartup>((EntityEventRefHandler<DestroyedXenoEggComponent, ComponentStartup>)OnStartup, (Type[])null, (Type[])null);
	}

	private void SetVisuals<T>(Entity<XenoEggComponent> ent, ref T args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		SpriteComponent val = default(SpriteComponent);
		if (!((EntitySystem)this).TryComp<SpriteComponent>(Entity<XenoEggComponent>.op_Implicit(ent), ref val))
		{
			return;
		}
		string currentSprite = ent.Comp.CurrentSprite;
		RSIResource val2 = default(RSIResource);
		if (_resourceCache.TryGetResource<RSIResource>(SpriteSpecifierSerializer.TextureRoot / currentSprite, ref val2))
		{
			if (val.BaseRSI != val2.RSI)
			{
				_sprite.SetBaseRsi(Entity<SpriteComponent>.op_Implicit((ent.Owner, val)), val2.RSI);
			}
			string text = ent.Comp.State switch
			{
				XenoEggState.Item => ent.Comp.ItemState, 
				XenoEggState.Growing => ent.Comp.GrowingState, 
				XenoEggState.Grown => ent.Comp.GrownState, 
				XenoEggState.Opened => ent.Comp.OpenedState, 
				XenoEggState.Opening => ent.Comp.OpeningState, 
				_ => null, 
			};
			int num = default(int);
			if (!string.IsNullOrWhiteSpace(text) && _sprite.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((ent.Owner, val)), (Enum)XenoEggLayers.Base, ref num, false))
			{
				_sprite.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((ent.Owner, val)), num, StateId.op_Implicit(text));
			}
		}
	}

	private void OnStartup(Entity<DestroyedXenoEggComponent> ent, ref ComponentStartup args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Expected O, but got Unknown
		//IL_008b: Expected O, but got Unknown
		if (!_animation.HasRunningAnimation(Entity<DestroyedXenoEggComponent>.op_Implicit(ent), "rmc_egg_destroying"))
		{
			_animation.Play(Entity<DestroyedXenoEggComponent>.op_Implicit(ent), new Animation
			{
				Length = ent.Comp.AnimationTime,
				AnimationTracks = { (AnimationTrack)new AnimationTrackSpriteFlick
				{
					LayerKey = ent.Comp.Layer,
					KeyFrames = 
					{
						new KeyFrame(StateId.op_Implicit(ent.Comp.AnimationState), 0f)
					}
				} }
			}, "rmc_egg_destroying");
		}
	}
}
