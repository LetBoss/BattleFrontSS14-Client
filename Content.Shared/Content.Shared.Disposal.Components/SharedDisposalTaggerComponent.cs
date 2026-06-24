using System;
using System.Text.RegularExpressions;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;

namespace Content.Shared.Disposal.Components;

public sealed class SharedDisposalTaggerComponent : Component, ISerializationGenerated<SharedDisposalTaggerComponent>, ISerializationGenerated
{
	[Serializable]
	[NetSerializable]
	public sealed class DisposalTaggerUserInterfaceState : BoundUserInterfaceState
	{
		public readonly string Tag;

		public DisposalTaggerUserInterfaceState(string tag)
		{
			Tag = tag;
		}
	}

	[Serializable]
	[NetSerializable]
	public sealed class UiActionMessage : BoundUserInterfaceMessage
	{
		public readonly UiAction Action;

		public readonly string Tag = "";

		public UiActionMessage(UiAction action, string tag)
		{
			Action = action;
			if (Action == UiAction.Ok)
			{
				Tag = tag.Substring(0, Math.Min(tag.Length, 30));
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
	public enum DisposalTaggerUiKey
	{
		Key
	}

	public static readonly Regex TagRegex = new Regex("^[a-zA-Z0-9 ]*$", RegexOptions.Compiled);

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref SharedDisposalTaggerComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (SharedDisposalTaggerComponent)(object)definitionCast;
		serialization.TryCustomCopy<SharedDisposalTaggerComponent>(this, ref target, hookCtx, false, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref SharedDisposalTaggerComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		SharedDisposalTaggerComponent cast = (SharedDisposalTaggerComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		SharedDisposalTaggerComponent cast = (SharedDisposalTaggerComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		SharedDisposalTaggerComponent def = (SharedDisposalTaggerComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override SharedDisposalTaggerComponent Instantiate()
	{
		return new SharedDisposalTaggerComponent();
	}
}
