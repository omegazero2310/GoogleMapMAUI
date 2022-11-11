namespace MAUIDotNet7;

public partial class MainPage : ContentPage
{
	int count = 0;

	public MainPage()
	{
		InitializeComponent();
	}
    private void Button_Clicked(object sender, EventArgs e)
    {
        var polyline = new Polyline();
        polyline.Positions.Add(new Position(40.77d, -73.93d));
        polyline.Positions.Add(new Position(40.81d, -73.91d));
        polyline.Positions.Add(new Position(40.83d, -73.87d));

        polyline.IsClickable = true;
        polyline.StrokeColor = Color.FromRgb(0, 0, 255);
        polyline.StrokeWidth = 5f;
        polyline.Tag = "POLYLINE";

        map.Polylines.Add(polyline);
        map.MoveCamera(new CameraUpdate(new Position(40.77d, -73.93d)));
    }
}

