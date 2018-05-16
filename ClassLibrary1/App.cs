#region Namespaces
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;
using System.Windows.Media.Imaging;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
#endregion
namespace ClassLibrary1
{
    
        class App : IExternalApplication
        {
            /// <summary>
            /// Object that subscribes to the idling event
            /// </summary>
            private SelectionChangedWatcher _selectionChangedWatcher;

            /// <summary>
            /// Implement this method to implement the external application which should be called when 
            /// Revit starts before a file or default template is actually loaded.
            /// </summary>
            /// <param name="application">An object that is passed to the external application which contains the controlled application.</param>
            /// <returns>Return the status of the external application. A result of Succeeded means that the external application successfully started. Cancelled can be used to signify that the user cancelled the external operation at some point. If false is returned then Revit should inform the user that the external application failed to load and release the internal reference.</returns>
            public Result OnStartup(UIControlledApplication application)
            {
                Result ret = Result.Failed;
                Debug.Print("App.OnStartup");
                //Utils.InfoMsg("App.OnStartup");
                // Initialise the global settings
                if (true)
                {
                    Process process = Process.GetCurrentProcess();

                    try
                    {
                        // Typical add in location "C:\ProgramData\Autodesk\ApplicationPlugins\3DIR 3D Information Retrieval.bundle\Contents\Source\ADR_3DIR.2015.dll"
                        // Typical help location   "C:\ProgramData\Autodesk\ApplicationPlugins\3DIR 3D Information Retrieval.bundle\Contents\Resources\3DIRComprehensiveHelp.htm"
                        string sAddInPath = typeof(App).Assembly.Location;
                        string sHelpFile = System.IO.Path.GetDirectoryName(sAddInPath);
                        sHelpFile = System.IO.Path.GetDirectoryName(sHelpFile) + "\\Resources\\3DIRComprehensiveHelp.htm";
                        string sNamespacePrefix = typeof(App).Namespace + ".";
                        // Add a 3DIR ribbon panel with 4 buttons
                        RibbonPanel ribbonPanel = application.CreateRibbonPanel("BIM_search");

                        PushButton pushButton = (PushButton)ribbonPanel.AddItem(new PushButtonData("Structure Archive", "Structure Archive", sAddInPath, sNamespacePrefix + "Lab1PlaceGroup"));
                        pushButton.LargeImage = new BitmapImage(new Uri(@"C:\Users\Tarun Sahu\AppData\Roaming\Autodesk\Revit\Addins\2015\Search-L.png"));
                        pushButton.Image = new BitmapImage(new Uri(@"C:\Users\Tarun Sahu\AppData\Roaming\Autodesk\Revit\Addins\2015\Search-S.png"));
                        pushButton.ToolTip = "Search the building model for related information.";
                        ContextualHelp contextHelp = new ContextualHelp(ContextualHelpType.ChmFile, sHelpFile);
                        pushButton.SetContextualHelp(contextHelp);
                         pushButton = (PushButton)ribbonPanel.AddItem(new PushButtonData("BIM Ask", "BIM Ask", sAddInPath, sNamespacePrefix + "Ask"));
                        pushButton.LargeImage = new BitmapImage(new Uri(@"C:\Users\Tarun Sahu\AppData\Roaming\Autodesk\Revit\Addins\2015\Ask.png", UriKind.RelativeOrAbsolute));
                        pushButton.Image = new BitmapImage(new Uri(@"C:\Users\Tarun Sahu\AppData\Roaming\Autodesk\Revit\Addins\2015\Ask-s.png", UriKind.RelativeOrAbsolute));
                        pushButton.ToolTip = "Search the building model for related information.";
                         contextHelp = new ContextualHelp(ContextualHelpType.ChmFile, sHelpFile);
                        pushButton.SetContextualHelp(contextHelp);

                        //_selectionChangedWatcher.SelectionChanged += new EventHandler(OnSelectionChanged);

                        ret = Result.Succeeded;
                    }
                    catch
                    {

                    }
                }
                return (ret);
            }
            public Result OnShutdown(UIControlledApplication a)
            {
                
                return Result.Succeeded;
            }
        }
   
}
