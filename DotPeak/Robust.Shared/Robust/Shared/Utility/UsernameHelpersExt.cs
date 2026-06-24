// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Utility.UsernameHelpersExt
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.AuthLib;

#nullable enable
namespace Robust.Shared.Utility;

public static class UsernameHelpersExt
{
  public static string ToText(this UsernameHelpers.UsernameInvalidReason reason)
  {
    string text;
    switch (reason)
    {
      case UsernameHelpers.UsernameInvalidReason.Valid:
        text = "Username is... valid?";
        break;
      case UsernameHelpers.UsernameInvalidReason.Empty:
        text = "Username can't be empty.";
        break;
      case UsernameHelpers.UsernameInvalidReason.TooLong:
        text = "Username is too long.";
        break;
      case UsernameHelpers.UsernameInvalidReason.InvalidCharacter:
        text = "Contains invalid characters. Only use A-Z, 0-9 and underscores.";
        break;
      default:
        text = "Unknown reason";
        break;
    }
    return text;
  }
}
