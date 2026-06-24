using System;
using Robust.Shared.Serialization;
using Robust.Shared.Utility;

namespace Robust.Shared.RichText;

public struct FormattedString : IEquatable<FormattedString>, ISelfSerialize
{
	public static readonly FormattedString Empty = new FormattedString("");

	public readonly string Markup;

	[Obsolete("Do not construct FormattedString directly")]
	public FormattedString()
	{
		throw new NotSupportedException("Do not construct FormattedString directly");
	}

	private FormattedString(string markup)
	{
		Markup = markup;
	}

	public static FormattedString FromMarkup(string markup)
	{
		if (!FormattedMessage.ValidMarkup(markup))
		{
			throw new ArgumentException("Invalid markup string");
		}
		return new FormattedString(markup);
	}

	public static FormattedString FromMarkupPermissive(string markup)
	{
		return (FormattedString)FormattedMessage.FromMarkupPermissive(markup);
	}

	public static FormattedString FromPlainText(string plainText)
	{
		return new FormattedString(FormattedMessage.EscapeText(plainText));
	}

	public static explicit operator FormattedString(FormattedMessage message)
	{
		return new FormattedString(message.ToMarkup());
	}

	public static explicit operator FormattedMessage(FormattedString str)
	{
		return FormattedMessage.FromMarkupOrThrow(str.Markup);
	}

	public static explicit operator string(FormattedString str)
	{
		return str.Markup;
	}

	public readonly bool Equals(FormattedString other)
	{
		return other.Markup == Markup;
	}

	public override readonly bool Equals(object? obj)
	{
		if (obj is FormattedString other)
		{
			return Equals(other);
		}
		return false;
	}

	public override readonly int GetHashCode()
	{
		return Markup.GetHashCode();
	}

	public static bool operator ==(FormattedString left, FormattedString right)
	{
		return left.Equals(right);
	}

	public static bool operator !=(FormattedString left, FormattedString right)
	{
		return !left.Equals(right);
	}

	void ISelfSerialize.Deserialize(string value)
	{
		this = FromMarkup(value);
	}

	readonly string ISelfSerialize.Serialize()
	{
		return Markup;
	}
}
