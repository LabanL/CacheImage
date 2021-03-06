﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Windows.Media.Imaging;
using System.IO.IsolatedStorage;
using System.IO;
using Microsoft.Phone.Net.NetworkInformation;
using System.Diagnostics;
using System.Windows.Media;
using System.ComponentModel;

namespace KhmelenkoLab
{
    public partial class CacheImage : UserControl
    {

        #region Dependency Properties

        // URL
        public string Url
        {
            get { return base.GetValue(UrlProperty) as string; }
            set { base.SetValue(UrlProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Url.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty UrlProperty =
            DependencyProperty.RegisterAttached("Url", typeof(string), typeof(CacheImage), new PropertyMetadata(OnUrlChanged));


        // Placeholder
        public string Placeholder
        {
            get { return base.GetValue(PlaceholderProperty) as string; }
            set { base.SetValue(PlaceholderProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Placeholder.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PlaceholderProperty =
            DependencyProperty.RegisterAttached("Placeholder", typeof(string), typeof(CacheImage), new PropertyMetadata(OnPlaceholderChanged));


        // DecodePixelHeight
        public int DecodePixelHeight
        {
            get { return (int)base.GetValue(DecodePixelHeightProperty); }
            set { base.SetValue(DecodePixelHeightProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DecodePixelHeight.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DecodePixelHeightProperty =
            DependencyProperty.RegisterAttached("DecodePixelHeight", typeof(int), typeof(CacheImage), new PropertyMetadata(OnDecodePixelHeightChanged));


        // DecodePixelWidth
        public int DecodePixelWidth
        {
            get { return (int)base.GetValue(DecodePixelWidthProperty); }
            set { base.SetValue(DecodePixelWidthProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DecodePixelWidth.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DecodePixelWidthProperty =
            DependencyProperty.RegisterAttached("DecodePixelWidth", typeof(int), typeof(CacheImage), new PropertyMetadata(OnDecodePixelWidthChanged));


        // DecodePixelWIdth for Placeholder
        public int DecodePixelWidthPlaceholder
        {
            get { return (int)base.GetValue(DecodePixelWidthPlaceholderProperty); }
            set { base.SetValue(DecodePixelWidthPlaceholderProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DecodePixelWidthPlaceholder.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DecodePixelWidthPlaceholderProperty =
            DependencyProperty.RegisterAttached("DecodePixelWidthPlaceholder", typeof(int), typeof(CacheImage), new PropertyMetadata(OnDecodePixelWidthPlaceholderChanged));



        // DecodePixelHeight for Placeholder
        public int DecodePixelHeightPlaceholder
        {
            get { return (int)base.GetValue(DecodePixelHeightPlaceholderProperty); }
            set { base.SetValue(DecodePixelHeightPlaceholderProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DecodePixelHeightPlaceholder.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DecodePixelHeightPlaceholderProperty =
            DependencyProperty.RegisterAttached("DecodePixelHeightPlaceholder", typeof(int), typeof(CacheImage), new PropertyMetadata(OnDecodePixelHeightPlaceholderChanged));


        // Stretch
        public Stretch Stretch
        {
            get { return (Stretch)base.GetValue(StretchProperty); }
            set { base.SetValue(StretchProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Stretch.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StretchProperty =
            DependencyProperty.RegisterAttached("Stretch", typeof(Stretch), typeof(CacheImage), new PropertyMetadata(OnStretchChanged));


        // Stretch for placeholder
        public Stretch StretchPlaceholder
        {
            get { return (Stretch)base.GetValue(StretchPlaceholderProperty); }
            set { base.SetValue(StretchPlaceholderProperty, value); }
        }

        // Using a DependencyProperty as the backing store for StretchPlaceholder.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StretchPlaceholderProperty =
            DependencyProperty.RegisterAttached("StretchPlaceholder", typeof(Stretch), typeof(CacheImage), new PropertyMetadata(OnStretchPlaceholderChanged));


        #endregion


        private const string imageStorageFolder = "ImagesCache";

        private WebClient _webClient = new WebClient();

        public CacheImage()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Called when bitmap Url changed
        /// </summary>
        private static void OnUrlChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            // put the task into the queue
            Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    var control = d as CacheImage;
                    control.Visibility = Visibility.Visible;

                    string url = e.NewValue.ToString();
                    Uri imageUri = new Uri(url);
                    if (imageUri.Scheme == "http" || imageUri.Scheme == "https")
                    {
                        var storage = IsolatedStorageFile.GetUserStoreForApplication();
                        if (storage.FileExists(control.GetFileNameInIsolatedStorage(imageUri)))
                        {
                            control.LoadFromLocalStorage(imageUri, control.bitmapImage);
                        }
                        else
                        {
                            if (NetworkInterface.GetIsNetworkAvailable())
                            {
                                control.LoadFromWebAndCache(imageUri, control.bitmapImage);
                            }
                        }
                    }
                    else
                    {
                        control.bitmapImage.UriSource = imageUri;
                    }

                });
        }

        /// <summary>
        /// Called when placeholder changed
        /// </summary>
        private static void OnPlaceholderChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    var control = d as CacheImage;
                    var placeholderPath = e.NewValue.ToString();
                    control.placeholderImage.UriSource = new Uri(placeholderPath, UriKind.RelativeOrAbsolute);
                });

        }

        /// <summary>
        /// Called when DecodePixelHeight changed
        /// </summary>
        private static void OnDecodePixelHeightChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as CacheImage;
            var newValue = e.NewValue as int?;
            if (newValue.HasValue)
            {
                control.bitmapImage.DecodePixelHeight = newValue.Value;
            }
        }

        /// <summary>
        /// Called when DecodePixelWidth changed
        /// </summary>
        private static void OnDecodePixelWidthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as CacheImage;
            var newValue = e.NewValue as int?;
            if (newValue.HasValue)
            {
                control.bitmapImage.DecodePixelWidth = newValue.Value;
            }
        }

        /// <summary>
        /// Called when DecodePixelHeightPlaceholder changed
        /// </summary>
        private static void OnDecodePixelHeightPlaceholderChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as CacheImage;
            var newValue = e.NewValue as int?;
            if (newValue.HasValue)
            {
                control.placeholderImage.DecodePixelHeight = newValue.Value;
            }
        }

        /// <summary>
        /// Called when DecodePixelWidthPlaceholder changed
        /// </summary>
        private static void OnDecodePixelWidthPlaceholderChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as CacheImage;
            var newValue = e.NewValue as int?;
            if (newValue.HasValue)
            {
                control.placeholderImage.DecodePixelWidth = newValue.Value;
            }
        }

        /// <summary>
        /// Called when image property Stretch changed
        /// </summary>
        private static void OnStretchChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as CacheImage;
            var newValue = e.NewValue as Stretch?;
            if (newValue.HasValue)
            {
                control.image.Stretch = newValue.Value;
            }
        }

        /// <summary>
        /// Called when placeholder property Stretch changed
        /// </summary>
        private static void OnStretchPlaceholderChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as CacheImage;
            var newValue = e.NewValue as Stretch?;
            if (newValue.HasValue)
            {
                control.placeholder.Stretch = newValue.Value;
            }
        }

        /// <summary>
        /// Loads an image from web and caches it
        /// </summary>
        /// <param name="imageUri">Image URI</param>
        /// <param name="bitmap">Bitmap for loaded image</param>
        private void LoadFromWebAndCache(Uri imageUri, BitmapImage bitmap)
        {
            if(_webClient.IsBusy)
            {
                _webClient.CancelAsync();
                _webClient = new WebClient();
            }
            
            _webClient.OpenReadCompleted += (o, e) =>
            {
                if (e.Error != null || e.Cancelled)
                    return;

                var stream = e.Result;
                if (stream.CanRead)
                {
                    bitmap.ImageOpened += bitmap_ImageOpened;

                    WriteToIsolatedStorage(e.Result, GetFileNameInIsolatedStorage(imageUri));
                    bitmap.SetSource(e.Result);
                    e.Result.Close();
                }
            };
            _webClient.OpenReadAsync(imageUri);
        }

        void bitmap_ImageOpened(object sender, RoutedEventArgs e)
        {
            (sender as BitmapImage).ImageOpened -= bitmap_ImageOpened;
            HidePlaceholder();
        }

        /// <summary>
        /// Hides placeholder
        /// </summary>
        private void HidePlaceholder()
        {
            placeholder.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// Loads an image from the local storage
        /// </summary>
        /// <param name="imageUri">Image URI</param>
        /// <returns>Loaded image</returns>
        private void LoadFromLocalStorage(Uri imageUri, BitmapImage bitmap)
        {
            string isolatedStoragePath = GetFileNameInIsolatedStorage(imageUri);
            var storage = IsolatedStorageFile.GetUserStoreForApplication();
            using (var sourceFile = storage.OpenFile(isolatedStoragePath, FileMode.Open, FileAccess.Read))
            {
                bitmap.SetSource(sourceFile);
            }

            HidePlaceholder();
        }

        /// <summary>
        /// Writes the stream data to isolated storage
        /// </summary>
        /// <param name="inputStream">Input stream</param>
        /// <param name="fileName">File name</param>
        private void WriteToIsolatedStorage(System.IO.Stream inputStream, string fileName)
        {
            var storage = IsolatedStorageFile.GetUserStoreForApplication();
            IsolatedStorageFileStream outputStream = null;
            try
            {
                if (!storage.DirectoryExists(imageStorageFolder))
                {
                    storage.CreateDirectory(imageStorageFolder);
                }
                if (storage.FileExists(fileName))
                {
                    storage.DeleteFile(fileName);
                }
                outputStream = storage.CreateFile(fileName);
                byte[] buffer = new byte[32768];
                int read;
                while ((read = inputStream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    outputStream.Write(buffer, 0, read);
                }
            }
            catch (Exception e)
            {
                // ignore exceptions
                Debug.WriteLine(e);
            }
            finally
            {
                if (outputStream != null)
                {
                    outputStream.Close();
                }
            }
        }

        /// <summary>
        /// Gets the file name in isolated storage for the Uri specified. This name should be used to search in the isolated storage.
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <returns></returns>
        public string GetFileNameInIsolatedStorage(Uri uri)
        {
            return imageStorageFolder + "\\" + uri.AbsoluteUri.GetHashCode() + ".img";
        }
    }
}
