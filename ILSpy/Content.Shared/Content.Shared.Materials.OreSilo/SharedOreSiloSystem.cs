using System;
using System.Collections.Generic;
using Content.Shared.Power.Components;
using Content.Shared.Power.EntitySystems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;

namespace Content.Shared.Materials.OreSilo;

public abstract class SharedOreSiloSystem : EntitySystem
{
	[Dependency]
	private SharedMaterialStorageSystem _materialStorage;

	[Dependency]
	private SharedPowerReceiverSystem _powerReceiver;

	[Dependency]
	private SharedTransformSystem _transform;

	private EntityQuery<OreSiloClientComponent> _clientQuery;

	public override void Initialize()
	{
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).SubscribeLocalEvent<OreSiloComponent, ToggleOreSiloClientMessage>((EntityEventRefHandler<OreSiloComponent, ToggleOreSiloClientMessage>)OnToggleOreSiloClient, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<OreSiloComponent, ComponentShutdown>((EntityEventRefHandler<OreSiloComponent, ComponentShutdown>)OnSiloShutdown, (Type[])null, (Type[])null);
		BoundUserInterfaceRegisterExt.BuiEvents<OreSiloComponent>(((EntitySystem)this).Subs, (object)OreSiloUiKey.Key, (BuiEventSubscriber<OreSiloComponent>)delegate(Subscriber<OreSiloComponent> subs)
		{
			subs.Event<BoundUIOpenedEvent>((EntityEventRefHandler<OreSiloComponent, BoundUIOpenedEvent>)OnBoundUIOpened);
		});
		((EntitySystem)this).SubscribeLocalEvent<OreSiloClientComponent, GetStoredMaterialsEvent>((EntityEventRefHandler<OreSiloClientComponent, GetStoredMaterialsEvent>)OnGetStoredMaterials, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<OreSiloClientComponent, ConsumeStoredMaterialsEvent>((EntityEventRefHandler<OreSiloClientComponent, ConsumeStoredMaterialsEvent>)OnConsumeStoredMaterials, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<OreSiloClientComponent, ComponentShutdown>((EntityEventRefHandler<OreSiloClientComponent, ComponentShutdown>)OnClientShutdown, (Type[])null, (Type[])null);
		_clientQuery = ((EntitySystem)this).GetEntityQuery<OreSiloClientComponent>();
	}

	private void OnToggleOreSiloClient(Entity<OreSiloComponent> ent, ref ToggleOreSiloClientMessage args)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		EntityUid client = ((EntitySystem)this).GetEntity(args.Client);
		OreSiloClientComponent clientComp = default(OreSiloClientComponent);
		if (!_clientQuery.TryComp(client, ref clientComp))
		{
			return;
		}
		if (ent.Comp.Clients.Contains(client))
		{
			clientComp.Silo = null;
			((EntitySystem)this).Dirty(client, (IComponent)(object)clientComp, (MetaDataComponent)null);
			ent.Comp.Clients.Remove(client);
			((EntitySystem)this).Dirty<OreSiloComponent>(ent, (MetaDataComponent)null);
			UpdateOreSiloUi(ent);
		}
		else
		{
			if (!CanTransmitMaterials(Entity<OreSiloComponent, TransformComponent>.op_Implicit((Entity<OreSiloComponent>.op_Implicit(ent), Entity<OreSiloComponent>.op_Implicit(ent))), client))
			{
				return;
			}
			Dictionary<ProtoId<MaterialPrototype>, int> clientMats = _materialStorage.GetStoredMaterials(Entity<MaterialStorageComponent>.op_Implicit(client), localOnly: true);
			Dictionary<string, int> inverseMats = new Dictionary<string, int>();
			foreach (var (mat, amount) in clientMats)
			{
				inverseMats.Add(ProtoId<MaterialPrototype>.op_Implicit(mat), -amount);
			}
			_materialStorage.TryChangeMaterialAmount(Entity<MaterialStorageComponent>.op_Implicit(client), inverseMats, localOnly: true);
			_materialStorage.TryChangeMaterialAmount(Entity<MaterialStorageComponent>.op_Implicit(ent.Owner), clientMats);
			ent.Comp.Clients.Add(client);
			((EntitySystem)this).Dirty<OreSiloComponent>(ent, (MetaDataComponent)null);
			clientComp.Silo = Entity<OreSiloComponent>.op_Implicit(ent);
			((EntitySystem)this).Dirty(client, (IComponent)(object)clientComp, (MetaDataComponent)null);
			UpdateOreSiloUi(ent);
		}
	}

	private void OnBoundUIOpened(Entity<OreSiloComponent> ent, ref BoundUIOpenedEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		UpdateOreSiloUi(ent);
	}

	private void OnSiloShutdown(Entity<OreSiloComponent> ent, ref ComponentShutdown args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		OreSiloClientComponent comp = default(OreSiloClientComponent);
		foreach (EntityUid client in ent.Comp.Clients)
		{
			if (_clientQuery.TryComp(client, ref comp))
			{
				comp.Silo = null;
				((EntitySystem)this).Dirty(client, (IComponent)(object)comp, (MetaDataComponent)null);
			}
		}
	}

	protected virtual void UpdateOreSiloUi(Entity<OreSiloComponent> ent)
	{
	}

	private void OnGetStoredMaterials(Entity<OreSiloClientComponent> ent, ref GetStoredMaterialsEvent args)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		if (args.LocalOnly)
		{
			return;
		}
		EntityUid? silo = ent.Comp.Silo;
		if (!silo.HasValue)
		{
			return;
		}
		EntityUid silo2 = silo.GetValueOrDefault();
		if (!CanTransmitMaterials(Entity<OreSiloComponent, TransformComponent>.op_Implicit(silo2), Entity<OreSiloClientComponent>.op_Implicit(ent)))
		{
			return;
		}
		foreach (var (mat, amount) in _materialStorage.GetStoredMaterials(Entity<MaterialStorageComponent>.op_Implicit(silo2)))
		{
			if (_materialStorage.IsMaterialWhitelisted(Entity<MaterialStorageComponent>.op_Implicit((Entity<MaterialStorageComponent>.op_Implicit(args.Entity), Entity<MaterialStorageComponent>.op_Implicit(args.Entity))), mat))
			{
				int existing = Extensions.GetOrNew<ProtoId<MaterialPrototype>, int>(args.Materials, mat);
				args.Materials[mat] = existing + amount;
			}
		}
	}

