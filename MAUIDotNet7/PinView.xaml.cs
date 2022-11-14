namespace MAUIDotNet7;

public partial class PinView : StackLayout
{
    public PinView()
    {
        InitializeComponent();
    }
    private string _display;

    public PinView(string display)
    {
        InitializeComponent();
        _display = display;
        BindingContext = this;
    }

    public string Display
    {
        get { return _display; }
    }
}