using Syncfusion.Pdf.Graphics;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Parsing;
using Syncfusion.Windows.PdfViewer;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Linq;
using System.IO;
using System.Windows.Controls.Primitives;

namespace SyncfusionPDFViewer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            AddButton();
            HideVerticalToolbar();
            ExpandAnnotationToolbar();
            HideToolbarItems();
        }

        private void AddButton()
        {
            DocumentToolbar toolbar = pdfViewer.Template.FindName("PART_Toolbar", pdfViewer) as DocumentToolbar;

            // Create the custom unload button.
            Button button = new Button();
            button.Margin = new Thickness(10, 0, 0, 0);
            button.Width = 48;
            button.Height = 24;
            button.Content = "Unload";
            button.Background = System.Windows.Media.Brushes.Transparent;
            button.BorderThickness = new Thickness(0, 0, 0, 0);

            // wire the click event.
            button.Click += Button_Click;

            // Define stackPanel and annotationPanel
            StackPanel stackPanel = (StackPanel)toolbar.Template.FindName("PART_ToolbarStack", toolbar);
            StackPanel annotationPanel = (StackPanel)toolbar.Template.FindName("PART_AnnotationsStack", toolbar);

            // Add button, can choose to add to stackPanel or annotationPanel
            // stackPanel.Children.Add(button);
            annotationPanel.Children.Add(button);
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // Define page and add text programmatically
            PdfLoadedDocument doc = pdfViewer.LoadedDocument;
            PdfLoadedPage page = doc.Pages[0] as PdfLoadedPage;
            PdfGraphics graphics = page.Graphics;
            PdfFont graphicFont = new PdfStandardFont(PdfFontFamily.Helvetica, 20);
            graphics.DrawString("Testing 123", graphicFont, PdfBrushes.Black, new PointF(0, 772));

            // Saves pdf to memory and reloads with change
            // Doesn't save PDF
            MemoryStream stream = new MemoryStream();
            doc.Save(stream);
            doc.Close();
            doc.Dispose();
            pdfViewer.Load(stream);
        }
        private void HideVerticalToolbar()
        {
            // Hides the thumbnail icon. 
            pdfViewer.ThumbnailSettings.IsVisible = false;
            // Hides the bookmark icon. 
            pdfViewer.IsBookmarkEnabled = false;
            // Hides the layer icon. 
            pdfViewer.EnableLayers = false;
            // Hides the organize page icon. 
            pdfViewer.PageOrganizerSettings.IsIconVisible = false;
            // Hides the redaction icon. 
            pdfViewer.EnableRedactionTool = false;
            // Hides the form icon. 
            pdfViewer.FormSettings.IsIconVisible = false;
        }
        private void ExpandAnnotationToolbar()
        {
            // Get the instance of the toolbar using its template name. 
            DocumentToolbar toolbar = pdfViewer.Template.FindName("PART_Toolbar", pdfViewer) as DocumentToolbar;
            // Get the instance of the annotation button using its template name. 
            ToggleButton annotationButton = (ToggleButton)toolbar.Template.FindName("PART_Annotations", toolbar);
            // Expand the annotation toolbar. 
            annotationButton.IsChecked = true;
        }

        private void HideToolbarItems()
        {
            // https://help.syncfusion.com/wpf/pdf-viewer/how-to/disable-toolbar-items
            // the link above lists the names for the buttons that can be disabled.
            DocumentToolbar toolbar = pdfViewer.Template.FindName("PART_Toolbar", pdfViewer) as DocumentToolbar;
            // Example of disabling a button is below.
            // Button textSearchButton = (Button)toolbar.Template.FindName("PART_ButtonTextSearch", toolbar);
            // textSearchButton.Visibility = System.Windows.Visibility.Collapsed;
        }
    }
}
