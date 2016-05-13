using System;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Markup;

namespace SpeechInfrastructure
{
    [ValueConversion(typeof(string), typeof(FlowDocument))]
    public class FlowToTextDocumentConverter : IValueConverter
    {
        #region IValueConverter Members

        /// <summary>
        /// Converts from TextDocumnent to a WPF FlowDocument.
        /// </summary>
        public object Convert(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            FlowDocument document = null;

            if ( value == null)
            {
                document = new FlowDocument();
                return document;
            }

            TextDocument doc = (TextDocument)value;
            if (doc.Type == TextDocument.TXT)
            {
                Paragraph paragraph = new Paragraph();
                paragraph.Inlines.Add(doc.Text);
                document = new FlowDocument(paragraph);
            }
            else
            {
                document = new FlowDocument();
            }

            return document;
        }

        /// <summary>
        /// Converts from a WPF FlowDocument to a XAML markup string.
        /// need work on plain text part
        /// </summary>
        public object ConvertBack(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            /* This converter does not insert returns or indentation into the XAML. If you need to 
             * indent the XAML in a text box, see http://www.knowdotnet.com/articles/indentxml.html */

            // Exit if FlowDocument is null
            if (value == null) return string.Empty;

            // Get flow document from value passed in
            var flowDocument = (FlowDocument)value;

            // Convert to XAML and return
            return XamlWriter.Save(flowDocument);
        }

        #endregion
    }

}