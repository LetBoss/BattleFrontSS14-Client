// Decompiled with JetBrains decompiler
// Type: Content.Client.UserInterface.BoundKeyHelper
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Robust.Client.Input;
using Robust.Shared.Input;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Utility;
using System.Diagnostics.CodeAnalysis;

#nullable enable
namespace Content.Client.UserInterface;

public static class BoundKeyHelper
{
  public static string ShortKeyName(BoundKeyFunction keyFunction)
  {
    string name;
    if (!BoundKeyHelper.TryGetShortKeyName(keyFunction, out name))
      return " ";
    string str;
    return !IoCManager.Resolve<ILocalizationManager>().TryGetString(name, ref str) ? name : str;
  }

  public static bool IsBound(BoundKeyFunction keyFunction)
  {
    return BoundKeyHelper.TryGetShortKeyName(keyFunction, out string _);
  }

  private static string? DefaultShortKeyName(BoundKeyFunction keyFunction)
  {
    if (string.IsNullOrEmpty(keyFunction.FunctionName))
      return (string) null;
    IDependencyCollection instance = IoCManager.Instance;
    IInputManager iinputManager;
    if (instance == null || !instance.TryResolveType<IInputManager>(ref iinputManager))
      return (string) null;
    string str = FormattedMessage.EscapeText(iinputManager.GetKeyFunctionButtonString(keyFunction));
    return str.Length <= 3 ? str : (string) null;
  }

  public static bool TryGetShortKeyName(BoundKeyFunction keyFunction, [NotNullWhen(true)] out string? name)
  {
    if (string.IsNullOrEmpty(keyFunction.FunctionName))
    {
      name = (string) null;
      return false;
    }
    IDependencyCollection instance = IoCManager.Instance;
    IInputManager iinputManager;
    if (instance == null || !instance.TryResolveType<IInputManager>(ref iinputManager))
    {
      name = (string) null;
      return false;
    }
    IKeyBinding ikeyBinding;
    if (iinputManager.TryGetKeyBinding(keyFunction, ref ikeyBinding))
    {
      Keyboard.Key baseKey = ikeyBinding.BaseKey;
      if (ikeyBinding.Mod1 != null || ikeyBinding.Mod2 != null || ikeyBinding.Mod3 != null)
      {
        name = (string) null;
        return false;
      }
      name = (string) null;
      string str;
      switch (baseKey - 1)
      {
        case 0:
          str = "ML";
          break;
        case 1:
          str = "MR";
          break;
        case 2:
          str = "MM";
          break;
        case 3:
          str = "M4";
          break;
        case 4:
          str = "M5";
          break;
        case 5:
          str = "M6";
          break;
        case 6:
          str = "M7";
          break;
        case 7:
          str = "M8";
          break;
        case 8:
          str = "M9";
          break;
        case 35:
          str = "0";
          break;
        case 36:
          str = "1";
          break;
        case 37:
          str = "2";
          break;
        case 38:
          str = "3";
          break;
        case 39:
          str = "4";
          break;
        case 40:
          str = "5";
          break;
        case 41:
          str = "6";
          break;
        case 42:
          str = "7";
          break;
        case 43:
          str = "8";
          break;
        case 44:
          str = "9";
          break;
        case 45:
          str = "0";
          break;
        case 46:
          str = "1";
          break;
        case 47:
          str = "2";
          break;
        case 48 /*0x30*/:
          str = "3";
          break;
        case 49:
          str = "4";
          break;
        case 50:
          str = "5";
          break;
        case 51:
          str = "6";
          break;
        case 52:
          str = "7";
          break;
        case 53:
          str = "8";
          break;
        case 54:
          str = "9";
          break;
        case 55:
          str = "Esc";
          break;
        case 61:
          str = "Men";
          break;
        case 62:
          str = "[";
          break;
        case 63 /*0x3F*/:
          str = "]";
          break;
        case 64 /*0x40*/:
          str = ";";
          break;
        case 65:
          str = ",";
          break;
        case 66:
          str = ".";
          break;
        case 67:
          str = "'";
          break;
        case 68:
          str = "/";
          break;
        case 69:
          str = "\\";
          break;
        case 70:
          str = "~";
          break;
        case 71:
          str = "=";
          break;
        case 72:
          str = "Spc";
          break;
        case 73:
          str = "Ret";
          break;
        case 74:
          str = "Ent";
          break;
        case 75:
          str = "Bks";
          break;
        case 76:
          str = "Tab";
          break;
        case 77:
          str = "PgU";
          break;
        case 78:
          str = "PgD";
          break;
        case 80 /*0x50*/:
          str = "Hom";
          break;
        case 81:
          str = "Ins";
          break;
        case 82:
          str = "Del";
          break;
        case 83:
          str = "-";
          break;
        case 85:
          str = "N-";
          break;
        case 86:
          str = "N/";
          break;
        case 87:
          str = "*";
          break;
        case 88:
          str = "N.";
          break;
        case 89:
          str = "Lft";
          break;
        case 90:
          str = "Rgt";
          break;
        case 92:
          str = "Dwn";
          break;
        case 117:
          str = "||";
          break;
        default:
          str = BoundKeyHelper.DefaultShortKeyName(keyFunction);
          break;
      }
      name = str;
      return name != null;
    }
    name = (string) null;
    return false;
  }
}
