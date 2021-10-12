namespace AspNetIdentity.Sample.Pages.Error
{
    using Duende.IdentityServer.Models;

    public class ViewModel
    {
        public ViewModel()
        {
        }

        public ViewModel(string error) => Error = new ErrorMessage { Error = error };

        public ErrorMessage Error { get; set; }
    }
}