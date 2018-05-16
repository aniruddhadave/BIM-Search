using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary1
{
   internal sealed class Global
   {
       private static readonly Global instance = new Global();
       public static Global Instance
       {
           get { return instance; }
       }
       private Form1 _OurForm = null;
       internal Form1 OurForm
       {
           get { return _OurForm; }
           set { _OurForm = value; }
       }
       private Form2 _OurForm2 = null;
       internal Form2 OurForm2{
           get { return _OurForm2; }
           set { _OurForm2 = value; }
       }
       private IntPtr _ptrWindowHandle;
       internal IntPtr RevitWindowHandle
       {
           get { return _ptrWindowHandle; }
           set { _ptrWindowHandle = value; }
       }
    }
}
