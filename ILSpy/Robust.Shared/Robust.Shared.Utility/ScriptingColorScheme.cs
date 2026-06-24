using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using Robust.Shared.Maths;

namespace Robust.Shared.Utility;

public static class ScriptingColorScheme
{
	public const string ClassName = "class name";

	public const string Comment = "comment";

	public const string EnumName = "enum name";

	public const string FieldName = "field name";

	public const string InterfaceName = "interface name";

	public const string Keyword = "keyword";

	public const string MethodName = "method name";

	public const string NamespaceName = "namespace name";

	public const string NumericLiteral = "number";

	public const string PropertyName = "property name";

	public const string StaticSymbol = "static symbol";

	public const string StringLiteral = "string";

	public const string StructName = "struct name";

	public const string Default = "default";

	public static readonly FrozenDictionary<string, Color> ColorScheme = new Dictionary<string, Color>
	{
		{
			"class name",
			Color.FromHex("#4EC9B0".AsSpan(), (Color?)null)
		},
		{
			"comment",
			Color.FromHex("#57A64A".AsSpan(), (Color?)null)
		},
		{
			"enum name",
			Color.FromHex("#B8D7A3".AsSpan(), (Color?)null)
		},
		{
			"field name",
			Color.FromHex("#C86E11".AsSpan(), (Color?)null)
		},
		{
			"interface name",
			Color.FromHex("#B8D7A3".AsSpan(), (Color?)null)
		},
		{
			"keyword",
			Color.FromHex("#569CD6".AsSpan(), (Color?)null)
		},
		{
			"method name",
			Color.FromHex("#11A3C8".AsSpan(), (Color?)null)
		},
		{
			"namespace name",
			Color.FromHex("#C8A611".AsSpan(), (Color?)null)
		},
		{
			"number",
			Color.FromHex("#b5cea8".AsSpan(), (Color?)null)
		},
		{
			"property name",
			Color.FromHex("#11C89D".AsSpan(), (Color?)null)
		},
		{
			"static symbol",
			Color.FromHex("#4EC9B0".AsSpan(), (Color?)null)
		},
		{
			"string",
			Color.FromHex("#D69D85".AsSpan(), (Color?)null)
		},
		{
			"struct name",
			Color.FromHex("#4EC9B0".AsSpan(), (Color?)null)
		},
		{
			"default",
			Color.FromHex("#D4D4D4".AsSpan(), (Color?)null)
		}
	}.ToFrozenDictionary();
}
