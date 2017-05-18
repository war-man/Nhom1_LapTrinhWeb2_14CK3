using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using System.Web.UI;
namespace Petsmart.Controllers
{
    public class ImagesProduct
    {
        public System.Drawing.Image Base64ToImage(string base64String)
        {
            string convert = base64String.Substring(base64String.IndexOf(",") + 1);

            // Convert Base64 String to byte[]
            byte[] imageBytes = Convert.FromBase64String(convert);
            MemoryStream ms = new MemoryStream(imageBytes, 0,
              imageBytes.Length);

            // Convert byte[] to Image
            ms.Write(imageBytes, 0, imageBytes.Length);
            System.Drawing.Image image = System.Drawing.Image.FromStream(ms);
            return image;
        }

        private string nameImage;
        private bool status;
        private string extension;
        private string _image;
        [JsonProperty("_image")]
        public string Image
        {
            get
            {
                return _image;
            }

            set
            {
                _image = value;
            }
        }

        [JsonProperty("_name")]
        public string NameImage
        {
            get
            {
                return nameImage;
            }

            set
            {
                nameImage = value;
            }
        }
        [JsonProperty("_status")]
        public bool Status
        {
            get
            {
                return status;
            }

            set
            {
                status = value;
            }
        }
        [JsonProperty("_extension")]
        public string Extension
        {
            get
            {
                return extension;
            }

            set
            {
                extension = value;
            }
        }
    }
}