using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Tesla.ProtocolEditorControls
{
    public partial class ProtocolTextbox : TextBox
    {
        //protected const string LEGAL_PROTOCOL_LABEL_DESC_AUTHOR = @"^[a-zA-Z0-9_ `~!@#$%*^()\-=_+\[\]{},.:;|?/\\\b\x01\x03\x16<>&'\042]*$"; //\042 is double quotes
        protected const string LEGAL_PROTOCOL_LABEL_DESC_AUTHOR = @"^[a-zA-Z0-9_ `~!@#$%*^()\-=_+\[\]{},.:;|?/\\\b\x01\x03\x16<>&']*$";
        protected const string LEGAL_CMD_LABEL = @"^[a-zA-Z0-9_ `~!@#$%*^()\-=_+\[\]{},.:;|?/\\\b\x01\x03\x16]*$";
        protected string illegalCharacters = "[\"]";// @"[`~!@#$%^&*\[\]{}/<>?;':=\042]";
        //protected string legalCharacters = @"^[a-zA-Z0-9_ `~!@#$%*^()-=_+\[\]{},.:;|?/]*$"; //‘, &, <, >
        protected string legalCharacters = LEGAL_PROTOCOL_LABEL_DESC_AUTHOR; //‘, &, <, >
        public ProtocolTextbox()
        {
            InitializeComponent();
        }
    }

    public class ProtocolLabelDecriptionAndAuthorTextbox : ProtocolTextbox
    {
        public ProtocolLabelDecriptionAndAuthorTextbox()
        {
            legalCharacters = LEGAL_PROTOCOL_LABEL_DESC_AUTHOR;
        }
    }
    public class ProtocolCommandLabel : ProtocolTextbox
    {
        public ProtocolCommandLabel()
        {
            legalCharacters = LEGAL_CMD_LABEL;
        }
    }
}
