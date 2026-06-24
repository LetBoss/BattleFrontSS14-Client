using System;
using Content.Shared.DoAfter;
using Content.Shared.Engineering.Components;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Verbs;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;

namespace Content.Shared.Engineering.Systems;

public sealed class DisassembleOnAltVerbSystem : EntitySystem
{
	[Dependency]
	private SharedDoAfterSystem _doAfter;

	[Dependency]
	private SharedHandsSystem _handsSystem;

	[Dependency]
	private INetManager _net;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<DisassembleOnAltVerbComponent, GetVerbsEvent<AlternativeVerb>>((EntityEventRefHandler<DisassembleOnAltVerbComponent, GetVerbsEvent<AlternativeVerb>>)AddDisassembleVerb, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<DisassembleOnAltVerbComponent, DisassembleDoAfterEvent>((EntityEventRefHandler<DisassembleOnAltVerbComponent, DisassembleDoAfterEvent>)OnDisassembleDoAfter, (Type[])null, (Type[])null);
	}

	private void AddDisassembleVerb(Entity<DisassembleOnAltVerbComponent> entity, ref GetVerbsEvent<AlternativeVerb> args)
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		if (args.CanInteract && args.CanAccess && args.Hands != null)
		{
			DoAfterArgs doAfterArgs = new DoAfterArgs((IEntityManager)(object)base.EntityManager, args.User, entity.Comp.DisassembleTime, new DisassembleDoAfterEvent(), Entity<DisassembleOnAltVerbComponent>.op_Implicit(entity), Entity<DisassembleOnAltVerbComponent>.op_Implicit(entity))
			{
				BreakOnMove = true
			};
			AlternativeVerb verb = new AlternativeVerb
			{
				Act = delegate
				{
					_doAfter.TryStartDoAfter(doAfterArgs);
				},
				Text = base.Loc.GetString("disassemble-system-verb-disassemble"),
				Priority = 2
			};
			args.Verbs.Add(verb);
		}
	}

	private void OnDisassembleDoAfter(Entity<DisassembleOnAltVerbComponent> entity, ref DisassembleDoAfterEvent args)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		if (_net.IsServer && !args.Cancelled)
		{
			EntProtoId? prototypeToSpawn = entity.Comp.PrototypeToSpawn;
			EntityUid? spawnedEnt = default(EntityUid?);
			if (((EntitySystem)this).TrySpawnNextTo(prototypeToSpawn.HasValue ? EntProtoId.op_Implicit(prototypeToSpawn.GetValueOrDefault()) : null, entity.Owner, ref spawnedEnt, (TransformComponent)null, (ComponentRegistry)null))
			{
				_handsSystem.TryPickup(args.User, spawnedEnt.Value);
			}
			((EntitySystem)this).QueueDel((EntityUid?)entity.Owner);
		}
	}
}
