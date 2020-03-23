using System.Threading.Tasks;
using Xamarin.Forms;

namespace Core
{
    public interface IDialogService
    {
        Task<bool> Display(string title, string message, string ok, string cancel);
        Task Display(string title, string message, string cancel);
        Task Display(string message, string cancel);
    }

    public class DialogService : IDialogService
    {
        [Xamarin.Forms.Internals.Preserve]
        public DialogService() {

        }

        public async Task<bool> Display(string title, string message, string ok, string cancel) =>
            await Application.Current.MainPage.DisplayAlert(title, message, ok, cancel);

        public async Task Display(string title, string message, string cancel) =>
            await Application.Current.MainPage.DisplayAlert(title, message, cancel);

        public async Task Display(string message, string cancel)
            => await Display(string.Empty, message, cancel);
    }
}
