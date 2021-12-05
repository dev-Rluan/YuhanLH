//using System;
//using System.Collections.Generic;
//using System.Drawing;
//using System.Drawing.Imaging;
//using System.IO;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace DummyClient2
//{
//    public class ScreenCopy
//    {
//        // 사용예: ScreenCopy.Copy("test.png");
//        // 
//        public static byte[] Copy()
//        {
//            // 주화면의 크기 정보 읽기
//            Rectangle rect = Screen.PrimaryScreen.Bounds;
//            // 2nd screen = Screen.AllScreens[1]

//            // 픽셀 포맷 정보 얻기 (Optional)
//            int bitsPerPixel = Screen.PrimaryScreen.BitsPerPixel;
//            PixelFormat pixelFormat = PixelFormat.Format32bppArgb;
//            if (bitsPerPixel <= 16)
//            {
//                pixelFormat = PixelFormat.Format16bppRgb565;
//            }
//            if (bitsPerPixel == 24)
//            {
//                pixelFormat = PixelFormat.Format24bppRgb;
//            }

//            // 화면 크기만큼의 Bitmap 생성
//            Bitmap bmp = new Bitmap(rect.Width, rect.Height, pixelFormat);

//            // Bitmap 이미지 변경을 위해 Graphics 객체 생성
//            using (Graphics gr = Graphics.FromImage(bmp))
//            {
//                // 화면을 그대로 카피해서 Bitmap 메모리에 저장
//                gr.CopyFromScreen(rect.Left, rect.Top, 0, 0, rect.Size);
//            }

//            byte[] t = ImageToByte(bmp);

//            return t;

//            // Bitmap 데이타를 파일로 저장
//            // bmp.Save(outputFilename); 
//            //bmp.Dispose();
//        }
//        public static Bitmap GetBitmap(byte[] sourceByteArray)
//        {
//            MemoryStream memoryStream = new MemoryStream(sourceByteArray);
//            Bitmap bitmap = new Bitmap(memoryStream); return bitmap;
//        }
//        public static byte[] ImageToByte(Image img)
//        {
//            ImageConverter converter = new ImageConverter();
//            return (byte[])converter.ConvertTo(img, typeof(byte[]));
//        }



//    }
//}
