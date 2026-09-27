using System;
using System.Collections.Generic;
using System.Linq;
using AssetsTools.NET;
using AssetsTools.NET.Extra;
using Newtonsoft.Json;

namespace UABEANext4.Logic.Modding
{
    public class VrcaDataExporter
    {
        private readonly AssetsManager _manager;
        private readonly AssetsFileInstance _file;

        public VrcaDataExporter(AssetsManager manager, AssetsFileInstance file)
        {
            _manager = manager;
            _file = file;
        }

        public string Export()
        {
            var dumper = new VrcaDumper(_manager, _file);
            var dump = dumper.Dump();
            return JsonConvert.SerializeObject(dump, Formatting.Indented);
        }
    }
}
