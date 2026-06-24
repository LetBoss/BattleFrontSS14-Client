using System;
using Robust.Shared.Utility;
using YamlDotNet.RepresentationModel;

namespace Robust.Shared.Graphics;

public struct TextureSampleParameters : IEquatable<TextureSampleParameters>
{
	public static readonly TextureSampleParameters Default = new TextureSampleParameters
	{
		Filter = false,
		WrapMode = TextureWrapMode.None
	};

	public bool Filter { get; set; }

	public TextureWrapMode WrapMode { get; set; }

	public static TextureSampleParameters FromYaml(YamlMappingNode node)
	{
		TextureWrapMode wrapMode = TextureWrapMode.None;
		bool filter = false;
		if (node.TryGetNode("filter", out YamlNode returnNode))
		{
			filter = returnNode.AsBool();
		}
		if (node.TryGetNode("wrap", out YamlNode returnNode2))
		{
			wrapMode = returnNode2.AsString() switch
			{
				"none" => TextureWrapMode.None, 
				"repeat" => TextureWrapMode.Repeat, 
				"mirrored_repeat" => TextureWrapMode.MirroredRepeat, 
				_ => throw new ArgumentException("Not a valid wrap mode."), 
			};
		}
		return new TextureSampleParameters
		{
			Filter = filter,
			WrapMode = wrapMode
		};
	}

	public bool Equals(TextureSampleParameters other)
	{
		if (Filter == other.Filter)
		{
			return WrapMode == other.WrapMode;
		}
		return false;
	}

	public override bool Equals(object? obj)
	{
		if (obj is TextureSampleParameters other)
		{
			return Equals(other);
		}
		return false;
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(Filter, (int)WrapMode);
	}

	public static bool operator ==(TextureSampleParameters left, TextureSampleParameters right)
	{
		return left.Equals(right);
	}

	public static bool operator !=(TextureSampleParameters left, TextureSampleParameters right)
	{
		return !left.Equals(right);
	}
}
