using System;
using System.Collections.Generic;
using Content.Shared._RMC14.Sound;
using Content.Shared.Popups;
using Content.Shared.Sound.Components;
using Content.Shared.Verbs;
using Robust.Shared.Audio;
using Robust.Shared.Collections;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;

namespace Content.Shared._RMC14.SelectableSounds;

public sealed class RMCSelectableSoundsSystem : EntitySystem
{
	[Dependency]
	private SharedPopupSystem _popup;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<RMCSelectableSoundsComponent, GetVerbsEvent<AlternativeVerb>>((EntityEventRefHandler<RMCSelectableSoundsComponent, GetVerbsEvent<AlternativeVerb>>)OnGetAltVerbs, (Type[])null, (Type[])null);
	}

	private void OnGetAltVerbs(Entity<RMCSelectableSoundsComponent> ent, ref GetVerbsEvent<AlternativeVerb> args)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		if (!args.CanAccess || !args.CanInteract || args.Hands == null)
		{
			return;
		}
		EntityUid user = args.User;
		ValueList<AlternativeVerb> verbs = default(ValueList<AlternativeVerb>);
		foreach (KeyValuePair<LocId, SoundSpecifier> soundEntry in ent.Comp.Sounds)
		{
			string name = base.Loc.GetString(LocId.op_Implicit(soundEntry.Key));
			SoundSpecifier sound = soundEntry.Value;
			AlternativeVerb newVerb = new AlternativeVerb
			{
				Text = name,
				IconEntity = ((EntitySystem)this).GetNetEntity(ent.Owner, (MetaDataComponent)null),
				Category = VerbCategory.SelectType,
				Act = delegate
				{
					//IL_0016: Unknown result type (might be due to invalid IL or missing references)
					//IL_0046: Unknown result type (might be due to invalid IL or missing references)
					//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
					//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
					EmitSoundOnUseComponent emitSoundOnUseComponent = default(EmitSoundOnUseComponent);
					if (((EntitySystem)this).TryComp<EmitSoundOnUseComponent>(ent.Owner, ref emitSoundOnUseComponent))
					{
						emitSoundOnUseComponent.Sound = sound;
					}
					EmitSoundOnActionComponent emitSoundOnActionComponent = default(EmitSoundOnActionComponent);
					if (((EntitySystem)this).TryComp<EmitSoundOnActionComponent>(ent.Owner, ref emitSoundOnActionComponent))
					{
						emitSoundOnActionComponent.Sound = sound;
					}
					string message = base.Loc.GetString("rmc-sound-select", (ValueTuple<string, object>)("sound", name));
					_popup.PopupClient(message, user, user);
				}
			};
			verbs.Add(newVerb);
		}
		args.Verbs.UnionWith((IEnumerable<AlternativeVerb>)(object)verbs);
	}
}
