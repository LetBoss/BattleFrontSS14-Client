using System;
using Content.Shared.Buckle;
using Content.Shared.Buckle.Components;
using Content.Shared.Construction.EntitySystems;
using Content.Shared.Popups;
using Content.Shared.Storage.Components;
using Content.Shared.Verbs;
using Robust.Shared.Analyzers;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Physics.Components;
using Robust.Shared.Serialization;
using Robust.Shared.Utility;

namespace Content.Shared.Foldable;

public sealed class FoldableSystem : EntitySystem
{
	[Serializable]
	[NetSerializable]
	public enum FoldedVisuals : byte
	{
		State
	}

	[Dependency]
	private SharedAppearanceSystem _appearance;

	[Dependency]
	private SharedBuckleSystem _buckle;

	[Dependency]
	private SharedContainerSystem _container;

	[Dependency]
	private AnchorableSystem _anchorable;

	[Dependency]
	private SharedPopupSystem _popup;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<FoldableComponent, GetVerbsEvent<AlternativeVerb>>((ComponentEventHandler<FoldableComponent, GetVerbsEvent<AlternativeVerb>>)AddFoldVerb, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<FoldableComponent, AfterAutoHandleStateEvent>((ComponentEventRefHandler<FoldableComponent, AfterAutoHandleStateEvent>)OnHandleState, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<FoldableComponent, ComponentInit>((ComponentEventHandler<FoldableComponent, ComponentInit>)OnFoldableInit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<FoldableComponent, ContainerGettingInsertedAttemptEvent>((ComponentEventHandler<FoldableComponent, ContainerGettingInsertedAttemptEvent>)OnInsertEvent, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<FoldableComponent, StorageOpenAttemptEvent>((ComponentEventRefHandler<FoldableComponent, StorageOpenAttemptEvent>)OnFoldableOpenAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<FoldableComponent, EntityStorageInsertedIntoAttemptEvent>((EntityEventRefHandler<FoldableComponent, EntityStorageInsertedIntoAttemptEvent>)OnEntityStorageAttemptInsert, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<FoldableComponent, StrapAttemptEvent>((ComponentEventRefHandler<FoldableComponent, StrapAttemptEvent>)OnStrapAttempt, (Type[])null, (Type[])null);
	}

	private void OnHandleState(EntityUid uid, FoldableComponent component, ref AfterAutoHandleStateEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		SetFolded(uid, component, component.IsFolded);
	}

	private void OnFoldableInit(EntityUid uid, FoldableComponent component, ComponentInit args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		SetFolded(uid, component, component.IsFolded);
	}

	private void OnFoldableOpenAttempt(EntityUid uid, FoldableComponent component, ref StorageOpenAttemptEvent args)
	{
		if (component.IsFolded)
		{
			args.Cancelled = true;
		}
	}

	public void OnStrapAttempt(EntityUid uid, FoldableComponent comp, ref StrapAttemptEvent args)
	{
		if (comp.IsFolded && !comp.FitIntoEntityStorage)
		{
			args.Cancelled = true;
		}
	}

	private void OnEntityStorageAttemptInsert(Entity<FoldableComponent> entity, ref EntityStorageInsertedIntoAttemptEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		if (entity.Comp.IsFolded)
		{
			args.Cancelled = true;
		}
	}

	public bool IsFolded(EntityUid uid, FoldableComponent? component = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<FoldableComponent>(uid, ref component, true))
		{
			return false;
		}
		return component.IsFolded;
	}

	public void SetFolded(EntityUid uid, FoldableComponent component, bool folded)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		component.IsFolded = folded;
		((EntitySystem)this).Dirty(uid, (IComponent)(object)component, (MetaDataComponent)null);
		_appearance.SetData(uid, (Enum)FoldedVisuals.State, (object)folded, (AppearanceComponent)null);
		if (component.EnableStrapOnUnfold)
		{
			_buckle.StrapSetEnabled(uid, !component.IsFolded);
		}
		FoldedEvent ev = new FoldedEvent(folded);
		((EntitySystem)this).RaiseLocalEvent<FoldedEvent>(uid, ref ev, false);
	}

	private void OnInsertEvent(EntityUid uid, FoldableComponent component, ContainerGettingInsertedAttemptEvent args)
	{
		if (!component.IsFolded && !component.CanFoldInsideContainer)
		{
			((CancellableEntityEventArgs)args).Cancel();
		}
	}

	public bool TryToggleFold(EntityUid uid, FoldableComponent comp, EntityUid? folder = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		bool num = TrySetFolded(uid, comp, !comp.IsFolded);
		if (!num && folder.HasValue)
		{
			if (comp.IsFolded)
			{
				_popup.PopupPredicted(base.Loc.GetString("foldable-unfold-fail", (ValueTuple<string, object>)("object", uid)), uid, folder.Value);
				return num;
			}
			_popup.PopupPredicted(base.Loc.GetString("foldable-fold-fail", (ValueTuple<string, object>)("object", uid)), uid, folder.Value);
		}
		return num;
	}

	public bool CanToggleFold(EntityUid uid, FoldableComponent? fold = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<FoldableComponent>(uid, ref fold, true))
		{
			return false;
		}
		if (_container.IsEntityInContainer(uid, (MetaDataComponent)null) && !fold.CanFoldInsideContainer)
		{
			return false;
		}
		PhysicsComponent body = default(PhysicsComponent);
		if (!((EntitySystem)this).TryComp<PhysicsComponent>(uid, ref body) || !_anchorable.TileFree(((EntitySystem)this).Transform(uid).Coordinates, body, uid))
		{
			return false;
		}
		if (fold.IsLocked)
		{
			return false;
		}
		FoldAttemptEvent ev = new FoldAttemptEvent(fold);
		((EntitySystem)this).RaiseLocalEvent<FoldAttemptEvent>(uid, ref ev, false);
		return !ev.Cancelled;
	}

	public bool TrySetFolded(EntityUid uid, FoldableComponent comp, bool state)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		if (state == comp.IsFolded)
		{
			return false;
		}
		if (!CanToggleFold(uid, comp))
		{
			return false;
		}
		SetFolded(uid, comp, state);
		return true;
	}

	private void AddFoldVerb(EntityUid uid, FoldableComponent component, GetVerbsEvent<AlternativeVerb> args)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Expected O, but got Unknown
		if (args.CanAccess && args.CanInteract && args.Hands != null && !component.IsLocked)
		{
			AlternativeVerb verb = new AlternativeVerb
			{
				Act = delegate
				{
					//IL_0007: Unknown result type (might be due to invalid IL or missing references)
					//IL_0018: Unknown result type (might be due to invalid IL or missing references)
					TryToggleFold(uid, component, args.User);
				},
				Text = (component.IsFolded ? base.Loc.GetString(LocId.op_Implicit(component.UnfoldVerbText)) : base.Loc.GetString(LocId.op_Implicit(component.FoldVerbText))),
				Icon = (SpriteSpecifier?)new Texture(new ResPath("/Textures/Interface/VerbIcons/fold.svg.192dpi.png")),
				Priority = ((!component.IsFolded) ? 2 : 0)
			};
			args.Verbs.Add(verb);
		}
	}
}
