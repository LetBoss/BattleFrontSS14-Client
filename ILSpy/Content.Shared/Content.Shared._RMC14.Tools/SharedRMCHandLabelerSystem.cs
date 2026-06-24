using System;
using Content.Shared.Interaction;
using Content.Shared.Labels.Components;
using Content.Shared.Labels.EntitySystems;
using Content.Shared.Tag;
using Content.Shared.Whitelist;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;

namespace Content.Shared._RMC14.Tools;

public abstract class SharedRMCHandLabelerSystem : EntitySystem
{
	[Dependency]
	private EntityWhitelistSystem _whitelist;

	[Dependency]
	private INetManager _net;

	[Dependency]
	private LabelSystem _labelSystem;

	[Dependency]
	private SharedAudioSystem _audio;

	[Dependency]
	private TagSystem _tag;

	private const string PillCanisterTag = "PillCanister";

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<RMCHandLabelerComponent, BeforeRangedInteractEvent>((EntityEventRefHandler<RMCHandLabelerComponent, BeforeRangedInteractEvent>)OnBeforeRangedInteract, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCHandLabelerComponent, AfterInteractEvent>((EntityEventRefHandler<RMCHandLabelerComponent, AfterInteractEvent>)OnAfterInteract, new Type[1] { typeof(SharedHandLabelerSystem) }, (Type[])null);
	}

	private void OnBeforeRangedInteract(Entity<RMCHandLabelerComponent> ent, ref BeforeRangedInteractEvent args)
	{
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		if (((HandledEntityEventArgs)args).Handled || !args.CanReach || !args.Target.HasValue)
		{
			return;
		}
		EntityUid target = args.Target.Value;
		HandLabelerComponent labeler = default(HandLabelerComponent);
		if (!_tag.HasTag(target, ProtoId<TagPrototype>.op_Implicit("PillCanister")) || !((EntitySystem)this).TryComp<HandLabelerComponent>(Entity<RMCHandLabelerComponent>.op_Implicit(ent), ref labeler))
		{
			return;
		}
		OnPillBottleInteract(ent, target, args.User);
		string labelText = labeler.AssignedLabel;
		LabelComponent labelComp = default(LabelComponent);
		if (!string.IsNullOrEmpty(labelText))
		{
			if (_whitelist.IsWhitelistFail(labeler.Whitelist, target))
			{
				((HandledEntityEventArgs)args).Handled = true;
				return;
			}
			ApplyLabel(target, labelText);
			_audio.PlayPredicted(ent.Comp.LabelSound, Entity<RMCHandLabelerComponent>.op_Implicit(ent), (EntityUid?)args.User, (AudioParams?)null);
		}
		else if (((EntitySystem)this).TryComp<LabelComponent>(target, ref labelComp) && !string.IsNullOrEmpty(labelComp.CurrentLabel))
		{
			ApplyLabel(target, null);
			_audio.PlayPredicted(ent.Comp.RemoveLabelSound, Entity<RMCHandLabelerComponent>.op_Implicit(ent), (EntityUid?)args.User, (AudioParams?)null);
		}
		((HandledEntityEventArgs)args).Handled = true;
	}

	private void OnAfterInteract(Entity<RMCHandLabelerComponent> ent, ref AfterInteractEvent args)
	{
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		if (((HandledEntityEventArgs)args).Handled || !args.CanReach || !args.Target.HasValue)
		{
			return;
		}
		EntityUid target = args.Target.Value;
		HandLabelerComponent labeler = default(HandLabelerComponent);
		if (!((EntitySystem)this).TryComp<HandLabelerComponent>(Entity<RMCHandLabelerComponent>.op_Implicit(ent), ref labeler))
		{
			return;
		}
		if (string.IsNullOrEmpty(labeler.AssignedLabel))
		{
			LabelComponent labelComp = default(LabelComponent);
			if (((EntitySystem)this).TryComp<LabelComponent>(target, ref labelComp) && !string.IsNullOrEmpty(labelComp.CurrentLabel))
			{
				_audio.PlayPredicted(ent.Comp.RemoveLabelSound, Entity<RMCHandLabelerComponent>.op_Implicit(ent), (EntityUid?)args.User, (AudioParams?)null);
			}
		}
		else if (!_whitelist.IsWhitelistFail(labeler.Whitelist, target))
		{
			_audio.PlayPredicted(ent.Comp.LabelSound, Entity<RMCHandLabelerComponent>.op_Implicit(ent), (EntityUid?)args.User, (AudioParams?)null);
		}
	}

	protected virtual void OnPillBottleInteract(Entity<RMCHandLabelerComponent> labeler, EntityUid pillBottle, EntityUid user)
	{
	}

	private void ApplyLabel(EntityUid target, string? labelText)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		if (_net.IsServer)
		{
			_labelSystem.Label(target, labelText);
		}
	}
}
