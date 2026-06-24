using System;
using Content.Client.Weapons.Ranged.Systems;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Client.Weapons.Ranged.Components;

[RegisterComponent]
[Access(new Type[] { typeof(GunSystem) })]
public sealed class MagazineVisualsComponent : Component, ISerializationGenerated<MagazineVisualsComponent>, ISerializationGenerated
{
	[DataField("magState", false, 1, false, false, null)]
	public string? MagState;

	[DataField("steps", false, 1, false, false, null)]
	public int MagSteps;

	[DataField("zeroVisible", false, 1, false, false, null)]
	public bool ZeroVisible;

	[DataField(null, false, 1, false, false, null)]
	public bool ZeroOnlyOnEmpty;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref MagazineVisualsComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Component val = (Component)(object)target;
		((Component)this).InternalCopy(ref val, serialization, hookCtx, context);
		target = (MagazineVisualsComponent)(object)val;
		if (!serialization.TryCustomCopy<MagazineVisualsComponent>(this, ref target, hookCtx, false, context))
		{
			string magState = null;
			if (!serialization.TryCustomCopy<string>(MagState, ref magState, hookCtx, false, context))
			{
				magState = MagState;
			}
			target.MagState = magState;
			int magSteps = 0;
			if (!serialization.TryCustomCopy<int>(MagSteps, ref magSteps, hookCtx, false, context))
			{
				magSteps = MagSteps;
			}
			target.MagSteps = magSteps;
			bool zeroVisible = false;
			if (!serialization.TryCustomCopy<bool>(ZeroVisible, ref zeroVisible, hookCtx, false, context))
			{
				zeroVisible = ZeroVisible;
			}
			target.ZeroVisible = zeroVisible;
			bool zeroOnlyOnEmpty = false;
			if (!serialization.TryCustomCopy<bool>(ZeroOnlyOnEmpty, ref zeroOnlyOnEmpty, hookCtx, false, context))
			{
				zeroOnlyOnEmpty = ZeroOnlyOnEmpty;
			}
			target.ZeroOnlyOnEmpty = zeroOnlyOnEmpty;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref MagazineVisualsComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		MagazineVisualsComponent target2 = (MagazineVisualsComponent)(object)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = (Component)(object)target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		MagazineVisualsComponent target2 = (MagazineVisualsComponent)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		MagazineVisualsComponent target2 = (MagazineVisualsComponent)(object)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = (IComponent)(object)target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override MagazineVisualsComponent Instantiate()
	{
		return new MagazineVisualsComponent();
	}
}
