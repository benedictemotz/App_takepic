using Android.App;
using Android.Widget;
using Android.OS;
using Java.IO;
using Android.Content;
using Android.Content.PM;
using System;
using Android.Provider;
using Android.Runtime;
using Android.Graphics;
using takePic2;
using System.Collections.Generic;
using Environment = Android.OS.Environment;
using Uri = Android.Net.Uri;
using Android.Database;
using Android.Views;
using Android.Graphics.Drawables;
using System.IO;
using Android.Media;

namespace TakePic2
{

    public static class App
    {
        public static Java.IO.File _file;
        public static Java.IO.File _dir;
        public static Bitmap bitmap;
        public static Java.IO.File cropImageFile;


    }
    [Activity(Label = "TakePic2", MainLauncher = true, Icon = "@drawable/icon", Theme = "@style/AppTheme")]
    public class MainActivity : Activity
    {
        ImageView _imageView;
        Intent CropIntent;
        private Uri saveUri = null;
        private Bitmap.CompressFormat outputFormat = Bitmap.CompressFormat.Jpeg;
        private int height;
        private int width;
        Context context = null;
        string filename;

        protected override void OnCreate(Bundle savedInstanceState)
        {
             base.OnCreate(savedInstanceState);

            // Set our view from the "main" layout resource
            
            SetContentView(Resource.Layout.Main);

            if (IsThereAnAppToTakePictures())
            {
                CreateDirectoryForPictures();

                Button buttonCam = FindViewById<Button>(Resource.Id.ButtonCam);
                Button buttonGal = FindViewById<Button>(Resource.Id.ButtonGal);
                Button buttonCrop = FindViewById<Button>(Resource.Id.ButtonCrop);
                //Button buttonDraw = FindViewById<Button>(Resource.Id.ButtonDraw);
                _imageView = FindViewById<ImageView>(Resource.Id.imageview1);
                buttonCam.Click += TakeAPicture;
                buttonGal.Click += OpenGallery;
                buttonCrop.Click += CropImage;
                //buttonDraw.Click += Draw;
                context = this;


            }
        }



        

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            

            if (requestCode == 0)
            {


                Intent mediaScanIntent = new Intent(Intent.ActionMediaScannerScanFile);
                Uri contentUri = Uri.FromFile(App._file);

                mediaScanIntent.SetData(contentUri);
                SendBroadcast(mediaScanIntent);


                height = Resources.DisplayMetrics.HeightPixels;
                width = _imageView.Height;
                App.bitmap = App._file.Path.LoadAndResizeBitmap(width, height);

               
                
                if (App.bitmap != null)
                {
                    _imageView.SetImageBitmap(App.bitmap);
                    //App.bitmap = null;
                }
                /*DrawView drawView = new DrawView(this);
                SetContentView(drawView);
                Drawable image = new BitmapDrawable(App.bitmap);
                _imageView.SetImageBitmap(App.bitmap);
                drawView.start();*/
                
                GC.Collect();
                
            }
          
            else if (requestCode == 2)
            {
                if (data != null)
                {   Bundle bundle = data.Extras;

                    App.bitmap = (Bitmap)bundle.GetParcelable("data");
                    _imageView.SetImageBitmap(App.bitmap);
                }
              
            }

            

            else if (requestCode ==3)
            {
                Uri selectedImage = data.Data;
                String[] filePathColumn = { MediaStore.MediaColumns.Data };
                var cresolv = this.ContentResolver;
                ICursor cursor = cresolv.Query(selectedImage, filePathColumn,null,null,null);
                cursor.MoveToFirst();

                int columnIndex = cursor.GetColumnIndex(filePathColumn[0]);
                String picturePath = cursor.GetString(columnIndex);
                cursor.Close();
                _imageView.SetImageBitmap(BitmapFactory.DecodeFile(picturePath));
                

               
            }

            

        }
     

        private void CreateDirectoryForPictures()
        {
            App._dir = new Java.IO.File(Environment.GetExternalStoragePublicDirectory(Environment.DirectoryPictures), "TakePic2");
            if (!App._dir.Exists())
            {
                App._dir.Mkdirs();
            }
        }

        private bool IsThereAnAppToTakePictures() 
        {
            Intent intent = new Intent(MediaStore.ActionImageCapture);
            IList<ResolveInfo> availableActivities = PackageManager.QueryIntentActivities (intent, PackageInfoFlags.MatchDefaultOnly);
            return availableActivities != null && availableActivities.Count > 0;
        }

        private void TakeAPicture(object sender,EventArgs eventArgs)
        {
            Intent intent = new Intent(MediaStore.ActionImageCapture);
            App._file = new Java.IO.File(App._dir, String.Format("Image_{0}.jpg", Guid.NewGuid()));
            intent.PutExtra(MediaStore.ExtraOutput, Uri.FromFile(App._file));
            StartActivityForResult(intent, 0);
        }

        private void OpenGallery(object sender, EventArgs eventArgs)
        {
            Intent ImageIntent = new Intent(Intent.ActionPick,MediaStore.Images.Media.ExternalContentUri);
            //ImageIntent.SetType("image/*");
            //ImageIntent.SetAction(Intent.ActionGetContent);
            
            StartActivityForResult(Intent.CreateChooser(ImageIntent, "Select Photo"), 3);

        }

        private void CropImage(object sender, EventArgs eventArgs)
        {
            try
            {
                CropIntent = new Intent("com.android.camera.action.CROP");
                CropIntent.SetDataAndType(Uri.FromFile(App._file), "image/*");


                CropIntent.PutExtra("crop", "true");
                CropIntent.PutExtra("outputX", 180);
                CropIntent.PutExtra("outputY", 180);
                CropIntent.PutExtra("aspectX", 3);
                CropIntent.PutExtra("aspectY", 4);
                CropIntent.PutExtra("image_path", Uri.FromFile(App._file));
                CropIntent.PutExtra("scaleUpIfNeeded", true);

                StartActivityForResult(CropIntent, 2);

            }
            catch(ActivityNotFoundException ex)
            {
                Toast.MakeText(this, "This device doesn't support the crop action!", ToastLength.Short).Show();
                
            }
        }

        private void Draw(object sender, EventArgs eventArgs)
        {
            
            Bitmap drawBitmap = App.bitmap;
            MemoryStream baos = new MemoryStream();
            drawBitmap.Compress(Bitmap.CompressFormat.Png, 100, baos);
            byte[] b = baos.ToArray();
            Intent intent = new Intent(this, typeof(MainActivity2));
            intent.PutExtra("drawImage", Uri.FromFile(App._file));

            StartActivity(intent);
        }
       

        
    }



}

