using System.Linq.Expressions;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.RegularExpressions;
using System.Xml;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using MvcWebPage.Attributes;
using Newtonsoft.Json;

namespace MvcWebPage.TagHelpers
{

    public interface IGridColumnInternal<TModel> where TModel : class
    {
        string TEXT { get; set; }
        string FIELD { get; set; }
        string TYPE { get; set; }
        string Evaluate(TModel model);
        string cellsrenderer { get; set; }
        string cellclassname { get; set; }
        string validation    { get; set; }
        string renderer      { get; set; }
        string width         { get; set; }
        string cellsformat   { get; set; }
        string columntype    { get; set; }
        string buttonclick   { get; set; }
        bool   visible       { get; set; }
        bool   hidden        { get; set; }
        string editable      { get; set; }
        string cellsalign    { get; set; }
    }


    public interface IGridColumn
    {
        IGridColumn Text(string              value);
        IGridColumn Type(string              value);
        IGridColumn CellsRenderer(string     value);
        IGridColumn CellClassName(string     value);
        IGridColumn Renderer(string          value);
        IGridColumn Renderer(HtmlString      value);
        IGridColumn Width(string             value, bool quotes = true);
        IGridColumn CellsFormat(string       value, bool quotes = true);
        IGridColumn CellsAlign(string        value, bool quotes = true);
        IGridColumn ColumnType(string        value, bool quotes = true);
        IGridColumn ButtonClick(string       value);
        IGridColumn ButtonClick(HtmlString   value);
        IGridColumn ButtonClick(IHtmlContent   value);
        IGridColumn Visible(bool             value);
        IGridColumn Hidden(bool value);
        IGridColumn Editable(bool            value);
        IGridColumn CellsRenderer(IHtmlContent value);
        IGridColumn Validation(string value);

    }


    public class GridColumn<TModel, TProperty> : IGridColumn, IGridColumnInternal<TModel> where TModel : class
    {


        public string TEXT          { get; set; }
        public string FIELD         { get; set; }
        public string TYPE          { get; set; }
        public string cellsrenderer { get; set; }
        public string validation    { get; set; }
        public string cellclassname { get; set; }
        public string renderer      { get; set; }
        public string rendered      { get; set; }
        public string width         { get; set; }
        public string cellsformat   { get; set; }
        public string cellsalign    { get; set; }
        public string columntype    { get; set; }
        public string buttonclick   { get; set; }
        public bool   visible       { get; set; }
        public bool   hidden        { get; set; }
        public string editable      { get; set; }

        public Func<TModel, TProperty> CompiledExpression { get; set; }

        public GridColumn(Expression<Func<TModel, TProperty>> expression)
        {
            var exp = expression.Body as MemberExpression;

            if (exp != null)
            {
                object[] attribute = exp.Member.GetCustomAttributes(typeof(GridColumnAttribute), true);

                if (attribute.Length > 0)
                {
                    GridColumnAttribute myAttribute = (GridColumnAttribute)attribute[0];
                    TEXT = myAttribute.TEXT.Quotes();
                }

                string propertyName = exp.Member.Name;
                this.FIELD = propertyName.Quotes();
                //this.FIELD = Regex.Replace(propertyName, "([a-z])([A-Z])", "$1 $2").Quotes();

                this.CompiledExpression = expression.Compile();
            }
            else
            {
                this.FIELD = expression.Compile().Invoke(null).ToString().Quotes();
            }
        }

        public IGridColumn ColumnType(string value, bool quotes = true)
        {
            this.columntype = value.Quotes(quotes);
            return this;
        }

        public IGridColumn ButtonClick(string value)
        {
            this.buttonclick = value;
            return this;
        }

        public IGridColumn Visible(bool value)
        {
            this.visible = value;
            return this;
        }

        public IGridColumn Hidden(bool value)
        {
            this.hidden = value;
            return this;
        }
        public IGridColumn Editable(bool value)
        {
            this.editable = value.ToString().ToLower();
            return this;
        }


        public IGridColumn ButtonClick(HtmlString value)
        {
            this.buttonclick = value.Str();
            return this;
        }

        public IGridColumn ButtonClick(IHtmlContent value)
        {
            this.buttonclick = value.Str();
            return this;
        }

        public IGridColumn CellsFormat(string value, bool quotes = true)
        {
            this.cellsformat = value.Quotes(quotes);
            return this;
        }

