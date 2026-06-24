using System;
using Content.Shared.ActionBlocker;
using Content.Shared.Clothing;
using Content.Shared.Inventory;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;

namespace Content.Shared.Chat.TypingIndicator;

public abstract class SharedTypingIndicatorSystem : EntitySystem
{
	[Dependency]
	private ActionBlockerSystem _actionBlocker;

	[Dependency]
	private SharedAppearanceSystem _appearance;

	[Dependency]
	private IGameTiming _timing;

	public static readonly ProtoId<TypingIndicatorPrototype> InitialIndicatorId = ProtoId<TypingIndicatorPrototype>.op_Implicit("default");

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<PlayerAttachedEvent>((EntityEventHandler<PlayerAttachedEvent>)OnPlayerAttached, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<TypingIndicatorComponent, PlayerDetachedEvent>((ComponentEventHandler<TypingIndicatorComponent, PlayerDetachedEvent>)OnPlayerDetached, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<TypingIndicatorClothingComponent, ClothingGotEquippedEvent>((EntityEventRefHandler<TypingIndicatorClothingComponent, ClothingGotEquippedEvent>)OnGotEquipped, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<TypingIndicatorClothingComponent, ClothingGotUnequippedEvent>((EntityEventRefHandler<TypingIndicatorClothingComponent, ClothingGotUnequippedEvent>)OnGotUnequipped, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<TypingIndicatorClothingComponent, InventoryRelayedEvent<BeforeShowTypingIndicatorEvent>>((EntityEventRefHandler<TypingIndicatorClothingComponent, InventoryRelayedEvent<BeforeShowTypingIndicatorEvent>>)BeforeShow, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeAllEvent<TypingChangedEvent>((EntitySessionEventHandler<TypingChangedEvent>)OnTypingChanged, (Type[])null, (Type[])null);
	}

	private void OnPlayerAttached(PlayerAttachedEvent ev)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).EnsureComp<TypingIndicatorComponent>(ev.Entity);
		((EntitySystem)this).EnsureComp<AppearanceComponent>(ev.Entity);
	}

	private void OnPlayerDetached(EntityUid uid, TypingIndicatorComponent component, PlayerDetachedEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		SetTypingIndicatorState(uid, TypingIndicatorState.None);
	}

	private void OnGotEquipped(Entity<TypingIndicatorClothingComponent> entity, ref ClothingGotEquippedEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		entity.Comp.GotEquippedTime = _timing.CurTime;
	}

	private void OnGotUnequipped(Entity<TypingIndicatorClothingComponent> entity, ref ClothingGotUnequippedEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		entity.Comp.GotEquippedTime = null;
	}

	private void BeforeShow(Entity<TypingIndicatorClothingComponent> entity, ref InventoryRelayedEvent<BeforeShowTypingIndicatorEvent> args)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		args.Args.TryUpdateTimeAndIndicator(entity.Comp.TypingIndicatorPrototype, entity.Comp.GotEquippedTime);
	}

	private void OnTypingChanged(TypingChangedEvent ev, EntitySessionEventArgs args)
	{
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? uid = ((EntitySessionEventArgs)(ref args)).SenderSession.AttachedEntity;
		if (!((EntitySystem)this).Exists(uid))
		{
			((EntitySystem)this).Log.Warning($"Client {((EntitySessionEventArgs)(ref args)).SenderSession} sent TypingChangedEvent without an attached entity.");
		}
		else if (!_actionBlocker.CanEmote(uid.Value) && !_actionBlocker.CanSpeak(uid.Value))
		{
			SetTypingIndicatorState(uid.Value, TypingIndicatorState.None);
		}
		else
		{
			SetTypingIndicatorState(uid.Value, ev.State);
		}
	}

	private void SetTypingIndicatorState(EntityUid uid, TypingIndicatorState state, AppearanceComponent? appearance = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<AppearanceComponent>(uid, ref appearance, false))
		{
			_appearance.SetData(uid, (Enum)TypingIndicatorVisuals.State, (object)state, appearance);
		}
	}
}
