using System;
using Content.Shared.Preferences;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Content.Shared.Humanoid;

[DataDefinition]
public sealed class HumanoidProfileExport : ISerializationGenerated<HumanoidProfileExport>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public string ForkId;

	[DataField(null, false, 1, false, false, null)]
	public int Version = 1;

	[DataField(null, false, 1, true, false, null)]
	public HumanoidCharacterProfile Profile;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref HumanoidProfileExport target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		if (serialization.TryCustomCopy<HumanoidProfileExport>(this, ref target, hookCtx, false, context))
		{
			return;
		}
		string ForkIdTemp = null;
		if (ForkId == null)
		{
			throw new NullNotAllowedException();
		}
		if (!serialization.TryCustomCopy<string>(ForkId, ref ForkIdTemp, hookCtx, false, context))
		{
			ForkIdTemp = ForkId;
		}
		target.ForkId = ForkIdTemp;
		int VersionTemp = 0;
		if (!serialization.TryCustomCopy<int>(Version, ref VersionTemp, hookCtx, false, context))
		{
			VersionTemp = Version;
		}
		target.Version = VersionTemp;
		HumanoidCharacterProfile ProfileTemp = null;
		if (Profile == null)
		{
			throw new NullNotAllowedException();
		}
		if (!serialization.TryCustomCopy<HumanoidCharacterProfile>(Profile, ref ProfileTemp, hookCtx, false, context))
		{
			if (Profile == null)
			{
				ProfileTemp = null;
			}
			else
			{
				serialization.CopyTo<HumanoidCharacterProfile>(Profile, ref ProfileTemp, hookCtx, context, true);
			}
		}
		target.Profile = ProfileTemp;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref HumanoidProfileExport target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		HumanoidProfileExport cast = (HumanoidProfileExport)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public HumanoidProfileExport Instantiate()
	{
		return new HumanoidProfileExport();
	}
}