        public IGridColumn CellsAlign(string value, bool quotes = true)
        {
            this.cellsalign = value.Quotes(quotes);
            return this;
        }

        public IGridColumn Width(string value, bool quotes = true)
        {
            this.width = value.Quotes(quotes);
            return this;
        }

        public IGridColumn Text(string value)
        {
            this.TEXT = value.Quotes();
            return this;
        }





        public IGridColumn CellsRenderer(string value)
        {
            this.cellsrenderer = value;
            return this;
        }
        public IGridColumn Validation(string value)
        {
            this.validation = value;
            return this;
        }

        public IGridColumn CellClassName(string value)
        {
            this.cellclassname = value;
            return this;
        }

        public IGridColumn CellsRenderer(IHtmlContent value)
        {
            this.cellsrenderer = value.Str();
            return this;
        }

        public IGridColumn Renderer(string value)
        {
            this.renderer = value;
            return this;
        }

        public IGridColumn Renderer(HtmlString value)
        {
            this.renderer = value.Str();
            return this;
        }

        public IGridColumn Type(string value)
        {
            this.TYPE = value.Quotes(); ;
            return this;
        }

        public string Evaluate(TModel model)
        {
            var result = this.CompiledExpression(model);
            return result == null ? string.Empty : result.ToString();
        }
    }

    public class Column<TModel> where TModel : class
    {
        public Grid<TModel> Grid { get; set; }

        public Column(Grid<TModel> grid)
        {
            Grid = grid;
        }

        public IGridColumn Add<TProperty>(Expression<Func<TModel, TProperty>> expression)
        {
            return Grid.AddColumn(expression);
        }


    }

    public interface IGrid<TModel> where TModel : class
    {
        //Grid<TModel> DataSource(IEnumerable<TModel> dataSource);
        Grid<TModel> Columns(Action<Column<TModel>> gridColumn);
        Grid<TModel> ColumnsHeight(string           value);
        Grid<TModel> RowsHeight(string              value);
        Grid<TModel> ScrollbarSize(string           value);
        Grid<TModel> AutoWidth(bool                 value);
        Grid<TModel> Width(string                   value, bool quotes = true);
        Grid<TModel> Height(string                  value, bool quotes = true);
        Grid<TModel> EnableHover(bool               value);
        Grid<TModel> Editable(bool               value);
        Grid<TModel> SelectionMode(string           value);
        Grid<TModel> EditMode(string           value);
        Grid<TModel> EnableBrowserSelection(bool    value);
        Grid<TModel> Sortable(bool                  value);
        Grid<TModel> ShowSortMenuItems(bool         value);
        Grid<TModel> UpdateRow(string         value);
        Grid<TModel> ShowFilterRow(bool         value);
        Grid<TModel> Filterable(bool         value);
        Grid<TModel> Rendered(string                value);
        Grid<TModel> Renderer(string                value);
        Grid<TModel> Renderer(IHtmlContent value);
        Grid<TModel> CellsRenderer(string           value);
        Grid<TModel> CellsRenderer(HtmlString       value);
        //Grid<TModel> MarginTop(string value, bool quotes = true);
        Grid<TModel> Ready(string value);
        Grid<TModel> Create();
        void DataSource(IEnumerable<TModel> dataSource);
        HtmlString SetSource(string         value);
        HtmlString Clear();
        HtmlString AutoResizeColumns();
    }


    public class Grid<TModel> : IGrid<TModel> where TModel : class
    {
        private IHtmlHelper HtmlHelper { get; set; }
        //private IEnumerable<TModel> Data { get; set; }
        private String id            { get; set; }
        private String renderer      { get; set; }
        private String cellsrenderer { get; set; }
        private String updaterow     { get; set; }
        private string ready         { get; set; }

        Settings op = new Settings();
        //Css css = new Css();

        private Grid()
        {
        }

        internal Grid(IHtmlHelper helper, string id)
        {
            this.HtmlHelper = helper;
            this.id = id;
            op.columnsheight = "25";
            op.rowsheight = "26";
            op.scrollbarsize = "15";

            this.GridColumns = new List<IGridColumnInternal<TModel>>();
        }

        /*        public Grid<TModel> DataSource(IEnumerable<TModel> dataSource)
                {
                     this.Data = dataSource;
                     return this;
                }*/
        /*
        public Grid<TModel> MarginTop(string value, bool quotes = true)
        {
             css.marginTop = value.Quotes(quotes);
              return this;
         }*/


