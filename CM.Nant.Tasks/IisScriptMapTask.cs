using System;
using System.DirectoryServices;
using NAnt.Core;
using NAnt.Core.Attributes;

namespace CM.Nant.Tasks
{
    [TaskName("iis-script-map")]
    public class IisScriptMapTask : Task
    {
        private string extension;

        [TaskAttribute("server")]
        public string Server { get; set; }

        [TaskAttribute("virtualDirectory")]
        public string VirtualDirectory { get; set; }

        [TaskAttribute("executable")]
        public string Executable { get; set; }

        [TaskAttribute("scriptEngine")]
        public bool ScriptEngine { get; set; }

        [TaskAttribute("fileMustExist")]
        public bool FileMustExist { get; set; }

        [TaskAttribute("verbs")]
        public string Verbs { get; set; }

        [TaskAttribute("extension")]
        public string Extension
        {
            get
            {
                if (extension == "*")
                    return extension;
                return extension.StartsWith(".") ? extension : "." + extension;
            }
            set { extension = value; }
        }

        private int Flags
        {
            get
            {
                var flags = 0;
                if (ScriptEngine) flags |= 1;
                if (FileMustExist) flags |= 4;
                return flags;
            }
        }

        public string PropertyValueText
        {
            get
            {
                return string.Format("{0},{1},{2},{3}", Extension, Executable, Flags, Verbs).TrimEnd(',');
            }
        }

        public bool Matches(string propertyValueText)
        {
            return propertyValueText.StartsWith(Extension + ",", StringComparison.InvariantCultureIgnoreCase);
        }

        private DirectoryEntry GetVirtualDirectory()
        {
            var root = new DirectoryEntry(string.Format("IIS://{0}/W3SVC/1/Root", Server));
            foreach (DirectoryEntry site in root.Children)
            {
                if (site.Name == VirtualDirectory && site.SchemaClassName == "IIsWebVirtualDir")
                    return site;
            }

            throw new InvalidOperationException("the virtual directory does not exist!");
        }

        protected override void ExecuteTask()
        {
            var virtualDirectory = GetVirtualDirectory();
            var scriptMaps = virtualDirectory.Properties["ScriptMaps"];
            var scriptMapIndex = FindMatchingScriptMapIndex(scriptMaps);
            if (scriptMapIndex < 0)
            {
                scriptMaps.Add(PropertyValueText);
            }
            else
            {
                scriptMaps[scriptMapIndex] = PropertyValueText;
            }
            virtualDirectory.CommitChanges();
        }

        private int FindMatchingScriptMapIndex(PropertyValueCollection scriptMaps)
        {
            for (var i = 0; i < scriptMaps.Count; i++)
            {
                var scriptMap = scriptMaps[i] as string;
                if (scriptMap == null) continue;

                if (Matches(scriptMap))
                {
                    return i;
                }
            }
            return -1;
        }
    }
}