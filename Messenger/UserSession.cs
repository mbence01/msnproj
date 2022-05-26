using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Messenger
{
    static class UserSession
    {
        public static User LoggedInUser = null;
        public static Dictionary<string, object> SessionVars = new Dictionary<string, object>();
        public static List<Form> OpenedWindows = new List<Form>();
    }
}
