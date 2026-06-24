using System;
using System.Numerics;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Utility;

namespace Content.Client.Paper.UI;

[RegisterComponent]
public sealed class PaperVisualsComponent : Component, ISerializationGenerated<PaperVisualsComponent>, ISerializationGenerated
{
	[DataField("backgroundImagePath", false, 1, false, false, null)]
	public string? BackgroundImagePath;

	[DataField("backgroundPatchMargin", false, 1, false, false, null)]
	public Box2 BackgroundPatchMargin;

	[DataField("backgroundModulate", false, 1, false, false, null)]
	public Color BackgroundModulate = Color.White;

	[DataField("backgroundImageTile", false, 1, false, false, null)]
	public bool BackgroundImageTile;

	[DataField("backgroundScale", false, 1, false, false, null)]
	public Vector2 BackgroundScale = Vector2.One;

	[DataField("headerImagePath", false, 1, false, false, null)]
	public string? HeaderImagePath;

	[DataField("headerImageModulate", false, 1, false, false, null)]
	public Color HeaderImageModulate = Color.White;

	[DataField("headerMargin", false, 1, false, false, null)]
	public Box2 HeaderMargin;

	[DataField(null, false, 1, false, false, null)]
	public ResPath? FooterImagePath;

	[DataField(null, false, 1, false, false, null)]
	public Color FooterImageModulate = Color.White;

	[DataField(null, false, 1, false, false, null)]
	public Box2 FooterMargin;

	[DataField("contentImagePath", false, 1, false, false, null)]
	public string? ContentImagePath;

	[DataField("contentImageModulate", false, 1, false, false, null)]
	public Color ContentImageModulate = Color.White;

	[DataField("contentMargin", false, 1, false, false, null)]
	public Box2 ContentMargin;

	[DataField("contentImageNumLines", false, 1, false, false, null)]
	public int ContentImageNumLines = 1;

	[DataField("fontAccentColor", false, 1, false, false, null)]
	public Color FontAccentColor = new Color((byte)223, (byte)223, (byte)213, byte.MaxValue);

