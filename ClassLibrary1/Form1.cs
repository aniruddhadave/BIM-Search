using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ClassLibrary1
{
    public partial class Form1 : Form
    {/// <summary>
        /// The search results
        /// </summary>
        private SortableBindingList<SearchHit> _results = null;
        private SelectionChangedWatcher _selectionChangedWatcher;
        internal Form1(SelectionChangedWatcher selectionChangedWatcher)
        {
            this._selectionChangedWatcher = selectionChangedWatcher;
            InitializeComponent();
            dataGridView1.DataSource = _results;
        }
       public void append(SortableBindingList<SearchHit> results)
       {        dataGridView1.SuspendLayout();
				dataGridView1.DataSource = results;
				//if (_results.Count > 0)
				{// Start with no results selected
				dataGridView1.ClearSelection();}
                dataGridView1.Columns[1].Visible=false;
				dataGridView1.ResumeLayout();
                
       }
  private void dataGridView1_SelectionChanged(object sender, EventArgs e)
       {
           int iSelected = dataGridView1.SelectedRows.Count;
          
               if (iSelected > 0)
               {
                   if (true)
                   {// Need to select / isolate / colour the elements in the current view
                       // Collect unique ids of selected rows
                       List<string> liUniqueIdsToSel = new List<string>(iSelected);
                       List<float> liScores = new List<float>(iSelected);
                       List<HitType> liTypes = new List<HitType>(iSelected);
                       for (int i = 0; i < iSelected; i++)
                       {
                           SearchHit hit = (SearchHit)dataGridView1.SelectedRows[i].DataBoundItem;
                           liUniqueIdsToSel.Add(hit.UniqueId);
                          // liScores.Add(hit.Score);
                          // liTypes.Add(hit.HitType);
                       }
                       // Raise appropriate event to select them
                       
                          _selectionChangedWatcher.Select(liUniqueIdsToSel);
                     
                   }
               }
               else
               {// Nothing now selected
                 _selectionChangedWatcher.UndoDisplay();
               }
          
       }
  private void copyAlltoClipboard()
  {
      dataGridView1.SelectAll();
      DataObject dataObj = dataGridView1.GetClipboardContent();
      if (dataObj != null)
          Clipboard.SetDataObject(dataObj);
  }
  public void button3_Click_1(object sender, EventArgs e)
  {
      copyAlltoClipboard();
      Microsoft.Office.Interop.Excel.Application xlexcel;
      Microsoft.Office.Interop.Excel.Workbook xlWorkBook;
      Microsoft.Office.Interop.Excel.Worksheet xlWorkSheet;
      object misValue = System.Reflection.Missing.Value;
      xlexcel = new Microsoft.Office.Interop.Excel.Application();
      xlexcel.Visible = true;
      xlWorkBook = xlexcel.Workbooks.Add(misValue);
      xlWorkSheet = (Microsoft.Office.Interop.Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);
      Microsoft.Office.Interop.Excel.Range CR = (Microsoft.Office.Interop.Excel.Range)xlWorkSheet.Cells[1, 1];
      CR.Select();
      xlWorkSheet.PasteSpecial(CR, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, true);
      dataGridView1.ClearSelection();
  }

  private void Form1_Load(object sender, EventArgs e)
  {

  }

    }
}
