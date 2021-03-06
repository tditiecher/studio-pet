﻿using System;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using Microsoft.VisualStudio.Text.Editor;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace StudioPet
{
    public class ViewportAdornment : IDisposable
    {
        public const string Name = "StudioPet.ViewportAdornment";

        private readonly IWpfTextView _view;
        private readonly IAdornmentLayer _adornmentLayer;
        private readonly ConfigurableImageShell _shellControl;

        public IImageShell Shell => _shellControl;

        public ViewportAdornment(IWpfTextView view)
        {
            _view = view;
            _adornmentLayer = view.GetAdornmentLayer(Name);

            var configuration = LoadConfiguration();
            _shellControl = new ConfigurableImageShell(configuration);

            _view.ViewportHeightChanged += OnSizeChanged;
            _view.ViewportWidthChanged += OnSizeChanged;
            _view.LayoutChanged += OnSizeChanged;
        }

        public void Dispose()
        {
            if (_view != null)
            {
                _view.ViewportHeightChanged -= OnSizeChanged;
                _view.ViewportWidthChanged -= OnSizeChanged;
                _view.LayoutChanged -= OnSizeChanged;
            }
        }

        private void OnSizeChanged(object sender, EventArgs e)
        {
            _adornmentLayer.RemoveAllAdornments();

            var zoomFactor = 100 / Math.Max(_view.ZoomLevel, 0.01);

            _shellControl.Width = _shellControl.Configuration.Width * zoomFactor;
            _shellControl.Height = _shellControl.Configuration.Height * zoomFactor;

            var margin = _shellControl.Configuration.Margin * zoomFactor;

            switch (_shellControl.Configuration.HorizontalAlignment)
            {
                case HorizontalAlignment.Left:
                    Canvas.SetLeft(_shellControl, _view.ViewportLeft + margin);
                    break;
                case HorizontalAlignment.Center:
                    Canvas.SetLeft(_shellControl, _view.ViewportLeft + (_view.ViewportWidth - _shellControl.Width) / 2);
                    break;
                default:
                    // Right
                    Canvas.SetLeft(_shellControl, _view.ViewportRight - _shellControl.Width - margin);
                    break;
            }

            switch (_shellControl.Configuration.VerticalAlignment)
            {
                case VerticalAlignment.Top:
                    Canvas.SetTop(_shellControl, _view.ViewportTop + margin);
                    break;
                case VerticalAlignment.Center:
                    Canvas.SetTop(_shellControl, _view.ViewportTop + (_view.ViewportHeight - _shellControl.Height) / 2);
                    break;
                default:
                    // Bottom
                    Canvas.SetTop(_shellControl, _view.ViewportBottom - _shellControl.Height - margin);
                    break;
            }

            _adornmentLayer.AddAdornment(AdornmentPositioningBehavior.ViewportRelative, null, null, _shellControl, null);
        }

        private static ImageShellConfiguration LoadConfiguration()
        {
            var configuration = new ImageShellConfiguration();

            var studioPetPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "StudioPet");
            var configFile = Path.Combine(studioPetPath, "studiopet.config");

            if (!Directory.Exists(studioPetPath))
            {
                Directory.CreateDirectory(studioPetPath);
                using (var file = File.CreateText(configFile))
                {
                    var serializer = new JsonSerializer {Formatting = Formatting.Indented};
                    serializer.Converters.Add(new StringEnumConverter());
                    serializer.Serialize(file, configuration);
                }


                var extensionPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                if (extensionPath != null)
                {
                    foreach (var imageFile in new DirectoryInfo(Path.Combine(extensionPath, "PictureSets")).GetFiles("*.*", SearchOption.AllDirectories))
                    {
                        var destFile = new FileInfo(imageFile.FullName.Replace(extensionPath, studioPetPath));
                        if (destFile.Directory != null && !destFile.Directory.Exists)
                        {
                            destFile.Directory.Create();
                        }
                        File.Copy(imageFile.FullName, destFile.FullName);
                    }
                }
            }

            if (File.Exists(configFile))
            {
                try
                {
                    var serializer = new JsonSerializer();
                    serializer.Converters.Add(new StringEnumConverter());

                    using (var file = File.OpenText(configFile))
                    {
                        using (var reader = new JsonTextReader(file))
                        {
                            configuration = serializer.Deserialize<ImageShellConfiguration>(reader);
                        }
                    }
                }
                catch (Exception err)
                {
                    var errorLog =
                        $"{DateTime.Now:dd-MM-yyyy HH:mm:ss}{Environment.NewLine}{err.GetType()}: {err.Message}{Environment.NewLine}{err.StackTrace}";
                    File.WriteAllText(Path.Combine(studioPetPath, "error.log"), errorLog);
                }
            }

            configuration.ImageFolder = Path.Combine(studioPetPath, "PictureSets", configuration.PictureSet);
            return configuration;
        }
    }
}
