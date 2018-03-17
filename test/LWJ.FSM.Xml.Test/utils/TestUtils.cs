using System.Reflection;
using System.Text;

namespace LWJ.FSM.Xml.Test
{
    public static class TestUtils
    {
        public static string LoadText(string path)
        {
            var assembly = Assembly.GetExecutingAssembly();

            using (var rs = assembly.GetManifestResourceStream(assembly.GetName().Name + "." + path))
            using (var s = new System.IO.StreamReader(rs, Encoding.UTF8))
            {
                return s.ReadToEnd();
            }
        }

    }
}
