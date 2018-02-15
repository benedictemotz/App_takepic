using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Uri = Android.Net.Uri;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Graphics;
using Android.Widget;
using TakePic2;
using Android.Graphics.Drawables;
using Android.Views;
using Android.Provider;

namespace takePic2
{
    [Activity(Label ="TakePic2",MainLauncher =true)]
    public class MainActivity2: Activity
    {

        
        Drawable image;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            
            ImageView _imView= FindViewById<ImageView>(Resource.Id.imageview1);
            
            //ImageView imageView = FindViewById<ImageView>(Resource.Id.imageView1);


            Intent mediaScanIntent = new Intent(Intent.ActionMediaScannerScanFile);
            Uri contentUri = Uri.FromFile(App._file);
            mediaScanIntent.SetData(contentUri);
            SendBroadcast(mediaScanIntent);
            
            BitmapFactory.Options options = new BitmapFactory.Options { InJustDecodeBounds = true };
            Bitmap bmp = BitmapFactory.DecodeFile(App._file.Path, options);
            
         


            if (App.bitmap != null)
            {
                image = new BitmapDrawable(bmp);
               
            }
            /*Bundle extras = this.Intent.Extras;
            byte[] b = extras.GetByteArray("drawImage");
            Bitmap bmp = BitmapFactory.DecodeByteArray(b, 0, b.Length);*/


            //_imView.SetImageDrawable(image);
            DrawView drawView = new DrawView(this);
            SetContentView(drawView);
            drawView.start();
            
        }

        private void TakeAPicture(object sender, EventArgs eventArgs)
        {
            Intent intent = new Intent(MediaStore.ActionImageCapture);
            App._file = new Java.IO.File(App._dir, String.Format("Image_{0}.jpg", Guid.NewGuid()));
            intent.PutExtra(MediaStore.ExtraOutput, Uri.FromFile(App._file));
            StartActivityForResult(intent, 0);
        }


    }
}