using System;
using System.Collections.Generic;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Utility;

namespace Content.Shared._PUBG;

[DataDefinition]
public sealed class ChunkedMapSettings : ISerializationGenerated<ChunkedMapSettings>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public bool Enabled;

	[DataField("light", false, 1, false, false, null)]
	public bool LightEnabled;

	[DataField("lightColor", false, 1, false, false, null)]
	public Color LightColor = Color.Black;

	[DataField(null, false, 1, false, false, null)]
	public int ChunkWidth;

	[DataField(null, false, 1, false, false, null)]
	public int ChunkHeight;

	[DataField(null, false, 1, false, false, null)]
	public List<ResPath> ChunkPaths = new List<ResPath>();

	[DataField(null, false, 1, false, false, null)]
	public ResPath? ChunkFolder;

	[DataField(null, false, 1, false, false, null)]
	public float ChunkLoadIntervalSeconds = 0.6f;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref ChunkedMapSettings target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		if (!serialization.TryCustomCopy<ChunkedMapSettings>(this, ref target, hookCtx, false, context))
		{
			bool EnabledTemp = false;
			if (!serialization.TryCustomCopy<bool>(Enabled, ref EnabledTemp, hookCtx, false, context))
			{
				EnabledTemp = Enabled;
			}
			target.Enabled = EnabledTemp;
			bool LightEnabledTemp = false;
			if (!serialization.TryCustomCopy<bool>(LightEnabled, ref LightEnabledTemp, hookCtx, false, context))
			{
				LightEnabledTemp = LightEnabled;
			}
			target.LightEnabled = LightEnabledTemp;
			Color LightColorTemp = default(Color);
			if (!serialization.TryCustomCopy<Color>(LightColor, ref LightColorTemp, hookCtx, false, context))
			{
				LightColorTemp = serialization.CreateCopy<Color>(LightColor, hookCtx, context, false);
			}
			target.LightColor = LightColorTemp;
			int ChunkWidthTemp = 0;
			if (!serialization.TryCustomCopy<int>(ChunkWidth, ref ChunkWidthTemp, hookCtx, false, context))
			{
				ChunkWidthTemp = ChunkWidth;
			}
			target.ChunkWidth = ChunkWidthTemp;
			int ChunkHeightTemp = 0;
			if (!serialization.TryCustomCopy<int>(ChunkHeight, ref ChunkHeightTemp, hookCtx, false, context))
			{
				ChunkHeightTemp = ChunkHeight;
			}
			target.ChunkHeight = ChunkHeightTemp;
			List<ResPath> ChunkPathsTemp = null;
			if (ChunkPaths == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<List<ResPath>>(ChunkPaths, ref ChunkPathsTemp, hookCtx, true, context))
			{
				ChunkPathsTemp = serialization.CreateCopy<List<ResPath>>(ChunkPaths, hookCtx, context, false);
			}
			target.ChunkPaths = ChunkPathsTemp;
			ResPath? ChunkFolderTemp = null;
			if (!serialization.TryCustomCopy<ResPath?>(ChunkFolder, ref ChunkFolderTemp, hookCtx, false, context))
			{
				ChunkFolderTemp = serialization.CreateCopy<ResPath?>(ChunkFolder, hookCtx, context, false);
			}
			target.ChunkFolder = ChunkFolderTemp;
			float ChunkLoadIntervalSecondsTemp = 0f;
			if (!serialization.TryCustomCopy<float>(ChunkLoadIntervalSeconds, ref ChunkLoadIntervalSecondsTemp, hookCtx, false, context))
			{
				ChunkLoadIntervalSecondsTemp = ChunkLoadIntervalSeconds;
			}
			target.ChunkLoadIntervalSeconds = ChunkLoadIntervalSecondsTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref ChunkedMapSettings target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ChunkedMapSettings cast = (ChunkedMapSettings)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public ChunkedMapSettings Instantiate()
	{
		return new ChunkedMapSettings();
	}
}
