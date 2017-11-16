using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;

namespace SearchIMG.Models
{
    public class ImageProcessing {

        public static ImageProcessing Singletoninstance;
        List<string> imgBitmapsColored = new List<string>(); //List with all images to return
        List<int[]> histogramas = new List<int[]>(); //List with histograms to compare

        // Returns the singleton instance
        public static ImageProcessing getSingleton()
        {
            if (Singletoninstance == null)
                return Singletoninstance = new ImageProcessing();
            System.Diagnostics.Debug.WriteLine("Se devolvio un singleton!!!");
            return Singletoninstance;
        }

        // Function to start processing each img from database
        public void imgStart() {

            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);
            string systempath = "@../../IMG";
            var filePaths = Directory.GetFiles(systempath);

            foreach (var path in filePaths) {
                using (var bmp = (Bitmap)Image.FromFile(path)) {
                    Bitmap resized = new Bitmap(bmp, new Size(16, 16));
                    Bitmap sadbitmap = sad(resized);
                    bool store = true; // Store the histogram created
                    createHistogram(sadbitmap, store);
                    string temp_str = ImageToBase64(bmp);
                    imgBitmapsColored.Add(temp_str); // Store the image encoded in base64str
                    bmp.Dispose();
                }
            }
            System.Diagnostics.Debug.WriteLine("El procesamiento de todas las imagenes ha terminado");
        }

        // Function to convert to black and white the img
        public Bitmap sad(Bitmap bmp) {
            Bitmap sadbitmap;
            sadbitmap = bmp.Clone(new Rectangle(0, 0, bmp.Width, bmp.Height), bmp.PixelFormat);
            for (int i = 0; i < bmp.Width; i++)
            {
                for (int j = 0; j < bmp.Height; j++)
                {
                    Color c = bmp.GetPixel(i, j);
                    int average = (c.R + c.G + c.B) / 3;
                    sadbitmap.SetPixel(i, j, Color.FromArgb(average, average, average));
                }
            }
            return sadbitmap;
        }

        // Function to convert from binary to decimal
        public int binTodec(string num) {
            int numfinal = 0;
            double exp = num.Length-1;
            int len = num.Length;
            while (len > 0) {
                if (num[0] == '1')
                    numfinal = numfinal + (int)(Math.Pow(2.0, exp));
                exp--;
                num = num.Substring(1, num.Length-1);
                len--;
            }
            System.Diagnostics.Debug.WriteLine("El resultado fue de " + numfinal);
            return numfinal;
        }

        // Function that creates the histogram to compare images RETURNS: final histogram of the img
        public int[] createHistogram(Bitmap bitmap, bool store)
        {
            int[] histogramatemp = new int[256];
            //double normalizer = bitmap.Width * bitmap.Height;
            for (int p = 0; p < 256; p++)
            {
                histogramatemp[p] = 0;
            }
            int width = bitmap.Width;
            int height = bitmap.Height;
            for (int i = 0; i < width; i++)
            {
                int max = 0;
                string numbinario = "";
                for (int j = 0; j < height; j++)
                {
                    Color pixel = bitmap.GetPixel(i, j);
                    Color comparar;
                    try {
                        comparar = bitmap.GetPixel(i - 1, j - 1);
                        if (comparar.R > pixel.R)
                            numbinario += "1";
                        else
                            numbinario += "0";
                    }
                    catch { numbinario += "0";  }
                    try
                    {
                        comparar = bitmap.GetPixel(i - 1, j);
                        if (comparar.R > pixel.R)
                            numbinario += "1";
                        else
                            numbinario += "0";
                    }
                    catch { numbinario += "0"; }
                    try
                    {
                        comparar = bitmap.GetPixel(i - 1, j + 1);
                        if (comparar.R > pixel.R)
                            numbinario += "1";
                        else
                            numbinario += "0";
                    }
                    catch { numbinario += "0"; }
                    try
                    {
                        comparar = bitmap.GetPixel(i, j + 1);
                        if (comparar.R > pixel.R)
                            numbinario += "1";
                        else
                            numbinario += "0";
                    }
                    catch { numbinario += "0"; }
                    try
                    {
                        comparar = bitmap.GetPixel(i + 1, j + 1);
                        if (comparar.R > pixel.R)
                            numbinario += "1";
                        else
                            numbinario += "0";
                    }
                    catch { numbinario += "0"; }
                    try
                    {
                        comparar = bitmap.GetPixel(i + 1, j);
                        if (comparar.R > pixel.R)
                            numbinario += "1";
                        else
                            numbinario += "0";
                    }
                    catch { numbinario += "0"; }
                    try
                    {
                        comparar = bitmap.GetPixel(i + 1, j - 1);
                        if (comparar.R > pixel.R)
                            numbinario += "1";
                        else
                            numbinario += "0";
                    }
                    catch { numbinario += "0"; }
                    try
                    {
                        comparar = bitmap.GetPixel(i, j - 1);
                        if (comparar.R > pixel.R)
                            numbinario += "1";
                        else
                            numbinario += "0";
                    }
                    catch { numbinario += "0"; }
                    // UNIFORM ---->
                    int k = 0;
                    while (k < 7)
                    {
                        int dec = this.binTodec(numbinario);
                        if (dec > max)
                        {
                            max = dec;
                            if (max == 255)
                                break;

                        }
                        if (numbinario[0] == '0')
                            numbinario += "0";
                        else
                            numbinario += "1";
                        numbinario = numbinario.Substring(1, numbinario.Length - 1);
                        k++;
                    }
                    histogramatemp[max] = histogramatemp[max] + 1;
                    numbinario = "";
                }
            }
            if (store) // If you want to store the img histogram
            {
                histogramas.Add(histogramatemp);
                System.Diagnostics.Debug.WriteLine("Se anadio un histograma");
            }
            return histogramatemp;
        }

