using System;
using System.Linq.Expressions;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.RegularExpressions;
using System.Xml;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using MvcWebPage.Attributes;
using Newtonsoft.Json;

namespace MvcWebPage.TagHelpers
{
    public interface IDateTimeInput
     {
          DateTimeInput Create();
          HtmlString GetScript();
          void ToScript();
          DateTimeInput Theme(string value, bool quotes = true);
          DateTimeInput Width(string value, bool quotes = true);
          DateTimeInput Height(string value, bool quotes = true);
          DateTimeInput Style(string value, bool quotes = true);
          DateTimeInput Class(string value);
          DateTimeInput Culture(string value, bool quotes = true);
          DateTimeInput TextAlign(string value, bool quotes = true);
          void SetDate(string value);
          void SetDate(HtmlString value);
     }

     public class DateTimeInput : IDateTimeInput
     {
          private IHtmlHelper HtmlHelper { get; set; }
          private String   id { get; set; }
          private Settings op = new Settings();
          private string   style  { get; set; }
          private string   _class { get; set; }
          public string setDate { get; set; }

          public DateTimeInput()
          {
          }

          public DateTimeInput(IHtmlHelper helper, string id)
          {
               this.HtmlHelper = helper;
               this.id         = id;
               op.height = "'27px'";
          }

          public DateTimeInput Create()
          {
              var htm = new TagBuilder("div");
              htm.MergeAttribute("id", id);
              htm.MergeAttribute("name", id);

              if (!String.IsNullOrEmpty(style))
              {
                  htm.MergeAttribute("style", style);
              }

              if (!String.IsNullOrEmpty(_class))
              {
                  htm.MergeAttribute("class", _class);
              }

              htm.TagRenderMode = TagRenderMode.Normal;
              HtmlHelper.ViewContext.Writer.WriteLine(htm);

              var script1 = string.Format(
                  @"$(""#{0}"").jqxDateTimeInput({1});" + Environment.NewLine,
                  id,
                  op.Json());

              script1.Send(HtmlHelper, "script");


              if (!string.IsNullOrEmpty(setDate))
              {
                  var script2 = string.Format(
                      @"$(""#{0}"").jqxDateTimeInput('val', {1});" + Environment.NewLine,
                      id,
                      setDate);

                  script2.Send(HtmlHelper, "script");
              }

              return this;
          }

          public HtmlString GetScript()
          {
               var script = string.Format(
                         @"$(""#{0}"").jqxDateTimeInput({1});" + Environment.NewLine,
                         id,
                         op.Json());

               return new HtmlString(script);
          }

          public void ToScript()
          {
              GetScript().Send(HtmlHelper, "script");
          }

          public void SetDate(string value)
          {

              if (!string.IsNullOrEmpty(value))
              {
                   var script = string.Format(
                             @"$(""#{0}"").jqxDateTimeInput('val', {1});" + Environment.NewLine,
                             id,
                             value);

                   script.Send(HtmlHelper, "script");
              }
          }

          public void SetDate(HtmlString value)
          {
              if (!string.IsNullOrEmpty(value.ToString()))
              {
                   var script = string.Format(
                             @"$(""#{0}"").jqxDateTimeInput('val', {1});" + Environment.NewLine,
                             id,
                             value.ToString());

                   script.Send(HtmlHelper, "script");
              }
          }

          public DateTimeInput Theme(string value, bool quotes = true)
          {
              op.theme = value.Quotes(quotes);
              return this;
          }

          public DateTimeInput Width(string value, bool quotes = true)
          {
               op.width = value.Quotes(quotes);
               return this;
          }

          public DateTimeInput Height(string value, bool quotes = true)
          {
              op.height = value.Quotes(quotes);
              return this;
          }

          public DateTimeInput Style(string value, bool quotes = true)
          {
              this.style = value.Quotes(quotes);
              return this;
          }

          public DateTimeInput Class(string value)
          {
              this._class = value;
              return this;
          }

          public DateTimeInput Culture(string value, bool quotes = true)
          {
               op.culture = value.Quotes(quotes);
               return this;
          }

          public DateTimeInput TextAlign(string value, bool quotes = true)
          {
               op.textAlign = value.Quotes(quotes);
               return this;
          }

          public partial class Settings
          {
               [JsonConverter(typeof(PlainJson))]
               public string textAlign;

               [JsonConverter(typeof(PlainJson))]
               public string theme { get; set; }

               [JsonConverter(typeof(PlainJson))]
               public string width { get; set; }

               [JsonConverter(typeof(PlainJson))]
               public string height { get; set; }

               [JsonConverter(typeof(PlainJson))]
               public string culture { get; set; }
          }
     }

     public static class jqxDateTimeInput
     {
         public static IDateTimeInput JqxDateTimeInput(this IHtmlHelper helper, string id)
         {
              return new DateTimeInput(helper, id);
          }
     }
}