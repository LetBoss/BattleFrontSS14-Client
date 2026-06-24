using System.Numerics;
using Content.Shared.Popups;
using Content.Shared.Xenoarchaeology.Artifact.XAE.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Random;
using Robust.Shared.Timing;

namespace Content.Shared.Xenoarchaeology.Artifact.XAE;

public sealed class XAERandomTeleportInvokerSystem : BaseXAESystem<XAERandomTeleportInvokerComponent>
{
	[Dependency]
	private IRobustRandom _random;

	[Dependency]
	private SharedPopupSystem _popup;

	[Dependency]
	private SharedTransformSystem _xform;

	[Dependency]
	private IGameTiming _timing;

	protected override void OnActivated(Entity<XAERandomTeleportInvokerComponent> ent, ref XenoArtifactNodeActivatedEvent args)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		if (_timing.IsFirstTimePredicted)
		{
			XAERandomTeleportInvokerComponent component = ent.Comp;
			TransformComponent xform = ((EntitySystem)this).Transform(ent.Owner);
			_popup.PopupCoordinates(((EntitySystem)this).Loc.GetString("blink-artifact-popup"), xform.Coordinates, PopupType.Medium);
			Vector2 offsetTo = _random.NextVector2(component.MinRange, component.MaxRange);
			SharedTransformSystem xform2 = _xform;
			EntityUid owner = ent.Owner;
			EntityCoordinates coordinates = xform.Coordinates;
			xform2.SetCoordinates(owner, xform, ((EntityCoordinates)(ref coordinates)).Offset(offsetTo), (Angle?)null, true, (TransformComponent)null, (TransformComponent)null);
		}
	}
}
