
using System.IO;
using Android.Graphics;

namespace takePic2
{
    public static class BitmapHelpers
    {
        public static Bitmap LoadAndResizeBitmap(this string filename, int width, int height)
        {
            //get dim of the file on the disk
            BitmapFactory.Options options = new BitmapFactory.Options { InJustDecodeBounds = true };
            BitmapFactory.DecodeFile(filename, options);

            //Calculate ratio needed to resize the image 
            int outHeight = options.OutHeight;
            int outWidth = options.OutWidth;
            int inSampleSize = 1;

            if (outHeight > height || outWidth > width)
            {
                inSampleSize = outWidth > outHeight ? outHeight / height : outWidth / width;

            }

            options.InSampleSize = inSampleSize;
            options.InJustDecodeBounds = false;
            Bitmap resizedBitmap = BitmapFactory.DecodeFile(filename, options);


            return resizedBitmap;

        }
    }
}