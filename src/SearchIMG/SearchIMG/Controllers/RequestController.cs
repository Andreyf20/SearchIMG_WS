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

namespace SearchIMG.Controllers
{
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
        [Route("api/Request/{number_images}")]
        public string Post(int number_images, [FromBody]string image)
        {
            //Este es el metodo al cual hay que enviar la imagen en formato base64str!!
            List<string> IMGstring = new List<string>(); // List with images
            string Final_Str = "ERROR-> No he cambiado el error inicial";

            //System.Diagnostics.Debug.WriteLine("El string de la imagen recibido fue en POST: {0}, {1}", number_images, image);
            byte[] imageBytes = Convert.FromBase64String(image);
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
            return IMGstring[0];
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
