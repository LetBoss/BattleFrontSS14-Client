using System;
using Content.Shared.IgnitionSource.Components;
using Content.Shared.Interaction;
using Content.Shared.Item;
using Content.Shared.Smoking;
using Content.Shared.Temperature;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Timing;

namespace Content.Shared.IgnitionSource.EntitySystems;

public sealed class MatchstickSystem : EntitySystem
{
	[Dependency]
	private SharedAppearanceSystem _appearance;

	[Dependency]
	private SharedAudioSystem _audio;

	[Dependency]
	private SharedItemSystem _item;

	[Dependency]
	private SharedPointLightSystem _lights;

	[Dependency]
	private IGameTiming _timing;

	[Dependency]
	private SharedIgnitionSourceSystem _ignition;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<MatchstickComponent, InteractUsingEvent>((EntityEventRefHandler<MatchstickComponent, InteractUsingEvent>)OnInteractUsing, (Type[])null, (Type[])null);
	}

	private void OnInteractUsing(Entity<MatchstickComponent> ent, ref InteractUsingEvent args)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		if (!((HandledEntityEventArgs)args).Handled)
		{
			IsHotEvent isHotEvent = new IsHotEvent();
			((EntitySystem)this).RaiseLocalEvent<IsHotEvent>(args.Used, isHotEvent, false);
			if (isHotEvent.IsHot)
			{
				((HandledEntityEventArgs)args).Handled = TryIgnite(ent, args.User);
			}
		}
	}

	public bool TryIgnite(Entity<MatchstickComponent> matchstick, EntityUid? user)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		if (matchstick.Comp.State != SmokableState.Unlit)
		{
			return false;
		}
		_audio.PlayPredicted(matchstick.Comp.IgniteSound, Entity<MatchstickComponent>.op_Implicit(matchstick), user, (AudioParams?)null);
		SetState(matchstick, SmokableState.Lit);
		matchstick.Comp.TimeMatchWillBurnOut = _timing.CurTime + matchstick.Comp.Duration;
		((EntitySystem)this).Dirty<MatchstickComponent>(matchstick, (MetaDataComponent)null);
		return true;
	}

	private void SetState(Entity<MatchstickComponent> ent, SmokableState newState)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		_lights.SetEnabled(Entity<MatchstickComponent>.op_Implicit(ent), newState == SmokableState.Lit, (SharedPointLightComponent)null, (MetaDataComponent)null);
		_appearance.SetData(Entity<MatchstickComponent>.op_Implicit(ent), (Enum)SmokingVisuals.Smoking, (object)newState, (AppearanceComponent)null);
		_ignition.SetIgnited(Entity<IgnitionSourceComponent>.op_Implicit(ent.Owner), newState == SmokableState.Lit);
		if (newState == SmokableState.Lit)
		{
			_item.SetHeldPrefix(Entity<MatchstickComponent>.op_Implicit(ent), "lit");
		}
		else
		{
			_item.SetHeldPrefix(Entity<MatchstickComponent>.op_Implicit(ent), "unlit");
		}
		ent.Comp.State = newState;
		((EntitySystem)this).Dirty<MatchstickComponent>(ent, (MetaDataComponent)null);
	}

	public override void Update(float frameTime)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).Update(frameTime);
		EntityQueryEnumerator<MatchstickComponent> query = ((EntitySystem)this).EntityQueryEnumerator<MatchstickComponent>();
		EntityUid uid = default(EntityUid);
		MatchstickComponent match = default(MatchstickComponent);
		while (query.MoveNext(ref uid, ref match))
		{
			if (match.State == SmokableState.Lit)
			{
				TimeSpan curTime = _timing.CurTime;
				TimeSpan? timeMatchWillBurnOut = match.TimeMatchWillBurnOut;
				if (curTime > timeMatchWillBurnOut)
				{
					SetState(Entity<MatchstickComponent>.op_Implicit((uid, match)), SmokableState.Burnt);
				}
			}
		}
	}
}
