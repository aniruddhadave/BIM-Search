using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.Attributes;
using System.Windows.Forms;
using System.IO;
namespace ClassLibrary1
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class Ask: IExternalCommand
    {
        public Result Execute(
        ExternalCommandData commandData,
        ref string message,
        ElementSet elements)
        {
           
            UIApplication uiApp = commandData.Application;
           Autodesk.Revit.ApplicationServices.Application app = uiApp.Application;
            UIDocument uiDoc = uiApp.ActiveUIDocument;
            Document doc = uiDoc.Document;
            SortableBindingList<SearchHit> results = new SortableBindingList<SearchHit>();
            ElementClassFilter FamilyInstanceFilter = new ElementClassFilter(typeof(FamilyInstance));
            FilteredElementCollector FamilyInstanceCollector = new FilteredElementCollector(doc);
            ICollection<Element> AllFamilyInstances = FamilyInstanceCollector.WherePasses(FamilyInstanceFilter).ToElements();
            List<ElementId> liIds = new List<ElementId>();
            List<string> ParamLst = new List<string>();
            Autodesk.Revit.DB.View view = doc.ActiveView;
            SelectionChangedWatcher _selectionChangedWatcher=new SelectionChangedWatcher( uiDoc,false);
            FamilySymbol FmlySmbl;
            Family Fmly;
            string s = string.Empty;

            Global.Instance.OurForm2 = new Form2(_selectionChangedWatcher);



            double cvolume = 0.00, svolume = 0.00;
            foreach (FamilyInstance f in AllFamilyInstances)
            {
                FmlySmbl = f.Symbol;
                Fmly = FmlySmbl.Family;
                // Add Instance Parameter names to list
                s += "\n" + f.Name + "\n";
                s += f.Category.Name + "\n";
                s += FmlySmbl.Name + "\n";
                s += Fmly.Name + "\n";
                ElementId elemId = f.Id;
                ICollection<ElementId> materials = f.GetMaterialIds(false);

                foreach (ElementId matId in materials)
                {
                    Material material = doc.GetElement(matId) as Material;
                    s += "Materials = " + material.Name + "\n";

                    s += "Material Volume =" + f.GetMaterialVolume(matId) + "\n";
                    s += "Material Area = " + f.GetMaterialArea(matId, false) + "\n";
                    double volume = f.GetMaterialVolume(matId);
                   volume = Math.Round(volume, 2);
                   double cost = Math.Round(volume * 100, 2);
                       BoundingBoxXYZ bounding = f.get_BoundingBox(null);
    XYZ center = (bounding.Max + bounding.Min) * 0.5;
    string sCoordinates = Math.Round(center.X, 5) + ", " + Math.Round(center.Y, 5) + ", " + Math.Round(center.Z, 5)  ;
    XYZ size = bounding.Max - bounding.Min;
    string sLength = " " +Math.Round( Math.Max(size.X, Math.Max(size.Y, size.Z)),2);

    int nbeams = 0, ncolumns = 0;
    if (material.Name.Contains("Concrete") || material.Name.Contains("Steel"))
    {
        if (material.Name.Contains("Concrete"))
        {
            cvolume += f.GetMaterialVolume(matId);
        }
        if (material.Name.Contains("Steel"))
        {
            svolume += f.GetMaterialVolume(matId);
        }
        
        
                        liIds.Add(f.Id);
                        Global.Instance.OurForm2.appendVolume(Math.Round(svolume,2), Math.Round(cvolume,2),nbeams,ncolumns);
                    }

                }
                // Get FamilyInstance AnalyticalModel type
                if (null != f.GetAnalyticalModel())
                {
                    //  s += "FamilyInstance AnalyticalModel is : " + f.GetAnalyticalModel() + "\n";
                }
                // Get FamilyInstance host name
                if (f.Host != null)
                {
                    s += "FamilyInstance host name is : " + f.Host.Name + "\n";
                }
                
            }
           // string path ="C:\\Users\\Tarun Sahu\\Desktop\\info.txt";
            
            //File.WriteAllText(path,s);
          
            Transaction tx = new Transaction(doc);
            tx.Start("Isolate search selection");
         // Requires transaction
           
           
           

            tx.Commit();
         
      Global.Instance.OurForm2.Show(NativeWindow.FromHandle(Global.Instance.RevitWindowHandle));
       // Global.Instance.OurForm.button3_Click_1();
            return Result.Succeeded;
        }
       
    }

}