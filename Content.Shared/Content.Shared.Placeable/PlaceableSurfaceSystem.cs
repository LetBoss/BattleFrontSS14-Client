using System;
using System.Numerics;
using Content.Shared.Hands.Components;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Interaction;
using Content.Shared.Storage;
using Content.Shared.Storage.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;

namespace Content.Shared.Placeable;

public sealed class PlaceableSurfaceSystem : EntitySystem
{
	[Dependency]
	private SharedHandsSystem _handsSystem;

	[Dependency]
	private SharedTransformSystem _transformSystem;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<PlaceableSurfaceComponent, AfterInteractUsingEvent>((ComponentEventHandler<PlaceableSurfaceComponent, AfterInteractUsingEvent>)OnAfterInteractUsing, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<PlaceableSurfaceComponent, StorageInteractUsingAttemptEvent>((EntityEventRefHandler<PlaceableSurfaceComponent, StorageInteractUsingAttemptEvent>)OnStorageInteractUsingAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<PlaceableSurfaceComponent, StorageAfterOpenEvent>((EntityEventRefHandler<PlaceableSurfaceComponent, StorageAfterOpenEvent>)OnStorageAfterOpen, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<PlaceableSurfaceComponent, StorageAfterCloseEvent>((EntityEventRefHandler<PlaceableSurfaceComponent, StorageAfterCloseEvent>)OnStorageAfterClose, (Type[])null, (Type[])null);
	}

	public void SetPlaceable(EntityUid uid, bool isPlaceable, PlaceableSurfaceComponent? surface = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<PlaceableSurfaceComponent>(uid, ref surface, false) && surface.IsPlaceable != isPlaceable)
		{
			surface.IsPlaceable = isPlaceable;
			((EntitySystem)this).Dirty(uid, (IComponent)(object)surface, (MetaDataComponent)null);
		}
	}

	public void SetPlaceCentered(EntityUid uid, bool placeCentered, PlaceableSurfaceComponent? surface = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<PlaceableSurfaceComponent>(uid, ref surface, true))
		{
			surface.PlaceCentered = placeCentered;
			((EntitySystem)this).Dirty(uid, (IComponent)(object)surface, (MetaDataComponent)null);
		}
	}

	public void SetPositionOffset(EntityUid uid, Vector2 offset, PlaceableSurfaceComponent? surface = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<PlaceableSurfaceComponent>(uid, ref surface, true))
		{
			surface.PositionOffset = offset;
			((EntitySystem)this).Dirty(uid, (IComponent)(object)surface, (MetaDataComponent)null);
		}
	}

	private void OnAfterInteractUsing(EntityUid uid, PlaceableSurfaceComponent surface, AfterInteractUsingEvent args)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		if (!((HandledEntityEventArgs)args).Handled && args.CanReach && surface.IsPlaceable && !((EntitySystem)this).HasComp<DumpableComponent>(args.Used) && _handsSystem.TryDrop(Entity<HandsComponent>.op_Implicit(args.User), args.Used))
		{
			SharedTransformSystem transformSystem = _transformSystem;
			EntityUid used = args.Used;
			EntityCoordinates val;
			if (!surface.PlaceCentered)
			{
				val = args.ClickLocation;
			}
			else
			{
				EntityCoordinates coordinates = ((EntitySystem)this).Transform(uid).Coordinates;
				val = ((EntityCoordinates)(ref coordinates)).Offset(surface.PositionOffset);
			}
			transformSystem.SetCoordinates(used, val);
			((HandledEntityEventArgs)args).Handled = true;
		}
	}

	private void OnStorageInteractUsingAttempt(Entity<PlaceableSurfaceComponent> ent, ref StorageInteractUsingAttemptEvent args)
	{
		args.Cancelled = true;
	}

	private void OnStorageAfterOpen(Entity<PlaceableSurfaceComponent> ent, ref StorageAfterOpenEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		SetPlaceable(ent.Owner, isPlaceable: true, ent.Comp);
	}

	private void OnStorageAfterClose(Entity<PlaceableSurfaceComponent> ent, ref StorageAfterCloseEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		SetPlaceable(ent.Owner, isPlaceable: false, ent.Comp);
	}
}
