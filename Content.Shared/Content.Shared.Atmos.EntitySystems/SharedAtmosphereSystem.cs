using System;
using System.Collections.Generic;
using Content.Shared.Atmos.Components;
using Content.Shared.Atmos.Prototypes;
using Content.Shared.Body.Components;
using Content.Shared.Body.Systems;
using Content.Shared.Clothing;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;

namespace Content.Shared.Atmos.EntitySystems;

public abstract class SharedAtmosphereSystem : EntitySystem
{
	[Dependency]
	private IPrototypeManager _prototypeManager;

	[Dependency]
	private SharedInternalsSystem _internals;

	private EntityQuery<InternalsComponent> _internalsQuery;

	protected readonly GasPrototype[] GasPrototypes = new GasPrototype[9];

	public IEnumerable<GasPrototype> Gases => GasPrototypes;

	private void InitializeBreathTool()
	{
		((EntitySystem)this).SubscribeLocalEvent<BreathToolComponent, ComponentShutdown>((EntityEventRefHandler<BreathToolComponent, ComponentShutdown>)OnBreathToolShutdown, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<BreathToolComponent, ItemMaskToggledEvent>((EntityEventRefHandler<BreathToolComponent, ItemMaskToggledEvent>)OnMaskToggled, (Type[])null, (Type[])null);
	}

	private void OnBreathToolShutdown(Entity<BreathToolComponent> entity, ref ComponentShutdown args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		DisconnectInternals(entity);
	}

	public void DisconnectInternals(Entity<BreathToolComponent> entity, bool forced = false)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? old = entity.Comp.ConnectedInternalsEntity;
		if (old.HasValue)
		{
			entity.Comp.ConnectedInternalsEntity = null;
			InternalsComponent internalsComponent = default(InternalsComponent);
			if (_internalsQuery.TryComp(old, ref internalsComponent))
			{
				_internals.DisconnectBreathTool(Entity<InternalsComponent>.op_Implicit((old.Value, internalsComponent)), entity.Owner, forced);
			}
			((EntitySystem)this).Dirty<BreathToolComponent>(entity, (MetaDataComponent)null);
		}
	}

	private void OnMaskToggled(Entity<BreathToolComponent> ent, ref ItemMaskToggledEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		InternalsComponent internals = default(InternalsComponent);
		if (args.Mask.Comp.IsToggled)
		{
			DisconnectInternals(ent, forced: true);
		}
		else if (_internalsQuery.TryComp(args.Wearer, ref internals))
		{
			_internals.ConnectBreathTool(Entity<InternalsComponent>.op_Implicit((args.Wearer.Value, internals)), Entity<BreathToolComponent>.op_Implicit(ent));
		}
	}

	public override void Initialize()
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).Initialize();
		_internalsQuery = ((EntitySystem)this).GetEntityQuery<InternalsComponent>();
		InitializeBreathTool();
		for (int i = 0; i < 9; i++)
		{
			GasPrototypes[i] = _prototypeManager.Index<GasPrototype>(i.ToString());
		}
	}

	public GasPrototype GetGas(int gasId)
	{
		return GasPrototypes[gasId];
	}

	public GasPrototype GetGas(Gas gasId)
	{
		return GasPrototypes[(int)gasId];
	}
}
