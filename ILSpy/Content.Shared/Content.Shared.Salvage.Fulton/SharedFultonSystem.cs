using System;
using System.Numerics;
using Content.Shared.DoAfter;
using Content.Shared.Examine;
using Content.Shared.Foldable;
using Content.Shared.Interaction;
using Content.Shared.Popups;
using Content.Shared.Stacks;
using Content.Shared.Verbs;
using Content.Shared.Whitelist;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Timing;

namespace Content.Shared.Salvage.Fulton;

public abstract class SharedFultonSystem : EntitySystem
{
	[Serializable]
	[NetSerializable]
	private sealed class FultonedDoAfterEvent : SimpleDoAfterEvent, ISerializationGenerated<FultonedDoAfterEvent>, ISerializationGenerated
	{
		[Obsolete("Use ISerializationManager.CopyTo instead")]
		public void InternalCopy(ref FultonedDoAfterEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
		{
			SimpleDoAfterEvent definitionCast = target;
			base.InternalCopy(ref definitionCast, serialization, hookCtx, context);
			target = (FultonedDoAfterEvent)definitionCast;
			serialization.TryCustomCopy<FultonedDoAfterEvent>(this, ref target, hookCtx, false, context);
		}

		[Obsolete("Use ISerializationManager.CopyTo instead")]
		public void Copy(ref FultonedDoAfterEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
		{
			InternalCopy(ref target, serialization, hookCtx, context);
		}

		[Obsolete("Use ISerializationManager.CopyTo instead")]
		public override void Copy(ref SimpleDoAfterEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
		{
			FultonedDoAfterEvent cast = (FultonedDoAfterEvent)target;
			Copy(ref cast, serialization, hookCtx, context);
			target = cast;
		}

		[Obsolete("Use ISerializationManager.CopyTo instead")]
		public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
		{
			FultonedDoAfterEvent cast = (FultonedDoAfterEvent)target;
			Copy(ref cast, serialization, hookCtx, context);
			target = cast;
		}

		[Obsolete("Use ISerializationManager.CreateCopy instead")]
		public override FultonedDoAfterEvent Instantiate()
		{
			return new FultonedDoAfterEvent();
		}
	}

	[Serializable]
	[NetSerializable]
	protected sealed class FultonAnimationMessage : EntityEventArgs
	{
		public NetEntity Entity;

		public NetCoordinates Coordinates;
	}

	[Dependency]
	protected IGameTiming Timing;

	[Dependency]
	private MetaDataSystem _metadata;

	[Dependency]
	protected SharedAudioSystem Audio;

	[Dependency]
	private SharedDoAfterSystem _doAfter;

	[Dependency]
	private FoldableSystem _foldable;

	[Dependency]
	protected SharedContainerSystem Container;

	[Dependency]
	private SharedPopupSystem _popup;

	[Dependency]
	private SharedStackSystem _stack;

	[Dependency]
	protected SharedTransformSystem TransformSystem;

	[Dependency]
	private EntityWhitelistSystem _whitelistSystem;

	public static readonly EntProtoId EffectProto = EntProtoId.op_Implicit("FultonEffect");

	protected static readonly Vector2 EffectOffset = Vector2.Zero;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<FultonedDoAfterEvent>((EntityEventHandler<FultonedDoAfterEvent>)OnFultonDoAfter, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<FultonedComponent, GetVerbsEvent<InteractionVerb>>((ComponentEventHandler<FultonedComponent, GetVerbsEvent<InteractionVerb>>)OnFultonedGetVerbs, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<FultonedComponent, ExaminedEvent>((ComponentEventHandler<FultonedComponent, ExaminedEvent>)OnFultonedExamine, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<FultonedComponent, EntGotInsertedIntoContainerMessage>((ComponentEventHandler<FultonedComponent, EntGotInsertedIntoContainerMessage>)OnFultonContainerInserted, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<FultonComponent, AfterInteractEvent>((ComponentEventHandler<FultonComponent, AfterInteractEvent>)OnFultonInteract, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<FultonComponent, StackSplitEvent>((ComponentEventRefHandler<FultonComponent, StackSplitEvent>)OnFultonSplit, (Type[])null, (Type[])null);
	}

	private void OnFultonContainerInserted(EntityUid uid, FultonedComponent component, EntGotInsertedIntoContainerMessage args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).RemCompDeferred<FultonedComponent>(uid);
	}

	private void OnFultonedExamine(EntityUid uid, FultonedComponent component, ExaminedEvent args)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		TimeSpan remaining = component.NextFulton + _metadata.GetPauseTime(uid, (MetaDataComponent)null) - Timing.CurTime;
		string message = base.Loc.GetString("fulton-examine", (ValueTuple<string, object>)("time", $"{remaining.TotalSeconds:0.00}"));
		args.PushText(message);
	}

	private void OnFultonedGetVerbs(EntityUid uid, FultonedComponent component, GetVerbsEvent<InteractionVerb> args)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		if (args.CanAccess && args.CanInteract)
		{
			args.Verbs.Add(new InteractionVerb
			{
				Text = base.Loc.GetString("fulton-remove"),
				Act = delegate
				{
					//IL_0007: Unknown result type (might be due to invalid IL or missing references)
					Unfulton(uid);
				}
			});
		}
	}

	private void Unfulton(EntityUid uid, FultonedComponent? component = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<FultonedComponent>(uid, ref component, false) && component.Removeable)
		{
			((EntitySystem)this).RemCompDeferred<FultonedComponent>(uid);
		}
	}

	private void OnFultonDoAfter(FultonedDoAfterEvent args)
	{
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		FultonComponent fulton = default(FultonComponent);
		if (!args.Cancelled && args.Target.HasValue && ((EntitySystem)this).TryComp<FultonComponent>(args.Used, ref fulton) && _stack.Use(args.Used.Value, 1))
		{
			FultonedComponent fultoned = ((EntitySystem)this).AddComp<FultonedComponent>(args.Target.Value);
			fultoned.Beacon = fulton.Beacon;
			fultoned.NextFulton = Timing.CurTime + fulton.FultonDuration;
			fultoned.FultonDuration = fulton.FultonDuration;
			fultoned.Removeable = fulton.Removeable;
			UpdateAppearance(args.Target.Value, fultoned);
			((EntitySystem)this).Dirty(args.Target.Value, (IComponent)(object)fultoned, (MetaDataComponent)null);
			Audio.PlayPredicted(fulton.FultonSound, args.Target.Value, (EntityUid?)args.User, (AudioParams?)null);
		}
	}

	private void OnFultonInteract(EntityUid uid, FultonComponent component, AfterInteractEvent args)
	{
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		if (!args.Target.HasValue || ((HandledEntityEventArgs)args).Handled || !args.CanReach)
		{
			return;
		}
		FultonBeaconComponent beacon = default(FultonBeaconComponent);
		if (((EntitySystem)this).TryComp<FultonBeaconComponent>(args.Target, ref beacon))
		{
			if (!_foldable.IsFolded(args.Target.Value))
			{
				component.Beacon = args.Target.Value;
				Audio.PlayPredicted(beacon.LinkSound, uid, (EntityUid?)args.User, (AudioParams?)null);
				_popup.PopupClient(base.Loc.GetString("fulton-linked"), uid, args.User);
			}
			else
			{
				component.Beacon = EntityUid.Invalid;
				_popup.PopupClient(base.Loc.GetString("fulton-folded"), uid, args.User);
			}
		}
		else if (((EntitySystem)this).Deleted(component.Beacon))
		{
			_popup.PopupClient(base.Loc.GetString("fulton-not-found"), uid, args.User);
		}
		else if (!CanApplyFulton(args.Target.Value, component))
		{
			_popup.PopupClient(base.Loc.GetString("fulton-invalid"), uid, uid);
		}
		else if (((EntitySystem)this).HasComp<FultonedComponent>(args.Target))
		{
			_popup.PopupClient(base.Loc.GetString("fulton-fultoned"), uid, uid);
		}
		else
		{
			((HandledEntityEventArgs)args).Handled = true;
			FultonedDoAfterEvent ev = new FultonedDoAfterEvent();
			_doAfter.TryStartDoAfter(new DoAfterArgs((IEntityManager)(object)base.EntityManager, args.User, component.ApplyFultonDuration, ev, args.Target, args.Target, args.Used)
			{
				MovementThreshold = 0.5f,
				BreakOnMove = true,
				Broadcast = true,
				NeedHand = true
			});
		}
	}

	private void OnFultonSplit(EntityUid uid, FultonComponent component, ref StackSplitEvent args)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		FultonComponent newFulton = ((EntitySystem)this).EnsureComp<FultonComponent>(args.NewId);
		newFulton.Beacon = component.Beacon;
		((EntitySystem)this).Dirty(args.NewId, (IComponent)(object)newFulton, (MetaDataComponent)null);
	}

	protected virtual void UpdateAppearance(EntityUid uid, FultonedComponent fultoned)
	{
	}

	protected bool CanApplyFulton(EntityUid targetUid, FultonComponent component)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		if (!CanFulton(targetUid))
		{
			return false;
		}
		if (_whitelistSystem.IsWhitelistFailOrNull(component.Whitelist, targetUid))
		{
			return false;
		}
		return true;
	}

	protected bool CanFulton(EntityUid uid)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Transform(uid).Anchored)
		{
			return false;
		}
		if (Container.IsEntityInContainer(uid, (MetaDataComponent)null))
		{
			return false;
		}
		return true;
	}
}
