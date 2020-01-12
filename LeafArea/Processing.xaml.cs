using SharedLogic;
using System.Drawing;
using System.Windows;

namespace LeafArea
{
    public partial class Processing : Window
    {
        System.Windows.Controls.Image imageControl;
        Bitmap forReset;
        Bitmap originalImage;
        ChangeInvoker changeInvoker;

        public Processing(System.Windows.Controls.Image mainImage)
        {
            InitializeComponent();
            imageControl = mainImage;
            originalImage = SourceBitmapConverter.BitmapFromSource(mainImage.Source);
            forReset = SourceBitmapConverter.BitmapFromSource(mainImage.Source);
        }

        private void GrayScale_Click(object sender, RoutedEventArgs e)
        {
            if (imageControl == null || imageControl.Source == null) return;
            imageControl.Source = SourceBitmapConverter.ImageSourceFromBitmap(CvProcessor.ToGray(originalImage));
        }

        private void BrightnessSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            changeInvoker = ChangeInvoker.Brightness;
            if (imageControl == null || imageControl.Source == null || InteractiveMode.IsChecked != true) return;
            float[] kernel = new float[9];
            kernel[0] = ((float)BrightnessSlider1.Value) / 10;
            kernel[1] = ((float)BrightnessSlider2.Value) / 10;
            kernel[2] = ((float)BrightnessSlider3.Value) / 10;
            kernel[3] = ((float)BrightnessSlider4.Value) / 10;
            kernel[4] = ((float)BrightnessSlider5.Value) / 10;
            kernel[5] = ((float)BrightnessSlider6.Value) / 10;
            kernel[6] = ((float)BrightnessSlider7.Value) / 10;
            kernel[7] = ((float)BrightnessSlider8.Value) / 10;
            kernel[8] = ((float)BrightnessSlider9.Value) / 10;
            imageControl.Source = SourceBitmapConverter.ImageSourceFromBitmap(CvProcessor.ChangeBrighness(originalImage, kernel));
        }

        private void AdaptiveSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            changeInvoker = ChangeInvoker.Threshold;
            if (imageControl == null || imageControl.Source == null || InteractiveMode.IsChecked != true) return;
            imageControl.Source = SourceBitmapConverter.ImageSourceFromBitmap(CvProcessor.AdaptiveThreshold(originalImage, (int)AdaptiveSlider1.Value, (int)AdaptiveSlider2.Value));
        }

        private void CannySlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            changeInvoker = ChangeInvoker.Canny;
            if (imageControl == null || imageControl.Source == null || InteractiveMode.IsChecked != true) return;
            imageControl.Source = SourceBitmapConverter.ImageSourceFromBitmap(CvProcessor.Canny(originalImage, (int)CannySlider1.Value, (int)CannySlider2.Value, (int)CannySlider3.Value));
        }

        private void SmoothSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            changeInvoker = ChangeInvoker.Smooth;
            if (imageControl == null || imageControl.Source == null || InteractiveMode.IsChecked != true) return;
            imageControl.Source = SourceBitmapConverter.ImageSourceFromBitmap(CvProcessor.ChangeSmooth(originalImage, (int)SmoothSlider.Value));
        }

        private void BinSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            changeInvoker = ChangeInvoker.Binarization;
            if (imageControl == null || imageControl.Source == null || InteractiveMode.IsChecked != true) return;
            imageControl.Source = SourceBitmapConverter.ImageSourceFromBitmap(CvProcessor.ChangeBin(originalImage, (int)BinSlider1.Value, (int)BinSlider2.Value));
        }

        private void Reset_Click(object sender, RoutedEventArgs e)
        {
            BinSlider1.Value = 0;
            BinSlider2.Value = 255;
            SmoothSlider.Value = 1;
            CannySlider1.Value = 0;
            CannySlider2.Value = 100;
            CannySlider3.Value = 1;
            AdaptiveSlider1.Value = 0;
            AdaptiveSlider2.Value = 101;
            BrightnessSlider1.Value = 2;
            BrightnessSlider2.Value = 2;
            BrightnessSlider3.Value = 2;
            BrightnessSlider4.Value = 2;
            BrightnessSlider5.Value = 0;
            BrightnessSlider6.Value = 2;
            BrightnessSlider7.Value = 2;
            BrightnessSlider8.Value = 2;
            BrightnessSlider9.Value = 2;
            imageControl.Source = SourceBitmapConverter.ImageSourceFromBitmap(forReset);
            originalImage = forReset;
        }

        private void Apply_Click(object sender, RoutedEventArgs e)
        {
            if (imageControl == null || imageControl.Source == null) return;
            switch (changeInvoker)
            {
                case ChangeInvoker.Smooth:
                    imageControl.Source = SourceBitmapConverter.ImageSourceFromBitmap(CvProcessor.ChangeSmooth(originalImage, (int)SmoothSlider.Value));
                    break;
                case ChangeInvoker.Brightness:
                    float[] kernel = new float[9];
                    kernel[0] = ((float)BrightnessSlider1.Value) / 10;
                    kernel[1] = ((float)BrightnessSlider2.Value) / 10;
                    kernel[2] = ((float)BrightnessSlider3.Value) / 10;
                    kernel[3] = ((float)BrightnessSlider4.Value) / 10;
                    kernel[4] = ((float)BrightnessSlider5.Value) / 10;
                    kernel[5] = ((float)BrightnessSlider6.Value) / 10;
                    kernel[6] = ((float)BrightnessSlider7.Value) / 10;
                    kernel[7] = ((float)BrightnessSlider8.Value) / 10;
                    kernel[8] = ((float)BrightnessSlider9.Value) / 10;
                    imageControl.Source = SourceBitmapConverter.ImageSourceFromBitmap(CvProcessor.ChangeBrighness(originalImage, kernel));
                    break;
                case ChangeInvoker.Canny:
                    imageControl.Source = SourceBitmapConverter.ImageSourceFromBitmap(CvProcessor.Canny(originalImage, (int)CannySlider1.Value, (int)CannySlider2.Value, (int)CannySlider3.Value));
                    break;
                case ChangeInvoker.Threshold:
                    imageControl.Source = SourceBitmapConverter.ImageSourceFromBitmap(CvProcessor.AdaptiveThreshold(originalImage, (int)AdaptiveSlider1.Value, (int)AdaptiveSlider2.Value));
                    break;
                case ChangeInvoker.Binarization:
                    imageControl.Source = SourceBitmapConverter.ImageSourceFromBitmap(CvProcessor.ChangeBin(originalImage, (int)BinSlider1.Value, (int)BinSlider2.Value));
                    break;
            }

            originalImage = SourceBitmapConverter.BitmapFromSource(imageControl.Source);
        }
    }
}