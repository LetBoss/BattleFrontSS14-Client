using System;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Content.Shared.Paper;

[Serializable]
[DataDefinition]
[NetSerializable]
public struct StampDisplayInfo : ISerializationGenerated<StampDisplayInfo>, ISerializationGenerated
{
	[DataField("stampedName", false, 1, false, false, null)]
	public string StampedName;

	[DataField("stampedColor", false, 1, false, false, null)]
	public Color StampedColor;

	private StampDisplayInfo(string s)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		StampedColor = default(Color);
		StampedName = s;
	}

	public StampDisplayInfo()
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		StampedName = null;
		StampedColor = default(Color);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref StampDisplayInfo target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		if (!serialization.TryCustomCopy<StampDisplayInfo>(this, ref target, hookCtx, false, context))
		{
			string StampedNameTemp = null;
			if (StampedName == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<string>(StampedName, ref StampedNameTemp, hookCtx, false, context))
			{
				StampedNameTemp = StampedName;
			}
			Color StampedColorTemp = default(Color);
			if (!serialization.TryCustomCopy<Color>(StampedColor, ref StampedColorTemp, hookCtx, false, context))
			{
				StampedColorTemp = serialization.CreateCopy<Color>(StampedColor, hookCtx, context, false);
			}
			StampDisplayInfo stampDisplayInfo = target;
			stampDisplayInfo.StampedName = StampedNameTemp;
			stampDisplayInfo.StampedColor = StampedColorTemp;
			target = stampDisplayInfo;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref StampDisplayInfo target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		StampDisplayInfo cast = (StampDisplayInfo)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public StampDisplayInfo Instantiate()
	{
		return new StampDisplayInfo();
	}
}
