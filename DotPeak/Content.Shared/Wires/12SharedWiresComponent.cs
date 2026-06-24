// Decompiled with JetBrains decompiler
// Type: Content.Shared.Wires.HackingWiresExt
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Localization;
using Robust.Shared.Maths;
using System;

#nullable enable
namespace Content.Shared.Wires;

public static class HackingWiresExt
{
  public static string Name(this WireColor color)
  {
    string messageId;
    switch (color)
    {
      case WireColor.Red:
        messageId = "Red";
        break;
      case WireColor.Blue:
        messageId = "Blue";
        break;
      case WireColor.Green:
        messageId = "Green";
        break;
      case WireColor.Orange:
        messageId = "Orange";
        break;
      case WireColor.Brown:
        messageId = "Brown";
        break;
      case WireColor.Gold:
        messageId = "Gold";
        break;
      case WireColor.Gray:
        messageId = "Gray";
        break;
      case WireColor.Cyan:
        messageId = "Cyan";
        break;
      case WireColor.Navy:
        messageId = "Navy";
        break;
      case WireColor.Purple:
        messageId = "Purple";
        break;
      case WireColor.Pink:
        messageId = "Pink";
        break;
      case WireColor.Fuchsia:
        messageId = "Fuchsia";
        break;
      default:
        throw new InvalidOperationException();
    }
    return Loc.GetString(messageId);
  }

  public static Color ColorValue(this WireColor color)
  {
    switch (color)
    {
      case WireColor.Red:
        return Color.Red;
      case WireColor.Blue:
        return Color.Blue;
      case WireColor.Green:
        return Color.LimeGreen;
      case WireColor.Orange:
        return Color.Orange;
      case WireColor.Brown:
        return Color.Brown;
      case WireColor.Gold:
        return Color.Gold;
      case WireColor.Gray:
        return Color.Gray;
      case WireColor.Cyan:
        return Color.Cyan;
      case WireColor.Navy:
        return Color.Navy;
      case WireColor.Purple:
        return Color.Purple;
      case WireColor.Pink:
        return Color.Pink;
      case WireColor.Fuchsia:
        return Color.Fuchsia;
      default:
        throw new InvalidOperationException();
    }
  }

  public static string Name(this WireLetter letter)
  {
    string messageId;
    switch (letter)
    {
      case WireLetter.α:
        messageId = "Alpha";
        break;
      case WireLetter.β:
        messageId = "Beta";
        break;
      case WireLetter.γ:
        messageId = "Gamma";
        break;
      case WireLetter.δ:
        messageId = "Delta";
        break;
      case WireLetter.ε:
        messageId = "Epsilon";
        break;
      case WireLetter.ζ:
        messageId = "Zeta";
        break;
      case WireLetter.η:
        messageId = "Eta";
        break;
      case WireLetter.θ:
        messageId = "Theta";
        break;
      case WireLetter.ι:
        messageId = "Iota";
        break;
      case WireLetter.κ:
        messageId = "Kappa";
        break;
      case WireLetter.λ:
        messageId = "Lambda";
        break;
      case WireLetter.μ:
        messageId = "Mu";
        break;
      case WireLetter.ν:
        messageId = "Nu";
        break;
      case WireLetter.ξ:
        messageId = "Xi";
        break;
      case WireLetter.ο:
        messageId = "Omicron";
        break;
      case WireLetter.π:
        messageId = "Pi";
        break;
      case WireLetter.ρ:
        messageId = "Rho";
        break;
      case WireLetter.σ:
        messageId = "Sigma";
        break;
      case WireLetter.τ:
        messageId = "Tau";
        break;
      case WireLetter.υ:
        messageId = "Upsilon";
        break;
      case WireLetter.φ:
        messageId = "Phi";
        break;
      case WireLetter.χ:
        messageId = "Chi";
        break;
      case WireLetter.ψ:
        messageId = "Psi";
        break;
      case WireLetter.ω:
        messageId = "Omega";
        break;
      default:
        throw new InvalidOperationException();
    }
    return Loc.GetString(messageId);
  }

  public static char Letter(this WireLetter letter)
  {
    switch (letter)
    {
      case WireLetter.α:
        return 'α';
      case WireLetter.β:
        return 'β';
      case WireLetter.γ:
        return 'γ';
      case WireLetter.δ:
        return 'δ';
      case WireLetter.ε:
        return 'ε';
      case WireLetter.ζ:
        return 'ζ';
      case WireLetter.η:
        return 'η';
      case WireLetter.θ:
        return 'θ';
      case WireLetter.ι:
        return 'ι';
      case WireLetter.κ:
        return 'κ';
      case WireLetter.λ:
        return 'λ';
      case WireLetter.μ:
        return 'μ';
      case WireLetter.ν:
        return 'ν';
      case WireLetter.ξ:
        return 'ξ';
      case WireLetter.ο:
        return 'ο';
      case WireLetter.π:
        return 'π';
      case WireLetter.ρ:
        return 'ρ';
      case WireLetter.σ:
        return 'σ';
      case WireLetter.τ:
        return 'τ';
      case WireLetter.υ:
        return 'υ';
      case WireLetter.φ:
        return 'φ';
      case WireLetter.χ:
        return 'χ';
      case WireLetter.ψ:
        return 'ψ';
      case WireLetter.ω:
        return 'ω';
      default:
        throw new InvalidOperationException();
    }
  }
}
