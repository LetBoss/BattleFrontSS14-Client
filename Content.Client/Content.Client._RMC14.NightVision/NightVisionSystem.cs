using System;
using Content.Shared._RMC14.NightVision;
using Content.Shared._RMC14.Xenonids;
using Content.Shared._RMC14.Xenonids.Burrow;
using Content.Shared.Examine;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Client.Player;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Player;

namespace Content.Client._RMC14.NightVision;

public sealed class NightVisionSystem : SharedNightVisionSystem
{
	[Dependency]
	private IEntityManager _entity;

	[Dependency]
	private ILightManager _light;

	[Dependency]
	private IOverlayManager _overlay;

	[Dependency]
	private IPlayerManager _player;

	[Dependency]
	private SharedEyeSystem _eye;

	[Dependency]
	private ExamineSystemShared _examine;

	[Dependency]
	private SpriteSystem _sprite;

	private EntityQuery<XenoComponent> _xenoQuery;

	private EntityQuery<NightVisionComponent> _nvQuery;

	private static readonly bool DisableMesons = true;

	public override void Initialize()
	{
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		base.Initialize();
		((EntitySystem)this).SubscribeLocalEvent<NightVisionComponent, LocalPlayerAttachedEvent>((EntityEventRefHandler<NightVisionComponent, LocalPlayerAttachedEvent>)OnNightVisionAttached, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<NightVisionComponent, LocalPlayerDetachedEvent>((EntityEventRefHandler<NightVisionComponent, LocalPlayerDetachedEvent>)OnNightVisionDetached, (Type[])null, (Type[])null);
		_xenoQuery = _entity.GetEntityQuery<XenoComponent>();
		_nvQuery = _entity.GetEntityQuery<NightVisionComponent>();
	}

	private void OnNightVisionAttached(Entity<NightVisionComponent> ent, ref LocalPlayerAttachedEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		NightVisionChanged(ent);
	}

	private void OnNightVisionDetached(Entity<NightVisionComponent> ent, ref LocalPlayerDetachedEvent args)
	{
		Off();
	}

	protected override void NightVisionChanged(Entity<NightVisionComponent> ent)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		EntityUid val = Entity<NightVisionComponent>.op_Implicit(ent);
		EntityUid? localEntity = ((ISharedPlayerManager)_player).LocalEntity;
		if (localEntity.HasValue && !(val != localEntity.GetValueOrDefault()))
		{
			switch (ent.Comp.State)
			{
			case NightVisionState.Off:
				Off();
				break;
			case NightVisionState.Half:
				Half(ent);
				break;
			case NightVisionState.Full:
				Full(ent);
				break;
			default:
				throw new ArgumentOutOfRangeException();
			}
		}
	}

	protected override void NightVisionRemoved(Entity<NightVisionComponent> ent)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		EntityUid val = Entity<NightVisionComponent>.op_Implicit(ent);
		EntityUid? localEntity = ((ISharedPlayerManager)_player).LocalEntity;
		if (localEntity.HasValue && !(val != localEntity.GetValueOrDefault()))
		{
			Off();
		}
	}

	private void SetMesons(bool on)
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		if (!DisableMesons && ((ISharedPlayerManager)_player).LocalEntity.HasValue)
		{
			_eye.SetDrawFov(((ISharedPlayerManager)_player).LocalEntity.Value, !on, (EyeComponent)null);
		}
	}

	private void Off()
	{
		_overlay.RemoveOverlay<NightVisionOverlay>();
		_overlay.RemoveOverlay<NightVisionFilterOverlay>();
		_overlay.RemoveOverlay<HalfNightVisionBrightnessOverlay>();
		_light.DrawLighting = true;
		SetMesons(on: false);
		SetMesonSprites(mesons: false);
	}

	private void Half(Entity<NightVisionComponent> ent)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		if (ent.Comp.Overlay)
		{
			_overlay.AddOverlay((Overlay)(object)new NightVisionOverlay());
		}
		if (ent.Comp.Green)
		{
			_overlay.AddOverlay((Overlay)(object)new NightVisionFilterOverlay());
		}
		_overlay.AddOverlay((Overlay)(object)new HalfNightVisionBrightnessOverlay());
		_light.DrawLighting = true;
		SetMesons(ent.Comp.Mesons);
	}

	private void Full(Entity<NightVisionComponent> ent)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		if (ent.Comp.Overlay)
		{
			_overlay.AddOverlay((Overlay)(object)new NightVisionOverlay());
		}
		if (ent.Comp.Green)
		{
			_overlay.AddOverlay((Overlay)(object)new NightVisionFilterOverlay());
		}
		_light.DrawLighting = false;
		SetMesons(ent.Comp.Mesons);
	}

	private void SetMesonSprites(bool mesons)
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		if (DisableMesons || !((ISharedPlayerManager)_player).LocalEntity.HasValue)
		{
			return;
		}
		bool flag = _xenoQuery.HasComp(((ISharedPlayerManager)_player).LocalEntity.Value);
		EntityQueryEnumerator<RMCMesonsNonviewableComponent, SpriteComponent> val = ((EntitySystem)this).EntityQueryEnumerator<RMCMesonsNonviewableComponent, SpriteComponent>();
		EntityUid val2 = default(EntityUid);
		RMCMesonsNonviewableComponent rMCMesonsNonviewableComponent = default(RMCMesonsNonviewableComponent);
		SpriteComponent item = default(SpriteComponent);
		XenoBurrowComponent xenoBurrowComponent = default(XenoBurrowComponent);
		while (val.MoveNext(ref val2, ref rMCMesonsNonviewableComponent, ref item))
		{
			if (flag && rMCMesonsNonviewableComponent.XenoVisible)
			{
				_sprite.SetVisible(Entity<SpriteComponent>.op_Implicit((val2, item)), true);
			}
			else if (!((EntitySystem)this).TryComp<XenoBurrowComponent>(val2, ref xenoBurrowComponent) || !xenoBurrowComponent.Active)
			{
				_sprite.SetVisible(Entity<SpriteComponent>.op_Implicit((val2, item)), !mesons || _examine.InRangeUnOccluded(((ISharedPlayerManager)_player).LocalEntity.Value, val2));
			}
		}
	}

	public override void Update(float frameTime)
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		NightVisionComponent nightVisionComponent = default(NightVisionComponent);
		if (((ISharedPlayerManager)_player).LocalEntity.HasValue && _nvQuery.TryComp(((ISharedPlayerManager)_player).LocalEntity.Value, ref nightVisionComponent) && nightVisionComponent.State != NightVisionState.Off)
		{
			SetMesonSprites(nightVisionComponent.Mesons);
		}
	}
}
