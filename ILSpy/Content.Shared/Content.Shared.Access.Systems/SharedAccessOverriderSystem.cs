using System;
using Content.Shared.Access.Components;
using Content.Shared.Containers.ItemSlots;
using Content.Shared.DoAfter;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Log;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;

namespace Content.Shared.Access.Systems;

public abstract class SharedAccessOverriderSystem : EntitySystem
{
	[Serializable]
	[NetSerializable]
	public sealed class AccessOverriderDoAfterEvent : DoAfterEvent, ISerializationGenerated<AccessOverriderDoAfterEvent>, ISerializationGenerated
	{
		public override DoAfterEvent Clone()
		{
			return this;
		}

		[Obsolete("Use ISerializationManager.CopyTo instead")]
		public void InternalCopy(ref AccessOverriderDoAfterEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
		{
			DoAfterEvent definitionCast = target;
			base.InternalCopy(ref definitionCast, serialization, hookCtx, context);
			target = (AccessOverriderDoAfterEvent)definitionCast;
			serialization.TryCustomCopy<AccessOverriderDoAfterEvent>(this, ref target, hookCtx, false, context);
		}

		[Obsolete("Use ISerializationManager.CopyTo instead")]
		public void Copy(ref AccessOverriderDoAfterEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
		{
			InternalCopy(ref target, serialization, hookCtx, context);
		}

		[Obsolete("Use ISerializationManager.CopyTo instead")]
		public override void Copy(ref DoAfterEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
		{
			AccessOverriderDoAfterEvent cast = (AccessOverriderDoAfterEvent)target;
			Copy(ref cast, serialization, hookCtx, context);
			target = cast;
		}

		[Obsolete("Use ISerializationManager.CopyTo instead")]
		public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
		{
			AccessOverriderDoAfterEvent cast = (AccessOverriderDoAfterEvent)target;
			Copy(ref cast, serialization, hookCtx, context);
			target = cast;
		}

		[Obsolete("Use ISerializationManager.CreateCopy instead")]
		public override AccessOverriderDoAfterEvent Instantiate()
		{
			return new AccessOverriderDoAfterEvent();
		}
	}

	[Dependency]
	private ItemSlotsSystem _itemSlotsSystem;

	[Dependency]
	private ILogManager _log;

	public const string Sawmill = "accessoverrider";

	protected ISawmill _sawmill;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		_sawmill = _log.GetSawmill("accessoverrider");
		((EntitySystem)this).SubscribeLocalEvent<AccessOverriderComponent, ComponentInit>((ComponentEventHandler<AccessOverriderComponent, ComponentInit>)OnComponentInit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<AccessOverriderComponent, ComponentRemove>((ComponentEventHandler<AccessOverriderComponent, ComponentRemove>)OnComponentRemove, (Type[])null, (Type[])null);
	}

	private void OnComponentInit(EntityUid uid, AccessOverriderComponent component, ComponentInit args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		_itemSlotsSystem.AddItemSlot(uid, AccessOverriderComponent.PrivilegedIdCardSlotId, component.PrivilegedIdSlot);
	}

	private void OnComponentRemove(EntityUid uid, AccessOverriderComponent component, ComponentRemove args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		_itemSlotsSystem.RemoveItemSlot(uid, component.PrivilegedIdSlot);
	}
}
