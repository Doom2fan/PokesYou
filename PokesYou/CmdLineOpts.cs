using CommandLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommandLine.Text;

namespace PokesYou {
    public class CmdLineOpts {
        [OptionList ("file", MetaValue = "FILE", HelpText = "Load a file as a patch/mod.")]
        public List<string> PatchFiles { get; set; }

        [ParserState]
        public IParserState LastParserState { get; set; }

        [HelpOption]
        public string GetUsage () {
            return HelpText.AutoBuild (this, current => HelpText.DefaultParsingErrorsHandler (this, current));
        }
    }
}