        public List<string> comparison_histograms(Bitmap bmp, int img_count) {
            List<string> IMGstringList = new List<string>(); // List of final images
            List<double> diferences = new List<double>(); // List with the diferences of histograms
            Bitmap sadbitmap = sad(bmp); // Black and white picture
            bool store = false; // If you want to store the histogram
            int[] current_Histogram = createHistogram(sadbitmap, store);
            int sumafinal = 0; // Final Diference between histograms
            //System.Diagnostics.Debug.WriteLine("La cantidad de histogramas es de: {0}", histogramas.Count);
            for (int i = 0; i < histogramas.Count; i++)
            {
                sumafinal = 0;
                for(int j = 0; j < 256; j++)
                {
                    int num1 = (int)((histogramas[i])[j]);
                    int num2 = current_Histogram[j];
                    int dif_temp = num1 - num2;
                    int suma = (int)Math.Pow((double)dif_temp, 2.0);
                    sumafinal = sumafinal + suma;
                }
                double dif = Math.Sqrt(sumafinal);
                if(IMGstringList.Count == 0)
                {
                    diferences.Add(dif);
                    IMGstringList.Add(imgBitmapsColored[i]);
                }
                else
                {
                    bool isIN = false;
                    for (int k = 0; k < diferences.Count; k++)
                    {
                        System.Diagnostics.Debug.WriteLine("{0} < {1}?", dif, diferences[k]);
                        if (dif < diferences[k])
                        {
                            diferences.Insert(k, dif);
                            IMGstringList.Insert(k, imgBitmapsColored[i]);
                            isIN = true;
                            break;
                        }
                        if (k == img_count-1)
                            break;
                    }
                    if (!isIN)
                    {
                        diferences.Add(dif);
                        IMGstringList.Add(imgBitmapsColored[i]);
                    }
                    while (IMGstringList.Count > img_count)
                    {
                        IMGstringList.RemoveAt(img_count);
                    }
                    while (diferences.Count > img_count)
                    {
                        diferences.RemoveAt(img_count);
                    }
                }
            }
            foreach(Object obj in diferences)
            {
                System.Diagnostics.Debug.WriteLine("La diferencia es de: {0}", obj);
            }
            return IMGstringList;
        }

        // To return each img encoded so Android, etc, can read them
        public string ImageToBase64(Image image)
        {
            if (image == null)
                return null;
            MemoryStream ms = new MemoryStream();

            // Convert Image to byte[]
            image.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            byte[] imageBytes = ms.ToArray();

            // Convert byte[] to base 64 string
            string base64String = Convert.ToBase64String(imageBytes);

            return base64String;
        }
    }
}