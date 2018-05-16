/***************************************************************************
	Module Name:  Utils.cs

//Copyright 2011 Adris Computing Concepts Ltd.     $Author: BrianP $

//$Date: 21 February 2014 15:30:49 $  $Revision: 1.2 $  $Version: NONE $

// Utility class / helper functions.

/**************************************************************************/
#region Namespaces
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Media.Imaging;
using WinForms = System.Windows.Forms;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System.Drawing;
#endregion

namespace ClassLibrary1
{
    class Utils
    {
        #region Geometrical Comparison
        const double _eps = 1.0e-9;

        public static double Eps
        {
            get
            {
                return _eps;
            }
        }

        public static double MinLineLength
        {
            get
            {
                return _eps;
            }
        }

        public static double TolPointOnPlane
        {
            get
            {
                return _eps;
            }
        }

        public static bool IsZero(double a, double tolerance)
        {
            return tolerance > Math.Abs(a);
        }

        public static bool IsZero(double a)
        {
            return IsZero(a, _eps);
        }

        public static bool IsEqual(double a, double b)
        {
            return IsZero(b - a);
        }

        public static int Compare(double a, double b)
        {
            return IsEqual(a, b) ? 0 : (a < b ? -1 : 1);
        }

        public static int Compare(XYZ p, XYZ q)
        {
            int diff = Compare(p.X, q.X);
            if (0 == diff)
            {
                diff = Compare(p.Y, q.Y);
                if (0 == diff)
                {
                    diff = Compare(p.Z, q.Z);
                }
            }
            return diff;
        }

        public static bool IsEqual(XYZ p, XYZ q)
        {
            return 0 == Compare(p, q);
        }

        public static bool IsParallel(XYZ p, XYZ q)
        {
            return p.CrossProduct(q).IsZeroLength();
        }

        public static bool IsHorizontal(XYZ v)
        {
            return IsZero(v.Z);
        }

        public static bool IsVertical(XYZ v)
        {
            return IsZero(v.X) && IsZero(v.Y);
        }

        public static bool IsVertical(XYZ v, double tolerance)
        {
            return IsZero(v.X, tolerance)
                && IsZero(v.Y, tolerance);
        }

        public static bool IsHorizontal(Edge e)
        {
            XYZ p = e.Evaluate(0);
            XYZ q = e.Evaluate(1);
            return IsHorizontal(q - p);
        }

        public static bool IsHorizontal(PlanarFace f)
        {
            return IsVertical(f.Normal);
        }

        public static bool IsVertical(PlanarFace f)
        {
            return IsHorizontal(f.Normal);
        }

        public static bool IsVertical(CylindricalFace f)
        {
            return IsVertical(f.Axis);
        }

        /// <summary>
        /// Return the maximum value from an array of real numbers.
        /// </summary>
        public static double Max(double[] a)
        {
            Debug.Assert(1 == a.Rank, "expected one-dimensional array");
            Debug.Assert(0 == a.GetLowerBound(0), "expected zero-based array");
            Debug.Assert(0 < a.GetUpperBound(0), "expected non-empty array");
            double max = a[0];
            for (int i = 1; i <= a.GetUpperBound(0); ++i)
            {
                if (max < a[i])
                {
                    max = a[i];
                }
            }
            return max;
        }

        /// <summary>
        /// Return the midpoint between two points.
        /// </summary>
        public static XYZ Midpoint(XYZ p, XYZ q)
        {
            return p + 0.5 * (q - p);
        }

        /// <summary>
        /// Return the midpoint of a Line.
        /// </summary>
        public static XYZ Midpoint(Line line)
        {
            return Midpoint(line.GetEndPoint(0),
                line.GetEndPoint(1));
        }

        /// <summary>
        /// Return the normal of a Line in the XY plane.
        /// </summary>
        public static XYZ Normal(Line line)
        {
            XYZ p = line.GetEndPoint(0);
            XYZ q = line.GetEndPoint(1);
            XYZ v = q - p;

            //Debug.Assert( IsZero( v.Z ), 
            //  "expected horizontal line" );

            return v.CrossProduct(XYZ.BasisZ).Normalize();
        }

        /// <summary>
        /// Calculate a new point a certain distance and angle from 
        /// an existing point in the XY plane.
        /// </summary>
        /// <param name="p">Existing point</param>
        /// <param name="angle">Angle</param>
        /// <param name="dist">Distance</param>
        /// <returns>New point</returns>
        public static XYZ Polar(XYZ p, double angle, double dist)
        {
            double dx = 0.0, dy = 0.0;
            dx = Math.Cos(angle) * dist;
            dy = Math.Sin(angle) * dist;
            return (new XYZ(p.X + dx, p.Y + dy, p.Z));
        }