        public HtmlString Clear()
        {
            var it1 = "if( s_" + id + ".localdata.length > 0 ){" + Environment.NewLine;
            var it2 = "r_" + id + " = false;";
            var it3 = "s_" + id + ".localdata = [];" + Environment.NewLine;
            var it4 = "a_" + id + ".dataBind();" + Environment.NewLine;
            var it5 = "$(\"#" + id + "\").jqxGrid('clearselection');" + Environment.NewLine;

            var it7 = "}";

            return new HtmlString(it1 + it2 + it3 + it4 + it5 + it7);
        }

        public HtmlString AutoResizeColumns()
        {
            StringBuilder st = new StringBuilder();

            st.AppendLine("if (!r_" + id + "){");
            st.AppendLine("r_" + id + " = true;");
            st.AppendLine("return;");
            st.AppendLine("}");

            st.AppendLine("$(\"#" + id + "\").jqxGrid(\"autoresizecolumns\");");

            return new HtmlString(st.ToString());
        }

        public HtmlString SetSource(string value)
        {

            var it1 = "s_" + id + ".localdata = " + value + ";" + Environment.NewLine;
            var it2 = "a_" + id + ".dataBind();" + Environment.NewLine;
            var it3 = "$(\"#" + id + "\").jqxGrid('clearselection');" + Environment.NewLine;

            return new HtmlString(it1 + it2 + it3);
        }

        public Grid<TModel> Renderer(String value)
        {
            this.renderer = value;
            return this;
        }
        

        public Grid<TModel> Renderer(IHtmlContent value)
        {

            this.renderer = value.Str();
            return this;
        }

        public Grid<TModel> CellsRenderer(String value)
        {
            this.cellsrenderer = value;
            return this;
        }

        public Grid<TModel> CellsRenderer(HtmlString value)
        {
            this.cellsrenderer = value.Str();

            return this;
        }


        public Grid<TModel> Ready(string value)
        {
            op.ready = value;
            return this;
        }

        public Grid<TModel> UpdateRow(string value)
        {
            this.updaterow = value;
            return this;
        }

        public Grid<TModel> ShowFilterRow(bool value)
        {
            op.showfilterrow = value.ToString().ToLower();
            return this;
        }

        public Grid<TModel> Filterable(bool value)
        {
            op.filterable = value.ToString().ToLower();
            return this;
        }

        public Grid<TModel> Rendered(String value)
        {
            op.rendered = value;
            return this;
        }

        public Grid<TModel> AutoWidth(bool value)
        {
            op.autowidth = value.ToString().Quotes();
            return this;
        }

        public Grid<TModel> Width(String value, bool quotes = true)
        {
            op.width = value.Quotes(quotes);
            return this;
        }

        public Grid<TModel> Height(String value, bool quotes = true)
        {
            op.height = value.Quotes(quotes);
            return this;
        }

        public Grid<TModel> EnableBrowserSelection(bool value)
        {
            op.enablebrowserselection = value.ToString().ToLower();
            return this;
        }

        public Grid<TModel> EnableHover(bool value)
        {
            op.enableHover = value.ToString().ToLower();
            return this;
        }
        public Grid<TModel> Editable(bool value)
        {
            op.editable = value.ToString().ToLower();
            return this;
        }

        public Grid<TModel> SelectionMode(string value)
        {
            op.selectionmode = value.Quotes();
            return this;
        }
        public Grid<TModel> EditMode(string value)
        {
            op.editmode = value.Quotes();
            return this;
        }

        public Grid<TModel> Sortable(bool value)
        {
            op.sortable = value.ToString().ToLower();
            return this;
        }

        public Grid<TModel> ShowSortMenuItems(bool value)
        {
            op.showsortmenuitems = value.ToString().ToLower();
            return this;
        }

        public Grid<TModel> ColumnsHeight(String value)
        {
            op.columnsheight = value;
            return this;
        }

        public Grid<TModel> RowsHeight(String value)
        {
            op.rowsheight = value;
            return this;
        }

        public Grid<TModel> ScrollbarSize(String value)
        {
            op.scrollbarsize = value;
            return this;
        }

        internal IList<IGridColumnInternal<TModel>> GridColumns { get; set; }

