using System;
using Content.Client.Examine;
using Robust.Shared.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Prototypes;

namespace Content.Client.Power.Generation.Teg;

public sealed class TegSystem : EntitySystem
{
	private static readonly EntProtoId ArrowPrototype = EntProtoId.op_Implicit("TegCirculatorArrow");

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<TegCirculatorComponent, ClientExaminedEvent>((ComponentEventHandler<TegCirculatorComponent, ClientExaminedEvent>)CirculatorExamined, (Type[])null, (Type[])null);
	}

	private void CirculatorExamined(EntityUid uid, TegCirculatorComponent component, ClientExaminedEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).Spawn(EntProtoId.op_Implicit(ArrowPrototype), new EntityCoordinates(uid, 0f, 0f));
	}
}
