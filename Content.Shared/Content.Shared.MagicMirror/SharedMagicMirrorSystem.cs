using System;
using System.Collections.Generic;
using Content.Shared.Humanoid;
using Content.Shared.Humanoid.Markings;
using Content.Shared.Humanoid.Prototypes;
using Content.Shared.Interaction;
using Content.Shared.UserInterface;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;

namespace Content.Shared.MagicMirror;

public abstract class SharedMagicMirrorSystem : EntitySystem
{
	[Dependency]
	private SharedInteractionSystem _interaction;

	[Dependency]
	protected SharedUserInterfaceSystem UISystem;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<MagicMirrorComponent, AfterInteractEvent>((EntityEventRefHandler<MagicMirrorComponent, AfterInteractEvent>)OnMagicMirrorInteract, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<MagicMirrorComponent, BeforeActivatableUIOpenEvent>((EntityEventRefHandler<MagicMirrorComponent, BeforeActivatableUIOpenEvent>)OnBeforeUIOpen, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<MagicMirrorComponent, ActivatableUIOpenAttemptEvent>((ComponentEventRefHandler<MagicMirrorComponent, ActivatableUIOpenAttemptEvent>)OnAttemptOpenUI, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<MagicMirrorComponent, BoundUserInterfaceCheckRangeEvent>((ComponentEventRefHandler<MagicMirrorComponent, BoundUserInterfaceCheckRangeEvent>)OnMirrorRangeCheck, (Type[])null, (Type[])null);
	}

	private void OnMagicMirrorInteract(Entity<MagicMirrorComponent> mirror, ref AfterInteractEvent args)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		if (args.CanReach && args.Target.HasValue)
		{
			UpdateInterface(Entity<MagicMirrorComponent>.op_Implicit(mirror), args.Target.Value, Entity<MagicMirrorComponent>.op_Implicit(mirror));
			UISystem.TryOpenUi(Entity<UserInterfaceComponent>.op_Implicit(mirror.Owner), (Enum)MagicMirrorUiKey.Key, args.User, false);
		}
	}

	private void OnMirrorRangeCheck(EntityUid uid, MagicMirrorComponent component, ref BoundUserInterfaceCheckRangeEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Invalid comparison between Unknown and I4
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		if ((int)args.Result != 2)
		{
			if (!component.Target.HasValue || !((EntitySystem)this).Exists(component.Target))
			{
				component.Target = null;
				args.Result = (BoundUserInterfaceRangeResult)2;
			}
			else if (!_interaction.InRangeUnobstructed(Entity<TransformComponent>.op_Implicit(component.Target.Value), Entity<TransformComponent>.op_Implicit(uid)))
			{
				args.Result = (BoundUserInterfaceRangeResult)2;
			}
		}
	}

	private void OnAttemptOpenUI(EntityUid uid, MagicMirrorComponent component, ref ActivatableUIOpenAttemptEvent args)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		EntityUid user = (EntityUid)(((_003F?)component.Target) ?? args.User);
		if (!((EntitySystem)this).HasComp<HumanoidAppearanceComponent>(user))
		{
			((CancellableEntityEventArgs)args).Cancel();
		}
	}

	private void OnBeforeUIOpen(Entity<MagicMirrorComponent> ent, ref BeforeActivatableUIOpenEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		UpdateInterface(Entity<MagicMirrorComponent>.op_Implicit(ent), args.User, Entity<MagicMirrorComponent>.op_Implicit(ent));
	}

	protected void UpdateInterface(EntityUid mirrorUid, EntityUid targetUid, MagicMirrorComponent component)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		HumanoidAppearanceComponent humanoid = default(HumanoidAppearanceComponent);
		if (((EntitySystem)this).TryComp<HumanoidAppearanceComponent>(targetUid, ref humanoid))
		{
			EntityUid valueOrDefault = component.Target.GetValueOrDefault();
			if (!component.Target.HasValue)
			{
				valueOrDefault = targetUid;
				component.Target = valueOrDefault;
			}
			IReadOnlyList<Marking> hairMarkings;
			List<Marking> hair = (humanoid.MarkingSet.TryGetCategory(MarkingCategories.Hair, out hairMarkings) ? new List<Marking>(hairMarkings) : new List<Marking>());
			IReadOnlyList<Marking> facialHairMarkings;
			List<Marking> facialHair = (humanoid.MarkingSet.TryGetCategory(MarkingCategories.FacialHair, out facialHairMarkings) ? new List<Marking>(facialHairMarkings) : new List<Marking>());
			MagicMirrorUiState state = new MagicMirrorUiState(ProtoId<SpeciesPrototype>.op_Implicit(humanoid.Species), hair, humanoid.MarkingSet.PointsLeft(MarkingCategories.Hair) + hair.Count, facialHair, humanoid.MarkingSet.PointsLeft(MarkingCategories.FacialHair) + facialHair.Count);
			component.Target = targetUid;
			UISystem.SetUiState(Entity<UserInterfaceComponent>.op_Implicit(mirrorUid), (Enum)MagicMirrorUiKey.Key, (BoundUserInterfaceState)(object)state);
			((EntitySystem)this).Dirty(mirrorUid, (IComponent)(object)component, (MetaDataComponent)null);
		}
	}
}
