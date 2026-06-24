using System;
using System.Collections.Generic;
using Content.Shared._RMC14.Marines.Skills;
using Content.Shared.Coordinates;
using Content.Shared.Examine;
using Content.Shared.Interaction;
using Content.Shared.Popups;
using Content.Shared.Stacks;
using Content.Shared.Storage;
using Content.Shared.Tools;
using Content.Shared.Tools.Components;
using Content.Shared.Tools.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;

namespace Content.Shared._RMC14.Tools;

public sealed class RMCToolSystem : EntitySystem
{
	[Dependency]
	private INetManager _net;

	[Dependency]
	private SharedPopupSystem _popup;

	[Dependency]
	private IPrototypeManager _prototypes;

	[Dependency]
	private SharedStackSystem _stack;

	[Dependency]
	private SharedToolSystem _tool;

	[Dependency]
	private SkillsSystem _skills;

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<RMCRefinableComponent, ExaminedEvent>((EntityEventRefHandler<RMCRefinableComponent, ExaminedEvent>)OnRefinableExamined, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCRefinableComponent, InteractUsingEvent>((EntityEventRefHandler<RMCRefinableComponent, InteractUsingEvent>)OnRefinableInteractUsing, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCRefinableComponent, RMCRefinableDoAfterEvent>((EntityEventRefHandler<RMCRefinableComponent, RMCRefinableDoAfterEvent>)OnRefinableDoAfter, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ToolComponent, RMCToolUseEvent>((EntityEventRefHandler<ToolComponent, RMCToolUseEvent>)OnToolUse, (Type[])null, (Type[])null);
	}

	private void OnRefinableExamined(Entity<RMCRefinableComponent> ent, ref ExaminedEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		ToolQualityPrototype tool = default(ToolQualityPrototype);
		if (!_prototypes.TryIndex<ToolQualityPrototype>(ent.Comp.Tool, ref tool))
		{
			return;
		}
		string quality = base.Loc.GetString(tool.ToolName);
		using (args.PushGroup("RMCRefinableComponent"))
		{
			args.PushMarkup(base.Loc.GetString("rmc-refinable-can-be-refined", (ValueTuple<string, object>)("tool", quality)));
		}
	}

	private void OnRefinableInteractUsing(Entity<RMCRefinableComponent> ent, ref InteractUsingEvent args)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		if (!((HandledEntityEventArgs)args).Handled && ((EntitySystem)this).HasComp<ToolComponent>(args.Used))
		{
			((HandledEntityEventArgs)args).Handled = true;
			if (ent.Comp.Amount > _stack.GetCount(Entity<RMCRefinableComponent>.op_Implicit(ent)))
			{
				_popup.PopupClient(base.Loc.GetString("rmc-refinable-not-enough", (ValueTuple<string, object>)("amount", ent.Comp.Amount), (ValueTuple<string, object>)("name", ((EntitySystem)this).Name(Entity<RMCRefinableComponent>.op_Implicit(ent), (MetaDataComponent)null))), Entity<RMCRefinableComponent>.op_Implicit(ent), args.User);
				return;
			}
			RMCRefinableDoAfterEvent ev = new RMCRefinableDoAfterEvent();
			float delay = (float)ent.Comp.Delay.TotalSeconds;
			_tool.UseTool(args.Used, args.User, Entity<RMCRefinableComponent>.op_Implicit(ent), delay, ProtoId<ToolQualityPrototype>.op_Implicit(ent.Comp.Tool), ev, ent.Comp.Fuel);
		}
	}

	private void OnRefinableDoAfter(Entity<RMCRefinableComponent> ent, ref RMCRefinableDoAfterEvent args)
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		if (args.Cancelled || ((HandledEntityEventArgs)args).Handled)
		{
			return;
		}
		((HandledEntityEventArgs)args).Handled = true;
		if (base.EntityManager.IsQueuedForDeletion(Entity<RMCRefinableComponent>.op_Implicit(ent)))
		{
			return;
		}
		if (((EntitySystem)this).HasComp<StackComponent>(Entity<RMCRefinableComponent>.op_Implicit(ent)))
		{
			if (!_stack.Use(Entity<RMCRefinableComponent>.op_Implicit(ent), ent.Comp.Amount))
			{
				_popup.PopupClient(base.Loc.GetString("rmc-refinable-not-enough", (ValueTuple<string, object>)("amount", ent.Comp.Amount), (ValueTuple<string, object>)("name", ((EntitySystem)this).Name(Entity<RMCRefinableComponent>.op_Implicit(ent), (MetaDataComponent)null))), Entity<RMCRefinableComponent>.op_Implicit(ent), args.User);
				return;
			}
		}
		else
		{
			if (_net.IsClient)
			{
				return;
			}
			((EntitySystem)this).QueueDel((EntityUid?)Entity<RMCRefinableComponent>.op_Implicit(ent));
		}
		if (_net.IsClient)
		{
			return;
		}
		foreach (string spawn in EntitySpawnCollection.GetSpawns((IEnumerable<EntitySpawnEntry>)ent.Comp.Spawn, (IRobustRandom?)null))
		{
			((EntitySystem)this).SpawnAtPosition(spawn, ent.Owner.ToCoordinates(), (ComponentRegistry)null);
		}
	}

	private void OnToolUse(Entity<ToolComponent> ent, ref RMCToolUseEvent args)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		SkillsComponent skills = default(SkillsComponent);
		if (((EntitySystem)this).TryComp<SkillsComponent>(args.User, ref skills) && !args.Handled)
		{
			args.Delay *= (double)_skills.GetSkillDelayMultiplier(Entity<SkillsComponent>.op_Implicit(args.User), ent.Comp.Skill);
			args.Handled = true;
		}
	}
}
