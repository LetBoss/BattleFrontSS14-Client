using System;
using System.Collections.Generic;
using System.Linq;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;

namespace Content.Shared.BarSign;

public sealed class BarSignSystem : EntitySystem
{
	[Dependency]
	private IPrototypeManager _prototypeManager;

	[Dependency]
	private IRobustRandom _random;

	[Dependency]
	private MetaDataSystem _metaData;

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<BarSignComponent, MapInitEvent>((EntityEventRefHandler<BarSignComponent, MapInitEvent>)OnMapInit, (Type[])null, (Type[])null);
		BoundUserInterfaceRegisterExt.BuiEvents<BarSignComponent>(((EntitySystem)this).Subs, (object)BarSignUiKey.Key, (BuiEventSubscriber<BarSignComponent>)delegate(Subscriber<BarSignComponent> subs)
		{
			subs.Event<SetBarSignMessage>((EntityEventRefHandler<BarSignComponent, SetBarSignMessage>)OnSetBarSignMessage);
		});
	}

	private void OnMapInit(Entity<BarSignComponent> ent, ref MapInitEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		if (!ent.Comp.Current.HasValue)
		{
			BarSignPrototype newPrototype = RandomExtensions.Pick<BarSignPrototype>(_random, (IReadOnlyList<BarSignPrototype>)GetAllBarSigns(_prototypeManager));
			SetBarSign(ent, newPrototype);
		}
	}

	private void OnSetBarSignMessage(Entity<BarSignComponent> ent, ref SetBarSignMessage args)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		BarSignPrototype signPrototype = default(BarSignPrototype);
		if (_prototypeManager.TryIndex<BarSignPrototype>(args.Sign, ref signPrototype))
		{
			SetBarSign(ent, signPrototype);
		}
	}

	public void SetBarSign(Entity<BarSignComponent> ent, BarSignPrototype newPrototype)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		MetaDataComponent meta = ((EntitySystem)this).MetaData(Entity<BarSignComponent>.op_Implicit(ent));
		string name = base.Loc.GetString(LocId.op_Implicit(newPrototype.Name));
		_metaData.SetEntityName(Entity<BarSignComponent>.op_Implicit(ent), name, meta, true);
		_metaData.SetEntityDescription(Entity<BarSignComponent>.op_Implicit(ent), base.Loc.GetString(LocId.op_Implicit(newPrototype.Description)), meta);
		ent.Comp.Current = ProtoId<BarSignPrototype>.op_Implicit(newPrototype.ID);
		((EntitySystem)this).Dirty<BarSignComponent>(ent, (MetaDataComponent)null);
	}

	public static List<BarSignPrototype> GetAllBarSigns(IPrototypeManager prototypeManager)
	{
		return (from p in prototypeManager.EnumeratePrototypes<BarSignPrototype>()
			where !p.Hidden
			select p).ToList();
	}
}
