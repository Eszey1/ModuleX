using AVFoundation;
using CoreFoundation;
using CoreGraphics;
using APEX.Mobile.Interfaces;
using Foundation;
using UIKit;

namespace APEX.Mobile.Platforms.iOS
{
    public class BarcodeScanner : IBarcodeScanner
    {
        public async Task<string?> ScanBarcodeAsync()
        {
            var tcs = new TaskCompletionSource<string?>();

            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                var scannerVC = new BarcodeScannerViewController();
                scannerVC.OnBarcodeDetected = (barcode) =>
                {
                    tcs.TrySetResult(barcode);
                    scannerVC.DismissViewController(true, null);
                };
                scannerVC.OnCancelled = () =>
                {
                    tcs.TrySetResult(null);
                    scannerVC.DismissViewController(true, null);
                };

                var navController = new UINavigationController(scannerVC);
                var window = UIApplication.SharedApplication.KeyWindow;
                var rootVC = window?.RootViewController;
                rootVC?.PresentViewController(navController, true, null);
            });

            return await tcs.Task;
        }
    }

    public class BarcodeScannerViewController : UIViewController
    {
        private AVCaptureSession _captureSession;
        private AVCaptureVideoPreviewLayer _previewLayer;
        public Action<string>? OnBarcodeDetected { get; set; }
        public Action? OnCancelled { get; set; }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            Title = "Barkod Okut";
            View.BackgroundColor = UIColor.Black;

            var cancelButton = new UIBarButtonItem(UIBarButtonSystemItem.Cancel, (s, e) =>
            {
                OnCancelled?.Invoke();
            });
            NavigationItem.LeftBarButtonItem = cancelButton;

            SetupCamera();
        }

        private void SetupCamera()
        {
            _captureSession = new AVCaptureSession();
            var videoCaptureDevice = AVCaptureDevice.GetDefaultDevice(AVMediaTypes.Video);

            if (videoCaptureDevice == null) return;

            var videoInput = AVCaptureDeviceInput.FromDevice(videoCaptureDevice);
            if (videoInput == null) return;

            _captureSession.AddInput(videoInput);

            var metadataOutput = new AVCaptureMetadataOutput();
            _captureSession.AddOutput(metadataOutput);

            metadataOutput.SetDelegate(new MetadataDelegate(barcode =>
            {
                OnBarcodeDetected?.Invoke(barcode);
            }), DispatchQueue.MainQueue);

            metadataOutput.MetadataObjectTypes = AVMetadataObjectType.EAN13Code |
                                                  AVMetadataObjectType.EAN8Code |
                                                  AVMetadataObjectType.Code128Code |
                                                  AVMetadataObjectType.QRCode;

            _previewLayer = new AVCaptureVideoPreviewLayer(_captureSession)
            {
                Frame = View.Bounds,
                VideoGravity = AVLayerVideoGravity.ResizeAspectFill
            };

            View.Layer.AddSublayer(_previewLayer);
            _captureSession.StartRunning();
        }

        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);
            _captureSession?.StopRunning();
        }
    }

    public class MetadataDelegate : AVCaptureMetadataOutputObjectsDelegate
    {
        private readonly Action<string> _onBarcodeDetected;

        public MetadataDelegate(Action<string> onBarcodeDetected)
        {
            _onBarcodeDetected = onBarcodeDetected;
        }

        public override void DidOutputMetadataObjects(AVCaptureMetadataOutput captureOutput,
            AVMetadataObject[] metadataObjects, AVCaptureConnection connection)
        {
            if (metadataObjects.Length > 0)
            {
                var metadata = metadataObjects[0] as AVMetadataMachineReadableCodeObject;
                if (metadata != null)
                {
                    _onBarcodeDetected?.Invoke(metadata.StringValue);
                }
            }
        }
    }
}
