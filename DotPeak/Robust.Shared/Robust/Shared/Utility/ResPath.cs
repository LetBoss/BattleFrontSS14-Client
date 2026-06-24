// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Utility.ResPath
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Serialization;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

#nullable enable
namespace Robust.Shared.Utility;

[NetSerializable]
[Serializable]
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

  public ResPath()
    : this("")
  {
  }

  public static bool IsValidPath(string path) => !path.Contains('\\');

  public static bool IsValidFilename([NotNullWhen(true)] string? filename)
  {
    return !string.IsNullOrEmpty(filename) && ResPath.IsValidPath(filename) && !filename.Contains('/') && filename != "." && filename != "..";
  }

  public bool IsSelf => this.CanonPath == ResPath.Self.CanonPath;

  [JsonIgnore]
  public ResPath Directory
  {
    get
    {
      if (this.IsSelf)
        return ResPath.Self;
      int num;
      if (this.CanonPath.Length > 1)
      {
        string canonPath = this.CanonPath;
        if (canonPath[canonPath.Length - 1] == '/')
        {
          num = this.CanonPath.LastIndexOf('/', this.CanonPath.Length - 2);
          goto label_6;
        }
      }
      num = this.CanonPath.LastIndexOf('/');
label_6:
      int length = num;
      ResPath directory;
      switch (length)
      {
        case -1:
          directory = ResPath.Self;
          break;
        case 0:
          directory = new ResPath(this.CanonPath.Substring(0, 1));
          break;
        default:
          directory = new ResPath(this.CanonPath.Substring(0, length));
          break;
      }
      return directory;
    }
  }

  public string Extension
  {
    get
    {
      string filename = this.Filename;
      int num = filename.LastIndexOf('.') + 1;
      if (num <= 1)
        return string.Empty;
      string str = filename;
      int startIndex = num;
      return str.Substring(startIndex, str.Length - startIndex);
    }
  }

  public string FilenameWithoutExtension
  {
    get
    {
      string filename = this.Filename;
      int length = filename.LastIndexOf('.');
      return length > 0 ? filename.Substring(0, length) : filename;
    }
  }

  public string Filename
  {
    get
    {
      string canonPath1 = this.CanonPath;
      if (canonPath1 == "." || canonPath1 != null && canonPath1.Length == 0)
        return ".";
      int num = this.CanonPath.LastIndexOf('/', this.CanonPath.Length - 2) + 1;
      string canonPath2 = this.CanonPath;
      if (canonPath2[canonPath2.Length - 1] != '/')
      {
        string canonPath3 = this.CanonPath;
        int startIndex = num;
        return canonPath3.Substring(startIndex, canonPath3.Length - startIndex);
      }
      string canonPath4 = this.CanonPath;
      int startIndex1 = num;
      return canonPath4.Substring(startIndex1, canonPath4.Length - 1 - startIndex1);
    }
  }

  public override string ToString() => this.CanonPath;

  public bool Equals(ResPath other) => this.CanonPath == other.CanonPath;

  public override bool Equals(object? obj) => obj is ResPath other && this.Equals(other);

  public override int GetHashCode() => this.CanonPath.GetHashCode();

  public static bool operator ==(ResPath left, ResPath right) => left.Equals(right);

  public static bool operator !=(ResPath left, ResPath right) => !left.Equals(right);

  public static ResPath operator /(ResPath left, ResPath right)
  {
    if (right.IsRooted)
      return right;
    if (right.IsSelf)
      return left;
    if (left == ResPath.Root)
      return new ResPath("/" + right.CanonPath);
    return left.CanonPath.EndsWith('/') ? new ResPath(left.CanonPath + right.CanonPath) : new ResPath($"{left.CanonPath}/{right.CanonPath}");
  }

  public static ResPath operator /(ResPath left, string right) => left / new ResPath(right);

  public ResPath WithExtension(string newExtension)
  {
    if (string.IsNullOrEmpty(newExtension))
      throw new ArgumentException("New file name cannot be null or empty.");
    if (newExtension.Contains('/'))
      throw new ArgumentException("New file name cannot contain the separator.");
    return this.WithName($"{this.FilenameWithoutExtension}.{newExtension}");
  }

  public ResPath WithName(string name)
  {
    if (string.IsNullOrEmpty(name))
      throw new ArgumentException("New file name cannot be null or empty.");
    if (name.Contains('/'))
      throw new ArgumentException("New file name cannot contain the separator.");
    return !(name == ".") ? new ResPath($"{this.Directory.ToString()}/{name}") : throw new ArgumentException("New file name cannot be '.'");
  }

  public bool IsRooted => this.CanonPath.Length > 0 && this.CanonPath[0] == '/';

  public bool IsRelative => !this.IsRooted;

  public ResPath RelativeTo(ResPath basePath)
  {
    ResPath? relative;
    if (this.TryRelativeTo(basePath, out relative))
      return relative.Value;
    throw new ArgumentException($"{this.CanonPath} does not start with '{basePath}'.");
  }

  public bool TryRelativeTo(ResPath basePath, [NotNullWhen(true)] out ResPath? relative)
  {
    if (this == basePath)
    {
      relative = new ResPath?(ResPath.Self);
      return true;
    }
    if (basePath == ResPath.Self && this.IsRelative)
    {
      relative = new ResPath?(this);
      return true;
    }
    if (this.CanonPath.StartsWith(basePath.CanonPath))
    {
      string canonPath1 = this.CanonPath;
      int length = basePath.CanonPath.Length;
      string canonPath2 = canonPath1.Substring(length, canonPath1.Length - length).Trim('/');
      relative = new ResPath?(canonPath2 == "" ? ResPath.Self : new ResPath(canonPath2));
      return true;
    }
    relative = new ResPath?();
    return false;
  }

  public ResPath ToRootedPath() => !this.IsRooted ? new ResPath("/" + this.CanonPath) : this;

  public ResPath ToRelativePath()
  {
    if (this.IsRelative)
      return this;
    if (this == ResPath.Root)
      return ResPath.Self;
    ResPath resPath;
    ref ResPath local = ref resPath;
    string canonPath1 = this.CanonPath;
    string canonPath2 = canonPath1.Substring(1, canonPath1.Length - 1);
    local = new ResPath(canonPath2);
    return !resPath.IsRelative ? resPath.ToRelativePath() : resPath;
  }

  public string ToRelativeSystemPath() => this.ToRelativePath().ChangeSeparator('\\');

  public static ResPath FromRelativeSystemPath(string path, char newSeparator = '\\')
  {
    return new ResPath(path.Replace(newSeparator, '/'));
  }

  public string ChangeSeparator(string newSeparator)
  {
    if (newSeparator.Length != 1)
      throw new InvalidOperationException("new separator must be a single character.");
    return this.ChangeSeparator(newSeparator[0]);
  }

  public string ChangeSeparator(char newSeparator)
  {
    if (newSeparator == char.MinValue || newSeparator == '.')
      throw new ArgumentException("New separator can't be `.` or `NULL`");
    return this.CanonPath.Replace('/', newSeparator);
  }
}
