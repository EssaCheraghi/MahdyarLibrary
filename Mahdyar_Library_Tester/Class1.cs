using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Documents;

namespace Mahdyar_Library_Tester
{
   public static class Class1
    {
        public static string Ext_GetText(this RichTextBox Rt)
        {
            return new TextRange(Rt.Document.ContentStart, Rt.Document.ContentEnd).Text;
        }
        public static void Ext_SetText(this RichTextBox Rt, string Text)
        {
            new TextRange(Rt.Document.ContentStart, Rt.Document.ContentEnd).Text = Text;
        }
    }
}
