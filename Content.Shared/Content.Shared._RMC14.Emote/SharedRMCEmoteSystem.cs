using System;
using Content.Shared._RMC14.CCVar;
using Content.Shared._RMC14.Weapons.Melee;
using Content.Shared._RMC14.Xenonids;
using Content.Shared.Chat.Prototypes;
using Content.Shared.Coordinates;
using Content.Shared.Interaction;
using Content.Shared.Movement.Events;
using Content.Shared.Popups;
using Content.Shared.Verbs;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;
using Robust.Shared.Utility;

namespace Content.Shared._RMC14.Emote;

public abstract class SharedRMCEmoteSystem : EntitySystem
{
	[Dependency]
	private IConfigurationManager _config;

	[Dependency]
	private INetManager _net;

	[Dependency]
	private IGameTiming _timing;

	[Dependency]
	private RotateToFaceSystem _rotate;

	[Dependency]
	private SharedAudioSystem _audio;

	[Dependency]
	private SharedRMCMeleeWeaponSystem _melee;

	[Dependency]
	private SharedPopupSystem _popup;

	[Dependency]
	private SharedInteractionSystem _interaction;

	[Dependency]
	private SharedTransformSystem _transform;

	private TimeSpan _emoteCooldown;

	private readonly float _interactRange = 1f;

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<RMCHandEmotesComponent, InteractHandEvent>((EntityEventRefHandler<RMCHandEmotesComponent, InteractHandEvent>)OnInteractHand, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCHandEmotesComponent, GetVerbsEvent<InteractionVerb>>((EntityEventRefHandler<RMCHandEmotesComponent, GetVerbsEvent<InteractionVerb>>)OnGetInteractionVerbs, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCHandEmotesComponent, MoveInputEvent>((EntityEventRefHandler<RMCHandEmotesComponent, MoveInputEvent>)OnMove, (Type[])null, (Type[])null);
		EntitySystemSubscriptionExt.CVar<float>(((EntitySystem)this).Subs, _config, RMCCVars.RMCEmoteCooldownSeconds, (Action<float>)delegate(float v)
		{
			_emoteCooldown = TimeSpan.FromSeconds(v);
		}, true);
	}

	public virtual void TryEmoteWithChat(EntityUid source, ProtoId<EmotePrototype> emote, bool hideLog = false, string? nameOverride = null, bool ignoreActionBlocker = false, bool forceEmote = false, TimeSpan? cooldown = null)
	{
	}

	public bool TryEmote(Entity<EmoteCooldownComponent?> cooldown)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<EmoteCooldownComponent>(Entity<EmoteCooldownComponent>.op_Implicit(cooldown), ref cooldown.Comp, false))
		{
			return true;
		}
		TimeSpan time = _timing.CurTime;
		if (time < cooldown.Comp.NextEmote)
		{
			return false;
		}
		cooldown.Comp.NextEmote = time + _emoteCooldown;
		((EntitySystem)this).Dirty<EmoteCooldownComponent>(cooldown, (MetaDataComponent)null);
		return true;
	}

	public void ResetCooldown(Entity<EmoteCooldownComponent?> cooldown)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<EmoteCooldownComponent>(Entity<EmoteCooldownComponent>.op_Implicit(cooldown), ref cooldown.Comp, false))
		{
			cooldown.Comp.NextEmote = _timing.CurTime + _emoteCooldown;
			((EntitySystem)this).Dirty<EmoteCooldownComponent>(cooldown, (MetaDataComponent)null);
		}
	}

	private void OnInteractHand(Entity<RMCHandEmotesComponent> ent, ref InteractHandEvent args)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		if (((HandledEntityEventArgs)args).Handled)
		{
			return;
		}
		EntityUid user = args.User;
		RMCHandEmotesComponent compUser = default(RMCHandEmotesComponent);
		if (user == args.Target || !((EntitySystem)this).TryComp<RMCHandEmotesComponent>(user, ref compUser) || !ent.Comp.Active || compUser.Active)
		{
			return;
		}
		EntityUid val = user;
		EntityUid? target = ent.Comp.Target;
		if (target.HasValue && !(val != target.GetValueOrDefault()) && (ent.Comp.State != RMCHandsEmoteState.Tailswipe || ((EntitySystem)this).HasComp<XenoComponent>(user)))
		{
			if (!_interaction.InRangeUnobstructed(Entity<TransformComponent>.op_Implicit(user), Entity<TransformComponent>.op_Implicit(args.Target), _interactRange))
			{
				string msg = base.Loc.GetString("rmc-hands-emotes-get-closer");
				_popup.PopupClient(msg, user, user);
			}
			else
			{
				((HandledEntityEventArgs)args).Handled = true;
				PerformEmote(ent, Entity<RMCHandEmotesComponent>.op_Implicit((user, compUser)));
			}
		}
	}

	private void OnGetInteractionVerbs(Entity<RMCHandEmotesComponent> ent, ref GetVerbsEvent<InteractionVerb> args)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Expected O, but got Unknown
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Expected O, but got Unknown
		//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e3: Expected O, but got Unknown
		//IL_022b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0235: Unknown result type (might be due to invalid IL or missing references)
		//IL_023f: Expected O, but got Unknown
		RMCHandEmotesComponent selfComp = default(RMCHandEmotesComponent);
		if (!args.CanAccess || !args.CanInteract || args.Hands == null || !((EntitySystem)this).TryComp<RMCHandEmotesComponent>(args.User, ref selfComp) || ent.Comp.Active || selfComp.Active || ent.Owner == args.User)
		{
			return;
		}
		EntityUid user = args.User;
		if (((EntitySystem)this).HasComp<XenoComponent>(user) && ((EntitySystem)this).HasComp<XenoComponent>(ent.Owner))
		{
			args.Verbs.Add(new InteractionVerb
			{
				Act = delegate
				{
					//IL_0007: Unknown result type (might be due to invalid IL or missing references)
					//IL_0017: Unknown result type (might be due to invalid IL or missing references)
					//IL_001d: Unknown result type (might be due to invalid IL or missing references)
					AttemptEmote(Entity<RMCHandEmotesComponent>.op_Implicit((user, selfComp)), ent, RMCHandsEmoteState.Tailswipe);
				},
				Text = base.Loc.GetString("rmc-hands-emotes-tailswipe-perform"),
				Priority = -27,
				Icon = (SpriteSpecifier?)new Rsi(new ResPath("_RMC14/Effects/emotes.rsi"), "emote_tailswipe")
			});
		}
		else if (!((EntitySystem)this).HasComp<XenoComponent>(user) && !((EntitySystem)this).HasComp<XenoComponent>(ent.Owner))
		{
			args.Verbs.Add(new InteractionVerb
			{
				Act = delegate
				{
					//IL_0007: Unknown result type (might be due to invalid IL or missing references)
					//IL_0017: Unknown result type (might be due to invalid IL or missing references)
					//IL_001d: Unknown result type (might be due to invalid IL or missing references)
					AttemptEmote(Entity<RMCHandEmotesComponent>.op_Implicit((user, selfComp)), ent, RMCHandsEmoteState.Fistbump);
				},
				Text = base.Loc.GetString("rmc-hands-emotes-fistbump-perform"),
				Priority = -25,
				Icon = (SpriteSpecifier?)new Rsi(new ResPath("_RMC14/Effects/emotes.rsi"), "emote_fistbump")
			});
			args.Verbs.Add(new InteractionVerb
			{
				Act = delegate
				{
					//IL_0007: Unknown result type (might be due to invalid IL or missing references)
					//IL_0017: Unknown result type (might be due to invalid IL or missing references)
					//IL_001d: Unknown result type (might be due to invalid IL or missing references)
					AttemptEmote(Entity<RMCHandEmotesComponent>.op_Implicit((user, selfComp)), ent, RMCHandsEmoteState.Highfive);
				},
				Text = base.Loc.GetString("rmc-hands-emotes-highfive-perform"),
				Priority = -26,
				Icon = (SpriteSpecifier?)new Rsi(new ResPath("_RMC14/Effects/emotes.rsi"), "emote_highfive")
			});
			args.Verbs.Add(new InteractionVerb
			{
				Act = delegate
				{
					//IL_0007: Unknown result type (might be due to invalid IL or missing references)
					//IL_0017: Unknown result type (might be due to invalid IL or missing references)
					//IL_001d: Unknown result type (might be due to invalid IL or missing references)
					AttemptEmote(Entity<RMCHandEmotesComponent>.op_Implicit((user, selfComp)), ent, RMCHandsEmoteState.Hug);
				},
				Text = base.Loc.GetString("rmc-hands-emotes-hug-perform"),
				Priority = -28,
				Icon = (SpriteSpecifier?)new Rsi(new ResPath("_RMC14/Effects/emotes.rsi"), "emote_hug")
			});
		}
	}

	private void OnMove(Entity<RMCHandEmotesComponent> ent, ref MoveInputEvent args)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		if (args.HasDirectionalMovement)
		{
			CancelHandEmotes(ent);
		}
	}

	public void AttemptEmote(Entity<RMCHandEmotesComponent> ent, Entity<RMCHandEmotesComponent> target, RMCHandsEmoteState state)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0219: Unknown result type (might be due to invalid IL or missing references)
		//IL_0229: Unknown result type (might be due to invalid IL or missing references)
		//IL_0246: Unknown result type (might be due to invalid IL or missing references)
		//IL_0252: Unknown result type (might be due to invalid IL or missing references)
		//IL_0258: Unknown result type (might be due to invalid IL or missing references)
		//IL_0259: Unknown result type (might be due to invalid IL or missing references)
		//IL_0268: Unknown result type (might be due to invalid IL or missing references)
		//IL_0279: Unknown result type (might be due to invalid IL or missing references)
		//IL_028e: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ca: Unknown result type (might be due to invalid IL or missing references)
		EntProtoId effect = (EntProtoId)(state switch
		{
			RMCHandsEmoteState.Fistbump => ent.Comp.FistBumpEffect, 
			RMCHandsEmoteState.Highfive => ent.Comp.HighFiveEffect, 
			RMCHandsEmoteState.Tailswipe => ent.Comp.TailSwipeEffect, 
			RMCHandsEmoteState.Hug => ent.Comp.HugEffect, 
			_ => throw new ArgumentOutOfRangeException(), 
		});
		string popup = state switch
		{
			RMCHandsEmoteState.Fistbump => base.Loc.GetString("rmc-hands-emotes-fistbump-attempt", (ValueTuple<string, object>)("ent", ent), (ValueTuple<string, object>)("target", target)), 
			RMCHandsEmoteState.Highfive => base.Loc.GetString("rmc-hands-emotes-highfive-attempt", (ValueTuple<string, object>)("ent", ent), (ValueTuple<string, object>)("target", target)), 
			RMCHandsEmoteState.Tailswipe => base.Loc.GetString("rmc-hands-emotes-tailswipe-attempt", (ValueTuple<string, object>)("ent", ent), (ValueTuple<string, object>)("target", target)), 
			RMCHandsEmoteState.Hug => base.Loc.GetString("rmc-hands-emotes-hug-attempt", (ValueTuple<string, object>)("ent", ent), (ValueTuple<string, object>)("target", target)), 
			_ => throw new ArgumentOutOfRangeException(), 
		};
		string popupSelf = state switch
		{
			RMCHandsEmoteState.Fistbump => base.Loc.GetString("rmc-hands-emotes-fistbump-attempt-self", (ValueTuple<string, object>)("ent", ent), (ValueTuple<string, object>)("target", target)), 
			RMCHandsEmoteState.Highfive => base.Loc.GetString("rmc-hands-emotes-highfive-attempt-self", (ValueTuple<string, object>)("ent", ent), (ValueTuple<string, object>)("target", target)), 
			RMCHandsEmoteState.Tailswipe => base.Loc.GetString("rmc-hands-emotes-tailswipe-attempt-self", (ValueTuple<string, object>)("ent", ent), (ValueTuple<string, object>)("target", target)), 
			RMCHandsEmoteState.Hug => base.Loc.GetString("rmc-hands-emotes-hug-attempt-self", (ValueTuple<string, object>)("ent", ent), (ValueTuple<string, object>)("target", target)), 
			_ => throw new ArgumentOutOfRangeException(), 
		};
		ent.Comp.Active = true;
		ent.Comp.Target = target.Owner;
		ent.Comp.LeaveHangingAt = _timing.CurTime + ent.Comp.LeftHangingDelay;
		ent.Comp.State = state;
		if (_net.IsServer)
		{
			ent.Comp.SpawnedEffect = ((EntitySystem)this).SpawnAttachedTo(EntProtoId.op_Implicit(effect), ent.Owner.ToCoordinates(), (ComponentRegistry)null, default(Angle));
		}
		_popup.PopupPredicted(popupSelf, popup, ent.Owner, ent.Owner, PopupType.Medium);
		((EntitySystem)this).Dirty<RMCHandEmotesComponent>(ent, (MetaDataComponent)null);
	}

	public void CancelHandEmotes(Entity<RMCHandEmotesComponent> ent)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		ent.Comp.Target = null;
		ent.Comp.Active = false;
		if (_net.IsServer && ent.Comp.SpawnedEffect.HasValue)
		{
			((EntitySystem)this).QueueDel(ent.Comp.SpawnedEffect);
		}
		ent.Comp.SpawnedEffect = null;
		((EntitySystem)this).Dirty<RMCHandEmotesComponent>(ent, (MetaDataComponent)null);
	}

	public void PerformEmote(Entity<RMCHandEmotesComponent> ent, Entity<RMCHandEmotesComponent> target)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01df: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_0247: Unknown result type (might be due to invalid IL or missing references)
		//IL_0257: Unknown result type (might be due to invalid IL or missing references)
		//IL_0213: Unknown result type (might be due to invalid IL or missing references)
		//IL_0223: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_033f: Unknown result type (might be due to invalid IL or missing references)
		//IL_034f: Unknown result type (might be due to invalid IL or missing references)
		//IL_030b: Unknown result type (might be due to invalid IL or missing references)
		//IL_031b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0375: Unknown result type (might be due to invalid IL or missing references)
		//IL_0376: Unknown result type (might be due to invalid IL or missing references)
		//IL_038a: Unknown result type (might be due to invalid IL or missing references)
		//IL_038b: Unknown result type (might be due to invalid IL or missing references)
		//IL_039d: Unknown result type (might be due to invalid IL or missing references)
		//IL_039e: Unknown result type (might be due to invalid IL or missing references)
		//IL_03aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_043d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0444: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_040d: Unknown result type (might be due to invalid IL or missing references)
		//IL_041e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0435: Unknown result type (might be due to invalid IL or missing references)
		//IL_0436: Unknown result type (might be due to invalid IL or missing references)
		if (_timing.IsFirstTimePredicted)
		{
			EntityUid uid = ent.Owner;
			EntityUid targetUid = target.Owner;
			RMCHandsEmoteState state = ent.Comp.State;
			SoundSpecifier sound = (SoundSpecifier)(state switch
			{
				RMCHandsEmoteState.Fistbump => ent.Comp.FistBumpSound, 
				RMCHandsEmoteState.Highfive => ent.Comp.HighFiveSound, 
				RMCHandsEmoteState.Hug => ent.Comp.HugSound, 
				RMCHandsEmoteState.Tailswipe => ent.Comp.TailSwipeSound, 
				_ => throw new ArgumentOutOfRangeException(), 
			});
			string popup = state switch
			{
				RMCHandsEmoteState.Fistbump => base.Loc.GetString("rmc-hands-emotes-fistbump", (ValueTuple<string, object>)("ent", uid), (ValueTuple<string, object>)("target", targetUid)), 
				RMCHandsEmoteState.Highfive => base.Loc.GetString("rmc-hands-emotes-highfive", (ValueTuple<string, object>)("ent", uid), (ValueTuple<string, object>)("target", targetUid)), 
				RMCHandsEmoteState.Hug => base.Loc.GetString("rmc-hands-emotes-hug", (ValueTuple<string, object>)("ent", uid), (ValueTuple<string, object>)("target", targetUid)), 
				RMCHandsEmoteState.Tailswipe => base.Loc.GetString("rmc-hands-emotes-tailswipe", (ValueTuple<string, object>)("ent", uid), (ValueTuple<string, object>)("target", targetUid)), 
				_ => throw new ArgumentOutOfRangeException(), 
			};
			string popupSelf = state switch
			{
				RMCHandsEmoteState.Fistbump => base.Loc.GetString("rmc-hands-emotes-fistbump-self", (ValueTuple<string, object>)("ent", uid), (ValueTuple<string, object>)("target", targetUid)), 
				RMCHandsEmoteState.Highfive => base.Loc.GetString("rmc-hands-emotes-highfive-self", (ValueTuple<string, object>)("ent", uid), (ValueTuple<string, object>)("target", targetUid)), 
				RMCHandsEmoteState.Hug => base.Loc.GetString("rmc-hands-emotes-hug-self", (ValueTuple<string, object>)("ent", uid), (ValueTuple<string, object>)("target", targetUid)), 
				RMCHandsEmoteState.Tailswipe => base.Loc.GetString("rmc-hands-emotes-tailswipe-self", (ValueTuple<string, object>)("ent", uid), (ValueTuple<string, object>)("target", targetUid)), 
				_ => throw new ArgumentOutOfRangeException(), 
			};
			string popupSelfTarget = state switch
			{
				RMCHandsEmoteState.Fistbump => base.Loc.GetString("rmc-hands-emotes-fistbump-self", (ValueTuple<string, object>)("ent", targetUid), (ValueTuple<string, object>)("target", uid)), 
				RMCHandsEmoteState.Highfive => base.Loc.GetString("rmc-hands-emotes-highfive-self", (ValueTuple<string, object>)("ent", targetUid), (ValueTuple<string, object>)("target", uid)), 
				RMCHandsEmoteState.Hug => base.Loc.GetString("rmc-hands-emotes-hug-self", (ValueTuple<string, object>)("ent", targetUid), (ValueTuple<string, object>)("target", uid)), 
				RMCHandsEmoteState.Tailswipe => base.Loc.GetString("rmc-hands-emotes-tailswipe-self", (ValueTuple<string, object>)("ent", targetUid), (ValueTuple<string, object>)("target", uid)), 
				_ => throw new ArgumentOutOfRangeException(), 
			};
			_popup.PopupClient(popupSelf, uid, uid, PopupType.Medium);
			_popup.PopupClient(popupSelfTarget, targetUid, targetUid, PopupType.Medium);
			_melee.DoLunge(targetUid, uid);
			_rotate.TryFaceCoordinates(uid, _transform.GetMapCoordinates(targetUid, (TransformComponent)null).Position);
			_rotate.TryFaceCoordinates(targetUid, _transform.GetMapCoordinates(uid, (TransformComponent)null).Position);
			if (_net.IsServer)
			{
				Filter others = Filter.PvsExcept(uid, 2f, (IEntityManager)null).RemovePlayerByAttachedEntity(targetUid);
				_popup.PopupEntity(popup, uid, others, recordReplay: true);
				_audio.PlayPvs(sound, uid, (AudioParams?)null);
				_melee.DoLunge(uid, targetUid);
			}
			CancelHandEmotes(ent);
			CancelHandEmotes(target);
		}
	}

	public override void Update(float frameTime)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		if (_net.IsClient)
		{
			return;
		}
		TimeSpan time = _timing.CurTime;
		EntityQueryEnumerator<RMCHandEmotesComponent, TransformComponent> handQuery = ((EntitySystem)this).EntityQueryEnumerator<RMCHandEmotesComponent, TransformComponent>();
		EntityUid uid = default(EntityUid);
		RMCHandEmotesComponent comp = default(RMCHandEmotesComponent);
		TransformComponent xform = default(TransformComponent);
		while (handQuery.MoveNext(ref uid, ref comp, ref xform))
		{
			if (comp.Active && !(time < comp.LeaveHangingAt))
			{
				CancelHandEmotes(Entity<RMCHandEmotesComponent>.op_Implicit((uid, comp)));
				string leaveHangingMessage = base.Loc.GetString("rmc-hands-emotes-left-hanging");
				_popup.PopupEntity(leaveHangingMessage, uid, uid, PopupType.SmallCaution);
			}
		}
	}
}