        /// <summary>
        /// Calculates the angle between 2 points in the XY plane, relative to the X axis
        /// </summary>
        /// <param name="p">Point 1</param>
        /// <param name="q">point 2</param>
        /// <returns>Angle in radians</returns>
        public static double Angle(XYZ p, XYZ q)
        {
            XYZ v = q - p;
            return (XYZ.BasisX.AngleOnPlaneTo(v, XYZ.BasisZ));
        }
        #endregion // Geometrical Comparison

        #region Unit Handling
        /// <summary>
        /// Base units currently used internally by Revit.
        /// </summary>
        enum BaseUnit
        {
            BU_Length = 0,         // length, feet (ft)
            BU_Angle,              // angle, radian (rad)
            BU_Mass,               // mass, kilogram (kg)
            BU_Time,               // time, second (s)
            BU_Electric_Current,   // electric current, ampere (A)
            BU_Temperature,        // temperature, kelvin (K)
            BU_Luminous_Intensity, // luminous intensity, candela (cd)
            BU_Solid_Angle,        // solid angle, steradian (sr)

            NumBaseUnits
        };

        const double _convertFeetToMm = 12 * 25.4;

        const double _convertFeetToMetre = _convertFeetToMm * 0.001;

        const double _convertSqrFeetToSqrMetre
            = _convertFeetToMetre
            * _convertFeetToMetre;

        const double _convertCubicFeetToCubicMetre
            = _convertFeetToMetre
            * _convertFeetToMetre
            * _convertFeetToMetre;

        /// <summary>
        /// Convert a given length in feet to millimetres.
        /// </summary>
        public static double FeetToMm(double length)
        {
            return length * _convertFeetToMm;
        }

        /// <summary>
        /// Convert a given length in feet to metres.
        /// </summary>
        public static double FeetToMetres(double length)
        {
            return length * _convertFeetToMetre;
        }

        /// <summary>
        /// Convert a given length in millimetres to feet.
        /// </summary>
        public static double MmToFeet(double length)
        {
            return length / _convertFeetToMm;
        }

        /// <summary>
        /// Convert a given length in metres to feet.
        /// </summary>
        public static double MetresToFeet(double length)
        {
            return length / _convertFeetToMetre;
        }

        /// <summary>
        /// Convert a given area in sqr feet to sqr metres.
        /// </summary>
        public static double SqrFeetToSqrMetres(double area)
        {
            return area * _convertSqrFeetToSqrMetre;
        }

        /// <summary>
        /// Convert a given volume in feet to cubic metres.
        /// </summary>
        public static double CubicFeetToCubicMetres(double volume)
        {
            return volume * _convertCubicFeetToCubicMetre;
        }
        #endregion // Unit Handling

        #region Display a message
        const string _caption = "3DIR - ";

        public static void InfoMsg(string msg)
        {
            Debug.WriteLine(msg);
            TaskDialog.Show(_caption + "information", msg);
        }

        public static void WarningMsg(string msg)
        {
            Debug.WriteLine(msg);
            TaskDialog.Show(_caption + "warning", msg);
        }

        public static void ErrorMsg(string msg)
        {
            Debug.WriteLine(msg);
            TaskDialog.Show(_caption + "error", msg);
        }

        /// <summary>
        /// Show a task dialog
        /// </summary>
        /// <param name="sTitle">The title for the dialog</param>
        /// <param name="sMainInstruction">The large primary text that appears at the top of the task dialog</param>
        /// <param name="sMainContent">The smaller text that appears just below the main instruction, optional</param>
        /// <param name="sVerification">Verification text is used to label the verification checkbox, optional</param>
        /// <param name="sFooter">Footer text is used to link to help or other files, optional</param>
        /// <returns>true if verification checkbox was checked</returns>
        public static bool ShowTaskDialog(string sTitle, string sMainInstruction, string sMainContent, string sVerification, string sFooter)
        {
            bool bRet = false, bVerification = false;
            if ((null == sTitle) || (sTitle.Length == 0))
            {
                sTitle = _caption + "information";
            }
            TaskDialog taskDialog = new TaskDialog(sTitle);
            taskDialog.TitleAutoPrefix = false;
            taskDialog.MainInstruction = sMainInstruction;
            if ((null != sMainContent) && (sMainContent.Length > 0))
            {
                taskDialog.MainContent = sMainContent;
            }
            if ((null != sVerification) && (sVerification.Length > 0))
            {
                taskDialog.VerificationText = sVerification;
                bVerification = true;
            }
            if ((null != sFooter) && (sFooter.Length > 0))
            {
                taskDialog.FooterText = sFooter;
            }
            taskDialog.Show();
            if (bVerification)
            {
                bRet = taskDialog.WasVerificationChecked();
            }
            return (bRet);
        }

