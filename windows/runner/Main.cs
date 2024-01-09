using System;
using System.Drawing;
using Flutter.Embedding;
using Flutter.Embedding.Win32;

var project = new DartProject("data");
var window = new FlutterWindow(project);
window.CreateAndShow("platform_channel", new Point(10, 10), new Size(1280, 720));
window.SetQuitOnClose(true);
window.RunMessageLoop();
