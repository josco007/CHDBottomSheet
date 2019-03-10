# CHDBottomSheet
This is a component to present a bottom sheet view easily in Xamarin Forms it supports scrollview.

![Alt Text](https://media.giphy.com/media/1gOAKQnF1c5FCqfk0P/giphy.gif)


<table>
<thead>
<tr>
<th>Platform</th>
<th align="center">Version</th>
</tr>
</thead>
<tbody>
<tr>
<td>Xamarin.iOS</td>
<td align="center">Not tested</td>
</tr>
<tr>
<td>Xamarin.Android</td>
<td align="center">API 14+, Not tested on lower</td>
</tr>
<tr>
<td>Windows 10 UWP</td>
<td align="center">Not tested</td>
</tr>
</tbody>
</table>


<h3> Install via nuget</h3>

https://www.nuget.org/packages/CHD.BottomSheet/

<h3>Initialize</h3>

<h5>You must set the root relative layout on the OnAppearingMethod</h5>
    
    protected override void OnAppearing()
        {
            base.OnAppearing();
            CHDBottomSheetBs.SetRelativeLayout(_rootRlt);
        }
       
by default the component has a height of 200 but you can change it in this way:

    CHDBottomSheetBs.DefaultHeight = 200;

<h3>Usage with scroll support</h3>

To use a scrollview inside this component you should add a global variable <b>scrollingByPan</b> and also add to your scroll the Scrolled event:
     
     private bool _scrollingByPan;
     
      private void BottomSheetScroll_Scrolled(object sender, ScrolledEventArgs e)
        {
            if (BottomSheetScroll.ScrollY == 0 && !_scrollingByPan)
            {
                BottomSheetScroll.IsEnabled = false;
                CHDBottomSheetBs.IsResizeEnabled = true;
            }
        }
also you need add <b>OnPanUpdatedEvent</b> and <b>OnPanDirection</b> events to the CHDBottomSheet component:
      
      public HomePage ()
		{
			InitializeComponent ();
            
            CHDBottomSheetBs.DefaultHeight = 200;
            CHDBottomSheetBs.OnPanUpdatedEvent += OnPanUpdated; // add this
            CHDBottomSheetBs.OnPanDirection += OnPanDirection; // add this
            BottomSheetScroll.Scrolled += BottomSheetScroll_Scrolled;

        }
the <b>OnPanUpdatedEvent</b> and <b>OnPanDirection<b> should look like this:

    private void OnPanDirection(BottomSheet.PanDirections panDirection, double totalY)
        {

            switch (panDirection)
            {
                case BottomSheet.PanDirections.Down:
                    if (BottomSheetScroll.ScrollY == 0)
                    {
                        BottomSheetScroll.IsEnabled = false;

                    }
                    if (CHDBottomSheetBs.Mode == BottomSheet.Modes.Full)
                    {
                        //scroll the scroll with the pan captured in CHDBottomSheet component
                        double yWithSpeed = -1 * Math.Max(Math.Min(0, (BottomSheetScroll.ScrollY  + totalY)*-1), -Math.Abs(BottomSheetScroll.Height * -1)); 
                        ScrollWithPan(yWithSpeed);
                    }
                    break;
                case BottomSheet.PanDirections.Up:
                    if (CHDBottomSheetBs.Mode == BottomSheet.Modes.Full)
                    {
                        BottomSheetScroll.IsEnabled = true;
                        CHDBottomSheetBs.IsResizeEnabled = false;

                        //scroll the scroll with the pan captured in CHDBottomSheet component
                        double yWithSpeed =  Math.Max(Math.Min(0, BottomSheetScroll.ScrollY + totalY), -Math.Abs(BottomSheetScroll.Height * -1));
                        ScrollWithPan(yWithSpeed);
                    }
                    else
                    {
                        BottomSheetScroll.IsEnabled = false;
 
                    }
                    
                    break;
            }
        }

        private void ScrollWithPan(double yWithSpeed)
        {
            _scrollingByPan = true;
            BottomSheetScroll.ScrollToAsync(0, BottomSheetScroll.ScrollY + (yWithSpeed * -1), false);
        }

        void OnPanUpdated(object sender, PanUpdatedEventArgs e)
        {

            switch (e.StatusType)
            {
                case GestureStatus.Running:
                    break;

                case GestureStatus.Completed:
                    _scrollingByPan = false;
                    if (BottomSheetScroll.ScrollY == 0)
                    {
                        BottomSheetScroll.IsEnabled = false;
                        CHDBottomSheetBs.IsResizeEnabled = true;
                    }
                    break;
            }
        }



<details><summary><b>XAML expand</b></summary>
<p>

<h5>This component needs a relativelayout as root view, your xaml should looks like this:</h5>

    <?xml version="1.0" encoding="UTF-8"?>
    <ContentPage xmlns="http://xamarin.com/schemas/2014/forms" xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             xmlns:local="clr-namespace:CHD;assembly=CHD.BottomSheet" x:Class="PanGesture.HomePage">
	<ContentPage.Content>


        <StackLayout Spacing="0" Padding="0,100,0,0">


            <RelativeLayout x:Name="_rootRlt" BackgroundColor="Green" VerticalOptions="FillAndExpand">

                <ScrollView RelativeLayout.XConstraint="0"
                                    RelativeLayout.YConstraint="0"
                                    RelativeLayout.WidthConstraint="{ConstraintExpression Type=RelativeToParent, ElementName=SearchControlsGrid,Property=Width, Factor=1, Constant=0 }"
                                    RelativeLayout.HeightConstraint="{ConstraintExpression Type=RelativeToParent, ElementName=SearchControlsGrid, Property=Height,Factor=1, Constant=-200}">
                    <StackLayout Orientation="Vertical">

                        <Label Text="YOUR CONTENT HERE FIRST ITEM"/>
                        <Label Text="YOUR CONTENT HERE"/>
                        <Label Text="YOUR CONTENT HERE"/>
                        <Label Text="YOUR CONTENT HERE"/>
                        <Label Text="YOUR CONTENT HERE"/>
                        <Label Text="YOUR CONTENT HERE LAST ITEM"/>
  
                    </StackLayout>
                </ScrollView>
                
                
                <local:BottomSheet x:Name="CHDBottomSheetBs" 
                                    HorizontalOptions="FillAndExpand" 
                                    VerticalOptions="FillAndExpand"
                                    BackgroundColor="Blue"
                                    RelativeLayout.XConstraint="0"
                                    RelativeLayout.YConstraint="{ConstraintExpression Type=RelativeToParent, ElementName=SearchControlsGrid,Property=Height, Factor=1, Constant=-200 }"
                                    RelativeLayout.WidthConstraint="{ConstraintExpression Type=RelativeToParent, ElementName=SearchControlsGrid,Property=Width, Factor=1, Constant=0 }"
                                    RelativeLayout.HeightConstraint="{ConstraintExpression Type=Constant, ElementName=SearchControlsGrid, Property=Height,Factor=1, Constant=200}">

                    <ScrollView x:Name="BottomSheetScroll" IsEnabled="False">
                        <StackLayout BackgroundColor="Aqua"  >
                            <StackLayout Orientation="Horizontal" HorizontalOptions="Center">
                                <Label Text="0"/>
                                <Image Source="MonoMonkey.jpg" WidthRequest="50" HeightRequest="50"  VerticalOptions="EndAndExpand"/>
                            </StackLayout>
                            <StackLayout Orientation="Horizontal" HorizontalOptions="Center">
                                <Label Text="1"/>
                                <Image Source="MonoMonkey.jpg" WidthRequest="50" HeightRequest="50"  VerticalOptions="EndAndExpand"/>
                            </StackLayout>
                            <StackLayout Orientation="Horizontal" HorizontalOptions="Center">
                                <Label Text="2"/>
                                <Image Source="MonoMonkey.jpg" WidthRequest="50" HeightRequest="50"  VerticalOptions="EndAndExpand"/>
                            </StackLayout>
                            <StackLayout Orientation="Horizontal" HorizontalOptions="Center">
                                <Label Text="3"/>
                                <Image Source="MonoMonkey.jpg" WidthRequest="50" HeightRequest="50"  VerticalOptions="EndAndExpand"/>
                            </StackLayout>
                            <StackLayout Orientation="Horizontal" HorizontalOptions="Center">
                                <Label Text="4"/>
                                <Image Source="MonoMonkey.jpg" WidthRequest="50" HeightRequest="50"  VerticalOptions="EndAndExpand"/>
                            </StackLayout>
                            <StackLayout Orientation="Horizontal" HorizontalOptions="Center">
                                <Label Text="5"/>
                                <Image Source="MonoMonkey.jpg" WidthRequest="50" HeightRequest="50"  VerticalOptions="EndAndExpand"/>
                            </StackLayout>
                        </StackLayout>
                    </ScrollView>

                </local:BottomSheet>
                
            </RelativeLayout>
            

        </StackLayout>
    </ContentPage.Content>
    </ContentPage>

</p>
</details>

   


<details><summary><b>CS expand</b></summary>
<p>

<h5>Your CS code should be like this</h5>
        
    public partial class HomePage : ContentPage
	{

        private bool _scrollingByPan;

        public HomePage ()
		{
			InitializeComponent ();
            
            CHDBottomSheetBs.DefaultHeight = 200;
            CHDBottomSheetBs.OnPanUpdatedEvent += OnPanUpdated;
            //CHDBottomSheetBs.OnModeChanged += OnModeChanged;
            CHDBottomSheetBs.OnPanDirection += OnPanDirection;
            BottomSheetScroll.Scrolled += BottomSheetScroll_Scrolled;

        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            CHDBottomSheetBs.SetRelativeLayout(_rootRlt);
        }

        private void BottomSheetScroll_Scrolled(object sender, ScrolledEventArgs e)
        {
            if (BottomSheetScroll.ScrollY == 0 && !_scrollingByPan)
            {
                BottomSheetScroll.IsEnabled = false;
                CHDBottomSheetBs.IsResizeEnabled = true;
            }
        }

        private void OnPanDirection(BottomSheet.PanDirections panDirection, double totalY)
        {

            switch (panDirection)
            {
                case BottomSheet.PanDirections.Down:
                    if (BottomSheetScroll.ScrollY == 0)
                    {
                        BottomSheetScroll.IsEnabled = false;

                    }
                    if (CHDBottomSheetBs.Mode == BottomSheet.Modes.Full)
                    {
                        //scroll the scroll with the pan captured in CHDBottomSheet component
                        double yWithSpeed = -1 * Math.Max(Math.Min(0, (BottomSheetScroll.ScrollY  + totalY)*-1), -Math.Abs(BottomSheetScroll.Height * -1)); 
                        ScrollWithPan(yWithSpeed);
                    }
                    break;
                case BottomSheet.PanDirections.Up:
                    if (CHDBottomSheetBs.Mode == BottomSheet.Modes.Full)
                    {
                        BottomSheetScroll.IsEnabled = true;
                        CHDBottomSheetBs.IsResizeEnabled = false;

                        //scroll the scroll with the pan captured in CHDBottomSheet component
                        double yWithSpeed =  Math.Max(Math.Min(0, BottomSheetScroll.ScrollY + totalY), -Math.Abs(BottomSheetScroll.Height * -1));
                        ScrollWithPan(yWithSpeed);
                    }
                    else
                    {
                        BottomSheetScroll.IsEnabled = false;
 
                    }
                    
                    break;
            }
        }

        private void ScrollWithPan(double yWithSpeed)
        {
            _scrollingByPan = true;
            BottomSheetScroll.ScrollToAsync(0, BottomSheetScroll.ScrollY + (yWithSpeed * -1), false);
        }

        void OnPanUpdated(object sender, PanUpdatedEventArgs e)
        {

            switch (e.StatusType)
            {
                case GestureStatus.Running:
                    break;

                case GestureStatus.Completed:
                    _scrollingByPan = false;
                    if (BottomSheetScroll.ScrollY == 0)
                    {
                        BottomSheetScroll.IsEnabled = false;
                        CHDBottomSheetBs.IsResizeEnabled = true;
                    }
                    break;
            }
        }

        void OnModeChanged(BottomSheet.Modes mode)
        {
            switch (mode)
            {
                case BottomSheet.Modes.Default:

                    break;
                case BottomSheet.Modes.Full:

                    break;
                case BottomSheet.Modes.Moving:
                    break;
            }
        }

    }
    

</p>
</details>

    
 If this project help you reduce time to develop, you can give me a cup of coffee :)
 
 [![paypal](https://www.paypalobjects.com/en_US/MX/i/btn/btn_donateCC_LG.gif)](https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=H2TEDQDPJ557A)
    
    
