using Robust.Client.UserInterface.CustomControls;

namespace Content.Client.Viewport;

public static class ViewportExt
{
	public static int GetRenderScale(this IViewportControl viewport)
	{
		if (viewport is ScalingViewport scalingViewport)
		{
			return scalingViewport.CurrentRenderScale;
		}
		return 1;
	}
}
