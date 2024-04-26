using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kelimecim.Utilities
{
    public static class PathDB
    {
        public static string GetPath(string nameDb)
        {
            string pathDbSqlite = string.Empty;
            if (DeviceInfo.Platform == DevicePlatform.Android)
            {
                pathDbSqlite = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), nameDb);
            }
            else if (DeviceInfo.Platform == DevicePlatform.iOS)
            {
                pathDbSqlite = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "..", "Library", nameDb);
            }
            else if (DeviceInfo.Platform == DevicePlatform.WinUI)
            {
                pathDbSqlite = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), nameDb);
            }
            return pathDbSqlite;
        }
    }


}