        public static string ElementDescription(Element e)
        {
            if (null == e)
            {
                return "<null>";
            }
            // for a wall, the element name equals the
            // wall type name, which is equivalent to the
            // family name ...
            FamilyInstance fi = e as FamilyInstance;
            string fn = (null == fi)
                ? string.Empty
                : fi.Symbol.Family.Name + " ";

            string cn = (null == e.Category)
                ? e.GetType().Name
                : e.Category.Name;

            return string.Format("{0} {1}<{2} {3}>",
                cn, fn, e.Id.IntegerValue, e.Name);
        }
        #endregion // Display a message

        #region Element filtering
        /// <summary>
        /// Return all elements of the requested class i.e. System.Type
        /// matching the given built-in category in the given document.
        /// </summary>
        public static FilteredElementCollector GetElementsOfType(
            Document doc,
            Type type,
            BuiltInCategory bic)
        {
            FilteredElementCollector collector
                = new FilteredElementCollector(doc);

            collector.OfCategory(bic);
            collector.OfClass(type);

            return collector;
        }
        #endregion // Element filtering

        #region Element access
        /// <summary>
        /// Get an element property from a built in parameter (as string)
        /// </summary>
        /// <param name="element">An instance of any element class</param>
        /// <param name="paraEnum">The property name</param>
        /// <returns>Property value as a string</returns>
        public static string GetProperty(Element element, BuiltInParameter paraEnum)
        {
            string propertyValue = string.Empty;  //the value of parameter 

            // get the parameter via the parameterId
            Parameter param = element.get_Parameter(paraEnum);
            if (null != param)
            {
                // get the parameter's storage type
                StorageType storageType = param.StorageType;
                switch (storageType)
                {
                    case StorageType.Integer:
                        int iVal = param.AsInteger();
                        propertyValue = iVal.ToString();
                        break;
                    case StorageType.String:
                        propertyValue = param.AsString();
                        break;
                    case StorageType.Double:
                        Double dVal = param.AsDouble();
                        dVal = Math.Round(dVal, 2);
                        propertyValue = dVal.ToString();
                        break;
                    default:
                        break;
                }
            }
            return (propertyValue);
        }

        /// <summary>
        /// Get an element property from a built in parameter (as ElementId)
        /// </summary>
        /// <param name="element">An instance of any element class</param>
        /// <param name="paraEnum">The property name</param>
        /// <returns>Property value as an ElementId</returns>
        public static ElementId GetIdProperty(Element element, BuiltInParameter paraEnum)
        {
            ElementId propertyValue = ElementId.InvalidElementId;  //the value of parameter 

            // get the parameter via the parameterId
            Parameter param = element.get_Parameter(paraEnum);
            if (null != param)
            {
                // get the parameter's storage type
                StorageType storageType = param.StorageType;
                switch (storageType)
                {
                    case StorageType.ElementId:
                        propertyValue = param.AsElementId();
                        break;
                }
            }
            return (propertyValue);
        }

        #endregion // Element access

        #region Application
        /// <summary>
        /// Gets the path where this module is located, includes backslash on end
        /// </summary>
        internal static string AppPath()
        {
            // find path to this application
            string sPath = Assembly.GetExecutingAssembly().Location;
            // trim filename from end
            sPath = System.IO.Path.GetDirectoryName(sPath);
            return (sPath + "\\");
        }

        /// <summary>
        /// Gets the version of this module
        /// </summary>
        internal static Version AppVer()
        {
            return (Assembly.GetExecutingAssembly().GetName().Version);
        }

        /// <summary>
        /// Load a new icon bitmap from embedded resources.
        /// For the BitmapImage, make sure you reference 
        /// WindowsBase and PresentationCore. 
        /// </summary>
        internal static BitmapImage NewBitmapFromResource(Assembly assembly, string sNamespacePrefix, string sImageName)
        {
            Stream s = assembly.GetManifestResourceStream(sNamespacePrefix + sImageName);
            BitmapImage img = new BitmapImage();
            img.BeginInit();
            img.CacheOption = BitmapCacheOption.OnLoad;
            img.UriSource = null;
            img.StreamSource = s;
            img.EndInit();
            return img;
        }

        /// <summary>
        /// Load a new icon bitmap from embedded resources.
        /// </summary>
        internal static Bitmap NewImageFromResource(Assembly assembly, string sNamespacePrefix, string sImageName)
        {
            Stream s = assembly.GetManifestResourceStream(sNamespacePrefix + sImageName);
            Bitmap img = new Bitmap(s);
            return img;
        }

        #endregion // Application

        /// <summary>
        /// Replaces all invalid file name chars in the given string
        /// </summary>
        /// <param name="sFileName">The filename to check</param>
        /// <param name="cNew">The new char to use instead of any invalid chars found</param>
        internal static void ReplaceInvalidChars(ref string sFileName, char cNew)
        {
            foreach (char c in Path.GetInvalidFileNameChars())
            {
                sFileName.Replace(c, cNew);
            }
        }
    }
}