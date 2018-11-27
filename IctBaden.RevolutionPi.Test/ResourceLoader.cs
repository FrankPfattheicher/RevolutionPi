using System.IO;
using System.Reflection;
using System.Text;

namespace IctBaden.RevolutionPi.Test
{
    public class ResourceLoader
    {
        public static string LoadAsString(Assembly assembly, string resourceName)
        {
            using (var manifestResourceStream = assembly.GetManifestResourceStream(resourceName))
            {
                if (manifestResourceStream == null)
                    return null;
                using (var streamReader = new StreamReader(manifestResourceStream, Encoding.Default))
                    return streamReader.ReadToEnd();
            }
        }
    }
}
