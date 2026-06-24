using System;
using System.Numerics;
using Content.Client._RMC14.ParaDrop;
using Content.Client._RMC14.Sprite;
using Content.Shared._RMC14.CrashLand;
using Content.Shared.ParaDrop;
using Robust.Client.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Client._RMC14.CrashLand;

public sealed class CrashLandSystem : SharedCrashLandSystem
{
	[Dependency]
	private AnimationPlayerSystem _animPlayer;

	[Dependency]
	private ParaDropSystem _paraDrop;

	[Dependency]
	private RMCSpriteSystem _rmcSprite;

	[Dependency]
	private SpriteSystem _sprite;

	private const string CrashingAnimationKey = "crashing-animation";

	public override void Initialize()
	{
		base.Initialize();
		((EntitySystem)this).SubscribeLocalEvent<CrashLandingComponent, ComponentRemove>((EntityEventRefHandler<CrashLandingComponent, ComponentRemove>)OnRemove, (Type[])null, (Type[])null);
	}

	private void OnRemove(Entity<CrashLandingComponent> ent, ref ComponentRemove args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).TerminatingOrDeleted(Entity<CrashLandingComponent>.op_Implicit(ent), (MetaDataComponent)null))
		{
			AnimationPlayerComponent item = default(AnimationPlayerComponent);
			if (((EntitySystem)this).TryComp<AnimationPlayerComponent>(Entity<CrashLandingComponent>.op_Implicit(ent), ref item))
			{
				_animPlayer.Stop(Entity<AnimationPlayerComponent>.op_Implicit((Entity<CrashLandingComponent>.op_Implicit(ent), item)), "crashing-animation");
			}
			SpriteComponent item2 = default(SpriteComponent);
			if (((EntitySystem)this).TryComp<SpriteComponent>(Entity<CrashLandingComponent>.op_Implicit(ent), ref item2))
			{
				_sprite.SetOffset(Entity<SpriteComponent>.op_Implicit((Entity<CrashLandingComponent>.op_Implicit(ent), item2)), default(Vector2));
			}
		}
	}

	public override void Update(float frameTime)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		base.Update(frameTime);
		EntityQueryEnumerator<CrashLandableComponent, CrashLandingComponent> val = ((EntitySystem)this).EntityQueryEnumerator<CrashLandableComponent, CrashLandingComponent>();
		EntityUid val2 = default(EntityUid);
		CrashLandableComponent crashLandableComponent = default(CrashLandableComponent);
		CrashLandingComponent crashLandingComponent = default(CrashLandingComponent);
		while (val.MoveNext(ref val2, ref crashLandableComponent, ref crashLandingComponent))
		{
			if (!((EntitySystem)this).HasComp<SkyFallingComponent>(val2))
			{
				if (!_animPlayer.HasRunningAnimation(val2, "crashing-animation") && crashLandableComponent.LastCrash.HasValue)
				{
					_paraDrop.PlayFallAnimation(val2, crashLandableComponent.CrashDuration, crashLandingComponent.RemainingTime, crashLandableComponent.FallHeight, "crashing-animation");
				}
				_rmcSprite.UpdatePosition(val2);
			}
		}
	}
}
