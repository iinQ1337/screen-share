using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Fleck;

class Program
{
    static void Main()
    {
        var server = new WebSocketServer("ws://127.0.0.1:3000");
        server.Start(socket =>
        {
            Console.WriteLine($"Connection opend: {socket.ConnectionInfo.ClientIpAddress}");

            socket.OnClose = () => Console.WriteLine($"Connection closed: {socket.ConnectionInfo.ClientIpAddress}");

            _ = Task.Run(() => StartCapture(socket));

        });

        Console.WriteLine("Server started");
        Console.ReadLine();

        server.Dispose();
    }

    private static void StartCapture(IWebSocketConnection socket)
    {
        while (socket.IsAvailable)
        {
            var screenshot = CaptureScreen();
            SendImage(socket, screenshot);
            //Task.Delay(100).Wait(); 
        }
    }

    private static Bitmap CaptureScreen()
    {
        var screenSize = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Size;
        var screenBmp = new Bitmap(screenSize.Width, screenSize.Height);

        using (var graphics = Graphics.FromImage(screenBmp))
        {
            graphics.CopyFromScreen(0, 0, 0, 0, screenSize);
        }

        return screenBmp;
    }

    private static void SendImage(IWebSocketConnection socket, Bitmap image)
    {
        using (var memoryStream = new MemoryStream())
        {
            var encoder = ImageCodecInfo.GetImageEncoders().First(c => c.FormatID == ImageFormat.Jpeg.Guid);
            var encoderParameters = new EncoderParameters(1);
            encoderParameters.Param[0] = new EncoderParameter(Encoder.Quality, 80L); 

            image.Save(memoryStream, encoder, encoderParameters);
            byte[] imageData = memoryStream.ToArray();

            string base64Image = Convert.ToBase64String(imageData);
            socket.Send(base64Image);
        }
    }
}
