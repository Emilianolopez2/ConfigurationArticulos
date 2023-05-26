using System;

namespace MvcWebPage.Xlsx
{
    /// <summary>
    /// Mark properties which represent xlsx table columns with this attribute
    /// </summary>
    public class XlsxColumnAttribute : Attribute
    {
        public string Name { get; set; }

        // IsMultiple should be true if there is more than one column with this name
        public bool IsMultiple { get; set; }

        public XlsxColumnAttribute()
        {
        }

        public XlsxColumnAttribute(string name)
        {
            Name = name;
        }

        public XlsxColumnAttribute(string name, bool isMultipe)
        {
            Name = name;
            IsMultiple = isMultipe;
        }
    }
}
