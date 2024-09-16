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
using Syncfusion.Pdf.Redaction;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Color = System.Drawing.Color;

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
            RedactButton();
            AddTextButton();
            FindButton();
            HideVerticalToolbar();
            ExpandAnnotationToolbar();
            HideToolbarItems();
            AddStamps();
        }
        private void AddTextButton()
        {
            DocumentToolbar toolbar = pdfViewer.Template.FindName("PART_Toolbar", pdfViewer) as DocumentToolbar;

            // Create the custom unload button.
            Button button = new Button();
            button.Margin = new Thickness(10, 0, 0, 0);
            button.Width = 60;
            button.Height = 24;
            button.Content = "Load Text";
            button.Background = System.Windows.Media.Brushes.Transparent;
            button.BorderThickness = new Thickness(0, 0, 0, 0);

            // wire the click event.
            button.Click += Add_Text_Button_Click;

            // Define stackPanel and annotationPanel
            // StackPanel stackPanel = (StackPanel)toolbar.Template.FindName("PART_ToolbarStack", toolbar);
            StackPanel annotationPanel = (StackPanel)toolbar.Template.FindName("PART_AnnotationsStack", toolbar);

            // Add button, can choose to add to stackPanel or annotationPanel
            // stackPanel.Children.Add(button);
            annotationPanel.Children.Add(button);
        }
        private void Add_Text_Button_Click(object sender, RoutedEventArgs e)
        {
            // Define page and add text programmatically
            PdfLoadedDocument doc = pdfViewer.LoadedDocument;
            PdfLoadedPage page = doc.Pages[0] as PdfLoadedPage;
            PdfGraphics graphics = page.Graphics;
            PdfFont graphicFont = new PdfStandardFont(PdfFontFamily.Helvetica, 20);
            graphics.DrawString("Test Text", graphicFont, PdfBrushes.Black, new PointF(0, 772));

            // Saves pdf to memory and reloads with change
            // Doesn't save PDF
            MemoryStream stream = new MemoryStream();
            doc.Save(stream);
            doc.Close();
            doc.Dispose();
            pdfViewer.Load(stream);
        }
        private void RedactButton()
        {
            DocumentToolbar toolbar = pdfViewer.Template.FindName("PART_Toolbar", pdfViewer) as DocumentToolbar;

            // Create the custom unload button.
            Button button = new Button();
            button.Margin = new Thickness(10, 0, 0, 0);
            button.Width = 60;
            button.Height = 24;
            button.Content = "Redact";
            button.Background = System.Windows.Media.Brushes.Transparent;
            button.BorderThickness = new Thickness(0, 0, 0, 0);

            button.Click += Redact_Button_Click;

            StackPanel annotationPanel = (StackPanel)toolbar.Template.FindName("PART_AnnotationsStack", toolbar);
            annotationPanel.Children.Add(button);
        }
        private void Redact_Button_Click(object sender, RoutedEventArgs e)
        {
            pdfViewer.PageRedactor.EnableRedactionMode = true;
            pdfViewer.RedactionSettings.FillColor = Colors.White;
            pdfViewer.FreeTextAnnotationSettings.BorderColor = Colors.White;
        }
        // Find these stamps under the stamp icon >> 'Standard Business' icon
        private void AddStamps()
        {
            // To add new stamps, add to the folder 'Stamps' and add file name to folder.
            string[] files =
            {
                "Axis.png",
                "CArrow.png",
                "LArrow.png",
                "RArrow.png",
                "Table.png",
                "UArrow.png",
            };

            foreach (string file in files) 
            {
                System.Windows.Controls.Image image = new System.Windows.Controls.Image();
                image.Source = new BitmapImage(new Uri(@"..\..\Stamps\" + file, UriKind.RelativeOrAbsolute));
                PdfStampAnnotation newStandardStamp = new PdfStampAnnotation(image);
                pdfViewer.ToolbarSettings.StampAnnotations.Add(newStandardStamp);

            }
        }
        private void FindButton()
        {
            DocumentToolbar toolbar = pdfViewer.Template.FindName("PART_Toolbar", pdfViewer) as DocumentToolbar;

            // Create the custom unload button.
            Button button = new Button();
            button.Margin = new Thickness(10, 0, 0, 0);
            button.Width = 48;
            button.Height = 24;
            button.Content = "Find";
            button.Background = System.Windows.Media.Brushes.Transparent;
            button.BorderThickness = new Thickness(0, 0, 0, 0);

            button.Click += Find_Button_Click;

            StackPanel annotationPanel = (StackPanel)toolbar.Template.FindName("PART_AnnotationsStack", toolbar);
            annotationPanel.Children.Add(button);
        }

        // This is currently tries to find and replace text. Unfortunately, it doesn't always work.
        // 1) It doesn't always find the text.
        // 2) The length of the old and new text needs to similar for a clean redaction.
        private void Find_Button_Click(object sender, RoutedEventArgs e)
        {
            PdfLoadedDocument doc = pdfViewer.LoadedDocument;
            PdfLoadedPage data = doc.Pages[0] as PdfLoadedPage;
            Dictionary<int, List<RectangleF>> matchedTextbounds = new Dictionary<int, List<RectangleF>>();

            string text = "Hello";
            pdfViewer.FindText(text, out matchedTextbounds);

            // Iterate through the matchedTextbounds dictionary.
            foreach (KeyValuePair<int, List<RectangleF>> matchedText in matchedTextbounds)
            {
                //Create a font to draw the replacement text.
                PdfFont font = new PdfStandardFont(PdfFontFamily.Helvetica, 12);
                int pageIndex = matchedText.Key; // Get the page index
                List<RectangleF> rectangles = matchedText.Value;
                // Access the page using the page index
                PdfLoadedPage page = doc.Pages[pageIndex] as PdfLoadedPage;

                // Now, you can work with the page as needed.
                for (int j = 0; j < rectangles.Count; j++)
                {
                    //Create a PDF redaction and set the fill color as transparent.
                    PdfRedaction redaction = new PdfRedaction(rectangles[j], Color.Transparent);
                    //Draw the replacement text on the redaction appearance.
                    redaction.Appearance.Graphics.DrawString("New Text", font, PdfBrushes.Black, PointF.Empty);
                    page.AddRedaction(redaction);
                    // page.Graphics.DrawRectangle(PdfBrushes.White, rectangles[j]);
                }
            }

            doc.Redact();

            // Saves pdf to memory and reloads with change
            // Doesn't save PDF
            MemoryStream stream = new MemoryStream();
            doc.Save(stream);
            doc.Close();
            doc.Dispose();
            pdfViewer.Load(stream);

            //TESTING
            //Get the occurrences of the target text and location.
            //Highlights in UI. Currently unable to access programatically.
            //pdfViewer.SearchText("TEST");
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
