using Maux.Sample.ViewModels;

namespace Maux.Sample.Pages;

public partial class AgendaPage
{
    public AgendaPage(IMauxApplication application, AgendaPageModel pageModel) : base(application, pageModel)
    {
        InitializeComponent();
    }
}