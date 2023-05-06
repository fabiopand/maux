using Maux.Sample.ViewModels;

namespace Maux.Sample.Pages;

public partial class AboutPage
{
    public AboutPage(IMauxApplication application, AboutPageModel model) : base(application, model)
    {
        InitializeComponent();
    }
}