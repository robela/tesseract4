using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using Ocr.Base.Extract.invoice;
using Ocr.Base.Models;
using Tesseract;

namespace OCR.App
{
    class DoOCR
    {
        public static List<Row> DoOcr2(string imageDir)
        {
           // string imageDir = @"C\test.";


            try
            {
                var rowlist = new List<Row>();
                var tets = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                using (var engine = new TesseractEngine(Path.Combine(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "tessdata"),
                    "eng", EngineMode.TesseractAndLstm))
                {
                    engine.SetVariable("tessedit_char_whitelist", "16.00ABCDEFGHIJKLMNOPQRSTUVWXYZ(quick) brown 1 2 3 4 5 6 7 8 9 0  fox jumps!over the $3,456.78 < lazy >: #90 dog & duck/goose, as 12.5% of Email from aspammer@website.com is spam?");
                    // engine.DefaultPageSegMode = PageSegMode.AutoOsd;
                    Pix pixImage = Pix.LoadFromFile(imageDir);
                    pixImage = pixImage.Deskew();
                    using (var image = new System.Drawing.Bitmap(imageDir))
                    {
                        using (var pix = PixConverter.ToPix(image))
                        {
                            Scew scew;
                            var pixDeskew = pix.Deskew(new ScewSweep(range: 90), Pix.DefaultBinarySearchReduction,
                                Tesseract.Pix.DefaultBinaryThreshold, out scew);

                            using (var page = engine.Process(pix))
                            {

                                var i = 1;
                                var j = 1;
                                var ki = page.GetIterator();
                                using (var iter = page.GetIterator())
                                {
                                    iter.Begin();

                                    do
                                    {
                                        do
                                        {
                                            // Console.WriteLine("in-looop");
                                            do
                                            {
                                                j = 1;
                                                do

                                                {
                                                    Rect symbolBounds;
                                                    if (iter.TryGetBoundingBox(PageIteratorLevel.Word,
                                                        out symbolBounds))
                                                    {

                                                    }
                                                    Identify strType = new Identify();
                                                    rowlist.Add(new Row
                                                    {
                                                        Line = i,
                                                        Colomun = j,
                                                        StringType =
                                                                Convert.ToString(
                                                                    strType.StringType(
                                                                        iter.GetText(PageIteratorLevel.Word))),
                                                        Type =
                                                                Convert.ToInt16(
                                                                    strType.StringType(
                                                                        iter.GetText(PageIteratorLevel.Word))),
                                                        Word = iter.GetText(PageIteratorLevel.Word),
                                                        XCordinateStart = symbolBounds.X1,
                                                        XCordinateEnd = symbolBounds.X2,
                                                        YCordinateEnd = symbolBounds.Y1,
                                                        YCordinateStart = symbolBounds.Y2
                                                    });
                                                    j = j + 1;

                                                } while (iter.Next(PageIteratorLevel.TextLine,
                                                    PageIteratorLevel.Word));


                                                i++;
                                            } while (iter.Next(PageIteratorLevel.Para, PageIteratorLevel.TextLine));
                                        } while (iter.Next(PageIteratorLevel.Block, PageIteratorLevel.Para));
                                    } while (iter.Next(PageIteratorLevel.Block));

                                }

                            }
                        }
                    }
                }
                XmlDocument doc = new XmlDocument();
                //create the serialiser to create the xml
                XmlSerializer serialiser = new XmlSerializer(typeof(List<Row>));
                if (!Directory.Exists(@"C:\TEST\"))
                    Directory.CreateDirectory(@"C:\TEST\");
                // Create the TextWriter for the serialiser to use
                TextWriter filestream = new StreamWriter(@"C:\TEST\" + Path.GetFileNameWithoutExtension(imageDir) + ".xml");

                //write to the file
                serialiser.Serialize(filestream, rowlist);

                // Close the file
                filestream.Close();
                return rowlist;
            }
            catch (Exception ex)
            {

                

                return null;

            }




        }
    }
}
