using System;
using Robust.Shared.Localization;
using Robust.Shared.Maths;

namespace Content.Shared.Wires;

public static class HackingWiresExt
{
	public static string Name(this WireColor color)
	{
		return Loc.GetString(color switch
		{
			WireColor.Red => "Red", 
			WireColor.Blue => "Blue", 
			WireColor.Green => "Green", 
			WireColor.Orange => "Orange", 
			WireColor.Brown => "Brown", 
			WireColor.Gold => "Gold", 
			WireColor.Gray => "Gray", 
			WireColor.Cyan => "Cyan", 
			WireColor.Navy => "Navy", 
			WireColor.Purple => "Purple", 
			WireColor.Pink => "Pink", 
			WireColor.Fuchsia => "Fuchsia", 
			_ => throw new InvalidOperationException(), 
		});
	}

	public static Color ColorValue(this WireColor color)
	{
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		return (Color)(color switch
		{
			WireColor.Red => Color.Red, 
			WireColor.Blue => Color.Blue, 
			WireColor.Green => Color.LimeGreen, 
			WireColor.Orange => Color.Orange, 
			WireColor.Brown => Color.Brown, 
			WireColor.Gold => Color.Gold, 
			WireColor.Gray => Color.Gray, 
			WireColor.Cyan => Color.Cyan, 
			WireColor.Navy => Color.Navy, 
			WireColor.Purple => Color.Purple, 
			WireColor.Pink => Color.Pink, 
			WireColor.Fuchsia => Color.Fuchsia, 
			_ => throw new InvalidOperationException(), 
		});
	}

	public static string Name(this WireLetter letter)
	{
		return Loc.GetString(letter switch
		{
			WireLetter.α => "Alpha", 
			WireLetter.β => "Beta", 
			WireLetter.γ => "Gamma", 
			WireLetter.δ => "Delta", 
			WireLetter.ε => "Epsilon", 
			WireLetter.ζ => "Zeta", 
			WireLetter.η => "Eta", 
			WireLetter.θ => "Theta", 
			WireLetter.ι => "Iota", 
			WireLetter.κ => "Kappa", 
			WireLetter.λ => "Lambda", 
			WireLetter.μ => "Mu", 
			WireLetter.ν => "Nu", 
			WireLetter.ξ => "Xi", 
			WireLetter.ο => "Omicron", 
			WireLetter.π => "Pi", 
			WireLetter.ρ => "Rho", 
			WireLetter.σ => "Sigma", 
			WireLetter.τ => "Tau", 
			WireLetter.υ => "Upsilon", 
			WireLetter.φ => "Phi", 
			WireLetter.χ => "Chi", 
			WireLetter.ψ => "Psi", 
			WireLetter.ω => "Omega", 
			_ => throw new InvalidOperationException(), 
		});
	}

	public static char Letter(this WireLetter letter)
	{
		return letter switch
		{
			WireLetter.α => 'α', 
			WireLetter.β => 'β', 
			WireLetter.γ => 'γ', 
			WireLetter.δ => 'δ', 
			WireLetter.ε => 'ε', 
			WireLetter.ζ => 'ζ', 
			WireLetter.η => 'η', 
			WireLetter.θ => 'θ', 
			WireLetter.ι => 'ι', 
			WireLetter.κ => 'κ', 
			WireLetter.λ => 'λ', 
			WireLetter.μ => 'μ', 
			WireLetter.ν => 'ν', 
			WireLetter.ξ => 'ξ', 
			WireLetter.ο => 'ο', 
			WireLetter.π => 'π', 
			WireLetter.ρ => 'ρ', 
			WireLetter.σ => 'σ', 
			WireLetter.τ => 'τ', 
			WireLetter.υ => 'υ', 
			WireLetter.φ => 'φ', 
			WireLetter.χ => 'χ', 
			WireLetter.ψ => 'ψ', 
			WireLetter.ω => 'ω', 
			_ => throw new InvalidOperationException(), 
		};
	}
}
