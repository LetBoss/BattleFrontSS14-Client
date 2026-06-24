using System;
using System.IO;
using System.Text;
using YamlDotNet.RepresentationModel;

namespace Robust.Shared.Utility;

public static class TypeAbbreviation
{
	private struct Abbreviation
	{
		public string Long { get; set; }

		public string Short { get; set; }

		public Abbreviation[] SubAbbreviations { get; set; }
	}

	private static readonly Abbreviation[] _abbreviations;

	static TypeAbbreviation()
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Expected O, but got Unknown
		using Stream stream = typeof(TypeAbbreviation).Assembly.GetManifestResourceStream("Robust.Shared.Utility.TypeAbbreviations.yaml");
		using StreamReader streamReader = new StreamReader(stream, EncodingHelpers.UTF8);
		YamlStream val = new YamlStream();
		val.Load((TextReader)streamReader);
		_abbreviations = ParseAbbreviations((YamlSequenceNode)val.Documents[0].RootNode);
	}

	public static string Abbreviate(Type type)
	{
		if (type.FullName == null)
		{
			return "<unnamed type>";
		}
		StringBuilder stringBuilder = new StringBuilder();
		AbbreviateName(type.FullName.Split('`')[0].AsSpan(), _abbreviations, stringBuilder);
		Type[] genericArguments = type.GetGenericArguments();
		if (genericArguments.Length != 0)
		{
			stringBuilder.Append("`").Append(genericArguments.Length).Append("[");
			Type[] array = genericArguments;
			for (int i = 0; i < array.Length; i++)
			{
				AbbreviateName(array[i].FullName.AsSpan(), _abbreviations, stringBuilder);
			}
			stringBuilder.Append("]");
		}
		return stringBuilder.ToString();
	}

	public static string Abbreviate(string name)
	{
		StringBuilder stringBuilder = new StringBuilder();
		AbbreviateName(name.AsSpan(), _abbreviations, stringBuilder);
		return stringBuilder.ToString();
	}

	private static void AbbreviateName(ReadOnlySpan<char> name, Abbreviation[] abbreviations, StringBuilder output)
	{
		for (int i = 0; i < abbreviations.Length; i++)
		{
			Abbreviation abbreviation = abbreviations[i];
			if (name.StartsWith(abbreviation.Long.AsSpan()))
			{
				output.Append(abbreviation.Short);
				int length = abbreviation.Long.Length;
				name = name.Slice(length, name.Length - length);
				if (abbreviation.SubAbbreviations.Length == 0)
				{
					break;
				}
				AbbreviateName(name, abbreviation.SubAbbreviations, output);
				return;
			}
		}
		output.Append(name);
	}

	private static Abbreviation[] ParseAbbreviations(YamlSequenceNode sequence)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Expected O, but got Unknown
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Expected O, but got Unknown
		//IL_006d: Expected O, but got Unknown
		Abbreviation[] array = new Abbreviation[sequence.Children.Count];
		for (int i = 0; i < array.Length; i++)
		{
			YamlMappingNode val = (YamlMappingNode)((YamlNode)sequence)[i];
			string text = YamlHelpers.GetNode(val, "long").AsString() + ".";
			string text2 = YamlHelpers.GetNode(val, "short").AsString() + ".";
			Abbreviation[] subAbbreviations = Array.Empty<Abbreviation>();
			if (YamlHelpers.TryGetNode<YamlSequenceNode>(val, "sub", out YamlSequenceNode returnNode))
			{
				subAbbreviations = ParseAbbreviations(returnNode);
			}
			Abbreviation abbreviation = new Abbreviation
			{
				Long = text,
				Short = text2,
				SubAbbreviations = subAbbreviations
			};
			array[i] = abbreviation;
		}
		return array;
	}
}