        internal IGridColumn AddColumn<TProperty>(Expression<Func<TModel, TProperty>> expression)
        {
            GridColumn<TModel, TProperty> column = new GridColumn<TModel, TProperty>(expression);

            column.visible = true;
            column.cellsrenderer = cellsrenderer;
            column.renderer = renderer;

            this.GridColumns.Add(column);
            return column;
        }


        public Grid<TModel> Columns(Action<Column<TModel>> gridColumn)
        {
            Column<TModel> builder = new Column<TModel>(this);
            gridColumn(builder);
            return this;
        }

        public Grid<TModel> Create()
        {
            var htm = new TagBuilder("div");
            htm.MergeAttribute("id", id);
            htm.TagRenderMode = TagRenderMode.Normal;

            HtmlHelper.ViewContext.Writer.WriteLine(htm);

            Source source = new Source();

            source.datatype  = "\"json\"";
            source.localdata = "[]";

            if (!string.IsNullOrEmpty(this.updaterow))
            {
                source.updaterow = updaterow;
            }
            
            

            foreach (IGridColumnInternal<TModel> tc in this.GridColumns)
            {
                var dataField = new DataField();
                dataField.name = tc.FIELD;
                dataField.type = tc.TYPE;

                source.datafields.Add(dataField);

                if (tc.visible)
                {
                    op.columns.Add(
                              new columns
                              {
                                  text          = tc.TEXT,
                                  dataField     = tc.FIELD,
                                  cellsrenderer = tc.cellsrenderer,
                                  cellclassname = tc.cellclassname,
                                  validation    = tc.validation,
                                  renderer      = tc.renderer,
                                  width         = tc.width,
                                  cellsformat   = tc.cellsformat,
                                  cellsalign    = tc.cellsalign,
                                  columntype    = tc.columntype,
                                  buttonclick   = tc.buttonclick,
                                  editable      =tc.editable,
                                  hidden        = tc.hidden,

                              });
                }
            }


            op.source = "a_" + id;
            
            var it1 = "var s_" + id + " = " + source.Json() + ";";
            it1.Send(HtmlHelper, "script");


            var it2 = "var a_" + id + " = new $.jqx.dataAdapter(s_" + id + ");";
            it2.Send(HtmlHelper, "script");

            var it3 = "$(\"#" + id + "\").jqxGrid(" + op.Json() + ");";
            it3.Send(HtmlHelper, "script");



            var it4 = "var r_" + id + " = true;";
            it4.Send(HtmlHelper, "script");
            


            var it6 =
@"#content" + id + @" > span {
         visibility: hidden !important;
    }";

            it6.Send(HtmlHelper, "style");



            return this;
        }



        public void DataSource(IEnumerable<TModel> dataSource)
        {

            if (dataSource != null)
            {
                var d = dataSource.Json();

                
                var it4 = "s_" + id + ".localdata = " + d + ";" + Environment.NewLine;
                it4.Send(HtmlHelper, "script");

                var it5 = "a_" + id + ".dataBind();" + Environment.NewLine;
                it5.Send(HtmlHelper, "script");

                var it6 = "$(\"#" + id + "\").jqxGrid('clearselection');" + Environment.NewLine;
                it6.Send(HtmlHelper, "script");
            }
        }

        public partial class Source
        {
            [JsonConverter(typeof(PlainJson))]
            public string updaterow { get; set; }
            [JsonConverter(typeof(PlainJson))]

            public string datatype { get; set; }

            public List<DataField> datafields = new List<DataField>();

            [JsonConverter(typeof(PlainJson))]
            public string localdata { get; set; }
        }

        public partial class DataField
        {
            [JsonConverter(typeof(PlainJson))]
            public string name { get; set; }

            [JsonConverter(typeof(PlainJson))]
            public string type { get; set; }
        }

        public class columns
        {
            [JsonConverter(typeof(PlainJson))]
            public string text { get; set; }

            [JsonConverter(typeof(PlainJson))]
            public string dataField { get; set; }

            [JsonConverter(typeof(PlainJson))]
            public string cellsrenderer { get; set; }

            [JsonConverter(typeof(PlainJson))]
            public string validation { get; set; }

            [JsonConverter(typeof(PlainJson))]
            public string cellclassname { get; set; }

            [JsonConverter(typeof(PlainJson))]
            public string renderer { get; set; }


            [JsonConverter(typeof(PlainJson))]
            public string width { get; set; }

            [JsonConverter(typeof(PlainJson))]
            public string cellsformat { get; set; }

