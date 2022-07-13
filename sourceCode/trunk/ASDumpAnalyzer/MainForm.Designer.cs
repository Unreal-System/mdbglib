
namespace ASDumpAnalyzer
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.myConsole = new System.Windows.Forms.RichTextBox();
            this.buttonPrintStack = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.textBoxDumpPath = new System.Windows.Forms.TextBox();
            this.textBoxSymbols = new System.Windows.Forms.TextBox();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openCrashDumpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.symbolFilePathToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.buttonBrowseSymbols = new System.Windows.Forms.Button();
            this.buttonReload = new System.Windows.Forms.Button();
            this.stackProgressBar2 = new ASDumpAnalyzer.StackProgressBar();
            this.stackProgressBar1 = new ASDumpAnalyzer.StackProgressBar();
            this.buttonBrowseDump = new System.Windows.Forms.Button();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // myConsole
            // 
            this.myConsole.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.myConsole.Location = new System.Drawing.Point(94, 79);
            this.myConsole.Name = "myConsole";
            this.myConsole.Size = new System.Drawing.Size(708, 567);
            this.myConsole.TabIndex = 0;
            this.myConsole.Text = "";
            // 
            // buttonPrintStack
            // 
            this.buttonPrintStack.Location = new System.Drawing.Point(15, 79);
            this.buttonPrintStack.Name = "buttonPrintStack";
            this.buttonPrintStack.Size = new System.Drawing.Size(73, 23);
            this.buttonPrintStack.TabIndex = 2;
            this.buttonPrintStack.Text = "Get Queries";
            this.buttonPrintStack.UseVisualStyleBackColor = true;
            this.buttonPrintStack.Click += new System.EventHandler(this.buttonPrintStack_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 30);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Dump";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 56);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(46, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Symbols";
            // 
            // textBoxDumpPath
            // 
            this.textBoxDumpPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxDumpPath.Location = new System.Drawing.Point(94, 27);
            this.textBoxDumpPath.Name = "textBoxDumpPath";
            this.textBoxDumpPath.ReadOnly = true;
            this.textBoxDumpPath.Size = new System.Drawing.Size(640, 20);
            this.textBoxDumpPath.TabIndex = 5;
            // 
            // textBoxSymbols
            // 
            this.textBoxSymbols.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxSymbols.Location = new System.Drawing.Point(94, 53);
            this.textBoxSymbols.Name = "textBoxSymbols";
            this.textBoxSymbols.Size = new System.Drawing.Size(573, 20);
            this.textBoxSymbols.TabIndex = 6;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(867, 24);
            this.menuStrip1.TabIndex = 7;
            this.menuStrip1.Text = "menuStrip1";
            this.menuStrip1.Visible = false;
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openCrashDumpToolStripMenuItem,
            this.symbolFilePathToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(35, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // openCrashDumpToolStripMenuItem
            // 
            this.openCrashDumpToolStripMenuItem.Name = "openCrashDumpToolStripMenuItem";
            this.openCrashDumpToolStripMenuItem.ShortcutKeyDisplayString = "Ctrl+D";
            this.openCrashDumpToolStripMenuItem.Size = new System.Drawing.Size(212, 22);
            this.openCrashDumpToolStripMenuItem.Text = "Open Crash &Dump...";
            this.openCrashDumpToolStripMenuItem.Click += new System.EventHandler(this.openCrashDumpToolStripMenuItem_Click);
            // 
            // symbolFilePathToolStripMenuItem
            // 
            this.symbolFilePathToolStripMenuItem.Name = "symbolFilePathToolStripMenuItem";
            this.symbolFilePathToolStripMenuItem.ShortcutKeyDisplayString = "Ctrl+S";
            this.symbolFilePathToolStripMenuItem.Size = new System.Drawing.Size(212, 22);
            this.symbolFilePathToolStripMenuItem.Text = "Symbol File Path...";
            // 
            // statusStrip1
            // 
            this.statusStrip1.Location = new System.Drawing.Point(0, 649);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(867, 22);
            this.statusStrip1.TabIndex = 8;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // buttonBrowseSymbols
            // 
            this.buttonBrowseSymbols.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonBrowseSymbols.Location = new System.Drawing.Point(740, 51);
            this.buttonBrowseSymbols.Name = "buttonBrowseSymbols";
            this.buttonBrowseSymbols.Size = new System.Drawing.Size(62, 23);
            this.buttonBrowseSymbols.TabIndex = 9;
            this.buttonBrowseSymbols.Text = "Browse";
            this.buttonBrowseSymbols.UseVisualStyleBackColor = true;
            this.buttonBrowseSymbols.Click += new System.EventHandler(this.buttonBrowse_Click);
            // 
            // buttonReload
            // 
            this.buttonReload.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonReload.Location = new System.Drawing.Point(673, 51);
            this.buttonReload.Name = "buttonReload";
            this.buttonReload.Size = new System.Drawing.Size(61, 23);
            this.buttonReload.TabIndex = 10;
            this.buttonReload.Text = "Reload";
            this.buttonReload.UseVisualStyleBackColor = true;
            this.buttonReload.Click += new System.EventHandler(this.buttonReload_Click);
            // 
            // stackProgressBar2
            // 
            this.stackProgressBar2.ActiveFrame = 0;
            this.stackProgressBar2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.stackProgressBar2.FrameCount = 0;
            this.stackProgressBar2.FrameGap = 1;
            this.stackProgressBar2.Location = new System.Drawing.Point(808, 27);
            this.stackProgressBar2.Name = "stackProgressBar2";
            this.stackProgressBar2.Progress = 0D;
            this.stackProgressBar2.Size = new System.Drawing.Size(21, 619);
            this.stackProgressBar2.TabIndex = 12;
            // 
            // stackProgressBar1
            // 
            this.stackProgressBar1.ActiveFrame = 0;
            this.stackProgressBar1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.stackProgressBar1.FrameCount = 0;
            this.stackProgressBar1.FrameGap = 1;
            this.stackProgressBar1.Location = new System.Drawing.Point(835, 27);
            this.stackProgressBar1.Name = "stackProgressBar1";
            this.stackProgressBar1.Progress = 0D;
            this.stackProgressBar1.Size = new System.Drawing.Size(20, 619);
            this.stackProgressBar1.TabIndex = 11;
            // 
            // buttonBrowseDump
            // 
            this.buttonBrowseDump.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonBrowseDump.Location = new System.Drawing.Point(740, 25);
            this.buttonBrowseDump.Name = "buttonBrowseDump";
            this.buttonBrowseDump.Size = new System.Drawing.Size(62, 23);
            this.buttonBrowseDump.TabIndex = 13;
            this.buttonBrowseDump.Text = "Browse";
            this.buttonBrowseDump.UseVisualStyleBackColor = true;
            this.buttonBrowseDump.Click += new System.EventHandler(this.buttonBrowseDump_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(867, 671);
            this.Controls.Add(this.buttonBrowseDump);
            this.Controls.Add(this.stackProgressBar2);
            this.Controls.Add(this.stackProgressBar1);
            this.Controls.Add(this.buttonReload);
            this.Controls.Add(this.buttonBrowseSymbols);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.textBoxSymbols);
            this.Controls.Add(this.textBoxDumpPath);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.buttonPrintStack);
            this.Controls.Add(this.myConsole);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "MainForm";
            this.Text = "ASDumpAnalyzer";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing_1);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RichTextBox myConsole;
        private System.Windows.Forms.Button buttonPrintStack;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBoxDumpPath;
        private System.Windows.Forms.TextBox textBoxSymbols;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripMenuItem openCrashDumpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem symbolFilePathToolStripMenuItem;
        private System.Windows.Forms.Button buttonBrowseSymbols;
        private System.Windows.Forms.Button buttonReload;
        private StackProgressBar stackProgressBar1;
        private StackProgressBar stackProgressBar2;
        private System.Windows.Forms.Button buttonBrowseDump;
    }
}

