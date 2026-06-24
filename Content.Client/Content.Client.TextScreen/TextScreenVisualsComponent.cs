using System;
using System.Collections.Generic;
using System.Numerics;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.ViewVariables;

namespace Content.Client.TextScreen;

[RegisterComponent]
public sealed class TextScreenVisualsComponent : Component, ISerializationGenerated<TextScreenVisualsComponent>, ISerializationGenerated
{
	public const float PixelSize = 1f / 32f;

	[DataField("color", false, 1, false, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public Color Color = new Color((byte)15, (byte)151, (byte)251, byte.MaxValue);

	[DataField("textOffset", false, 1, false, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public Vector2 TextOffset = Vector2.Zero;

	[DataField("timerOffset", false, 1, false, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public Vector2 TimerOffset = Vector2.Zero;

	[DataField("rows", false, 1, false, false, null)]
	public int Rows = 2;

	[DataField("rowOffset", false, 1, false, false, null)]
	public int RowOffset = 7;

	[DataField("rowLength", false, 1, false, false, null)]
	public int RowLength = 5;

	[DataField("text", false, 1, false, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public string?[] Text = new string[2];

	public string?[] TextToDraw = new string[2];

	[DataField("layerStatesToDraw", false, 1, false, false, null)]
	public Dictionary<string, string?> LayerStatesToDraw = new Dictionary<string, string>();

	[DataField("hourFormat", false, 1, false, false, null)]
	public string HourFormat = "D2";

	[DataField("minuteFormat", false, 1, false, false, null)]
	public string MinuteFormat = "D2";

	[DataField("secondFormat", false, 1, false, false, null)]
	public string SecondFormat = "D2";

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref TextScreenVisualsComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0234: Unknown result type (might be due to invalid IL or missing references)
		Component val = (Component)(object)target;
		((Component)this).InternalCopy(ref val, serialization, hookCtx, context);
		target = (TextScreenVisualsComponent)(object)val;
		if (!serialization.TryCustomCopy<TextScreenVisualsComponent>(this, ref target, hookCtx, false, context))
		{
			Color color = default(Color);
			if (!serialization.TryCustomCopy<Color>(Color, ref color, hookCtx, false, context))
			{
				color = serialization.CreateCopy<Color>(Color, hookCtx, context, false);
			}
			target.Color = color;
			Vector2 textOffset = default(Vector2);
			if (!serialization.TryCustomCopy<Vector2>(TextOffset, ref textOffset, hookCtx, false, context))
			{
				textOffset = serialization.CreateCopy<Vector2>(TextOffset, hookCtx, context, false);
			}
			target.TextOffset = textOffset;
			Vector2 timerOffset = default(Vector2);
			if (!serialization.TryCustomCopy<Vector2>(TimerOffset, ref timerOffset, hookCtx, false, context))
			{
				timerOffset = serialization.CreateCopy<Vector2>(TimerOffset, hookCtx, context, false);
			}
			target.TimerOffset = timerOffset;
			int rows = 0;
			if (!serialization.TryCustomCopy<int>(Rows, ref rows, hookCtx, false, context))
			{
				rows = Rows;
			}
			target.Rows = rows;
			int rowOffset = 0;
			if (!serialization.TryCustomCopy<int>(RowOffset, ref rowOffset, hookCtx, false, context))
			{
				rowOffset = RowOffset;
			}
			target.RowOffset = rowOffset;
			int rowLength = 0;
			if (!serialization.TryCustomCopy<int>(RowLength, ref rowLength, hookCtx, false, context))
			{
				rowLength = RowLength;
			}
			target.RowLength = rowLength;
			string[] text = null;
			if (Text == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<string[]>(Text, ref text, hookCtx, true, context))
			{
				text = serialization.CreateCopy<string[]>(Text, hookCtx, context, false);
			}
			target.Text = text;
			Dictionary<string, string> layerStatesToDraw = null;
			if (LayerStatesToDraw == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<Dictionary<string, string>>(LayerStatesToDraw, ref layerStatesToDraw, hookCtx, true, context))
			{
				layerStatesToDraw = serialization.CreateCopy<Dictionary<string, string>>(LayerStatesToDraw, hookCtx, context, false);
			}
			target.LayerStatesToDraw = layerStatesToDraw;
			string hourFormat = null;
			if (HourFormat == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<string>(HourFormat, ref hourFormat, hookCtx, false, context))
			{
				hourFormat = HourFormat;
			}
			target.HourFormat = hourFormat;
			string minuteFormat = null;
			if (MinuteFormat == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<string>(MinuteFormat, ref minuteFormat, hookCtx, false, context))
			{
				minuteFormat = MinuteFormat;
			}
			target.MinuteFormat = minuteFormat;
			string secondFormat = null;
			if (SecondFormat == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<string>(SecondFormat, ref secondFormat, hookCtx, false, context))
			{
				secondFormat = SecondFormat;
			}
			target.SecondFormat = secondFormat;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref TextScreenVisualsComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		TextScreenVisualsComponent target2 = (TextScreenVisualsComponent)(object)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = (Component)(object)target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		TextScreenVisualsComponent target2 = (TextScreenVisualsComponent)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		TextScreenVisualsComponent target2 = (TextScreenVisualsComponent)(object)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = (IComponent)(object)target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override TextScreenVisualsComponent Instantiate()
	{
		return new TextScreenVisualsComponent();
	}
}
