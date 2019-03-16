using CHD;
using System;
using System.Diagnostics;
using System.Linq.Expressions;
using Xamarin.Forms;

namespace PanGesture
{
	public partial class HomePage : ContentPage
	{

        private bool _scrollingByPan;

        public HomePage ()
		{
			InitializeComponent ();
            CHDBottomSheetBs.MarginTop = 60;
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
}

