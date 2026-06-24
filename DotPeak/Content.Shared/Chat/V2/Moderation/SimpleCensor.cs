// Decompiled with JetBrains decompiler
// Type: Content.Shared.Chat.V2.Moderation.SimpleCensor
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Unicode;

#nullable enable
namespace Content.Shared.Chat.V2.Moderation;

public sealed class SimpleCensor : IChatCensor
{
  private bool _shouldSanitizeLeetspeak;
  private FrozenDictionary<char, char> _leetspeakReplacements = FrozenDictionary<char, char>.Empty;
  private bool _shouldSanitizeSpecialCharacters;
  private HashSet<char> _specialCharacterReplacements = new HashSet<char>();
  private string[] _censoredWords = Array.Empty<string>();
  private string[] _falsePositives = Array.Empty<string>();
  private string[] _falseNegatives = Array.Empty<string>();
  private UnicodeRange[] _allowedUnicodeRanges = Array.Empty<UnicodeRange>();

  public bool Censor(string input, out string output, char replaceWith = '*')
  {
    output = this.Censor(input, replaceWith);
    return !string.Equals(input, output);
  }

  public string Censor(string input, char replaceWith = '*')
  {
    input = this.SanitizeOutBlockedUnicode(input);
    char[] charArray = input.ToCharArray();
    input = this.SanitizeInput(input);
    List<char> list = input.ToList<char>();
    input = this.CheckProfanity(input, list, this._falseNegatives, replaceWith);
    char[] falsePositives = this.FindFalsePositives(list, replaceWith);
    this.CheckProfanity(input, list, this._censoredWords, replaceWith);
    for (int index = 0; index < falsePositives.Length; ++index)
    {
      if ((int) falsePositives[index] != (int) replaceWith)
        list[index] = falsePositives[index];
    }
    for (int index = 0; index < charArray.Length; ++index)
    {
      if (charArray[index] == ' ')
        list.Insert(index, ' ');
      else if (this._shouldSanitizeSpecialCharacters && this._specialCharacterReplacements.Contains(charArray[index]))
      {
        list.Insert(index, charArray[index]);
      }
      else
      {
        if ((this._shouldSanitizeLeetspeak || this._shouldSanitizeSpecialCharacters) && charArray[index] == '(' && index != charArray.Length - 1 && charArray[index + 1] == ')')
          list.Insert(index + 1, (int) list[index] != (int) replaceWith ? ')' : replaceWith);
        if ((int) list[index] != (int) replaceWith)
          list[index] = charArray[index];
      }
    }
    return string.Concat<char>((IEnumerable<char>) list);
  }

  public SimpleCensor WithSanitizeLeetSpeak()
  {
    this._shouldSanitizeLeetspeak = true;
    return this.BuildCharacterReplacements();
  }

  public SimpleCensor WithSanitizeSpecialCharacters()
  {
    this._shouldSanitizeSpecialCharacters = true;
    return this.BuildCharacterReplacements();
  }

  public SimpleCensor WithRanges(UnicodeRange[] ranges)
  {
    this._allowedUnicodeRanges = ranges;
    return this;
  }

  public SimpleCensor WithCustomDictionary(string[] naughtyWords)
  {
    this._censoredWords = naughtyWords;
    return this;
  }

  public SimpleCensor WithFalsePositives(string[] falsePositives)
  {
    this._falsePositives = falsePositives;
    return this;
  }

  public SimpleCensor WithFalseNegatives(string[] falseNegatives)
  {
    this._falseNegatives = falseNegatives;
    return this;
  }

  public SimpleCensor WithLeetspeakReplacements(Dictionary<char, char> replacements)
  {
    this._leetspeakReplacements = replacements.ToFrozenDictionary<char, char>();
    return this;
  }

  public SimpleCensor WithSpecialCharacterReplacements(Dictionary<char, char> replacements)
  {
    this._leetspeakReplacements = replacements.ToFrozenDictionary<char, char>();
    return this;
  }

