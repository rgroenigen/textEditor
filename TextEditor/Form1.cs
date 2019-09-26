using System;
using System.Drawing;
using System.Drawing.Printing;
using System.Windows.Forms;
using System.IO;

namespace TextEditor
{
    public partial class Form1 : Form
    {

        OpenFileDialog file_open = new OpenFileDialog();
        public string filename = "new.rtf";
        
        public Form1()
        {
            InitializeComponent();
            this.Text = "TextEditor+ new.rtf";
            
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult dr = MessageBox.Show("Are you sure? You will loose any unsaved work!",
                                  "WARNING!", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
            switch (dr)
            {
                case DialogResult.OK:
                    Close();
                    break;
                case DialogResult.Cancel:
                    break;
            }
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // To filter files from OpenFileDialog
            file_open.Filter = "Rich Text File (*.rtf)|*.rtf| Plain Text File (*.txt)|*.txt";
            file_open.FilterIndex = 1;
            file_open.Title = "Open text or RTF file";

            RichTextBoxStreamType stream_type;
            stream_type = RichTextBoxStreamType.RichText;
            if (DialogResult.OK == file_open.ShowDialog())
            {
                if (string.IsNullOrEmpty(file_open.FileName))
                    return;
                if (file_open.FilterIndex == 2)
                    stream_type = RichTextBoxStreamType.PlainText;
                richTextBox1.LoadFile(file_open.FileName, stream_type);
            }
            this.Text = "TextEditor+ " + file_open.FileName;
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveDlg = new SaveFileDialog();
            string filename = "new.rtf";
            
            this.Text = "TextEditor+ " + filename;
            // To filter files from SaveFileDialog
            saveDlg.Filter = "Rich Text File (*.rtf)|*.rtf|Plain Text File (*.txt)|*.txt";
            saveDlg.DefaultExt = "*.rtf";
            saveDlg.FilterIndex = 1;
            saveDlg.Title = "Save the contents";

            DialogResult retval = saveDlg.ShowDialog();
            if (retval == DialogResult.OK)
                filename = saveDlg.FileName;
            else
                return;

            RichTextBoxStreamType stream_type;
            if (saveDlg.FilterIndex == 2)
                stream_type = RichTextBoxStreamType.PlainText;
            else
                stream_type = RichTextBoxStreamType.RichText;

            richTextBox1.SaveFile(filename, stream_type);
            MessageBox.Show("File Saved", "Saved", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveDlg = new SaveFileDialog();
            string filename = "";

            // To filter files from SaveFileDialog
            saveDlg.Filter = "Rich Text File (*.rtf)|*.rtf|Plain Text File (*.txt)|*.txt";
            saveDlg.DefaultExt = "*.rtf";
            saveDlg.FilterIndex = 1;
            saveDlg.Title = "Save the contents";

            filename = file_open.FileName;

            RichTextBoxStreamType stream_type;
            // Checks the extension of the file to save
            if (filename.Contains(".txt"))
                stream_type = RichTextBoxStreamType.PlainText;
            else
                stream_type = RichTextBoxStreamType.RichText;

            richTextBox1.SaveFile(filename, stream_type);
            MessageBox.Show("File Saved", "Saved", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult dr = MessageBox.Show("Are you sure? You will loose any unsaved work!",
                                  "WARNING!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            switch (dr)
            {
                case DialogResult.Yes:
                    richTextBox1.Text = "";
                    this.Text = "TextEditor+ new.rtf";
                    break;
                case DialogResult.No:
                    break;
            }
        }

        private void aboutTextEditorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult dr = MessageBox.Show("TextEditor + v0.1\n\nby René van Groenigen\n(c) 2018\nContact: rene@groenigen.nl",
                                  "About!", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void chaneBackgroundColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ColorDialog colorDialog1 = new ColorDialog();

            // Set the initial color of the dialog to the current text color.
            colorDialog1.Color = richTextBox1.SelectionColor;

            // Determine if the user clicked OK in the dialog and that the color has changed.
            if (colorDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK &&
               colorDialog1.Color != richTextBox1.SelectionColor)
            {
                // Change the selection color to the user specified color.
                richTextBox1.BackColor = colorDialog1.Color;
            }


        }

        private void changeTextColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ColorDialog colorDialog1 = new ColorDialog();

            // Set the initial color of the dialog to the current text color.
            colorDialog1.Color = richTextBox1.SelectionColor;

            // Determine if the user clicked OK in the dialog and that the color has changed.
            if (colorDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK &&
               colorDialog1.Color != richTextBox1.SelectionColor)
            {
                // Change the selection color to the user specified color.
                richTextBox1.SelectionColor = colorDialog1.Color;
            }
        }

        private void printToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PrintDialog printDialog = new PrintDialog();
            PrintDocument documentToPrint = new PrintDocument();
            printDialog.Document = documentToPrint;

            if (printDialog.ShowDialog() == DialogResult.OK)
            {
                StringReader reader = new StringReader(richTextBox1.Text);
                documentToPrint.PrintPage += new PrintPageEventHandler(DocumentToPrint_PrintPage);
                documentToPrint.Print();
            }

        }
        private void DocumentToPrint_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            StringReader reader = new StringReader(richTextBox1.Text);
            float LinesPerPage = 0;
            float YPosition = 0;
            int Count = 0;
            float LeftMargin = e.MarginBounds.Left;
            float TopMargin = e.MarginBounds.Top;
            string Line = null;
            Font PrintFont = this.richTextBox1.Font;
            SolidBrush PrintBrush = new SolidBrush(Color.Black);

            LinesPerPage = e.MarginBounds.Height / PrintFont.GetHeight(e.Graphics);

            while (Count < LinesPerPage && ((Line = reader.ReadLine()) != null))
            {
                YPosition = TopMargin + (Count * PrintFont.GetHeight(e.Graphics));
                e.Graphics.DrawString(Line, PrintFont, PrintBrush, LeftMargin, YPosition, new StringFormat());
                Count++;
            }

            if (Line != null)
            {
                e.HasMorePages = true;
            }
            else
            {
                e.HasMorePages = false;
            }
            PrintBrush.Dispose();
        }

        private void changeFontToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FontDialog fontDlg = new FontDialog();
            fontDlg.ShowColor = true;
            fontDlg.ShowApply = true;
            fontDlg.ShowEffects = true;
            fontDlg.ShowHelp = true;
            fontDlg.MaxSize = 40;
            fontDlg.MinSize = 6;
            if (fontDlg.ShowDialog() != DialogResult.Cancel)
            {
                richTextBox1.SelectionFont = fontDlg.Font;
                //richTextBox1.SelectionColor = fontDlg.Color;
            }
        }

        private void insertPictureToolStripMenuItem_Click(object sender, EventArgs e)
        {

            {
                OpenFileDialog OpenFile1 = new OpenFileDialog();
                if (OpenFile1.ShowDialog() == DialogResult.OK)
                {
                    // Show the Open File dialog. If the user clicks OK, load the 
                    // picture that the user chose.   
                    Image img = Image.FromFile(OpenFile1.FileName);
                    Clipboard.SetImage(img);
                    PictureBox pic = new PictureBox();
                    //pic.Load(OpenFile1.FileName)
                    //
                    // we need to resize the picture somhow
                    //
                    richTextBox1.Paste();
                    richTextBox1.Focus();
                }
                else
                {
                    richTextBox1.Focus();
                }

            }
        }

        private void cutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (richTextBox1.SelectionLength > 0)
                richTextBox1.Cut();
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (richTextBox1.SelectionLength > 0)
                richTextBox1.Copy();
        }

        private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Clipboard.GetDataObject().GetDataPresent(DataFormats.Text))
            {
                richTextBox1.Paste();
            }
        }

        private void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            richTextBox1.Undo();
        }

        private void redoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            richTextBox1.Redo();
        }
    }
}
