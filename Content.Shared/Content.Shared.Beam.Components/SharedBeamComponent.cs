using System;
using System.Collections.Generic;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Beam.Components;

public abstract class SharedBeamComponent : Component, ISerializationGenerated<SharedBeamComponent>, ISerializationGenerated
{
	[DataField("hitTargets", false, 1, false, false, null)]
	public HashSet<EntityUid> HitTargets = new HashSet<EntityUid>();

	[DataField("virtualBeamController", false, 1, false, false, null)]
	public EntityUid? VirtualBeamController;

	[DataField("originBeam", false, 1, false, false, null)]
	public EntityUid OriginBeam;

	[DataField("beamShooter", false, 1, false, false, null)]
	public EntityUid BeamShooter;

	[DataField("createdBeams", false, 1, false, false, null)]
	public HashSet<EntityUid> CreatedBeams = new HashSet<EntityUid>();

	[ViewVariables(/*Could not decode attribute arguments.*/)]
	[DataField("sound", false, 1, false, false, null)]
	public SoundSpecifier? Sound;

	public SharedBeamComponent()
	{
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public virtual void InternalCopy(ref SharedBeamComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (SharedBeamComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<SharedBeamComponent>(this, ref target, hookCtx, false, context))
		{
			HashSet<EntityUid> HitTargetsTemp = null;
			if (HitTargets == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<HashSet<EntityUid>>(HitTargets, ref HitTargetsTemp, hookCtx, true, context))
			{
				HitTargetsTemp = serialization.CreateCopy<HashSet<EntityUid>>(HitTargets, hookCtx, context, false);
			}
			target.HitTargets = HitTargetsTemp;
			EntityUid? VirtualBeamControllerTemp = null;
			if (!serialization.TryCustomCopy<EntityUid?>(VirtualBeamController, ref VirtualBeamControllerTemp, hookCtx, false, context))
			{
				VirtualBeamControllerTemp = serialization.CreateCopy<EntityUid?>(VirtualBeamController, hookCtx, context, false);
			}
			target.VirtualBeamController = VirtualBeamControllerTemp;
			EntityUid OriginBeamTemp = default(EntityUid);
			if (!serialization.TryCustomCopy<EntityUid>(OriginBeam, ref OriginBeamTemp, hookCtx, false, context))
			{
				OriginBeamTemp = serialization.CreateCopy<EntityUid>(OriginBeam, hookCtx, context, false);
			}
			target.OriginBeam = OriginBeamTemp;
			EntityUid BeamShooterTemp = default(EntityUid);
			if (!serialization.TryCustomCopy<EntityUid>(BeamShooter, ref BeamShooterTemp, hookCtx, false, context))
			{
				BeamShooterTemp = serialization.CreateCopy<EntityUid>(BeamShooter, hookCtx, context, false);
			}
			target.BeamShooter = BeamShooterTemp;
			HashSet<EntityUid> CreatedBeamsTemp = null;
			if (CreatedBeams == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<HashSet<EntityUid>>(CreatedBeams, ref CreatedBeamsTemp, hookCtx, true, context))
			{
				CreatedBeamsTemp = serialization.CreateCopy<HashSet<EntityUid>>(CreatedBeams, hookCtx, context, false);
			}
			target.CreatedBeams = CreatedBeamsTemp;
			SoundSpecifier SoundTemp = null;
			if (!serialization.TryCustomCopy<SoundSpecifier>(Sound, ref SoundTemp, hookCtx, true, context))
			{
				SoundTemp = serialization.CreateCopy<SoundSpecifier>(Sound, hookCtx, context, false);
			}
			target.Sound = SoundTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public virtual void Copy(ref SharedBeamComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		SharedBeamComponent cast = (SharedBeamComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		SharedBeamComponent cast = (SharedBeamComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		SharedBeamComponent def = (SharedBeamComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override SharedBeamComponent Instantiate()
	{
		throw new NotImplementedException();
	}
}
