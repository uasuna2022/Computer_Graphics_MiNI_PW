using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project3_CustomFunctionImageFilter
{
    public sealed class EditorWorkspace
    {
        private static EditorWorkspace? _instance;
        public static EditorWorkspace Instance => _instance ??= new EditorWorkspace();
        private EditorWorkspace() { }
        public float Gamma { get; set; } = 1.0F; // [0.1; 5.0]
        public int Brightness { get; set; } = 0; // [-230; 230]
        public int Contrast { get; set; } = 0;   // [-127; 127]

        public bool NoFilter { get; set; } = true;
        public bool NegationFilter { get; set; } = false;
        public bool CustomFunctionFilter { get; set; } = false;
        public bool GammaFilter { get; set; } = false;
        public bool BrightnessFilter { get; set; } = false;
        public bool ContrastFilter { get; set; } = false;


    }
}
