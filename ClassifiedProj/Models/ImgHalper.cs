using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ClassifiedProj.Models
{
    public class ImgHalper
    {
        public static IHtmlString DisplayImage(byte[] image)
        {
            if (image == null)
                return null;

            TagBuilder tagBuilder = new TagBuilder("img");

            var imgUrl = Convert.ToBase64String(image);
            var imgSrc = String.Format("data:image;base64,{0}", imgUrl);
            tagBuilder.Attributes.Add("src", imgSrc);

            return new MvcHtmlString(tagBuilder.ToString(TagRenderMode.SelfClosing));
        }

        public static IHtmlString DisplayImage(byte[] image, string width, string height)
        {
            if (image == null)
                return null;

            TagBuilder tagBuilder = new TagBuilder("img");

            var imgUrl = Convert.ToBase64String(image);
            var imgSrc = String.Format("data:image;base64,{0}", imgUrl);
            tagBuilder.Attributes.Add("src", imgSrc);
            tagBuilder.Attributes.Add("width", width);
            tagBuilder.Attributes.Add("height", height);

            return new MvcHtmlString(tagBuilder.ToString(TagRenderMode.SelfClosing));
        }

        public static IHtmlString DisplayImage(byte[] image, string @class)
        {
            if (image == null)
                return null;

            TagBuilder tagBuilder = new TagBuilder("img");

            var imgUrl = Convert.ToBase64String(image);
            var imgSrc = String.Format("data:image;base64,{0}", imgUrl);
            tagBuilder.Attributes.Add("src", imgSrc);
            tagBuilder.AddCssClass(@class);


            return new MvcHtmlString(tagBuilder.ToString(TagRenderMode.SelfClosing));
        }
    }
}