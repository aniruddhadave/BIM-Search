using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Autodesk.Revit.UI;



namespace ClassLibrary1
{
    public partial class Form2 : Form
    {/// <summary>
        /// The search results
        /// </summary>
        private SortableBindingList<SearchHit> _results = null;
        double cvolume = 0.00, svolume = 0.00;
        int nbeam = 0, ncolumn = 0;
        private SelectionChangedWatcher _selectionChangedWatcher;
        internal Form2(SelectionChangedWatcher selectionChangedWatcher)
        {
            this._selectionChangedWatcher = selectionChangedWatcher;
            InitializeComponent();
            
        }
       public void append(SortableBindingList<SearchHit> results)
       {
           
       }
       public void appendVolume(double svolume,double cvolume,int nbeam,int ncolumn)
       {
           this.cvolume=cvolume;
           this.svolume=svolume;
           this.nbeam = nbeam;
           this.ncolumn = ncolumn;
       }
  

  private void Form2_Load(object sender, EventArgs e)
  {
      
  }

  private void button1_Click(object sender, EventArgs e)
  {
      if (comboBoxKeywords.Text.Contains("Concrete"))
      {
          TaskDialog.Show("Revit", "Total volume of concrete used is "+cvolume +" Cubic meter");
      }
      if (comboBoxKeywords.Text.Contains("Steel"))
      {
          TaskDialog.Show("Revit", "Total volume of steel used is " + svolume + " Cubic meter");
      }
      if (comboBoxKeywords.Text.Contains("Beams"))
      {
          TaskDialog.Show("Revit", "Total no of beams used " + nbeam);
      }
      if (comboBoxKeywords.Text.Contains("Column"))
      {
          TaskDialog.Show("Revit", "Total no. of Columns are " + ncolumn );
      }
  }

    }
}
