using System;
using Content.Shared.Mobs;
using Content.Shared.Mobs.Systems;
using Content.Shared.Sound;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Network;
using Robust.Shared.Random;
using Robust.Shared.Timing;

namespace Content.Shared._RMC14.Sound;

public sealed class CMSoundSystem : EntitySystem
{
	[Dependency]
	private SharedAudioSystem _audio;

	[Dependency]
	private SharedEmitSoundSystem _emitSound;

	[Dependency]
	private MobStateSystem _mobState;

	[Dependency]
	private INetManager _net;

	[Dependency]
	private IRobustRandom _random;

	[Dependency]
	private IGameTiming _timing;

	[Dependency]
	private SharedTransformSystem _transform;

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<RMCEmitSoundOnSpawnComponent, MapInitEvent>((EntityEventRefHandler<RMCEmitSoundOnSpawnComponent, MapInitEvent>)OnEmitSpawnOnInit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RandomSoundComponent, MapInitEvent>((EntityEventRefHandler<RandomSoundComponent, MapInitEvent>)OnRandomMapInit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<SoundOnDeathComponent, MobStateChangedEvent>((EntityEventRefHandler<SoundOnDeathComponent, MobStateChangedEvent>)OnDeathMobStateChanged, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<SoundOnDeathComponent, EntityTerminatingEvent>((EntityEventRefHandler<SoundOnDeathComponent, EntityTerminatingEvent>)OnDeathMobTerminating, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<SoundOnDeathSoundComponent, EntityTerminatingEvent>((EntityEventRefHandler<SoundOnDeathSoundComponent, EntityTerminatingEvent>)OnDeathSoundTerminating, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<EmitSoundOnActionComponent, SoundActionEvent>((EntityEventRefHandler<EmitSoundOnActionComponent, SoundActionEvent>)OnEmitSoundOnAction, (Type[])null, (Type[])null);
	}

	private void OnEmitSpawnOnInit(Entity<RMCEmitSoundOnSpawnComponent> ent, ref MapInitEvent args)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		if (!_net.IsClient && ent.Comp.Sound != null)
		{
			ent.Comp.Entity = _audio.PlayPvs(ent.Comp.Sound, ent.Owner, (AudioParams?)null)?.Item1;
			EntityCoordinates coordinates = _transform.GetMoverCoordinates(Entity<RMCEmitSoundOnSpawnComponent>.op_Implicit(ent));
			if (!((EntitySystem)this).TerminatingOrDeleted(coordinates.EntityId, (MetaDataComponent)null) && ent.Comp.Entity.HasValue)
			{
				_transform.SetCoordinates(ent.Comp.Entity.Value, coordinates);
				((EntitySystem)this).QueueDel((EntityUid?)ent.Owner);
			}
		}
	}

	private void OnRandomMapInit(Entity<RandomSoundComponent> ent, ref MapInitEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		TimeSpan min = ent.Comp.Min;
		TimeSpan max = ent.Comp.Max;
		if (max <= min)
		{
			max = min.Add(TimeSpan.FromTicks(1L));
		}
		ent.Comp.PlayAt = _timing.CurTime + _random.Next(min, max);
		((EntitySystem)this).Dirty<RandomSoundComponent>(ent, (MetaDataComponent)null);
	}

	private void OnDeathMobStateChanged(Entity<SoundOnDeathComponent> ent, ref MobStateChangedEvent args)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		if (args.NewMobState == MobState.Dead && _net.IsServer)
		{
			ent.Comp.Entity = _audio.PlayPvs(ent.Comp.Sound, Entity<SoundOnDeathComponent>.op_Implicit(ent), (AudioParams?)null)?.Item1;
			((EntitySystem)this).Dirty<SoundOnDeathComponent>(ent, (MetaDataComponent)null);
		}
	}

	private void OnDeathMobTerminating(Entity<SoundOnDeathComponent> ent, ref EntityTerminatingEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		if (ent.Comp.Entity.HasValue && !((EntitySystem)this).TerminatingOrDeleted(ent.Comp.Entity, (MetaDataComponent)null))
		{
			EntityCoordinates coordinates = _transform.GetMoverCoordinates(Entity<SoundOnDeathComponent>.op_Implicit(ent));
			if (!((EntitySystem)this).TerminatingOrDeleted(coordinates.EntityId, (MetaDataComponent)null))
			{
				_transform.SetCoordinates(ent.Comp.Entity.Value, coordinates);
				ent.Comp.Entity = null;
			}
		}
	}

	private void OnDeathSoundTerminating(Entity<SoundOnDeathSoundComponent> ent, ref EntityTerminatingEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? parent = ent.Comp.Parent;
		ent.Comp.Parent = null;
		SoundOnDeathComponent death = default(SoundOnDeathComponent);
		if (((EntitySystem)this).TryComp<SoundOnDeathComponent>(parent, ref death))
		{
			death.Entity = null;
			((EntitySystem)this).Dirty(parent.Value, (IComponent)(object)death, (MetaDataComponent)null);
		}
	}

	private void OnEmitSoundOnAction(Entity<EmitSoundOnActionComponent> ent, ref SoundActionEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		_emitSound.TryEmitSound(Entity<EmitSoundOnActionComponent>.op_Implicit(ent), Entity<EmitSoundOnActionComponent>.op_Implicit(ent), args.Performer);
		if (ent.Comp.Handle)
		{
			((HandledEntityEventArgs)args).Handled = true;
		}
	}

	public override void Update(float frameTime)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		if (_net.IsClient)
		{
			return;
		}
		TimeSpan time = _timing.CurTime;
		EntityQueryEnumerator<RandomSoundComponent> random = ((EntitySystem)this).EntityQueryEnumerator<RandomSoundComponent>();
		EntityUid uid = default(EntityUid);
		RandomSoundComponent comp = default(RandomSoundComponent);
		while (random.MoveNext(ref uid, ref comp))
		{
			if (!_mobState.IsDead(uid))
			{
				TimeSpan value = time;
				TimeSpan? playAt = comp.PlayAt;
				if (!(value <= playAt))
				{
					comp.PlayAt = time + _random.Next(comp.Min, comp.Max);
					((EntitySystem)this).Dirty(uid, (IComponent)(object)comp, (MetaDataComponent)null);
					_audio.PlayPvs(comp.Sound, uid, (AudioParams?)null);
				}
			}
		}
	}
}
