using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TestPowerBI
{
    public class TextBoxWriter : TextWriter
    {
        // The control where we will write text.
        private TextBox MyControl;
        private readonly SynchronizationContext synchronizationContext;
        public TextBoxWriter(TextBox control)
        {
            MyControl = control;
            synchronizationContext = SynchronizationContext.Current;
        }

        public override void Write(char value)
        {
            var deferredWrite = new SendOrPostCallback(o =>
            {
                MyControl.AppendText( o.ToString() );
            } );

            Task.Run(() => {
               synchronizationContext.Send(deferredWrite, value);
            });
        }

        public override void Write(string value)
        {
            var deferredWrite = new SendOrPostCallback(o =>
            {
                MyControl.AppendText(o.ToString());
            });

            Task.Run(() => {
                synchronizationContext.Send(deferredWrite, value);
            });
        }

        public override void WriteLine(string s)
        {
            var deferredWrite = new SendOrPostCallback(o =>
            {
                MyControl.AppendText(o.ToString());
            });

            Task.Run(() => {
                synchronizationContext.Send(deferredWrite, s + "\r\n" );
            });
        }

        public override Encoding Encoding {
            get { return Encoding.Unicode; }
        }
    }
}
