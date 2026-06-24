using System;
using Content.Shared.Examine;
using Content.Shared.GPS.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;

namespace Content.Shared.GPS.Systems;

public sealed class HandheldGpsSystem : EntitySystem
{
	[Dependency]
	private SharedTransformSystem _transform;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<HandheldGPSComponent, ExaminedEvent>((EntityEventRefHandler<HandheldGPSComponent, ExaminedEvent>)OnExamine, (Type[])null, (Type[])null);
	}

	private void OnExamine(Entity<HandheldGPSComponent> ent, ref ExaminedEvent args)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		string posText = "Error";
		MapCoordinates pos = _transform.GetMapCoordinates(Entity<HandheldGPSComponent>.op_Implicit(ent), (TransformComponent)null);
		if (pos.MapId != MapId.Nullspace)
		{
			int x = (int)pos.Position.X;
			int y = (int)pos.Position.Y;
			posText = $"({x}, {y})";
		}
		args.PushMarkup(base.Loc.GetString("handheld-gps-coordinates-title", (ValueTuple<string, object>)("coordinates", posText)));
	}
}