	private void OnConsumeStoredMaterials(Entity<OreSiloClientComponent> ent, ref ConsumeStoredMaterialsEvent args)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		if (args.LocalOnly)
		{
			return;
		}
		EntityUid? silo = ent.Comp.Silo;
		if (!silo.HasValue)
		{
			return;
		}
		EntityUid silo2 = silo.GetValueOrDefault();
		MaterialStorageComponent materialStorage = default(MaterialStorageComponent);
		if (!((EntitySystem)this).TryComp<MaterialStorageComponent>(silo2, ref materialStorage) || !CanTransmitMaterials(Entity<OreSiloComponent, TransformComponent>.op_Implicit(silo2), Entity<OreSiloClientComponent>.op_Implicit(ent)))
		{
			return;
		}
		foreach (var (mat, amount) in args.Materials)
		{
			if (_materialStorage.TryChangeMaterialAmount(silo2, ProtoId<MaterialPrototype>.op_Implicit(mat), amount, materialStorage))
			{
				args.Materials[mat] = 0;
			}
		}
	}

	private void OnClientShutdown(Entity<OreSiloClientComponent> ent, ref ComponentShutdown args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		OreSiloComponent silo = default(OreSiloComponent);
		if (((EntitySystem)this).TryComp<OreSiloComponent>(ent.Comp.Silo, ref silo))
		{
			silo.Clients.Remove(Entity<OreSiloClientComponent>.op_Implicit(ent));
			((EntitySystem)this).Dirty(ent.Comp.Silo.Value, (IComponent)(object)silo, (MetaDataComponent)null);
			UpdateOreSiloUi(Entity<OreSiloComponent>.op_Implicit((ent.Comp.Silo.Value, silo)));
		}
	}

	public bool CanTransmitMaterials(Entity<OreSiloComponent?, TransformComponent?> silo, EntityUid client)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<OreSiloComponent, TransformComponent>(Entity<OreSiloComponent, TransformComponent>.op_Implicit(silo), ref silo.Comp1, ref silo.Comp2, true))
		{
			return false;
		}
		if (!_powerReceiver.IsPowered(Entity<SharedApcPowerReceiverComponent>.op_Implicit(silo.Owner)))
		{
			return false;
		}
		EntityUid? grid = _transform.GetGrid(Entity<TransformComponent>.op_Implicit(client));
		EntityUid? grid2 = _transform.GetGrid(Entity<TransformComponent>.op_Implicit(silo.Owner));
		if (grid.HasValue != grid2.HasValue || (grid.HasValue && grid.GetValueOrDefault() != grid2.GetValueOrDefault()))
		{
			return false;
		}
		if (!_transform.InRange(Entity<TransformComponent>.op_Implicit((silo.Owner, silo.Comp2)), Entity<TransformComponent>.op_Implicit(client), silo.Comp1.Range))
		{
			return false;
		}
		return true;
	}
}
