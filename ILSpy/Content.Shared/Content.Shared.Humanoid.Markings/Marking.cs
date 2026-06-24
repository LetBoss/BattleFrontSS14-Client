using System;
using System.Collections.Generic;
using System.Linq;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Humanoid.Markings;

[Serializable]
[DataDefinition]
[NetSerializable]
public sealed class Marking : IEquatable<Marking>, IComparable<Marking>, IComparable<string>, ISerializationGenerated<Marking>, ISerializationGenerated
{
	[DataField("markingColor", false, 1, false, false, null)]
	private List<Color> _markingColors = new List<Color>();

	[DataField("visible", false, 1, false, false, null)]
	public bool Visible = true;

	[ViewVariables]
	public bool Forced;

	[DataField("markingId", false, 1, true, false, null)]
	public string MarkingId { get; private set; }

	[ViewVariables]
	public IReadOnlyList<Color> MarkingColors => _markingColors;

	private Marking()
	{
	}

	public Marking(string markingId, List<Color> markingColors)
	{
		MarkingId = markingId;
		_markingColors = markingColors;
	}

	public Marking(string markingId, IReadOnlyList<Color> markingColors)
		: this(markingId, new List<Color>(markingColors))
	{
	}

	public Marking(string markingId, int colorCount)
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		MarkingId = markingId;
		List<Color> colors = new List<Color>();
		for (int i = 0; i < colorCount; i++)
		{
			colors.Add(Color.White);
		}
		_markingColors = colors;
	}

	public Marking(Marking other)
	{
		MarkingId = other.MarkingId;
		_markingColors = new List<Color>(other.MarkingColors);
		Visible = other.Visible;
		Forced = other.Forced;
	}

	public void SetColor(int colorIndex, Color color)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		_markingColors[colorIndex] = color;
	}

	public void SetColor(Color color)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < _markingColors.Count; i++)
		{
			_markingColors[i] = color;
		}
	}

	public int CompareTo(Marking? marking)
	{
		if (marking == null)
		{
			return 1;
		}
		return string.Compare(MarkingId, marking.MarkingId, StringComparison.Ordinal);
	}

	public int CompareTo(string? markingId)
	{
		if (markingId == null)
		{
			return 1;
		}
		return string.Compare(MarkingId, markingId, StringComparison.Ordinal);
	}

	public bool Equals(Marking? other)
	{
		if (other == null)
		{
			return false;
		}
		if (MarkingId.Equals(other.MarkingId) && _markingColors.SequenceEqual(other._markingColors) && Visible.Equals(other.Visible))
		{
			return Forced.Equals(other.Forced);
		}
		return false;
	}

	public new string ToString()
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		string sanitizedName = MarkingId.Replace('@', '_');
		List<string> colorStringList = new List<string>();
		foreach (Color markingColor in _markingColors)
		{
			Color color = markingColor;
			colorStringList.Add(((Color)(ref color)).ToHex());
		}
		return sanitizedName + "@" + string.Join(',', colorStringList);
	}

	public static Marking? ParseFromDbString(string input)
	{
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		if (input.Length == 0)
		{
			return null;
		}
		string[] split = input.Split('@');
		if (split.Length != 2)
		{
			return null;
		}
		List<Color> colorList = new List<Color>();
		string[] array = split[1].Split(',');
		foreach (string color in array)
		{
			colorList.Add(Color.FromHex((ReadOnlySpan<char>)color, (Color?)null));
		}
		return new Marking(split[0], colorList);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref Marking target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		if (!serialization.TryCustomCopy<Marking>(this, ref target, hookCtx, false, context))
		{
			List<Color> _markingColorsTemp = null;
			if (_markingColors == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<List<Color>>(_markingColors, ref _markingColorsTemp, hookCtx, true, context))
			{
				_markingColorsTemp = serialization.CreateCopy<List<Color>>(_markingColors, hookCtx, context, false);
			}
			target._markingColors = _markingColorsTemp;
			string MarkingIdTemp = null;
			if (MarkingId == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<string>(MarkingId, ref MarkingIdTemp, hookCtx, false, context))
			{
				MarkingIdTemp = MarkingId;
			}
			target.MarkingId = MarkingIdTemp;
			bool VisibleTemp = false;
			if (!serialization.TryCustomCopy<bool>(Visible, ref VisibleTemp, hookCtx, false, context))
			{
				VisibleTemp = Visible;
			}
			target.Visible = VisibleTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref Marking target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Marking cast = (Marking)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public Marking Instantiate()
	{
		return new Marking();
	}
}
