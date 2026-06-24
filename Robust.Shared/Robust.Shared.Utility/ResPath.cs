using System;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using Robust.Shared.Serialization;

namespace Robust.Shared.Utility;

[Serializable]
[NetSerializable]
public readonly struct ResPath(string canonPath) : IEquatable<ResPath>
{
	public const char SystemSeparator = '\\';

	public const string SystemSeparatorStr = "\\";

	public const char Separator = '/';

	public const string SeparatorStr = "/";

	public static readonly ResPath Self = new ResPath(".");

	public static readonly ResPath Root = new ResPath("/");

	public static readonly ResPath Empty = new ResPath("");

	public readonly string CanonPath = canonPath;

	public bool IsSelf => CanonPath == Self.CanonPath;

	[JsonIgnore]
	public ResPath Directory
	{
		get
		{
			if (IsSelf)
			{
				return Self;
			}
			int num;
			if (CanonPath.Length > 1)
			{
				string canonPath = CanonPath;
				if (canonPath[canonPath.Length - 1] == '/')
				{
					num = CanonPath.LastIndexOf('/', CanonPath.Length - 2);
					goto IL_005c;
				}
			}
			num = CanonPath.LastIndexOf('/');
			goto IL_005c;
			IL_005c:
			int num2 = num;
			return num2 switch
			{
				-1 => Self, 
				0 => new ResPath(CanonPath.Substring(0, 1)), 
				_ => new ResPath(CanonPath.Substring(0, num2)), 
			};
		}
	}

	public string Extension
	{
		get
		{
			string filename = Filename;
			int num = filename.LastIndexOf('.') + 1;
			if (num > 1)
			{
				string text = filename;
				int num2 = num;
				return text.Substring(num2, text.Length - num2);
			}
			return string.Empty;
		}
	}

	public string FilenameWithoutExtension
	{
		get
		{
			string filename = Filename;
			int num = filename.LastIndexOf('.');
			if (num > 0)
			{
				return filename.Substring(0, num);
			}
			return filename;
		}
	}

	public string Filename
	{
		get
		{
			string canonPath = CanonPath;
			if ((canonPath == "." || (canonPath != null && canonPath.Length == 0)) ? true : false)
			{
				return ".";
			}
			int num = CanonPath.LastIndexOf('/', CanonPath.Length - 2) + 1;
			string canonPath2 = CanonPath;
			int num2;
			if (canonPath2[canonPath2.Length - 1] != '/')
			{
				canonPath = CanonPath;
				num2 = num;
				return canonPath.Substring(num2, canonPath.Length - num2);
			}
			canonPath = CanonPath;
			num2 = num;
			return canonPath.Substring(num2, canonPath.Length - 1 - num2);
		}
	}

	public bool IsRooted
	{
		get
		{
			if (CanonPath.Length > 0)
			{
				return CanonPath[0] == '/';
			}
			return false;
		}
	}

	public bool IsRelative => !IsRooted;

	public ResPath()
		: this("")
	{
	}

	public static bool IsValidPath(string path)
	{
		return !path.Contains('\\');
	}

	public static bool IsValidFilename([NotNullWhen(true)] string? filename)
	{
		if (!string.IsNullOrEmpty(filename) && IsValidPath(filename) && !filename.Contains('/') && filename != ".")
		{
			return filename != "..";
		}
		return false;
	}

	public override string ToString()
	{
		return CanonPath;
	}

	public bool Equals(ResPath other)
	{
		return CanonPath == other.CanonPath;
	}

	public override bool Equals(object? obj)
	{
		if (obj is ResPath other)
		{
			return Equals(other);
		}
		return false;
	}

	public override int GetHashCode()
	{
		return CanonPath.GetHashCode();
	}

	public static bool operator ==(ResPath left, ResPath right)
	{
		return left.Equals(right);
	}

	public static bool operator !=(ResPath left, ResPath right)
	{
		return !left.Equals(right);
	}

	public static ResPath operator /(ResPath left, ResPath right)
	{
		if (right.IsRooted)
		{
			return right;
		}
		if (right.IsSelf)
		{
			return left;
		}
		if (left == Root)
		{
			return new ResPath("/" + right.CanonPath);
		}
		if (left.CanonPath.EndsWith('/'))
		{
			return new ResPath(left.CanonPath + right.CanonPath);
		}
		return new ResPath(left.CanonPath + "/" + right.CanonPath);
	}

	public static ResPath operator /(ResPath left, string right)
	{
		return left / new ResPath(right);
	}

	public ResPath WithExtension(string newExtension)
	{
		if (string.IsNullOrEmpty(newExtension))
		{
			throw new ArgumentException("New file name cannot be null or empty.");
		}
		if (newExtension.Contains('/'))
		{
			throw new ArgumentException("New file name cannot contain the separator.");
		}
		return WithName(FilenameWithoutExtension + "." + newExtension);
	}

	public ResPath WithName(string name)
	{
		if (string.IsNullOrEmpty(name))
		{
			throw new ArgumentException("New file name cannot be null or empty.");
		}
		if (name.Contains('/'))
		{
			throw new ArgumentException("New file name cannot contain the separator.");
		}
		if (name == ".")
		{
			throw new ArgumentException("New file name cannot be '.'");
		}
		return new ResPath(Directory.ToString() + "/" + name);
	}

	public ResPath RelativeTo(ResPath basePath)
	{
		if (TryRelativeTo(basePath, out var relative))
		{
			return relative.Value;
		}
		throw new ArgumentException($"{CanonPath} does not start with '{basePath}'.");
	}

	public bool TryRelativeTo(ResPath basePath, [NotNullWhen(true)] out ResPath? relative)
	{
		if (this == basePath)
		{
			relative = Self;
			return true;
		}
		if (basePath == Self && IsRelative)
		{
			relative = this;
			return true;
		}
		if (CanonPath.StartsWith(basePath.CanonPath))
		{
			string canonPath = CanonPath;
			int length = basePath.CanonPath.Length;
			string text = canonPath.Substring(length, canonPath.Length - length).Trim('/');
			relative = ((text == "") ? Self : new ResPath(text));
			return true;
		}
		relative = null;
		return false;
	}

	public ResPath ToRootedPath()
	{
		if (!IsRooted)
		{
			return new ResPath("/" + CanonPath);
		}
		return this;
	}

	public ResPath ToRelativePath()
	{
		if (IsRelative)
		{
			return this;
		}
		if (this == Root)
		{
			return Self;
		}
		string canonPath = CanonPath;
		ResPath result = new ResPath(canonPath.Substring(1, canonPath.Length - 1));
		if (!result.IsRelative)
		{
			return result.ToRelativePath();
		}
		return result;
	}

	public string ToRelativeSystemPath()
	{
		return ToRelativePath().ChangeSeparator('\\');
	}

	public static ResPath FromRelativeSystemPath(string path, char newSeparator = '\\')
	{
		return new ResPath(path.Replace(newSeparator, '/'));
	}

	public string ChangeSeparator(string newSeparator)
	{
		if (newSeparator.Length != 1)
		{
			throw new InvalidOperationException("new separator must be a single character.");
		}
		return ChangeSeparator(newSeparator[0]);
	}

	public string ChangeSeparator(char newSeparator)
	{
		if ((newSeparator == '\0' || newSeparator == '.') ? true : false)
		{
			throw new ArgumentException("New separator can't be `.` or `NULL`");
		}
		return CanonPath.Replace('/', newSeparator);
	}
}