            [JsonConverter(typeof(PlainJson))]
            public string cellsalign { get; set; }

            [JsonConverter(typeof(PlainJson))]
            public string columntype { get; set; }

            [JsonConverter(typeof(PlainJson))]
            public string buttonclick { get; set; }

            [JsonConverter(typeof(PlainJson))]
            public bool hidden { get; set; }

            [JsonConverter(typeof(PlainJson))]
            public string editable { get; set; }
        }

        public partial class Settings
        {
            [JsonConverter(typeof(PlainJson))]
            public string source { get; set; }

            [JsonConverter(typeof(PlainJson))]
            public string columnsheight { get; set; }

            [JsonConverter(typeof(PlainJson))]
            public string rowsheight { get; set; }

            [JsonConverter(typeof(PlainJson))]
            public string scrollbarsize { get; set; }

            [JsonConverter(typeof(PlainJson))]
            public string height { get; set; }

            [JsonConverter(typeof(PlainJson))]
            public string width { get; set; }

            [JsonConverter(typeof(PlainJson))]
            public string ready { get; set; }

            [JsonConverter(typeof(PlainJson))]
            public string enablebrowserselection { get; set; }

            [JsonConverter(typeof(PlainJson))]
            public string autowidth { get; set; }

            [JsonConverter(typeof(PlainJson))]
            public string selectionmode { get; set; }

            [JsonConverter(typeof(PlainJson))]
            public string editmode { get; set; }

            [JsonConverter(typeof(PlainJson))]
            public string enableHover { get; set; }

            [JsonConverter(typeof(PlainJson))]
            public string editable { get; set; }

            [JsonConverter(typeof(PlainJson))]
            public string sortable { get; set; }

            [JsonConverter(typeof(PlainJson))]
            public string showsortmenuitems { get; set; }

            [JsonConverter(typeof(PlainJson))]
            public string rendered { get; set; }

            [JsonConverter(typeof(PlainJson))]
            public string showfilterrow { get; set; }

            [JsonConverter(typeof(PlainJson))]
            public string filterable { get; set; }

            public List<columns> columns = new List<columns>();
        }

        public partial class Css
        {
            [JsonProperty("margin-top")]
            [JsonConverter(typeof(PlainJson))]
            public string marginTop { get; set; }

        }

        public HtmlString ToHtml()
        {

            XmlDocument html = new XmlDocument();
            XmlElement table = html.CreateElement("table");
            html.AppendChild(table);
            table.SetAttribute("border", "1px");
            table.SetAttribute("cellpadding", "5px");
            table.SetAttribute("cellspacing", "0px");
            XmlElement thead = html.CreateElement("thead");
            table.AppendChild(thead);
            XmlElement tr = html.CreateElement("tr");
            thead.AppendChild(tr);

            foreach (IGridColumnInternal<TModel> tc in this.GridColumns)
            {
                XmlElement td = html.CreateElement("td");
                td.SetAttribute("style", "background-color:Black; color:White;font-weight:bold;");

                if (!string.IsNullOrEmpty(tc.TEXT))
                {
                    td.InnerText = tc.TEXT;
                }
                else
                {
                    td.InnerText = tc.FIELD;
                }

                tr.AppendChild(td);
            }

            XmlElement tbody = html.CreateElement("tbody");
            table.AppendChild(tbody);

            int row = 0;
            /*            if (this.Data != null)
                        {
                            foreach (TModel model in this.Data)
                            {
                                tr = html.CreateElement("tr");
                                tbody.AppendChild(tr);

                                foreach (IGridColumnInternal<TModel> tc in this.GridColumns)
                                {
                                    XmlElement td = html.CreateElement("td");
                                    td.InnerText = tc.Evaluate(model);
                                    tr.AppendChild(td);
                                }
                                row++;
                            }
                        }*/

            return new HtmlString(id + " <br>" + html.OuterXml);
        }

    }



    public static class jqxGrid
    {
        public static IGrid<TModel> JqxGrid<TModel>(this IHtmlHelper helper, string id) where TModel : class
        {
            return new Grid<TModel>(helper, id);
        }
    }


    /*
    public static class CustomHtmlHelpers
    {

        
        public static IHtmlContent SubmitButton(this IHtmlHelper htmlHelper, string id)
        {
            string str = "<input type='submit' />";
            return new HtmlString(str);
        }
        

    }*/
}
