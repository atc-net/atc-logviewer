global using System.ComponentModel;
global using System.Diagnostics;
global using System.Diagnostics.CodeAnalysis;
global using System.Globalization;
global using System.IO;
global using System.Text.Json;
global using System.Windows;
global using System.Windows.Controls;
global using System.Windows.Data;
global using System.Windows.Input;
global using System.Windows.Media;
global using System.Windows.Media.Imaging;
global using System.Windows.Threading;

global using Atc.Helpers;
global using Atc.LogAnalyzer;
global using Atc.LogCollector;
global using Atc.LogCollector.Log4Net;
global using Atc.LogCollector.NLog;
global using Atc.LogCollector.Serilog;
global using Atc.LogCollector.Syslog;
global using Atc.LogViewer.Wpf.App.Dialogs;
global using Atc.LogViewer.Wpf.App.Extensions;
global using Atc.LogViewer.Wpf.App.Factories;
global using Atc.LogViewer.Wpf.App.Models;
global using Atc.LogViewer.Wpf.App.Options;
global using Atc.LogViewer.Wpf.App.UserControls;
global using Atc.Serialization;
global using Atc.Wpf.Collections;
global using Atc.Wpf.Command;
global using Atc.Wpf.Controls.Dialogs;
global using Atc.Wpf.Controls.LabelControls;
global using Atc.Wpf.Controls.LabelControls.Abstractions;
global using Atc.Wpf.Diagnostics;
global using Atc.Wpf.Helpers;
global using Atc.Wpf.Messaging;
global using Atc.Wpf.Mvvm;
global using Atc.Wpf.Serialization.JsonConverters;
global using Atc.Wpf.Translation;

global using ControlzEx.Theming;

global using LiveChartsCore;
global using LiveChartsCore.Measure;
global using LiveChartsCore.SkiaSharpView;
global using LiveChartsCore.SkiaSharpView.Painting;
global using LiveChartsCore.SkiaSharpView.VisualElements;

global using Microsoft.Extensions.Configuration;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.Hosting;
global using Microsoft.Extensions.Logging;
global using Microsoft.Win32;

global using SkiaSharp;