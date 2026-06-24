using System;
using Content.Shared._RMC14.Medical.Unrevivable;
using Content.Shared._RMC14.Stun;
using Content.Shared.Body.Components;
using Content.Shared.Examine;
using Content.Shared.Mobs.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Utility;

namespace Content.Shared._RMC14.Medical.Examine;

public sealed class RMCMedicalExamineSystem : EntitySystem
{
	[Dependency]
	private MobStateSystem _mobState;

	[Dependency]
	private RMCSizeStunSystem _sizeStun;

	[Dependency]
	private RMCUnrevivableSystem _unrevivable;

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<RMCMedicalExamineComponent, ExaminedEvent>((EntityEventRefHandler<RMCMedicalExamineComponent, ExaminedEvent>)OnExamined, (Type[])null, (Type[])null);
	}

	private void OnExamined(Entity<RMCMedicalExamineComponent> ent, ref ExaminedEvent args)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		using (args.PushGroup("RMCMedicalExamineSystem", -1))
		{
			if (ent.Comp.Simple && _mobState.IsDead(ent.Owner))
			{
				args.PushMarkup(base.Loc.GetString(LocId.op_Implicit(ent.Comp.DeadText), (ValueTuple<string, object>)("victim", ent.Owner)));
			}
			else if (!((EntitySystem)this).HasComp<RMCBlockMedicalExamineComponent>(args.Examiner))
			{
				args.PushMessage(GetExamineText(ent));
			}
		}
	}

	public FormattedMessage GetExamineText(Entity<RMCMedicalExamineComponent> ent)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Expected O, but got Unknown
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		FormattedMessage msg = new FormattedMessage();
		BloodstreamComponent bloodstream = default(BloodstreamComponent);
		if (((EntitySystem)this).TryComp<BloodstreamComponent>(Entity<RMCMedicalExamineComponent>.op_Implicit(ent), ref bloodstream) && bloodstream.BleedAmount > 0f)
		{
			msg.AddMarkupOrThrow(base.Loc.GetString(LocId.op_Implicit(ent.Comp.BleedText), (ValueTuple<string, object>)("victim", ent.Owner)));
		}
		LocId? stateText = null;
		if (_mobState.IsDead(Entity<RMCMedicalExamineComponent>.op_Implicit(ent)))
		{
			stateText = (_unrevivable.IsUnrevivable(Entity<RMCMedicalExamineComponent>.op_Implicit(ent)) ? ent.Comp.UnrevivableText : ent.Comp.DeadText);
		}
		else if (_mobState.IsCritical(Entity<RMCMedicalExamineComponent>.op_Implicit(ent)) || _sizeStun.IsKnockedOut(Entity<RMCMedicalExamineComponent>.op_Implicit(ent)))
		{
			stateText = ent.Comp.CritText;
		}
		if (stateText.HasValue)
		{
			ILocalizationManager loc = base.Loc;
			LocId? val = stateText;
			msg.AddMarkupOrThrow(loc.GetString(val.HasValue ? LocId.op_Implicit(val.GetValueOrDefault()) : null, (ValueTuple<string, object>)("victim", ent.Owner)));
		}
		return msg;
	}
}
