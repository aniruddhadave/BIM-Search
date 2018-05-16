using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary1
{
    internal class SelectionChangedWatcher
    {
        internal event EventHandler SelectionChanged;
        internal event EventHandler PointPicked;
        internal event EventHandler SphereFiltered;
        internal event EventHandler HopCompleted;
        internal event EventHandler ViewActivated;

        #region Enums
        /// <summary>
        /// Revit view type
        /// </summary>
        internal enum ViewType
        {
            ViewUnknown = 0,
            View3D,
            ViewSection,
            ViewSheet,
            ViewDrafting,
            TableView,
            ViewPlan
        }

        /// <summary>
        /// How to select / highlight the elements
        /// </summary>
        internal enum SelectMode
        {
            Nothing = 0,
            Select,
            Isolate,
            Colour,
            Undo
        }

        /// <summary>
        /// Operation mode for next idling event
        /// </summary>
        internal enum OpMode
        {
            Default = 0,
            FilterSphereSearchHits,	// Filter the search hits by a sphere
            FilterSphereAll,				// Filter all elements by a sphere
            PickPoint,							// Allow user to pick a point
            SelectElements,					// Select elements using selectmode
            UndoSelection,					// Undo previous selection
            HopToConnectedElements	// Hop to connected elements
        }

        #endregion Enums

        private UIDocument _uiApp = null;
        /// <summary>
        /// The Revit UI application
        /// </summary>

        private List<string> _liUniqueIds = null;
        /// <summary>
        /// List of selected elements
        /// </summary>
        private List<Element> _liSelection = null;

        /// <summary>
        /// List of unique ids to be selected programmatically
        /// </summary>
        private List<string> _liUniqueIdsToSel = null;
        /// <summary>
        /// Operation mode for the next idle event
        /// </summary>
        private OpMode _opMode = OpMode.Default;

        /// <summary>
        /// What to do with the selection on the next idle event
        /// </summary>
        private SelectMode _selectionMode = SelectMode.Nothing;
        internal void Select(List<string> liUniqueIdsToSel)
        {
            _liUniqueIdsToSel = liUniqueIdsToSel;
            SelectElements(_uiApp);
        }

        internal void UndoDisplay()
        {

            _selectionMode = SelectMode.Undo;
        }
        internal SelectionChangedWatcher(UIDocument uiApp, bool bStart)
        {
            _uiApp = uiApp;
            _liSelection = new List<Element>();
            //_liLastSelIds = new List<int>();
            _liUniqueIds = new List<string>();

        }
        /// <summary>
        /// Selects the search hit elements in the view
        /// </summary>
        /// <param name="activeUIDoc"></param>
        private void SelectElements(UIDocument activeUIDoc)
        {
            ICollection<ElementId> newCollection = new List<ElementId>();
            _liSelection.Clear();
            _liUniqueIds.Clear();
            foreach (string sUniqueId in _liUniqueIdsToSel)
            {
                Element elem = activeUIDoc.Document.GetElement(sUniqueId);
                if (null != elem)
                {
                    newCollection.Add(elem.Id);
                    _liSelection.Add(elem);
                    _liUniqueIds.Add(sUniqueId);
                }
            }
            activeUIDoc.Selection.SetElementIds(newCollection);
            //uiApp.ActiveUIDocument.Document.Regenerate();
            activeUIDoc.ShowElements(newCollection);
            activeUIDoc.RefreshActiveView();
        }
    }
}
