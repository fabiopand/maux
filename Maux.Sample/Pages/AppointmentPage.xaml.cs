using Maux.Sample.ViewModels;

namespace Maux.Sample.Pages;

public partial class AppointmentPage
{
    public AppointmentPage(IMauxApplication application, AppointmentPageModel viewModel) : base(application, viewModel)
    {
        InitializeComponent();
    }
}