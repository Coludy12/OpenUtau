﻿using System.IO;
using OpenUtau.Core;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace OpenUtau.Classic {
    public enum SymbolSetPreset { unknown, hiragana, arpabet }

    public class SymbolSet {
        public SymbolSetPreset Preset { get; set; }
        public string Head { get; set; } = "-";
        public string Tail { get; set; } = "R";
    }

    /// <summary>
    /// A subbank of a voicebank. A valid subbank should be:
    ///   1. All its sounds are contained in a single oto.ini.
    ///   2. All its sounds use the same prefix and suffix.
    /// If a directory does not meet these conditions, do not list it as a subbank.
    /// </summary>
    public class Subbank {
        /// <summary>
        /// Subbank directory relative to character.txt. Leav unspecified if not in a subfolder.
        /// </summary>
        public string Dir { get; set; }

        /// <summary>
        /// Subbank prefix. Leave unspecified if none.
        /// </summary>
        public string Prefix { get; set; }

        /// <summary>
        /// Subbank suffix. Leave unspecified if none.
        /// </summary>
        public string Suffix { get; set; }

        /// <summary>
        /// Flavor of subbank, e.g., "power", "whisper". Leave unspecified for the main bank.
        /// </summary>
        public string Flavor { get; set; }

        /// <summary>
        /// Start of tone range in prefix.map. Required.
        /// </summary>
        public string ToneStart { get; set; }

        /// <summary>
        /// End of tone range in prefix.map. Required.
        /// </summary>
        public string ToneEnd { get; set; }
    }

    public class VoicebankConfig {
        public SymbolSet SymbolSet { get; set; }
        public Subbank[] Subbanks { get; set; }

        public void Save(Stream stream) {
            using (var writer = new StreamWriter(stream)) {
                Yaml.DefaultSerializer.Serialize(writer, this);
            }
        }

        public static VoicebankConfig Load(Stream stream) {
            using (var reader = new StreamReader(stream)) {
                return Yaml.DefaultDeserializer.Deserialize<VoicebankConfig>(reader);
            }
        }
    }
}
