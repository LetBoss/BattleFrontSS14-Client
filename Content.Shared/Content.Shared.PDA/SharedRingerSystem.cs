using System;
using Content.Shared.PDA.Ringer;
using Content.Shared.Popups;
using Content.Shared.Store;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Network;
using Robust.Shared.Player;
using Robust.Shared.Timing;

namespace Content.Shared.PDA;

public abstract class SharedRingerSystem : EntitySystem
{
	public const int RingtoneLength = 6;

	public const int NoteTempo = 300;

	public const float NoteDelay = 0.2f;

	[Dependency]
	private IGameTiming _timing;

	[Dependency]
	private INetManager _net;

	[Dependency]
	private SharedAudioSystem _audio;

	[Dependency]
	private SharedPdaSystem _pda;

	[Dependency]
	private SharedPopupSystem _popup;

	[Dependency]
	private SharedTransformSystem _xform;

	[Dependency]
	protected SharedUserInterfaceSystem UI;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<RingerComponent, RingerSetRingtoneMessage>((EntityEventRefHandler<RingerComponent, RingerSetRingtoneMessage>)OnSetRingtone, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RingerComponent, RingerPlayRingtoneMessage>((EntityEventRefHandler<RingerComponent, RingerPlayRingtoneMessage>)OnPlayRingtone, (Type[])null, (Type[])null);
	}

