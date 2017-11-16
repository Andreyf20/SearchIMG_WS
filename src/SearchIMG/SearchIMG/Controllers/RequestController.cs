using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using SearchIMG.Models;
using System.Text;
using System.Drawing;
using System.IO;
using System.Collections;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SearchIMG.Controllers
{
    public class IMG {
        public string encodedimg;
    }

    public class RequestController : ApiController
    {
        private ImageProcessing baseimg = ImageProcessing.getSingleton();

        // GET: api/Request
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/Request/5
        public string Get(int id)
        {
            return "NULL";
        }

        // GET: api/Request/number_images/image
        [Route("api/Request/{number_images}/{image}")]
        public string GetImage(int number_images, string image)
        {
            return "null";
        }

        // POST: api/Request
        //IEnumerable<string>
        [Route("api/Request/{number_images}")]
        public string Post(int number_images, [FromBody]JObject image)
        {
            //JObject jobject = JObject.Parse(image.encodedimg);
            string parseIMG = (string)image["image"];

            System.Diagnostics.Debug.WriteLine("Se recibio una peticion!!!");
            if (image == null)
            {
                System.Diagnostics.Debug.WriteLine("La imagen fue nula!!!");
                return "NULL";

            }

            //base64str!! (image format)
            List<string> IMGstring = new List<string>(); // List with images to return

            // Convert string to byte[]
            byte[] imageBytes = Convert.FromBase64String(parseIMG);
            using (var ms = new MemoryStream(imageBytes, 0, imageBytes.Length))
            {
                // Convert byte[] to Image
                ms.Write(imageBytes, 0, imageBytes.Length);
                Image final_image = Image.FromStream(ms, true);

                // Resized bitmap for the operation
                Bitmap image_bitmap = new Bitmap(final_image, new Size(16, 16));

                // Final list of imgs
                IMGstring = baseimg.comparison_histograms(image_bitmap, number_images);
            }

            //Return json response with all images
            var dict = new Dictionary<string, IMG[]>();
            dict.Add("image", new IMG[number_images]);
            int i = 0;
            while (i < number_images)
            {
                dict["image"][i] = new IMG { encodedimg = IMGstring[i] };
                i++;
            }
            var json = JsonConvert.SerializeObject(dict);
            return json;
        }

        // POST: api/Request/test/
        [Route("api/Request/test/")]
        public string postSTR([FromBody]Object image) {
            return image.GetType().ToString();
        }

        // PUT: api/Request/5
        public void Put([FromBody]string image)
        {
        }

        // DELETE: api/Request/5
        public void Delete(int id)
        {
        }
    }
}
