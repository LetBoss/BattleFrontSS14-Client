using System;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Content.Shared.Xenoarchaeology.Artifact.XAE.Components;

[RegisterComponent]
[Access(new Type[] { typeof(XAEApplyComponentsSystem) })]
public sealed class XAEApplyComponentsComponent : Component, ISerializationGenerated<XAEApplyComponentsComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public ComponentRegistry Components = new ComponentRegistry();

	[DataField(null, false, 1, false, false, null)]
	public bool ApplyIfAlreadyHave { get; set; }

	[DataField(null, false, 1, false, false, null)]
	public bool RefreshOnReactivate { get; set; }

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref XAEApplyComponentsComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (XAEApplyComponentsComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<XAEApplyComponentsComponent>(this, ref target, hookCtx, false, context))
		{
			ComponentRegistry ComponentsTemp = null;
			if (Components == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<ComponentRegistry>(Components, ref ComponentsTemp, hookCtx, false, context))
			{
				ComponentsTemp = serialization.CreateCopy<ComponentRegistry>(Components, hookCtx, context, false);
			}
			target.Components = ComponentsTemp;
			bool ApplyIfAlreadyHaveTemp = false;
			if (!serialization.TryCustomCopy<bool>(ApplyIfAlreadyHave, ref ApplyIfAlreadyHaveTemp, hookCtx, false, context))
			{
				ApplyIfAlreadyHaveTemp = ApplyIfAlreadyHave;
			}
			target.ApplyIfAlreadyHave = ApplyIfAlreadyHaveTemp;
			bool RefreshOnReactivateTemp = false;
			if (!serialization.TryCustomCopy<bool>(RefreshOnReactivate, ref RefreshOnReactivateTemp, hookCtx, false, context))
			{
				RefreshOnReactivateTemp = RefreshOnReactivate;
			}
			target.RefreshOnReactivate = RefreshOnReactivateTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref XAEApplyComponentsComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		XAEApplyComponentsComponent cast = (XAEApplyComponentsComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		XAEApplyComponentsComponent cast = (XAEApplyComponentsComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		XAEApplyComponentsComponent def = (XAEApplyComponentsComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override XAEApplyComponentsComponent Instantiate()
	{
		return new XAEApplyComponentsComponent();
	}
}
