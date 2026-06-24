using System;
using Content.Shared.Emag.Systems;
using Content.Shared.Examine;
using Content.Shared.Storage.Components;
using Content.Shared.Xenoarchaeology.Equipment.Components;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;

namespace Content.Shared.Xenoarchaeology.Equipment;

public abstract class SharedArtifactCrusherSystem : EntitySystem
{
	[Dependency]
	protected SharedAppearanceSystem Appearance;

	[Dependency]
	protected SharedAudioSystem AudioSystem;

	[Dependency]
	protected SharedContainerSystem ContainerSystem;

	[Dependency]
	private EmagSystem _emag;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<ArtifactCrusherComponent, ComponentInit>((EntityEventRefHandler<ArtifactCrusherComponent, ComponentInit>)OnInit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ArtifactCrusherComponent, StorageAfterOpenEvent>((EntityEventRefHandler<ArtifactCrusherComponent, StorageAfterOpenEvent>)OnStorageAfterOpen, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ArtifactCrusherComponent, StorageOpenAttemptEvent>((EntityEventRefHandler<ArtifactCrusherComponent, StorageOpenAttemptEvent>)OnStorageOpenAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ArtifactCrusherComponent, ExaminedEvent>((EntityEventRefHandler<ArtifactCrusherComponent, ExaminedEvent>)OnExamine, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ArtifactCrusherComponent, GotEmaggedEvent>((EntityEventRefHandler<ArtifactCrusherComponent, GotEmaggedEvent>)OnEmagged, (Type[])null, (Type[])null);
	}

	private void OnInit(Entity<ArtifactCrusherComponent> ent, ref ComponentInit args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		ent.Comp.OutputContainer = ContainerSystem.EnsureContainer<Container>(Entity<ArtifactCrusherComponent>.op_Implicit(ent), ent.Comp.OutputContainerName, (ContainerManagerComponent)null);
	}

	private void OnStorageAfterOpen(Entity<ArtifactCrusherComponent> ent, ref StorageAfterOpenEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		StopCrushing(ent);
		ContainerSystem.EmptyContainer((BaseContainer)(object)ent.Comp.OutputContainer, false, (EntityCoordinates?)null, true);
	}

	private void OnEmagged(Entity<ArtifactCrusherComponent> ent, ref GotEmaggedEvent args)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		if (_emag.CompareFlag(args.Type, EmagType.Interaction) && !_emag.CheckFlag(Entity<ArtifactCrusherComponent>.op_Implicit(ent), EmagType.Interaction) && !ent.Comp.AutoLock)
		{
			ent.Comp.AutoLock = true;
			args.Handled = true;
		}
	}

	private void OnStorageOpenAttempt(Entity<ArtifactCrusherComponent> ent, ref StorageOpenAttemptEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		if (ent.Comp.AutoLock && ent.Comp.Crushing)
		{
			args.Cancelled = true;
		}
	}

	private void OnExamine(Entity<ArtifactCrusherComponent> ent, ref ExaminedEvent args)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		args.PushMarkup(ent.Comp.AutoLock ? base.Loc.GetString("artifact-crusher-examine-autolocks") : base.Loc.GetString("artifact-crusher-examine-no-autolocks"));
	}

	public void StopCrushing(Entity<ArtifactCrusherComponent> ent, bool early = true)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		Entity<ArtifactCrusherComponent> val = ent;
		EntityUid val2 = default(EntityUid);
		ArtifactCrusherComponent artifactCrusherComponent = default(ArtifactCrusherComponent);
		val.Deconstruct(ref val2, ref artifactCrusherComponent);
		ArtifactCrusherComponent crusher = artifactCrusherComponent;
		if (crusher.Crushing)
		{
			crusher.Crushing = false;
			Appearance.SetData(Entity<ArtifactCrusherComponent>.op_Implicit(ent), (Enum)ArtifactCrusherVisuals.Crushing, (object)false, (AppearanceComponent)null);
			if (early)
			{
				AudioSystem.Stop(crusher.CrushingSoundEntity?.Item1, crusher.CrushingSoundEntity?.Item2);
				crusher.CrushingSoundEntity = null;
			}
			((EntitySystem)this).Dirty(Entity<ArtifactCrusherComponent>.op_Implicit(ent), (IComponent)(object)ent.Comp, (MetaDataComponent)null);
		}
	}
}