  private string CheckProfanity(
    string input,
    List<char> censored,
    string[] words,
    char replaceWith = '*')
  {
    foreach (string word in words)
    {
      int length = word.Length;
      int startIndex1 = 0;
      int startIndex2;
      for (int index1 = input.IndexOf(word, startIndex1, StringComparison.OrdinalIgnoreCase); index1 > -1; index1 = input.IndexOf(word, startIndex2, StringComparison.OrdinalIgnoreCase))
      {
        startIndex2 = index1 + length;
        for (int index2 = 0; index2 < length; ++index2)
          censored[index1 + index2] = replaceWith;
      }
    }
    return input;
  }

  private char[] FindFalsePositives(List<char> chars, char replaceWith = '*')
  {
    string source = string.Concat<char>((IEnumerable<char>) chars);
    char[] array1 = Enumerable.Repeat<char>(replaceWith, source.Length).ToArray<char>();
    char[] array2 = source.ToArray<char>();
    foreach (string falsePositive in this._falsePositives)
    {
      int length = falsePositive.Length;
      int startIndex1 = 0;
      int startIndex2;
      for (int index1 = source.IndexOf(falsePositive, startIndex1, StringComparison.OrdinalIgnoreCase); index1 > -1; index1 = source.IndexOf(falsePositive, startIndex2, StringComparison.OrdinalIgnoreCase))
      {
        startIndex2 = index1 + length;
        for (int index2 = index1; index2 < startIndex2; ++index2)
          array1[index2] = array2[index2];
      }
    }
    return array1;
  }

  private string SanitizeInput(string input)
  {
    if (this._shouldSanitizeLeetspeak || this._shouldSanitizeSpecialCharacters)
      input = input.Replace("()", "o");
    StringBuilder stringBuilder = new StringBuilder();
    foreach (char key in input)
    {
      if (key != ' ' && (!this._shouldSanitizeSpecialCharacters || !this._specialCharacterReplacements.Contains(key)))
      {
        char ch;
        if (this._shouldSanitizeLeetspeak && this._leetspeakReplacements.TryGetValue(key, out ch))
          stringBuilder.Append(ch);
        else
          stringBuilder.Append(key);
      }
    }
    return stringBuilder.ToString();
  }

  private string SanitizeOutBlockedUnicode(string input)
  {
    if (this._allowedUnicodeRanges.Length == 0)
      return input;
    StringBuilder stringBuilder = new StringBuilder();
    foreach (Rune enumerateRune in input.EnumerateRunes())
    {
      foreach (UnicodeRange allowedUnicodeRange in this._allowedUnicodeRanges)
      {
        if (enumerateRune.Value >= allowedUnicodeRange.FirstCodePoint && enumerateRune.Value < allowedUnicodeRange.FirstCodePoint + allowedUnicodeRange.Length)
        {
          stringBuilder.Append((object) enumerateRune);
          break;
        }
      }
    }
    return stringBuilder.ToString();
  }

  private SimpleCensor BuildCharacterReplacements()
  {
    if (this._shouldSanitizeSpecialCharacters)
      this._specialCharacterReplacements = new HashSet<char>()
      {
        '-',
        '_',
        '|',
        '.',
        ',',
        '(',
        ')',
        '<',
        '>',
        '"',
        '`',
        '~',
        '*',
        '&',
        '%',
        '$',
        '#',
        '@',
        '!',
        '?',
        '+'
      };
    if (this._shouldSanitizeLeetspeak)
      this._leetspeakReplacements = new Dictionary<char, char>()
      {
        ['4'] = 'a',
        ['$'] = 's',
        ['!'] = 'i',
        ['+'] = 't',
        ['#'] = 'h',
        ['@'] = 'a',
        ['0'] = 'o',
        ['1'] = 'i',
        ['7'] = 'l',
        ['3'] = 'e',
        ['5'] = 's',
        ['9'] = 'g',
        ['<'] = 'c'
      }.ToFrozenDictionary<char, char>();
    return this;
  }
}
