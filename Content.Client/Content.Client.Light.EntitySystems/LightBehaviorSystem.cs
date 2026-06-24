using System;
using System.Collections.Generic;
using System.Linq;
using Content.Client.Light.Components;
using Robust.Client.Animations;
using Robust.Client.GameObjects;
using Robust.Shared.Animations;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Random;

namespace Content.Client.Light.EntitySystems;

public sealed class LightBehaviorSystem : EntitySystem
{
	[Dependency]
	private IRobustRandom _random;

	[Dependency]
	private AnimationPlayerSystem _player;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<LightBehaviourComponent, ComponentStartup>((EntityEventRefHandler<LightBehaviourComponent, ComponentStartup>)OnLightStartup, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<LightBehaviourComponent, AnimationCompletedEvent>((ComponentEventHandler<LightBehaviourComponent, AnimationCompletedEvent>)OnBehaviorAnimationCompleted, (Type[])null, (Type[])null);
	}

	private void OnBehaviorAnimationCompleted(EntityUid uid, LightBehaviourComponent component, AnimationCompletedEvent args)
	{
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		if (args.Finished)
		{
			LightBehaviourComponent.AnimationContainer animationContainer = component.Animations.FirstOrDefault((LightBehaviourComponent.AnimationContainer x) => x.FullKey == args.Key);
			if (animationContainer != null && animationContainer.LightBehaviour.IsLooped)
			{
				animationContainer.LightBehaviour.UpdatePlaybackValues(animationContainer.Animation);
				_player.Play(uid, animationContainer.Animation, animationContainer.FullKey);
			}
		}
	}

	private void OnLightStartup(Entity<LightBehaviourComponent> entity, ref ComponentStartup args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).EnsureComp<AnimationPlayerComponent>(Entity<LightBehaviourComponent>.op_Implicit(entity));
		foreach (LightBehaviourComponent.AnimationContainer animation in entity.Comp.Animations)
		{
			animation.LightBehaviour.Initialize(Entity<LightBehaviourComponent>.op_Implicit(entity), _random, (IEntityManager)(object)base.EntityManager);
		}
		foreach (LightBehaviourComponent.AnimationContainer animation2 in entity.Comp.Animations)
		{
			if (animation2.LightBehaviour.Enabled)
			{
				StartLightBehaviour(entity, animation2.LightBehaviour.ID);
			}
		}
	}

	private void CopyLightSettings(Entity<LightBehaviourComponent> entity, string property)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		PointLightComponent val = default(PointLightComponent);
		if (((EntitySystem)this).TryComp<PointLightComponent>(Entity<LightBehaviourComponent>.op_Implicit(entity), ref val))
		{
			object animatableProperty = AnimationHelper.GetAnimatableProperty((object)val, property);
			if (animatableProperty != null)
			{
				entity.Comp.OriginalPropertyValues[property] = animatableProperty;
			}
		}
		else
		{
			((EntitySystem)this).Log.Warning($"{((EntitySystem)this).Comp<MetaDataComponent>(Entity<LightBehaviourComponent>.op_Implicit(entity)).EntityName} has a {"LightBehaviourComponent"} but it has no {"PointLightComponent"}! Check the prototype!");
		}
	}

	public void StartLightBehaviour(Entity<LightBehaviourComponent> entity, string id = "")
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		AnimationPlayerComponent val = default(AnimationPlayerComponent);
		if (!((EntitySystem)this).TryComp<AnimationPlayerComponent>(Entity<LightBehaviourComponent>.op_Implicit(entity), ref val))
		{
			return;
		}
		foreach (LightBehaviourComponent.AnimationContainer animation in entity.Comp.Animations)
		{
			if ((animation.LightBehaviour.ID == id || id == string.Empty) && !_player.HasRunningAnimation(Entity<LightBehaviourComponent>.op_Implicit(entity), val, "LightBehaviourComponent" + animation.Key))
			{
				CopyLightSettings(entity, animation.LightBehaviour.Property);
				animation.LightBehaviour.UpdatePlaybackValues(animation.Animation);
				_player.Play(Entity<LightBehaviourComponent>.op_Implicit(entity), animation.Animation, "LightBehaviourComponent" + animation.Key);
			}
		}
	}

	public void StopLightBehaviour(Entity<LightBehaviourComponent> entity, string id = "", bool removeBehaviour = false, bool resetToOriginalSettings = false)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		AnimationPlayerComponent val = default(AnimationPlayerComponent);
		if (!((EntitySystem)this).TryComp<AnimationPlayerComponent>(Entity<LightBehaviourComponent>.op_Implicit(entity), ref val))
		{
			return;
		}
		LightBehaviourComponent comp = entity.Comp;
		List<LightBehaviourComponent.AnimationContainer> list = new List<LightBehaviourComponent.AnimationContainer>();
		foreach (LightBehaviourComponent.AnimationContainer animation in comp.Animations)
		{
			if (animation.LightBehaviour.ID == id || id == string.Empty)
			{
				if (_player.HasRunningAnimation(Entity<LightBehaviourComponent>.op_Implicit(entity), val, "LightBehaviourComponent" + animation.Key))
				{
					_player.Stop(Entity<LightBehaviourComponent>.op_Implicit(entity), val, "LightBehaviourComponent" + animation.Key);
				}
				if (removeBehaviour)
				{
					list.Add(animation);
				}
			}
		}
		foreach (LightBehaviourComponent.AnimationContainer item in list)
		{
			comp.Animations.Remove(item);
		}
		PointLightComponent val2 = default(PointLightComponent);
		if (resetToOriginalSettings && ((EntitySystem)this).TryComp<PointLightComponent>(Entity<LightBehaviourComponent>.op_Implicit(entity), ref val2))
		{
			foreach (var (text2, obj2) in comp.OriginalPropertyValues)
			{
				AnimationHelper.SetAnimatableProperty((object)val2, text2, obj2);
			}
		}
		comp.OriginalPropertyValues.Clear();
	}

	public bool HasRunningBehaviours(Entity<LightBehaviourComponent> entity)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		AnimationPlayerComponent animation = default(AnimationPlayerComponent);
		if (!((EntitySystem)this).TryComp<AnimationPlayerComponent>(Entity<LightBehaviourComponent>.op_Implicit(entity), ref animation))
		{
			return false;
		}
		return entity.Comp.Animations.Any((LightBehaviourComponent.AnimationContainer container) => _player.HasRunningAnimation(Entity<LightBehaviourComponent>.op_Implicit(entity), animation, "LightBehaviourComponent" + container.Key));
	}

	public void AddNewLightBehaviour(Entity<LightBehaviourComponent> entity, LightBehaviourAnimationTrack behaviour, bool playImmediately = true)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Expected O, but got Unknown
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		int key = 0;
		LightBehaviourComponent comp;
		for (comp = entity.Comp; comp.Animations.Any((LightBehaviourComponent.AnimationContainer x) => x.Key == key); key++)
		{
		}
		Animation animation = new Animation
		{
			AnimationTracks = { (AnimationTrack)(object)behaviour }
		};
		behaviour.Initialize(entity.Owner, _random, (IEntityManager)(object)base.EntityManager);
		LightBehaviourComponent.AnimationContainer item = new LightBehaviourComponent.AnimationContainer(key, animation, behaviour);
		comp.Animations.Add(item);
		if (playImmediately)
		{
			StartLightBehaviour(entity, behaviour.ID);
		}
	}
}
