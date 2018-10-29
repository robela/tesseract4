using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OCR.App
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
           
            var testFiles = Directory.EnumerateFiles(@"c:\test\image");
            var maxDegreeOfParallelism = Environment.ProcessorCount;

            Parallel.ForEach(testFiles, new ParallelOptions { MaxDegreeOfParallelism = maxDegreeOfParallelism }, (fileName) =>
            {
                DoOCR.DoOcr2(fileName);
                var imageFile = File.ReadAllBytes(fileName);
               
               
            });
           
        }
    }
}
