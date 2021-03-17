using System;
using System.Collections.Generic;
using System.Text;

namespace WhiteLabel.ConvertToYeoman.Helpers
{
    public static class ArgValidatorHelper
    {
        public static string Check(string[] args)
        {
            var res = string.Empty;

            if (args.Length != 1)
            {
                return "Incorrect parameters. Parameter1 = Destination directory where the solution will be copied with the Yeoman variables applied.";
            }

            if (!System.IO.Directory.Exists(args[0]))
            {
                return "The specified directory doesn't exist.";
            }

            //if (System.IO.Directory.GetFiles(args[0]).Length > 0)
            //{
            //    return "The specified directory is not empty";
            //}


            return res;
        }
    }
}
