using Grasshopper.Kernel;
using System;
using System.Drawing;

namespace WFC
{
    public class WFCInfo : GH_AssemblyInfo
    {
        public override string Name
        {
            get
            {
                return "WFC";
            }
        }
        public override Bitmap Icon
        {
            get
            {
                //Return a 24x24 pixel bitmap to represent this GHA library.
                return null;
            }
        }
        public override string Description
        {
            get
            {
                //Return a short string describing the purpose of this GHA library.
                return "Wave function collapse modelling tools.";
            }
        }
        public override Guid Id
        {
            get
            {
                return new Guid("dca00d70-3656-492b-8742-698326c6bf77");
            }
        }

        public override string AuthorName
        {
            get
            {
                //Return a string identifying you or your company.
                return "Schmidt Hammer Lassen Digital Practice";
            }
        }
        public override string AuthorContact
        {
            get
            {
                //Return a string representing your preferred contact details.
                return "rhu@shl.dk";
            }
        }
    }
}
