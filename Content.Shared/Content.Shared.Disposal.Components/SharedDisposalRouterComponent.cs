using System;
using System.Text.RegularExpressions;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;

namespace Content.Shared.Disposal.Components;

public sealed class SharedDisposalRouterComponent : Component, ISerializationGenerated<SharedDisposalRouterComponent>, ISerializationGenerated
{
	[Serializable]
	[NetSerializable]
	public sealed class DisposalRouterUserInterfaceState : BoundUserInterfaceState
	{
		public readonly string Tags;

		public DisposalRouterUserInterfaceState(string tags)
		{
			Tags = tags;
		}
	}

	[Serializable]
	[NetSerializable]
	public sealed class UiActionMessage : BoundUserInterfaceMessage
	{
		public readonly UiAction Action;

		public readonly string Tags = "";

		public UiActionMessage(UiAction action, string tags)
		{
			Action = action;
			if (Action == UiAction.Ok)
			{
				Tags = tags.Substring(0, Math.Min(tags.Length, 150));
			}
		}
	}

	[Serializable]
	[NetSerializable]
	public enum UiAction
	{
		Ok
	}

	[Serializable]
	[NetSerializable]
	public enum DisposalRouterUiKey
	{
		Key
	}

	public static readonly Regex TagRegex = new Regex("^[a-zA-Z0-9, ]*$", RegexOptions.Compiled);

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref SharedDisposalRouterComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (SharedDisposalRouterComponent)(object)definitionCast;
		serialization.TryCustomCopy<SharedDisposalRouterComponent>(this, ref target, hookCtx, false, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref SharedDisposalRouterComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		SharedDisposalRouterComponent cast = (SharedDisposalRouterComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		SharedDisposalRouterComponent cast = (SharedDisposalRouterComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		SharedDisposalRouterComponent def = (SharedDisposalRouterComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override SharedDisposalRouterComponent Instantiate()
	{
		return new SharedDisposalRouterComponent();
	}
}