	[DataField("maxWritableArea", false, 1, false, false, null)]
	public Vector2? MaxWritableArea;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref PaperVisualsComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0211: Unknown result type (might be due to invalid IL or missing references)
		//IL_0213: Unknown result type (might be due to invalid IL or missing references)
		//IL_021a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0222: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0208: Unknown result type (might be due to invalid IL or missing references)
		//IL_020d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0248: Unknown result type (might be due to invalid IL or missing references)
		//IL_024a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0236: Unknown result type (might be due to invalid IL or missing references)
		//IL_023f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0244: Unknown result type (might be due to invalid IL or missing references)
		//IL_0279: Unknown result type (might be due to invalid IL or missing references)
		//IL_0281: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0295: Unknown result type (might be due to invalid IL or missing references)
		//IL_029e: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02de: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02da: Unknown result type (might be due to invalid IL or missing references)
		//IL_030f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0317: Unknown result type (might be due to invalid IL or missing references)
		//IL_033d: Unknown result type (might be due to invalid IL or missing references)
		//IL_033f: Unknown result type (might be due to invalid IL or missing references)
		//IL_032b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0334: Unknown result type (might be due to invalid IL or missing references)
		//IL_0339: Unknown result type (might be due to invalid IL or missing references)
		Component val = (Component)(object)target;
		((Component)this).InternalCopy(ref val, serialization, hookCtx, context);
		target = (PaperVisualsComponent)(object)val;
		if (!serialization.TryCustomCopy<PaperVisualsComponent>(this, ref target, hookCtx, false, context))
		{
			string backgroundImagePath = null;
			if (!serialization.TryCustomCopy<string>(BackgroundImagePath, ref backgroundImagePath, hookCtx, false, context))
			{
				backgroundImagePath = BackgroundImagePath;
			}
			target.BackgroundImagePath = backgroundImagePath;
			Box2 backgroundPatchMargin = default(Box2);
			if (!serialization.TryCustomCopy<Box2>(BackgroundPatchMargin, ref backgroundPatchMargin, hookCtx, false, context))
			{
				backgroundPatchMargin = serialization.CreateCopy<Box2>(BackgroundPatchMargin, hookCtx, context, false);
			}
			target.BackgroundPatchMargin = backgroundPatchMargin;
			Color backgroundModulate = default(Color);
			if (!serialization.TryCustomCopy<Color>(BackgroundModulate, ref backgroundModulate, hookCtx, false, context))
			{
				backgroundModulate = serialization.CreateCopy<Color>(BackgroundModulate, hookCtx, context, false);
			}
			target.BackgroundModulate = backgroundModulate;
			bool backgroundImageTile = false;
			if (!serialization.TryCustomCopy<bool>(BackgroundImageTile, ref backgroundImageTile, hookCtx, false, context))
			{
				backgroundImageTile = BackgroundImageTile;
			}
			target.BackgroundImageTile = backgroundImageTile;
			Vector2 backgroundScale = default(Vector2);
			if (!serialization.TryCustomCopy<Vector2>(BackgroundScale, ref backgroundScale, hookCtx, false, context))
			{
				backgroundScale = serialization.CreateCopy<Vector2>(BackgroundScale, hookCtx, context, false);
			}
			target.BackgroundScale = backgroundScale;
			string headerImagePath = null;
			if (!serialization.TryCustomCopy<string>(HeaderImagePath, ref headerImagePath, hookCtx, false, context))
			{
				headerImagePath = HeaderImagePath;
			}
			target.HeaderImagePath = headerImagePath;
			Color headerImageModulate = default(Color);
			if (!serialization.TryCustomCopy<Color>(HeaderImageModulate, ref headerImageModulate, hookCtx, false, context))
			{
				headerImageModulate = serialization.CreateCopy<Color>(HeaderImageModulate, hookCtx, context, false);
			}
			target.HeaderImageModulate = headerImageModulate;
			Box2 headerMargin = default(Box2);
			if (!serialization.TryCustomCopy<Box2>(HeaderMargin, ref headerMargin, hookCtx, false, context))
			{
				headerMargin = serialization.CreateCopy<Box2>(HeaderMargin, hookCtx, context, false);
			}
			target.HeaderMargin = headerMargin;
			ResPath? footerImagePath = null;
			if (!serialization.TryCustomCopy<ResPath?>(FooterImagePath, ref footerImagePath, hookCtx, false, context))
			{
				footerImagePath = serialization.CreateCopy<ResPath?>(FooterImagePath, hookCtx, context, false);
			}
			target.FooterImagePath = footerImagePath;
			Color footerImageModulate = default(Color);
			if (!serialization.TryCustomCopy<Color>(FooterImageModulate, ref footerImageModulate, hookCtx, false, context))
			{
				footerImageModulate = serialization.CreateCopy<Color>(FooterImageModulate, hookCtx, context, false);
			}
			target.FooterImageModulate = footerImageModulate;
			Box2 footerMargin = default(Box2);
			if (!serialization.TryCustomCopy<Box2>(FooterMargin, ref footerMargin, hookCtx, false, context))
			{
				footerMargin = serialization.CreateCopy<Box2>(FooterMargin, hookCtx, context, false);
			}
			target.FooterMargin = footerMargin;
			string contentImagePath = null;
			if (!serialization.TryCustomCopy<string>(ContentImagePath, ref contentImagePath, hookCtx, false, context))
			{
				contentImagePath = ContentImagePath;
			}
			target.ContentImagePath = contentImagePath;
			Color contentImageModulate = default(Color);
			if (!serialization.TryCustomCopy<Color>(ContentImageModulate, ref contentImageModulate, hookCtx, false, context))
			{
				contentImageModulate = serialization.CreateCopy<Color>(ContentImageModulate, hookCtx, context, false);
			}
			target.ContentImageModulate = contentImageModulate;
			Box2 contentMargin = default(Box2);
			if (!serialization.TryCustomCopy<Box2>(ContentMargin, ref contentMargin, hookCtx, false, context))
			{
				contentMargin = serialization.CreateCopy<Box2>(ContentMargin, hookCtx, context, false);
			}
			target.ContentMargin = contentMargin;
			int contentImageNumLines = 0;
			if (!serialization.TryCustomCopy<int>(ContentImageNumLines, ref contentImageNumLines, hookCtx, false, context))
			{
				contentImageNumLines = ContentImageNumLines;
			}
			target.ContentImageNumLines = contentImageNumLines;
			Color fontAccentColor = default(Color);
			if (!serialization.TryCustomCopy<Color>(FontAccentColor, ref fontAccentColor, hookCtx, false, context))
			{
				fontAccentColor = serialization.CreateCopy<Color>(FontAccentColor, hookCtx, context, false);
			}
			target.FontAccentColor = fontAccentColor;
			Vector2? maxWritableArea = null;
			if (!serialization.TryCustomCopy<Vector2?>(MaxWritableArea, ref maxWritableArea, hookCtx, false, context))
			{
				maxWritableArea = serialization.CreateCopy<Vector2?>(MaxWritableArea, hookCtx, context, false);
			}
			target.MaxWritableArea = maxWritableArea;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref PaperVisualsComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		PaperVisualsComponent target2 = (PaperVisualsComponent)(object)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = (Component)(object)target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		PaperVisualsComponent target2 = (PaperVisualsComponent)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		PaperVisualsComponent target2 = (PaperVisualsComponent)(object)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = (IComponent)(object)target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override PaperVisualsComponent Instantiate()
	{
		return new PaperVisualsComponent();
	}
}
