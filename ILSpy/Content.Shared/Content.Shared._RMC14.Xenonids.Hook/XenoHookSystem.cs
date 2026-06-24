using System;
using System.Collections.Generic;
using Content.Shared._RMC14.Line;
using Content.Shared.Throwing;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;

namespace Content.Shared._RMC14.Xenonids.Hook;

public sealed class XenoHookSystem : EntitySystem
{
	[Dependency]
	private INetManager _net;

	[Dependency]
	private SharedAppearanceSystem _appearance;

	[Dependency]
	private LineSystem _line;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<XenoHookComponent, MoveEvent>((EntityEventRefHandler<XenoHookComponent, MoveEvent>)OnHookSourceMove, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoHookComponent, EntityTerminatingEvent>((EntityEventRefHandler<XenoHookComponent, EntityTerminatingEvent>)OnHookDelete, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoHookedComponent, MoveEvent>((EntityEventRefHandler<XenoHookedComponent, MoveEvent>)OnHookedMove, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoHookedComponent, StopThrowEvent>((EntityEventRefHandler<XenoHookedComponent, StopThrowEvent>)OnHookedStop, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoHookedComponent, ComponentShutdown>((EntityEventRefHandler<XenoHookedComponent, ComponentShutdown>)OnHookedRemoved, (Type[])null, (Type[])null);
	}

	private void OnHookSourceMove(Entity<XenoHookComponent> xeno, ref MoveEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		if (xeno.Comp.Hooked.Count == 0)
		{
			return;
		}
		List<EntityUid> toRemove = new List<EntityUid>();
		XenoHookedComponent hookComp = default(XenoHookedComponent);
		foreach (EntityUid hooked in xeno.Comp.Hooked)
		{
			if (!((EntitySystem)this).TryComp<XenoHookedComponent>(hooked, ref hookComp))
			{
				toRemove.Add(hooked);
			}
			else
			{
				UpdateTail(Entity<XenoHookedComponent>.op_Implicit((hooked, hookComp)));
			}
		}
		foreach (EntityUid ent in toRemove)
		{
			xeno.Comp.Hooked.Remove(ent);
		}
	}

	private void OnHookDelete(Entity<XenoHookComponent> xeno, ref EntityTerminatingEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		if (xeno.Comp.Hooked.Count == 0)
		{
			return;
		}
		foreach (EntityUid hooked in xeno.Comp.Hooked)
		{
			((EntitySystem)this).RemCompDeferred<XenoHookedComponent>(hooked);
		}
		xeno.Comp.Hooked.Clear();
	}

	private void OnHookedMove(Entity<XenoHookedComponent> ent, ref MoveEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		UpdateTail(ent);
	}

	private void OnHookedStop(Entity<XenoHookedComponent> ent, ref StopThrowEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).RemCompDeferred<XenoHookedComponent>(Entity<XenoHookedComponent>.op_Implicit(ent));
	}

	private void OnHookedRemoved(Entity<XenoHookedComponent> ent, ref ComponentShutdown args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		XenoHookComponent hookSource = default(XenoHookComponent);
		if (((EntitySystem)this).TryComp<XenoHookComponent>(ent.Comp.Source, ref hookSource))
		{
			hookSource.Hooked.Remove(Entity<XenoHookedComponent>.op_Implicit(ent));
		}
		ent.Comp.StopUpdating = true;
		((EntitySystem)this).Dirty<XenoHookedComponent>(ent, (MetaDataComponent)null);
		_line.DeleteBeam(ent.Comp.Tail);
		_appearance.SetData(Entity<XenoHookedComponent>.op_Implicit(ent), (Enum)HookedVisuals.Hooked, (object)false, (AppearanceComponent)null);
	}

	public bool TryHookTarget(Entity<XenoHookComponent> xeno, EntityUid target)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).HasComp<XenoHookedComponent>(target))
		{
			return false;
		}
		XenoHookedComponent hook = ((EntitySystem)this).EnsureComp<XenoHookedComponent>(target);
		hook.Source = Entity<XenoHookComponent>.op_Implicit(xeno);
		hook.TailProto = xeno.Comp.TailProto;
		xeno.Comp.Hooked.Add(target);
		((EntitySystem)this).Dirty<XenoHookComponent>(xeno, (MetaDataComponent)null);
		_appearance.SetData(target, (Enum)HookedVisuals.Hooked, (object)true, (AppearanceComponent)null);
		UpdateTail(Entity<XenoHookedComponent>.op_Implicit((target, hook)));
		return true;
	}

	public void UpdateTail(Entity<XenoHookedComponent> ent)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		if (_net.IsClient)
		{
			return;
		}
		XenoHookedComponent hook = ent.Comp;
		if (!hook.StopUpdating)
		{
			if (hook.Tail.Count != 0)
			{
				_line.DeleteBeam(hook.Tail);
			}
			if (_line.TryCreateLine(hook.Source, Entity<XenoHookedComponent>.op_Implicit(ent), EntProtoId.op_Implicit(hook.TailProto), out List<EntityUid> lines))
			{
				hook.Tail = lines;
			}
		}
	}
}