	public override void Update(float frameTime)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		EntityQueryEnumerator<RingerComponent, TransformComponent> ringerQuery = ((EntitySystem)this).EntityQueryEnumerator<RingerComponent, TransformComponent>();
		EntityUid uid = default(EntityUid);
		RingerComponent ringer = default(RingerComponent);
		TransformComponent xform = default(TransformComponent);
		while (ringerQuery.MoveNext(ref uid, ref ringer, ref xform))
		{
			if (!ringer.Active || !ringer.NextNoteTime.HasValue)
			{
				continue;
			}
			TimeSpan curTime = _timing.CurTime;
			if (!(curTime < ringer.NextNoteTime.Value))
			{
				if (_net.IsServer)
				{
					SharedAudioSystem audio = _audio;
					SoundPathSpecifier sound = GetSound(ringer.Ringtone[ringer.NoteCount]);
					Filter obj = Filter.Empty().AddInRange(_xform.GetMapCoordinates(uid, xform), ringer.Range, (ISharedPlayerManager)null, (IEntityManager)null);
					EntityUid val = uid;
					AudioParams val2 = ((AudioParams)(ref AudioParams.Default)).WithMaxDistance(ringer.Range);
					audio.PlayEntity((SoundSpecifier)(object)sound, obj, val, true, (AudioParams?)((AudioParams)(ref val2)).WithVolume(ringer.Volume));
				}
				ringer.NextNoteTime = curTime + TimeSpan.FromSeconds(0.20000000298023224);
				ringer.NoteCount++;
				((EntitySystem)this).DirtyFields<RingerComponent>(uid, ringer, (MetaDataComponent)null, new string[2] { "NextNoteTime", "NoteCount" });
				if (ringer.NoteCount >= 6)
				{
					ringer.Active = false;
					ringer.NextNoteTime = null;
					ringer.NoteCount = 0;
					((EntitySystem)this).DirtyFields<RingerComponent>(uid, ringer, (MetaDataComponent)null, new string[3] { "Active", "NextNoteTime", "NoteCount" });
					UpdateRingerUi(Entity<RingerComponent>.op_Implicit((uid, ringer)));
				}
			}
		}
	}

	public void RingerPlayRingtone(Entity<RingerComponent?> ent)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<RingerComponent>(Entity<RingerComponent>.op_Implicit(ent), ref ent.Comp, true))
		{
			StartRingtone(Entity<RingerComponent>.op_Implicit((Entity<RingerComponent>.op_Implicit(ent), ent.Comp)));
		}
	}

	public bool TryToggleRingerUi(EntityUid uid, EntityUid actor)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		UI.TryToggleUi(Entity<UserInterfaceComponent>.op_Implicit(uid), (Enum)RingerUiKey.Key, actor);
		return true;
	}

	public void LockUplink(Entity<RingerUplinkComponent?> ent)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<RingerUplinkComponent>(Entity<RingerUplinkComponent>.op_Implicit(ent), ref ent.Comp, true))
		{
			ent.Comp.Unlocked = false;
			UI.CloseUi(Entity<UserInterfaceComponent>.op_Implicit(ent.Owner), (Enum)StoreUiKey.Key);
		}
	}

	public virtual bool TryToggleUplink(EntityUid uid, Note[] ringtone, EntityUid? user = null)
	{
		return false;
	}

	private void OnSetRingtone(Entity<RingerComponent> ent, ref RingerSetRingtoneMessage args)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		TimeSpan curTime = _timing.CurTime;
		if (!(ent.Comp.NextRingtoneSetTime > curTime))
		{
			ent.Comp.NextRingtoneSetTime = curTime + ent.Comp.Cooldown;
			((EntitySystem)this).DirtyField<RingerComponent>(ent.AsNullable(), "NextRingtoneSetTime", (MetaDataComponent)null);
			if (args.Ringtone.Length == 6 && !TryToggleUplink(Entity<RingerComponent>.op_Implicit(ent), args.Ringtone))
			{
				UpdateRingerRingtone(ent, args.Ringtone);
			}
		}
	}

	private void OnPlayRingtone(Entity<RingerComponent> ent, ref RingerPlayRingtoneMessage args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		StartRingtone(ent);
	}

	private void StartRingtone(Entity<RingerComponent> ent)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		if (!ent.Comp.Active)
		{
			ent.Comp.Active = true;
			ent.Comp.NoteCount = 0;
			ent.Comp.NextNoteTime = _timing.CurTime;
			UpdateRingerUi(ent);
			_popup.PopupPredicted(base.Loc.GetString("comp-ringer-vibration-popup"), Entity<RingerComponent>.op_Implicit(ent), ent.Owner, Filter.Pvs(Entity<RingerComponent>.op_Implicit(ent), 0.05f, (IEntityManager)null, (ISharedPlayerManager)null, (IConfigurationManager)null), recordReplay: false, PopupType.Medium);
			((EntitySystem)this).DirtyFields<RingerComponent>(ent.AsNullable(), (MetaDataComponent)null, new string[3] { "NextNoteTime", "Active", "NoteCount" });
		}
	}

	protected void UpdateRingerRingtone(Entity<RingerComponent> ent, Note[] ringtone)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		ent.Comp.Ringtone = ringtone;
		((EntitySystem)this).DirtyField<RingerComponent>(ent.AsNullable(), "Ringtone", (MetaDataComponent)null);
		UpdateRingerUi(ent);
	}

	protected bool ToggleUplinkInternal(Entity<RingerUplinkComponent> ent)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		ent.Comp.Unlocked = !ent.Comp.Unlocked;
		PdaComponent pda = default(PdaComponent);
		if (((EntitySystem)this).TryComp<PdaComponent>(Entity<RingerUplinkComponent>.op_Implicit(ent), ref pda))
		{
			_pda.UpdatePdaUi(Entity<RingerUplinkComponent>.op_Implicit(ent), pda);
		}
		if (!ent.Comp.Unlocked)
		{
			UI.CloseUi(Entity<UserInterfaceComponent>.op_Implicit(ent.Owner), (Enum)StoreUiKey.Key);
		}
		return true;
	}

	private static SoundPathSpecifier GetSound(Note note)
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Expected O, but got Unknown
		return new SoundPathSpecifier("/Audio/Effects/RingtoneNotes/" + note.ToString().ToLower() + ".ogg", (AudioParams?)null);
	}

	protected virtual void UpdateRingerUi(Entity<RingerComponent> ent)
	{
	}
}
