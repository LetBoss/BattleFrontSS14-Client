using System;
using Robust.Shared.Utility;
using YamlDotNet.RepresentationModel;

namespace Robust.Shared.Graphics;

public struct TextureLoadParameters : IEquatable<TextureLoadParameters>
{
	public static readonly TextureLoadParameters Default = new TextureLoadParameters
	{
		SampleParameters = TextureSampleParameters.Default,
		Srgb = true,
		Preload = true
	};

	public TextureSampleParameters SampleParameters { get; set; }

	public bool Srgb { get; set; }

	public bool Preload { get; set; }

	public static TextureLoadParameters FromYaml(YamlMappingNode yaml)
	{
		TextureLoadParameters result = Default;
		if (yaml.TryGetNode<YamlMappingNode>("sample", out YamlMappingNode returnNode))
		{
			result.SampleParameters = TextureSampleParameters.FromYaml(returnNode);
		}
		if (yaml.TryGetNode("srgb", out YamlNode returnNode2))
		{
			result.Srgb = returnNode2.AsBool();
		}
		if (yaml.TryGetNode("preload", out YamlNode returnNode3))
		{
			result.Preload = returnNode3.AsBool();
		}
		return result;
	}

	public bool Equals(TextureLoadParameters other)
	{
		if (SampleParameters.Equals(other.SampleParameters) && Srgb == other.Srgb)
		{
			return Preload == other.Preload;
		}
		return false;
	}

	public override bool Equals(object? obj)
	{
		if (obj is TextureLoadParameters other)
		{
			return Equals(other);
		}
		return false;
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(SampleParameters, Srgb);
	}

	public static bool operator ==(TextureLoadParameters left, TextureLoadParameters right)
	{
		return left.Equals(right);
	}

	public static bool operator !=(TextureLoadParameters left, TextureLoadParameters right)
	{
		return !left.Equals(right);
	}
}
