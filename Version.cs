using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Convert;

namespace WPFClinica.Scripts {

    class Version {

        public List<int> version;

        public Version (string a) {
            version = Array.ConvertAll(a.Split('.'), x => ToInt32(x)).ToList();
        }

        /// <summary>
        /// Compares to versions to find out which one is greater.
        /// <para>0, if equal. -1, if compared version is greater. 1 otherwise</para>
        /// </summary>
        /// <param Funcionarios.f.TableName="v"></param>
        /// <returns></returns>
        public int Compare (Version v) {
            while (true) {
                if (version.Count == v.version.Count) {
                    break;
                } else if (version.Count > v.version.Count) {
                    v.version.Add(0);
                } else {
                    version.Add(0);
                }
            }
            for (int i = 0; i < version.Count; i++) {
                var current = version[i];
                var another = v.version[i];
                if (current > another) {
                    return 1;
                } else if (current < another) {
                    return -1;
                }
            }
            return 0;
        }

        public override string ToString() {
            return String.Join(".", version);
        }

    }
}
