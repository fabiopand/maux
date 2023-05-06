using Maux.Sample.Pages;

namespace Maux.Sample;

public partial class App
{
    public App(IServiceProvider serviceProvider)
    {
        InitializeComponent();
        MainPage = new InitializationPage(serviceProvider);
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        var window = base.CreateWindow(activationState);

#if WINDOWS
        window.Width = 600;
        window.Height = 800;
#endif

        return window;
    }
}