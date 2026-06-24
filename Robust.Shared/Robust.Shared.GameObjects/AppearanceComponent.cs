using System;
using System.Collections.Generic;
using Robust.Shared.Analyzers;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Robust.Shared.GameObjects;

[RegisterComponent]
[NetworkedComponent]
[Access(new Type[] { typeof(SharedAppearanceSystem) })]
public sealed class AppearanceComponent : Component, ISerializationGenerated<AppearanceComponent>, ISerializationGenerated
{
	[ViewVariables]
	internal bool AppearanceDirty;

	[ViewVariables]
	internal bool UpdateQueued;

	[ViewVariables]
	internal Dictionary<Enum, object> AppearanceData = new Dictionary<Enum, object>();

	private Dictionary<Enum, object>? _appearanceDataInit;

	[DataField(null, true, 1, false, false, null)]
	public Dictionary<Enum, object>? AppearanceDataInit
	{
		get
		{
			return _appearanceDataInit;
		}
		set
		{
			AppearanceData = value ?? AppearanceData;
			_appearanceDataInit = value;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref AppearanceComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Component target2 = target;
		base.InternalCopy(ref target2, serialization, hookCtx, context);
		target = (AppearanceComponent)target2;
		if (!serialization.TryCustomCopy(this, ref target, hookCtx, hasHooks: false, context))
		{
			Dictionary<Enum, object> target3 = null;
			if (!serialization.TryCustomCopy(AppearanceDataInit, ref target3, hookCtx, hasHooks: true, context))
			{
				target3 = serialization.CreateCopy(AppearanceDataInit, hookCtx, context);
			}
			target.AppearanceDataInit = target3;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref AppearanceComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		AppearanceComponent target2 = (AppearanceComponent)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		AppearanceComponent target2 = (AppearanceComponent)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		AppearanceComponent target2 = (AppearanceComponent)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override AppearanceComponent Instantiate()
	{
		return new AppearanceComponent();
	}
}
