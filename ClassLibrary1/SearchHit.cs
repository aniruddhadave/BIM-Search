using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ClassLibrary1
{
    /// <summary>
    /// The type of hit
    /// </summary>
    public enum HitType
    {
        Parameter = 0,
        Txt,
        Pdf,
        Excel,
        Word,
        Other,

        NumberOfHitTypes
    }

    /// <summary>
    /// Manage data for a string search hit.
    /// </summary>
    public class SearchHit
    {
        

        /// <summary>
        /// ID
        /// </summary>
        public string Name { get; set; }

       
        /// <summary>
        /// Element's unique id.
        /// </summary>
        public string UniqueId { get; set; }

        /// <summary>
        /// The type of hit
        /// </summary>
        public double Volume_CF { get; set; }

        /// <summary>
        /// Linked document path
        /// </summary>
        public double Cost_Rs { get; set; }
        /// <summary>
        /// Linked coordinates
        /// </summary>
        public string Coordinates_Ft { get; set; }
        /// <summary>
        /// Linked Levels
        /// </summary>
        public string Length_Ft { get; set; }
         
        public SearchHit(string sname,string sUniqueId,double sVolume,double sCost, string sCoordinates, string sLength)
        {
            Name = sname;
            UniqueId = sUniqueId;
            Volume_CF = sVolume;
            Cost_Rs = sCost;
            Coordinates_Ft = sCoordinates;
            Length_Ft = sLength;
        }

       /* public override string ToString()
        {
           // return string.Format("Category {0} parameter '{1}' on element with unique id {2}, type {3}",
               // Category, ParameterName, UniqueId, HitType);
        }*/
    }
}
