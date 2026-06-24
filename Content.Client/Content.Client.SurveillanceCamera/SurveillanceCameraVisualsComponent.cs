using System;
using System.Collections.Generic;
using Content.Shared.SurveillanceCamera;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Content.Client.SurveillanceCamera;

[RegisterComponent]
public sealed class SurveillanceCameraVisualsComponent : Component, ISerializationGenerated<SurveillanceCameraVisualsComponent>, ISerializationGenerated
{
	[DataField("sprites", false, 1, false, false, null)]
	public Dictionary<SurveillanceCameraVisuals, string> CameraSprites = new Dictionary<SurveillanceCameraVisuals, string>();

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref SurveillanceCameraVisualsComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		Component val = (Component)(object)target;
		((Component)this).InternalCopy(ref val, serialization, hookCtx, context);
		target = (SurveillanceCameraVisualsComponent)(object)val;
		if (!serialization.TryCustomCopy<SurveillanceCameraVisualsComponent>(this, ref target, hookCtx, false, context))
		{
			Dictionary<SurveillanceCameraVisuals, string> cameraSprites = null;
			if (CameraSprites == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<Dictionary<SurveillanceCameraVisuals, string>>(CameraSprites, ref cameraSprites, hookCtx, true, context))
			{
				cameraSprites = serialization.CreateCopy<Dictionary<SurveillanceCameraVisuals, string>>(CameraSprites, hookCtx, context, false);
			}
			target.CameraSprites = cameraSprites;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref SurveillanceCameraVisualsComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		SurveillanceCameraVisualsComponent target2 = (SurveillanceCameraVisualsComponent)(object)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = (Component)(object)target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		SurveillanceCameraVisualsComponent target2 = (SurveillanceCameraVisualsComponent)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		SurveillanceCameraVisualsComponent target2 = (SurveillanceCameraVisualsComponent)(object)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = (IComponent)(object)target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override SurveillanceCameraVisualsComponent Instantiate()
	{
		return new SurveillanceCameraVisualsComponent();
	}
}
