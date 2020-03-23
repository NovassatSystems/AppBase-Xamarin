using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Core
{
    public static class AnimationExtensions
    {
        public static Task<bool> ChangeBackgroundColorTo(this Button self, Color newColor, uint length = 40, Easing easing = null)
        {
            Task<bool> ret = new Task<bool>(() => false);

            if (self.AnimationIsRunning(nameof(ChangeBackgroundColorTo)))
                self.AbortAnimation(nameof(ChangeBackgroundColorTo));
            Color fromColor = self.BackgroundColor;

            try
            {
                Func<double, Color> transform = (t) =>
                  Color.FromRgba(fromColor.R + t * (newColor.R - fromColor.R),
                                 fromColor.G + t * (newColor.G - fromColor.G),
                                 fromColor.B + t * (newColor.B - fromColor.B),
                                 fromColor.A + t * (newColor.A - fromColor.A));

                ret = TransmuteColorAnimation(self, nameof(ChangeBackgroundColorTo), transform, length, easing);
            }
            catch
            {
                self.BackgroundColor = fromColor;
            }

            return ret;
        }

        private static Task<bool> TransmuteColorAnimation(Button button, string name, Func<double, Color> transform, uint length, Easing easing)
        {
            easing = easing ?? Easing.Linear;
            var taskCompletionSource = new TaskCompletionSource<bool>();

            button.Animate(name, transform, (color) => { button.BackgroundColor = color; }, 16, length, easing, (v, c) => taskCompletionSource.SetResult(c));
            return taskCompletionSource.Task;
        }
    }
}
