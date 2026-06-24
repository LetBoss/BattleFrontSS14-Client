using System;
using System.Numerics;
using Content.Client.Pointing.Components;
using Content.Shared.Pointing;
using Content.Shared.Verbs;
using Robust.Client.Animations;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.Animations;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Utility;

namespace Content.Client.Pointing;

public sealed class PointingSystem : SharedPointingSystem
{
	[Dependency]
	private SpriteSystem _sprite;

	[Dependency]
	private IEyeManager _eyeManager;

	[Dependency]
	private AnimationPlayerSystem _animationPlayer;

	[Dependency]
	private TransformSystem _transformSystem;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<GetVerbsEvent<Verb>>((EntityEventHandler<GetVerbsEvent<Verb>>)AddPointingVerb, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<PointingArrowComponent, ComponentStartup>((ComponentEventHandler<PointingArrowComponent, ComponentStartup>)OnArrowStartup, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RoguePointingArrowComponent, ComponentStartup>((ComponentEventHandler<RoguePointingArrowComponent, ComponentStartup>)OnRogueArrowStartup, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<PointingArrowComponent, ComponentHandleState>((EntityEventRefHandler<PointingArrowComponent, ComponentHandleState>)HandleCompState, (Type[])null, (Type[])null);
		InitializeVisualizer();
	}

	private void AddPointingVerb(GetVerbsEvent<Verb> args)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Expected O, but got Unknown
		if (!((EntitySystem)this).IsClientSide(args.Target, (MetaDataComponent)null) && !((EntitySystem)this).HasComp<PointingArrowComponent>(args.Target) && CanPoint(args.User))
		{
			Verb item = new Verb
			{
				Text = ((EntitySystem)this).Loc.GetString("pointing-verb-get-data-text"),
				Icon = (SpriteSpecifier?)new Texture(new ResPath("/Textures/Interface/VerbIcons/point.svg.192dpi.png")),
				ClientExclusive = true,
				Act = delegate
				{
					//IL_0012: Unknown result type (might be due to invalid IL or missing references)
					//IL_0018: Unknown result type (might be due to invalid IL or missing references)
					((EntitySystem)this).RaiseNetworkEvent((EntityEventArgs)(object)new PointingAttemptEvent(((EntitySystem)this).GetNetEntity(args.Target, (MetaDataComponent)null)));
				}
			};
			args.Verbs.Add(item);
		}
	}

	private void OnArrowStartup(EntityUid uid, PointingArrowComponent component, ComponentStartup args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		SpriteComponent item = default(SpriteComponent);
		if (((EntitySystem)this).TryComp<SpriteComponent>(uid, ref item))
		{
			_sprite.SetDrawDepth(Entity<SpriteComponent>.op_Implicit((uid, item)), 13);
		}
		BeginPointAnimation(uid, component.StartPosition, component.Offset, component.AnimationKey);
	}

	private void OnRogueArrowStartup(EntityUid uid, RoguePointingArrowComponent arrow, ComponentStartup args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		SpriteComponent val = default(SpriteComponent);
		if (((EntitySystem)this).TryComp<SpriteComponent>(uid, ref val))
		{
			_sprite.SetDrawDepth(Entity<SpriteComponent>.op_Implicit((uid, val)), 13);
			val.NoRotation = false;
		}
	}

	private void HandleCompState(Entity<PointingArrowComponent> entity, ref ComponentHandleState args)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		if (((ComponentHandleState)(ref args)).Current is SharedPointingArrowComponentState sharedPointingArrowComponentState)
		{
			entity.Comp.StartPosition = sharedPointingArrowComponentState.StartPosition;
			entity.Comp.EndTime = sharedPointingArrowComponentState.EndTime;
		}
	}

	public void InitializeVisualizer()
	{
		((EntitySystem)this).SubscribeLocalEvent<PointingArrowComponent, AnimationCompletedEvent>((ComponentEventHandler<PointingArrowComponent, AnimationCompletedEvent>)OnAnimationCompleted, (Type[])null, (Type[])null);
	}

	private void OnAnimationCompleted(EntityUid uid, PointingArrowComponent component, AnimationCompletedEvent args)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		if (args.Key == component.AnimationKey)
		{
			_animationPlayer.Stop(Entity<AnimationPlayerComponent>.op_Implicit(uid), component.AnimationKey);
		}
	}

	private void BeginPointAnimation(EntityUid uid, Vector2 startPosition, Vector2 offset, string animationKey)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01df: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0205: Expected O, but got Unknown
		//IL_0206: Expected O, but got Unknown
		//IL_020c: Unknown result type (might be due to invalid IL or missing references)
		if (!_animationPlayer.HasRunningAnimation(uid, animationKey))
		{
			Angle val = new Angle(Angle.op_Implicit(_eyeManager.CurrentEye.Rotation + ((SharedTransformSystem)_transformSystem).GetWorldRotation(uid)));
			startPosition = ((Angle)(ref val)).RotateVec(ref startPosition);
			Animation val2 = new Animation
			{
				Length = PointDuration,
				AnimationTracks = { (AnimationTrack)new AnimationTrackComponentProperty
				{
					ComponentType = typeof(SpriteComponent),
					Property = "Offset",
					InterpolationMode = (AnimationInterpolationMode)1,
					KeyFrames = 
					{
						new KeyFrame((object)startPosition, 0f, (Func<float, float>)null)
					},
					KeyFrames = 
					{
						new KeyFrame((object)Vector2.Lerp(startPosition, offset, 0.9f), PointKeyTimeMove, (Func<float, float>)null)
					},
					KeyFrames = 
					{
						new KeyFrame((object)offset, PointKeyTimeMove, (Func<float, float>)null)
					},
					KeyFrames = 
					{
						new KeyFrame((object)Vector2.Zero, PointKeyTimeMove, (Func<float, float>)null)
					},
					KeyFrames = 
					{
						new KeyFrame((object)offset, PointKeyTimeHover, (Func<float, float>)null)
					},
					KeyFrames = 
					{
						new KeyFrame((object)Vector2.Zero, PointKeyTimeHover, (Func<float, float>)null)
					},
					KeyFrames = 
					{
						new KeyFrame((object)offset, PointKeyTimeHover, (Func<float, float>)null)
					},
					KeyFrames = 
					{
						new KeyFrame((object)Vector2.Zero, PointKeyTimeHover, (Func<float, float>)null)
					},
					KeyFrames = 
					{
						new KeyFrame((object)offset, PointKeyTimeHover, (Func<float, float>)null)
					},
					KeyFrames = 
					{
						new KeyFrame((object)Vector2.Zero, PointKeyTimeHover, (Func<float, float>)null)
					},
					KeyFrames = 
					{
						new KeyFrame((object)offset, PointKeyTimeHover, (Func<float, float>)null)
					},
					KeyFrames = 
					{
						new KeyFrame((object)Vector2.Zero, PointKeyTimeHover, (Func<float, float>)null)
					}
				} }
			};
			_animationPlayer.Play(uid, val2, animationKey);
		}
	}
}
