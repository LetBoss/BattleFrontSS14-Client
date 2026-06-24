using System;
using System.Collections.Generic;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Content.Shared.Roles.RoleCodeword;

[Serializable]
[DataDefinition]
[NetSerializable]
public struct CodewordsData : ISerializationGenerated<CodewordsData>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public Color Color;

	[DataField(null, false, 1, false, false, null)]
	public List<string> Codewords;

	public CodewordsData(Color color, List<string> codewords)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		Color = color;
		Codewords = codewords;
	}

	public CodewordsData()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		Color = default(Color);
		Codewords = null;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref CodewordsData target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		if (!serialization.TryCustomCopy<CodewordsData>(this, ref target, hookCtx, false, context))
		{
			Color ColorTemp = default(Color);
			if (!serialization.TryCustomCopy<Color>(Color, ref ColorTemp, hookCtx, false, context))
			{
				ColorTemp = serialization.CreateCopy<Color>(Color, hookCtx, context, false);
			}
			List<string> CodewordsTemp = null;
			if (Codewords == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<List<string>>(Codewords, ref CodewordsTemp, hookCtx, true, context))
			{
				CodewordsTemp = serialization.CreateCopy<List<string>>(Codewords, hookCtx, context, false);
			}
			CodewordsData codewordsData = target;
			codewordsData.Color = ColorTemp;
			codewordsData.Codewords = CodewordsTemp;
			target = codewordsData;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref CodewordsData target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		CodewordsData cast = (CodewordsData)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public CodewordsData Instantiate()
	{
		return new CodewordsData();
	}
}
