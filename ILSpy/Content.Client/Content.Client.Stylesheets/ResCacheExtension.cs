using System;
using Content.Client.Resources;
using Robust.Client.Graphics;
using Robust.Client.ResourceManagement;

namespace Content.Client.Stylesheets;

public static class ResCacheExtension
{
	public static Font NotoStack(this IResourceCache resCache, string variation = "Regular", int size = 10, bool display = false)
	{
		string value = (display ? "Display" : "");
		string text = (variation.StartsWith("Bold", StringComparison.Ordinal) ? "Bold" : "Regular");
		return resCache.GetFont(new string[3]
		{
			$"/Fonts/NotoSans{value}/NotoSans{value}-{variation}.ttf",
			"/Fonts/NotoSans/NotoSansSymbols-" + text + ".ttf",
			"/Fonts/NotoSans/NotoSansSymbols2-Regular.ttf"
		}, size);
	}
}
