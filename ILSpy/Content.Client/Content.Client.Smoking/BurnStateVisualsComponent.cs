using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Content.Client.Smoking;

[RegisterComponent]
public sealed class BurnStateVisualsComponent : Component, ISerializationGenerated<BurnStateVisualsComponent>, ISerializationGenerated
{
	[DataField("burntIcon", false, 1, false, false, null)]
	public string BurntIcon = "burnt-icon";

	[DataField("litIcon", false, 1, false, false, null)]
	public string LitIcon = "lit-icon";

	[DataField("unlitIcon", false, 1, false, false, null)]
	public string UnlitIcon = "icon";

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref BurnStateVisualsComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		Component val = (Component)(object)target;
		((Component)this).InternalCopy(ref val, serialization, hookCtx, context);
		target = (BurnStateVisualsComponent)(object)val;
		if (!serialization.TryCustomCopy<BurnStateVisualsComponent>(this, ref target, hookCtx, false, context))
		{
			string burntIcon = null;
			if (BurntIcon == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<string>(BurntIcon, ref burntIcon, hookCtx, false, context))
			{
				burntIcon = BurntIcon;
			}
			target.BurntIcon = burntIcon;
			string litIcon = null;
			if (LitIcon == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<string>(LitIcon, ref litIcon, hookCtx, false, context))
			{
				litIcon = LitIcon;
			}
			target.LitIcon = litIcon;
			string unlitIcon = null;
			if (UnlitIcon == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<string>(UnlitIcon, ref unlitIcon, hookCtx, false, context))
			{
				unlitIcon = UnlitIcon;
			}
			target.UnlitIcon = unlitIcon;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref BurnStateVisualsComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		BurnStateVisualsComponent target2 = (BurnStateVisualsComponent)(object)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = (Component)(object)target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		BurnStateVisualsComponent target2 = (BurnStateVisualsComponent)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		BurnStateVisualsComponent target2 = (BurnStateVisualsComponent)(object)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = (IComponent)(object)target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override BurnStateVisualsComponent Instantiate()
	{
		return new BurnStateVisualsComponent();
	}
}
